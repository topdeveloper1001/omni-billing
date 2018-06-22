	IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ZESynchDBManual')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ZESynchDBManual
GO

/****** Object:  StoredProcedure [dbo].[SPROC_ZESynchDBManual]    Script Date: 22-03-2018 15:35:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SPROC_ZESynchDBManual]
(  
	@pCID int = 12,  --- CorporateID
	@pFID int = 18,  --- FacilityID
	@pYear nvarchar(4) = '2015',  ----- Fiscalyear
	@pDataType nvarchar(2) = '1'
)
AS
BEGIN
Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFID))
Declare @MasterFID int = @pFID

--- Change Requested to have Corporate Driven as Master unless Facility Speicific Data is there --- STARTS
if not exists (Select 1 from DashboardIndicators Where FacilityId = @pFID)
Begin
	Set @MasterFID = 0
End

Declare @DBMDT table (IndID int,IndNum nvarchar(50),SC1 nvarchar(50),SC2 nvarchar(50), DBMonth int, UVC nvarchar(200), InsertStatus bit)
Declare @Cnt int = 1

--- Following can removed if Front end is corrected to handle that Cat1 and Cat2 will never be inserted as NULL -  STARTS
Update DashboardIndicators Set SubCategory1 = 0 where SubCategory1 is NULL
Update DashboardIndicators Set SubCategory2 = 0 where SubCategory2 is NULL
Update DashboardIndicatorData Set SubCategory1 = 0 where SubCategory1 is NULL
Update DashboardIndicatorData Set SubCategory2 = 0 where SubCategory2 is NULL
Update DashboardIndicatorData Set ExternalValue3 = '1' where ExternalValue3 is NULL OR ExternalValue3 =''
--- Following can removed if Front end is corrected to handle that Cat1 and Cat2 will never be inserted as NULL -  ENDS

insert into @DBMDT
Select DI.IndicatorID,DI.IndicatorNumber,DI.SubCategory1,DI.SubCategory2,1,(DI.IndicatorNumber+DI.SubCategory1+DI.SubCategory2+Cast(@pCID as nvarchar(10)) + Cast(@pFID as nvarchar(10)) + @pYear + @pDataType +'1') UV, 1 from DashboardIndicators DI 
Where DI.CorporateId = @pCID and DI.FacilityId = @MasterFID and IsActive = 1
order by DI.CorporateId,DI.FacilityId,DI.IndicatorNumber

While @Cnt < 12
Begin
		
	insert into @DBMDT
	Select max(IndID),IndNum,SC1,SC2, (DBMonth+@Cnt),(IndNum+SC1+SC2+Cast(@pCID as nvarchar(10)) + Cast(@pFID as nvarchar(10)) + @pYear + @pDataType +Cast((@Cnt+1) as nvarchar(2))),1 
	from @DBMDT where DBMonth = 1
	Group by IndNum,SC1,SC2,DBMonth;

	Set @Cnt = @Cnt+1
End


Update @DBMDT Set InsertStatus=0 where UVC in 
(Select (IndicatorNumber+SubCategory1+SubCategory2+Cast(@pCID as nvarchar(10)) + Cast(@pFID as nvarchar(10)) + @pYear + @pDataType +Cast([Month] as nvarchar(2)))
from DashboardIndicatorData Where CorporateId = @pCID and FacilityId = @pFID and [Year] = @pYear and ExternalValue1 = @pDataType and IndicatorNumber = IndNum )

insert into DashboardIndicatorData
(IndicatorId, IndicatorNumber, SubCategory1, SubCategory2, StatisticData, Month, Year, FacilityId, CorporateId, CreatedBy, CreatedDate, ExternalValue1,IsActive, DepartmentNumber)
Select IndID,IndNum,SC1,SC2,0,DBMonth,@pYear,@pFID,@pCID,100,@CurrentDate,@pDataType,1,0 from @DBMDT Where InsertStatus = 1 -- group by IndNum,DBMonth


END











GO


