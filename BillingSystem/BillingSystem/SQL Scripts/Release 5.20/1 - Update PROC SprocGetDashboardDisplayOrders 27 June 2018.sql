-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetDashboardDisplayOrders','P') IS NOT NULL
   DROP PROCEDURE SprocGetDashboardDisplayOrders
GO

CREATE PROCEDURE SprocGetDashboardDisplayOrders
(
	@pCId bigint,
	@pFId bigint
)
As
Begin
	Declare @FName nvarchar(100)=(Select TOP 1 FacilityName From Facility Where FacilityId=@pFId)
	Declare @ExternalDashboarType nvarchar(100)='4345',@KpiSubCategories1 nvarchar(100)='4347',@KpiSubCategories2 nvarchar(100)='4351'


	Select D.[Id],D.[DashboardId],D.[SectionId],D.[IndicatorNumber],D.[SubCategory1],D.[SubCategory2],D.[FacilityId],D.[CorporateId],D.[SortOrder]
	,D.[CreatedBy],D.[CreatedDate],D.[IsDeleted],FacilityStr = @FName
	,DashboardTypeStr =
	(
		Case When D.DashboardId > 0 
		THEN (Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=D.DashboardId And G.GlobalCodeCategoryValue=@ExternalDashboarType) 
		ELSE '' END
	)
	,DashboardSectionStr =
	(
		Case When D.DashboardId > 0 AND ISNULL(D.SectionId,'') !=''
		THEN 
		(
			Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=D.SectionId 
			And G.GlobalCodeCategoryValue=
			(
				Select TOP 1 G1.ExternalValue1 
				From GlobalCodes G1 
				Where G1.GlobalCodeCategoryValue=@ExternalDashboarType And G1.GlobalCodeValue=D.DashboardId
			)
		)
		ELSE '' END
	)
	,IndicatorStr=(Select TOP 1 I.[Description] From DashboardIndicators I Where I.IndicatorNumber=D.IndicatorNumber And I.CorporateId=@pCId)
	,SubCategory1Str =
	(
		Case When ISNULL(D.SubCategory1,'') !=''
		THEN (Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=D.SubCategory1 And G.GlobalCodeCategoryValue=@KpiSubCategories1) 
		ELSE '' END
	)
	,SubCategory2Str =
	(
		Case When ISNULL(D.SubCategory1,'') !='' AND IsNumeric(ISNULL(D.SubCategory1,'a'))=1 AND ISNULL(D.SubCategory2,'') !=''
		THEN 
		(
			Select TOP 1 G.GlobalCodeName From GlobalCodes G 
			Where G.GlobalCodeValue=D.SubCategory2 And G.GlobalCodeCategoryValue=@KpiSubCategories2
		)
		ELSE '' END
	)
	From DashboardDisplayOrder D Where D.CorporateId=@pCId 
	And D.IsDeleted=0 And D.FacilityId=@pFId
	For Json Path, Root('DashboardResult'),Include_Null_Values
End