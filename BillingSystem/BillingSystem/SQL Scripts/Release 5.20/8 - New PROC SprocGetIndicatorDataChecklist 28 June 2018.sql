IF OBJECT_ID('SprocGetIndicatorDataChecklist ','P') IS NOT NULL
	DROP PROCEDURE SprocGetIndicatorDataChecklist 
GO

/****** Object:  StoredProcedure [dbo].[SPROC_SortDashboardIndicatorCols]    Script Date: 6/27/2018 2:46:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Exec SprocGetIndicatorDataChecklist 1016,0 'IndicatorNumber','ASC',1016,0,1
CREATE Procedure [dbo].[SprocGetIndicatorDataChecklist]
(
	@pCID bigint=NULL,
	@pFID bigint=Null,
	@pYear int=NULL,
	@pBudgetType int=NULL
)
As
Begin
	Declare @True bit=1,@False bit=0

	SELECT DI.*
	,CusM1=(Case WHEN DI.M1='1' THEN @True Else @False END),CusM2=(Case WHEN DI.M2='1' THEN @True Else @False END)
	,CusM3=(Case WHEN DI.M3='1' THEN @True Else @False END),CusM4=(Case WHEN DI.M4='1' THEN @True Else @False END)
	,CusM5=(Case WHEN DI.M5='1' THEN @True Else @False END),CusM6=(Case WHEN DI.M6='1' THEN @True Else @False END)
	,CusM7=(Case WHEN DI.M7='1' THEN @True Else @False END),CusM8=(Case WHEN DI.M8='1' THEN @True Else @False END)
	,CusM9=(Case WHEN DI.M9='1' THEN @True Else @False END),CusM10=(Case WHEN DI.M10='1' THEN @True Else @False END)
	,CusM11=(Case WHEN DI.M11='1' THEN @True Else @False END),CusM12=(Case WHEN DI.M12='1' THEN @True Else @False END)
	,CusMonth=(Case WHEN ISNULL(DI.ExternalValue2,'0') > 0 THEN @True Else @False END)
	,FacilityName=(Select TOP 1 F.FacilityName From Facility F Where F.FacilityId=DI.FacilityId)	
	FROM IndicatorDataChecklist AS DI
	Where DI.CorporateId=@pCID 
	And (ISNULL(@pFID,0)=0 OR DI.FacilityId=@pFID )
	And (ISNULL(@pYear,0)=0 OR DI.[Year]=@pYear )
	And (ISNULL(@pBudgetType,0)=0 OR DI.BudgetType=@pBudgetType)
	And ISNULL(DI.IsActive,1)=1
	FOR JSON Path,Root('IndicatorData'),Include_Null_Values
End


