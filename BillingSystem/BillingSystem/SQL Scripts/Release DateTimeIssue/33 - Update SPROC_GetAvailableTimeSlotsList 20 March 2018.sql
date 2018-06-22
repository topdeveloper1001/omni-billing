IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetAvailableTimeSlotsList')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetAvailableTimeSlotsList
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetAvailableTimeSlotsList]    Script Date: 20-03-2018 18:03:20 ******/
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
CREATE PROCEDURE [dbo].[SPROC_GetAvailableTimeSlotsList] --[dbo].[SPROC_GetAvailableTimeSlotsList_KP] '8/31/16 12:00:00 AM','8/31/16 12:00:00 AM',1,8,1054
(
	@pFromDate nvarchar(50),
	@pToDate nvarchar(50),
	@pAppointMentType int,
	@pFacilityId int,
	@pPhysicianId int
)
AS
BEGIN
	DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))
	
	Declare @aUserType int
	Declare @TempClinicians Table(PhyId int, DepId int, DepName nvarchar(100), OpeningTime nvarchar(10), ClosingTime nvarchar(10))
	Declare @TempDeps Table(DepId int, OpeningTime nvarchar(10), ClosingTime nvarchar(10),ClinicianId bigint)

	Delete From @TempClinicians
	
	IF NOT Exists (Select 1 from Physician where Id=@pPhysicianId And UserType = (Select Cast(ISNULL(A.ExtValue2,'0') as Int) from AppointmentTypes A where A.Id=@pAppointMentType))
	BEGIN
		Insert Into @TempClinicians
		Select ID,FacultyDepartment,(Select  TOP 1 FacilityStructureName From FacilityStructure Where FacilityStructureId = PY.FacultyDepartment),'',''   
		From Physician PY 
		WHERE UserType IN (Select Cast(ISNULL(A.ExtValue2,'0') as Int) from AppointmentTypes A where A.Id=@pAppointMentType) 
		And FacilityId = @pFacilityId
	END
	Else
	Begin
		Insert Into @TempClinicians
		Select ID,FacultyDepartment,(Select  TOP 1 FacilityStructureName From FacilityStructure Where FacilityStructureId = PY.FacultyDepartment),'',''
		From Physician PY Where Id=@pPhysicianId
	End


SET DATEFIRST 1;

Declare @fromDate datetime; 

SET @fromDate = CAST (@pFromDate as Datetime)

--- set the to date toe check for till mid night
Declare @toDate datetime; 

--set @toDate = CAST (@pToDate as Datetime); Set @toDate = DATEADD(hour,23,@toDate);
--Set @toDate = DATEADD(minute,59,@toDate);


Set @toDate = DATEADD(DAY, DATEDIFF(DAY, '19000101', @pToDate), '19000101 23:59:00.00') 

---- Set Default time interval if there is not any we should pick as 30 mins default timeslot
Declare @timeDiffernece int = 30

---- Declare the time slot table
Declare @TimeSlotTable table([ID] [int] IDENTITY(1,1) NOT NULL,DateTimeInterval datetime)

--- declare the occupied time slot table
Declare @OccupiedTimeSlots table(DateTimeInterval datetime)

--- declare the occupied time slots table
Declare @OccupiedTimeSlotsTable table(FromDate datetime, ToDate datetime)

Declare @TableToReturn table(DateTimeInterval nvarchar(50), DepId int)
Declare @TempTimeSlots table(AllTimeSlot1 nvarchar(50),AllTimeSlot2 nvarchar(50))
Declare @OccupiedTimeSlotsTable1 table(OccupiedTimeSlot1 nvarchar(50),OccupiedTimeSlot2 nvarchar(50),ClinicianId bigint)
Declare @FinalTableResult table(ID int,TimeSlot nvarchar(50),PhysicianId int, DeptId int , DeptName nvarchar(100), Clinician nvarchar(200))

Update @TempClinicians Set OpeningTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,DT.OpeningTime, 0), 108),
ClosingTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,DT.ClosingTime, 0), 108)
From @TempClinicians T
Inner join DeptTimming DT ON T.DepId = DT.FacilityStructureId
Where RIGHT(DT.OpeningDayId,1) = DATEPART(dw,@fromDate)



