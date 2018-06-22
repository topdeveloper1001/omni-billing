IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocGetTabsInRoleTabsView') 
  DROP PROCEDURE SprocGetTabsInRoleTabsView;
 
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
CREATE PROCEDURE SprocGetTabsInRoleTabsView
(
@RId bigint=0,
@UId bigint=0,
@FId bigint=0,
@CId bigint=0,
@IsDeleted bit=0,
@IsActive bit=1
)
AS
BEGIN

	--This is done temporarily
	Set @FId=0

	Declare @TabsCustom Table ([TabId] [int],
	[TabName] [nvarchar](100),
	[Controller] [varchar](100),
	[Action] [varchar](100),
	[RouteValues] [varchar](500),
	[TabOrder] [int],
	[TabImageUrl] [varchar](1000),
	[ParentTabId] [int],
	[IsActive] [bit],
	[IsVisible] [bit],
	[IsDeleted] [bit],
	[ScreenID] [int],
	[CreatedBy] [int],
	[CreatedDate] [datetime],
	[ModifiedBy] [int],
	[ModifiedDate] [datetime],
	[DeletedBy] [int],
	[DeletedDate] [datetime],
	[TabHierarchy] [nvarchar](500),ParentTabName [nvarchar](100),HasChilds bit)

	Declare @Tabs2 Table (TabRId bigint)
	Delete From @Tabs2

	INSERT INTO @Tabs2
	Select M.TabId From ModuleAccess M Where ISNULL(M.IsDeleted,0)=0 
	And M.CorporateID=@CId
	AND (@FId=0 OR M.FacilityID=@FId)
	And (@RId=0 OR M.TabID IN (Select R.TabID From RoleTabs R Where R.RoleID=@RId And ISNULL(IsDeleted,0)=0))
	
	INSERT INTO @TabsCustom
	Select T1.*
		,(Case When T1.ParentTabId > 0 
				Then (Select T2.TabName From Tabs T2 Where T2.TabId=T1.ParentTabId) 
			   Else ''
		  End) ParentTabName
		,(Case When Exists (Select Count(1) From Tabs T3 Where T3.ParentTabId=T1.TabId)
				Then Cast(1 As bit)
			   Else Cast(0 As bit)
		  End) HasChilds
		From Tabs T1 Where T1.IsActive=@IsActive And T1.IsDeleted=@IsDeleted
		And T1.TabName != 'Setup'
		And T1.TabId IN (Select TabRId From @Tabs2)
		Order by T1.TabOrder

	--List of Tabs Model
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
      ,[TabHierarchy] From @TabsCustom
	  Order by TabOrder


	--List of TabsCustom Model 
	  Select  Cast(TabId as nvarchar) TabId,ISNULL(ParentTabName,'') ParentTabName,HasChilds From @TabsCustom
END
GO
