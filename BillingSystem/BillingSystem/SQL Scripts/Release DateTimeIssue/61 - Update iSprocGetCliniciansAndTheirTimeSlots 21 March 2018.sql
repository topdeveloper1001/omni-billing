IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'iSprocGetCliniciansAndTheirTimeSlots')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE iSprocGetCliniciansAndTheirTimeSlots
GO
/****** Object:  StoredProcedure [dbo].[iSprocGetCliniciansAndTheirTimeSlots]    Script Date: 21-03-2018 13:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[iSprocGetCliniciansAndTheirTimeSlots]
(
	@pAppointmentDate nvarchar(50)='2017-12-21',
	@pAppointMentType int=1,
	@pPhysicianId int=null,
	@pSpecialtyId bigint=null,
	@pFirst bit=null,
	@pCityId int=null,
	@pTimeFrom nvarchar(10)=null,
	@pTimeTill nvarchar(10)=null,
	@pMaxTimeSlotsCount int=null,
	@pFacilityId bigint=null,
	@pStateId int=null
)
AS
BEGIN
	--------------Declarations---------------------------------------------------------------
	DECLARE @LocalDateTime datetime
	Declare @aUserType int
	Declare @PhysiciansData CliniciansT
	Declare @DayOfWeek nvarchar(20)
	--------------Declarations---------------------------------------------------------------

	--Arrangement--------------------------------------------------
	SET @pPhysicianId = ISNULL(@pPhysicianId,0)
	SET @pSpecialtyId = ISNULL(@pSpecialtyId,0)
	SET @pMaxTimeSlotsCount = ISNULL(@pMaxTimeSlotsCount,100)
	SET @pCityId = ISNULL(@pCityId,0)
	SET @pStateId = ISNULL(@pStateId,0)
	SET @pFacilityId = ISNULL(@pFacilityId,0)
	SET @pTimeFrom = ISNULL(@pTimeFrom,'')
	SET @pTimeTill = ISNULL(@pTimeTill,'')
	--Arrangement--------------------------------------------------
	

	--Get Facility ID of the Clinician
	IF @pFacilityId=0
	Begin
		IF @pPhysicianId > 0
			Select TOP 1 @pFacilityId = Facilityid From Physician Where Id=@pPhysicianId
		Else 
			Select TOP 1 @pFacilityId = Facilityid From AppointmentTypes Where Id=@pAppointMentType
	End


	SET @LocalDateTime= (Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))
	
	
	SET DATEFIRST 1;

	IF NOT Exists (Select 1 from Physician where Id=@pPhysicianId And UserType = (Select Cast(ISNULL(A.ExtValue2,'0') as Int) from AppointmentTypes A where A.Id=@pAppointMentType))
	BEGIN
		Insert Into @PhysiciansData (ClinicianId,ClinicianName,DepartmentId,DepartmentName,OpeningTime,ClosingTime)
		Select ID,PhysicianName,FacultyDepartment,(Select  TOP 1 FacilityStructureName From FacilityStructure Where FacilityStructureId = PY.FacultyDepartment),'',''   
		From Physician PY 
		WHERE UserType IN (Select Cast(ISNULL(A.ExtValue2,'0') as Int) from AppointmentTypes A where A.Id=@pAppointMentType)
		And (@pSpecialtyId=0 OR PY.FacultySpeciality=@pSpecialtyId)
		AND (@pStateId=0 OR PY.FacilityId IN (Select F.FacilityId From Facility F Where F.FacilityState=@pStateId))
		AND (@pCityId=0 OR PY.FacilityId IN (Select F.FacilityId From Facility F Where F.FacilityCity=@pCityId))
		AND (@pFacilityId=0 OR PY.FacilityId=@pFacilityId)
	END
	Else
	Begin
		Insert Into @PhysiciansData (ClinicianId,ClinicianName,DepartmentId,DepartmentName,OpeningTime,ClosingTime)
		Select ID,'',FacultyDepartment,(Select  TOP 1 FacilityStructureName From FacilityStructure Where FacilityStructureId = PY.FacultyDepartment),'',''
		From Physician PY Where Id=@pPhysicianId
		AND (@pStateId=0 OR PY.FacilityId IN (Select F.FacilityId From Facility F Where F.FacilityState=@pStateId))
		AND (@pCityId=0 OR PY.FacilityId IN (Select F.FacilityId From Facility F Where F.FacilityCity=@pCityId))
		AND (@pFacilityId=0 OR PY.FacilityId=@pFacilityId)
	End

	Declare @fromDate datetime = CAST(@pAppointmentDate as Datetime)

	--- set the to date toe check for till mid night
	Declare @toDate datetime = DATEADD(DAY, DATEDIFF(DAY, '19000101', @pAppointmentDate)
										,'19000101 23:59:00.00')

	IF ISNULL(@pFirst,0)=1
	Begin		
		Declare @MaxTime time = (Select max(DT.ClosingTime) From DeptTimming DT 
		Where FacilityStructureID IN (Select DISTINCT DepartmentId From @PhysiciansData) ANd RIGHT(DT.OpeningDayId,1)=DATEPART(dw,@LocalDateTime))

		--Select @MaxTime	
		IF CAST(@LocalDateTime as time)>=DATEADD(MINUTE, -30, @MaxTime) OR @MaxTime IS NULL
		Begin
			SET @DayOfWeek=DATENAME(DW, @LocalDateTime)			
			IF @DayOfWeek='Friday'
			Begin
				SET @LocalDateTime=CAST(@LocalDateTime + 2 as date)
			End
			Else
				SET @LocalDateTime=CAST(@LocalDateTime + 1 as date)
		End	

		SET @fromDate = CAST(@LocalDateTime as date)
		SET @toDate = CAST(@LocalDateTime as date)
		SET @pAppointmentDate = CAST(@LocalDateTime as date)

		--Select @fromDate,@toDate,@pAppointmentDate,@pToDate
	End


	Update T Set T.OpeningTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,DT.OpeningTime, 0), 108),
	T.ClosingTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,DT.ClosingTime, 0), 108)
	From @PhysiciansData T
	Inner join DeptTimming DT ON T.DepartmentId = DT.FacilityStructureID
	Where RIGHT(DT.OpeningDayId,1) = DATEPART(dw,@fromDate)

	---- Set Default time interval if there is not any we should pick as 30 mins default timeslot
	Declare @timeDiffernece int = 30

	Declare @TableToReturn table(DateTimeInterval nvarchar(50), ClinicianId int)

	Declare @FinalTableResult table(Id int,TimeSlot nvarchar(50),PhysicianId int, DeptId int , DeptName nvarchar(100), Clinician nvarchar(200),SpecialtyId bigint, Specialty nvarchar(100))


	IF Exists (Select 1 from FacultyRooster where FacultyId IN (SELECT ClinicianId from @PhysiciansData) 
	AND (CAST(@pAppointmentDate as Date) between CAST(FromDate as Date) and CAST(ToDate as Date)))
	BEGIN
		Update @PhysiciansData Set OpeningTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,FR.FromDate, 0), 108),
		ClosingTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,FR.ToDate, 0), 108)
		From @PhysiciansData T
		Inner join FacultyRooster FR ON T.ClinicianId = FR.FacultyId
		Where CAST(@pAppointmentDate as Date) Between CAST(FR.FromDate as Date) and CAST(FR.ToDate as Date)
	END


	--- Get the time differnece from the appointment types and global codes to get the default time interval for the selected appointment type
	Select TOP 1 @timeDiffernece=ISNULL(DefaultTime,'0') 
	FROM AppointmentTypes 
	Where Id = @pAppointMentType

	--Select @timeDiffernece As TimDifference

	IF(@timeDiffernece <> 0)
	BEGIN
		Declare @HolidayCheck1 nvarchar(10), @HolidayCheck2 nvarchar(10), @IsHoliday bit=0

		---- Appointment Type Room-Allocation
		Declare @appiontmentRoomExists bit = 0, @FromDateIsCurrentDate bit = IIF(CAST(@fromDate as Date)=CAST(@LocalDateTime as Date),1,0)
		Declare @DepCount int = (Select Count(1) From @PhysiciansData), @CurrentDepId int, @Dept_OpeningTime nvarchar(10), @Dept_ClosingTime nvarchar(10)
		
		IF EXISTS (Select 1 From FacilityStructure S
		 outer apply dbo.Split(',',S.ExternalValue4) S1
		 inner join AppointmentTypes Ap ON S1.IDValue=Ap.Id
		 Where S.GlobalCodeId = 84 AND ISNULL(S.IsActive,1) = 1 And ISNULL(S.IsDeleted,0)=0
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
				--Select @fromDate,@toDate,@timeDiffernece,@pAppointMentType,@LocalDateTime;
				
				INSERT INTO @TableToReturn
				Exec SprocGetTimeSlots @fromDate,@toDate,@timeDiffernece,@pAppointMentType,@LocalDateTime,@PhysiciansData,@pTimeFrom,@pTimeTill
				,@pMaxTimeSlotsCount
			End
		END

		IF  EXISTS (Select 1 from @TableToReturn)
		BEGIN
			;With Final2
			As
			(
				Select DISTINCT T.DateTimeInterval,TC.ClinicianId,TC.DepartmentId,TC.DepartmentName
				,(Select TOP 1 PhysicianName + '(' + TC.DepartmentName + ')' From Physician Where Id = TC.ClinicianId) As Clinician 
				,(Select TOP 1 P2.FacultySpeciality From Physician P2 Where P2.Id = TC.ClinicianId) As SpecialtyId 
				,(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeCategoryValue='1121' 
				And G.GlobalCodeValue=(Select TOP 1 P3.FacultySpeciality From Physician P3 Where P3.Id = TC.ClinicianId)) As SpecialtyName
				FROM @TableToReturn T
				INNER JOIN @PhysiciansData TC ON T.ClinicianId=TC.ClinicianId
			)

			Insert into @FinalTableResult
			Select Row_Number() over (order by FF.DateTimeInterval) as row_num,FF.DateTimeInterval,FF.ClinicianId,FF.DepartmentId,FF.DepartmentName,FF.Clinician
			,FF.SpecialtyId,FF.SpecialtyName
			From Final2 FF
			Order by FF.ClinicianId,row_num
			
			IF EXISTS(Select 1 From @FinalTableResult Where Timeslot <> '23:30 - 00:00')
			BEGIN
				--Select DISTINCT F2.PhysicianId As Id,F2.Clinician As [Name]
				--From @FinalTableResult F2


				--Select DISTINCT FF.TimeSlot,FF.DeptId,FF.DeptName,FF.SpecialtyId,FF.Specialty,FF.PhysicianId
				--,STUFF((SELECT ',' + CAST(D.OpeningDayId AS VARCHAR(MAX)) From DeptTimming D Where D.FacilityStructureID = FF.DeptId FOR XML PATH('')),1,1,'') DepOpeningDays,
				--(Case WHEN ISNULL(@pFirst,0)=1 THEN @LocalDateTime ELSE CAST(@pAppointmentDate as datetime) END) As AppointmentDate
				--,(Select TOP 1 F.FacilityName From Facility F Where F.FacilityId= (Select TOP 1 P.FacilityId From Physician P Where P.Id=FF.PhysicianId)) [Location]
				--FROM @FinalTableResult FF
				--WHERE Timeslot <> '23:30 - 00:00'
				--FOR JSON PATH, ROOT('TimeSlotsData')

				--Id int,TimeSlot nvarchar(50),PhysicianId int, DeptId int , DeptName nvarchar(100), Clinician nvarchar(200),SpecialtyId bigint, Specialty nvarchar(100)
				Select DISTINCT F2.PhysicianId As Id,F2.Clinician As [Name], TimeSlots=(
				Select DISTINCT FF.TimeSlot,FF.DeptId,FF.DeptName,FF.SpecialtyId,FF.Specialty
				,STUFF((SELECT ',' + CAST(D.OpeningDayId AS VARCHAR(MAX)) From DeptTimming D Where D.FacilityStructureID = FF.DeptId FOR XML PATH('')),1,1,'') DepOpeningDays,
				(Case WHEN ISNULL(@pFirst,0)=1 THEN @LocalDateTime ELSE CAST(@pAppointmentDate as datetime) END) As AppointmentDate
				,(Select TOP 1 F.FacilityName From Facility F Where F.FacilityId= (Select TOP 1 P.FacilityId From Physician P Where P.Id=FF.PhysicianId)) [Location]
				,(Select TOP 1 CAST(ISNULL(F.FacilityId,0) As bigint) From Facility F Where F.FacilityId= (Select TOP 1 P.FacilityId From Physician P Where P.Id=FF.PhysicianId)) FacilityId
				FROM @FinalTableResult FF
				WHERE Timeslot <> '23:30 - 00:00'
				AND FF.PhysicianId=F2.PhysicianId
				For Json Path
				)
				From @FinalTableResult F2
				FOR JSON PATH, ROOT('TimeSlotsData')
			END
			ELSE
				Select '0' as TimeSlot 
		
		END
		ELSE
			Select '0' as TimeSlot 
	END
END