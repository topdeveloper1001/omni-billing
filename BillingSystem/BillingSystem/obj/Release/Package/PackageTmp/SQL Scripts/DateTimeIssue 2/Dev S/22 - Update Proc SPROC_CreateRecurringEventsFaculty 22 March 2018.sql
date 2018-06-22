IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CreateRecurringEventsFaculty')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CreateRecurringEventsFaculty
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CreateRecurringEventsFaculty]    Script Date: 22-03-2018 19:12:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Shashank>
-- Create date: <Create Date,11/27/2015 12:22:21 PM >
-- Description:	<Description, Create the Recurring visit for the pattern user have added for the Recurring visits>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_CreateRecurringEventsFaculty]
(
	@pPhyId int =60,
	@pFId int,
	@pCId int
)
AS
BEGIN
Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFId))


Declare @pId int = 1
--- Split pattern table
Declare @SplitTable Table(ID int, Value nvarchar(10));
Declare @LocalDateTable Table(ID int, Value nvarchar(10));
--Declare Local Variables
Declare @Counter int =0;Declare @pSelectedDate datetime,@pSelectedDate1 datetime,
@pDailyCaseFailure int = 0,@pDailyCount int,
@WeekInterval int =0,@WeekCounter int,@pWeeklyCount int,@pWeeklyCaseFailure int = 0,@pWeeklyAdd int,
@RecuranceType nvarchar(10),@CurrentWeekDay int,
@MonthlyInterval int,@pMonthlyCount int,@pMonthlyCounter int,@pMonthlyAdd int,@pMonhtlyCaseFailure int,@pMonhtlyAdd int,@pMonthlyDay int,@pMonthFrequnecy int;

--- Dynamic Event Id
DECLARE @DynamicEventId nvarchar(50) =  '1766' + CAST(floor(rand() * 100000 - 1)  as Nvarchar(20))
Declare @PhyDeptId int, @FacultyType int
Declare @WeekNumber int ;



