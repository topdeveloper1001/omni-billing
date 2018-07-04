IF OBJECT_ID('SprocGetDashboardIndicatorsByIndicatorNumber','P') IS NOT NULL
	DROP PROCEDURE SprocGetDashboardIndicatorsByIndicatorNumber
GO

/****** Object:  StoredProcedure [dbo].[SPROC_SortDashboardIndicatorCols]    Script Date: 6/27/2018 2:46:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Exec SprocGetDashboardIndicatorsByIndicatorNumber 'IndicatorNumber','ASC',1016,0,1
CREATE Procedure [dbo].[SprocGetDashboardIndicatorsByIndicatorNumber]
(
	@pCID bigint=NULL,
	@PValue nvarchar(50)=NULL,
	@PId bigint=0
)
As
Begin
	Declare @KpiSubCategories1 nvarchar(5)='4347',@KpiSubCategories2 nvarchar(5)='4351'

	select DI.IndicatorId, DI.IndicatorNumber, DI.Dashboard, DI.[Description],DI.Defination
	,SubCategoryFirst =
	(
		Case When ISNULL(DI.SubCategory1,'') !='' AND ISNULL(DI.SubCategory1,'0') !='0'
		THEN (Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.SubCategory1 And G.GlobalCodeCategoryValue=@KpiSubCategories1) 
		ELSE '' END
	)
	,SubCategorySecond =
	(
		Case When ISNULL(DI.SubCategory1,'') !='' AND IsNumeric(ISNULL(DI.SubCategory1,'a'))=1 AND ISNULL(DI.SubCategory2,'') !=''
		THEN 
		(
			Select TOP 1 G.GlobalCodeName From GlobalCodes G 
			Where G.GlobalCodeValue=DI.SubCategory2 And G.GlobalCodeCategoryValue=@KpiSubCategories2
		)
		ELSE '' END
	)
	,FormatTypeStr=(Select TOP 1 G1.GlobalCodeName From GlobalCodes G1 Where G1.GlobalCodeCategoryValue='4343' And G1.GlobalCodeValue=DI.FormatType)
	,DI.DecimalNumbers
	,FerquencyTypeStr=(Select TOP 1 G1.GlobalCodeName From GlobalCodes G1 Where G1.GlobalCodeCategoryValue='4344' And G1.GlobalCodeValue=DI.FerquencyType)
	,DI.[OwnerShip], DI.[FacilityId],DI.[CorporateId],DI.CreatedBy,DI.CreatedDate,DI.[IsActive],DI.[ExternalValue1],DI.[ExternalValue2]
	,DI.[ExternalValue3], DI.[ReferencedIndicators], DI.[ExternalValue4], DI.[ExternalValue5], DI.[ExternalValue6],DI.[SortOrder]
	,DI.[ExpressionText], DI.[ExpressionValue], DI.[SpecialCase]
	,TypeOFData=
	(
		Case WHEN ISNULL(DI.ExternalValue3,'') !='' THEN 
		(Select TOP 1 G1.GlobalCodeName From GlobalCodes G1 Where G1.GlobalCodeCategoryValue='4407' And G1.GlobalCodeValue=DI.ExternalValue3)
		ELSE '' 
		END
	)
	,FacilityNameStr=(Select TOP 1 FacilityName From Facility Where FacilityId=DI.FacilityId)
	,UsernameStr=(Select TOP 1 U.UserName From Users U Where U.UserID=DI.CreatedBy)
	FROM DashboardIndicators AS DI
	Where (ISNULL(@PValue,'')='' OR DI.IndicatorNumber=@PValue)
	AND DI.CorporateId=@pCID
	AND (@PId=0 OR DI.IndicatorID=@PId)
	FOR JSON Path,Root('DashboardResult'),Include_Null_Values
End


