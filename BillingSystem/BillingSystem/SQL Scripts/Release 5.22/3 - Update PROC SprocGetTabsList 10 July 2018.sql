IF OBJECT_ID('SprocGetTabsList','P') IS NOT NULL
	DROP PROCEDURE [dbo].[SprocGetTabsList]
GO

/****** Object:  StoredProcedure [dbo].[SprocGetTabsList]    Script Date: 7/10/2018 2:34:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetTabsList]
(
	@pRId bigint=0,
	@pUId bigint=0,
	@pFId bigint=0,
	@pCId bigint=0,
	@pIsDeleted bit=0,
	@pIsActive bit=1,
	@pPortalKey int
)
AS
BEGIN
	
	--This is done temporarily
	Set @pFId=0


	/*
	Below is done only for Scheduler Tabs
	*/
	Declare @SchControllerName nvarchar(50)= 'Scheduler', @SchAction nvarchar(20)='index'

	If (Select TOP 1 UserName From [Users] Where UserID=@pUId) = 'sysadmin'
		SET @pCId=0

	--Hiding the other views under Scheduler to show
	Declare @SchedulerRouteValues nvarchar(10)='',@SchedulerTabId int
	Declare @SchedulerTabs Table (TabId int)

	INSERT INTO @SchedulerTabs
	Select T2.TabId From Tabs T2 
	Where T2.ParentTabId=(Select TOP 1 T.TabId From Tabs T Where T.Controller=@SchControllerName And T.[Action] = @SchAction)

	Select TOP 1 @SchedulerRouteValues=T.RouteValues
	From Tabs T Where T.TabId IN (Select R.TabId From RoleTabs R Where R.RoleID=@pRId And PortalKey=@pPortalKey)
	And T.IsActive=1 AND T.IsDeleted=0
	And T.TabId IN (Select TabId From @SchedulerTabs)

	;With TabsCustom
	As
	(
		Select T1.TabId
		From Tabs T1 Where T1.IsActive=@pIsActive And T1.IsDeleted=@pIsDeleted
		And T1.TabId IN (Select R.TabId From RoleTabs R Where R.RoleID=@pRId And R.PortalKey=@pPortalKey)
		And (
			@pCId=0
			OR
			(@pFId=0 And T1.TabId IN (Select M.TabId From ModuleAccess M Where M.CorporateID=@pCId))
			OR 
			T1.TabId IN (Select M.TabId From ModuleAccess M Where M.FacilityID=@pFId)
			)
		And T1.TabId NOT IN (Select * From @SchedulerTabs)
	)

	--List of Tabs Model
	Select T.[TabId]
      ,T.[TabName]
      ,T.[Controller]
      ,T.[Action]
      ,(Case WHEN
			T.Controller =@SchControllerName And T.[Action] = @SchAction
			THEN @SchedulerRouteValues 
			ELSE
			T.[RouteValues]
		END) As RouteValues
      ,T.[TabOrder]
      ,T.[TabImageUrl]
      ,T.[ParentTabId]
      ,T.[IsActive]
      ,T.[IsVisible]
      ,T.[IsDeleted]
      ,T.[ScreenID]
      ,T.[CreatedBy]
      ,T.[CreatedDate]
      ,T.[ModifiedBy]
      ,T.[ModifiedDate]
      ,T.[DeletedBy]
      ,T.[DeletedDate]
      ,T.[TabHierarchy] From Tabs T
	  Where T.TabId IN (Select C.TabId From TabsCustom C)
	  Order by T.TabOrder
	  --FOR JSON PATH,Root('Tabs'),Include_Null_Values
END
GO


