IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_SetDBCountersActuals')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_SetDBCountersActuals
		 
GO

/****** Object:  StoredProcedure [dbo].[SPROC_SetDBCountersActuals]    Script Date: 3/22/2018 7:54:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SPROC_SetDBCountersActuals]
(  
	@CID int,  --- CorporateID
	@FID int,  --- FacilityID
	@BudgetFor nvarchar(20), --- BudgetFor is Which Budget figures to be seen eg: 1 = OutPatient Bills etc. 
	@FiscalYear nvarchar(5)  --- Fiscal Year we are asking eg:'2015'
)
AS
BEGIN
Declare @LocalDateTime datetime = (Select dbo.GetCurrentDatetimeByEntity(@FID))
--- Declare @CID int = 9, @FID int = 8,@BudgetFor int = 10, @FiscalYear nvarchar(5) = '2015'

Declare @FromDate datetime, @TillDate datetime,@EncounterPatientType int, @BudgetForParam varchar(20)

Set @FromDate = @FiscalYear + '-01-01'
Set @TillDate = @FiscalYear + '-12-31 23:59:59'
SET @BudgetForParam = @BudgetFor

Declare @TA1 table( BudgetType int,BudgetDescription nvarchar(60),Department numeric(15,0),FiscalYear nvarchar(5),M1 numeric(15,2),M2 numeric(15,2),M3 numeric(15,2),
			M4 numeric(15,2),M5 numeric(15,2),M6 numeric(15,2),M7 numeric(15,2),M8 numeric(15,2),M9 numeric(15,2),M10 numeric(15,2),M11 numeric(15,2),M12 numeric(15,2))

Declare @TAFirst table (BudgetType int,BudgetDescription Nvarchar(100), Department numeric(15,0), FiscalYear nvarchar(5), Gross numeric (15,2), MonthType nvarchar(3))

--- Based on Input Param BudgetFor Makeappropriate selections - STARTS

IF( @BudgetFor = '1023')--- Budget type for is ADC
  SET @BudgetFor ='1001' --- Set Budget type for to Patient days

IF (@BudgetForParam = '1023')
BEGIN
insert into @TAFirst
	Select  2 'BudgetType', 'Monthly - Actuals' 'BudgetDescription', 1000 'Department',@FiscalYear 'FiscalYear', sum(DTC.ActivityTotal)/MAX(DAY(DATEADD(DD,-1,DATEADD(MM,DATEDIFF(MM,-1,DTC.ActivityDay),0)))) 'Gross',
	Max('M'+Cast( DatePart(mm,DTC.ActivityDay) as nvarchar(2))) 'MonthType'
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = @CID and DTC.FacilityID = @FID and DTC.StatisticDescription = @BudgetFor and DTC.ActivityDay between @FromDate and @TillDate
	 Group By DatePart(mm,DTC.ActivityDay)
END 
ELSE
Begin 
insert into @TAFirst
	Select  2 'BudgetType', 'Monthly - Actuals' 'BudgetDescription', 1000 'Department',@FiscalYear 'FiscalYear', sum(DTC.ActivityTotal) 'Gross',
	Max('M'+Cast( DatePart(mm,DTC.ActivityDay) as nvarchar(2))) 'MonthType'
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = @CID and DTC.FacilityID = @FID and DTC.StatisticDescription = @BudgetFor and DTC.ActivityDay between @FromDate and @TillDate
	 Group By DatePart(mm,DTC.ActivityDay)
END


--- Insert one Blank row if there is no Data for passed in Param Else Graphs/sections gets screwed up at front end
If not exists (select 'A' from @TAFirst)
Begin
	insert into @TAFirst
	Select 2 'BudgetType', 'Monthly - Actuals' 'BudgetDescription', 1000 'Department',@FiscalYear 'FiscalYear', 0 'Gross','M1' 'MonthType'	
End

--- Based on Input Param BudgetFor Makeappropriate selections - ENDS

---->>> ACTUALS --- STARTS 
;With Report     
AS    
(    
select * from  
(
Select * from @TAFirst
) src    
pivot    
(    
MAX(GROSS)    
for MonthType in (M1,M2,M3,M4,M5,M6,M7,M8,M9,M10,M11,M12)    
)piv    
)

insert into @TA1 select * from report;

If exists (Select  'A' from [dbo].[DashboardBudget] where CorporateId = @CID and FacilityId = @FID and BudgetFor = @BudgetForParam and BudgetType = 2)
Begin
	Delete from [dbo].[DashboardBudget] where CorporateId = @CID and FacilityId = @FID and BudgetFor = @BudgetForParam and BudgetType = 2;
End
	insert into [dbo].[DashboardBudget]
	(BudgetType, BudgetDescription, DepartmentNumber, FiscalYear, JanuaryBudget, FebruaryBudget, MarchBudget, AprilBudget, MayBudget, JuneBudget, JulyBudget, 
	AugustBudget, SeptemberBudget, OctoberBudget, NovemberBudget, DecemberBudget, CorporateId, FacilityId, CreatedBy, CreatedDate,ModifiedBy, ModifiedDate, IsActive, BudgetFor)
	Select BudgetType,BudgetDescription,Department,FiscalYear,isnull(M1,0) 'M1',isnull(M2,0) 'M2',isnull(M3,0) 'M3',isnull(M4,0) 'M4',isnull(M5,0) 'M5',isnull(M6,0) 'M6',
	isnull(M7,0) 'M7',isnull(M8,0) 'M8',isnull(M9,0) 'M9',isnull(M10,0) 'M10',isnull(M11,0) 'M11',isnull(M12,0) 'M12',
	@CID 'CID',@FID 'FID',1 'CBy',@LocalDateTime 'CDate',null 'MBy',null 'MDate',1 'IsActive', @BudgetForParam 'BudgetFor' from @TA1


	 Select * from [dbo].[DashboardBudget]

END











GO


