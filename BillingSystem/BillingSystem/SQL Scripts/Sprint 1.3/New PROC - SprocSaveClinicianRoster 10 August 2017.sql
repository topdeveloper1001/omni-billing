IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocSaveClinicianRoster') 
  DROP PROCEDURE SprocSaveClinicianRoster;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE SprocSaveClinicianRoster
(
@Id bigint,
@ClinicianId bigint,
@ReasonId nvarchar(100),
@Comments nvarchar(200),
@RosterTypeId nvarchar(10),
@DateFrom datetime,
@TimeFrom nvarchar(5),
@DateTo datetime='',
@TimeTo nvarchar(5)='',
@CId bigint,
@FId bigint,
@DaysOfWeek nvarchar(200)='',
@UserId bigint,
@ExtValue1 nvarchar(50)='',
@ExtValue2 nvarchar(50)=''
)
AS
BEGIN
	Declare @NewId nvarchar(500)='', @TFAsTime time=CAST(@TimeFrom as time), @TTAsTime time=CAST(@TimeTo as time)
	,@ExistingCount int=0
	
	--Declare @TEnteredDateRanges Table (DateFrom datetime,ClinicianId bigint)

	
	Declare @TempScheduled Table (SchedulingId int, PhysicianId nvarchar(10), ScheduleFrom datetime, ScheduleTo datetime
	,AppointmentType nvarchar(200),PatientName nvarchar(200),PhysicianName nvarchar(200),FacilityName nvarchar(200))
	
	DECLARE @LocalDateTime datetime, @TimeZone nvarchar(50)
	
	Declare @MainSchedulingTable Table (Id bigint, SchDayOfWeek nvarchar(10))

	-------------------------------------------------------------------------------------------------------------------

	--Get the local time based on Time ZONE of the current facility
	SET @TimeZone=(Select TimeZ from Facility Where Facilityid=@FId)
	SET @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,GETDATE()))
	--Get the local time based on Time ZONE of the current facility

	
	/* In case User doesn't enter the TILL DATE value, then it should be treated as current date for the comparison 
	in the scheduling table
	*/
	IF ISNULL(@DateTo,'')=''
		Set @DateTo = DATEADD(Year,10,@LocalDateTime)


	----Get the individual dates from the 2 i.e. Start Date and End Date
	--;WITH T(DateFrom,ClinicianId)
	--AS
	--( 
	--	SELECT @DateFrom, @ClinicianId As ClinicianId
	--	UNION ALL
	--	SELECT DateAdd(day,1,T.DateFrom), @ClinicianId As ClinicianId FROM T WHERE T.DateFrom < @DateTo
	--)

	--INSERT INTO @TEnteredDateRanges
	--Select * From T
	----Get the individual dates from the 2 i.e. Start Date and End Date




	/* 
	Get the Days from Scheduled Date in the Scheduling table
	Those days data will be compared to the actual days that are chosen by the current user.
	*/
	IF ISNULL(@DaysOfWeek,'') != '' AND @DaysOfWeek !='ALL'
	Begin
		INSERT INTO @MainSchedulingTable
		Select SchedulingId,DATENAME(DW,ScheduleFrom) From Scheduling Where PhysicianId=@ClinicianId
	END

	---################## Check if there is any Already-Scheduled within the ranges of dates and times entered, against the current Clinician ##########
		;WITH T(DateFrom,ClinicianId)
		AS
		( 
		SELECT @DateFrom, @ClinicianId As ClinicianId
		UNION ALL
		SELECT DateAdd(day,1,T.DateFrom), @ClinicianId As ClinicianId FROM T WHERE T.DateFrom < @DateTo
		)

		INSERT INTO @TempScheduled
		Select S.SchedulingId, S.PhysicianId, S.ScheduleFrom,S.ScheduleTo,S.TypeOfVisit As AppointmentType
		,(Select TOP 1 (P.PersonFirstName + ' ' + P.PersonLastName) From PatientInfo P Where P.PatientID=S.AssociatedId) As PatientName
		,(Select TOP 1 P.PhysicianName From Physician P Where P.Id=S.PhysicianId) As ClinicianName
		,S.[Location] As FacilityName
		From Scheduling S
		INNER JOIN T ON S.PhysicianId=T.ClinicianId
		INNER JOIN @MainSchedulingTable S1 ON S.SchedulingId=S1.Id
		Where (T.DateFrom Between Cast(S.ScheduleFrom as date) And Cast(S.ScheduleTo as date))
		And T.ClinicianId=@ClinicianId
		And
		(
		(Cast(DATEADD(minute,1,S.ScheduleFrom) as time) Between Cast(@TimeFrom as time) And Cast(@TimeTo as time))
		OR
		(ISNULL(@TimeTo,'') != '' AND Cast(DATEADD(minute,-1,S.ScheduleTo) as time) Between Cast(@TimeFrom as time) And Cast(@TimeTo as time))
		)
		AND 
		(
			ISNULL(@DaysOfWeek,'ALL')='ALL'
			OR
			S1.SchDayOfWeek IN (Select IDValue From dbo.Split(',',@DaysOfWeek))
		)
	---################## Check if there is any Already-Scheduled within the ranges of dates and times entered, against the current Clinician ##########
	
	/*
	Get all existing appointments of current Clinician that exists between the entered date and time values.
	*/
	--Select * From @TempScheduled


	---################## Save / Update Clinician Roster Section ##########
	IF Exists (Select 1 From @TempScheduled)
	Begin	
		--Error Message 1
		Select TOP 1 @NewId= ('System found some Scheduled Appointments against the Clinician ' 
		+ PhysicianName 
		+ ' within the Date / Time Range entered. Kindly Change the appropriate selections!') 
		From Physician Where Id=@ClinicianId
	End
	Else
	Begin
		
		;With T (DateFrom, ClinicianId,DateTo,Id)
		As
		(
			SELECT C.DateFrom, C.ClinicianId,C.DateTo,C.Id From ClinicianRoster C Where C.ClinicianId =1004
			UNION ALL
			SELECT DateAdd(day,1,T.DateFrom), T.ClinicianId,DateTo,T.Id FROM T 
			WHERE T.DateFrom < T.DateTo
		)


		Select @ExistingCount = Count(T.ClinicianId) From T
		INNER JOIN ClinicianRoster C ON T.ClinicianId=C.ClinicianId AND (@Id=0 OR C.Id=@Id)
		Where T.DateFrom Between @DateFrom And @DateTo
		And
		(
			(Cast(C.TimeFrom as time) between @TFAsTime and @TTAsTime)
		  OR
			(Cast(C.TimeTo as time) between @TFAsTime and @TTAsTime)
		  OR 
			(@TFAsTime between DATEADD(minute,1,Cast(C.TimeFrom as time)) and DATEADD(minute,1,Cast(C.TimeTo as time)))
		  OR 
			(@TTAsTime between DATEADD(minute,1,Cast(C.TimeFrom as time)) and DATEADD(minute,1,Cast(C.TimeTo as time)))
		)
		AND 
		(
			ISNULL(@DaysOfWeek,'ALL')='ALL'
			OR
			Exists (Select 1 From dbo.Split(',',C.RepeatitiveDaysInWeek) Where @DaysOfWeek Like '%' + IDValue + '%')
		)
		And 
		(
		@Id=0
		OR 
		T.Id!=@Id
		)

		IF @ExistingCount > 0
		Begin
			--Error Message 2
			Select TOP 1 @NewId= ('Duplicates found against the Clinician ' 
			+ PhysicianName 
			+ ' within the Date / Time Range entered. Kindly Change the appropriate selections!') 
			From Physician Where Id=@ClinicianId
		End
		Else
		Begin
			If @Id = 0
			Begin
				INSERT INTO ClinicianRoster (ClinicianId,[ReasonId],[Comments],[RosterTypeId],[DateFrom],[TimeFrom],[DateTo],[TimeTo]
				,[FacilityId],[CorporateId],[RepeatitiveDaysInWeek],[IsActive],[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate],[ExtValue1],[ExtValue2])
			   VALUES (@ClinicianId,@ReasonId,@Comments,@RosterTypeId,@DateFrom, @TimeFrom,@DateTo,@TimeTo
			   ,@FId,@CId,@DaysOfWeek,1,@UserId,@LocalDateTime,0,NULL,@ExtValue1,@ExtValue2)
			   Set @NewId = Cast(SCOPE_IDENTITY() as nvarchar)
			End
			ELSE
			Begin
				UPDATE ClinicianRoster
				SET [ReasonId] = @ReasonId
				,[Comments] = @Comments
				,[RosterTypeId] = @RosterTypeId
				,[DateFrom] = @DateFrom
				,[TimeFrom] = @TimeFrom
				,[DateTo] = @DateTo
				,[TimeTo] = @TimeTo
				,[RepeatitiveDaysInWeek] = @DaysOfWeek
				,[ModifiedBy] = @UserId
				,[ModifiedDate] = @LocalDateTime
				WHERE Id=@Id
				Set @NewId = Cast(@Id as nvarchar)
			End
		End
	End
	---################## Save / Update Clinician Roster Section ##########

	Select @NewId As QueryExecutionStatus

	--Get the updated list after save / updates done
	--Exec SprocGetClinicianRosterList @CId,@FId,@UserId,0
END
GO
