IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetADCYearToDateCalc')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetADCYearToDateCalc
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetADCYearToDateCalc]    Script Date: 22-03-2018 20:19:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		SHashank
-- Create date: APril 1st 2015
-- Description:	To calculate the ADC year to date Actuals and Budgets
-- For Budget type Calculate using the Budget of months / Number of Months
--- For actuals Calcualte using the Inpatient days / number days till that day
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetADCYearToDateCalc]
	(
	@CID int,  --- CorporateID
	@FID int,  --- FacilityID
	@BudgetFor nvarchar(20), --- BudgetFor is Which Budget figures to be seen eg: 1 = OutPatient Bills
	@FiscalYear nvarchar(5)  --- Fiscal Year we are asking eg:'2015'
	)
AS
BEGIN

--- Declare local variables
Declare @MonthNum int, @CurMonthBudget numeric(15,2),@C1 nvarchar(20), @Q1 nvarchar(500), @CurMonthTotalDays int, @CurMonthDays int, @TodaysDate datetime,
		@Counter int=1, @YTDBudget numeric(15,2)=0, @YTDActual numeric(15,2)=0,@Budget numeric(15,2)=0, @Actuals numeric(15,2)=0

--- Declare result table
Declare @TResults1 table( BudgetType int,BudgetDescription nvarchar(60),Department numeric(15,0),FiscalYear nvarchar(5),M1 numeric(15,2),M2 numeric(15,2),M3 numeric(15,2),
			M4 numeric(15,2),M5 numeric(15,2),M6 numeric(15,2),M7 numeric(15,2),M8 numeric(15,2),M9 numeric(15,2),M10 numeric(15,2),M11 numeric(15,2),M12 numeric(15,2),BudgetFor nvarchar(20),Sorting int)

-- Declare temp Table to get the names and columns
DECLARE @NewTable TABLE(ColumnLabel nvarchar(10),ColumnValue varchar(10),Ordinal int);

--- Get the Budgets and actuals of in patient days
insert into @TResults1
Select BudgetType,BudgetDescription,DepartmentNumber,FiscalYear,JanuaryBudget,FebruaryBudget,MarchBudget,AprilBudget,MayBudget,JuneBudget,JulyBudget,AugustBudget,SeptemberBudget,OctoberBudget,NovemberBudget,DecemberBudget,BudgetFor,BudgetType
 from [dbo].[DashboardBudget] Where CorporateId = @CID and FacilityId = @FID and BudgetFor = '1001' and BudgetType in (1,2) and FiscalYear = @FiscalYear

 Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@FID))

Set @TodaysDate = @CurrentDate

Set @MonthNum = DatePart(mm,@TodaysDate)

--Set @TodaysDate = @CurrentDate
--Set @MonthNum = DatePart(mm,@TodaysDate)

Declare @YearStartDate varchar(10);
SET @YearStartDate =  @FiscalYear +'-01-01';

--- Calculate the Monthly budgets
insert into @TResults1
Select CASE WHEN BudgetType = 1 THEN 3 ELSE 4 END , CASE WHEN BudgetType = 1 THEN 'abc' ELSE 'abc1' END ,Department,FiscalYear, 
	M1, (M1+M2), (M1+M2+M3),(M1+M2+M3+M4),(M1+M2+M3+M4+M5),(M1+M2+M3+M4+M5+M6),(M1+M2+M3+M4+M5+M6+M7),(M1+M2+M3+M4+M5+M6+M7+M8),
	(M1+M2+M3+M4+M5+M6+M7+M8+M9),(M1+M2+M3+M4+M5+M6+M7+M8+M9+M10),(M1+M2+M3+M4+M5+M6+M7+M8+M9+M10+M11),(M1+M2+M3+M4+M5+M6+M7+M8+M9+M10+M11+M12),'1001',  
	CASE WHEN BudgetType = 1 THEN 5 ELSE 6 END
	From @TResults1 Where BudgetType in (1,2) and BudgetFor = '1001' and FiscalYear = @FiscalYear Order by BudgetType

