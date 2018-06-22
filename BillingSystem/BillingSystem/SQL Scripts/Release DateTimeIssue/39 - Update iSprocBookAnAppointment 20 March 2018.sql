IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'iSprocBookAnAppointment')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE iSprocBookAnAppointment
GO

/****** Object:  StoredProcedure [dbo].[iSprocBookAnAppointment]    Script Date: 20-03-2018 18:21:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Exec [iSprocBookAnAppointment] 22
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[iSprocBookAnAppointment]
(
@pId bigint,
@pPatientId bigint,
@pClinicianId bigint,
@pAppointmentTypeId bigint,
@pSpecialty bigint,
@pFacilityId nvarchar(200),
@pAppointmentDate datetime,
@pTimeFrom nvarchar(10),
@pTimeTill nvarchar(10),
@pCreatedBy bigint,
@pTitle nvarchar(100)=null,
@pClinicianReferredBy bigint=null,
@pAppDetails nvarchar(400)=null,
@pCountryId bigint,
@pStateId bigint,
@pCityId bigint,
@pEventId nvarchar(50),
@pEventParentId nvarchar(50),
@pWeekDay nvarchar(50),
@pToken nvarchar(20)
)
AS
BEGIN
	Begin Try
			---###### Declarations ##################
			
		/*
			--Duplicate: -1
			--No Room: -2
			--No Equipment: -3
			--Other: -4
			--Error: 0
			--Patient ID not provided: -5
		*/
		Declare @LocalDateTime datetime
		Declare @AddedToSchedulingFlag bit=0,@ExecutedStatus int=0
		,@ResponseMessage nvarchar(100)='Success'
		Declare @ScheduleTimeFrom datetime = DATEADD(day, DATEDIFF(day,'19000101',@pAppointmentDate), CAST(@pTimeFrom AS DATETIME2(7)))
		Declare @ScheduleTimeTill datetime = DATEADD(day, DATEDIFF(day,'19000101',@pAppointmentDate), CAST(@pTimeTill AS DATETIME2(7)))
		Declare @TypeOfVisit nvarchar(100)
		Declare @FId int = 0, @CId int=0,@FacilityName nvarchar(100)='',@EquipmentRequired bit=0
		Declare @TRoom Table (RoomId bigint,EqId bigint,DepId bigint,IsAppointed bit,SchId bigint)

		---###### Declarations ##################

		IF ISNULL(@pPatientId,0)=0
		Begin
			SET @ExecutedStatus=-5
			SET @ResponseMessage='Patient ID Not Provided'
		End
		
		
		IF @ExecutedStatus=0
		Begin
			Select TOP 1 @TypeOfVisit=[Name],@EquipmentRequired=Cast(ExtValue1 As bit) From AppointmentTypes Where Id=@pAppointmentTypeId


			Select TOP 1 @FId=P.FacilityId, @CId=P.CorporateId
			,@FacilityName = (Select TOP 1 F.FacilityName From Facility F Where F.FacilityId=P.FacilityId)
			From Physician P Where Id=@pClinicianId

			SET @LocalDateTime=(Select dbo.GetCurrentDatetimeByEntity(@FId))
			SET @pAppointmentDate=CAST(@pAppointmentDate as date)

			/*
			Assigning the Room ID and Equipment ID to the Appointment Details.
			*/
			INSERT INTO @TRoom (RoomId,EqId,DepId,IsAppointed)
			Exec SPROC_CheckRoomsAndEquipmentsInSchedulingUpdated @FId,@pAppointmentTypeId
			,@pAppointmentDate,@pTimeFrom,@pTimeTill,0,@pId,@pPatientId

			IF Exists (Select 1 From @TRoom)
				Update @TRoom SET SchId=@pId
			Else
			Begin
				IF Not Exists (Select 1 From @TRoom Where RoomId=0)			--Check If There is room available.
				Begin
					SET @ExecutedStatus=-2
					SET @ResponseMessage='No Room Available'
				End	
				Else If @EquipmentRequired=1 AND Not Exists (Select 1 From @TRoom Where EqId=0)		--Check If there is equipment avaialble
				Begin
					SET @ExecutedStatus=-3
					SET @ResponseMessage='No Equipment Available'
				End	
				Else
				Begin
					SET @ExecutedStatus=-4
					SET @ResponseMessage='Error while fetching Rooms and Equipments'
				End
			End

			IF Exists (Select 1 From Scheduling Where CAST(ScheduleFrom as date)=@pAppointmentDate
			And (
			(DATEADD(MINUTE,1,CAST(@pTimeFrom as time)) BETWEEN Cast(ScheduleFrom as time) And Cast(ScheduleTo as time))
			OR (DATEADD(MINUTE,-1,CAST(@pTimeTill as time)) BETWEEN Cast(ScheduleFrom as time) And Cast(ScheduleTo as time))
			)
			And IsActive=1
			And [Status] NOT IN ('4','5')
			And PhysicianId=@pClinicianId
			And SchedulingId!=@PId
			)
			Begin
				SET @ExecutedStatus=-1
				SET @ResponseMessage='There is already an Scheduled Appointment at this time. Try different timings.'
			End
		
			IF @ExecutedStatus=0 AND @pId=0
			BEGIN
				INSERT INTO [dbo].[Scheduling]
				([AssociatedId],[AssociatedType],[SchedulingType],[Status],[StatusType],[ScheduleFrom],[ScheduleTo]
				,[TypeOfVisit],[PhysicianSpeciality],[PhysicianId],[TypeOfProcedure],[FacilityId],[CorporateId]
				,[Comments],[Location],[CreatedBy],[CreatedDate],[IsActive],[ExtValue1],[ExtValue3],[ExtValue4]
				,[IsRecurring],[EventId],[WeekDay],[EventParentId],[RoomAssigned],[EquipmentAssigned],[PhysicianReferredBy])
				Select @pPatientId,'1','1' As SchedulingType,'1',NULL As StatusType,@ScheduleTimeFrom,@ScheduleTimeTill
				,@TypeOfVisit,@pSpecialty,@pClinicianId,@pAppointmentTypeId,@FId,@CId
				,ISNULL(@pAppDetails,''),@FacilityName,@pCreatedBy,@LocalDateTime,1 As IsActive,DepId,'False' As MultipleProcedure,@pToken
				, 0 As IsRecurring,@pEventId,@pWeekDay,@pEventParentId,RoomId,EqId,@pClinicianReferredBy
				From @TRoom

				Set @ExecutedStatus=SCOPE_IDENTITY()
			END
			Else IF @pId > 0
			BEGIN
				Update S Set S.ScheduleFrom=@ScheduleTimeFrom,S.ScheduleTo=@ScheduleTimeTill
				,S.PhysicianId=@pClinicianId,S.TypeOfProcedure=@pAppointmentTypeId
				,S.FacilityId= @FId,S.CorporateId= @CId
				,S.ModifiedBy=@pCreatedBy,S.ModifiedDate=@LocalDateTime
				,S.RoomAssigned=T.RoomId, S.ExtValue1=T.DepId
				,S.[EquipmentAssigned]=T.EqId
				From Scheduling S
				INNER JOIN @TRoom T ON S.SchedulingId=T.SchId
				Where S.SchedulingId=@pId And S.[Status]='1' --Check Appointment Status: It should be Always be Initial Booking in case of updates.

				Set @ExecutedStatus=@pId
			END
		End
	End Try
	Begin Catch
		DECLARE @ErrorMessage NVARCHAR(4000);  
		DECLARE @ErrorSeverity INT;  
		DECLARE @ErrorState INT;  

		SELECT @ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE();  

		SET @ExecutedStatus=0
		SET @ResponseMessage = @ErrorMessage

		-- Use RAISERROR inside the CATCH block to return error  
		-- information about the original error that caused  
		-- execution to jump to the CATCH block.  
		RAISERROR (@ErrorMessage, -- Message text.  
				@ErrorSeverity, -- Severity.  
				@ErrorState -- State.  
				); 

		SET @ExecutedStatus=0
		SET @ResponseMessage = @ErrorMessage
	End Catch

	Select @ExecutedStatus As [Status], @ResponseMessage As [Message]
END