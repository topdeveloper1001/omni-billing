IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_SyncDashboardDataToManual')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_SyncDashboardDataToManual
GO

/****** Object:  StoredProcedure [dbo].[SPROC_SyncDashboardDataToManual]    Script Date: 20-03-2018 16:51:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_SyncDashboardDataToManual]
(
	@pCorporateId int=12,
	@pFacilityid int=17
)
AS
BEGIN

	Declare @CurrentDate datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

	Declare @IndicatorNumber nvarchar(10),@Subcategory1 nvarchar(10),@SubCategory2 nvarchar(10),@ExternalValue1 nvarchar(10),@EV1_BudgetActual int = 1, 
@MonthNumber int= 1,@year int = Year(@CurrentDate),@priorYear int = (Year(@CurrentDate) -1)



Declare  CursurCalCulation Cursor For 
(Select MAX(IndicatorNumber),MAX(Subcategory1),MAX(SubCategory2) From DashboardIndicators where CorporateId =@pCorporateId  Group by IndicatorNumber,Subcategory1,SubCategory2)

OPEN CursurCalCulation;  

FETCH NEXT FROM CursurCalCulation INTO @IndicatorNumber,@Subcategory1,@SubCategory2
WHILE @@FETCH_STATUS = 0
       BEGIN  
       Print @IndicatorNumber
        EXEC SPROC_UpdateCalculativeIndicatorData_SA @IndicatorNumber,@pCorporateId,@pFacilityid,@year,1,28,@Subcategory1,@SubCategory2;
        EXEC SPROC_UpdateCalculativeIndicatorData_SA @IndicatorNumber,@pCorporateId,@pFacilityid,@year,2,28,@Subcategory1,@SubCategory2;
        EXEC SPROC_UpdateCalculativeIndicatorData_SA @IndicatorNumber,@pCorporateId,@pFacilityid,@priorYear,2,28,@Subcategory1,@SubCategory2;
	   --EXEC SPROC_CopyToManualDashboard_SA @IndicatorNumber,@pCorporateId,@pFacilityid,28,@Subcategory1,@SubCategory2
FETCH NEXT FROM CursurCalCulation INTO @IndicatorNumber,@Subcategory1,@SubCategory2
END
CLOSE CursurCalCulation;
DEALLOCATE CursurCalCulation;

END









GO


