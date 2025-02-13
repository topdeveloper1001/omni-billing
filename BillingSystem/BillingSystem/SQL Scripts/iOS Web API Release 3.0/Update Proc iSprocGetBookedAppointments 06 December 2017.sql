USE [OmniStagingDB]
GO
/****** Object:  StoredProcedure [dbo].[iSprocGetBookedAppointments]    Script Date: 06-12-2017 11:13:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[iSprocGetBookedAppointments]
(
@pPatientId bigint,
@pUserId bigint,
@pOnlyUpcoming bit=0,
@pId bigint=null
)
AS
BEGIN
	Declare @LocalDateTime datetime	
	SET @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone('+04:00',GETDATE()))
			

	Select 
	--ROW_NUMBER() OVER (ORDER BY S.ScheduleFrom DESC) AS RN,
	S.SchedulingId As Id, CAST(S.ScheduleFrom as date) As AppointmentDate, CONVERT(nvarchar(5),S.ScheduleFrom,114) As TimeFrom
	,CONVERT(nvarchar(5),S.ScheduleTo,114) As TimeTill
	,CAST(S.AssociatedId as bigint) As PatientId,CAST(S.PhysicianId as bigint) As ClinicianId
	,(Select TOP 1 PhysicianName From Physician P Where P.Id=S.PhysicianId) As ClinicianName
	,S.TypeOfVisit,S.ExtValue1 As DepartmentId,S.RoomAssigned,S.EquipmentAssigned,S.[Location]
	,CAST(S.TypeOfProcedure as int) As AppointmentTypeId, ISNULL(S.Comments,'') As [Description]
	,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue='4903' 
	And GlobalCodeValue=S.[Status]) As [Status]
	,ISNULL(S.TypeOfVisit,'') As AppointmentName
	,S.CreatedBy As UserId
	From Scheduling S
	Where
	IsActive=1
	And (ISNULL(@pPatientId,0)=0 OR S.AssociatedId=@pPatientId)
	And (ISNULL(@pUserId,0)=0 OR FacilityId=(Select TOP 1 FacilityId From [Users] Where UserId=@pUserId))	
	And (ISNULL(@pId,0)=0 OR S.SchedulingId=@pId)
	And (ISNULL(@pOnlyUpcoming,0)=0 OR S.ScheduleFrom >= @LocalDateTime)
	Order by S.ScheduleFrom Desc
	For Json Path, Root('BookedAppointments')
END





