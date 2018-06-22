IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetAllTabs')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetAllTabs
GO

/****** Object:  StoredProcedure [dbo].[SprocGetAllTabs]    Script Date: 4/18/2018 8:28:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SprocGetAllTabs]
(
@pUserId bigint,
@pStatus bit
)
As
Begin
	Select T.[TabId] As 'CurrentTab.TabId'
	,T.[TabName] As 'CurrentTab.TabName'
	,T.[Controller] As 'CurrentTab.Controller'
	,T.[Action] As 'CurrentTab.Action'
	,T.[RouteValues] As 'CurrentTab.RouteValues'
	,T.[TabOrder] As 'CurrentTab.TabOrder'
	,T.[TabImageUrl] As 'CurrentTab.TabImageUrl'
	,T.[ParentTabId] As 'CurrentTab.ParentTabId'
	,T.[IsActive] As 'CurrentTab.IsActive'
	,T.[IsVisible] As 'CurrentTab.IsVisible'
	,T.[IsDeleted] As 'CurrentTab.IsDeleted'
	,T.[ScreenID] As 'CurrentTab.ScreenID'
	,T.[CreatedBy] As 'CurrentTab.CreatedBy'
	,T.[CreatedDate] As 'CurrentTab.CreatedDate'
	,T.[ModifiedBy] As 'CurrentTab.ModifiedBy'
	,T.[ModifiedDate] As 'CurrentTab.ModifiedDate'
	,T.[DeletedBy] As 'CurrentTab.DeletedBy'
	,T.[DeletedDate] As 'CurrentTab.DeletedDate'
	,T.[TabHierarchy] As 'CurrentTab.TabHierarchy'
	,ParentTabName=(Select TOP 1 T1.TabName From Tabs T1 Where T1.TabId=T.ParentTabId)
	,HasChilds=(Case WHEN Exists (Select 1 From Tabs T2 Where T2.ParentTabId=T.TabId And T2.IsActive=1 And T2.IsDeleted=0 And T2.IsVisible=1) 
				THEN Cast(1 As bit)
				ELSE Cast(0 As bit) END)
	From Tabs T Where T.IsActive=@pStatus And T.IsDeleted=0 And T.IsVisible=1
	And Exists (Select 1 From Users U Where U.UserID=@pUserId And U.IsActive=1 And ISNULL(U.IsDeleted,0)=0)
	For Json Path,Root('Tabs'),Include_null_values
End
GO


