-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetTechnicalSpecifications','P') IS NOT NULL
   DROP PROCEDURE SprocGetTechnicalSpecifications
GO

CREATE Procedure [dbo].[SprocGetTechnicalSpecifications]  ---- SPROC_GetTechnicalSpecifications
(
@FacilityId int,
@CorporateId int
)
AS
BEGIN
	
	SELECT * From TechnicalSpecifications 
	WHERE FacilityId=@FacilityId and CorporateId=@CorporateId
END