--- Calculate the BUdgets monthly for Inpaitent days types
insert into @TResults1
Select 99,'MonthSUM',Department,FiscalYear, 
	M1,
	M2 = (M1+M2),
	M3 = (M1+M2+M3),
	M4 = (M1+M2+M3+M4),
	M5 = (M1+M2+M3+M4+M5),
	M6 = (M1+M2+M3+M4+M5+M6),
	M7 = (M1+M2+M3+M4+M5+M6+M7),
	M8 = (M1+M2+M3+M4+M5+M6+M7+M8),
	M9 = (M1+M2+M3+M4+M5+M6+M7+M8+M9),
	M10 = (M1+M2+M3+M4+M5+M6+M7+M8+M9+M10),
	M11 = (M1+M2+M3+M4+M5+M6+M7+M8+M9+M10+M11),
	M12 = (M1+M2+M3+M4+M5+M6+M7+M8+M9+M10+M11+M12),
	@BudgetFor,  
	CASE WHEN BudgetType = 1 THEN 101 ELSE 6 END
	From @TResults1 Where BudgetType in (1) and BudgetFor = '1001'
	Order by BudgetType

--- Calculate the Days in the months
-- also if months has cuurent date lies in it then the days till that date
insert into @TResults1
Select 97,'MonthCalc',Department,FiscalYear, 
	M1 = DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)),
	M2 = ((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))),
	M3 =((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,2,CAST(@YearStartDate as dateTime))))), 
	M4 = 
	((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,2,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,3,CAST(@YearStartDate as dateTime))))),
	M5 = 
	((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,2,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,3,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,4,CAST(@YearStartDate as dateTime))))),
	M6 =
	((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,2,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,3,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,4,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,5,CAST(@YearStartDate as dateTime))))),
	M7 = 
	((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,2,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,3,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,4,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,5,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,6,CAST(@YearStartDate as dateTime))))),
	M8 = 
	((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,2,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,3,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,4,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,5,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,6,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,7,CAST(@YearStartDate as dateTime))))),
	M9 = 
	((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,2,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,3,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,4,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,5,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,6,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,7,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,8,CAST(@YearStartDate as dateTime))))),
	M10 =
	((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,2,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,3,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,4,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,5,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,6,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,7,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,8,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,9,CAST(@YearStartDate as dateTime))))),
	M11 =
	((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,2,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,3,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,4,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,5,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,6,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,7,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,8,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,9,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,10,CAST(@YearStartDate as dateTime))))),
	M12 =
	((DBO.GetDaysInMonth(CAST(@YearStartDate as dateTime)))
	+(DBO.GetDaysInMonth(DateAdd(mm,1,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,2,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,3,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,4,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,5,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,6,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,7,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,8,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,9,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,10,CAST(@YearStartDate as dateTime))))
	+(DBO.GetDaysInMonth(DateAdd(mm,11,CAST(@YearStartDate as dateTime))))),
	@BudgetFor,  
	CASE WHEN BudgetType = 1 THEN 100 ELSE 6 END
	From @TResults1 Where BudgetType in (1) and BudgetFor = '1001'
	Order by BudgetType


--- Calcualtion part to caluculate the Year to Date Budget
--- Formula: Budget for each month/ Number of days in the each month for inpaient type
--insert into @TResults1
--Select 95, 'Year to Date - Budget' ,Department,FiscalYear, 
--	M1 = (Select TOP(1)M1 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M1 From @TResults1 Where BudgetType = 97),
--	M2 = (Select TOP(1)M2 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M2 From @TResults1 Where BudgetType = 97), 
--	M3 = (Select TOP(1)M3 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M3 From @TResults1 Where BudgetType = 97), 
--	M4 = (Select TOP(1)M4 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M4 From @TResults1 Where BudgetType = 97),
--	M5 = (Select TOP(1)M5 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M5 From @TResults1 Where BudgetType = 97),
--	M6 = (Select TOP(1)M6 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M6 From @TResults1 Where BudgetType = 97),
--	M7 = (Select TOP(1)M7 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M7 From @TResults1 Where BudgetType = 97),
--	M8 = (Select TOP(1)M8 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M8 From @TResults1 Where BudgetType = 97),
--	M9 = (Select TOP(1)M9 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M9 From @TResults1 Where BudgetType = 97),
--	M10 = (Select TOP(1)M10 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M10 From @TResults1 Where BudgetType = 97),
--	M11 = (Select TOP(1)M11 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M11 From @TResults1 Where BudgetType = 97),
--	M12 =(Select TOP(1)M12 From @TResults1 Where BudgetType = 99)/(Select TOP(1)M12 From @TResults1 Where BudgetType = 97),
--	@BudgetFor,  
--	5
--	From @TResults1 Where BudgetType = 3 and BudgetFor = '1001'
--	Order by BudgetType

