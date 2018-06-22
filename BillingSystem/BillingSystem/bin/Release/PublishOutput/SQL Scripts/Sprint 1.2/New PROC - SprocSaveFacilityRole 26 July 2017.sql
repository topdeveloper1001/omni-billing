IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocSaveFacilityRole') 
  DROP PROCEDURE SprocSaveFacilityRole;
 
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
CREATE PROCEDURE SprocSaveFacilityRole
(
@FRoleId bigint,
@FId bigint,
@RId bigint,
@CId bigint,
@UId bigint,
@AddToAll bit,
@IsDeleted bit=0,
@SchedulingApplied bit=0,
@CarePlanApplied bit=0,
@RName nvarchar(100),
@CurrentDate datetime
)
AS
BEGIN	
	Declare @RoleTemp Table (RoleId int, FacilityId int, CId int)

	Declare @NewRoleKey int = 0
	Declare @SetupCorporateId int,@SetupFacilityId int, @IsGeneric bit=0
		
	Select TOP 1 @SetupCorporateId = R.CorporateId, @SetupFacilityId=R.FacilityId From [Role] R Where R.RoleKey = '1'

	If @CId=@SetupCorporateId
		Set @IsGeneric=1

	Select TOP 1 @NewRoleKey = RoleKey From [Role] Where RoleName = @RName

	IF ISNULL(@NewRoleKey,0)=0
		Select @NewRoleKey= (MAX(Cast(RoleKey as int)) + 1) From [Role] Where IsActive=1 And IsDeleted=0

	If @RId=0 AND @RName !=''
	Begin
		INSERT INTO [Role]
		OUTPUT inserted.RoleID, inserted.FacilityId, inserted.CorporateId INTO @RoleTemp (RoleId,FacilityId,CId)
		Select DISTINCT 1 As IsActive, @RName As RoleName,@UId As CreatedBy,@CurrentDate As CreatedDate,NULL,NULL,0 As IsDeleted,NULL,NULL,F.CorporateID,F.FacilityId,0 As IsGeneric
		,@NewRoleKey As RoleKey
		From Facility F
		Where F.CorporateID = @CId And F.IsActive=1 And F.IsDeleted=0
		And F.FacilityId NOT IN (Select R.FacilityId From [Role] R Where R.RoleName=@RName And R.CorporateId=@CId And R.IsActive=1 And R.IsDeleted=0)
		And ((@AddToAll=0 And F.FacilityId = @FId) OR @AddToAll=1)

		IF Exists (Select 1 From @RoleTemp)
		Begin
			INSERT INTO FacilityRole (RoleId,CorporateId,FacilityId,CreatedBy
			,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,DeletedBy,DeletedDate,IsActive
			,SchedulingApplied,CarePlanAccessible)
			Select DISTINCT RoleId,CId,FacilityId,@UId,@CurrentDate,NULL,NULL,0 As IsDeleted,NULL,NULL,1 As IsActive
			,@SchedulingApplied,@CarePlanApplied
			From @RoleTemp
		End
	End
	
	Delete From @RoleTemp

	If @FRoleId > 0
	Begin
		Update FacilityRole Set RoleId = @RId, FacilityId = @FId Where FacilityRoleId=@FRoleId

		IF @AddToAll=1 And @RId > 0
		Begin
			Select @RName = RoleName From [Role] Where RoleID=@RId
			INSERT INTO [Role]
			OUTPUT inserted.RoleID, inserted.FacilityId, inserted.CorporateId INTO @RoleTemp (RoleId,FacilityId,CId)
			Select DISTINCT 1 As IsActive, @RName As RoleName,@UId As CreatedBy,@CurrentDate As CreatedDate,@UId As ModifiedBy,@CurrentDate As ModifiedDate,0 As IsDeleted,NULL,NULL,F.CorporateID,F.FacilityId,0 As IsGeneric
			,@NewRoleKey As RoleKey
			From Facility F
			Where F.CorporateID = @CId And F.IsActive=1 And F.IsDeleted=0
			And F.FacilityId NOT IN (Select R.FacilityId From [Role] R Where R.RoleName=@RName And R.CorporateId=@CId And R.IsActive=1 And R.IsDeleted=0)

			IF Exists (Select 1 From @RoleTemp)
			Begin
				INSERT INTO FacilityRole (RoleId,CorporateId,FacilityId,CreatedBy
				,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,DeletedBy,DeletedDate,IsActive
				,SchedulingApplied,CarePlanAccessible)
				Select DISTINCT RoleId,CId,FacilityId,@UId,@CurrentDate,@UId As ModifiedBy,@CurrentDate As ModifiedDate,0 As IsDeleted,NULL,NULL,1 As IsActive
				,@SchedulingApplied,@CarePlanApplied
				From @RoleTemp
			End
		End
	End
END
GO
