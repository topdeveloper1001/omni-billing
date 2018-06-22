IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_DefaultFacilityItems_CopyFrom')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_DefaultFacilityItems_CopyFrom
GO

/****** Object:  StoredProcedure [dbo].[SPROC_DefaultFacilityItems_CopyFrom]    Script Date: 21-03-2018 10:49:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_DefaultFacilityItems_CopyFrom]
(@aFacilityID INT
,@aFacilityName Varchar(100)
,@aCreatedBy INT
)
AS
BEGIN
  -- BEGIN TRY
     SET NOCOUNT ON
        SET XACT_ABORT ON
          DECLARE @pPreFixChars VARCHAR(6);
          DECLARE @pFacilityName VARCHAR(10);
          DECLARE @pCorporateAdmin VARCHAR(100);
          --DECLARE @pFacilityID INT;
          DECLARE @pRoleID INT;
          DECLARE @pUserID INT;
          DECLARE @UniqueNumber BIGINT ;  
		  DECLARE @pDefaultFacilityId INT  = 8;
		  DECLARE @pDefaultCorporateID INT = 9;
		  DECLARE @ptabIDSetup INT = 154;-- Should availabe ony for servicedot and spadez
		  --DECLARE @pDefaultFacilityId INT = (SELECT MIN(FacilityID) from Facility where	
				--										CorporateID IN  (
				--										SELECT CorporateID FROM Facility WHERE FacilityId = @aFacilityID 
				--														))
			
		  DECLARE @aCorporateName VARCHAR(70) = (SELECT (CorporateName) from Corporate where	
														CorporateID IN  (
														SELECT CorporateID FROM Facility WHERE FacilityId = @aFacilityID 
																		))
		  DECLARE @pCorporateID INT = (SELECT top 1 CorporateID FROM Facility WHERE FacilityId = @aFacilityID)
		  DECLARE @pFacilityCount INT = (SELECT COUNT(CorporateID) FROM Facility WHERE  CorporateID =  @pCorporateID)
		  DECLARE @pFacilityNumber Varchar(50) = (SELECT top 1 FacilityNumber FROM Facility WHERE FacilityId = @aFacilityID)
		 

			DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@aFacilityID))

        -- Get 3 PreFix
  ----      SET @pPreFixChars =  (SELECT  CASE WHEN LEN(@aFacilityName) <= 3 
  ----           THEN @aFacilityName
  ----           ELSE LEFT(@aFacilityName, 3)
		----		END  AS Comments) + CONVERT(varchar(3),@pFacilityCount)
  ----      -- Get 3 PReFix
  ----      SET @pFacilityName = @pPreFixChars + 'F1'
  ----      SET @pCorporateAdmin = @aCorporateName + 'FacilityAdmin'

		----DECLARE @pFacilityUserName VARCHAR(50) =	(SELECT CASE CHARINDEX(' ', LTRIM(@aFacilityName), 1)
		----											WHEN 0 THEN LTRIM(@aFacilityName)
		----											ELSE SUBSTRING(LTRIM(@aFacilityName), 1, CHARINDEX(' ',LTRIM(@aFacilityName), 1) - 1)
		----											END FirstWordOfFacilityName) 
		----SET @pFacilityUserName = RTRIM( LTRIM( @pFacilityUserName)) + RTRIM( LTRIM(@pFacilityNumber));											
		--Get RoleOfCorporateAdmin
		DECLARE @pCorporateRoleID INT = (select MIN ([RoleID]) From [Role] where CorporateId = @pCorporateID and ISNULL( IsDeleted,0) = 0 and ISNULL( IsActive,0)= 1  )
 
       (SELECT @UniqueNumber= NEXT VALUE FOR SEQUniqueValue)
 
		--SELECT  @pCorporateID,@pDefaultFacilityId, @aFacilityID,@pFacilityCount

     --   BEGIN TRANSACTION
			--01 Facility
		IF((@pFacilityCount) > 0)--IF IT IS GT 1	
		BEGIN
		
			--SET @pFacilityID = @aFacilityID; 
			
			
			INSERT INTO FacilityStructure (GlobalCodeID, FacilityStructureValue, FacilityStructureName, Description, ParentId, ParentTypeGlobalID,  FacilityId, SortOrder, IsActive, 
                         CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsDeleted, DeletedBy, DeletedDate, ExternalValue1, ExternalValue2, ExternalValue3, ExternalValue4, 
                         ExternalValue5)
					SELECT         GlobalCodeID, FacilityStructureValue, FacilityStructureName, Description, ParentId, ParentTypeGlobalID, @aFacilityID, SortOrder, IsActive, 
								   FacilityStructureId AS  CreatedBy, @LocalDateTime CreatedDate, ParentId AS  ModifiedBy, NULL ModifiedDate, 0 IsDeleted, NULL DeletedBy, NULL DeletedDate, ExternalValue1, ExternalValue2, ExternalValue3, ExternalValue4, 
									 ExternalValue5--@pPreFixChars + FacilityStructureName removed it
			FROM            FacilityStructure
			WHERE        (FacilityId = @pDefaultFacilityId) AND IsDeleted =  0
			
				--  START OF FACILITY STRUCTURE LOGIC OBJECTIVE TO ADD ACCURATE PARENTS ID AND ENTRY IN UBEDMASTER
				
			DECLARE @FacilityMasterSetup TABLE(
					FacilityMasterSetupID INT IDENTITY(1,1)
					,MasterFacilityStructureID INT
					,MasterParentID INT
					,CreatedBy INT
					,ModifiedBy INT
				)
			DECLARE @cMasterFacilityStructureID INT
			DECLARE @cMasterParentID INT
			DECLARE @cCreatedByID INT
			DECLARE @cModifiedByID INT
			--DECLARE @pNewFalicityID INT = @pFacilityID;
			DECLARE @pNewParentID INT;
			INSERT INTO @FacilityMasterSetup(MasterFacilityStructureID,MasterParentID,CreatedBy,ModifiedBy)
			SELECT     FacilityStructureID, ParentID,CreatedBy,ModifiedBy FROM  FacilityStructure WHERE FacilityID = @aFacilityID

				DECLARE Cursor_BedTransactions CURSOR FOR  
				SELECT  MasterFacilityStructureID,MasterParentID,CreatedBy,ModifiedBy FROM @FacilityMasterSetup
				OPEN Cursor_BedTransactions  
				FETCH NEXT FROM Cursor_BedTransactions INTO   @cMasterFacilityStructureID,@cMasterParentID,@cCreatedByID,@cModifiedByID
				WHILE @@FETCH_STATUS = 0  
				BEGIN  
					--Parent change logic start
					SET @pNewParentID = (SELECT  FacilityStructureID FROM FacilityStructure WHERE Createdby = @cModifiedByID AND  FacilityId = @aFacilityID);
					UPDATE FacilityStructure
					SET parentid = @pNewParentID
					WHERE FacilityStructureID = @cMasterFacilityStructureID
					--and FacilityId = @pNewFalicityID
					--Add in UbedMaster:-Start
					IF EXISTS (SELECT 'X' FROM ubedmaster WHERE facilityid = @pDefaultFacilityId AND FacilityStructureid = @cCreatedByID )
					BEGIN
				--03 UBedMaster	
					INSERT INTO ubedmaster (  FacilityId, FacilityStructureId, BedType, Rate, StartDate, IsOccupied, IsRateApplied, SortOrder, IsActive, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, 
							 IsDeleted, DeletedBy, DeletedDate)
							 SELECT @aFacilityID AS FacilityId, @cMasterFacilityStructureID AS FacilityStructureId, BedType, Rate, @LocalDateTime StartDate, 0 IsOccupied, NULL IsRateApplied, SortOrder, 1 IsActive, 1 CreatedBy, @LocalDateTime CreatedDate, ModifiedBy, ModifiedDate, 
							 IsDeleted, DeletedBy, DeletedDate FROM ubedmaster WHERE  facilityid = @pDefaultFacilityId AND FacilityStructureid = @cCreatedByID 
				END
				--Add in UbedMaster:-End
				FETCH NEXT FROM Cursor_BedTransactions INTO @cMasterFacilityStructureID,@cMasterParentID,@cCreatedByID,@cModifiedByID
				END  --END OF @@FETCH_STATUS = 0  
			CLOSE Cursor_BedTransactions  
			DEALLOCATE Cursor_BedTransactions 

			UPDATE FacilityStructure 
			SET CreatedBy = 1
			,ModifiedBy = NULL
			WHERE FacilityId =  @aFacilityID
			--  END OF FACILITY STRUCTURE LOGIC OBJECTIVE TO ADD ACCURATE PARENTS ID AND ENTRY IN UBEDMASTER
			 -- 04 Role1: Facility Type
			------INSERT INTO [ROLE]( [IsActive], [RoleName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsDeleted], [DeletedBy], [DeletedDate], [CorporateId], [FacilityId])
			------ SELECT    1, @pCorporateAdmin, 1, @LocalDateTime, NULL, NULL, 0, NULL, NULL, @pCorporateID, @pFacilityID
			------ SET @pRoleID = (SELECT IDENT_CURRENT('ROLE'))
			--------Role2: Default Roles from setup
			------INSERT INTO ROLE (  IsActive, RoleName,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsDeleted, DeletedBy, DeletedDate, CorporateId, FacilityId)
			------	SELECT         IsActive,  RoleName, 1 CreatedBy, @LocalDateTime CreatedDate, ModifiedBy, ModifiedDate, 0 IsDeleted, DeletedBy, DeletedDate, @pCorporateID CorporateId, @pFacilityID FacilityId
			------	FROM            ROLE  
			------	WHERE        (CorporateId = @pDefaultCorporateID) AND  ISNULL( IsDeleted,0)= 0 and RoleID <> 40
			------	------ XXXXXXXXXXXXXXXXXXXXX NOTE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx------
			------			----- BB 11-Nov-2014 - Added one more AND condition in above NOT to Select SYSADMIN (ROLEID = 40) for every new Corporate
			------			------ XXXXXXXXXXXXXXXXXXXXX NOTE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx------


			------->>BB -- 29-Jan-2015->> New Requirement by (AJ) of Mapping default Roles created to new built Facility --- STARTS
				insert into FacilityRole  (FacilityID,RoleID,CorporateID,CreatedBy,Createddate,Isdeleted,IsActive) 
				Select FacilityID,RoleID,CorporateID,1,@LocalDateTime,0,1 From ROLE Where CorporateId = @pCorporateID and FacilityId = @aFacilityID
			------->>BB -- 29-Jan-2015->> New Requirement by (AJ) of Mapping default Roles created to new built Facility --- ENDS


		----	-- 05 User -- @pPreFixChars +
		----	INSERT INTO [Users]( [CountryID], [StateID], [CityID], [UserGroup], [UserName], [FirstName], [LastName], [Answer], [Password], [Address], [Email], [Phone], [HomePhone], [AdminUser], [IsActive], [FailedLoginAttempts], [LastInvalidLogin], [LastResetPassword], [LastLogin], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsDeleted], [DeletedBy], [DeletedDate],[CorporateId], [FacilityId])
		----		SELECT  45, 3, 5, NULL, @pFacilityUserName, @aCorporateName,  (@pPreFixChars), (@pPreFixChars), N'GyHl3AQLBV4=', (@pPreFixChars +'Address'), N'test@test.com', N'+971-1234567890', N'+971-9999999999', 1, 1, NULL, NULL, NULL, NULL, 1, @LocalDateTime, 1, @LocalDateTime, 0, NULL, NULL, @pCorporateID, @aFacilityID
		----		SET @pUserID = (SELECT IDENT_CURRENT('Users'))
			
		----	--06 UserRole
		----	INSERT INTO UserRole (UserID,RoleID,IsActive,CreatedBy,CreatedDate )
		----	SELECT @pUserID AS UserID, @pRoleID AS RoleID,1 AS IsActive, 1 AS CreatedBy, @LocalDateTime AS CreatedDate
		------	union all
		------	SELECT @aCreatedBy AS UserID, @pRoleID AS RoleID,1 AS IsActive, 1 AS CreatedBy, @LocalDateTime AS CreatedDate
		---------- XXXXXXXXXXXXXXXXXXXXX NOTE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx------
		--------- BB 11-Nov-2014 - COMMENTED ABove UNION ALL part because it was creating extra ROLE for SYSADMIN each time
		---------- XXXXXXXXXXXXXXXXXXXXX NOTE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx------
		
		----	--07 RoleTabs
		----	INSERT INTO RoleTabs(  RoleID, TabID)
		----	SELECT @pRoleID, TabId  FROM Tabs WHERE ISNULL(IsDeleted,0) = 0 AND tabID <> @ptabIDSetup
			
			--08 FacilityRole
			--INSERT INTO FacilityRole(FacilityId, RoleId, CorporateId, CreatedBy, CreatedDate, IsActive)
			--SELECT @pFacilityID FacilityId, @pRoleID RoleId, @pCorporateID CorporateId, 1 CreatedBy, @LocalDateTime CreatedDate, 1 IsActive
		--	union all
		--	SELECT @pFacilityID FacilityId, @pCorporateRoleID RoleId, @pCorporateID CorporateId, 1 CreatedBy, @LocalDateTime CreatedDate, 1 IsActive
						------ XXXXXXXXXXXXXXXXXXXXX NOTE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx------
						----- BB 11-Nov-2014 - COMMENTED ABove UNION ALL part because it was creating extra ROLE for SYSADMIN each time
						------ XXXXXXXXXXXXXXXXXXXXX NOTE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx------
		 END--IF IT IS GT 1
      --  COMMIT TRANSACTION
    --END TRY
    --BEGIN CATCH
    --    IF @@TRANCOUNT > 0 AND XACT_STATE() <> 0 
    --        ROLLBACK TRAN
 
    --     --Do the Necessary Error logging if required
    --     --Take Corrective Action if Required
 
       
    --END CATCH
END

/*
begin tran
delete from Corporate where CorporateID = (select top 1 CorporateID from Corporate order by 1 desc )
delete from FacilityRole where FacilityID =  (select top 1 FacilityID from [Facility] order by 1 desc)
delete from [Facility] where  FacilityStreetAddress = 'AutoFacilityStreetAddress'
delete from RoleTabs where RoleID =  (select top 1 ROLEID from [ROLE] order by 1 desc)
delete from [ROLE] where  ROLEID = (select top 1 [RoleID] from [ROLE] order by 1 desc)
delete from [Users] where userid = (select top 1 userid from [Users] order by 1 desc)
delete from  UserRole where   userid = (select top 1 userid from [Users] order by 1 desc)

			            commit
			            select * from Facility order by 1 desc
			            
			            */
			            















GO


