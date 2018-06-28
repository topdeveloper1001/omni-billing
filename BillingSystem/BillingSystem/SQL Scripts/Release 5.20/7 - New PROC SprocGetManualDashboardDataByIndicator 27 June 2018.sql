
IF OBJECT_ID('SprocGetManualDashboardDataByIndicator ','P') IS NOT NULL
	DROP PROCEDURE SprocGetManualDashboardDataByIndicator 
GO

/****** Object:  StoredProcedure [dbo].[SPROC_SortDashboardIndicatorCols]    Script Date: 6/27/2018 2:46:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Exec SprocGetManualDashboardDataByIndicator 1016,0 'IndicatorNumber','ASC',1016,0,1
CREATE Procedure [dbo].[SprocGetManualDashboardDataByIndicator]
(
	@pCID bigint=NULL,
	@pFID bigint=Null,
	@pYear nvarchar(5)=NULL,
	@pValue nvarchar(50)=NULL,
	@pBudgetType nvarchar(10)=NULL,
	@pCategory1 nvarchar(10)=NULL,
	@pCategory2 nvarchar(10)=NULL
)
As
Begin
	Declare @KpiSubCategories1 nvarchar(5)='4347',@KpiSubCategories2 nvarchar(5)='4351',@DashboardType nvarchar(5)= '4401'
	
	SELECT DI.*
	,DashboardTypeStr=
	(
		Case WHEN ISNULL(DI.DashboardType,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.DashboardType And G.GlobalCodeCategoryValue='4345')
		ELSE '' END
	)
	,BudgetTypeStr=
	(
		Case WHEN ISNULL(DI.BudgetType,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.BudgetType And G.GlobalCodeCategoryValue='3112')
		ELSE '' END
	)
	,KPICategoryTypeStr=
	(
		Case WHEN ISNULL(DI.KPICategory,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.KPICategory And G.GlobalCodeCategoryValue='4346')
		ELSE '' END
	)
	,FrequencyTypeStr=
	(
		Case WHEN ISNULL(DI.Frequency,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.Frequency And G.GlobalCodeCategoryValue='4344')
		ELSE '' END
	)
	,IndicatorTypeStr=
	(
		Case WHEN ISNULL(DI.Indicators,'') !='' THEN
		(Select TOP 1 G.[Description] From DashboardIndicators G Where G.IndicatorNumber=DI.Indicators And G.CorporateId=@pCID)
		ELSE '' END
	)
	,FacilityStr=(Select TOP 1 F.FacilityName From Facility F Where F.FacilityId=DI.FacilityId)
	,CorporateStr=(Select TOP 1 F.CorporateName From Corporate F Where F.CorporateId=DI.CorporateId)
	,SubCategoryValue1Str =
	(
		Case When ISNULL(DI.SubCategory1,'') !='' AND ISNULL(DI.SubCategory1,'0') !='0'
		THEN (Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.SubCategory1 And G.GlobalCodeCategoryValue=@KpiSubCategories1) 
		ELSE '' END
	)
	,SubCategoryValue2Str =
	(
		Case When ISNULL(DI.SubCategory1,'') !='' AND IsNumeric(ISNULL(DI.SubCategory1,'a'))=1 AND ISNULL(DI.SubCategory2,'') !=''
		THEN 
		(
			Select TOP 1 G.GlobalCodeName From GlobalCodes G 
			Where G.GlobalCodeValue=DI.SubCategory2 And G.GlobalCodeCategoryValue=@KpiSubCategories2
		)
		ELSE '' END
	)
	FROM ManualDashboard AS DI
	Where DI.CorporateId=@pCID And DI.FacilityId=@pFID
	AND (@pYear IS NULL OR (DI.[Year] = @pYear AND DI.ExternalValue3 = '1'))
	And DI.BudgetType=@pBudgetType And DI.Indicators=@pValue
	And DI.SubCategory1=@pCategory1 And DI.SubCategory2=@pCategory2
	FOR JSON Path,Root('DashboardResult'),Include_Null_Values
End