IF Exists (Select 1 from FacultyRooster where FacultyId in (SELECT PhyId from @TempClinicians) 
AND (CAST(@pFromDate as Date) between CAST(FromDate as Date) and CAST(ToDate as Date)))
BEGIN
	Update @TempClinicians Set OpeningTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,FR.FromDate, 0), 108),
	ClosingTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,FR.ToDate, 0), 108)
	From @TempClinicians T
	Inner join FacultyRooster FR ON T.PhyId = FR.FacultyId
	Where CAST(@pFromDate as Date) Between CAST(FR.FromDate as Date) and CAST(FR.ToDate as Date)
END

Select * From @TempClinicians

INSERT INTO @TempDeps
Select DISTINCT DepId,OpeningTime,ClosingTime,PhyId From @TempClinicians

--- Get the time differnece from the appointment types and global codes to get the default time interval for the selected appointment type
Select TOP 1 @timeDiffernece=ISNULL(DefaultTime,'0') 
FROM AppointmentTypes 
Where Id = @pAppointMentType and FacilityId = @pFacilityId

IF(@timeDiffernece <> 0)
BEGIN
;with cteNumbers as (
    select
        row_number() over (order by o1.object_id, o2.object_id) - 1 as rn
    from
        sys.objects o1
            cross join sys.objects o2
)

Insert into @TimeSlotTable -- Isnert the list in the Table
select dateadd(minute,(rn * @timeDiffernece) , @fromDate) as DateTimeInterval
from cteNumbers
where dateadd(minute,(rn * @timeDiffernece) -1, @fromDate) between @fromDate and @toDate
order by DateTimeInterval
	
	Insert into @TempTimeSlots
	Select CONVERT(VARCHAR(5),CONVERT(DATETIME,a.DateTimeInterval, 0), 108),
	CONVERT(VARCHAR(5),CONVERT(DATETIME,b.DateTimeInterval, 0), 108) from @TimeSlotTable a
	cross join @TimeSlotTable b
	where (a.ID % 2) = 1
	and (b.ID % 2) = 0
	and (b.ID - a.ID) = 1

	Insert into @TempTimeSlots
	Select CONVERT(VARCHAR(5),CONVERT(DATETIME,a.DateTimeInterval, 0), 108),
	CONVERT(VARCHAR(5),CONVERT(DATETIME,b.DateTimeInterval, 0), 108)  from @TimeSlotTable a 
	cross join @TimeSlotTable b
	where (a.ID % 2) = 0
	and (b.ID % 2) = 1
	and (b.ID - a.ID) = 1


	--Select * From @TempTimeSlots

	--- Query to get the Time intervals using the time slot differnece
	--- Insert the data from the Scheduling table to the temp table (Occupied Time slots)
	INSERT into @OccupiedTimeSlotsTable1
	Select DISTINCT CONVERT(VARCHAR(5),CONVERT(DATETIME,ScheduleFrom, 0), 108),
	CONVERT(VARCHAR(5),CONVERT(DATETIME,ScheduleTo, 0), 108),PhysicianId FROM Scheduling
	WHERE
	PhysicianId IN (SELECT DISTINCT PhyId from @TempClinicians)
	--OR (AssociatedId  in (select PhyId from @TempClinicians)
	--AND (AssociatedType IN ('2','3'))))
	and Cast(ScheduleFrom as Date) = Cast(@fromDate as Date)


	Declare @HolidayCheck1 nvarchar(10), @HolidayCheck2 nvarchar(10), @IsHoliday bit=0


	--- Table to return values (TIme slots that are avialable and are in between the department opening and closing time)
	---- VACATION CHECK
	IF EXISTS (Select 1 from @OccupiedTimeSlotsTable1)
	BEGIN
		 Select Top 1 @HolidayCheck1 = OccupiedTimeSlot1,@HolidayCheck2 = OccupiedTimeSlot2 From @OccupiedTimeSlotsTable1

		 IF (@HolidayCheck1 = '00:00' AND @HolidayCheck2 = '23:59') OR 
		 Exists(Select 1 from Scheduling Where FacilityId = @pFacilityId 
		 and TypeOfProcedure = 12 and ExtValue2 is not null and Cast(ScheduleFrom as date) = Cast(@pFromDate as Date))
		 begin
			Set @IsHoliday=1
		 end
	END

	Delete From @TempTimeSlots


	--DECLARE @dtStart AS DATETIME ='2017-10-03'
 --       ,@dtEnd AS DATETIME = '2017-10-04'
 --       ,@iInterval AS INT = 30;  --30 MIN Interval

