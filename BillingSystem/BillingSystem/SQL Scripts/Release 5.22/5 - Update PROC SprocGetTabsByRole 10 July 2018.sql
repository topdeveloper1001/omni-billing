IF OBJECT_ID('SprocGetTabsByRole','P') IS NOT NULL
	DROP PROCEDURE [dbo].[SprocGetTabsByRole]
GO

/****** Object:  StoredProcedure [dbo].[SprocGetTabsByRole]    Script Date: 7/10/2018 3:43:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SprocGetTabsByRole]
(
@pUsername nvarchar(50),
@pRoleId bigint,
@pPortalKey int=1
)
As
Begin
	Select [TabId]
	,[TabName]
	,[Controller]
	,[Action]
	,[RouteValues]
	,[TabOrder]
	,[TabImageUrl]
	,[ParentTabId]
	,[IsActive]
	,[IsVisible]
	,[IsDeleted]
	,[ScreenID]
	,[CreatedBy]
	,[CreatedDate]
	,[ModifiedBy]
	,[ModifiedDate]
	,[DeletedBy]
	,[DeletedDate]
	,[TabHierarchy]
	From Tabs Where IsActive=1 And IsDeleted=0 And IsVisible=1
	AND TabId IN (Select R.TabId From RoleTabs R Where R.RoleID=@pRoleId And PortalKey=@pPortalKey) 
	Order by TabOrder
	For Json Path,Root('Tabs'),Include_null_values
End
GO