insert into @TResults1
Select 95, 'Year to Date - Budget' ,DepartmentNumber,FiscalYear, 
	(JanuaryBudget)/1,
	(JanuaryBudget + FebruaryBudget)/2,
	(JanuaryBudget + FebruaryBudget + MarchBudget)/3,
	(JanuaryBudget + FebruaryBudget + MarchBudget+AprilBudget)/4,
	(JanuaryBudget + FebruaryBudget + MarchBudget+AprilBudget+MayBudget)/5,
	(JanuaryBudget + FebruaryBudget + MarchBudget+AprilBudget+MayBudget+ JuneBudget)/6,
	(JanuaryBudget + FebruaryBudget + MarchBudget+AprilBudget+MayBudget+ JuneBudget+JulyBudget)/7,
	(JanuaryBudget + FebruaryBudget + MarchBudget+AprilBudget+MayBudget+ JuneBudget+JulyBudget+AugustBudget)/8,
	(JanuaryBudget + FebruaryBudget + MarchBudget+AprilBudget+MayBudget+ JuneBudget+JulyBudget+AugustBudget+SeptemberBudget)/9,
	(JanuaryBudget + FebruaryBudget + MarchBudget+AprilBudget+MayBudget+ JuneBudget+JulyBudget+AugustBudget+SeptemberBudget+OctoberBudget)/10,
	(JanuaryBudget + FebruaryBudget + MarchBudget+AprilBudget+MayBudget+ JuneBudget+JulyBudget+AugustBudget+SeptemberBudget+OctoberBudget+NovemberBudget)/11,
	(JanuaryBudget + FebruaryBudget + MarchBudget+AprilBudget+MayBudget+ JuneBudget+JulyBudget+AugustBudget+SeptemberBudget+OctoberBudget+NovemberBudget+DecemberBudget)/12,	
	@BudgetFor,  
	5
	From DashboardBudget Where BudgetType = 1 and BudgetFor = @BudgetFor and FiscalYear = @FiscalYear
	And CorporateId = @CID And FacilityId = @FID
	Order by BudgetType

--- Calcualtion part to caluculate the Year to Date Actuals
--- Formula: Actuals for each month/ Number of days in the each month for inpaitent type
insert into @TResults1
Select 96, 'Year to Date - Actuals' ,Department,FiscalYear, 
	M1 = (Select TOP(1)M1 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M1 From @TResults1 Where BudgetType = 97),
	M2 = (Select TOP(1)M2 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M2 From @TResults1 Where BudgetType = 97), 
	M3 = (Select TOP(1)M3 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M3 From @TResults1 Where BudgetType = 97), 
	M4 = (Select TOP(1)M4 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M4 From @TResults1 Where BudgetType = 97),
	M5 = (Select TOP(1)M5 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M5 From @TResults1 Where BudgetType = 97),
	M6 = (Select TOP(1)M6 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M6 From @TResults1 Where BudgetType = 97),
	M7 = (Select TOP(1)M7 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M7 From @TResults1 Where BudgetType = 97),
	M8 = (Select TOP(1)M8 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M8 From @TResults1 Where BudgetType = 97),
	M9 = (Select TOP(1)M9 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M9 From @TResults1 Where BudgetType = 97),
	M10 = (Select TOP(1)M10 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M10 From @TResults1 Where BudgetType = 97),
	M11 = (Select TOP(1)M11 From @TResults1 Where BudgetType = 4)/(Select TOP(1)M11 From @TResults1 Where BudgetType = 97),
	M12 =(Select TOP(1)M12 From @TResults1 Where BudgetType =  4)/(Select TOP(1)M12 From @TResults1 Where BudgetType = 97),
	@BudgetFor,  
	6
	From @TResults1 Where BudgetType = 4 and BudgetFor = '1001'
	Order by BudgetType


--- Select the desired rows data like aActuals and Budget for year to date 
Select * from @TResults1 
WHERE BudgetType in (95,96)

-- Delete the data
Delete From @TResults1;
END





GO