--WITH aCTE
--AS(
--    SELECT 
--        @fromDate AS StartDateTime,
--        DATEADD(MINUTE,@timeDiffernece,@fromDate) AS EndDateTime
--    UNION ALL
--    SELECT
--        DATEADD(MINUTE,@timeDiffernece,StartDateTime),
--        DATEADD(MINUTE,@timeDiffernece,EndDateTime)
--    FROM aCTE
--    WHERE
--        DATEADD(MINUTE,@timeDiffernece,EndDateTime) <= @toDate
--)

--INSERT INTO @TempDeps
--SELECT
--CONVERT(VARCHAR(5),CONVERT(DATETIME,StartDateTime, 0), 108)
--,
--CONVERT(VARCHAR(5),CONVERT(DATETIME,EndDateTime, 0), 108)
--    ---- 10:00:00 AM 
--    --CONVERT(VARCHAR(10),StartDateTime,108) 
--    --+ ' ' + RIGHT(CONVERT(VARCHAR(30), StartDateTime, 9), 2) 
--    --+ ' - ' +
--    ---- 10:30:00 AM
--    --CONVERT(VARCHAR(10),EndDateTime,108) 
--    --+ ' ' + RIGHT(CONVERT(VARCHAR(30), EndDateTime, 9), 2) AS Result
--FROM aCTE
--OPTION (MAXRECURSION 0)








	---- Appointment Type Room-Allocation
	Declare @appiontmentRoomExists bit = 0, @FromDateIsCurrentDate bit = IIF(CAST(@fromDate as Date)=CAST(@LocalDateTime as Date),1,0)
	Declare @DepCount int = (Select Count(1) From @TempDeps), @CurrentDepId int, @Dept_OpeningTime nvarchar(10), @Dept_ClosingTime nvarchar(10)
		
	IF EXISTS (Select 1 From FacilityStructure S
	 outer apply dbo.Split(',',S.ExternalValue4) S1
	 inner join AppointmentTypes Ap ON S1.IDValue=Ap.Id
	 Where S.GlobalCodeId = 84 And S.FacilityId = @pFacilityId AND ISNULL(S.IsActive,1) = 1 And ISNULL(S.IsDeleted,0)=0
	 And Ap.Id = @pAppointMentType And ISNULL(S.ExternalValue4,'') != '')
	BEGIN
		SET @appiontmentRoomExists = 1
	END

	Select * From @OccupiedTimeSlotsTable1

	Select @DepCount

