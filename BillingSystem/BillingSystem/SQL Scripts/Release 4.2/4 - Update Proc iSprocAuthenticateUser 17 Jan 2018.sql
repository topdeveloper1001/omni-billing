IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'iSprocAuthenticateUser')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROC iSprocAuthenticateUser
GO
/****** Object:  StoredProcedure [dbo].[iSprocAuthenticateUser]    Script Date: 16-01-2018 11:35:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[iSprocAuthenticateUser]
(
@pUsername nvarchar(100)='',
@pPassword nvarchar(100)='',
@pDeviceToken nvarchar(50)='',
@pPlatform nvarchar(50)='',
@pIsEmail bit=1,
@pUserId int=2536
)
AS
BEGIN
	
	--$$$$$$$$$$ Declarations $$$$$$$$$$$$$$----
	Declare @UserId bigint=0,@IsDefaultDevice bit=0,@IsAuthenticated bit=0,@CurrentDate datetime=null
	Declare @IsPatient bit=0
	--$$$$$$$$$$ Declarations $$$$$$$$$$$$$$----

	--Check if User is Patient or Medical-Provider.
	Select @UserId=U.UserID From [Users] U
	Where 
	((@pUserId=0 AND (
		@pUsername=(Case @pIsEmail WHEN 1 THEN U.Email ELSE U.UserName END)
		And [Password]=@pPassword
	)) OR 
	U.UserID=@pUserId
	) 
	And ISNULL(IsDeleted,0)=0
	AND U.UserID IN (Select UR.UserID From UserRole UR Where ISNULL(UR.IsActive,1)=1 AND ISNULL(UR.IsDeleted,0)=0)
	
	IF @UserId>0 SET @IsPatient=0
	Else
	Begin
		Select @UserId=P.PatientId From PatientLoginDetail P
		Where ((@pUserId=0 AND (P.Email=@pUsername And [Password]=@pPassword)) OR P.PatientId=@pUserId)
		And ISNULL(IsDeleted,0)=0

		IF @UserId>0 SET @IsPatient=1
	End
	IF @UserId > 0
	Begin
		IF @CurrentDate IS NULL
			SET @CurrentDate=GETUTCDATE()

		--Patient is using app for the first time, then device is set to be default
		IF (Select Count(1) From UserDevice Where UserId=@UserId)=0
			Set @IsDefaultDevice=1


		IF ISNULL(@pDeviceToken,'') !='' AND ISNULL(@pPlatform,'') !=''
		Begin
			IF Exists(Select 1 From UserDevice Where DeviceToken=@pDeviceToken And IsActive=1 And UserId=@UserId)
			Begin
				Update UserDevice SET [Platform]=@pPlatform, ModifiedBy=@UserId,ModifiedDate=@CurrentDate 
				Where UserId=@UserId And DeviceToken=@pDeviceToken
			End
			Else
				INSERT INTO UserDevice (UserId,IsPatient,DeviceToken,[Platform],IsDefault,CreatedBy,CreatedDate,IsActive)
				Select @UserId,1,@pDeviceToken,@pPlatform,@IsDefaultDevice,@UserId,@CurrentDate,1
		End


		SET @IsAuthenticated=1

		Select @IsAuthenticated As IsAuthenticated

		IF @IsPatient=1
			Select @IsPatient As IsPatient,P.PatientID As UserID,P.PersonEmailAddress As Email
			,P.PersonContactMobileNumber [Phone], (P.PersonFirstName +  ' ' + P.PersonLastName) [Name]
			,P.PersonEmailAddress As UserName,'Email' As LoginType,
			P.PersonFirstName As FirstName, P.PersonLastName As LastName
			,(Select TOP 1 PhoneNo From PatientPhone PH Where P.PatientId=PH.PatientId AND IsPrimary=1) As HomePhone
			, ISNULL(P.PersonGender,'') As Sex
			,ISNULL(L.ExternalValue2,'') As HomeCity
			,P.PersonBirthDate As BirthDate
			,Cast(0 As bit) As AdminUser, L.[Password]
			,Cast(ISNULL(P.FacilityId,0) as bigint) As FacilityId
			,Cast(ISNULL(P.PersonCountry,'0') As bigint) As CountryId
			,Cast(ISNULL(P.PersonCity,'0') As bigint) As CityId
			,Cast(ISNULL(P.PersonArea,'0') As bigint) As StateId
			,(Select TOP 1 [Name] From Country C Where C.CountryId=ISNULL(P.PersonCountry,0)) As CountryName
			,(Select TOP 1 CityName From City S Where S.CityId=ISNULL(P.PersonCity,0)) As CityName
			,(Select TOP 1 [StateName] From [State] C Where C.StateId=ISNULL(P.PersonArea,0)) As StateName
			From PatientInfo P
			INNER JOIN PatientLoginDetail L ON P.PatientId=L.PatientId
			Where P.PatientID = @UserId And ISNULL(P.IsDeleted,0)=0
			FOR JSON PATH, ROOT('UserDto')
		Else
			Select @IsPatient As IsPatient,U.UserID,U.Email,U.Phone,(U.FirstName + ' ' + U.LastName) As [Name]
			,U.UserName,'Username' as LoginType
			,U.FirstName, U.LastName
			,(Case 
				ISNULL(U.FacilityId,0) When 
											0 THEN  ''
									   ELSE
									(Select TOP 1 ISNULL(FacilityName,'') From Facility Where FacilityId=U.FacilityId) 
									END) As HomeCity
			,U.AdminUser, U.[Password]
			,CAST(U.FacilityId as bigint) As FacilityId
			,CAST(ISNULL(CountryID,0) as bigint) As CountryId
			,CAST(ISNULL(CityID,0) as bigint) As CityId
			,CAST(ISNULL(StateID,0) as bigint) As StateId
			From Users U
			Where IsActive=1 And U.UserID=@UserId And ISNULL(IsDeleted,0)=0 
			FOR	JSON PATH, ROOT('UserDto')
	End
	Else
		Select @IsAuthenticated As IsAuthenticated
END
