IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocGetTimeSlots') 
  DROP PROCEDURE SprocGetTimeSlots;
GO
/****** Object:  StoredProcedure [dbo].[SprocGetTimeSlots]    Script Date: 10/4/2017 7:11:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetTimeSlots]
(
@dtStart datetime,
@dtEnd datetime,
@iInterval int,
@AppointmentTypeId bigint=0,
@CurrentDate datetime,
@PhysiciansData CliniciansT Readonly
)
AS
BEGIN
	Set @dtEnd = DATEADD(DAY, DATEDIFF(DAY, '19000101', @dtEnd), '19000101 23:59:00.00')
	Declare @TimeSlotsT Table (Id int, TimeFrom nvarchar(10),TimeTo nvarchar(10))
	Declare @OccupiedTimeSlotsT Table (Id int, ClinicianId bigint, TimeFrom nvarchar(20),TimeTo nvarchar(20))
	Declare @ConcernedDate date = Cast(@dtStart as date)
	Declare @TableToReturn table(DateTimeInterval nvarchar(50), ClinicianId int,TimeFrom time,TimeTo time)
	Declare @CurrentTime as time = Cast(@CurrentDate as time)
	Declare @FromDateIsCurrentDate bit = IIF(CAST(@dtStart as Date)=CAST(@CurrentDate as Date),1,0)
	Declare @OccupiedCount int=0
	Declare @CliniciansT Table (ClinicianId bigint)

	INSERT INTO @CliniciansT
	Select DISTINCT ClinicianId From @PhysiciansData


	;WITH aCTE
	AS(
		SELECT  @dtStart AS StartDateTime,DATEADD(MINUTE,@iInterval,@dtStart) AS EndDateTime
		UNION ALL
		SELECT DATEADD(MINUTE,@iInterval,StartDateTime), DATEADD(MINUTE,@iInterval,EndDateTime)
		FROM aCTE
		WHERE DATEADD(MINUTE,@iInterval,EndDateTime) <= @dtEnd
	)

	INSERT INTO @TimeSlotsT
	SELECT ROW_NUMBER() OVER(ORDER BY StartDateTime ASC) AS Id,	
	CONVERT(VARCHAR(5),CONVERT(DATETIME,StartDateTime, 0), 108) TimeSlot1,
	CONVERT(VARCHAR(5),CONVERT(DATETIME,EndDateTime, 0), 108) TimeSlot2
	FROM aCTE
	OPTION (MAXRECURSION 0)

	
	INSERT INTO @OccupiedTimeSlotsT
	Select DISTINCT ROW_NUMBER() OVER(ORDER BY PhysicianId ASC) AS Id,PhysicianId,Cast(Cast(ScheduleFrom as time) as nvarchar(5)) TimeFrom, Cast(Cast(ScheduleTo as time) as nvarchar(5)) TimeTo 
	From Scheduling Where Cast(ScheduleFrom as date)=@ConcernedDate And IsActive=1 And ISNULL(PhysicianId,'') !=''
	And PhysicianId NOT IN (
						Select DISTINCT S.PhysicianId From Scheduling S
						Where Cast(S.ScheduleFrom as date)=@ConcernedDate And TypeOfProcedure=12 And IsActive=1 --Type of Procedure is 12 for Holidays. 
						   )
	And PhysicianId IN (Select * From @CliniciansT)

	Declare @CliniciansCount int = (Select Count(1) From @CliniciansT)
	Declare @CId int


	--Fill all the Default Time slots for every Clinician here.
	While Exists (Select * From @CliniciansT)
	Begin
		Declare @OpeningTime time, @ClosingTime time
		Select TOP 1 @CId=ClinicianId From @CliniciansT
		Select TOP 1 @OpeningTime=OpeningTime,@ClosingTime=ClosingTime From @PhysiciansData Where ClinicianId=@CId

		INSERT INTO @TableToReturn
		Select CONVERT(VARCHAR(5),CONVERT(DATETIME,T1.TimeFrom, 0), 108) + ' - '+
		CONVERT(VARCHAR(5),CONVERT(DATETIME,T1.TimeTo, 0), 108), @CId, T1.TimeFrom,T1.TimeTo from @TimeSlotsT T1
		WHERE CAST(T1.TimeFrom as TIME) >= @OpeningTime AND CAST(T1.TimeTo as TIME) <= @ClosingTime
		AND (CAST(T1.TimeFrom as TIME) >= @CurrentTime OR @FromDateIsCurrentDate=0)
		Order by T1.TimeFrom

		Delete From @CliniciansT Where ClinicianId=@CId
	End



	/* ###############---Below Section will delete those time slots from the Clinician's Availability 
			that are already booked in their schedule ---############### */

	INSERT INTO @CliniciansT
	Select DISTINCT ClinicianId From @OccupiedTimeSlotsT

	While Exists (Select * From @CliniciansT)
	Begin
		Select TOP 1 @CId=ClinicianId From @CliniciansT

		Delete T1 From @TableToReturn T1
		INNER JOIN @OccupiedTimeSlotsT T2 ON T1.ClinicianId=T2.ClinicianId
		Where T1.TimeFrom between DATEADD(MINUTE,1,Cast(T2.TimeFrom as time)) And DATEADD(MINUTE,-1,Cast(T2.TimeTo as time))
		OR T1.TimeTo between DATEADD(MINUTE,1,Cast(T2.TimeFrom as time)) And DATEADD(MINUTE,-1,Cast(T2.TimeTo as time))
		OR (T1.TimeFrom = Cast(T2.TimeFrom as TIME) AND T1.TimeTo = Cast(T2.TimeTo as TIME))

		Delete From @CliniciansT Where ClinicianId=@CId
	End

	/* ###############---The above Section will delete those time slots from the Clinician's Availability 
			that are already booked in their schedule ---############### */



	Select DateTimeInterval,ClinicianId From @TableToReturn
END
