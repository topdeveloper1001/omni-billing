-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetCategories','P') IS NOT NULL
   DROP PROCEDURE SprocGetCategories
GO

CREATE Procedure [dbo].[SprocGetCategories]  ---- SPROC_GetCategories
AS
BEGIN
	
	SELECT * From Categories
END