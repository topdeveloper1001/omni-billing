IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocValidateAppointment') 
  DROP PROCEDURE SprocValidateAppointment;
GO
/****** Object:  StoredProcedure [dbo].[SprocValidateAppointment]    Script Date: 10/8/2017 4:18:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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

	---- Local variable for the Department timming
	Declare @DepartmentTimming Table (OpeningDay nvarchar(5),OpeningTime nvarchar(10),ClosingTime nvarchar(10),TrunAroundTime nvarchar(10))

	--- Scheduling Temp table
	Declare @SchedulingTableTemp SchedulerArrayTT;

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
END






