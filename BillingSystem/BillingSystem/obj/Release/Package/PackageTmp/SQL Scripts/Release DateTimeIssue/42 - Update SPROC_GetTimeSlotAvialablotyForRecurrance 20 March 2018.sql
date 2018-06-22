IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetTimeSlotAvialablotyForRecurrance')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetTimeSlotAvialablotyForRecurrance
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetTimeSlotAvialablotyForRecurrance]    Script Date: 21-03-2018 10:29:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetTimeSlotAvialablotyForRecurrance]
(
@StartDate Datetime= '02-19-2015', 
@EndDate datetime= '01-19-2016',
@TimeFrom nvarchar(10) ='18:00',
@TimeTo nvarchar(10) ='18:30',
@pFacilityid int = 6,
@pSchedulingId int = 0,
@pPhysicianid int =2036,
@pRec_Pattern nvarchar(20) = 'day_1___'
)
AS
BEGIN
		DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityid))

	Declare @Counter int =0;Declare @pSelectedDate datetime, 
@pDailyCaseFailure int = 0,@pDailyCount int,
@WeekInterval int =0,@WeekCounter int,@pWeeklyCount int,@pWeeklyCaseFailure int = 0,@pWeeklyAdd int,
@RecuranceType nvarchar(10),
@MonthlyInterval int,@pMonthlyCount int,@pMonthlyCounter int,@pMonthlyAdd int,@pMonhtlyCaseFailure int,@pMonhtlyAdd int,@pMonthlyDay int,@pMonthFrequnecy int;

Declare @SplitTable Table(ID int, Value nvarchar(10));

Insert into @SplitTable
Select * from dbo.Split('_',@pRec_Pattern)

SET @RecuranceType = (Select Value from @SplitTable where ID =1)

IF @RecuranceType = 'day'
BEGIN
Select @Counter = DATEDIFF(DD,@StartDate,@EndDate)
--- Case for the Daily Check Starts
WHILE @Counter > 0
BEGIN
	SET @pSelectedDate = DateAdd(DD,@Counter,@LocalDateTime) 

	(Select @pDailyCount=  Count(SchedulingID) 
	From Scheduling  where ((@TimeFrom >= Cast(ScheduleFrom as time) AND
	@TimeFrom <= Cast(ScheduleTo as time)) or (@TimeTo >=  Cast(ScheduleFrom as time) AND @TimeTo <= Cast(ScheduleTo as time)) 
	OR Cast(ScheduleFrom as time) between @TimeFrom AND @TimeTo)
	AND Cast(ScheduleFrom as Date) = Cast(@pSelectedDate as Date)
	AND FacilityId = @pFacilityid and SchedulingId <>  @pSchedulingId
	AND (PhysicianId = @pPhysicianid OR (AssociatedId = @pPhysicianid and AssociatedType = '2') AND AssociatedType <> '3'))

	IF @pDailyCount > 0
		SET @pDailyCaseFailure = @pDailyCount
	ELSE
		SET @pDailyCaseFailure = @pDailyCaseFailure
	Set @Counter = @Counter -1;
