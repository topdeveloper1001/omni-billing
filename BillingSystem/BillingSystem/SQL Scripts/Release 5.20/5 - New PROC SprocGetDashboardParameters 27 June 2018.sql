IF OBJECT_ID('SprocGetDashboardParameters','P') IS NOT NULL
	DROP PROCEDURE SprocGetDashboardParameters
GO

/****** Object:  StoredProcedure [dbo].[SPROC_SortDashboardIndicatorCols]    Script Date: 6/27/2018 2:46:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Exec SprocGetDashboardParameters 'IndicatorNumber','ASC',1016,0,1
CREATE Procedure [dbo].[SprocGetDashboardParameters]
(
	@pCID bigint=NULL,
	@pFID bigint=Null,
	@pTypeId int=null
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
	,IndicatorCategoryStr=
	(
		Case WHEN ISNULL(DI.IndicatorCategory,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.IndicatorCategory And G.GlobalCodeCategoryValue='4345')
		ELSE '' END
	)
	,DataFieldStr=
	(
		Case WHEN ISNULL(DI.DataField,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.DataField And G.GlobalCodeCategoryValue='4345')
		ELSE '' END
	)
	,ValueTypeStr=
	(
		Case WHEN ISNULL(DI.ValueType,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.ValueType And G.GlobalCodeCategoryValue='4345')
		ELSE '' END
	)
	,ArgumentStr=
	(
		Case WHEN ISNULL(DI.Argument,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.Argument And G.GlobalCodeCategoryValue='4345')
		ELSE '' END
	)
	,ColorCodeStr=
	(
		Case WHEN ISNULL(DI.ColorCode,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.ColorCode And G.GlobalCodeCategoryValue='4345')
		ELSE '' END
	)
	,ExternalDashboardTypeStr=
	(
		Case WHEN ISNULL(DI.ExternalValue1,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.ExternalValue1 And G.GlobalCodeCategoryValue='4345')
		ELSE '' END
	)
	FROM DashboardParameters AS DI
	Where DI.CorporateId=@pCID And DI.FacilityId=@pFID
	AND (ISNULL(@pTypeId,0)=0 OR DI.DashboardType=@pTypeId)
	FOR JSON Path,Root('DashboardResult'),Include_Null_Values
End