--- Scheduling Temp table
Declare @SchedulingTableTemp Table(
    [FacultyId] [int] NULL,[FacultyType] [int] NULL,[DeptId] [int] NULL,[AvailabilityType] [nvarchar](50) NULL,[Pattern] [nvarchar](50) NULL,[WorkingDay] [int] NULL,
	[WeekNumber] [int] NULL,[FromDate] [datetime] NULL,[ToDate] [datetime] NULL,[FacilityId] [int] NULL,[CorporateId] [int] NULL,[IsActive] [bit] NULL,[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,[RecurreanceDateTo] [datetime] NULL,[RecurreanceDateFrom] [datetime] NULL,[ExtValue1] [nvarchar](50) NULL,[ExtValue2] [nvarchar](50) NULL,
	[ExtValue3] [nvarchar](50) NULL,EventParentId nvarchar(50) null)

--- Declare the local  Cursor varibales
Declare @CUR_Id int, @CUR_FacultyId int, @CUR_FacultyType int,@CUR_DeptId int ,@CUR_AvailabilityType nvarchar(50) ,@CUR_Pattern nvarchar(50) ,@CUR_WorkingDay int ,
	@CUR_WeekNumber int ,@CUR_FromDate datetime ,@CUR_ToDate Datetime ,@CUR_FacilityId int ,@CUR_CorporateId int,
	@CUR_IsActive bit ,@CUR_CreatedBy int ,@CUR_CreatedDate Datetime ,@CUR_RecurreanceDateTo Datetime,@CUR_RecurreanceDateFrom Datetime ,@CUR_ExtValue1 nvarchar(50) ,
	@CUR_ExtValue2 nvarchar(50) ,@CUR_ExtValue3 nvarchar(50),@CUR_EventParentId nvarchar(50)

---- Local variable for the Department timming
Declare @DepartmentTimming Table (OpeningDay nvarchar(5),OpeningTime nvarchar(10),ClosingTime nvarchar(10),TrunAroundTime nvarchar(10))

(select @PhyDeptId =FacultyDepartment,@FacultyType=UserType from Physician where ID = @pPhyId)
SET @PhyDeptId = ISNULL(@PhyDeptId,0)

IF(@PhyDeptId <> 0)
BEGIN
	Insert Into @DepartmentTimming --(OpeningDay,OpeningTime,ClosingTime,TrunAroundTime)
		Select RIGHT(OpeningDayId,1),OpeningTime,ClosingTime,TrunAroundTime from DeptTimming Where FacilityStructureID = @PhyDeptId And IsActive = 1

	If NOt Exists (Select 1 from FacultyRooster where FacultyId = @pPhyId and ExtValue1 = '1')
	BEGIN
	Insert INTO @SchedulingTableTemp
				Select  @pPhyId , @FacultyType ,@PhyDeptId  ,'1'  ,NUll  ,NUll  ,
					NUll  ,NUll  ,NUll  ,@pFId  ,@pCId ,1  ,1  ,@LocalDateTime  ,NUll ,NUll  ,'1'  ,
					'Default Value'  ,NUll ,NUll
	END
END
--- Case for the Daily Check Starts
Declare Cursur_AddScheduling CURSOR for 
Select * from FacultyRooster where AvailabilityType Is NULL and  FacultyId = @pPhyId

Open Cursur_AddScheduling

Fetch Next from Cursur_AddScheduling INTO @CUR_Id,@CUR_FacultyId , @CUR_FacultyType ,@CUR_DeptId  ,@CUR_AvailabilityType  ,@CUR_Pattern  ,@CUR_WorkingDay  ,
	@CUR_WeekNumber  ,@CUR_FromDate  ,@CUR_ToDate  ,@CUR_FacilityId  ,@CUR_CorporateId ,
	@CUR_IsActive  ,@CUR_CreatedBy  ,@CUR_CreatedDate  ,@CUR_RecurreanceDateTo ,@CUR_RecurreanceDateFrom  ,@CUR_ExtValue1  ,
	@CUR_ExtValue2  ,@CUR_ExtValue3 ,@CUR_EventParentId

--- Cursor loop
WHILE @@FETCH_STATUS = 0  
BEGIN 

Delete from @DepartmentTimming
-- Get the Physician department Opening days and Opening time and Closing time for opening days
Insert Into @DepartmentTimming --(OpeningDay,OpeningTime,ClosingTime,TrunAroundTime)
		Select RIGHT(OpeningDayId,1),OpeningTime,ClosingTime,TrunAroundTime from DeptTimming Where FacilityStructureID = @CUR_DeptId And IsActive = 1

Insert into @SplitTable
Select * from dbo.Split('_',@CUR_Pattern)

SET @RecuranceType = (Select Value from @SplitTable where ID = 1)

--- Recurrance type daily starts
IF @RecuranceType = 'day'
BEGIN
	Select @Counter = DATEDIFF(DD,@CUR_RecurreanceDateFrom,@CUR_RecurreanceDateTo)
	Declare @DayDifferance int = (Select CAST(Value as int) from @SplitTable where ID = 2)
	Declare @DailyLoopCounter int =0;

--- Datediff loop
WHILE @Counter >= @DailyLoopCounter
BEGIN

	IF @DailyLoopCounter = 0
	Begin
		SET @pSelectedDate = DateAdd(DD,@DailyLoopCounter,@CUR_FromDate)
		SET @pSelectedDate1 = DateAdd(DD,@DailyLoopCounter,@CUR_ToDate)
	END
	ELSE
	BEGIN
		SET @pSelectedDate = DateAdd(DD,@DayDifferance,@pSelectedDate)
		SET @pSelectedDate1 = DateAdd(DD,@DayDifferance,@pSelectedDate1)
	END
	Set @CurrentWeekDay =  DatePart(DW,@pSelectedDate) 

	---- ********************************** Check for the deptartment opening Days ***************************************** ----
	IF(@CurrentWeekDay -1 in (Select OpeningDay from @DepartmentTimming))
	BEGIN
		-- Insert the Data in the local Table and then Insert the data from Local table to scheduling table at the end of Proc
		IF NOT EXISTS (Select 1 from FacultyRooster where [FacultyId] = @CUR_FacultyId and 
		(FromDate = @pSelectedDate and ToDate = @pSelectedDate1 AND (FromDate Between @pSelectedDate and @pSelectedDate1) AND (ToDate Between @pSelectedDate and @pSelectedDate1)))
		BEGIN
		Set @WeekNumber = DatePart(wk,@pSelectedDate)
			Insert INTO @SchedulingTableTemp
				Select  @CUR_FacultyId , @FacultyType ,@CUR_DeptId  ,'1'  ,@CUR_Pattern  ,@CurrentWeekDay  ,
					@WeekNumber  ,@pSelectedDate  ,@pSelectedDate1  ,@CUR_FacilityId  ,@CUR_CorporateId ,
					@CUR_IsActive  ,@CUR_CreatedBy  ,@CUR_CreatedDate  ,@CUR_RecurreanceDateTo ,@CUR_RecurreanceDateFrom  ,'2'  ,
					'Recurrence'  ,@CUR_ExtValue3 ,@DynamicEventId
		END
	END

	--- Loop counter increases
	Set @DailyLoopCounter = @DailyLoopCounter + @DayDifferance;
END
-- Datediff loop ends
END
--- Recurrance type daily Ends

--- Recurrance type weekly starts
ELSE IF @RecuranceType = 'week'
BEGIN
SET @WeekInterval = (Select Value from @SplitTable where ID =2)
Select @WeekCounter = DATEDIFF(wk,@CUR_RecurreanceDateFrom,@CUR_RecurreanceDateTo) 
-- Comment this
--Select @WeekCounter as 'WeekInterval'
---
Set @pWeeklyAdd = 0;
--- Case for the Weekly Check Starts
WHILE @pWeeklyAdd <= @WeekCounter
BEGIN
	SET @pSelectedDate = DateAdd(wk,@pWeeklyAdd,@CUR_RecurreanceDateFrom) --- Get the Week Date
	SET @pSelectedDate1 = DateAdd(wk,@pWeeklyAdd,@CUR_RecurreanceDateTo) --- Get the Week Date1
	DECLARE @WeekStartDate datetime = DATEADD(dd, -(DATEPART(dw, @pSelectedDate)-1), @pSelectedDate);-- week start date
	DECLARE @WeekStartDate1 datetime = DATEADD(dd, -(DATEPART(dw, @pSelectedDate)-1), @pSelectedDate1);-- week start date
	DECLARE @WeekEndDate datetime = DATEADD(dd, 7-(DATEPART(dw, @pSelectedDate)), @pSelectedDate) -- week end Date
	

	--Select @pWeeklyAdd % @WeekInterval
	IF @pWeeklyAdd % @WeekInterval = 0
	BEGIN
		Declare @WEEKDAY nvarchar(10) = (Select Value from @SplitTable where ID =5)

		--Select * from @SplitTable

		--- CHECK FOR THE SPECIAL CHARACTER IF EXIST THEN GO for Day number Date
		IF (charindex(',', (Select Value from @SplitTable where ID =5)) > 0)
		BEGIN
			Delete from @LocalDateTable
			Insert Into @LocalDateTable
			Select * from dbo.Split(',',(Select Value from @SplitTable where ID =5))


			IF EXISTS (Select 1 from @LocalDateTable)
			BEGIN
				Set @WeekNumber = (Select DATEPART(wk,@pSelectedDate))
				
				--Select @WeekStartDate,@WeekEndDate,@CUR_RecurreanceDateFrom,@CUR_RecurreanceDateTo
				--(Select * from @DepartmentTimming)

				WHILE ((@WeekStartDate <= @WeekEndDate))
				BEGIN
				IF ((DatePart(dw,@WeekStartDate) - 1 IN (Select Value from @LocalDateTable)) 
					and (@WeekStartDate >= @CUR_RecurreanceDateFrom) and @WeekStartDate <= @CUR_RecurreanceDateTo)
				BEGIN
					
					Set @CurrentWeekDay =  DatePart(DW,@WeekStartDate)
					---- ********************************** Check for the deptartment opening Days ***************************************** ----
					IF(@CurrentWeekDay -1 in (Select OpeningDay from @DepartmentTimming))
					BEGIN
						IF NOT EXISTS (Select 1 from FacultyRooster where [FacultyId] = @CUR_FacultyId and 
							(FromDate = @WeekStartDate and ToDate = @WeekStartDate1 AND (FromDate Between @WeekStartDate and @WeekStartDate1) AND (ToDate Between @WeekStartDate and @WeekStartDate1)))
							BEGIN
								Insert INTO @SchedulingTableTemp
									Select  @CUR_FacultyId , @FacultyType ,@CUR_DeptId  ,'1'  ,@CUR_Pattern  ,@CurrentWeekDay - 1  ,
										@CUR_WeekNumber ,CONVERT(DATETIME, CONVERT(CHAR(8), @WeekStartDate, 112) + ' ' + CONVERT(CHAR(8), Dateadd(minute,10,@CUR_FromDate), 108))  
										,CONVERT(DATETIME, CONVERT(CHAR(8), @WeekStartDate, 112) + ' ' + CONVERT(CHAR(8), Dateadd(minute,10,@CUR_ToDate), 108)),@CUR_FacilityId  ,@CUR_CorporateId ,
										@CUR_IsActive  ,@CUR_CreatedBy  ,@CUR_CreatedDate  ,@CUR_RecurreanceDateTo ,@CUR_RecurreanceDateFrom  ,'1'  ,
										'Recurrence'  ,@CUR_ExtValue3,@DynamicEventId
							END
					END
				END
				SET @WeekStartDate = DATEADD(dd,1,@WeekStartDate);
				END
			END
		END
		ELSE
		BEGIN
		-- Comment this 
		--Select @WeekStartDate as '@WeekStartDate',@WeekEndDate as '@WeekEndDate',DatePart(dw,@WeekStartDate) 'Daynumber',@WEEKDAY '@insertedNumber'
			WHILE ((@WeekStartDate <= @WeekEndDate))
			BEGIN
				IF ((DatePart(dw,@WeekStartDate) -1  = @WEEKDAY) and (@WeekStartDate >= @CUR_RecurreanceDateFrom) and @WeekStartDate <= @CUR_RecurreanceDateTo)
				BEGIN
					Set @CurrentWeekDay =  DatePart(DW,@WeekStartDate)
					---- ********************************** Check for the deptartment opening Days ***************************************** ----
					IF(@CurrentWeekDay -1 in (Select OpeningDay from @DepartmentTimming))
					BEGIN
							IF NOT EXISTS (Select 1 from FacultyRooster where [FacultyId] = @CUR_FacultyId and 
							(FromDate = @WeekStartDate and ToDate = @WeekStartDate1 AND (FromDate Between @WeekStartDate and @WeekStartDate1) AND (ToDate Between @WeekStartDate and @WeekStartDate1)))
							BEGIN
								Insert INTO @SchedulingTableTemp
									Select  @CUR_FacultyId , @FacultyType ,@CUR_DeptId  ,'1' ,@CUR_Pattern  ,@CurrentWeekDay - 1  ,
										@CUR_WeekNumber  ,CONVERT(DATETIME, CONVERT(CHAR(8), @WeekStartDate, 112) + ' ' + CONVERT(CHAR(8), Dateadd(minute,10,@CUR_FromDate), 108))  
										,CONVERT(DATETIME, CONVERT(CHAR(8), @WeekStartDate, 112) + ' ' + CONVERT(CHAR(8), Dateadd(minute,10,@CUR_ToDate), 108)) ,@CUR_FacilityId  ,@CUR_CorporateId ,
										@CUR_IsActive  ,@CUR_CreatedBy  ,@CUR_CreatedDate  ,@CUR_RecurreanceDateTo ,@CUR_RecurreanceDateFrom  ,'1'  ,
										'Recurrence'  ,@CUR_ExtValue3,@DynamicEventId
							END
					END
				END
				SET @WeekStartDate = DATEADD(dd,1,@WeekStartDate);
			END
		END
	END

	Set @pWeeklyAdd = @pWeeklyAdd + 1;
END
END
--- Recurrance type weekly Ends

--- Recurrance type weekly starts
ELSE IF @RecuranceType = 'month'
BEGIN
SET @MonthlyInterval = (Select Value from @SplitTable where ID =2)
SET @pMonthlyDay = (Select ISNULL(Value,0) from @SplitTable where ID =3)
SET @pMonthFrequnecy = (Select ISNULL(Value,0) from @SplitTable where ID =4)
Select @pMonthlyCounter = DATEDIFF(MM,@CUR_RecurreanceDateFrom,@CUR_RecurreanceDateTo)
Declare @DateTable table (ID int not null, DateFrom datetime)
Set @pMonhtlyAdd = 0;
--- Case for the MOnthly Check Starts
WHILE @pMonhtlyAdd <= @pMonthlyCounter
BEGIN
	DELETE FROM @DateTable
	SET @pSelectedDate = DateAdd(MM,@pMonhtlyAdd,@CUR_FromDate) 
	DECLARE @pSelectedTime time = Cast (@CUR_FromDate as TIME) 
	Declare @monthDayIndex int = 1
	IF @pMonthlyDay <> 0
	BEGIN
		Declare @MonthStartDateCol date = (CAST (Datepart(MM,@pSelectedDate) as Nvarchar(10))+'-01-'+CAST (Datepart(YYYY,@pSelectedDate) as Nvarchar(10))),
		@MonthEndDate date = EOMONTH((CAST (Datepart(MM,@pSelectedDate) as Nvarchar(10))+'-01-'+CAST (Datepart(YYYY,@pSelectedDate) as Nvarchar(10))))
		--- loop through the month start date and end date starts here
		WHILE (@MonthStartDateCol <= @MonthEndDate )
		BEGIN
			IF datepart(dw,@MonthStartDateCol) = @pMonthlyDay
			BEGIN 
			  INSERT INTO @DateTable 
			  SELECT @monthDayIndex, DATEADD(DAY, 1, @MonthStartDateCol ) 
			  WHERE  datepart(dw,@MonthStartDateCol) = @pMonthlyDay

			  Set @monthDayIndex = @monthDayIndex + 1;
			END
		  Set @MonthStartDateCol = DATEADD(DAY, 1, @MonthStartDateCol ) 
		END
		--- loop through month ended here
		--Select DateFrom from @DateTable

		SET @pSelectedDate = (Select DateFrom from @DateTable Where ID = @pMonthFrequnecy)
		SET @pSelectedDate = CONVERT(DATETIME, CONVERT(CHAR(8), @pSelectedDate, 112)   + ' ' + CONVERT(CHAR(8), @pSelectedTime, 108))
		SET @pSelectedDate1 = CONVERT(DATETIME, CONVERT(CHAR(8), @pSelectedDate, 112)   + ' ' + CONVERT(CHAR(8), @CUR_ToDate, 108))
	END
	
	Set @CurrentWeekDay =  DatePart(DW,@pSelectedDate)
	---- ********************************** Check for the deptartment opening Days ***************************************** ----
	IF(@CurrentWeekDay -1 in (Select OpeningDay from @DepartmentTimming))
	BEGIN
		IF NOT EXISTS (Select 1 from FacultyRooster where [FacultyId] = @CUR_FacultyId and 
			(FromDate = @pSelectedDate and ToDate = @pSelectedDate1 AND (FromDate Between @pSelectedDate and @pSelectedDate1) AND (ToDate Between @pSelectedDate and @pSelectedDate1)))
			BEGIN
				Insert INTO @SchedulingTableTemp
					Select  @CUR_FacultyId , @FacultyType ,@CUR_DeptId  ,'1'  ,@CUR_Pattern  ,@CUR_WorkingDay  ,
						@CUR_WeekNumber  ,CONVERT(DATETIME, CONVERT(CHAR(8), @pSelectedDate, 112) + ' ' + CONVERT(CHAR(8), Dateadd(minute,10,@CUR_FromDate), 108))  
										,CONVERT(DATETIME, CONVERT(CHAR(8), @pSelectedDate, 112) + ' ' + CONVERT(CHAR(8), Dateadd(minute,10,@CUR_ToDate), 108))
						  ,@CUR_FacilityId  ,@CUR_CorporateId ,
						@CUR_IsActive  ,@CUR_CreatedBy  ,@CUR_CreatedDate  ,@CUR_RecurreanceDateTo ,@CUR_RecurreanceDateFrom  ,'2'  ,
						'Recurrence'  ,@CUR_ExtValue3,@DynamicEventId
		END
	END
	Set @pMonhtlyAdd = @pMonhtlyAdd + 1;
END
END
--- Recurrance type MOnthly Ends

Fetch Next from Cursur_AddScheduling INTO @CUR_Id,@CUR_FacultyId , @CUR_FacultyType ,@CUR_DeptId  ,@CUR_AvailabilityType  ,@CUR_Pattern  ,@CUR_WorkingDay  ,
	@CUR_WeekNumber  ,@CUR_FromDate  ,@CUR_ToDate  ,@CUR_FacilityId  ,@CUR_CorporateId ,
	@CUR_IsActive  ,@CUR_CreatedBy  ,@CUR_CreatedDate  ,@CUR_RecurreanceDateTo ,@CUR_RecurreanceDateFrom  ,@CUR_ExtValue1  ,
	@CUR_ExtValue2  ,@CUR_ExtValue3 ,@CUR_EventParentId

END
--- Cursor END
---CLean Up the cursor
CLOSE Cursur_AddScheduling;  
DEALLOCATE Cursur_AddScheduling;

Delete from FacultyRooster where AvailabilityType Is NULL 

Insert into FacultyRooster
Select * from @SchedulingTableTemp

--Select * from @SchedulingTableTemp
END





GO


