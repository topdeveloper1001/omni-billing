IF OBJECT_ID('SPROC_AddUpdateRolePermission','P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPROC_AddUpdateRolePermission]
GO

/****** Object:  StoredProcedure [dbo].[SPROC_AddUpdateRolePermission]    Script Date: 7/10/2018 3:12:02 PM ******/
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
@pFId bigint,
@pPortalKey int=1
)
As
Begin
	Declare @SetupTabId int = (Select TOP 1 TabId From Tabs Where TabName='Setup')
	,@CurrentDatetime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFId))

	Declare @SetupTabs Table(TabId int)

	Delete From @SetupTabs

	INSERT INTO @SetupTabs
	Select TabId From Tabs Where TabId=@SetupTabId OR ParentTabId=@SetupTabId

	--Declare role id parameter to store role id from the passing list
	Declare @CurrentRoleId Int, @CurrentRoleKey int

	--Get the top first role id from the list
	Select TOP 1 @CurrentRoleId = R1.RoleID, @CurrentRoleKey=(Select TOP 1 R.RoleKey From [Role] R Where R.RoleID=R1.RoleID) From @pRoleTabsList R1 Where R1.RoleID Is Not Null

	--Delete records from RoleTabs
	Delete From RoleTabs Where RoleID IN (@CurrentRoleId) And (@CurrentRoleKey=1 OR (@CurrentRoleKey!=1 AND TabID NOT IN (Select TabID From @SetupTabs)))
	And PortalKey=@pPortalKey

	Declare @Table Table(TabId Int, RoleTabId Int)

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
	Select TabId,@CurrentRoleId From TabHierarchy

	If Exists (Select U.UserRoleID From UserRole U Where U.UserId=@pUId And U.RoleID=(Select TOP 1 R.RoleID From [Role] R Where R.RoleKey='1' And R.IsDeleted=0))
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
		SELECT distinct TabId,@CurrentRoleId From TabsHierarchy Where TabId NOT IN (Select TabId From @SetupTabs)
	End

	Insert InTo RoleTabs(TabID, RoleID,PortalKey,CreatedBy,CreatedDate)
	SELECT distinct TabId, RoleTabId,@pPortalKey,@pUId,@CurrentDatetime from @Table
End





GO


