IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_AddUpdateModuleAccess')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_AddUpdateModuleAccess
GO

/****** Object:  StoredProcedure [dbo].[SPROC_AddUpdateModuleAccess]    Script Date: 21-03-2018 10:35:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_AddUpdateModuleAccess]
(
    @pModuleAccessList ModuleAccessTT readonly,
	@pCorporateId Int,
	@pFacilityId Int,
	@pLoggedInUserId int=1,
	@pCurrentDate DateTime=''
)
AS
Begin

	--Temp table to store full Module Access data
	Declare @ModuleAccessTempTable Table
	(	
		[ModuleAccessID]	[int] NOT NULL,
		[CorporateID]		[int] NULL,
		[FacilityID]		[int] NULL,
		[TabID]				[int] NULL
		--,[TabName] [nvarchar](100) NULL,
		--[IsDeleted] [bit] NULL,
		--[CreatedBy] [int] NULL,
		--[CreatedDate] [datetime] NULL,
		--[ModifiedBy] [int] NULL,
		--[ModifiedDate] [datetime] NULL,
		--[DeletedBy] [int] NULL,
		--[DeletedDate] [datetime] NULL
	)

	Delete From @ModuleAccessTempTable

	INSERT INTO @ModuleAccessTempTable
	SELECT 0,@pCorporateId,@pFacilityId,TabID From @pModuleAccessList

	Declare @SetupTabId int = (Select TOP 1 TabId From Tabs Where TabName='Setup')
	
	If Exists (Select U.UserRoleID From UserRole U Where U.UserId=@pLoggedInUserId And U.RoleID=(Select TOP 1 R.RoleID From [Role] R Where R.RoleName='Sys Adm' And R.IsDeleted=0))
	Begin
		Declare @SetupTabs Table (TabId int)
		
		;WITH TabsHierarchy AS
		(
			SELECT TabId, ParentTabId, TabName, 1 AS 'Level'
			FROM Tabs
			WHERE TabId = @SetupTabId And IsDeleted=0 And IsActive=1 And IsVisible=1
			UNION ALL
			SELECT F.TabId, F.ParentTabId, F.TabName, 
			FH.Level + 1 AS 'Level'
			FROM Tabs F
			INNER JOIN TabsHierarchy FH ON FH.TabId = F.ParentTabId
			Where F.IsDeleted=0 And F.IsActive=1 And F.IsVisible=1
		)

		--SELECT TabId, ParentTabId, TabName, 
		--[Level] = CAST([Level] as int) 
		--FROM FileTreeHierarchy Order By [Level] Desc 


		INSERT INTO @ModuleAccessTempTable ([ModuleAccessID],[CorporateID],[FacilityID],[TabID])
		Select 0,@pCorporateId,@pFacilityId,TabId From TabsHierarchy		
	End
	

	If @pCurrentDate=''
	Begin
		SET @pCurrentDate=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))
	End


	
	----If facility is not equal to zero
	--If(@pFacilityId != 0)
	--Begin
	--	--Insert in the temp table from Module Access table where facility id equal to passing facility 
	--	Insert InTo @ModuleAccessTempTable
	--	Select * From ModuleAccess Where FacilityId = @pFacilityId 
	--	And TabID NOT IN (Select M.TabID From Tabs M Where M.TabId=@SetupTabId And M.ParentTabId=@SetupTabId)
	--End
	--Else
	--Begin
	--	--Insert in the temp table from Module Access table where corporate id equal to passing corporate
	--	Insert InTo @ModuleAccessTempTable 
	--	Select * From ModuleAccess Where CorporateID = @pCorporateId
	--	And TabID NOT IN (Select M.TabID From Tabs M Where M.TabId=@SetupTabId And M.ParentTabId=@SetupTabId)
	--End

	--Delete data from Module Access where Module Access exist in temp table
	Delete From ModuleAccess Where CorporateID=@pCorporateId

	--Insert data from passing table type parameter to Module Access table
	Insert InTo ModuleAccess ([CorporateID],[FacilityID],[TabID],[TabName],[IsDeleted],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[DeletedBy],[DeletedDate])
	Select P.[CorporateID], P.[FacilityID], P.[TabID], (Select TOP 1 T.TabName From Tabs T Where T.TabId=P.TabID) As [TabName], 0 As [IsDeleted], 
	@pLoggedInUserId, @pCurrentDate, null, null, null, null
	From @ModuleAccessTempTable P



	----Temp table to store only Module Access Ids
	--Declare @ModuleAccessTempTableIds Table([ModuleAccessID] [int] NOT NULL,[CorporateID] [int] NULL,[FacilityID] [int] NULL,[CreatedBy] [int] NULL,[CreatedDate] [datetime] NULL,[TabID] [int] NULL)

	--If(@pFacilityId != 0)
	--Begin
	--	--Insert ids from Module Access table to temp table where facility id equals to the passing facility
	--	Insert InTo @ModuleAccessTempTableIds Select ModuleAccessId, CorporateID, FacilityID, CreatedBy, CreatedDate, TabID From ModuleAccess Where FacilityId = @pFacilityId
	--End
	--Else
	--Begin
	--	--Insert ids from Module Access table to temp table where facility is zero and corporate equals to the passing corporate id
	--	Insert InTo @ModuleAccessTempTableIds Select ModuleAccessId, CorporateID, FacilityID, CreatedBy, CreatedDate, TabID From ModuleAccess Where FacilityId = 0 And CorporateID = @pCorporateId
	--End

	----Delete the records from the Module Access temp table
	--Delete From @ModuleAccessTempTable


	-----------------------------Cursor Start--------------------------------
	--Declare @cTabId Int
	--Declare @cCorporateId Int
	--Declare @cFacilityId Int
	--Declare @cCreatedBy Int
	--Declare @cCreatedDate DateTime
	--Declare TabsCursor Cursor For Select TabId, CorporateId, FacilityId, CreatedBy, CreatedDate From @ModuleAccessTempTableIds
	--OPEN TabsCursor 
	--Fetch Next From TabsCursor INTO @cTabId, @cCorporateId, @cFacilityId, @cCreatedBy, @cCreatedDate
	--While @@Fetch_Status = 0
	--Begin
	--Declare @Count Int
	--Select @Count = Count(TabId) From Tabs Where TabId = @cTabId
	--If(@count > 0)
	--Begin
	--	Insert InTo @ModuleAccessTempTable(TabID, CorporateID, FacilityID, CreatedBy, CreatedDate, TabName, DeletedBy, [DeletedDate], [ModuleAccessID])
	--	Select TOP 1 ParentTabId, @cCorporateId, @cFacilityId, @cCreatedBy, @cCreatedDate,
	--	--(Case When ISNULL(TabName,'')='' Then (Select TabName From Tabs Where TabId = @cTabId) Else TabName End) TabName,
	--	(Select TabName From Tabs Where TabId = @cTabId) TabName,
	--	Null, 0, 0 From Tabs Where TabId IN (Select ParentTabId From Tabs Where TabId = @cTabId And ParentTabId != 0)
	--End
	--Fetch Next From TabsCursor INTO @cTabId, @cCorporateId, @cFacilityId, @cCreatedBy, @cCreatedDate
	--End
	--Close TabsCursor 
	--Deallocate TabsCursor
	-----------------------------Cursor End-----------------------------------


	--Insert InTo ModuleAccess(TabID,CorporateID,FacilityID,CreatedBy,CreatedDate,TabName,DeletedBy,DeletedDate)
	--Select TabID,CorporateID, FacilityID, @pLoggedInUserId, @pCurrentDate, TabName, DeletedBy, [DeletedDate] 
	--From @ModuleAccessTempTable
	--Where TabID NOT IN (Select TabID From @ModuleAccessTempTableIds)
End


GO


