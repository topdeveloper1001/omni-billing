IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_AddUpdateFacultyLunchTimming')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_AddUpdateFacultyLunchTimming
GO

/****** Object:  StoredProcedure [dbo].[SPROC_AddUpdateFacultyLunchTimming]    Script Date: 22-03-2018 20:14:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Exec [SprocGetOrderCodesToExport] '','ATC',9,8,'','',10
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_AddUpdateFacultyLunchTimming]  -- SPROC_AddUpdateFacultyLunchTimming 1033,1081,3045,23681,'12:30','14:30'
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

	Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFID))
	---- Check to be added we Need to add the check for the date range Iff any
	---- Need to get the Faculty Working days and Add the Data only for the Open slots
	---- Need to Add the Data only in open slots For the Department as for the Department Timming
	SET DATEFIRST 1;--- Use this to set the Week Start Day as Monday for the Sql Datetime Operation to be performed,
	---- Local variable for the Department timming
	Declare @DepartmentTimming Table (OpeningDay nvarchar(5),OpeningTime nvarchar(10),ClosingTime nvarchar(10),TrunAroundTime nvarchar(10))


	Declare @PhysicianLunchTimeFrom nvarchar(10),@PhysicianLunchTimeTill nvarchar(10),@FacultyType int,@PhysicianSpeciality nvarchar(50);
	--**********************************************************************************************************************************
	Declare @WeekNumberToAdd nvarchar(10),@AvialableDateFrom Datetime,@AvialableDateTill Datetime,@DynamicEventId nvarchar(50),@FacilityName nvarchar(50),@EventParentId nvarchar(50)
	--**********************************************************************************************************************************
	
	Declare @CurrentDate Datetime =@LocalDateTime --@LocalDateTime;
	Declare @CurrentMonth int = Month(@CurrentDate);
	Declare @FirstDateOfMonth datetime = Cast(Cast( YEAR(@CurrentDate) as nvarchar(10)) + ' - '+ cast(@CurrentMonth as nvarchar(10)) +' - ' + '01' as Datetime)
	Declare @CurrentDaysInMonth int =  DAY(EOMONTH(@CurrentDate))  * 12
	Declare @CurrentDays int =  DAY((@CurrentDate));
	--Declare @CurrentWeekNumber int =  datepart(Wk, @CurrentDate);
	Declare @WeekDay int

	Declare @AppointmentType nvarchar(100)='Break Time'


	--- Delete the previously entered data from Today Date
	Delete from Scheduling where ((AssociatedId=@pUserId) AND [AssociatedType] = '3' AND 
	SchedulingType ='3' and CAST(ScheduleFrom as Date) >= CAST(@LocalDateTime as Date))

	---- GET THE Physician Lunch Timming from the Physician Table
	Select @PhysicianLunchTimeFrom= ISNULL(FacultyLunchTimeFrom,'') , @PhysicianLunchTimeTill = ISNULL(FacultyLunchTimeTill,'') , @FacultyType= UserType,
	@PhysicianSpeciality = FacultySpeciality
	from Physician
	Where Id = @pUserId and IsActive = 1

	--- Get The Facility Name By Facility ID
	Select @FacilityName = FacilityName From Facility where FacilityId = @pFID
	Declare @FromTime TIME = CAST(@PhysicianLunchTimeFrom+':00' as TIME) , @TillTime TIME = CAST(@PhysicianLunchTimeTill+':00' as TIME)
	SET @EventParentId =  floor(rand() * 100000 - 1) 
	SET @DynamicEventId =  floor(rand() * 100000 - 1)

	--- Dynamicallly Genearate a number and Insert in to the DB
	IF NOT EXISTS (SELECT 1 FROM [Scheduling] WHERE [EventId] = '141766'+@DynamicEventId)
		SET @DynamicEventId =  floor(rand()*100000-1) 

	--- Dynamicallly Genearate a number and Insert in to the DB
	IF NOT EXISTS (SELECT 1 FROM [Scheduling] WHERE [EventParentId] = '1024'+@EventParentId)
		SET @EventParentId =  floor(rand()*100000-1) 

	--- Check if the Lunch Time is not null or Equal to __:__ for data to add in DB.
	IF( @PhysicianLunchTimeFrom <> '' and @PhysicianLunchTimeFrom <> '__:__')
	BEGIN
		--- Insert the Data For the Department timming 
		Insert Into @DepartmentTimming --(OpeningDay,OpeningTime,ClosingTime,TrunAroundTime)
		Select OpeningDayId,OpeningTime,ClosingTime,TrunAroundTime from DeptTimming Where FacilityStructureID = @pDeptId And IsActive = 1

		IF((Select Count(1) from @DepartmentTimming) > 0)
		BEGIN
			-- Declare Cursor for the data to be Added in Timeslots for the Lunch time of Selected Faculty for one month BY Default
			-- With check of the Department SHould be open in that Day
			While @CurrentDays <= @CurrentDaysInMonth
			BEGIN
				
				--- Dynamicallly Genearate a number and Insert in to the DB
				SET @DynamicEventId =  floor(rand()*100000-1) 
				IF NOT EXISTS (SELECT 1 FROM [Scheduling] WHERE [EventId] = '141766'+@DynamicEventId )
					SET @DynamicEventId =  floor(rand()*100000-1) 

				--- Dynamicallly Genearate a number and Insert in to the DB
				SET @EventParentId =  floor(rand()*100000-1)
				IF NOT EXISTS (SELECT 1 FROM [Scheduling] WHERE [EventParentId] = '1024'+@EventParentId)
					SET @EventParentId =  floor(rand()*100000-1) 
				
				
				SET @AvialableDateFrom = DATEADD(day,@CurrentDays,@FirstDateOfMonth); 
				SET @AvialableDateTill = DATEADD(day,@CurrentDays,@FirstDateOfMonth)
				SET @WeekNumberToAdd =  DATEPART(ISOWk, @AvialableDateFrom)
				SET @WeekDay =  ((DATEPART(dw, @AvialableDateFrom)+5)%7)+2;

				--PRINT @WeekDay

				--Set @WDTest = @WeekDay

				If @WeekDay = 8
					Set @WeekDay = 9091
				Else If @WeekDay = 2
					Set @WeekDay = 9092
				Else If @WeekDay = 3
					Set @WeekDay = 9093
				Else If @WeekDay = 4
					Set @WeekDay = 9094
				Else If @WeekDay = 5
					Set @WeekDay = 9095
				Else If @WeekDay = 6
					Set @WeekDay = 9096
				Else If @WeekDay = 7
					Set @WeekDay = 9097

				SET @AvialableDateFrom = CONVERT(DATETIME, CONVERT(CHAR(8), @AvialableDateFrom, 112)   + ' ' + CONVERT(CHAR(8), @FromTime, 108))
				SET @AvialableDateTill =CONVERT(DATETIME, CONVERT(CHAR(8), @AvialableDateTill, 112)   + ' ' + CONVERT(CHAR(8), @TillTime, 108))

				IF EXISTS (Select 1 from @DepartmentTimming where (@FromTime Between OpeningTime and ClosingTime) and OpeningDay = @WeekDay
							AND (@TillTime Between OpeningTime and ClosingTime))
				 BEGIN
					--- Dynamicallly Generate a number and Insert in to the DB
					IF NOT EXISTS (SELECT 1 FROM [Scheduling] WHERE [ScheduleFrom] = @AvialableDateFrom AND [ScheduleTo] = @AvialableDateTill and [AssociatedId] =@pUserId And [AssociatedType] = '3')
					BEGIN
						--- Insert into the table For the TimeSlot of the Selected user			
						Insert into [Scheduling] ([AssociatedId],[AssociatedType],[SchedulingType],[Status],[StatusType],[ScheduleFrom],[ScheduleTo],[TypeOfVisit],[PhysicianSpeciality]
							,[PhysicianId],[TypeOfProcedure],[FacilityId],[CorporateId],[Comments],[Location],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate]
							,[IsActive],[DeletedBy],[DeletedDate],[ExtValue1],[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5],[IsRecurring],[RecType],[RecPattern]
							,[RecEventlength],[RecEventPId],[RecurringDateFrom],[RecurringDateTill],[EventId],[WeekDay],[EventParentId],[RoomAssigned],[EquipmentAssigned])

						Select @pUserId,'3','3','6',null,@AvialableDateFrom,@AvialableDateTill,@AppointmentType As TypeOfVisit,@PhysicianSpeciality
						,@pUserId As PhysicianId,NULL,@pFID,@pCID,@AppointmentType As Comments,@FacilityName
						,'999',@LocalDateTime,NULL,NULL,1,NULL,NULL,CAST(@pDeptId as Nvarchar(10)),NULL,'True',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'141766'+@DynamicEventId,@WeekNumberToAdd,'1024'+@EventParentId
						,NULL,NULL
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

				--- Dynamicallly Genearate a number and Insert in to the DB
				SET @DynamicEventId =  floor(rand()*100000-1) 
				IF NOT EXISTS (SELECT 1 FROM [Scheduling] WHERE [EventId] = '141766'+@DynamicEventId)
					SET @DynamicEventId =  floor(rand()*100000-1) 

				--- Dynamicallly Genearate a number and Insert in to the DB
				SET @EventParentId =  floor(rand()*100000-1) 
				IF NOT EXISTS (SELECT 1 FROM [Scheduling] WHERE [EventParentId] = '1024'+@EventParentId)
					SET @EventParentId =  floor(rand()*100000-1) 

				SET @AvialableDateFrom = DATEADD(day,@CurrentDays,@FirstDateOfMonth); SET @AvialableDateTill = DATEADD(day,@CurrentDays,@FirstDateOfMonth)
				SET @WeekNumberToAdd =  DATEPART(Wk, @AvialableDateFrom)
				SET @AvialableDateFrom = CONVERT(DATETIME, CONVERT(CHAR(8), @AvialableDateFrom, 112)   + ' ' + CONVERT(CHAR(8), @FromTime, 108))
				SET @AvialableDateTill = CONVERT(DATETIME, CONVERT(CHAR(8), @AvialableDateTill, 112)   + ' ' + CONVERT(CHAR(8), @TillTime, 108))
				--SET @DynamicEventId =  floor(rand()*100000-1) 

				--- Dynamicallly Genearate a number and Insert in to the DB				
				IF NOT EXISTS (SELECT 1 FROM [Scheduling] WHERE [ScheduleFrom] = @AvialableDateFrom AND [ScheduleTo] = @AvialableDateTill and [AssociatedId] =@pUserId And [AssociatedType] = '3')
					BEGIN
					--- Insert into the table For the TimeSlot of the Selected user			
						Insert into [Scheduling] ([AssociatedId],[AssociatedType],[SchedulingType],[Status],[StatusType],[ScheduleFrom],[ScheduleTo],[TypeOfVisit],[PhysicianSpeciality]
							,[PhysicianId],[TypeOfProcedure],[FacilityId],[CorporateId],[Comments],[Location],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate]
							,[IsActive],[DeletedBy],[DeletedDate],[ExtValue1],[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5],[IsRecurring],[RecType],[RecPattern]
							,[RecEventlength],[RecEventPId],[RecurringDateFrom],[RecurringDateTill],[EventId],[WeekDay],[EventParentId],[RoomAssigned],[EquipmentAssigned])
						Select @pUserId,'3','3','6',null,@AvialableDateFrom,@AvialableDateTill,@AppointmentType As TypeOfVisit,@PhysicianSpeciality
						,@pUserId As PhysicianId,NULL,@pFID,@pCID,@AppointmentType As Comments,@FacilityName
						,'999',@LocalDateTime,NULL,NULL,1,NULL,NULL,CAST(@pDeptId as Nvarchar(10)),NULL,'True',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'141766'+@DynamicEventId,@WeekNumberToAdd,'1024'+@EventParentId,
						NULL,NULL
					END
				SET @CurrentDays = @CurrentDays +1;
					
			END

		END

	END
END





GO


