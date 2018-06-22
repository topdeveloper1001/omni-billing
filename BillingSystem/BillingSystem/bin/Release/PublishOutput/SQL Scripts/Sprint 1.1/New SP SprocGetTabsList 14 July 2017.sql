IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocGetTabsList') 
  DROP PROCEDURE SprocGetTabsList;
 
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
CREATE PROCEDURE SprocGetTabsList
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

	If (Select TOP 1 UserName From [Users] Where UserID=@UId) = 'sysadmin'
		SET @CId=0


	;With TabsCustom
	As
	(
	Select T1.TabId
	From Tabs T1 Where T1.IsActive=@IsActive And T1.IsDeleted=@IsDeleted
	And T1.TabId IN (Select R.TabId From RoleTabs R Where R.RoleID=@RId)
	And (
		@CId=0 
		OR
		(@FId=0 And T1.TabId IN (Select M.TabId From ModuleAccess M Where M.CorporateID=@CId)) 
		OR 
		T1.TabId IN (Select M.TabId From ModuleAccess M Where M.FacilityID=@FId)
		)
	)

	--List of Tabs Model
	Select T.[TabId]
      ,T.[TabName]
      ,T.[Controller]
      ,T.[Action]
      ,T.[RouteValues]
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
END
GO
