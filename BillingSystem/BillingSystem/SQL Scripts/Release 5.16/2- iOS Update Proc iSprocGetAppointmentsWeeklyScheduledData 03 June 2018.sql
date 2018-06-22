IF OBJECT_ID('iSprocGetAppointmentsWeeklyScheduledData','P') IS NOT NULL
	DROP PROCEDURE iSprocGetAppointmentsWeeklyScheduledData

GO

/****** Object:  StoredProcedure [dbo].[iSprocGetAppointmentsWeeklyScheduledData]    Script Date: 6/3/2018 10:02:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



--Exec iSprocGetAppointmentsWeeklyScheduledData 1270,8
CREATE PROCEDURE [dbo].[iSprocGetAppointmentsWeeklyScheduledData]
(
@pClinicianId bigint=0,
@pFacilityId bigint=0
)
As
Begin
	IF ISNULL(@pFacilityId,0)=0
		SET @pFacilityId=8
	
	SET DATEFIRST 1			--@@DATEFIRST iS 7

	----- $$$$$$$$$ Declaration $$$$$$$$$$$$$$$$$$$$$$-------------------
	Declare @TWeeklyData As Table ([Monday] nvarchar(10),[Tuesday] nvarchar(10),[Wednesday] nvarchar(10)
	,[Thursday] nvarchar(10), [Friday] nvarchar(10), [Saturday] nvarchar(10), [Sunday] nvarchar(10))

	Select TOP 1 @pClinicianId=Id From Physician Where UserId=@pClinicianId

	DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))

	Declare @PatientsSeenInMonth nvarchar(10)=0,@YtdPatientsSeen nvarchar(10)=0
	,@AvgPatientsSeenInMonth nvarchar(10)=0
	Declare @TAvailableTimes As Table(TotalTimeInMinutes numeric(18,2),SDate date)
	Declare @TBookedTimes As Table(TotalTimeInMinutes numeric(18,2),SDate date)

	--This is done to set the start day of the week, as Monday of Next Week if the current dates falls on Saturday or Sunday.
	IF DATEPART(dw,GETDATE()) = 6
		SET @LocalDateTime=DATEADD(day,2,@LocalDateTime)
	else if DATEPART(dw,GETDATE()) = 7
		SET @LocalDateTime=DATEADD(day,1,@LocalDateTime)
	
	
	Declare @SDate datetime = dateadd(day, 1-datepart(dw, @LocalDateTime), CONVERT(date,@LocalDateTime))
	Declare @EDate datetime = dateadd(day, 5-datepart(dw, @LocalDateTime), CONVERT(date,@LocalDateTime)) 

	Declare @DepartmentId bigint= (Select TOP 1 CAST(ISNULL(FacultyDepartment,'0') as bigint) From Physician Where Id=@pClinicianId)
	----- $$$$$$$$$ Declaration $$$$$$$$$$$$$$$$$$$$$$-------------------
	
	--27287
	IF(@DepartmentId=0)
		SET @DepartmentId=2775


	;WITH Calendar ( [Day] ) 
	AS 
	( 
		SELECT @SDate UNION ALL 
		SELECT DATEADD(dd, 1, [Day]) 
		FROM Calendar WHERE [Day] < @EDate
	)


	INSERT INTO @TAvailableTimes (TotalTimeInMinutes,SDate)
	Select DISTINCT TOP 7 (datediff(Minute, OpeningTime, ClosingTime)) AS TotalTimeInMinutes,[Day]
	FROM DeptTimming,Calendar
	Where FacilityStructureID=(Select TOP 1 P.FacultyDepartment From Physician P Where P.Id=@pClinicianId)
	--AND S.ScheduleFrom >= @SDate
	--AND S.ScheduleFrom <  @EDate
	--AND S.SchedulingType NOT IN ('1')

	INSERT INTO @TBookedTimes (TotalTimeInMinutes,SDate)
	Select Sum(S.TotalTimeInMinutes),S.SDate From
	(
		Select datediff(Minute, S.ScheduleFrom, S.ScheduleTo) AS TotalTimeInMinutes
		,CAST(S.ScheduleFrom as date) As SDate
		FROM Scheduling S
		Where S.ScheduleFrom >= @SDate
		AND S.ScheduleFrom <  @EDate
		AND S.SchedulingType NOT IN ('1')
		And S.PhysicianId=@pClinicianId
	) S
	Group By S.SDate
		

	Update A SET A.TotalTimeInMinutes = A.TotalTimeInMinutes-B.TotalTimeInMinutes
	From @TAvailableTimes A 
	INNER JOIN @TBookedTimes B ON A.SDate=B.SDate

	Delete From @TBookedTimes

	INSERT INTO @TBookedTimes (TotalTimeInMinutes,SDate)
	Select Sum(S.TotalTimeInMinutes),S.SDate From
	(
		Select DATEDIFF(Minute, S.ScheduleFrom, S.ScheduleTo) AS TotalTimeInMinutes
		,CAST(S.ScheduleFrom as date) As SDate
		FROM Scheduling S
		Where S.ScheduleFrom >= @SDate
		AND S.ScheduleFrom < @EDate
		AND S.SchedulingType IN ('1')
		And S.PhysicianId=@pClinicianId
	) S
	Group By S.SDate

	Update A SET A.TotalTimeInMinutes = CAST(((ISNULL(B.TotalTimeInMinutes,0)*100)/A.TotalTimeInMinutes) as numeric(18,2))
	From @TAvailableTimes A
	LEFT JOIN @TBookedTimes B ON A.SDate=B.SDate

	INSERT INTO @TWeeklyData ([Monday], [Tuesday], [Wednesday], [Thursday], [Friday], [Saturday], [Sunday])
	SELECT [Monday], [Tuesday], [Wednesday], [Thursday], [Friday], [Saturday], [Sunday]
	FROM 
	(
		SELECT  DATENAME(dw, SDate) AS DayWeek, TotalTimeInMinutes
		FROM @TAvailableTimes
	) AS src
	pivot (
			SUM(TotalTimeInMinutes) FOR DayWeek IN ([Monday], [Tuesday], [Wednesday], [Thursday]
			,[Friday],[Saturday], [Sunday])
	) AS pvt
		
	Select @PatientsSeenInMonth=Count(SchedulingId)
	From Scheduling Where Month(ScheduleFrom)=MONTH(@LocalDateTime)
	And YEAR(ScheduleFrom)=Year(@LocalDateTime) And PhysicianId=@pClinicianId
	And [Status] IN ('14','15')
	And SchedulingType='1'
		

	Select @YtdPatientsSeen=Count(SchedulingId)
	From Scheduling Where 
	YEAR(ScheduleFrom)=Year(@LocalDateTime) And PhysicianId=@pClinicianId
	And [Status] IN ('14','15')
	And SchedulingType='1'
		

	SELECT @AvgPatientsSeenInMonth=ISNULL(AVG(S.Count),'0') FROM 
	(
		Select Count(SchedulingId) As Count
		From Scheduling Where 
		--Month(ScheduleFrom)=MONTH(@LocalDateTime)
		YEAR(ScheduleFrom)=Year(@LocalDateTime) 
		And PhysicianId=@pClinicianId
		And SchedulingType='1'
		And [Status] IN ('14','15')
		Group By MONTH(ScheduleFrom)
	) S
		
	SET DATEFIRST 7			--@@DATEFIRST iS 7


	Select WeeklyBookedPercentage=(Select ([Monday] + ' %') As [Monday], ([Tuesday] + ' %') As [Tuesday]
	,([Wednesday] + ' %') As [Wednesday], ([Thursday] + ' %') As [Thursday]
	, ([Friday] + ' %') [Friday],([Saturday] + ' %') As [Saturday], ([Sunday]+' %') As [Sunday]
	FROM @TWeeklyData
	FOR JSON PATH),
	PatientsSeenInCurrentMonth=@PatientsSeenInMonth
	,YtdPatientsSeen=@YtdPatientsSeen
	,AveragePatientsSeenPerMonth=@AvgPatientsSeenInMonth
	For Json Path, Root('AppointmentWeeklyScheduledData')
End
	
GO


