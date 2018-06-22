IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetAvailableTimeSlotsNew')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetAvailableTimeSlotsNew
GO

/****** Object:  StoredProcedure [dbo].[SprocGetAvailableTimeSlotsNew]    Script Date: 20-03-2018 17:57:39 ******/
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
CREATE PROCEDURE [dbo].[SprocGetAvailableTimeSlotsNew] --[dbo].[SprocGetAvailableTimeSlots_KP] '8/31/16 12:00:00 AM','8/31/16 12:00:00 AM',1,8,1054
(
	@pFromDate nvarchar(50),
	@pToDate nvarchar(50),
	@pAppointMentType int,
	@pFacilityId int,
	@pPhysicianId int
)
AS
BEGIN
	DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))
	
	Declare @aUserType int
	Declare @PhysiciansData CliniciansT
	
	
	SET DATEFIRST 1;

	Declare @fromDate datetime = CAST(@pFromDate as Datetime)

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

	Update T Set T.OpeningTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,DT.OpeningTime, 0), 108),
	T.ClosingTime = CONVERT(VARCHAR(5),CONVERT(DATETIME,DT.ClosingTime, 0), 108)
	From @PhysiciansData T
	Inner join DeptTimming DT ON T.DepartmentId = DT.FacilityStructureId
	Where RIGHT(DT.OpeningDayId,1) = DATEPART(dw,@fromDate)

	--Insert Into @PhysiciansData (ClinicianId,ClinicianName,DepartmentId,DepartmentName,OpeningTime,ClosingTime)
	--Select PY.Id,'' As ClinicianName,PY.FacultyDepartment As DepartmentId,
	--(Select TOP 1 FacilityStructureName From FacilityStructure Where FacilityStructureId = PY.FacultyDepartment),
	--CONVERT(VARCHAR(5),CONVERT(DATETIME,DT.OpeningTime, 0), 108),CONVERT(VARCHAR(5),CONVERT(DATETIME,DT.ClosingTime, 0), 108)
	--From Physician PY
	--INNER JOIN DeptTimming DT ON PY.FacultyDepartment = DT.FacilityStructureId 
	--WHERE UserType IN (Select Cast(ISNULL(A.ExtValue2,'0') as Int) FROM AppointmentTypes A where A.Id=@pAppointMentType)
	--And FacilityId = @pFacilityId
	--And RIGHT(DT.OpeningDayId,1) = DATEPART(dw,@fromDate)
	--AND (@pPhysicianId = 0 OR PY.Id=@pPhysicianId)

	--- set the to date toe check for till mid night
	Declare @toDate datetime = DATEADD(DAY, DATEDIFF(DAY, '19000101', @pToDate), '19000101 23:59:00.00') 

	---- Set Default time interval if there is not any we should pick as 30 mins default timeslot
	Declare @timeDiffernece int = 30

	Declare @TableToReturn table(DateTimeInterval nvarchar(50), ClinicianId int)

	Declare @FinalTableResult table(ID int,TimeSlot nvarchar(50),PhysicianId int, DeptId int , DeptName nvarchar(100), Clinician nvarchar(200))


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

				--Select * FROM @TableToReturn
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
				--Order by row_num
			)

			Insert into @FinalTableResult
			Select Row_Number() over (order by DateTimeInterval) as row_num,DateTimeInterval,ClinicianId,DepartmentId,DepartmentName,Clinician From Final2
			Order by ClinicianId,row_num
			
			IF EXISTS(Select 1 From @FinalTableResult Where Timeslot <> '23:30 - 00:00')
			BEGIN
				Select DISTINCT *
				,STUFF((SELECT ',' + CAST(OpeningDayId AS VARCHAR(MAX)) From DeptTimming Where FacilityStructureID = DeptId FOR XML PATH('')),1,1,'') DepOpeningDays 
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


