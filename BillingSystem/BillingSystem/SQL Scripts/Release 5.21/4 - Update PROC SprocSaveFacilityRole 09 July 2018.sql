IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'SprocSaveFacilityRole') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE SprocSaveFacilityRole
END
GO

CREATE PROCEDURE [dbo].[SprocSaveFacilityRole] 
(@FRoleId BIGINT, @FId BIGINT, @RId BIGINT, @CId BIGINT, @UId BIGINT, @AddToAll BIT, @IsDeleted BIT = 0, @SchedulingApplied BIT = 0, @CarePlanApplied BIT = 0, 
@RName NVARCHAR(100), @CurrentDate DATETIME, @pPortalId INT)
AS
BEGIN
	DECLARE @RoleTemp TABLE (RoleId INT, FacilityId INT, CId INT)
	DECLARE @NewRoleKey INT = 0
	DECLARE @SetupCorporateId INT, @SetupFacilityId INT, @IsGeneric BIT = 0

	SELECT TOP 1 @SetupCorporateId = R.CorporateId, @SetupFacilityId = R.FacilityId
	FROM [Role] R
	WHERE R.RoleKey = '1'

	IF @CId = @SetupCorporateId
		SET @IsGeneric = 1

	SELECT TOP 1 @NewRoleKey = RoleKey
	FROM [Role]
	WHERE RoleName = @RName

	IF ISNULL(@NewRoleKey, 0) = 0
		SELECT @NewRoleKey = (MAX(Cast(RoleKey AS INT)) + 1)
		FROM [Role]
		WHERE IsActive = 1 AND IsDeleted = 0

	IF @RId = 0 AND @RName != ''
	BEGIN
		INSERT INTO [Role]
		OUTPUT inserted.RoleID, inserted.FacilityId, inserted.CorporateId
		INTO @RoleTemp(RoleId, FacilityId, CId)
		SELECT DISTINCT 1 AS IsActive, @RName AS RoleName, @UId AS CreatedBy, @CurrentDate AS CreatedDate, NULL, NULL, 0 AS IsDeleted, NULL, NULL, F.CorporateID, F.FacilityId, 0 AS IsGeneric, @NewRoleKey AS RoleKey, @pPortalId
		FROM Facility F
		WHERE F.CorporateID = @CId AND F.IsActive = 1 AND F.IsDeleted = 0 AND F.FacilityId NOT IN (
				SELECT R.FacilityId
				FROM [Role] R
				WHERE R.RoleName = @RName AND R.CorporateId = @CId AND R.IsActive = 1 AND R.IsDeleted = 0
				) AND ((@AddToAll = 0 AND F.FacilityId = @FId) OR @AddToAll = 1)

		IF EXISTS (
				SELECT 1
				FROM @RoleTemp
				)
		BEGIN
			INSERT INTO FacilityRole (RoleId, CorporateId, FacilityId, CreatedBy, CreatedDate, IsDeleted, IsActive, SchedulingApplied, CarePlanAccessible)
			SELECT DISTINCT RoleId, CId, FacilityId, @UId, @CurrentDate, 0 AS IsDeleted, 1 AS IsActive, @SchedulingApplied, @CarePlanApplied
			FROM @RoleTemp
		END
	END

	DELETE
	FROM @RoleTemp

	IF @FRoleId > 0
	BEGIN
		UPDATE FacilityRole
		SET RoleId = @RId, FacilityId = @FId, SchedulingApplied = @SchedulingApplied, CarePlanAccessible = @CarePlanApplied, ModifiedBy = @UId, ModifiedDate = @CurrentDate
		WHERE FacilityRoleId = @FRoleId

		IF @AddToAll = 1 AND @RId > 0
		BEGIN
			SELECT @RName = RoleName
			FROM [Role]
			WHERE RoleID = @RId

			INSERT INTO [Role]
			OUTPUT inserted.RoleID, inserted.FacilityId, inserted.CorporateId
			INTO @RoleTemp(RoleId, FacilityId, CId)
			SELECT DISTINCT 1 AS IsActive, @RName AS RoleName, @UId AS CreatedBy, @CurrentDate AS CreatedDate, @UId AS ModifiedBy, @CurrentDate AS ModifiedDate, 0 AS IsDeleted, NULL, NULL, F.CorporateID, F.FacilityId, 0 AS IsGeneric, @NewRoleKey AS RoleKey, @pPortalId
			FROM Facility F
			WHERE F.CorporateID = @CId AND F.IsActive = 1 AND F.IsDeleted = 0 AND F.FacilityId NOT IN (
					SELECT R.FacilityId
					FROM [Role] R
					WHERE R.RoleName = @RName AND R.CorporateId = @CId AND R.IsActive = 1 AND R.IsDeleted = 0
					)

			IF EXISTS (
					SELECT 1
					FROM @RoleTemp
					)
			BEGIN
				INSERT INTO FacilityRole (RoleId, CorporateId, FacilityId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsDeleted, DeletedBy, DeletedDate, IsActive, SchedulingApplied, CarePlanAccessible)
				SELECT DISTINCT RoleId, CId, FacilityId, @UId, @CurrentDate, @UId AS ModifiedBy, @CurrentDate AS ModifiedDate, 0 AS IsDeleted, NULL, NULL, 1 AS IsActive, @SchedulingApplied, @CarePlanApplied
				FROM @RoleTemp
			END
		END
	END
END