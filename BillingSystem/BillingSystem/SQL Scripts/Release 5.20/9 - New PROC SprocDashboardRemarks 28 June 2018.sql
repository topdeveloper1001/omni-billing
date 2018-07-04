
IF OBJECT_ID('SprocDashboardRemarks','P') IS NOT NULL
	DROP PROCEDURE SprocDashboardRemarks 
GO

/****** Object:  StoredProcedure [dbo].[SPROC_SortDashboardIndicatorCols]    Script Date: 6/27/2018 2:46:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Exec SprocDashboardRemarks 1016,0 'IndicatorNumber','ASC',1016,0,1
CREATE Procedure [dbo].[SprocDashboardRemarks]
(
	@pCID bigint=NULL,
	@pFID bigint=Null,
	@pTypeId int=Null,
	@pMonth int=null
)
As
Begin
	Declare @KpiSubCategories1 nvarchar(5)='4347',@KpiSubCategories2 nvarchar(5)='4351',@DashboardType nvarchar(5)= '4345'
	
	SELECT DI.*
	,FacilityStr=(Select TOP 1 F.FacilityName From Facility F Where F.FacilityId=DI.FacilityId)
	,DashboardTypeStr=
	(
		Case WHEN ISNULL(DI.DashboardType,'') !='' THEN 
		(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.DashboardType And G.GlobalCodeCategoryValue=@DashboardType)
		ELSE '' END
	)
	,DashboardSectionStr=
	(
		Case WHEN ISNULL(DI.DashboardSection,'') !='' AND ISNULL(DI.DashboardType,'') !='' 
		AND Exists
		(Select TOP 1 G.ExternalValue1 From GlobalCodes G Where G.GlobalCodeValue=DI.DashboardType And G.GlobalCodeCategoryValue=@DashboardType)
		THEN
		(
			Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.DashboardSection 
			And G.GlobalCodeCategoryValue=
			(Select TOP 1 G1.ExternalValue1 From GlobalCodes G1 Where G1.GlobalCodeValue=DI.DashboardType And G1.GlobalCodeCategoryValue=@DashboardType)
		)
		ELSE '' END
	)
	,DashboardMonth=DATENAME(month, DATEADD(month, DI.[Month]-1, CAST('2001-01-01' AS datetime)))
	
	FROM DashboardRemark AS DI
	Where DI.CorporateId=@pCID
	AND (ISNULL(@pMonth,0)=0 OR DI.[Month]=@pMonth)
	And (ISNULL(DI.DashboardType,0)=0 OR DI.DashboardType=DI.DashboardType)
	AND (ISNULL(@pFID,0)=0 OR DI.FacilityId=@pFID)
	ANd ISNULL(DI.IsActive,1)=1
	FOR JSON Path,Root('DashboardResult'),Include_Null_Values
End	


