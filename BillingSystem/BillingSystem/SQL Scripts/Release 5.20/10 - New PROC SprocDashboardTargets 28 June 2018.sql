IF OBJECT_ID('SprocDashboardTargets','P') IS NOT NULL
	DROP PROCEDURE SprocDashboardTargets 
GO

/****** Object:  StoredProcedure [dbo].[SPROC_SortDashboardIndicatorCols]    Script Date: 6/27/2018 2:46:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Exec SprocDashboardTargets 1016,0 'IndicatorNumber','ASC',1016,0,1
CREATE Procedure [dbo].[SprocDashboardTargets]
(
	@pCID bigint=NULL,
	@pFID bigint=Null
)
As
Begin
	Declare @KpiSubCategories1 nvarchar(5)='4347',@KpiSubCategories2 nvarchar(5)='4351',@DashboardType nvarchar(5)= '4345'
	
	SELECT DI.*
	,RoleName=(Select TOP 1 G.RoleName From [Role] G Where G.RoleId=DI.RoleId)
	,UOMstr=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.UnitOfMeasure And G.GlobalCodeCategoryValue='1012')
	,TimmingIncrementStr=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.TimingIncrement 
		And G.GlobalCodeCategoryValue='1013')	
	FROM DashboardTargets AS DI
	Where DI.CorporateId=@pCID
	AND (ISNULL(@pFID,0)=0 OR DI.FacilityId=@pFID)
	ANd ISNULL(DI.IsActive,1)=1
	FOR JSON Path,Root('DashboardResult'),Include_Null_Values
End