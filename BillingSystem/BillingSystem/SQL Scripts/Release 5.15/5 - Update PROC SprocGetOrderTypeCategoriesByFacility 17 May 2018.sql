IF OBJECT_ID('SprocGetOrderTypeCategoriesByFacility','P') IS NOT NULL
   DROP PROCEDURE SprocGetOrderTypeCategoriesByFacility
GO

/****** Object:  StoredProcedure [dbo].[SprocGetOrderTypeCategoriesByFacility]    Script Date: 5/17/2018 6:22:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--Exec SprocGetOrderTypeCategoriesByFacility 1269,1106,1
CREATE PROCEDURE [dbo].[SprocGetOrderTypeCategoriesByFacility]
(
	@pUserId bigint,
	@pFacilityId bigint,
	@pStatusId bit=1
)
As
Begin
	Select G.GlobalCodeCategoryID,G.GlobalCodeCategoryName,G.GlobalCodeCategoryValue, G.CreatedBy
	,G.ExternalValue1,G.ExternalValue2,G.ExternalValue3,G.IsActive,G.FacilityNumber
	,ExternalValue4=
	(
		   STUFF((SELECT distinct ', ' + cast(G2.GlobalCodeCategoryName as varchar(500))
           FROM dbo.Split(',',G.ExternalValue4) G1
		   INNER JOIN GlobalCodeCategory G2 ON G2.GlobalCodeCategoryValue=G1.IDValue 
		   AND G2.FacilityNumber='0' AND G2.ExternalValue3='OrderCategory'
           FOR XML PATH('')),1,1,'')
	)
	From GlobalCodeCategory G
	Where G.IsActive=@pStatusId And ISNULL(G.IsDeleted,0)=0
	And G.FacilityNumber=@pFacilityId
	AND G.ExternalValue3='OrderCategory'
	FOR JSON PATH,Root('GlobalCategory'),INCLUDE_NULL_VALUES
End
GO


