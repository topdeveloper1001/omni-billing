IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_DefaultCorporateItems')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_DefaultCorporateItems
GO
/****** Object:  StoredProcedure [dbo].[SPROC_DefaultCorporateItems]    Script Date: 3/21/2018 1:22:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_DefaultCorporateItems]-- [SPROC_DefaultCorporateItems] 1059,'ry'
(
 @aCorporateID INT
,@aCorporateName VARCHAR(70)
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
        DECLARE @pUserID INT;
        DECLARE @UniqueNumber BIGINT ;  
		declare @pDefaultFacilityId INT  = 8;
		declare @pDefaultCorporateID INT = 9;
		declare @ptabIDSetup INT = 154;-- Should availabe ony for servicedot and spadez
		Declare @DefaultRoleKey nvarchar(5) = '1001'
		Declare @LocalDatetime DATETIME

		If @aCorporateID=0
		Begin
			Select @aCorporateID = Ident_Current('Corporate')
		End

		--Set the Default Facility and Corporate ID from where the default roles will be taken.
		Select TOP 1 @pDefaultFacilityId = FacilityId, @pDefaultCorporateID = CorporateId From [Role] Where RoleKey='1'

		SET @LocalDatetime = (Select dbo.GetCurrentDatetimeByEntity(0))

        -- Get 3 PreFix

        SET @pPreFixChars = (SELECT CASE WHEN LEN(@aCorporateName) <= 3 
             THEN @aCorporateName
             ELSE LEFT(@aCorporateName, 3)
				END  AS Comments)
        -- Get 3 PReFix
        SET @pFacilityName = @pPreFixChars + ' F1'
        SET @pCorporateAdmin = @aCorporateName + ' Admin'

		declare @pFacilityUserName varchar(50) =	(SELECT CASE CHARINDEX(' ', LTRIM(@aCorporateName), 1)
													WHEN 0 THEN LTRIM(@aCorporateName)
													ELSE SUBSTRING(LTRIM(@aCorporateName), 1, CHARINDEX(' ',LTRIM(@aCorporateName), 1) - 1)
													END FirstWordofCorporateName)
        --  Code Which Doesn't Require Transaction
        
		--PRINT 'Before Facility'
       (SELECT @UniqueNumber= NEXT VALUE FOR SEQUniqueValue)
		--PRINT 'After UNIQUE'
        BEGIN TRANSACTION
			INSERT INTO [Facility]([FacilityNumber], [FacilityName], [FacilityStreetAddress], [FacilityStreetAddress2], 
			[FacilityCity], [FacilityState], [FacilityZipCode], [FacilityMainPhone], [FacilityFax], [FacilitySecondPhone], 
			[FacilityPOBox], [FacilityLicenseNumber], [FacilityLicenseNumberExpire], [FacilityTypeLicense], [FacilityRelated], 
			[FacilityTotalLicenseBed], [FacilityTotalStaffedBed], [FacilityAffiliationNumber], [LoggedInID], [CreatedBy], 
			[CreatedDate], [ModifiedBy], [ModifiedDate], [IsDeleted], [DeletedBy], [DeletedDate], [IsActive], [CountryID], 
			[CorporateID],FacilityTimeZone,SenderID, TimeZ)
			SELECT  CONVERT(NVARCHAR(99), @UniqueNumber), @pFacilityName, N'AutoFacilityStreetAddress', N'AutoFacilityStreetAddress2', 
			3, 3, '9999', '+971-9999999', '+971-9999999', '+971-9999999', 
			N'11111112', N'888'+CONVERT(NVARCHAR(99), @UniqueNumber), @LocalDatetime, N'Licence'+CONVERT(NVARCHAR(99), @UniqueNumber), null, 
			999, 999, 999, 0, 1, 
			@LocalDatetime,0, @LocalDatetime, 0, NULL, NULL, 1, 45,
			@aCorporateID,'Central Standard Time','','-06:00'

			SET @pFacilityID = (select IDENT_CURRENT('Facility'))

			/*
			WHO: Shashank
			WHAT:Insert billing system parameters for the new facility
			WHEN: 13 April 2016
			WHY: Insert billing system parameters missing
			*/
			--PRINT 'Before Billing Parameters' + '     New Facility ID: ' + Cast(@pFacilityID as nvarchar)
			INSERT INTO [dbo].[BillingSystemParameters]
			([FacilityNumber],[CorporateId],[BillHoldDays],[CPTTableNumber],[ServiceCodeTableNumber],[DRGTableNumber],[HCPCSTableNumber],[DiagnosisTableNumber]
           ,[DrugTableNumber],[BillEditRuleTableNumber],[IsActive],[CreatedBy],[CreatedDate],DefaultCountry)
		   Select CONVERT(NVARCHAR(99), @UniqueNumber),@aCorporateID,10,[DefaultCPTTableNumber],[DefaultServiceCodeTableNumber],[DefaultDRGTableNumber],[DefaultHCPCSTableNumber]
		   ,[DefaultDiagnosisTableNumber],[DefaultDRUGTableNumber],[BillEditRuleTableNumber],1,[CreatedBy],[CreatedDate],ISNULL(CountryID,199)
		   From Corporate where CorporateId = @aCorporateID

			/*   
			--Defalut Room Structure
			*/
			--Print 'FacilityStructure';

			PRINT 'Before FacilityStructure'

			INSERT INTO FacilityStructure (GlobalCodeID, FacilityStructureValue, FacilityStructureName, Description, ParentId, ParentTypeGlobalID,  FacilityId, SortOrder, IsActive, 
                         CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsDeleted, DeletedBy, DeletedDate, ExternalValue1, ExternalValue2, ExternalValue3, ExternalValue4, 
                         ExternalValue5)
					SELECT         GlobalCodeID, FacilityStructureValue, FacilityStructureName, Description, ParentId, ParentTypeGlobalID, @pFacilityID, SortOrder, IsActive, 
								   FacilityStructureId as  CreatedBy, @LocalDatetime CreatedDate, ParentId as  ModifiedBy, NULL ModifiedDate, 0 IsDeleted, NULL DeletedBy, NULL DeletedDate, ExternalValue1, ExternalValue2, ExternalValue3, ExternalValue4, 
									 ExternalValue5
			FROM            FacilityStructure -- Removed  @pPreFixChars +FacilityStructureName
			WHERE        (FacilityId = @pDefaultFacilityId) and IsDeleted =  0
				-- 20141105: START OF FACILITY STRUCTURE

