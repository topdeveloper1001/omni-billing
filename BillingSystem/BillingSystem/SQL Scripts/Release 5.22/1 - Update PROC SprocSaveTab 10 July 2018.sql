IF OBJECT_ID('SprocSaveTab','P') IS NOT NULL
	DROP PROCEDURE [dbo].[SprocSaveTab]
GO

/****** Object:  StoredProcedure [dbo].[SprocSaveTab]    Script Date: 7/10/2018 12:26:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SprocSaveTab]
(
@pId int,
@pTabName nvarchar(200),
@pController nvarchar(100),
@pAction nvarchar(100),
@pRouteValues nvarchar(500),
@pTabOrder int,
@pTabImageUrl nvarchar(1000),
@pParentTabId int,
@pIsActive bit,
@pUserId bigint,
@pCurrentDate datetime,
@pCurrentRoleId bigint,
@pPortalKey int
)
As
Begin
	Declare @ExecutionStatus int=0
	--Declare @SysRoleId bigint=(Select TOP 1 RoleId From [Role] Where RoleKey='1')
	Declare @CId bigint=(Select TOP 1 CorporateId From Corporate Where CorporateName='ServicesDot')

	BEGIN TRY;
		IF Exists (Select 1 From Tabs Where TabName=@pTabName AND ParentTabId=@pParentTabId And IsActive=1 AND IsDeleted=0 And TabId !=@pId)
			SET @ExecutionStatus=-1

		IF @ExecutionStatus=0
		Begin
			IF @pId=0
			Begin

				INSERT INTO Tabs (TabName,Controller,[Action],RouteValues,TabOrder,TabImageUrl,ParentTabId,IsActive,IsDeleted,IsVisible,CreatedBy,CreatedDate,ScreenID)
				VALUES (@pTabName,@pController,@pAction,@pRouteValues,@pTabOrder,@pTabImageUrl,@pParentTabId,1,0,1,@pUserId,@pCurrentDate,1)

				SET @pId=SCOPE_IDENTITY()

				SET @ExecutionStatus=1

				INSERT INTO ModuleAccess 
				Select @CId,0,M.[TabID],M.[TabName],0,1,GETDATE(),NULL,NULL,NULL,NULL From Tabs M
				Where M.TabId=@pId
				
				Insert Into RoleTabs (RoleId,TabId,PortalKey,CreatedBy,CreatedDate)
				Select TOP 1 R.RoleId,@pId,@pPortalKey,@pUserId,@pCurrentDate From [Role] R
				Where Not Exists (Select 1 From RoleTabs R1 
								Where R1.PortalKey=@pPortalKey and R1.TabId=@pId And R1.RoleId=R.RoleId)
				And R.RoleKey='1'

				--INSERT INTO RoleTabs (RoleID,TabID)
				--Select TOP 1 RoleId,@pId From [Role] Where RoleKey=1

				--SET @ExecutionStatus=1
			End
			ELSE
			Begin
				Update Tabs SET TabName=@pTabName,Controller=@pController,[Action]=@pAction,RouteValues=@pRouteValues,TabOrder=@pTabOrder
				,TabImageUrl=@pTabImageUrl,IsActive=@pIsActive,ModifiedBy=@pUserId,ModifiedDate=@pCurrentDate,ParentTabId=@pParentTabId Where TabId=@pId
				
				SET @ExecutionStatus=1

				INSERT INTO ModuleAccess 
				Select @CId,0,M.[TabID],M.[TabName],0,1,GETDATE(),NULL,NULL,NULL,NULL From Tabs M
				Where M.TabId=@pId
				
				Insert Into RoleTabs (RoleId,TabId,PortalKey,CreatedBy,CreatedDate)
				Select TOP 1 R.RoleId,@pId,@pPortalKey,@pUserId,@pCurrentDate From [Role] R
				Where Not Exists (Select 1 From RoleTabs R1 
								Where R1.PortalKey=@pPortalKey and R1.TabId=@pId And R1.RoleId=R.RoleId)
				And R.RoleKey='1'

				--IF NOT EXISTS (Select 1 From RoleTabs Where TabId=@pId And RoleID=(Select RoleId From [Role] Where RoleKey=1))
				--Begin
				--	INSERT INTO RoleTabs (RoleID,TabID)
				--	Select TOP 1 RoleId,@pId From [Role] Where RoleKey=1

				--	SET @ExecutionStatus=1
				--End
			End

			SELECT @ExecutionStatus

			Exec SprocGetAllTabs @pUserId,1

			Exec SprocGetTabsByRole '',@pCurrentRoleId,@pPortalKey
		End
	END TRY
	BEGIN CATCH
		PRINT Error_Message()
		SET @ExecutionStatus=0
	END CATCH

	IF @ExecutionStatus <=0
		Select @ExecutionStatus
End
GO


