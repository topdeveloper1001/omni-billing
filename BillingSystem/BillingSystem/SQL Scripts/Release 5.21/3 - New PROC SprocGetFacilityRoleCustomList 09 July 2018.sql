IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetFacilityRoleCustomList')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE SprocGetFacilityRoleCustomList
END
GO
CREATE PROCEDURE dbo.SprocGetFacilityRoleCustomList
( @pCID BIGINT, @pFID BIGINT, @pPortalId INT, @pRoleId INT=0,@pShowInActive BIT)
AS
BEGIN
		if @pRoleId=40
		BEGIN
			SET @pRoleId=0
		END

		Select * from (
		Select F.*,
		FacilityName=(Select Top 1 F1.FacilityName FROM Facility F1 WHERE F1.FacilityId=F.FacilityId),
		CorporateName=(Select Top 1 C.CorporateName FROM Corporate C WHERE C.CorporateId=F.CorporateId),
		RoleName=(Select Top 1 R.RoleName FROM [Role] R WHERE R.RoleId=F.RoleId)
		from FacilityRole F WHERE F.CorporateId=@pCID  AND F.FacilityId=@pFID
		AND (F.RoleId=@pRoleId or @pRoleId=0) AND F.IsActive=@pShowInActive
		AND F.RoleId IN (Select  R1.RoleId FROM [Role] R1 WHERE R1.PortalId=@pPortalId)
		) A
		Order By A.RoleName
		FOR JSON PATH, Root('Role'), INCLUDE_NULL_VALUES

END