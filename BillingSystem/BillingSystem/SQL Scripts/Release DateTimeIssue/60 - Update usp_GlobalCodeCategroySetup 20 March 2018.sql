IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'usp_GlobalCodeCategroySetup')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE usp_GlobalCodeCategroySetup
GO

/****** Object:  StoredProcedure [dbo].[usp_GlobalCodeCategroySetup]    Script Date: 21-03-2018 11:25:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GlobalCodeCategroySetup]
(
 @pCreatedBy INT
,@pGlobalCodeCategoryList VARCHAR(MAX)
,@pFacilityId VARCHAR(MAX)
)
AS
BEGIN
DECLARE @CreatedDate DATETIME=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))
	INSERT INTO GlobalCodeCategory (FacilityNumber,GlobalCodeCategoryValue,GlobalCodeCategoryName,CreatedBy, CreatedDate, IsActive)
	SELECT @pFacilityId AS FacilityNumber,GlobalCodeCategoryValue,GlobalCodeCategoryName,@pCreatedBy AS CreatedBy,  @CreatedDate AS CreatedDate, 1 AS IsActive  FROM GlobalCodeCategory
	WHERE GlobalCodeCategoryID IN (SELECT IDValue FROM dbo.Split(',',@pGlobalCodeCategoryList))
END













GO


