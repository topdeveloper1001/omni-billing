IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'iSprocGetAvailableTimeSlots')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE iSprocGetAvailableTimeSlots

GO

/****** Object:  StoredProcedure [dbo].[iSprocGetAvailableTimeSlots]    Script Date: 20-03-2018 16:10:03 ******/
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
CREATE PROCEDURE [dbo].[iSprocGetAvailableTimeSlots] --[dbo].[SprocGetAvailableTimeSlots_KP] '8/31/16 12:00:00 AM','8/31/16 12:00:00 AM',1,8,1054
(
	@pFromDate nvarchar(50)='2017-12-17',
	@pToDate nvarchar(50)='2017-12-17',
	@pAppointMentType int=1,
	@pPhysicianId int=1005,
	@pSpecialtyId bigint=0,
	@pFirst bit=null,
	@pTimeFrom nvarchar(10)=null,
	@pTimeTill nvarchar(10)=null,
	@pMaxTimeSlotsCount int=5
)
AS
BEGIN
	--------------Declarations---------------------------------------------------------------
	Declare @pFacilityId int=0
	DECLARE @LocalDateTime datetime, @TimeZone nvarchar(50)
	Declare @aUserType int
	Declare @PhysiciansData CliniciansT
	Declare @DayOfWeek nvarchar(20)
	--------------Declarations---------------------------------------------------------------

	
	--Get Facility ID of the Clinician
	IF @pPhysicianId > 0
		Select TOP 1 @pFacilityId = Facilityid From Physician Where Id=@pPhysicianId
	Else 
		Select TOP 1 @pFacilityId = Facilityid From Physician Where FacultySpeciality=@pSpecialtyId


	SET @LocalDateTime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))
	
	SET DATEFIRST 1;

	IF NOT Exists (Select 1 from Physician where Id=@pPhysicianId And UserType = (Select Cast(ISNULL(A.ExtValue2,'0') as Int) from AppointmentTypes A where A.Id=@pAppointMentType))
	BEGIN
		Insert Into @PhysiciansData (ClinicianId,ClinicianName,DepartmentId,DepartmentName,OpeningTime,ClosingTime)
		Select ID,PhysicianName,FacultyDepartment,(Select  TOP 1 FacilityStructureName From FacilityStructure Where FacilityStructureId = PY.FacultyDepartment),'',''   
		From Physician PY 
		WHERE UserType IN (Select Cast(ISNULL(A.ExtValue2,'0') as Int) from AppointmentTypes A where A.Id=@pAppointMentType) 
		And (@pSpecialtyId=0 OR PY.FacultySpeciality=@pSpecialtyId)
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
		SET @pFromDate = CAST(@LocalDateTime as date)
		SET @pToDate = CAST(@LocalDateTime as date)

		--Select @fromDate,@toDate,@pFromDate,@pToDate
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
	AND (CAST(@pFromDate as Date) between CAST(FromDate as Date) and CAST(ToDate as Date)))
	BEGIN
		Update @PhysiciansData Set OpeningTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,FR.FromDate, 0), 108),
		ClosingTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,FR.ToDate, 0), 108)
		From @PhysiciansData T
		Inner join FacultyRooster FR ON T.ClinicianId = FR.FacultyId
		Where CAST(@pFromDate as Date) Between CAST(FR.FromDate as Date) and CAST(FR.ToDate as Date)
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
				Select DISTINCT FF.*
				,STUFF((SELECT ',' + CAST(D.OpeningDayId AS VARCHAR(MAX)) From DeptTimming D Where D.FacilityStructureID = FF.DeptId FOR XML PATH('')),1,1,'') DepOpeningDays,
				(Case WHEN ISNULL(@pFirst,0)=1 THEN @LocalDateTime ELSE CAST(@pFromDate as datetime) END) As AppointmentDate
				,(Select TOP 1 F.FacilityName From Facility F Where F.FacilityId= (Select TOP 1 P.FacilityId From Physician P Where P.Id=FF.PhysicianId)) [Location]
				FROM @FinalTableResult FF
				WHERE Timeslot <> '23:30 - 00:00'
				ORDER BY Clinician,ID
				For Json Path, Root('TimeSlots')
			END
			ELSE
				Select '0' as TimeSlot 
		
		END
		ELSE
			Select '0' as TimeSlot 
	END
END
GO