declare @FacilityMasterSetup table(
		FacilityMasterSetupID INT identity(1,1)
		,MasterFacilityStructureID INT
		,MasterParentID INT
		,CreatedBy INT
		,ModifiedBy INT
	)

declare @cMasterFacilityStructureID int
declare @cMasterParentID int
declare @cCreatedByID int
declare @cModifiedByID int
declare @pNewFalicityID int = @pFacilityID;
declare @pNewParentID INT;
insert into @FacilityMasterSetup(MasterFacilityStructureID,MasterParentID,CreatedBy,ModifiedBy)
SELECT     FacilityStructureID, ParentID,CreatedBy,ModifiedBy from  FacilityStructure where FacilityID = @pNewFalicityID


Print 'Before Cursor_BedTransactions'
DECLARE Cursor_BedTransactions CURSOR FOR  
				SELECT  MasterFacilityStructureID,MasterParentID,CreatedBy,ModifiedBy from @FacilityMasterSetup
	
	OPEN Cursor_BedTransactions  

	FETCH NEXT FROM Cursor_BedTransactions INTO   @cMasterFacilityStructureID,@cMasterParentID,@cCreatedByID,@cModifiedByID
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		--Parent change logic start
		set @pNewParentID = (Select  FacilityStructureID from FacilityStructure Where Createdby = @cModifiedByID and  FacilityId = @pNewFalicityID);
		update FacilityStructure
		set parentid = @pNewParentID
		where FacilityStructureID = @cMasterFacilityStructureID
		--and FacilityId = @pNewFalicityID

	--Add in UbedMaster:-Start
		if exists (select 'X' from ubedmaster where facilityid = @pDefaultFacilityId and FacilityStructureid = @cCreatedByID )
		begin
		insert into ubedmaster (  FacilityId, FacilityStructureId, BedType, Rate, StartDate, IsOccupied, IsRateApplied, SortOrder, IsActive, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, 
		                  IsDeleted, DeletedBy, DeletedDate)
				 select @pNewFalicityID as FacilityId, @cMasterFacilityStructureID as FacilityStructureId, BedType, Rate, @LocalDatetime StartDate, 0 IsOccupied, null IsRateApplied, SortOrder, 1 IsActive, 1 CreatedBy, @LocalDatetime CreatedDate, ModifiedBy, ModifiedDate, 
		                  IsDeleted, DeletedBy, DeletedDate from ubedmaster where  facilityid = @pDefaultFacilityId and FacilityStructureid = @cCreatedByID 
		end
	--Add in UbedMaster:-End
	FETCH NEXT FROM Cursor_BedTransactions INTO @cMasterFacilityStructureID,@cMasterParentID,@cCreatedByID,@cModifiedByID
	
	END  --END OF @@FETCH_STATUS = 0  
