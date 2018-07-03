 IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocAuthenticateUser')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE SprocAuthenticateUser
END

GO

CREATE PROCEDURE [dbo].[SprocAuthenticateUser]
(
	@pUsername nvarchar(100),
	@pPassword nvarchar(100),
	@pCurrentDateTime datetime,
	@pIPAddress nvarchar(20),
	@pLoginTypeId nvarchar(10)='1',
	@pIsEmail bit
)
As
Begin
	Declare @IsAuthenticated bit=0,@UserId bigint=0,@StatusId int=0,@ExecutedStatus bit=0,@CId bigint,@FId bigint,@FailedLoginAttempts int=0
	,@RolesCount int=0,@IsFirstTimeLoggedIn bit=0,@HasAccess bit=1,@NoAccess bit=0 

	Declare @CurrentRole nvarchar(100),@CurrentRoleId int,@CurrentRoleKey nvarchar(10)
	Declare @FacilityNumber nvarchar(20),@FName nvarchar(200),@FTimezone nvarchar(100),@DefaultCountryId bigint
	Declare @Tabs Table([TabId] [int],[TabName] [nvarchar](100),[Controller] [varchar](100),[Action] [varchar](100),[RouteValues] [varchar](500)
	,[TabOrder] [int],[TabImageUrl] [varchar](1000),[ParentTabId] [int],[IsActive] [bit],[IsVisible] [bit],[IsDeleted] [bit],[ScreenID] [int]
	,[CreatedBy] [int],[CreatedDate] [datetime],[ModifiedBy] [int],[ModifiedDate] [datetime],[DeletedBy] [int],[DeletedDate] [datetime]
	,[TabHierarchy] [nvarchar](500))


	--Check If Login Time is between the Configured Timing given in the System.
	IF NOT Exists (Select 1 From SystemConfiguration Where (CAST(@pCurrentDateTime as time) Between LoginStartTime and LoginEndTime)
				And IsActive=1 And ISNULL(IsDeleted,0)=0)
		SET @StatusId=-1	--Cannot Login in Odd-Times.
		

	IF @StatusId=0
	Begin
		IF @pLoginTypeId=1
		Begin
			Select @UserId=U.UserId,@CId=U.CorporateId,@FId=U.FacilityId From [Users] U 
			Where @pUsername=(Case @pIsEmail WHEN 1 THEN U.Email ELSE U.UserName END)
			And U.[Password]=@pPassword
			And ISNULL(U.IsDeleted,0)=0
			AND U.UserID IN (Select UR.UserID From UserRole UR Where ISNULL(UR.IsActive,1)=1 AND ISNULL(UR.IsDeleted,0)=0)

			IF ISNULL(@UserId,0)=0
				SET @StatusId=-3		--Cannot Login at Web
		End
		Else
			SET @StatusId=-2		--Cannot Login at Web


		If @StatusId=0
			Set @IsAuthenticated=1
		Else
		Begin
			IF Exists (Select 1 From Users Where UserID=@UserId And IsActive=1 And IsDeleted=0)
			Begin
				Update Users Set FailedLoginAttempts=FailedLoginAttempts+1 From Users Where UserID=@UserId

				Set @StatusId=-3	--Invalid Login

				If Exists (Select 1 From Users Where UserID=@UserId And IsActive=1 And IsDeleted=0 And FailedLoginAttempts=5)
				Begin
					INSERT INTO AuditLog (UserId,CreatedDate,TableName,FieldName,PrimaryKey,OldValue,NewValue,CorporateId,FacilityId,EventType)
					Select UserID,@pCurrentDateTime,'Users','Password_Disabled',0,0,1,CorporateId,FacilityId,'Added' From Users Where UserId=@UserId

					SET @StatusId=-4		--FailedAttempts Are Over
				End
			End
		End

		If @IsAuthenticated=1
		Begin
			--Save Login Tracking 
			Insert Into LoginTracking (ID,LoginTime,LogoutTime,IPAddress,LoginUserType,CreatedBy,CreatedDate,IsActive,FacilityId,CorporateId)
			Select @UserId,@pCurrentDateTime,NULL,@pIPAddress,@pLoginTypeId,@UserId,@pCurrentDateTime,1,@FId,@CId

			IF @StatusId=0
			Begin
				Select @RolesCount=Count(1) From UserRole Where UserId=@UserId And IsActive=1 And IsDeleted=0

				IF @RolesCount=1
				Begin
					Select TOP 1 @CurrentRoleId=R.RoleID,@CurrentRole=R.RoleName,@CurrentRoleKey=R.RoleKey From UserRole UR
					INNER JOIN [Role] R ON UR.RoleID=R.RoleID
					Where UR.UserId=@UserId And UR.IsActive=1 And UR.IsDeleted=0

					INSERT INTO @Tabs
					Exec SprocGetTabsList @CurrentRoleId,@UserId,@FId,@CId,0,1
				End

				--Update FailedLoginAttempts to ZERO.
				Update Users Set FailedLoginAttempts=0,ModifiedBy=@UserId,ModifiedDate=@pCurrentDateTime Where UserId=@UserId

				IF @FId > 0 AND @CId > 0 AND @StatusId=0
				Begin
					Select @FacilityNumber=FacilityNumber,@FName=FacilityName,@FTimezone=FacilityTimeZone From Facility Where FacilityId=@FId
					Select TOP 1 @DefaultCountryId=(Case WHEN ISNULL(DefaultCountry,199)=0 THEN 199 ELSE DefaultCountry END) 
					From BillingSystemParameters Where CorporateId=@CId
				End
			End
		End
		Else
		Begin
			Set @StatusId=-3	--Invalid Login
		End
	End

	Select @StatusId

	IF @StatusId=0
	Begin 
		Select U.UserID,U.CountryID,U.StateID,U.CityID,U.UserGroup,U.UserName,U.FirstName,U.LastName
		,[Name]=U.FirstName + ' ' + U.LastName,U.Answer,U.[Password],U.Email,U.[Address],U.Phone,U.HomePhone
		,AdminUser=ISNULL(U.AdminUser,0)
		,U.FailedLoginAttempts
		,FacilityNumber=@FacilityNumber,FacilityId=@FId,CorporateId=@CId,FacilityName=@FName
		,FacilityTimeZone=@FTimezone,DefaultCountryId=@DefaultCountryId
		,RoleName=@CurrentRole,RoleId=@CurrentRoleId,RoleKey=@CurrentRoleKey
		,StatusId=@StatusId
		,IsFirstTimeLoggedIn=
		(
			Case When Exists (Select 1 From LoginTracking Where Id=@UserId and LoginUserType=@pLoginTypeId) 
			THEN Cast(0 as bit) Else Cast(1 as bit) 
			END
		)
		,RolesCount=@RolesCount
		,Tabs=(Select * From @Tabs FOR JSON PATH,Include_Null_Values),
		IsEhrAccessible=
		(
			Case WHEN Exists (SELECT 1 from Tabs T WHERE T.TabName='EHR' AND Controller='Summary' And [Action]='PatientSummary' 
								And Exists(Select 1 From RoleTabs RT Where RT.TabId=T.TabId And RT.RoleId=@CurrentRoleId) And T.IsActive=1 And T.IsDeleted=0) 
			THEN @HasAccess
			Else @NoAccess END
		),
		IsActiveEncountersAccessible=
		(
			Case WHEN Exists (SELECT 1 from Tabs T WHERE T.TabName='Active Encounters' AND Controller='ActiveEncounter' And [Action]='ActiveEncounter' 
								And Exists(Select 1 From RoleTabs RT Where RT.TabId=T.TabId And RT.RoleId=@CurrentRoleId) And T.IsActive=1 And T.IsDeleted=0) 
			THEN @HasAccess
			Else @NoAccess END
		),
		IsAuthorizationAccessible=
		(
			Case WHEN Exists (SELECT 1 from Tabs T WHERE T.TabName='Obtain Insurance Authorization' AND Controller='Authorization' And [Action]='AuthorizationMain' 
								And Exists(Select 1 From RoleTabs RT Where RT.TabId=T.TabId And RT.RoleId=@CurrentRoleId) And T.IsActive=1 And T.IsDeleted=0) 
			THEN @HasAccess
			Else @NoAccess END
		),
		IsBillHeaderViewAccessible=
		(
			Case WHEN Exists (SELECT 1 from Tabs T WHERE T.TabName='Generate Preliminary Bill' AND Controller='BillHeader' And [Action]='Index' 
								And Exists(Select 1 From RoleTabs RT Where RT.TabId=T.TabId And RT.RoleId=@CurrentRoleId) And T.IsActive=1 And T.IsDeleted=0) 
			THEN @HasAccess
			Else @NoAccess END
		),
		IsPatientSearchAccessible=
		(
			Case WHEN Exists (SELECT 1 from Tabs T WHERE T.TabName='Patient Lookup' AND Controller='PatientSearch' And [Action]='PatientSearch' 
								And Exists(Select 1 From RoleTabs RT Where RT.TabId=T.TabId And RT.RoleId=@CurrentRoleId) And T.IsActive=1 And T.IsDeleted=0) 
			THEN @HasAccess
			Else @NoAccess END
		),
		SchedularAccessible=
		(
			Case WHEN Exists (SELECT 1 from Tabs T WHERE T.TabName='Scheduling' AND Controller='' And [Action]='' 
								And Exists(Select 1 From RoleTabs RT Where RT.TabId=T.TabId And RT.RoleId=@CurrentRoleId) And T.IsActive=1 And T.IsDeleted=0) 
			THEN @HasAccess
			Else @NoAccess END
		),CptTableNumber=(Select C.DefaultCPTTableNumber from Corporate C WHERE C.CorporateId=@CId),
HcPcsTableNumber=(Select C.DefaultHCPCSTableNumber from Corporate C WHERE C.CorporateId=@CId),
DrugTableNumber=(Select C.DefaultDRUGTableNumber from Corporate C WHERE C.CorporateId=@CId),
DrgTableNumber=(Select C.DefaultDRGTableNumber from Corporate C WHERE C.CorporateId=@CId),
ServiceCodeTableNumber=(Select C.DefaultServiceCodeTableNumber from Corporate C WHERE C.CorporateId=@CId),
DiagnosisCodeTableNumber=(Select C.DefaultDiagnosisTableNumber from Corporate C WHERE C.CorporateId=@CId),
BillEditRuleTableNumber=(Select C.BillEditRuleTableNumber from Corporate C WHERE C.CorporateId=@CId)
		From Users U
		Where U.UserId=@UserId 
		FOR JSON Path,Root('UserDto'),Include_Null_Values
	End
End
GO


