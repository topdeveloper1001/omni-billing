-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetDashboardIndicatorData','P') IS NOT NULL
   DROP PROCEDURE SprocGetDashboardIndicatorData
GO

CREATE PROCEDURE SprocGetDashboardIndicatorData
(
	@pCId bigint,
	@pFId bigint
)
As
Begin
	Declare @FName nvarchar(100)=(Select TOP 1 FacilityName From Facility Where FacilityId=@pFId)
	Declare @ExternalDashboarType nvarchar(5)='4345',@KpiSubCategories1 nvarchar(5)='4347',@KpiSubCategories2 nvarchar(5)='4351'
	Declare @DashBoardBudgetType nvarchar(5)= '3112'

	Select D.ID,D.IndicatorId,D.IndicatorNumber,D.SubCategory1,D.SubCategory2,D.StatisticData,D.[Month],D.[Year],D.FacilityId,D.CorporateId,D.CreatedBy
	,D.CreatedDate,D.ExternalValue1,D.ExternalValue2,D.ExternalValue3,D.IsActive,D.DepartmentNumber
	,FacilityNameStr = @FName
	,MonthStr=DATENAME(month, DATEADD(month, D.[Month]-1, CAST('2001-01-01' AS datetime)))
	,IndicatorStr=(Select TOP 1 I.[Description] From DashboardIndicators I Where I.IndicatorNumber=D.IndicatorNumber And I.CorporateId=@pCId)
	,BudgetType =
	(
		Case When ISNULL(D.ExternalValue1,'') !=''
		THEN (Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=D.ExternalValue1 
			  And G.GlobalCodeCategoryValue=@DashBoardBudgetType) 
		ELSE '' END
	)
	,SubCategory1Str =
	(
		Case When ISNULL(D.SubCategory1,'') !='' AND ISNULL(D.SubCategory1,'0') !='0'
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
	From DashboardIndicatorData D Where D.CorporateId=@pCId 
	And ISNULL(D.IsActive,1)=1 And D.FacilityId=@pFId
	For Json Path, Root('DashboardResult'),Include_Null_Values
End