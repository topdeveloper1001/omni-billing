IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_AddUpdateFacultyLunchTimming_1')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_AddUpdateFacultyLunchTimming_1

/****** Object:  StoredProcedure [dbo].[SPROC_AddUpdateFacultyLunchTimming_1]    Script Date: 3/22/2018 7:51:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[SPROC_AddUpdateFacultyLunchTimming_1]  -- SPROC_AddUpdateFacultyLunchTimming 6,4,2118,377,'12:30','14:30'
(
@pCID int,
@pFID int,
@pUserId int,
@pDeptId int,
@pLunchTimeFrom nvarchar(10),
@pLunchTimeTill nvarchar(10)
)
AS
BEGIN
	---- Check to be added we Need to add the check for the date range Iff any
	---- Need to get the Faculty Working days and Add the Data only for the Open slots
	---- Need to Add the Data only in open slots For the Department as for the Department Timming
	SET DATEFIRST 1;--- Use this to set the Week Start Day as Monday for the Sql Datetime Operation to be performed,
	---- Local variable for the Department timming
	Declare @DepartmentTimming Table (OpeningDay nvarchar(5),OpeningTime nvarchar(10),ClosingTime nvarchar(10),TrunAroundTime nvarchar(10))

	Declare @PhysicianLunchTimeFrom nvarchar(10),@PhysicianLunchTimeTill nvarchar(10),@FacultyType int;
	--**********************************************************************************************************************************
	Declare @WeekNumberToAdd nvarchar(10),@AvialableDateFrom Datetime,@AvialableDateTill Datetime,@DynamicEventId nvarchar(50)
	--**********************************************************************************************************************************
	Declare @CurrentDate Datetime = (Select dbo.GetCurrentDatetimeByEntity(@pFID));
	Declare @CurrentMonth int = Month(@CurrentDate);
	Declare @FirstDateOfMonth datetime = Cast(Cast( YEAR(@CurrentDate) as nvarchar(10)) + ' - '+ cast(@CurrentMonth as nvarchar(10)) +' - ' + '01' as Datetime)
	Declare @CurrentDaysInMonth int =  DAY(EOMONTH(@CurrentDate)) 
	Declare @CurrentDays int =  DAY((@CurrentDate)) ;
	Declare @CurrentWeekNumber int =  datepart(Wk, @CurrentDate);
	Declare @WeekDay int
	---- GET THE Physician Lunch Timming from the Physician Table
	Select @PhysicianLunchTimeFrom= ISNULL( FacultyLunchTimeFrom,'') , @PhysicianLunchTimeTill = ISNULL(FacultyLunchTimeTill,''),@FacultyType= UserType from Physician
	Where UserId = @pUserId and IsActive = 1

	DECLARE @FromTime TIME = CAST(@PhysicianLunchTimeFrom+':00' as TIME) , @TillTime TIME = CAST(@PhysicianLunchTimeTill+':00' as TIME)

	--- Check if the Lunch Time is not null or Equal to __:__ for data to add in DB.
	IF( @PhysicianLunchTimeFrom <> '' and @PhysicianLunchTimeFrom <> '__:__')
	BEGIN
		--- Insert the Data For the Department timming 
		Insert Into @DepartmentTimming --(OpeningDay,OpeningTime,ClosingTime,TrunAroundTime)
		Select OpeningDayId,OpeningTime,ClosingTime,TrunAroundTime from DeptTimming Where FacilityStructureID = @pDeptId And IsActive = 1

		--Select * from @DepartmentTimming

		IF((Select Count(1) from @DepartmentTimming) > 0)
		BEGIN
			-- Declare Cursor for the data to be Added in Timeslots for the Lunch time of Selected Faculty for one month BY Default
			-- With check of the Department SHould be open in that Day
			While @CurrentDays <= @CurrentDaysInMonth
			BEGIN
				SET @AvialableDateFrom = DATEADD(day,@CurrentDays,@FirstDateOfMonth); SET @AvialableDateTill = DATEADD(day,@CurrentDays,@FirstDateOfMonth)
				SET @WeekNumberToAdd =  DATEPART(ISOWk, @AvialableDateFrom)
				SET @WeekDay =  ((DATEPART(dw, @AvialableDateFrom)+5)%7)+2;

				SET @AvialableDateFrom = CONVERT(DATETIME, CONVERT(CHAR(8), @AvialableDateFrom, 112)   + ' ' + CONVERT(CHAR(8), @FromTime, 108))
				SET @AvialableDateTill =CONVERT(DATETIME, CONVERT(CHAR(8), @AvialableDateTill, 112)   + ' ' + CONVERT(CHAR(8), @TillTime, 108))
				SET @DynamicEventId =  floor(rand()*100000-1) 


				IF EXISTS (Select 1 from @DepartmentTimming where @FromTime Between  OpeningTime and ClosingTime and OpeningDay = @WeekDay
							AND @TillTime Between OpeningTime and ClosingTime)
				 BEGIN
					--- Dynamicallly Genearate a number and Insert in to the DB
					IF NOT EXISTS (SELECT 1 FROM FacultyTimeSlots WHERE [EventId] = '14401766'+@DynamicEventId)
						SET @DynamicEventId =  floor(rand()*100000-1) 


					IF NOT EXISTS (SELECT 1 FROM FacultyTimeSlots WHERE [AvailableDateFrom] = @AvialableDateFrom AND [AvailableDateTill] = @AvialableDateTill and [UserID] =@pUserId)
					BEGIN
					--- Insert into the table For the TimeSlot of the Selected user			
						Insert into FacultyTimeSlots ([FacultyType],[UserID],[WeekDay],[AvailableDateFrom],[AvailableDateTill],[Description],[FacilityId],[CorporateId]
							,[SlotAvailability],[SlotColor],[SlotTextColor],[EventId],[IsRecurring],[RecType],[RecPattern],[RecEventlength]
							,[RecEventPId],[RecurringDateFrom],[RecurringDateTill],[CreatedBy],[CreatedDate])
						Select @FacultyType,@pUserId,@WeekNumberToAdd,@AvialableDateFrom,@AvialableDateTill,'Lunch Time',@pFID,@pCID,'3','Orange',NULL,'14401766'+@DynamicEventId,
						0,NULL,NULL,0,0,NULL,NULL,1,@CurrentDate
					END
				END
				SET @CurrentDays = @CurrentDays +1;
					
			END
		END
		ELSE
		BEGIN
			-- Declare Cursor for the data to be Added in Timeslots for the Lunch time of Selected Faculty for one month BY Default
			-- Here Insert Data With out any Check (Least expected case.)
			While @CurrentDays <= @CurrentDaysInMonth
			BEGIN
				SET @AvialableDateFrom = DATEADD(day,@CurrentDays,@FirstDateOfMonth); SET @AvialableDateTill = DATEADD(day,@CurrentDays,@FirstDateOfMonth)
				SET @WeekNumberToAdd =  DATEPART(Wk, @AvialableDateFrom)
				SET @AvialableDateFrom = CONVERT(DATETIME, CONVERT(CHAR(8), @AvialableDateFrom, 112)   + ' ' + CONVERT(CHAR(8), @FromTime, 108))
				SET @AvialableDateTill = CONVERT(DATETIME, CONVERT(CHAR(8), @AvialableDateTill, 112)   + ' ' + CONVERT(CHAR(8), @TillTime, 108))
				SET @DynamicEventId =  floor(rand()*100000-1) 

				--- Dynamicallly Genearate a number and Insert in to the DB
				IF NOT EXISTS (SELECT 1 FROM FacultyTimeSlots WHERE [EventId] = '14401766'+@DynamicEventId)
					SET @DynamicEventId =  floor(rand()*100000-1) 
				
				IF NOT EXISTS (SELECT 1 FROM FacultyTimeSlots WHERE [AvailableDateFrom] = @AvialableDateFrom AND [AvailableDateTill] = @AvialableDateTill and [UserID] =@pUserId)
				BEGIN
				--- Insert into the table For the TimeSlot of the Selected user			
					Insert into FacultyTimeSlots ([FacultyType],[UserID],[WeekDay],[AvailableDateFrom],[AvailableDateTill],[Description],[FacilityId],[CorporateId]
						,[SlotAvailability],[SlotColor],[SlotTextColor],[EventId],[IsRecurring],[RecType],[RecPattern],[RecEventlength]
						,[RecEventPId],[RecurringDateFrom],[RecurringDateTill],[CreatedBy],[CreatedDate])
						Select @FacultyType,@pUserId,@WeekNumberToAdd,@AvialableDateFrom,@AvialableDateTill,'Lunch Time',@pFID,@pCID,'3','Orange',NULL,'14401766'+@DynamicEventId,
						0,NULL,NULL,0,0,NULL,NULL,1,@CurrentDate
				END
				SET @CurrentDays = @CurrentDays +1;
					
			END

		END

	END
END





GO


