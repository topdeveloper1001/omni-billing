IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocSaveClinicianRoster')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocSaveClinicianRoster
GO

/****** Object:  StoredProcedure [dbo].[SprocSaveClinicianRoster]    Script Date: 22-03-2018 17:10:31 ******/
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
CREATE PROCEDURE [dbo].[SprocSaveClinicianRoster]
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
@ExtValue1 nvarchar(50)=NULL,
@ExtValue2 nvarchar(50)=''
)
AS
BEGIN
	Declare @NewId nvarchar(500)='', @TFAsTime time=CAST(@TimeFrom as time), @TTAsTime time=CAST(@TimeTo as time)
	,@ExistingCount int=0
	
	Declare @TempScheduled Table (SchedulingId int, PhysicianId nvarchar(10), ScheduleFrom datetime, ScheduleTo datetime
	,AppointmentType nvarchar(200),PatientName nvarchar(200),PhysicianName nvarchar(200),FacilityName nvarchar(200))
	
	DECLARE @Reason nvarchar(100)

	Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@FId))
	
	Declare @TempNewEntries dbo.SchedulerArrayTT

	Declare @DynamicEventId nvarchar(50)='0'
	,@EventParentId nvarchar(50)=(Cast(ROUND(((99 - 10 -1) * RAND() + 10), 0) AS nvarchar) + CAST(IDENT_CURRENT('Scheduling') + 1  as nvarchar))
	,@FacilityName nvarchar(200)

	-------------------------------------------------------------------------------------------------------------------
	-------------------------------------------------------------------------------------------------------------------

	/* In case of Full Day, Times should be set from 00:00 to 23:59*/
	IF ISNULL(@ExtValue1,'0') !='0'
	Begin
		SET @TimeFrom = '00:00'
		SET @TimeTo = '23:59'
		SET @ExtValue1 = '1'		--This is the default Value as full Day
	End

	Select TOP 1 @Reason = GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = '80441' And GlobalCodeValue = @ReasonId And IsActive = 1

	
	/* In case User doesn't enter the TILL DATE value, then it should be treated as current date for the comparison 
	in the scheduling table
	*/
	IF ISNULL(@DateTo,'')=''
		Set @DateTo = DATEADD(Year,2,@LocalDateTime)


	--/* 
	--Get the Days from Scheduled Date in the Scheduling table
	--Those days data will be compared to the actual days that are chosen by the current user.
	--*/
	--IF ISNULL(@DaysOfWeek,'') != '' AND @DaysOfWeek !='ALL'
	--Begin
	--	INSERT INTO @MainSchedulingTable
	--	Select SchedulingId,DATENAME(DW,ScheduleFrom) From Scheduling Where PhysicianId=@ClinicianId And (ISNULL(@Id,0)=0 OR [StatusType] = @Id)
	--END

	---################## Check if there is any Already-Scheduled within the ranges of dates and times entered, against the current Clinician ##########
		;WITH T (DateFrom,ClinicianId)
		AS
		( 
		SELECT @DateFrom, @ClinicianId As ClinicianId
		UNION ALL
		SELECT DateAdd(day,1,T.DateFrom), @ClinicianId As ClinicianId FROM T WHERE T.DateFrom < @DateTo
		)
		
		INSERT INTO @TempNewEntries (SchedulingId,ScheduleFrom,ScheduleTo,PhysicianId,EventId,EventParentId,FacilityId,CorporateId,TypeOfVisit,AssociatedType
		,SchedulingType,[Status],PhysicianSpeciality,[StatusType],CreatedBy,CreatedDate)
		Select 0,DATEADD(day, DATEDIFF(day, 0, T.DateFrom), @TimeFrom),DATEADD(day, DATEDIFF(day, 0, T.DateFrom), @TimeTo),@ClinicianId
		,Cast(ROW_NUMBER() OVER( ORDER BY @DateFrom) + (Cast(@EventParentId as int) + 1) as nvarchar) As EventId
		,@EventParentId,@FId,@CId,@Reason As TypeOfVisit,3 As AssociatedType,3 As SchedulingType,'6' As [Status]
		,(Select top 1 FacultySpeciality From Physician P Where P.Id=@ClinicianId) As ClinicianSpeciality
		,0 As ClinicianRosterId,@UserId,@LocalDateTime
		From T
		Where
		ISNULL(@DaysOfWeek,'ALL')='ALL'
		OR
		DATENAME(DW,T.DateFrom) IN (Select IDValue From dbo.Split(',',@DaysOfWeek))
		OPTION (MAXRECURSION 0)

		INSERT INTO @TempScheduled
		Select S.SchedulingId, S.PhysicianId, S.ScheduleFrom,S.ScheduleTo,S.TypeOfVisit As AppointmentType
		,
		(Case 
			WHEN ISNULL(S.AssociatedId,'0')=0 THEN '' 
			ELSE (Select TOP 1 (P.PersonFirstName + ' ' + P.PersonLastName) From PatientInfo P Where P.PatientID=S.AssociatedId) 
		 END) As PatientName
		,(Select TOP 1 P.PhysicianName From Physician P Where P.Id=S.PhysicianId) As ClinicianName
		,S.[Location] As FacilityName
		From Scheduling S
		INNER JOIN @TempNewEntries T ON S.PhysicianId=T.PhysicianId
		Where 
		(
		CAST(T.ScheduleFrom as date) Between Cast(S.ScheduleFrom as date) And Cast(S.ScheduleTo as date)
		OR
		CAST(T.ScheduleTo as date) Between Cast(S.ScheduleFrom as date) And Cast(S.ScheduleTo as date)
		)
		And T.PhysicianId=@ClinicianId
		And
		(
		(Cast(DATEADD(minute,1,S.ScheduleFrom) as time) Between Cast(@TimeFrom as time) And Cast(@TimeTo as time))
		OR
		(ISNULL(@TimeTo,'') != '' AND Cast(DATEADD(minute,-1,S.ScheduleTo) as time) Between Cast(@TimeFrom as time) And Cast(@TimeTo as time))
		)
		AND (@DaysOfWeek= 'ALL' OR DATENAME(DW,S.ScheduleFrom) IN (Select IDValue From dbo.Split(',',@DaysOfWeek)))
		And (ISNULL(@Id,0)=0 OR ISNULL(S.[StatusType],'0') != @Id)



	---################## Check if there is any Already-Scheduled within the ranges of dates and times entered, against the current Clinician ##########
	
	---################## Save / Update Clinician Roster Section ##########
	IF Exists (Select 1 From @TempScheduled)
	Begin
		--Error Message 1
		Select TOP 1 @NewId= ('System found already Scheduled Appointments against the Clinician ' 
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
			   ,@FId,@CId,@DaysOfWeek,1,@UserId,@LocalDateTime,0,NULL,ISNULL(@ExtValue1,'0'),@ExtValue2)
			   Set @NewId = Cast(SCOPE_IDENTITY() as nvarchar)
			End
			ELSE
			Begin
				If @Id > 0
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

			Select TOP 1 @FacilityName = FacilityName From Facility Where FacilityId=@FId

			Update @TempNewEntries Set [StatusType]=ISNULL(@NewId,'0')

			Exec SprocScheduleClinicianOffTimings @pTempNewEntries=@TempNewEntries,@pUserId=@UserId,@pLocalDateTime=@LocalDateTime,@pClinicianRosterId=@NewId,@pFacilityName=@FacilityName
		End
	End
	---################## Save / Update Clinician Roster Section ##########

	Select @NewId As QueryExecutionStatus

	--Get the updated list after save / updates done
	--Exec SprocGetClinicianRosterList @CId,@FId,@UserId,0
END
GO


