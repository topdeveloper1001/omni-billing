IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SPROC_AddUpdateModuleAccess') 
  DROP PROCEDURE SPROC_AddUpdateModuleAccess;
  
 
GO
/****** Object:  StoredProcedure [dbo].[SPROC_AddUpdateModuleAccess]    Script Date: 7/13/2017 5:27:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPROC_AddUpdateModuleAccess]
(
    @pModuleAccessList ModuleAccessTT readonly,
	@pCorporateId Int,
	@pFacilityId Int,
	@LoggedInUserId int,
	@CurrentDate DateTime
)
AS
--Temp table to store full Module Access data
Declare @ModuleAccessTempTable Table
(	
	[ModuleAccessID] [int] NOT NULL,
	[CorporateID] [int] NULL,
	[FacilityID] [int] NULL,
	[TabID] [int] NULL,
	[TabName] [nvarchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedBy] [int] NULL,
	[DeletedDate] [datetime] NULL
)

Declare @SetupTabId int = (Select TOP 1 TabId From Tabs Where TabName='Setup')

--If facility is not equal to zero
If(@pFacilityId != 0)
Begin
	--Insert in the temp table from Module Access table where facility id equal to passing facility 
	Insert InTo @ModuleAccessTempTable
	Select * From ModuleAccess Where FacilityId = @pFacilityId 
	And TabID NOT IN (Select M.TabID From Tabs M Where M.TabId=@SetupTabId And M.ParentTabId=@SetupTabId)
End
Else
Begin
	--Insert in the temp table from Module Access table where corporate id equal to passing corporate
	Insert InTo @ModuleAccessTempTable 
	Select * From ModuleAccess Where CorporateID = @pCorporateId
	And TabID NOT IN (Select M.TabID From Tabs M Where M.TabId=@SetupTabId And M.ParentTabId=@SetupTabId)
End

--Delete data from Module Access where Module Access exist in temp table
Delete From ModuleAccess Where ModuleAccessID IN (Select ModuleAccessID From @ModuleAccessTempTable)

--Insert data from passing table type parameter to Module Access table
Insert InTo ModuleAccess ([CorporateID],[FacilityID],[TabID],[TabName],[IsDeleted],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[DeletedBy],[DeletedDate])
Select P.[CorporateID], P.[FacilityID], P.[TabID], (Select T.TabName From Tabs T Where T.TabId=TabID), P.[IsDeleted], 
P.[CreatedBy], P.[CreatedDate], P.[ModifiedBy], P.[ModifiedDate], P.[DeletedBy], P.[DeletedDate] 
From @pModuleAccessList P

--Temp table to store only Module Access Ids
Declare @ModuleAccessTempTableIds Table([ModuleAccessID] [int] NOT NULL,[CorporateID] [int] NULL,[FacilityID] [int] NULL,[CreatedBy] [int] NULL,[CreatedDate] [datetime] NULL,[TabID] [int] NULL)

If(@pFacilityId != 0)
Begin
	--Insert ids from Module Access table to temp table where facility id equals to the passing facility
	Insert InTo @ModuleAccessTempTableIds Select ModuleAccessId, CorporateID, FacilityID, CreatedBy, CreatedDate, TabID From ModuleAccess Where FacilityId = @pFacilityId
End
Else
Begin
	--Insert ids from Module Access table to temp table where facility is zero and corporate equals to the passing corporate id
	Insert InTo @ModuleAccessTempTableIds Select ModuleAccessId, CorporateID, FacilityID, CreatedBy, CreatedDate, TabID From ModuleAccess Where FacilityId = 0 And CorporateID = @pCorporateId
End

--Delete the records from the Module Access temp table
Delete From @ModuleAccessTempTable

---------------------------Cursor Start--------------------------------
Declare @cTabId Int
Declare @cCorporateId Int
Declare @cFacilityId Int
Declare @cCreatedBy Int
Declare @cCreatedDate DateTime
Declare TabsCursor Cursor For Select TabId, CorporateId, FacilityId, CreatedBy, CreatedDate From @ModuleAccessTempTableIds
OPEN TabsCursor 
Fetch Next From TabsCursor INTO @cTabId, @cCorporateId, @cFacilityId, @cCreatedBy, @cCreatedDate
While @@Fetch_Status = 0
Begin
Declare @Count Int
Select @Count = Count(TabId) From Tabs Where TabId = @cTabId
If(@count > 0)
Begin
	Insert InTo @ModuleAccessTempTable(TabID, CorporateID, FacilityID, CreatedBy, CreatedDate, TabName, DeletedBy, [DeletedDate], [ModuleAccessID])
	Select TOP 1 ParentTabId, @cCorporateId, @cFacilityId, @cCreatedBy, @cCreatedDate,
	(Case When (TabName Is Null OR TabName = '') Then (Select TabName From Tabs Where TabId = @cTabId) Else TabName End) TabName,
	Null, 0, 0 From Tabs Where TabId IN(Select ParentTabId From Tabs Where TabId = @cTabId And ParentTabId != 0)
End
Fetch Next From TabsCursor INTO @cTabId, @cCorporateId, @cFacilityId, @cCreatedBy, @cCreatedDate
End
Close TabsCursor 
Deallocate TabsCursor
---------------------------Cursor End-----------------------------------


Insert InTo ModuleAccess(TabID,CorporateID,FacilityID,CreatedBy,CreatedDate,TabName,DeletedBy,DeletedDate)
Select TabID,CorporateID, FacilityID, CreatedBy, CreatedDate, TabName, DeletedBy, [DeletedDate] From @ModuleAccessTempTable 
Where TabID NOT IN(Select TabID From @ModuleAccessTempTableIds)




