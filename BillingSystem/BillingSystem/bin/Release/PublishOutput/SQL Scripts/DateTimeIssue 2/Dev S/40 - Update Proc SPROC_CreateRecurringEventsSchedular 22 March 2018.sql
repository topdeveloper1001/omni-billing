IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CreateRecurringEventsSchedular')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CreateRecurringEventsSchedular
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CreateRecurringEventsSchedular]    Script Date: 22-03-2018 20:02:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Shashank>
-- Create date: <Create Date,11/27/2015 12:22:21 PM >
-- Description:	<Description, Create the Recurring visit for the pattern user have added for the Recurring visits>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_CreateRecurringEventsSchedular]
(
	@pSchedulingId int =1720
)
AS
BEGIN
		DECLARE @Facility_Id int=(select FacilityId from dbo.Scheduling where SchedulingId=@pSchedulingId)

		Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))

SET DATEFIRST 1;
--Declare @pSchedulingId int = 12
--- Split pattern table
Declare @SplitTable Table(ID int, Value nvarchar(10));
Declare @LocalDateTable Table(ID int, Value nvarchar(10));
--Declare Local Variables
Declare @Counter int =0;Declare @pSelectedDate datetime, 
@pDailyCaseFailure int = 0,@pDailyCount int,
@WeekInterval int =0,@WeekCounter int,@pWeeklyCount int,@pWeeklyCaseFailure int = 0,@pWeeklyAdd int,
@RecuranceType nvarchar(10),@CurrentWeekDay int,
@MonthlyInterval int,@pMonthlyCount int,@pMonthlyCounter int,@pMonthlyAdd int,@pMonhtlyCaseFailure int,@pMonhtlyAdd int,@pMonthlyDay int,@pMonthFrequnecy int;

--- Dynamic Event Id
DECLARE @DynamicEventId nvarchar(50) =  '1766' + CAST(floor(rand() * 100000 - 1)  as Nvarchar(20))
print(@DynamicEventId)
Declare @PhyDeptId int, @PhyidH int;

---- Local variable for the Department timming
Declare @DepartmentTimming Table (OpeningDay nvarchar(5),OpeningTime nvarchar(10),ClosingTime nvarchar(10),TrunAroundTime nvarchar(10))