CLOSE Cursor_BedTransactions  
DEALLOCATE Cursor_BedTransactions 
--Print 'FacilityStructure';
UPDATE FacilityStructure SET CreatedBy = 1,ModifiedBy = null
WHERE FacilityId =  @pNewFalicityID
				-- 20141105: END OF FACILITY STRUCTURE
	-- update start UBedMaster:
--	insert into ubedmaster(FacilityStructureid)
--select FacilityStructureid from FacilityStructure where  facilityid = @pNewParentID

--select UB1.BedType from ubedmaster UB1 inner join FacilityStructure FS on 
-- update end UBedMaster:
Print 'Before Role'

			 --Role
			 --Print 'Role';
			INSERT INTO [ROLE]( [IsActive], [RoleName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsDeleted], [DeletedBy], [DeletedDate], [CorporateId], [FacilityId],[IsGeneric],[RoleKey])
			SELECT    1, @pCorporateAdmin, 1, @LocalDatetime, NULL, NULL, 0, NULL, NULL, @aCorporateID,@pFacilityID,0,@DefaultRoleKey

			SET @pRoleID = (select IDENT_CURRENT('ROLE'))
					/**/
						insert into role (  IsActive, RoleName,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsDeleted, DeletedBy, DeletedDate
						,CorporateId, FacilityId,[IsGeneric],[RoleKey])
						SELECT         IsActive, RoleName, 1 CreatedBy, @LocalDatetime CreatedDate, ModifiedBy, ModifiedDate, 0 IsDeleted, DeletedBy, DeletedDate
						,@aCorporateID As CorporateId, @pFacilityID As FaciliyId, 0 AS [IsGeneric],[RoleKey]
						FROM  [Role]
						WHERE (FacilityId = @pDefaultFacilityId) and ISNULL( IsDeleted,0)= 0
						--and RoleID NOT in (218,329,644,240,284,351,373,621,666,688)
						And IsGeneric=1 And RoleKey != '1' And RoleKey != '0'
						------ XXXXXXXXXXXXXXXXXXXXX NOTE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx------
						----- BB 11-Nov-2014 - Added one more AND condition in above NOT to Select SYSADMIN (ROLEID = 40) for every new Corporate
						------ XXXXXXXXXXXXXXXXXXXXX NOTE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx------
		------->>BB -- 29-Jan-2015->> New Requirement by (AJ) of Mapping default Roles created to new built Facility --- STARTS
		Print 'FacilityRole';
				insert into FacilityRole  (FacilityID,RoleID,CorporateID,CreatedBy,Createddate,Isdeleted,IsActive,SchedulingApplied,CarePlanAccessible) 
				Select FacilityID,RoleID,CorporateID,1,@LocalDatetime,0,1,0,0 From [ROLE] Where CorporateId = @aCorporateID and FacilityId = @pFacilityID
			------->>BB -- 29-Jan-2015->> New Requirement by (AJ) of Mapping default Roles created to new built Facility --- ENDS
 -- removed @pPreFixChars +
			--User
			Print 'User';
			INSERT INTO [Users]( [CountryID], [StateID], [CityID], [UserGroup], [UserName], [FirstName], [LastName], [Answer], [Password], [Address], [Email], [Phone], [HomePhone], [AdminUser], [IsActive], [FailedLoginAttempts], [LastInvalidLogin], [LastResetPassword], 
			[LastLogin], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsDeleted], [DeletedBy], [DeletedDate], [CorporateId],[FacilityId])
			SELECT  45, 3, 5, NULL, @pFacilityUserName, @aCorporateName,  (@pPreFixChars), (@pPreFixChars), N'GyHl3AQLBV4=', (@pPreFixChars +'Address'), N'test@test.com', N'+971-1234567890', N'+971-9999999999', 1, 1, NULL, NULL, NULL, NULL, 1, @LocalDatetime, 1, @LocalDatetime, 
			0, NULL, NULL, @aCorporateID,@pFacilityID

			SET @pUserID = (select IDENT_CURRENT('Users'))
			-- UserRole
			-- Get the Default Role for user
			Print 'UserRole';
			INSERT INTO UserRole (UserID,RoleID,IsActive,CreatedBy,CreatedDate)
			SELECT @pUserID AS UserID, @pRoleID AS RoleID,1 AS IsActive, 1 AS CreatedBy, @LocalDatetime AS CreatedDate


			---Module Acesss To Corporate
			Print 'ModuleAccess'
			INSERT INTO ModuleAccess
			SELECT @aCorporateID CorporateID, 0 AS FacilityID,
			TabID, TabName, [IsDeleted],[CreatedBy],@LocalDatetime,Null,NUll,0,Null
			From ModuleAccess where CorporateID = @pDefaultCorporateID and FacilityID =0

			INSERT INTO ModuleAccess
			SELECT @aCorporateID CorporateID, @pFacilityID AS FacilityID,
			TabID, TabName, [IsDeleted],[CreatedBy],@LocalDatetime,Null,NUll,0,Null
			From ModuleAccess where CorporateID = @pDefaultCorporateID and FacilityID =@pDefaultFacilityId

			/* RoleTabs  */	
			Print 'Before RoleTabs'
			INSERT INTO RoleTabs(RoleID, TabID)
			SELECT @pRoleID, TabId  FROM Tabs WHERE ISNULL(IsDeleted,0) = 0 and tabID <> @ptabIDSetup

			-- FacilityRole

			--INSERT INTO FacilityRole(FacilityId, RoleId, CorporateId, CreatedBy, CreatedDate, IsActive)
			--SELECT @pFacilityID FacilityId, @pRoleID RoleId, @aCorporateID CorporateId, 1 CreatedBy, @LocalDatetime CreatedDate, 1 IsActive

			--- Indicators 
			--Print 'DashBoardIndicators'

			/*
			Changes by Amit Jain on 06 April, 2016
			Purpose: SpecialCase Value was being stored as ZERO by default in Indicators for New Corporate, 
			which is now changed to dynamic value coming from the Table based on Corporate 12 (i.e. Cambridge)
			*/

			--**************************Changes Start here****************************--

			--Insert Into DashBoardIndicators
			--Select 
			--[IndicatorNumber],[Dashboard],[Description],[Defination],[SubCategory1],[SubCategory2],[FormatType],[DecimalNumbers]
   --        ,[FerquencyType],[OwnerShip],0,@aCorporateID,[CreatedBy],@LocalDatetime,[IsActive],[ExternalValue1],[ExternalValue2]
   --        ,[ExternalValue3],ReferencedIndicators,ExternalValue4,ExternalValue5,ExternalValue6,SortOrder,ExpressionText,ExpressionValue,0
			-- from DashboardIndicators where CorporateID = 12 and FacilityID =0

			PRINT 'Before Dashbaord Indicators'
			Insert Into DashBoardIndicators
			Select 
			[IndicatorNumber],[Dashboard],[Description],[Defination],[SubCategory1],[SubCategory2],[FormatType],[DecimalNumbers]
           ,[FerquencyType],[OwnerShip],0 As FacilityID,@aCorporateID,[CreatedBy],@LocalDatetime,[IsActive],[ExternalValue1],[ExternalValue2]
           ,[ExternalValue3],ReferencedIndicators,ExternalValue4,ExternalValue5,ExternalValue6,SortOrder,ExpressionText,ExpressionValue,SpecialCase 
			 from DashboardIndicators where CorporateID = 12 and FacilityID =0

			--**************************Changes END here****************************--


			Declare @YearValue nvarchar(5) = Cast(DATEPART(yyyy,@LocalDatetime) as Nvarchar(5));
			Declare @PreviousYearValue nvarchar(5) = Cast((@YearValue) as Int) - 1;
			--EXEC SPROC_CreateDashboardIndicatorData @aCorporateID,@pNewFalicityID,@YearValue
			--EXEC SPROC_CreateDashboardIndicatorData @aCorporateID,@pNewFalicityID,@PreviousYearValue

			--PRINT 'Before PROC: SPROC_InsertDefaulValuesInDashboardIndicatorData'

			EXEC SPROC_InsertDefaulValuesInDashboardIndicatorData @aCorporateID,@pNewFalicityID,'C'
			
			--EXEC SPROC_CreateDataForAllFacility @aCorporateID,@YearValue


			--- Create auto dashboard Budgets for the Corporate facility
			--PRINT 'Before Another PROC: SPROC_CreateDefaultAutoDashboardBudgets'
			EXEC SPROC_CreateDefaultAutoDashboardBudgets @aCorporateID,@pNewFalicityID
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





