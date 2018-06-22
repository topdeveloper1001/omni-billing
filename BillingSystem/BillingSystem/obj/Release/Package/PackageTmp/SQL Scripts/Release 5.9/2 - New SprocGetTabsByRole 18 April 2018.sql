IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetTabsByRole')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetTabsByRole
GO

/****** Object:  StoredProcedure [dbo].[SprocGetTabsByRole]    Script Date: 4/18/2018 8:28:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SprocGetTabsByRole]
(
@pUsername nvarchar(50),
@pRoleId bigint
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
	AND TabId IN (Select R.TabId From RoleTabs R Where R.RoleID=@pRoleId) 
	--And Exists (Select 1 From Users U Where U.UserID=@pUserId And U.IsActive=1 And ISNULL(U.IsDeleted,0)=0)
	Order by TabOrder
	For Json Path,Root('Tabs'),Include_null_values
End
GO


