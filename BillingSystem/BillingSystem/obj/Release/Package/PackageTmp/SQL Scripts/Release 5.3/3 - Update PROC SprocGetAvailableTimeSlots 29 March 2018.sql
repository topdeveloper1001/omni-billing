 IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetAvailableTimeSlots')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE SprocGetAvailableTimeSlots
END

GO

CREATE PROCEDURE [dbo].[SprocGetAvailableTimeSlots] --[dbo].[SprocGetAvailableTimeSlots_KP] '8/31/16 12:00:00 AM','8/31/16 12:00:00 AM',1,8,1054
(
	@pFromDate nvarchar(50),
	@pToDate nvarchar(50),
	@pAppointMentType int,
	@pFacilityId int,
	@pPhysicianId int,
	@pFirst bit=null
)
AS
BEGIN
	DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))
	
	Declare @aUserType int
	Declare @PhysiciansData CliniciansT
	
	SET DATEFIRST 1;

	

	IF NOT Exists (Select 1 from Physician where Id=@pPhysicianId And UserType = (Select Cast(ISNULL(A.ExtValue2,'0') as Int) from AppointmentTypes A where A.Id=@pAppointMentType))
	BEGIN
		Insert Into @PhysiciansData (ClinicianId,ClinicianName,DepartmentId,DepartmentName,OpeningTime,ClosingTime)
		Select ID,'',FacultyDepartment,(Select  TOP 1 FacilityStructureName From FacilityStructure Where FacilityStructureId = PY.FacultyDepartment),'',''   
		From Physician PY 
		WHERE UserType IN (Select Cast(ISNULL(A.ExtValue2,'0') as Int) from AppointmentTypes A where A.Id=@pAppointMentType) 
		And FacilityId = @pFacilityId
	END
	Else
	Begin
		Insert Into @PhysiciansData (ClinicianId,ClinicianName,DepartmentId,DepartmentName,OpeningTime,ClosingTime)
		Select ID,'',FacultyDepartment,(Select  TOP 1 FacilityStructureName From FacilityStructure Where FacilityStructureId = PY.FacultyDepartment),'',''
		From Physician PY Where Id=@pPhysicianId
	End

	Declare @fromDate datetime = CAST(@pFromDate as Datetime)

	--- set the to date toe check for till mid night
	Declare @toDate datetime = DATEADD(DAY, DATEDIFF(DAY, '19000101', @pToDate), '19000101 23:59:00.00')

	IF ISNULL(@pFirst,0)=1
	Begin
		Declare @MaxTime time = (Select max(DT.ClosingTime) From DeptTimming DT 
		Where FacilityStructureID IN (Select DISTINCT DepartmentId From @PhysiciansData) ANd RIGHT(DT.OpeningDayId,1)=DATEPART(dw,@LocalDateTime))

		--Select @MaxTime,CAST(@LocalDateTime as time)
		IF CAST(@LocalDateTime as time) >=DATEADD(MINUTE, -30, @MaxTime)
		Begin
			SET @LocalDateTime=CAST(@LocalDateTime + 1 as date)
		End

		SET @fromDate = CAST(@LocalDateTime as date)
		SET @toDate = CAST(@LocalDateTime as date)
		SET @pFromDate = CAST(@LocalDateTime as date)
		SET @pToDate = CAST(@LocalDateTime as date)
	End


	Update T Set T.OpeningTime =IIF(CAST(@fromDate as Date)=CAST(@LocalDateTime as Date), case when convert(char(5), DT.OpeningTime,108)  >= convert(char(5), @LocalDateTime, 108)
	 then   CONVERT(VARCHAR(5),CONVERT(DATETIME,DT.OpeningTime, 0), 108) else  convert(char(5),  @LocalDateTime, 108) END, convert(char(5),DT.OpeningTime,108))
	 ,
	T.ClosingTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,DT.ClosingTime, 0), 108)
	From @PhysiciansData T
	Inner join DeptTimming DT ON T.DepartmentId = DT.FacilityStructureID
	Where RIGHT(DT.OpeningDayId,1) = DATEPART(dw,@fromDate)

	 

	---- Set Default time interval if there is not any we should pick as 30 mins default timeslot
	Declare @timeDiffernece int = 30

	Declare @TableToReturn table(DateTimeInterval nvarchar(50), ClinicianId int)

	Declare @FinalTableResult table(ID int,TimeSlot nvarchar(50),PhysicianId int, DeptId int , DeptName nvarchar(100), Clinician nvarchar(200))


	IF Exists (Select 1 from FacultyRooster where FacultyId IN (SELECT ClinicianId from @PhysiciansData) 
	AND (CAST(@pFromDate as Date) between CAST(FromDate as Date) and CAST(ToDate as Date)))
	BEGIN
		Update @PhysiciansData Set OpeningTime =IIF(CAST(@fromDate as Date)=CAST(@LocalDateTime as Date), case when convert(char(5), FR.FromDate,108)  >= convert(char(5),  @LocalDateTime, 108)
	 then   CONVERT(VARCHAR(5),CONVERT(DATETIME,FR.FromDate, 0), 108) else  convert(char(5),  @LocalDateTime, 108) END,  convert(char(5), FR.FromDate,108)) ,
		ClosingTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,FR.ToDate, 0), 108)
		From @PhysiciansData T
		Inner join FacultyRooster FR ON T.ClinicianId = FR.FacultyId
		Where CAST(@pFromDate as Date) Between CAST(FR.FromDate as Date) and CAST(FR.ToDate as Date)
	END

	--- Get the time differnece from the appointment types and global codes to get the default time interval for the selected appointment type
	Select TOP 1 @timeDiffernece=ISNULL(DefaultTime,'0') 
	FROM AppointmentTypes 
	Where Id = @pAppointMentType and FacilityId = @pFacilityId

	IF(@timeDiffernece <> 0)
	BEGIN
		Declare @HolidayCheck1 nvarchar(10), @HolidayCheck2 nvarchar(10), @IsHoliday bit=0

		---- Appointment Type Room-Allocation
		Declare @appiontmentRoomExists bit = 0, @FromDateIsCurrentDate bit = IIF(CAST(@fromDate as Date)=CAST(@LocalDateTime as Date),1,0)
		Declare @DepCount int = (Select Count(1) From @PhysiciansData), @CurrentDepId int, @Dept_OpeningTime nvarchar(10), @Dept_ClosingTime nvarchar(10)
		
		IF EXISTS (Select 1 From FacilityStructure S
		 outer apply dbo.Split(',',S.ExternalValue4) S1
		 inner join AppointmentTypes Ap ON S1.IDValue=Ap.Id
		 Where S.GlobalCodeId = 84 And S.FacilityId = @pFacilityId AND ISNULL(S.IsActive,1) = 1 And ISNULL(S.IsDeleted,0)=0
		 And Ap.Id = @pAppointMentType And ISNULL(S.ExternalValue4,'') != '')
		BEGIN
			SET @appiontmentRoomExists = 1
		END

		-- If not holiday then go to the logic
		If(@IsHoliday = 1)
			Select '2' as TimeSlot
		ELSE
		BEGIN
			IF (@appiontmentRoomExists=0)
				Select '1' as TimeSlot
			Else
			Begin
				INSERT INTO @TableToReturn
				Exec SprocGetTimeSlots @fromDate,@toDate,@timeDiffernece,@pAppointMentType,@LocalDateTime,@PhysiciansData
			End
		END

		IF  EXISTS (Select 1 from @TableToReturn)
		BEGIN
			;With Final2
			As
			(

				Select DISTINCT T.DateTimeInterval,TC.ClinicianId,TC.DepartmentId,TC.DepartmentName
				,(Select PhysicianName + '(' + TC.DepartmentName + ')' From Physician Where Id = TC.ClinicianId) As Clinician 
				FROM @TableToReturn T
				INNER JOIN @PhysiciansData TC ON T.ClinicianId=TC.ClinicianId
			)

			Insert into @FinalTableResult
			Select Row_Number() over (order by DateTimeInterval) as row_num,DateTimeInterval,ClinicianId,DepartmentId,DepartmentName,Clinician From Final2
			Order by ClinicianId,row_num
			
			IF EXISTS(Select 1 From @FinalTableResult Where Timeslot <> '23:30 - 00:00')
			BEGIN
				Select DISTINCT *
				,STUFF((SELECT ',' + CAST(OpeningDayId AS VARCHAR(MAX)) From DeptTimming Where FacilityStructureID = DeptId FOR XML PATH('')),1,1,'') DepOpeningDays 
				FROM @FinalTableResult
				WHERE Timeslot <> '23:30 - 00:00'
				ORDER BY Clinician,ID
			END
			ELSE
				Select '0' as TimeSlot 
		
		END
		ELSE
			Select '0' as TimeSlot 
	END

	IF ISNULL(@pFirst,0)=1
		Select @LocalDateTime
END
GO