--- Scheduling Temp table
Declare @SchedulingTableTemp Table(
    [AssociatedId] [int] NULL,[AssociatedType] [nvarchar](50) NULL,[SchedulingType] [nvarchar](50) NULL,[Status] [nvarchar](50) NULL,[StatusType] [nvarchar](50) NULL,
	[ScheduleFrom] [datetime] NULL,[ScheduleTo] [datetime] NULL,[TypeOfVisit] [nvarchar](100) NULL,[PhysicianSpeciality] [nvarchar](100) NULL,[PhysicianId] [nvarchar](100) NULL,
	[TypeOfProcedure] [nvarchar](100) NULL,[FacilityId] [int] NULL,[CorporateId] [int] NULL,[Comments] [nvarchar](500) NULL,[Location] [nvarchar](500) NULL,[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,[ModifiedBy] [int] NULL,[ModifiedDate] [datetime] NULL,[IsActive] [bit] NULL,[DeletedBy] [int] NULL,[DeletedDate] [datetime] NULL,
	[ExtValue1] [nvarchar](200) NULL,[ExtValue2] [nvarchar](200) NULL,[ExtValue3] [nvarchar](200) NULL,[ExtValue4] [nvarchar](200) NULL,[ExtValue5] [nvarchar](200) NULL,
	[IsRecurring] [bit] NULL,[RecType] [nvarchar](50) NULL,[RecPattern] [nvarchar](50) NULL,[RecEventlength] [bigint] NULL,[RecEventPId] [int] NULL,[RecurringDateFrom] [datetime] NULL,
	[RecurringDateTill] [datetime] NULL,[EventId] [nvarchar](50) NULL,[WeekDay] [nvarchar](10) NULL,[EventParentId] [nvarchar](50) NULL,[RoomAssigned] [int] NULL,
	[EquipmentAssigned] [int] NULL, PhysicianReferredBy bigint)


--- Holidays Temp Table 
Declare @HolidaysTableTemp Table(
    [SchedulingId] int, [AssociatedId] [int] NULL,[AssociatedType] [nvarchar](50) NULL,[SchedulingType] [nvarchar](50) NULL,[Status] [nvarchar](50) NULL,[StatusType] [nvarchar](50) NULL,
	[ScheduleFrom] [datetime] NULL,[ScheduleTo] [datetime] NULL,[TypeOfVisit] [nvarchar](100) NULL,[PhysicianSpeciality] [nvarchar](100) NULL,[PhysicianId] [nvarchar](100) NULL,
	[TypeOfProcedure] [nvarchar](100) NULL,[FacilityId] [int] NULL,[CorporateId] [int] NULL,[Comments] [nvarchar](500) NULL,[Location] [nvarchar](500) NULL,[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,[ModifiedBy] [int] NULL,[ModifiedDate] [datetime] NULL,[IsActive] [bit] NULL,[DeletedBy] [int] NULL,[DeletedDate] [datetime] NULL,
	[ExtValue1] [nvarchar](200) NULL,[ExtValue2] [nvarchar](200) NULL,[ExtValue3] [nvarchar](200) NULL,[ExtValue4] [nvarchar](200) NULL,[ExtValue5] [nvarchar](200) NULL,
	[IsRecurring] [bit] NULL,[RecType] [nvarchar](50) NULL,[RecPattern] [nvarchar](50) NULL,[RecEventlength] [bigint] NULL,[RecEventPId] [int] NULL,[RecurringDateFrom] [datetime] NULL,
	[RecurringDateTill] [datetime] NULL,[EventId] [nvarchar](50) NULL,[WeekDay] [nvarchar](10) NULL,[EventParentId] [nvarchar](50) NULL,[RoomAssigned] [int] NULL,
	[EquipmentAssigned] [int] NULL, PhysicianReferredBy bigint)

--- Declare the local  Cursor varibales
Declare @CUR_SchedulingId int, @CUR_AssociatedId int,@CUR_AssociatedType nvarchar(50) ,@CUR_SchedulingType nvarchar(50) ,@CUR_Status nvarchar(50) ,@CUR_StatusType nvarchar(50) ,
	@CUR_ScheduleFrom datetime ,@CUR_ScheduleTo datetime ,@CUR_TypeOfVisit nvarchar(100) ,@CUR_PhysicianSpeciality nvarchar(100) ,@CUR_PhysicianId nvarchar(100) ,
	@CUR_TypeOfProcedure nvarchar(100) ,@CUR_FacilityId int ,@CUR_CorporateId int ,@CUR_Comments nvarchar(500) ,@CUR_Location nvarchar(500) ,@CUR_CreatedBy int ,
	@CUR_CreatedDate datetime ,@CUR_ModifiedBy int ,@CUR_ModifiedDate datetime ,@CUR_IsActive bit ,@CUR_DeletedBy int ,@CUR_DeletedDate datetime ,
	@CUR_ExtValue1 nvarchar(200) ,@CUR_ExtValue2 nvarchar(200) ,@CUR_ExtValue3 nvarchar(200) ,@CUR_ExtValue4 nvarchar(200) ,@CUR_ExtValue5 nvarchar(200) ,
	@CUR_IsRecurring bit ,@CUR_RecType nvarchar(50) ,@CUR_RecPattern nvarchar(50) ,@CUR_RecEventlength bigint ,@CUR_RecEventPId int ,@CUR_RecurringDateFrom datetime ,
	@CUR_RecurringDateTill datetime ,@CUR_EventId nvarchar(50) ,@CUR_WeekDay nvarchar(10) ,@CUR_EventParentId nvarchar(50) ,@CUR_RoomAssigned int ,
	@CUR_EquipmentAssigned int, @PhysicianReferredBy bigint;

--Select * from Scheduling where SchedulingId = @pSchedulingId

--- Case for the Daily Check Starts
Declare Cursur_AddScheduling CURSOR for 
Select * from Scheduling S where S.SchedulingId = @pSchedulingId

Open Cursur_AddScheduling

Fetch Next from Cursur_AddScheduling INTO @CUR_SchedulingId,@CUR_AssociatedId,@CUR_AssociatedType,@CUR_SchedulingType,@CUR_Status,@CUR_StatusType,@CUR_ScheduleFrom
,@CUR_ScheduleTo,@CUR_TypeOfVisit,@CUR_PhysicianSpeciality,@CUR_PhysicianId,@CUR_TypeOfProcedure,@CUR_FacilityId,@CUR_CorporateId,@CUR_Comments,@CUR_Location,@CUR_CreatedBy
,@CUR_CreatedDate,@CUR_ModifiedBy,@CUR_ModifiedDate,@CUR_IsActive,@CUR_DeletedBy,@CUR_DeletedDate,@CUR_ExtValue1,@CUR_ExtValue2,@CUR_ExtValue3,@CUR_ExtValue4,@CUR_ExtValue5
,@CUR_IsRecurring,@CUR_RecType,@CUR_RecPattern,@CUR_RecEventlength,@CUR_RecEventPId,@CUR_RecurringDateFrom,@CUR_RecurringDateTill,@CUR_EventId,@CUR_WeekDay,@CUR_EventParentId
,@CUR_RoomAssigned,@CUR_EquipmentAssigned,@PhysicianReferredBy

--- Cursor loop
WHILE @@FETCH_STATUS = 0  
BEGIN 
-- Get Physician Dept Id to add the check for the Dept opening and closing time/Day
Set @PhyDeptId = (select FacultyDepartment from Physician where ID = @CUR_PhysicianId)
SET @PhyDeptId = ISNULL(@PhyDeptId,0)
Set @PhyidH = ISNULL(@CUR_PhysicianId,0)

-- Get the Physician department Opening days and Opening time and Closing time for opening days
Insert Into @DepartmentTimming --(OpeningDay,OpeningTime,ClosingTime,TrunAroundTime)
		Select RIGHT(OpeningDayId,1),OpeningTime,ClosingTime,TrunAroundTime from DeptTimming Where FacilityStructureID = @PhyDeptId And IsActive = 1

Insert into @SplitTable
Select * from dbo.Split('_',@CUR_RecPattern)

SET @RecuranceType = (Select Value from @SplitTable where ID = 1)
--- Recurrance type daily starts
IF @RecuranceType = 'day'
BEGIN

	Select @Counter = DATEDIFF(DD,@CUR_RecurringDateFrom,@CUR_RecurringDateTill)
	Declare @DayDifferance int = (Select CAST(Value as int) from @SplitTable where ID = 2)
	Declare @DailyLoopCounter int =0;
	
--- Datediff loop
WHILE @Counter >= @DailyLoopCounter
BEGIN

	IF @DailyLoopCounter = 0
		SET @pSelectedDate = DateAdd(DD,@DailyLoopCounter,@CUR_RecurringDateFrom)
	ELSE
		SET @pSelectedDate = DateAdd(DD,@DayDifferance,@pSelectedDate)
	
	Set @CurrentWeekDay =  DatePart(DW,@pSelectedDate)
	--Print @pSelectedDate
	--Print @CurrentWeekDay
	--- dynamic Event ID
	
	
	WHile ((SELECT Count(1) FROM [Scheduling] WHERE [EventId] = @DynamicEventId) > 0 OR (SELECT Count(1) FROM @SchedulingTableTemp WHERE [EventId] = @DynamicEventId) > 0)
	BEGIN
		SET @DynamicEventId =  '1766' + CAST(floor(rand() * 100000 - 1)  as Nvarchar(20))
		END

	---- ********************************** Check for the deptartment opening Days ***************************************** ----
	
	IF(@CurrentWeekDay in (Select OpeningDay from @DepartmentTimming))
	BEGIN
	If (DATEADD(ss,@CUR_RecEventlength,@pSelectedDate) >@LocalDateTime)
		
		BEGIN
		
			-- Insert the Data in the local Table and then Insert the data from Local table to scheduling table at the end of Proc
			IF NOT  EXISTS (Select 1 from Scheduling where [PhysicianId] = @CUR_PhysicianId and ScheduleFrom = @pSelectedDate and ScheduleTo = DATEADD(ss,@CUR_RecEventlength,@pSelectedDate))
			BEGIN
			
				Insert INTO @SchedulingTableTemp
					Select @CUR_AssociatedId,@CUR_AssociatedType,@CUR_SchedulingType,@CUR_Status,@CUR_StatusType,@pSelectedDate,DATEADD(ss,@CUR_RecEventlength,@pSelectedDate),@CUR_TypeOfVisit
					,@CUR_PhysicianSpeciality,@CUR_PhysicianId,@CUR_TypeOfProcedure,@CUR_FacilityId,@CUR_CorporateId,@CUR_Comments,@CUR_Location,@CUR_CreatedBy,@CUR_CreatedDate,@CUR_ModifiedBy,@CUR_ModifiedDate,@CUR_IsActive,@CUR_DeletedBy,@CUR_DeletedDate
					,@CUR_ExtValue1,@CUR_ExtValue2,@CUR_ExtValue3,@CUR_ExtValue4,@CUR_ExtValue5
					,@CUR_IsRecurring,@CUR_RecType,@CUR_RecPattern,@CUR_RecEventlength,@CUR_RecEventPId,@CUR_RecurringDateFrom,@CUR_RecurringDateTill,@DynamicEventId,@CUR_WeekDay,@CUR_EventParentId
					,@CUR_RoomAssigned,@CUR_EquipmentAssigned,@PhysicianReferredBy
			END
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
Select @WeekCounter = DATEDIFF(wk,@CUR_RecurringDateFrom,@CUR_RecurringDateTill) 
-- Comment this
--Select @WeekCounter as 'WeekInterval'
---
Set @pWeeklyAdd = 0;
--- Case for the Weekly Check Starts
WHILE @pWeeklyAdd <= @WeekCounter
BEGIN
	SET @pSelectedDate = DateAdd(wk,@pWeeklyAdd,@CUR_RecurringDateFrom) --- Get the Week Date
	DECLARE @WeekStartDate datetime = DATEADD(dd, -(DATEPART(dw, @pSelectedDate)-1), @pSelectedDate);-- week start date
	DECLARE @WeekEndDate datetime = DATEADD(dd, 7-(DATEPART(dw, @pSelectedDate)), @pSelectedDate) -- week end Date
	--- dynamic Event ID
	WHile ((SELECT Count(1) FROM [Scheduling] WHERE [EventId] = @DynamicEventId) > 0 OR (SELECT Count(1) FROM @SchedulingTableTemp WHERE [EventId] = @DynamicEventId) > 0)
	BEGIN
		SET @DynamicEventId =  '1766' + CAST(floor(rand() * 100000 - 1)  as Nvarchar(20))
	END
	IF @pWeeklyAdd % @WeekInterval = 0
	BEGIN
		Declare @WEEKDAY nvarchar(10) = (Select Value from @SplitTable where ID =5)

		--- CHECK FOR THE SPECIAL CHARACTER IF EXIST THEN GO for Day number Date
		IF (charindex(',', (Select Value from @SplitTable where ID =5)) > 0)
		BEGIN
			Delete from @LocalDateTable
			Insert Into @LocalDateTable
			Select * from dbo.Split(',',(Select Value from @SplitTable where ID =5))


			IF EXISTS (Select 1 from @LocalDateTable)
			BEGIN
				Declare @WeekNumber int = (Select DATEPART(wk,@pSelectedDate))
				
				---------------------------------------------------------------- commneted code
				--Select @WeekStartDate,@WeekEndDate
				--Select (DatePart(dw,@WeekStartDate)),DatePart(DW,@WeekStartDate)-1,@WeekStartDate,@WeekEndDate,* from @LocalDateTable
				---------------------------------------------------------------- commneted code

				WHILE ((@WeekStartDate <= @WeekEndDate))
				BEGIN
				IF ((DatePart(DW,@WeekStartDate)  IN (Select Value from @LocalDateTable)) 
					and (@WeekStartDate >= @CUR_RecurringDateFrom) and @WeekStartDate <= @CUR_RecurringDateTill)
				BEGIN
					--- dynamic Event ID
					WHile ((SELECT Count(1) FROM [Scheduling] WHERE [EventId] = @DynamicEventId) > 0 OR (SELECT Count(1) FROM @SchedulingTableTemp WHERE [EventId] = @DynamicEventId) > 0)
					BEGIN
						SET @DynamicEventId =  '1766' + CAST(floor(rand() * 100000 - 1)  as Nvarchar(20))
					END
					Set @CurrentWeekDay =  DatePart(DW,@WeekStartDate)
					---- ********************************** Check for the deptartment opening Days ***************************************** ----
					IF(@CurrentWeekDay  in (Select OpeningDay from @DepartmentTimming))
					BEGIN
						If (DATEADD(ss,@CUR_RecEventlength,@WeekStartDate) > @LocalDateTime)
						BEGIN
							IF NOT EXISTS (Select 1 from Scheduling where [PhysicianId] = @CUR_PhysicianId and ScheduleFrom = @WeekStartDate and ScheduleTo = DATEADD(ss,@CUR_RecEventlength,@WeekStartDate))
							BEGIN
								Insert INTO @SchedulingTableTemp
									Select @CUR_AssociatedId,@CUR_AssociatedType,@CUR_SchedulingType,@CUR_Status,@CUR_StatusType,@WeekStartDate,DATEADD(ss,@CUR_RecEventlength,@WeekStartDate),@CUR_TypeOfVisit
									,@CUR_PhysicianSpeciality,@CUR_PhysicianId,@CUR_TypeOfProcedure,@CUR_FacilityId,@CUR_CorporateId,@CUR_Comments,@CUR_Location,@CUR_CreatedBy,@CUR_CreatedDate,@CUR_ModifiedBy,@CUR_ModifiedDate,@CUR_IsActive,@CUR_DeletedBy,@CUR_DeletedDate
									,@CUR_ExtValue1,@CUR_ExtValue2,@CUR_ExtValue3,@CUR_ExtValue4,@CUR_ExtValue5
									,@CUR_IsRecurring,@CUR_RecType,@CUR_RecPattern,@CUR_RecEventlength,@CUR_RecEventPId,@CUR_RecurringDateFrom,@CUR_RecurringDateTill,@DynamicEventId,@CUR_WeekDay,@CUR_EventParentId
									,@CUR_RoomAssigned,@CUR_EquipmentAssigned,@PhysicianReferredBy
							END
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
				IF ((DatePart(dw,@WeekStartDate)   = @WEEKDAY) and (@WeekStartDate >= @CUR_RecurringDateFrom) and @WeekStartDate <= @CUR_RecurringDateTill)
				BEGIN
					Set @CurrentWeekDay =  DatePart(DW,@WeekStartDate)

					--select 'WeekDay Current',@CurrentWeekDay,'StartDate',@WeekStartDate,* from @DepartmentTimming

					---- ********************************** Check for the deptartment opening Days ***************************************** ----
					IF(@CurrentWeekDay in (Select OpeningDay from @DepartmentTimming))
					BEGIN
						If (DATEADD(ss,@CUR_RecEventlength,@WeekStartDate) > @LocalDateTime)
						BEGIN
							IF NOT  EXISTS (Select 1 from Scheduling where [PhysicianId] = @CUR_PhysicianId and ScheduleFrom = @WeekStartDate and ScheduleTo = DATEADD(ss,@CUR_RecEventlength,@WeekStartDate))
							BEGIN
								Insert INTO @SchedulingTableTemp
									Select @CUR_AssociatedId,@CUR_AssociatedType,@CUR_SchedulingType,@CUR_Status,@CUR_StatusType,@WeekStartDate,DATEADD(ss,@CUR_RecEventlength,@WeekStartDate),@CUR_TypeOfVisit
									,@CUR_PhysicianSpeciality,@CUR_PhysicianId,@CUR_TypeOfProcedure,@CUR_FacilityId,@CUR_CorporateId,@CUR_Comments,@CUR_Location,@CUR_CreatedBy,@CUR_CreatedDate,@CUR_ModifiedBy,@CUR_ModifiedDate,@CUR_IsActive,@CUR_DeletedBy,@CUR_DeletedDate
									,@CUR_ExtValue1,@CUR_ExtValue2,@CUR_ExtValue3,@CUR_ExtValue4,@CUR_ExtValue5
									,@CUR_IsRecurring,@CUR_RecType,@CUR_RecPattern,@CUR_RecEventlength,@CUR_RecEventPId,@CUR_RecurringDateFrom,@CUR_RecurringDateTill,@DynamicEventId,@CUR_WeekDay,@CUR_EventParentId
									,@CUR_RoomAssigned,@CUR_EquipmentAssigned,@PhysicianReferredBy
							END
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
Select @pMonthlyCounter = DATEDIFF(MM,@CUR_RecurringDateFrom,@CUR_RecurringDateTill)

--Select @CUR_RecurringDateFrom '@CUR_RecurringDateFrom',@CUR_RecurringDateFrom '@CUR_RecurringDateFrom'

Declare @DateTable table (ID int not null, DateFrom datetime)
Set @pMonhtlyAdd = 0;
--- Case for the MOnthly Check Starts
WHILE @pMonhtlyAdd <= @pMonthlyCounter
BEGIN

	DELETE FROM @DateTable
	SET @pSelectedDate = DateAdd(MM,@pMonhtlyAdd,@CUR_ScheduleFrom) 
	DECLARE @pSelectedTime time = Cast (@CUR_RecurringDateFrom as TIME) 
	Declare @monthDayIndex int = 1

	--Select DateAdd(MM,@pMonhtlyAdd,@CUR_ScheduleFrom) 'PP',@pSelectedTime 'ST',@pMonthlyDay
	SET @pSelectedDate = CONVERT(DATETIME, CONVERT(CHAR(8), @pSelectedDate, 112)   + ' ' + CONVERT(CHAR(8), @pSelectedTime, 108))

	IF @pMonthlyDay <> 0
	BEGIN
		--Declare @MonthStartDateCol date = (CAST (Datepart(MM,@pSelectedDate) as Nvarchar(10))+'-01-'+CAST (Datepart(YYYY,@pSelectedDate) as Nvarchar(10))),
		--@MonthEndDate date = EOMONTH((CAST (Datepart(MM,@pSelectedDate) as Nvarchar(10))+'-01-'+CAST (Datepart(YYYY,@pSelectedDate) as Nvarchar(10))))

		Declare @MonthStartDateCol date = (CAST (MONTH(@pSelectedDate) as Nvarchar(10))+'-01-'+CAST (Year(@pSelectedDate) as Nvarchar(10))),
		@MonthEndDate date = EOMONTH((CAST (MONTH(@pSelectedDate) as Nvarchar(10))+'-01-'+CAST (Year(@pSelectedDate) as Nvarchar(10))))
		--- loop through the month start date and end date starts here
		WHILE (@MonthStartDateCol <= @MonthEndDate )
		BEGIN
			IF datepart(dw,@MonthStartDateCol) = @pMonthlyDay
			BEGIN 
			  INSERT INTO @DateTable 
			  SELECT @monthDayIndex,@MonthStartDateCol-- DATEADD(DAY, 1, @MonthStartDateCol ) 
			  WHERE  datepart(dw,@MonthStartDateCol) = @pMonthlyDay

			  Set @monthDayIndex = @monthDayIndex + 1;
			END
		  Set @MonthStartDateCol = DATEADD(DAY, 1, @MonthStartDateCol ) 
		END
		--- loop through month ended here
		--Select DateFrom from @DateTable


		SET @pSelectedDate = (Select DateFrom from @DateTable Where ID = @pMonthFrequnecy)
		SET @pSelectedDate = CONVERT(DATETIME, CONVERT(CHAR(8), @pSelectedDate, 112)   + ' ' + CONVERT(CHAR(8), @pSelectedTime, 108))
	END
	--- dynamic Event ID
	WHile ((SELECT Count(1) FROM [Scheduling] WHERE [EventId] = @DynamicEventId) > 0 OR (SELECT Count(1) FROM @SchedulingTableTemp WHERE [EventId] = @DynamicEventId) > 0)
	BEGIN
		SET @DynamicEventId =  '1766' + CAST(floor(rand() * 100000 - 1)  as Nvarchar(20))
	END
	
	--select 'selecteddate',@pSelectedDate,'Day',DatePart(DW,@pSelectedDate),* from @DepartmentTimming 

	Set @CurrentWeekDay =  DatePart(DW,@pSelectedDate)
	---- ********************************** Check for the deptartment opening Days ***************************************** ----
	IF(@CurrentWeekDay in (Select OpeningDay from @DepartmentTimming))
	BEGIN
		If (DATEADD(ss,@CUR_RecEventlength,@pSelectedDate) > @LocalDateTime)
		BEGIN
			IF NOT EXISTS (Select 1 from Scheduling where [PhysicianId] = @CUR_PhysicianId and ScheduleFrom = @WeekStartDate and ScheduleTo = DATEADD(ss,@CUR_RecEventlength,@WeekStartDate))
			BEGIN
				 Insert INTO @SchedulingTableTemp
						Select @CUR_AssociatedId,@CUR_AssociatedType,@CUR_SchedulingType,@CUR_Status,@CUR_StatusType,@pSelectedDate,DATEADD(ss,@CUR_RecEventlength,@pSelectedDate),@CUR_TypeOfVisit
						,@CUR_PhysicianSpeciality,@CUR_PhysicianId,@CUR_TypeOfProcedure,@CUR_FacilityId,@CUR_CorporateId,@CUR_Comments,@CUR_Location,@CUR_CreatedBy,@CUR_CreatedDate,@CUR_ModifiedBy,@CUR_ModifiedDate,@CUR_IsActive,@CUR_DeletedBy,@CUR_DeletedDate
						,@CUR_ExtValue1,@CUR_ExtValue2,@CUR_ExtValue3,@CUR_ExtValue4,@CUR_ExtValue5
						,@CUR_IsRecurring,@CUR_RecType,@CUR_RecPattern,@CUR_RecEventlength,@CUR_RecEventPId,@CUR_RecurringDateFrom,@CUR_RecurringDateTill,@DynamicEventId,@CUR_WeekDay,@CUR_EventParentId
						,@CUR_RoomAssigned,@CUR_EquipmentAssigned,@PhysicianReferredBy
			END
		END
	END
	Set @pMonhtlyAdd = @pMonhtlyAdd + 1;
END
END
--- Recurrance type weekly Ends

Fetch Next from Cursur_AddScheduling INTO @CUR_SchedulingId,@CUR_AssociatedId,@CUR_AssociatedType,@CUR_SchedulingType,@CUR_Status,@CUR_StatusType,@CUR_ScheduleFrom,
@CUR_ScheduleTo,@CUR_TypeOfVisit,@CUR_PhysicianSpeciality,@CUR_PhysicianId,@CUR_TypeOfProcedure,@CUR_FacilityId,@CUR_CorporateId,@CUR_Comments,@CUR_Location,@CUR_CreatedBy,
@CUR_CreatedDate,@CUR_ModifiedBy,@CUR_ModifiedDate,@CUR_IsActive,@CUR_DeletedBy,@CUR_DeletedDate,@CUR_ExtValue1,@CUR_ExtValue2,@CUR_ExtValue3,@CUR_ExtValue4,@CUR_ExtValue5
,@CUR_IsRecurring,@CUR_RecType,@CUR_RecPattern,@CUR_RecEventlength,@CUR_RecEventPId,@CUR_RecurringDateFrom,@CUR_RecurringDateTill,@CUR_EventId,@CUR_WeekDay,@CUR_EventParentId
,@CUR_RoomAssigned,@CUR_EquipmentAssigned,@PhysicianReferredBy

END
--- Cursor END
---CLean Up the cursor
CLOSE Cursur_AddScheduling;  
DEALLOCATE Cursur_AddScheduling;

--- Implement the Holiday check here
Insert into @HolidaysTableTemp
	Select * from scheduling where AssociatedId = @PhyidH and AssociatedType in ('2')

--Select * from @SchedulingTableTemp
--Select * from @HolidaysTableTemp

--Delete From Scheduling where SchedulingId = @pSchedulingId
select * from @SchedulingTableTemp
-- Implement the Holiday check here
Insert into Scheduling
	Select STT.* from @SchedulingTableTemp STT 
	Where CAST (STT.[ScheduleFrom] as Date) not IN  (Select Cast(ScheduleFrom as Date) from @HolidaysTableTemp)
	and STT.[ScheduleTo] not IN (Select Cast(ScheduleTo as Date) from @HolidaysTableTemp)


Delete From Scheduling where SchedulingId = @pSchedulingId

--Insert into Scheduling
--Select * from @SchedulingTableTemp
--Select STT.* from @SchedulingTableTemp STT 
--Where CAST (STT.[ScheduleFrom] as Date) not IN  (Select Cast(ScheduleFrom as Date) from @HolidaysTableTemp)
--and STT.[ScheduleTo] not IN (Select Cast(ScheduleTo as Date) from @HolidaysTableTemp)

--Select * from @SchedulingTableTemp
END
GO


