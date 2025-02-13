IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetFavoriteOrderById')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetFavoriteOrderById
GO
/****** Object:  StoredProcedure [dbo].[SPROC_GetMostOrdered]    Script Date: 3/28/2018 9:39:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 --Exec SprocGetFavoriteOrderById 437,1106,1269
CREATE PROCEDURE [dbo].SprocGetFavoriteOrderById 
(  
@pFavoriteId bigint,  
@pFacilityId bigint,
@pUserId bigint
)
AS  
BEGIN

	DECLARE @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))
	Declare @CategoryId bigint=0,@SubCategoryId bigint=0


	Select TOP 1 @CategoryId=O.CategoryId,@SubCategoryId=O.SubCategoryId From UserDefinedDescriptions U	
	INNER JOIN OpenOrder O ON U.CodeId=O.OrderCode AND U.CategoryId=O.OrderType
	Where O.FacilityId=@pFacilityId And ISNULL(O.IsActive,1)=1 AND ISNULL(O.IsDeleted,0)=0
	AND U.UserId=@pUserId
	Order by O.OpenOrderID Desc
	--- Query to Return a List as requested
  
	IF (@CategoryId > 0 AND @SubCategoryId > 0)
	Begin
		Select OrderStatus='1',CreatedDate=@LocalDateTime, DiagnosisCode=CodeId,OpenOrderPrescribedDate=@LocalDateTime
		,StartDate=@LocalDateTime,EndDate=@LocalDateTime,OrderCode=CodeId,CreatedBy=1,Quantity=0
		,OrderType=CategoryId
		,CategoryId=@CategoryId
		,SubCategoryId=@SubCategoryId
		from [dbo].UserDefinedDescriptions
		Where UserDefinedDescriptionID=@pFavoriteId
		FOR JSON PATH,ROOT('FavoriteResult'),Include_Null_values
	End
END












