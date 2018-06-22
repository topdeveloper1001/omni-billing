-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocSaveTab','P') IS NOT NULL
   DROP PROCEDURE SprocSaveTab
GO

CREATE PROCEDURE SprocSaveTab
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
--@pIsDeleted bit,
--@pIsVisible bit,
@pUserId bigint,
@pCurrentDate datetime,
@pCurrentRoleId bigint
)
As
Begin
	Declare @ExecutionStatus int=0

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

				INSERT INTO RoleTabs (RoleID,TabID)
				Select TOP 1 RoleId,@pId From [Role] Where RoleKey=1

				SET @ExecutionStatus=1
			End
			ELSE
			Begin
				Update Tabs SET TabName=@pTabName,Controller=@pController,[Action]=@pAction,RouteValues=@pRouteValues,TabOrder=@pTabOrder
				,TabImageUrl=@pTabImageUrl,IsActive=@pIsActive,ModifiedBy=@pUserId,ModifiedDate=@pCurrentDate Where TabId=@pId
				SET @ExecutionStatus=1

				IF NOT EXISTS (Select 1 From RoleTabs Where TabId=@pId And RoleID=(Select RoleId From [Role] Where RoleKey=1))
				Begin
					INSERT INTO RoleTabs (RoleID,TabID)
					Select TOP 1 RoleId,@pId From [Role] Where RoleKey=1

					SET @ExecutionStatus=1
				End	
			End

			SELECT @ExecutionStatus

			Exec SprocGetAllTabs @pUserId,1

			Exec SprocGetTabsByRole '',@pCurrentRoleId
		End
	END TRY
	BEGIN CATCH
		PRINT Error_Message()
		SET @ExecutionStatus=0
	END CATCH

	IF @ExecutionStatus <=0
		Select @ExecutionStatus
End