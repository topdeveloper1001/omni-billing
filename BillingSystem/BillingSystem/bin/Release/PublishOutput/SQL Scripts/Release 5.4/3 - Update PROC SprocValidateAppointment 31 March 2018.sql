IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocValidateAppointment')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE SprocValidateAppointment
END
GO


-- =============================================
-- Exec [SprocValidateAppointment] '','ATC',9,8,'','',10
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocValidateAppointment]
(
	@pSchedulingList SchedulerArrayTT READONLY,
	@pFId INT,
	@pUserId INT
)
AS
BEGIN

	SET DATEFIRST 1;

	Declare @PhyDeptId int;

	Declare @TotalAppointmentsCount int = (Select Count(1) From @pSchedulingList)
	,@RightAppCount int=0, @ValidStatusCode int=0

	--- Scheduling Temp table
	Declare @SchedulingTableTemp SchedulerArrayTT;

	--%%%%%%%%%%%%%%%%% Checking If User Role matches in the 'AppointmentTypes' and the 'Physician' Tables %%%%%%%%%%%%%%%%%%%
	
	--E.g. Dental Appointment Type shouldn't booked with Clinician whose speciality is Orthopaedic.
	Select @RightAppCount = Count(PY.Id)
	From Physician PY
	INNER JOIN @pSchedulingList S ON PY.Id = S.PhysicianId
	WHERE PY.UserType IN (Select DISTINCT Cast(ISNULL(A.ExtValue2,'0') as Int) FROM AppointmentTypes A where A.Id=ISNULL(S.TypeOfProcedure,'0')) 
	And PY.FacilityId = @pFId


	IF @RightAppCount < @TotalAppointmentsCount AND @TotalAppointmentsCount > 0
	Begin
		Select *,'' As PhysicianName From @SchedulingTableTemp
		Select 1 As 'Status'	--1 Here means the Appointment Type doesn't belong to its Associated Clinician..Invalid Booking.

		RETURN;
	End

	--%%%%%%%%%%%%%%%%% Checking If User Role matches in the 'AppointmentTypes' and the 'Physician' Tables %%%%%%%%%%%%%%%%%%%


	---- Local variable for the Department timming
	Declare @DepartmentTimming Table (OpeningDay nvarchar(5),OpeningTime nvarchar(10),ClosingTime nvarchar(10),TrunAroundTime nvarchar(10))

	

	INSERT INTO @SchedulingTableTemp
	Exec SPROC_ValidateSchedulerAppointment @pSchedulingList,@pFId,@pUserId

	--- Declare the local  Cursor varibales
	Declare @CUR_SchedulingId int, @CUR_AssociatedId int,@CUR_PhysicianId nvarchar(100),@CUR_ScheduleFrom datetime,@CUR_ScheduleTo datetime,@CUR_FacilityId int
	

	/* ####################################--- Curor Starts Here ---################################### */

	--- Case for the Daily Check Starts
	Declare CR_CheckAppointments CURSOR FOR SELECT SchedulingId,AssociatedId,PhysicianId,ScheduleFrom,ScheduleTo,FacilityId FROM @pSchedulingList

	Open CR_CheckAppointments

	Fetch Next from CR_CheckAppointments INTO @CUR_SchedulingId,@CUR_AssociatedId,@CUR_PhysicianId,@CUR_ScheduleFrom,@CUR_ScheduleTo,@CUR_FacilityId

	--- Cursor loop
	WHILE @@FETCH_STATUS = 0
	BEGIN 
		Declare @STimeF time=Cast(@CUR_ScheduleFrom as time),@STimeT time=Cast(@CUR_ScheduleTo as time),@SDate as date=Cast(@CUR_ScheduleFrom as date)

		-- Get Physician Dept Id to add the check for the Dept opening and closing time/Day
		SELECT TOP 1 @PhyDeptId=ISNULL(FacultyDepartment,'0') FROM Physician where ID = @CUR_PhysicianId
	
		-- Get the Physician department Opening days and Opening time and Closing time for opening days
		Insert Into @DepartmentTimming --(OpeningDay,OpeningTime,ClosingTime,TrunAroundTime)
		Select RIGHT(OpeningDayId,1),OpeningTime,ClosingTime,TrunAroundTime from DeptTimming Where FacilityStructureID = @PhyDeptId And IsActive = 1


		/*
			Check for Already booked timings of the Clinicians in the Scheduling Table. If Found, then get only those items from the list 
			that needs to be changed. Those will be returned back to the User.
		*/
		INSERT INTO @SchedulingTableTemp
		Select S.[SchedulingId],S.[AssociatedId],S.[AssociatedType],S.[SchedulingType],S.[Status],S.[StatusType],S.[ScheduleFrom],S.[ScheduleTo]
		,S.[TypeOfVisit],S.[PhysicianSpeciality],S.[PhysicianId],S.[TypeOfProcedure],S.[FacilityId],S.[CorporateId],S.[Comments],S.[Location],S.[CreatedBy]
		,S.[CreatedDate],S.[ModifiedBy],S.[ModifiedDate],S.[IsActive],S.[DeletedBy],S.[DeletedDate],S.[ExtValue1],ISNULL(S.[ExtValue2],'1')
		,S.[ExtValue3],S.[ExtValue4],ISNULL(S.[ExtValue5],'Not Available'),S.[IsRecurring],S.[RecType],S.[RecPattern],S.[RecEventlength]
		,S.[RecEventPId],S.[RecurringDateFrom],S.[RecurringDateTill],S.[EventId]
		,S.[WeekDay],S.[EventParentId],S.[RoomAssigned],S.[EquipmentAssigned],S.[PhysicianReferredBy] 
		From Scheduling S 
		Where (S.PhysicianId=@CUR_PhysicianId) And S.FacilityId=@CUR_FacilityId And S.IsActive=1
		And S.SchedulingId!=@CUR_SchedulingId
		And Cast(S.ScheduleFrom as date)=@SDate
		And (
		(DATEADD(MINUTE,1,@STimeF) BETWEEN Cast(S.ScheduleFrom as time) And Cast(S.ScheduleTo as time))
		OR (DATEADD(MINUTE,-1,@STimeT) BETWEEN Cast(S.ScheduleFrom as time) And Cast(S.ScheduleTo as time))
		)
		AND S.[Status] not in (4)

		INSERT INTO @SchedulingTableTemp
		Select S.[SchedulingId],S.[AssociatedId],S.[AssociatedType],S.[SchedulingType],S.[Status],S.[StatusType],S.[ScheduleFrom],S.[ScheduleTo]
		,S.[TypeOfVisit],S.[PhysicianSpeciality],S.[PhysicianId],S.[TypeOfProcedure],S.[FacilityId],S.[CorporateId],S.[Comments],S.[Location],S.[CreatedBy]
		,S.[CreatedDate],S.[ModifiedBy],S.[ModifiedDate],S.[IsActive],S.[DeletedBy],S.[DeletedDate],S.[ExtValue1],ISNULL(S.[ExtValue2],'2')
		,S.[ExtValue3],S.[ExtValue4],ISNULL(S.[ExtValue5],'Self-Booked Already'),S.[IsRecurring],S.[RecType],S.[RecPattern],S.[RecEventlength]
		,S.[RecEventPId],S.[RecurringDateFrom],S.[RecurringDateTill],S.[EventId]
		,S.[WeekDay],S.[EventParentId],S.[RoomAssigned],S.[EquipmentAssigned],S.[PhysicianReferredBy] 
		From Scheduling S 
		Where (S.AssociatedId=@CUR_AssociatedId) And S.FacilityId=@CUR_FacilityId And S.IsActive=1
		And S.SchedulingId!=@CUR_SchedulingId
		And Cast(S.ScheduleFrom as date)=@SDate
		And (
		(DATEADD(MINUTE,1,@STimeF) BETWEEN Cast(S.ScheduleFrom as time) And Cast(S.ScheduleTo as time))
		OR (DATEADD(MINUTE,-1,@STimeT) BETWEEN Cast(S.ScheduleFrom as time) And Cast(S.ScheduleTo as time))
		)
		AND S.[Status] not in (4)

		Fetch Next from CR_CheckAppointments INTO @CUR_SchedulingId,@CUR_AssociatedId,@CUR_PhysicianId,@CUR_ScheduleFrom,@CUR_ScheduleTo,@CUR_FacilityId
	END

	--- Cursor END
	---Clean Up the cursor
	CLOSE CR_CheckAppointments;  
	DEALLOCATE CR_CheckAppointments;

	/* ####################################--- Curor ENDs Here ---################################### */

	---Implement the Holiday check here
	Insert into @SchedulingTableTemp
	Select DISTINCT S.[SchedulingId],S.[AssociatedId],S.[AssociatedType],S.[SchedulingType],S.[Status],S.[StatusType],S.[ScheduleFrom],S.[ScheduleTo]
	,S.[TypeOfVisit],S.[PhysicianSpeciality],S.[PhysicianId],S.[TypeOfProcedure],S.[FacilityId],S.[CorporateId],S.[Comments],S.[Location],S.[CreatedBy]
	,S.[CreatedDate],S.[ModifiedBy],S.[ModifiedDate],S.[IsActive],S.[DeletedBy],S.[DeletedDate],S.[ExtValue1],ISNULL(S.[ExtValue2],'2')
	,S.[ExtValue3],S.[ExtValue4],ISNULL(S.[ExtValue5],'Holiday'),S.[IsRecurring],S.[RecType],S.[RecPattern],S.[RecEventlength]
	,S.[RecEventPId],S.[RecurringDateFrom],S.[RecurringDateTill],S.[EventId]
	,S.[WeekDay],S.[EventParentId],S.[RoomAssigned],S.[EquipmentAssigned],S.[PhysicianReferredBy] 
	From Scheduling S 
	INNER JOIN @pSchedulingList S1 ON S.AssociatedId=S1.PhysicianId And S.AssociatedType='2' AND S.IsActive=1
	Where Cast(S1.ScheduleFrom as date)=CAST(S.ScheduleFrom as date) And S.FacilityId=S1.FacilityId

	Select T.*
	,(Select TOP 1 P.PhysicianName From Physician P Where P.Id=T.PhysicianId) As PhysicianName
	FROM @SchedulingTableTemp T

	IF Exists (Select 1 From @SchedulingTableTemp)
		Set @ValidStatusCode=2

	Select @ValidStatusCode
END






GO


