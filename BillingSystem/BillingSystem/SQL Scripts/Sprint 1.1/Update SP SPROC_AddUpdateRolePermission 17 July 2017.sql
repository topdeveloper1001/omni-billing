IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SPROC_AddUpdateRolePermission') 
  DROP PROCEDURE SPROC_AddUpdateRolePermission;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[SPROC_AddUpdateRolePermission]
(
@pRoleTabsList RoleTabsTT readonly,
@pUId bigint,
@pCId bigint,
@pFId bigint
)
As
Begin
	--Truncate Table [RoleTabsTemp]
	--Insert into [RoleTabsTemp]
	--Select 0,RoleID,TabID from @pRoleTabsList
	--Declare role id parameter to store role id from the passing list
	Declare @RoleId Int

	--Get the top first role id from the list
	Select TOP 1 @RoleId = RoleID From @pRoleTabsList Where RoleID Is Not Null

	--Delete records from RoleTabs
	Delete From RoleTabs Where RoleID IN (@RoleId)

	Declare @Table Table(TabId Int, RoleTabId Int)

	---------------------------Cursor Start--------------------------------
	--Declare @cTabId Int
	--Declare @cRoleId Int
	--Declare @cRoleTabId Int
	--Declare TabsCursor Cursor For Select TabID, RoleID, ID From RoleTabsTemp

	--OPEN TabsCursor 
	--Fetch Next From TabsCursor INTO @cTabId, @cRoleId, @cRoleTabId
	--While @@Fetch_Status = 0
	--Begin
	--	Declare @icTabId Int
	--	Declare @icParentTabId Int
	--	Declare @icTabCount Int
	--	Declare @icParentTabCount Int
	--	Select TOP 1 @icTabId = TabID, @icParentTabId = ParentTabId From Tabs Where TabID = @cTabId
	--Insert into RoleTabs 
	--Select @icTabId, @cRoleId
	;WITH TabHierarchy AS
	(
	SELECT TabId, ParentTabId
	FROM Tabs 	
	WHERE TabId IN (Select TabID From @pRoleTabsList)
 
	UNION ALL
 
	SELECT F.TabId, F.ParentTabId
	FROM Tabs F
	INNER JOIN TabHierarchy FH ON FH.ParentTabId = F.TabId
	)
	
	INSERT INTO @Table
	Select TabId,@RoleId From TabHierarchy

	Declare @SetupTabId int = (Select TOP 1 TabId From Tabs Where TabName='Setup')


	If Exists (Select U.UserRoleID From UserRole U Where U.UserId=@pUId And U.RoleID=(Select TOP 1 R.RoleID From [Role] R Where R.RoleName='Sys Adm' And R.IsDeleted=0))
	Begin
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


		INSERT INTO @Table
		Select TabId,@RoleId From TabsHierarchy		
	End

	--Fetch Next From TabsCursor INTO @cTabId, @cRoleId, @cRoleTabId
	--End
	--Close TabsCursor 
	--Deallocate TabsCursor
	---------------------------Cursor End-----------------------------------

	Insert InTo RoleTabs(TabID, RoleID)
	select distinct TabId, RoleTabId from @Table

	--Insert InTo RoleTabs
	--Select RoleID, TabID From RoleTabsTemp
End





