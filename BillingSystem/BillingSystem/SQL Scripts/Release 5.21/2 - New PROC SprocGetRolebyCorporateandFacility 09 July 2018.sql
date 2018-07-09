IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetRolebyCorporateandFacility')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE SprocGetRolebyCorporateandFacility
END
GO
CREATE PROCEDURE dbo.SprocGetRolebyCorporateandFacility
( @pCID BIGINT, @pFID BIGINT, @pPortalId INT)
AS
BEGIN
		SELECT R.RoleID,R.RoleName
		FROM FacilityRole F
		INNER JOIN [Role] R on F.RoleId=R.RoleID AND R.PortalId=@pPortalId
		WHERE F.CorporateId = @pCID AND F.FacilityId = @pFID
		FOR JSON PATH, Root('Role'), INCLUDE_NULL_VALUES

END