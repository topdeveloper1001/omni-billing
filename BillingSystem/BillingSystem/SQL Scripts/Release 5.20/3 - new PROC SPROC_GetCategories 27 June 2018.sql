-- Drop stored procedure if it already exists
IF OBJECT_ID('SPROC_GetCategories','P') IS NOT NULL
   DROP PROCEDURE SPROC_GetCategories
GO

CREATE Procedure [dbo].[SPROC_GetCategories]  ---- SPROC_GetCategories
AS
BEGIN
	
	SELECT Id, ProdCatNumber, ProdCat, ProdSubcat, ProdSubcat2, ProdSubcat3, CreatedBy From Categories
END