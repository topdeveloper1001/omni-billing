IF OBJECT_ID('SPROC_DefaultCorporateItems','P') IS NOT NULL
   DROP PROCEDURE SPROC_DefaultCorporateItems

GO

/****** Object:  StoredProcedure [dbo].[SPROC_DefaultCorporateItems]    Script Date: 6/20/2018 1:04:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_DefaultCorporateItems] -- [SPROC_DefaultCorporateItems] 1059,'ry'
(
	@aCorporateID INT,
	@aCorporateName VARCHAR(70)
)
AS
BEGIN
   BEGIN TRY
     SET NOCOUNT ON
        --SET XACT_ABORT ON
        DECLARE @pPreFixChars VARCHAR(4);
        DECLARE @pFacilityName VARCHAR(10);
        DECLARE @pCorporateAdmin VARCHAR(100);
        DECLARE @pFacilityID INT;
        DECLARE @pRoleID INT;
        DECLARE @UserId INT;
        DECLARE @UniqueNumber BIGINT ;  
		declare @pDefaultFacilityId INT;
		declare @pDefaultCorporateID INT;
		Declare @DefaultRoleKey nvarchar(5) = '101'
		Declare @LocalDatetime DATETIME, @CityId int, @StateId int,@CountryId int,@ZipCode nvarchar(10),@CMainPhone nvarchar(25)
		declare @FacilityMasterSetup table(FacilityMasterSetupID INT identity(1,1),MasterFacilityStructureID INT,MasterParentID INT
				,CreatedBy INT,ModifiedBy INT)
		declare @cMasterFacilityStructureID int
		declare @cMasterParentID int
		declare @cCreatedByID int
		declare @cModifiedByID int
		declare @pNewParentID INT;


		If @aCorporateID=0
			Select @aCorporateID = Ident_Current('Corporate')

		--Set the Default Facility and Corporate ID from where the default roles will be taken.
		Select TOP 1 @pDefaultFacilityId = FacilityId, @pDefaultCorporateID = CorporateId From [Role] Where RoleKey='1'


		Select @CountryId=CountryID,@StateId=StateID,@CityId=CityID,@ZipCode=ISNULL(CorporateZipCode,''),@CMainPhone=CorporateMainPhone
		,@UserId=CreatedBy
		From Corporate Where CorporateID=@aCorporateID

		SET @LocalDatetime = (Select dbo.GetCurrentDatetimeByEntity(0))

        -- Get 3 PreFix
        SET @pPreFixChars = (SELECT CASE WHEN LEN(@aCorporateName) <= 3 THEN @aCorporateName ELSE LEFT(@aCorporateName, 3) END  AS Comments)

        -- Get 3 PReFix
        SET @pFacilityName = @pPreFixChars + ' F1'
        SET @pCorporateAdmin = @aCorporateName + ' Admin'

		declare @pFacilityUserName varchar(50) =	(SELECT CASE CHARINDEX(' ', LTRIM(@aCorporateName), 1)
													WHEN 0 THEN LTRIM(@aCorporateName)
													ELSE SUBSTRING(LTRIM(@aCorporateName), 1, CHARINDEX(' ',LTRIM(@aCorporateName), 1) - 1)
													END FirstWordofCorporateName)
        --  Code Which Doesn't Require Transaction
        
       (SELECT @UniqueNumber= NEXT VALUE FOR SEQUniqueValue)
		--PRINT 'After UNIQUE'
        BEGIN TRANSACTION
			INSERT INTO [Facility]([FacilityNumber], [FacilityName],[FacilityCity], [FacilityState], [FacilityZipCode]
			,[FacilityMainPhone],[FacilityPOBox], [FacilityLicenseNumber],[FacilityTypeLicense], 
			[FacilityTotalLicenseBed], [FacilityTotalStaffedBed], [LoggedInID],[CreatedBy],[CreatedDate],[IsDeleted],[IsActive]
			,[CountryID],[CorporateID],FacilityTimeZone,SenderID, TimeZ)
			SELECT  CONVERT(NVARCHAR(99), @UniqueNumber), @pFacilityName,@CityId, @StateId, @ZipCode
			, @CMainPhone, @ZipCode,N'888'+CONVERT(NVARCHAR(99), @UniqueNumber), N'Licence'+CONVERT(NVARCHAR(99), @UniqueNumber)
			,1, 1, @UserId,@UserId,@LocalDatetime,0,1,@CountryId,@aCorporateID,'Central Standard Time','','-06:00'

			SET @pFacilityID = SCOPE_IDENTITY()

			INSERT INTO [dbo].[BillingSystemParameters]
			([FacilityNumber],[CorporateId],[BillHoldDays],[CPTTableNumber],[ServiceCodeTableNumber],[DRGTableNumber],[HCPCSTableNumber],[DiagnosisTableNumber]
			,[DrugTableNumber],[BillEditRuleTableNumber],[IsActive],[CreatedBy],[CreatedDate],DefaultCountry)
			Select CONVERT(NVARCHAR(99), @UniqueNumber),@aCorporateID,10,[DefaultCPTTableNumber],[DefaultServiceCodeTableNumber],[DefaultDRGTableNumber]
			,[DefaultHCPCSTableNumber],[DefaultDiagnosisTableNumber],[DefaultDRUGTableNumber],[BillEditRuleTableNumber],1
			,[CreatedBy],[CreatedDate],ISNULL(@CountryId,199)
			From Corporate where CorporateId = @aCorporateID

			INSERT INTO FacilityStructure (GlobalCodeID, FacilityStructureValue, FacilityStructureName, [Description], ParentId, ParentTypeGlobalID
			,FacilityId, SortOrder, IsActive,CreatedBy, CreatedDate, IsDeleted
			,ExternalValue1, ExternalValue2, ExternalValue3, ExternalValue4, ExternalValue5)
			SELECT GlobalCodeID, FacilityStructureValue, FacilityStructureName, [Description], ParentId, ParentTypeGlobalID,
			@pFacilityID, SortOrder, IsActive, @UserId, @LocalDatetime CreatedDate
			,0 IsDeleted, ExternalValue1, ExternalValue2, ExternalValue3, ExternalValue4, ExternalValue5
			FROM FacilityStructure 
			WHERE (FacilityId = @pDefaultFacilityId) and IsDeleted =  0

			insert into @FacilityMasterSetup(MasterFacilityStructureID,MasterParentID,CreatedBy,ModifiedBy)
			SELECT  FacilityStructureID, ParentID,CreatedBy,ModifiedBy from FacilityStructure where FacilityID = @pFacilityID


			DECLARE Cursor_BedTransactions CURSOR FOR
			SELECT MasterFacilityStructureID,MasterParentID,CreatedBy,ModifiedBy from @FacilityMasterSetup
	
			OPEN Cursor_BedTransactions

			FETCH NEXT FROM Cursor_BedTransactions INTO   @cMasterFacilityStructureID,@cMasterParentID,@cCreatedByID,@cModifiedByID
			WHILE @@FETCH_STATUS = 0  
			BEGIN  
				--Parent change logic start
				SET @pNewParentID = (Select FacilityStructureID from FacilityStructure Where Createdby = @cModifiedByID and  FacilityId = @pFacilityID);

				Update FacilityStructure SET parentid = @pNewParentID 
				WHERE FacilityStructureID = @cMasterFacilityStructureID
		
				FETCH NEXT FROM Cursor_BedTransactions INTO @cMasterFacilityStructureID,@cMasterParentID,@cCreatedByID,@cModifiedByID
	
			END  --END OF @@FETCH_STATUS = 0  
			CLOSE Cursor_BedTransactions  
			DEALLOCATE Cursor_BedTransactions 


			UPDATE FacilityStructure SET CreatedBy = @UserId,ModifiedBy = null WHERE FacilityId =  @pFacilityID


			 --Role
			INSERT INTO [ROLE]([IsActive],[RoleName], [CreatedBy], [CreatedDate], [IsDeleted],[CorporateId], [FacilityId],[IsGeneric],[RoleKey])
			SELECT  1, @pCorporateAdmin, @UserId, @LocalDatetime,0, @aCorporateID,@pFacilityID,1,@DefaultRoleKey

			SELECT @pRoleID=IDENT_CURRENT('ROLE')

			INSERT INTO [Role] ([IsActive],[RoleName], [CreatedBy], [CreatedDate], [IsDeleted],[CorporateId], [FacilityId],[IsGeneric],[RoleKey])
			SELECT IsActive, RoleName, @UserId, @LocalDatetime, 0 IsDeleted,@aCorporateID As CorporateId, @pFacilityID, 0 AS [IsGeneric],[RoleKey]
			FROM  [Role] WHERE FacilityId = @pDefaultFacilityId and ISNULL( IsDeleted,0)=0
			And IsGeneric=1 And RoleKey != '1' And RoleKey != '0'

			------ XXXXXXXXXXXXXXXXXXXXX NOTE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx------
			----- BB 11-Nov-2014 - Added one more AND condition in above NOT to Select SYSADMIN (ROLEKEY = 1) for every new Corporate
			------ XXXXXXXXXXXXXXXXXXXXX NOTE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx------

			------->>BB -- 29-Jan-2015->> New Requirement by (AJ) of Mapping default Roles created to new built Facility --- STARTS
		
			INSERT into FacilityRole  (FacilityID,RoleID,CorporateID,CreatedBy,CreatedDate,IsDeleted,IsActive,SchedulingApplied,CarePlanAccessible) 
			Select FacilityID,RoleID,CorporateID,@UserId,@LocalDatetime,0,1,1,1
			From [ROLE] Where CorporateId = @aCorporateID and FacilityId = @pFacilityID

			------->>BB -- 29-Jan-2015->> New Requirement by (AJ) of Mapping default Roles created to new built Facility --- ENDS
			
			--User
			INSERT INTO [Users]( [CountryID], [StateID], [CityID], [UserName], [FirstName], [LastName], [Answer], [Password], [Address], [Email]
			,[AdminUser], [IsActive], [CreatedBy], [CreatedDate],[IsDeleted], [CorporateId],[FacilityId])
			SELECT  @CountryId,@StateId,@CityId, @pFacilityUserName, @aCorporateName,  (@pPreFixChars), (@pPreFixChars), N'GyHl3AQLBV4='
			, (@pPreFixChars +'Address'), N'admin@'+ @aCorporateName +'.com', 1, 1, @UserId, @LocalDatetime,0, @aCorporateID,@pFacilityID

			SET @UserId = (select IDENT_CURRENT('Users'))
			-- UserRole

			-- Get the Default Role for user
			INSERT INTO UserRole (UserID,RoleID,IsActive,CreatedBy,CreatedDate)
			SELECT @UserId AS UserID, @pRoleID AS RoleID,1 AS IsActive, @UserId, @LocalDatetime AS CreatedDate


			---Module Acesss To Corporate
			INSERT INTO ModuleAccess
			SELECT @aCorporateID CorporateID, 0 AS FacilityID,
			TabID, TabName,[IsDeleted],@UserId,@LocalDatetime,Null,NUll,0,Null
			From ModuleAccess where CorporateID = @pDefaultCorporateID and FacilityID =0
			UNION
			SELECT @aCorporateID CorporateID, @pFacilityID AS FacilityID,
			TabID, TabName, [IsDeleted],@UserId,@LocalDatetime,Null,NUll,0,Null
			From ModuleAccess where CorporateID = @pDefaultCorporateID and FacilityID =@pDefaultFacilityId

			/* RoleTabs  */	
			INSERT INTO RoleTabs(RoleID, TabID)
			SELECT @pRoleID, TabId FROM Tabs WHERE ISNULL(IsDeleted,0) = 0 and tabID!=(Select TOP 1 TabId From Tabs Where TabName='Setup')

			--Insert Into DashBoardIndicators
			--Select [IndicatorNumber],[Dashboard],[Description],[Defination],[SubCategory1],[SubCategory2],[FormatType],[DecimalNumbers]
			--,[FerquencyType],[OwnerShip],0 As FacilityID,@aCorporateID,[CreatedBy],@LocalDatetime,[IsActive],[ExternalValue1],[ExternalValue2]
			--,[ExternalValue3],ReferencedIndicators,ExternalValue4,ExternalValue5,ExternalValue6,SortOrder,ExpressionText,ExpressionValue,SpecialCase 
			--from DashboardIndicators where CorporateID=9 and FacilityID=0

			--**************************Changes END here****************************--
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 AND XACT_STATE() <> 0 
            ROLLBACK TRAN

			PRINT ERROR_Message()
         --Do the Necessary Error logging if required
         --Take Corrective Action if Required
    END CATCH
END

/*

begin tran
delete from Corporate where CorporateID = (select top 1 CorporateID from Corporate order by 1 desc )
delete from FacilityRole where FacilityID =  (select top 1 FacilityID from [Facility] order by 1 desc)
delete from [Facility] where  FacilityID =  (select top 1 FacilityID from [Facility] order by 1 desc)
delete from RoleTabs where RoleID =  (select top 1 ROLEID from [ROLE] order by 1 desc)
delete from [ROLE] where  ROLEID = (select top 1 [RoleID] from [ROLE] order by 1 desc)
delete from  UserRole where   userid = (select top 1 userid from [Users] order by 1 desc)
delete from [Users] where userid = (select top 1 userid from [Users] order by 1 desc)
Delete From AuditLog where userid = (select top 1 userid from [Users] order by 1 desc)

			            commit
			            select * from Facility order by 1 desc
			            */
GO
