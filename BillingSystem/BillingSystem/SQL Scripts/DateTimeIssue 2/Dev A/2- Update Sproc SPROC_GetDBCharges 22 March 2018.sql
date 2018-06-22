
IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetDBCharges')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetDBCharges

/****** Object:  StoredProcedure [dbo].[SPROC_GetDBCharges]    Script Date: 3/22/2018 6:18:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetDBCharges]   --- SPROC_GetDBCharges 8,9,1014,2016
(  
	@CID int,  --- CorporateID
	@FID int,  --- FacilityID
	@BudgetFor nvarchar(20), --- BudgetFor is Which Budget figures to be seen eg: 1 = OutPatient Bills
	@FiscalYear nvarchar(5)  --- Fiscal Year we are asking eg:'2015'
)
AS
BEGIN
Declare   @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@FID))
Declare @BudgetForCustom nvarchar(20);
SET @BudgetForCustom = @BudgetFor;
-- Declare @CID int = 9 , @FID int = 8, @BudgetFor nvarchar(20) = '10', @FiscalYear nvarchar(5) = '2015'

If(@BudgetFor = '1023')----Inpatient ADC changes
	SET @BudgetFor = '1001'

Declare @MonthNum int, @CurMonthBudget numeric(15,2),@C1 nvarchar(20), @Q1 nvarchar(500), @CurMonthTotalDays int, @CurMonthDays int, @TodaysDate datetime,
		@Counter int=1, @YTDBudget numeric(15,2)=0, @YTDActual numeric(15,2)=0,@Budget numeric(15,2)=0, @Actuals numeric(15,2)=0

Declare @TResults table( BudgetType int,BudgetDescription nvarchar(60),Department numeric(15,0),FiscalYear nvarchar(5),M1 numeric(15,2),M2 numeric(15,2),M3 numeric(15,2),
			M4 numeric(15,2),M5 numeric(15,2),M6 numeric(15,2),M7 numeric(15,2),M8 numeric(15,2),M9 numeric(15,2),M10 numeric(15,2),M11 numeric(15,2),M12 numeric(15,2),BudgetFor nvarchar(20),Sorting int)

Declare @TResultsVar table( BudgetType int,BudgetDescription nvarchar(60),Department numeric(15,0),FiscalYear nvarchar(5),M1 numeric(15,2),M2 numeric(15,2),M3 numeric(15,2),
			M4 numeric(15,2),M5 numeric(15,2),M6 numeric(15,2),M7 numeric(15,2),M8 numeric(15,2),M9 numeric(15,2),M10 numeric(15,2),M11 numeric(15,2),M12 numeric(15,2),BudgetFor nvarchar(20),Sorting int)


--- Adjust the Current Month Budget as per Days passed in current Month - STARTS
--- Clean Up
Truncate table DashboardBudgetTempDB;

--- Get records asked for
If Exists (Select Count(1) from  [dbo].[DashboardBudget] Where CorporateId = @CID and FacilityId = @FID and BudgetFor = @BudgetForCustom and BudgetType in (1,2) and FiscalYear = @FiscalYear)
BEGIN
	insert into DashboardBudgetTempDB
	Select BudgetType,BudgetDescription,DepartmentNumber,FiscalYear,JanuaryBudget,FebruaryBudget,MarchBudget,AprilBudget,MayBudget,JuneBudget,JulyBudget,AugustBudget,SeptemberBudget,OctoberBudget,NovemberBudget,DecemberBudget,BudgetFor
	 from [dbo].[DashboardBudget] Where CorporateId = @CID and FacilityId = @FID and BudgetFor = @BudgetForCustom and BudgetType in (1,2) and FiscalYear = @FiscalYear
END
ELSE
BEGIN
	insert into DashboardBudgetTempDB
	Select 1,'Monlthy Budget','',@FiscalYear,0,0,0,0,0,0,0,0,0,0,0,0,@BudgetForCustom
	 Union 
	Select 2,'Monlthy Actuals','',@FiscalYear,0,0,0,0,0,0,0,0,0,0,0,0,@BudgetForCustom
END

--Budget Type: Yearly Budget
If Exists (Select Count(1) from  [dbo].[DashboardBudget] Where CorporateId = @CID and FacilityId = @FID and BudgetFor = @BudgetForCustom and BudgetType in (1,2) and FiscalYear = @FiscalYear)
BEGIN
insert into @TResults (BudgetType,BudgetDescription,Department,FiscalYear,M12,BudgetFor,Sorting)
	Select 102, 'Yearly Budget', 1000, @FiscalYear, 
	JanuaryBudget+FebruaryBudget+MarchBudget+AprilBudget+MayBudget+JuneBudget+JulyBudget+AugustBudget+SeptemberBudget+OctoberBudget+NovemberBudget+DecemberBudget
	,BudgetFor,11
	from [dbo].[DashboardBudget] where BudgetFor = @BudgetForCustom and BudgetType = 1 and CorporateId = @CID and FacilityId = @FID and FiscalYear = @FiscalYear
END
ELSE
BEGIN
	insert into @TResults (BudgetType,BudgetDescription,Department,FiscalYear,M12,BudgetFor,Sorting)
	Select 102,'Yearly Budget',1000,@FiscalYear,0,@BudgetForCustom,11
END
----Budget Type: Projected Year End
--insert into @TResults (BudgetType,BudgetDescription,Department,FiscalYear,M1,BudgetFor,Sorting)
--Select 103, 'Projected Year End', 1000, @FiscalYear, 
--JanuaryBudget+FebruaryBudget+MarchBudget+AprilBudget+MayBudget+JuneBudget+JulyBudget+AugustBudget+SeptemberBudget+OctoberBudget+NovemberBudget+DecemberBudget
--,BudgetFor,10
--from [dbo].[DashboardBudget] where BudgetFor = @BudgetFor and BudgetType = 3 and CorporateId = @CID and FacilityId = @FID


--*********************** COMMENTED BELOW CALCULATION AS PER THE SKYPE TALK WITH KEN ON 1St April 2015*************

Set @TodaysDate = @LocalDateTime
Set @MonthNum = DatePart(mm,@TodaysDate)

--	Set @C1 = 'M'+cast(@MonthNum as nvarchar(2))
--	Set @Q1 = 'Select @CurMonthBudget ='+ @C1 +' From  DashboardBudgetTempDB Where BudgetType = 1' 
--	Exec sp_executesql @Q1, N'@CurMonthBudget numeric(15,2) output', @CurMonthBudget=@CurMonthBudget Output


--Set @CurMonthTotalDays =  DAY(EOMONTH(@TodaysDate)) 
--Set @CurMonthDays = DAY(@TodaysDate)
--Set @CurMonthBudget = (@CurMonthBudget/@CurMonthTotalDays)*@CurMonthDays

--Set @Q1 = 'Update DashboardBudgetTempDB Set '+ @C1 +' = ' +Cast(@CurMonthBudget as nvarchar(20)) + ' Where BudgetType = 1' 
-- Exec sp_executesql @Q1

--*********************** COMMENTED BELOW CALCULATION AS PER THE SKYPE TALK WITH KEN ON 1St April 2015*************
-- Select * from DashboardBudgetTempDB
-- Select @CurMonthTotalDays,@CurMonthDays,@CurMonthBudget

--- Getting Budget and Actuals for asked BudgetFor
insert into @TResults
Select BudgetType, BudgetDescription, Department, FiscalYear, M1, M2, M3, M4, M5, M6, M7,M8,M9,M10,M11,M12,BudgetFor,BudgetType
from [dbo].[DashboardBudgetTempDB] where BudgetFor = @BudgetForCustom -- and budgetType = 2


--- Calculate Variance - Monthly
insert into @TResults
Select 90 'BudgetType',('Monthly - Variance') 'Desc', TR.Department,TR.FiscalYear,(TR.M1 - DB.M1) 'V1',(TR.M2 - DB.M2) 'V2',(TR.M3 - DB.M3) 'V3',
(TR.M4 - DB.M4) 'V4',(TR.M5 - DB.M5) 'V5',(TR.M6 - DB.M6) 'V6',(TR.M7 - DB.M7) 'V7',( TR.M8 -DB.M8) 'V8',(TR.M9 - DB.M9) 'V9',
(TR.M10 - DB.M10) 'V10',(TR.M11 - DB.M11 ) 'V11',(TR.M12 - DB.M12) 'V12', @BudgetForCustom,4
from @TResults TR
inner join DashboardBudgetTempDB DB on DB.BudgetFor = TR.BudgetFor and DB.BudgetType = 1
Where TR.BudgetType = 2

Declare @YearStartDate varchar(10),@DayTillToday int
SET @YearStartDate =  @FiscalYear +'-01-01';
Set @DayTillToday = DATEDIFF(day, CAST(@YearStartDate as dateTime), @LocalDateTime);
Set @DayTillToday = @DayTillToday +1;
--print ''+@DayTillToday+''
--print ''+@YearStartDate+''
--@YearStartDate
--- Getting Budget and Actuals for asked BudgetFor
insert into @TResults(BudgetType,BudgetDescription,Department,FiscalYear,M12,BudgetFor,Sorting)
Select 103,'Projected Year End', 1000, @FiscalYear,(M1+M2+M3+M4+M5+M6+M7+M8+M9+M10+M11+M12)/@DayTillToday * 365,BudgetFor,10
from [dbo].[DashboardBudgetTempDB] where BudgetFor = @BudgetForCustom and budgetType = 2



---Changes for Inpatient ADC calculations  DateAdd(mm,1,another_date_reference)
If(@BudgetForCustom = '1023')
BEGIN
insert into @TResults
     Exec SPROC_GetADCYearToDateCalc @CID,@FID,@BudgetForCustom,@FiscalYear
END
--- Calulate  -- Year To Date Budget and Actuals
ELSE
BEGIN
	insert into @TResults
	Select CASE WHEN BudgetType = 1 THEN 95 ELSE 96 END , CASE WHEN BudgetType = 1 THEN 'Year to Date - Budget' ELSE 'Year to Date - Actuals' END ,Department,FiscalYear, 
	M1, (M1+M2), (M1+M2+M3),(M1+M2+M3+M4),(M1+M2+M3+M4+M5),(M1+M2+M3+M4+M5+M6),(M1+M2+M3+M4+M5+M6+M7),(M1+M2+M3+M4+M5+M6+M7+M8),
	(M1+M2+M3+M4+M5+M6+M7+M8+M9),(M1+M2+M3+M4+M5+M6+M7+M8+M9+M10),(M1+M2+M3+M4+M5+M6+M7+M8+M9+M10+M11),(M1+M2+M3+M4+M5+M6+M7+M8+M9+M10+M11+M12),@BudgetForCustom,  
	CASE WHEN BudgetType = 1 THEN 5 ELSE 6 END
	From @TResults Where BudgetType in (1,2) and BudgetFor = @BudgetForCustom Order by BudgetType
END

--- Calculate Variance - YTD
insert into @TResultsVar Select * from @TResults Where BudgetType = 95 and BudgetFor = @BudgetForCustom

insert into @TResults
Select 99,'Year to Date - Variance', TR.Department,TR.FiscalYear,(TR.M1 - DB.M1) 'V1',(TR.M2 - DB.M2) 'V2',(TR.M3 - DB.M3) 'V3',
(TR.M4 - DB.M4) 'V4',(TR.M5 - DB.M5) 'V5',(TR.M6 - DB.M6) 'V6',(TR.M7 - DB.M7) 'V7',( TR.M8 -DB.M8) 'V8',(TR.M9 - DB.M9) 'V9',
(  TR.M10 - DB.M10) 'V10',(TR.M11 - DB.M11 ) 'V11',(TR.M12 - DB.M12) 'V12', @BudgetForCustom,7
 from @TResults TR
inner join @TResultsVar DB on DB.BudgetFor = TR.BudgetFor and DB.BudgetType = 95
Where TR.BudgetType = 96


----- YTD - Summary Lines - STARTS


 While @Counter <= @MonthNum
 Begin

	Set @C1 = 'M'+cast(@Counter as nvarchar(2))
	Set @Q1 = 'Select @Budget ='+ @C1 +' From  DashboardBudgetTempDB Where BudgetType = 1' 
	Exec sp_executesql @Q1, N'@Budget numeric(15,2) output', @Budget=@Budget Output
		
	Set @Q1 = 'Select @Actuals ='+ @C1 +' From  DashboardBudgetTempDB Where BudgetType = 2' 
	Exec sp_executesql @Q1, N'@Actuals numeric(15,2) output', @Actuals=@Actuals Output

	Set @YTDBudget = @YTDBudget + isnull(@Budget,0)
	Set @YTDActual = @YTDActual + isnull(@Actuals,0)

	
 Set @Counter = @Counter + 1
 End

If(@BudgetForCustom = '1023' OR @BudgetForCustom = '15' OR @BudgetForCustom = '16' OR @BudgetForCustom = '14')
BEGIN
insert into  @TResults (BudgetType,BudgetDescription,Department,FiscalYear,M12,BudgetFor,Sorting)
			Select 100,'Year To Date Budget',1000,@FiscalYear,@YTDBudget/@MonthNum,@BudgetForCustom,8

If(@BudgetForCustom = '1023') --- For Inpatinet ADC
	insert into  @TResults (BudgetType,BudgetDescription,Department,FiscalYear,M12,BudgetFor,Sorting)
			Select 101,'Year To Date Actual',1000,@FiscalYear,@YTDActual,@BudgetForCustom,9
END
ELSE
BEGIN
insert into  @TResults (BudgetType,BudgetDescription,Department,FiscalYear,M12,BudgetFor,Sorting)
			Select 100,'Year To Date Budget',1000,@FiscalYear,@YTDBudget,@BudgetForCustom,8

insert into  @TResults (BudgetType,BudgetDescription,Department,FiscalYear,M12,BudgetFor,Sorting)
			Select 101,'Year To Date Actual',1000,@FiscalYear,@YTDActual,@BudgetForCustom,9
END

--- YTD Per Encounter visit and Inpatinet days type
If(@BudgetForCustom = '15' OR @BudgetForCustom = '16' OR @BudgetForCustom = '14')
BEGIN
insert into  @TResults (BudgetType,BudgetDescription,Department,FiscalYear,M12,BudgetFor,Sorting)
			Select 101,'Year To Date Actual',1000,@FiscalYear,@YTDActual/@MonthNum,@BudgetForCustom,9
END 


 ----- YTD - Summary Lines - ENDS

--- YEAR TO DATE ACTUAL UPDATES Starts
Declare @YeartodateBudget numeric(15,2), @YearToDateActual numeric(15,2),@yearlyBudget numeric(15,2);
SET @YeartodateBudget = (Select M12 from @TResults WHERE BudgetType =100 and Sorting = 8 and BudgetFor =@BudgetForCustom);
SET @YearToDateActual = (Select M12 from @TResults WHERE BudgetType =101 and Sorting = 9 and BudgetFor =@BudgetForCustom);
SET @yearlyBudget = (Select M12 from @TResults WHERE BudgetType =102 and Sorting = 11 and BudgetFor =@BudgetForCustom);

--PRINT @YearToDateActual;
--PRINT @YeartodateBudget;
--PRINT @yearlyBudget;
UPDATE @TResults SET M12 = (CASE WHEN @YeartodateBudget= 0 THEN 0 ELSE (@YearToDateActual/@YeartodateBudget)*@yearlyBudget END) WHERE BudgetType =103 and Sorting = 10 and BudgetFor =@BudgetForCustom
---YEAR TO DATE ACTUAL UPDATES END
--- Return the final results
	Select BudgetType, BudgetDescription,ISNULL(Department,0) As Department,FiscalYear,ISNULL(M1,0.00) M1,ISNULL(M2,0.00) As M2,ISNULL(M3,0.00) As M3
	, ISNULL(M4,0.00) As M4,ISNULL(M5,0.00) As M5,ISNULL(M6,0.00) As M6, ISNULL(M7,0.00) As M7,ISNULL(M8,0.00) As M8
	,ISNULL(M9,0.00) As M9,ISNULL(M10,0.00) As M10,ISNULL(M11,0.00) As M11,ISNULL(M12,0.00) As M12,BudgetFor,Sorting from @TResults order by Sorting


 

END





GO