-- If not holiday then go to the logic
	If(@IsHoliday = 1)
		Select '2' as TimeSlot
	ELSE
	BEGIN
		IF (@appiontmentRoomExists = 0)
			Select '1' as TimeSlot
		ELSE IF EXISTS (Select 1 from @OccupiedTimeSlotsTable1) --IF (CAST(@fromDate as Date)= CAST(Getdate() as Date))
		BEGIN
			While @DepCount > 0
			Begin
				Select TOP 1 @CurrentDepId = ClinicianId, @Dept_OpeningTime = OpeningTime, @Dept_ClosingTime = ClosingTime From @TempDeps

				Select @CurrentDepId,@Dept_OpeningTime,@Dept_ClosingTime

				Select CONVERT(VARCHAR(5),CONVERT(DATETIME,AllTimeSlot1, 0), 108) + ' - '+
				CONVERT(VARCHAR(5),CONVERT(DATETIME,AllTimeSlot2, 0), 108) from @TempTimeSlots
				where AllTimeSlot1 not in (Select OccupiedTimeSlot1  from @OccupiedTimeSlotsTable1)
				and AllTimeSlot2 not in (Select OccupiedTimeSlot2  from @OccupiedTimeSlotsTable1)
				and (CAST(AllTimeSlot1 as TIME) >= @Dept_OpeningTime and CAST(AllTimeSlot2 as TIME) <= @Dept_ClosingTime)
				AND (CAST(AllTimeSlot1 as TIME) >= CAST(@LocalDateTime as TIME) OR @FromDateIsCurrentDate=0)
				Order by AllTimeSlot1

				Insert into @TableToReturn (DateTimeInterval)
				Select CONVERT(VARCHAR(5),CONVERT(DATETIME,AllTimeSlot1, 0), 108) + ' - '+
				CONVERT(VARCHAR(5),CONVERT(DATETIME,AllTimeSlot2, 0), 108) from @TempTimeSlots
				where AllTimeSlot1 not in (Select OccupiedTimeSlot1 from @OccupiedTimeSlotsTable1)
				and AllTimeSlot2 not in (Select OccupiedTimeSlot2  from @OccupiedTimeSlotsTable1) 
				and (CAST(AllTimeSlot1 as TIME) >= @Dept_OpeningTime and CAST(AllTimeSlot2 as TIME) <= @Dept_ClosingTime)
				AND (CAST(AllTimeSlot1 as TIME) >= CAST(@LocalDateTime as TIME) OR @FromDateIsCurrentDate=0)
				Order by AllTimeSlot1


				Delete From @TempDeps Where ClinicianId = @CurrentDepId
				Set @DepCount = @DepCount-1;
			End
		End
		ELSE
		BEGIN
			Select TOP 1 @CurrentDepId = ClinicianId, @Dept_OpeningTime = OpeningTime, @Dept_ClosingTime = ClosingTime From @TempDeps

			Insert into @TableToReturn (DateTimeInterval)
			Select CONVERT(VARCHAR(5),CONVERT(DATETIME,AllTimeSlot1, 0), 108) + ' - '+
			CONVERT(VARCHAR(5),CONVERT(DATETIME,AllTimeSlot2, 0), 108) from @TempTimeSlots
			where (CAST(AllTimeSlot1 as TIME) >= @Dept_OpeningTime and CAST(AllTimeSlot2 as TIME) <= @Dept_ClosingTime)
			and (CAST(AllTimeSlot1 as TIME) >= CAST(@LocalDateTime as TIME) OR @FromDateIsCurrentDate=0)
			Order by AllTimeSlot1

			Delete From @TempDeps Where DepId = @CurrentDepId
			Set @DepCount = @DepCount-1;
		END
	END

	IF  EXISTS (Select 1 from @TableToReturn)
	BEGIN
		With Final
		As
		(
			Select DISTINCT T.DateTimeInterval,TC.PhyId,TC.DepId,TC.DepName, 
			(Select PhysicianName + ' (' + TC.DepName + ') '		
			From Physician Where Id = TC.PhyId) Clinician from @TableToReturn T,@TempClinicians TC
			--Order by TC.PhyId
		)
		
		Insert into @FinalTableResult
		Select Row_Number() over (order by DateTimeInterval) as row_num,* From Final
		Order by PhyId		

		IF EXISTS(Select 1 From @FinalTableResult Where Timeslot <> '23:30 - 00:00')
		BEGIN
			Select DISTINCT *,STUFF((SELECT ',' + CAST(OpeningDayId AS VARCHAR(MAX)) From DeptTimming Where FacilityStructureID = DeptId FOR XML PATH('')),1,1,'') DepOpeningDays 
			from @FinalTableResult Where Timeslot <> '23:30 - 00:00'
			order by PhysicianId,TimeSlot
		END
		ELSE
		Select '0' as TimeSlot 
		
	END
	ELSE
		Select '0' as TimeSlot 
	END
END





GO


