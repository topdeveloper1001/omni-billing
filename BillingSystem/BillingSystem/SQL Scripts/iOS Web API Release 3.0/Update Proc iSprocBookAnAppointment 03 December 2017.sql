USE [OmniStagingDB]
GO
/****** Object:  StoredProcedure [dbo].[iSprocBookAnAppointment]    Script Date: 03-12-2017 20:47:18 ******/
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
ALTER PROCEDURE [dbo].[iSprocBookAnAppointment]
(
@pId bigint,
@pPatientId bigint,
@pClinicianId bigint,
@pAppointmentTypeId bigint,
@pSpecialty bigint,
@pPatientLocation nvarchar(200),
@pAppointmentDate datetime,
@pTimeFrom nvarchar(10),
@pTimeTill nvarchar(10),
@pCreatedBy bigint,
@pTitle nvarchar(100)=null,
@pClinicianReferredBy bigint=null,
@pAppDetails nvarchar(400)=null,
@pCountryId bigint,
@pStateId bigint,
@pCityId bigint
)
AS
BEGIN
	---###### Declarations ##################

	Declare @LocalDateTime datetime, @TimeZone nvarchar(50)
	Declare @AddedToSchedulingFlag bit=0,@AlreadyExists bit=0

	---###### Declarations ##################

	--Currently working only for UAE Times.
	SET @TimeZone = '+04:00'

	SET @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,GETDATE()))

	SET @pAppointmentDate=CAST(@pAppointmentDate as date)

	IF Exists (Select 1 From Appointment Where CAST([ScheduleDate] as date)=@pAppointmentDate
	And (
		(DATEADD(MINUTE,1,CAST(@pTimeFrom as time)) BETWEEN Cast(TimeFrom as time) And Cast(TimeTill as time))
		OR (DATEADD(MINUTE,-1,CAST(@pTimeTill as time)) BETWEEN Cast(TimeFrom as time) And Cast(TimeTill as time))
		))
	Begin
		Set @AlreadyExists=1
		Select -2
		Return;
	End

	IF ISNULL(@pId,0)=0
	Begin
		INSERT INTO Appointment ([Title],[AppointmentDetails],[AppointmentTypeId],[ClinicianId],[Specialty],[PatientId]
		,[ClinicianReferredBy],[Status],[ScheduleDate],[TimeFrom],[TimeTill],[Address],[CreatedBy],[CreatedDate]
		,[IsActive],IsAddedToMain,CountryId,StateId,CityId)
		Select @pTitle,@pAppDetails,@pAppointmentTypeId,@pClinicianId,@pSpecialty,@pPatientId,@pClinicianReferredBy,'New',@pAppointmentDate
		,@pTimeFrom,@pTimeTill,@pPatientLocation,@pCreatedBy,@LocalDateTime,1,0,@pCountryId,@pStateId,@pCityId

		Set @pId=SCOPE_IDENTITY()
	End
	Else
	Begin
		IF Exists (Select 1 From Scheduling Where ExtValue5=@pId)
			Set @AddedToSchedulingFlag=1

		If @AddedToSchedulingFlag=1
		Begin
			Update Scheduling Set ScheduleFrom= DATEADD(day, DATEDIFF(day,'19000101',@pAppointmentDate), CAST(@pTimeFrom AS DATETIME2(7)))
			,ScheduleTo=DATEADD(day, DATEDIFF(day,'19000101',@pAppointmentDate), CAST(@pTimeTill AS DATETIME2(7)))
			,PhysicianId=@pClinicianId,TypeOfProcedure=@pAppointmentTypeId
			,FacilityId= (Select TOP 1 FacilityId From Physician Where Id=@pClinicianId)
			,CorporateId= (Select TOP 1 CorporateId From Physician Where Id=@pClinicianId)			
			Where ExtValue5=@pId
		End

		Update Appointment Set ScheduleDate=@pAppointmentDate,TimeFrom=@pTimeFrom,TimeTill=@pTimeTill
		,AppointmentTypeId=AppointmentTypeId,IsAddedToMain=Cast(@AddedToSchedulingFlag as nvarchar)
		,ModifiedBy=@pCreatedBy,ModifiedDate=@LocalDateTime
		,[Status]= (Case @AddedToSchedulingFlag WHEN 1 THEN 'Close' ELSE 'New' END)
		,CountryId=@pCountryId, StateId=@pStateId,CityId=@pCityId
		,ClinicianReferredBy=@pClinicianReferredBy
		Where Id=@pId
	End

	Select @pId As AppointmentId
END