END
--- Case for the Daily Check Ends
Select Case when @pDailyCaseFailure > 0 THEN 1 ELSE 0 END as 'TimeSlotAvailable'
END
ELSE IF @RecuranceType = 'week'
BEGIN
SET @WeekInterval = (Select Value from @SplitTable where ID =2)
Select @WeekCounter = DATEDIFF(wk,@StartDate,@EndDate) 
Set @pWeeklyAdd = 0;
--- Case for the WEEKly Check Starts
WHILE @pWeeklyAdd <= @WeekCounter
BEGIN
	SET @pSelectedDate = DateAdd(wk,@pWeeklyAdd,@LocalDateTime) 

	IF @pWeeklyAdd % @WeekInterval = 0
	BEGIN

	--Select @pWeeklyAdd % @WeekInterval,Cast(@pSelectedDate as Date)
		(Select @pWeeklyCount=  Count(SchedulingID) 
		From Scheduling  where ((@TimeFrom >= Cast(ScheduleFrom as time) AND
		@TimeFrom <= Cast(ScheduleTo as time)) or (@TimeTo >=  Cast(ScheduleFrom as time) AND @TimeTo <= Cast(ScheduleTo as time)) 
		OR Cast(ScheduleFrom as time) between @TimeFrom AND @TimeTo)
		AND Cast(ScheduleFrom as Date) = Cast(@pSelectedDate as Date)
		AND FacilityId = @pFacilityid and SchedulingId <>  @pSchedulingId
		AND (PhysicianId = @pPhysicianid OR (AssociatedId = @pPhysicianid and AssociatedType = '2') AND AssociatedType <> '3'))
	END
	
	IF @pWeeklyCount > 0
		SET @pWeeklyCaseFailure = @pWeeklyCount
	ELSE
		SET @pWeeklyCaseFailure = @pDailyCaseFailure

	Set @pWeeklyAdd = @pWeeklyAdd + 1;
END
--- Case for the WEEKly Check Ends
Select Case when @pWeeklyCaseFailure > 0 THEN 1 ELSE 0 END as 'TimeSlotAvailable'
END
ELSE IF @RecuranceType = 'month'
BEGIN
SET @MonthlyInterval = (Select Value from @SplitTable where ID =2)
SET @pMonthlyDay = (Select ISNULL(Value,0) from @SplitTable where ID =3)
SET @pMonthFrequnecy = (Select ISNULL(Value,0) from @SplitTable where ID =4)
Select @pMonthlyCounter = DATEDIFF(MM,@StartDate,@EndDate)
Declare @DateTable table(ID int not null, DateFrom datetime)
Set @pMonhtlyAdd = 0;
--- Case for the MOnthly Check Starts
WHILE @pMonhtlyAdd <= @pMonthlyCounter
BEGIN
	DELETE FROM @DateTable
	SET @pSelectedDate = DateAdd(MM,@pMonhtlyAdd,@LocalDateTime) 
	Declare @monthDayIndex int = 1
	IF @pMonthlyDay <> 0
	BEGIN
		Declare @MonthStartDateCol date = (CAST (Datepart(MM,@pSelectedDate) as Nvarchar(10))+'-01-'+CAST (Datepart(YYYY,@pSelectedDate) as Nvarchar(10))),
		@MonthEndDate date = EOmonth((CAST (Datepart(MM,@pSelectedDate) as Nvarchar(10))+'-01-'+CAST (Datepart(YYYY,@pSelectedDate) as Nvarchar(10))))
		
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

		SET @pSelectedDate = (Select DateFrom from  @DateTable Where ID = @pMonthFrequnecy)
	END


		(Select @pMonthlyCount=  Count(SchedulingID) 
		From Scheduling  where ((@TimeFrom >= Cast(ScheduleFrom as time) AND
		@TimeFrom <= Cast(ScheduleTo as time)) or (@TimeTo >=  Cast(ScheduleFrom as time) AND @TimeTo <= Cast(ScheduleTo as time)) 
		OR Cast(ScheduleFrom as time) between @TimeFrom AND @TimeTo)
		AND Cast(ScheduleFrom as Date) = Cast(@pSelectedDate as Date)
		AND FacilityId = @pFacilityid and SchedulingId <>  @pSchedulingId
		AND (PhysicianId = @pPhysicianid OR (AssociatedId = @pPhysicianid and AssociatedType = '2') AND AssociatedType <> '3'))

	IF @pMonthlyCount > 0
		SET @pMonhtlyCaseFailure = @pMonthlyCount
	ELSE
		SET @pMonhtlyCaseFailure = @pMonhtlyCaseFailure

	Set @pMonhtlyAdd = @pMonhtlyAdd + 1;
END
--- Case for the MOnthly Check Ends

Select Case when @pMonthlyCount > 0 THEN 1 ELSE 0 END as 'TimeSlotAvailable'
END
END





GO


