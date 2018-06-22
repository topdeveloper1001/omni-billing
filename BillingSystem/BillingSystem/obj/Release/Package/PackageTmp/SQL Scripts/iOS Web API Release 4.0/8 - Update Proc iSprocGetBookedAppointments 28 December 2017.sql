IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'iSprocGetBookedAppointments')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE iSprocGetBookedAppointments


GO
/****** Object:  StoredProcedure [dbo].[iSprocGetBookedAppointments]    Script Date: 20-12-2017 22:58:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Exec [iSprocGetBookedAppointments] @pAppointmentStatus='14,15'
-- Exec [iSprocGetBookedAppointments] @pId=16645
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[iSprocGetBookedAppointments]
(
@pPatientId bigint=null,
@pUserId bigint=null,
@pOnlyUpcoming bit=null,
@pId bigint=null,
@pForToday bit=null,
@pFId bigint=null,
@pAppointmentStatus nvarchar(50)=null,
@pAppointmentDate datetime=null
)
AS
BEGIN
	Declare @LocalDateTime datetime
	SET @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone('+04:00',GETDATE()))
	
	SET @pPatientId = ISNULL(@pPatientId,0)
	SET @pUserId = ISNULL(@pUserId,0)
	--SET @pOnlyUpcoming = ISNULL(@pOnlyUpcoming,0)
	SET @pId = ISNULL(@pId,0)
	SET @pForToday = ISNULL(@pForToday,0)
	SET @pFId = ISNULL(@pFId,0)
	SET @pAppointmentStatus = ISNULL(@pAppointmentStatus,'')

	IF @pId > 0
	Begin
		Select 
		--ROW_NUMBER() OVER (ORDER BY S.ScheduleFrom DESC) AS RN,
		S.SchedulingId As Id, CAST(S.ScheduleFrom as date) As AppointmentDate, CONVERT(nvarchar(5),S.ScheduleFrom,114) As TimeFrom
		,CONVERT(nvarchar(5),S.ScheduleTo,114) As TimeTill
		,CAST(S.AssociatedId as bigint) As PatientId
		,(Select TOP 1 (P1.PersonFirstName + ' ' + P1.PersonLastName) From PatientInfo P1 Where S.AssociatedId=P1.PatientID) As PatientName
		,CAST(S.PhysicianId as bigint) As ClinicianId
		,(Select TOP 1 PhysicianName From Physician P Where P.Id=S.PhysicianId) As ClinicianName
		,S.TypeOfVisit,S.ExtValue1 As DepartmentId,S.RoomAssigned,S.EquipmentAssigned,S.[Location]
		,CAST(S.TypeOfProcedure as int) As AppointmentTypeId, ISNULL(S.Comments,'') As [Description]
		,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue='4903' 
		And GlobalCodeValue=S.[Status]) As [Status]
		,ISNULL(S.TypeOfVisit,'') As AppointmentName
		,S.CreatedBy As UserId
		,IsFavorite=(Case
			WHEN Exists (Select 1 From FavoriteClinician Where PatientId=@pPatientId And ClinicianId=S.PhysicianId)
			THEN Cast(1 As bit)
			Else Cast(0 As bit)
		 End)
		 ,FavoriteId=(Select TOP 1 ISNULL(Id,0) From FavoriteClinician Where PatientId=@pPatientId And ClinicianId=S.PhysicianId)
		,Cast(F.FacilityId as bigint) As FacilityId
		,F.[FacilityName] As FacilityName
		,Cast(F.FacilityCity as bigint) As CityId
		,(Select TOP 1 CT.[Name] From City CT Where CT.CityId=F.FacilityCity) As CityName
		,Cast(F.FacilityState as bigint) As StateId
		,(Select TOP 1 SS.StateName From [State] SS Where SS.StateId=F.FacilityState) As StateName
		,Cast(F.CountryId as bigint) As CountryId
		,(Select TOP 1 C.CountryName From Country C Where C.CountryId=F.CountryId) As CountryName
		,Cast(S.PhysicianSpeciality as bigint) As SpecialtyId
		,(Select TOP 1 G.GlobalCodeName From  GlobalCodes G Where G.GlobalCodeValue=S.PhysicianSpeciality 
		And G.GlobalCodeCategoryValue='1121') As SpecialtyName
		From Scheduling S
		INNER JOIN Facility F ON S.FacilityId=F.FacilityId
		Where S.SchedulingId=@pId
	End
	Else
	Begin
		Select 
		--ROW_NUMBER() OVER (ORDER BY S.ScheduleFrom DESC) AS RN,
		S.SchedulingId As Id, CAST(S.ScheduleFrom as date) As AppointmentDate, CONVERT(nvarchar(5),S.ScheduleFrom,114) As TimeFrom
		,CONVERT(nvarchar(5),S.ScheduleTo,114) As TimeTill
		,CAST(S.AssociatedId as bigint) As PatientId
		,(Select TOP 1 (P1.PersonFirstName + ' ' + P1.PersonLastName) From PatientInfo P1 Where S.AssociatedId=P1.PatientID) As PatientName
		,CAST(S.PhysicianId as bigint) As ClinicianId
		,(Select TOP 1 PhysicianName From Physician P Where P.Id=S.PhysicianId) As ClinicianName
		,S.TypeOfVisit,S.ExtValue1 As DepartmentId,S.RoomAssigned,S.EquipmentAssigned,S.[Location]
		,CAST(S.TypeOfProcedure as int) As AppointmentTypeId, ISNULL(S.Comments,'') As [Description]
		,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue='4903' 
		And GlobalCodeValue=S.[Status]) As [Status]
		,ISNULL(S.TypeOfVisit,'') As AppointmentName
		,S.CreatedBy As UserId
		,IsFavorite=(Case
			WHEN Exists (Select 1 From FavoriteClinician Where PatientId=@pPatientId And ClinicianId=S.PhysicianId)
			THEN Cast(1 As bit)
			Else Cast(0 As bit)
			End)
			,FavoriteId=(Select TOP 1 ISNULL(Id,0) From FavoriteClinician Where PatientId=@pPatientId And ClinicianId=S.PhysicianId)
		From Scheduling S
		Where
		IsActive=1
		And (@pPatientId =0 OR S.AssociatedId=@pPatientId)
		And (@pUserId=0 OR FacilityId=(Select TOP 1 FacilityId From [Users] Where UserId=@pUserId))
		And (
			(@pOnlyUpcoming IS NULL OR (@pOnlyUpcoming=0 AND S.ScheduleFrom <= DATEADD(MINUTE,-1,@LocalDateTime)))
			OR
			(@pOnlyUpcoming=1 AND S.ScheduleFrom >=@LocalDateTime)
			)
		AND 
		(
			@pForToday=0
			OR 
			CAST(S.ScheduleFrom as date)=CAST(@LocalDateTime as date)
		)
		And S.[Status] NOT IN ('6','7','8','9')
		AND
		(@pAppointmentStatus='' OR S.[Status] IN (Select Cast(IDValue as nvarchar) From dbo.Split(',',@pAppointmentStatus)))
		AND
		(@pFId =0 OR S.FacilityId=@pFId)
		AND
		(@pId=0 OR S.SchedulingId=@pId)
		AND (@pAppointmentDate IS NULL OR CAST(S.ScheduleFrom as Date)=CAST(@pAppointmentDate as date))
		Order by S.ScheduleFrom Desc
	End


	--SELECT 
	--BookedAppointments=ISNULL((Select 
	----ROW_NUMBER() OVER (ORDER BY S.ScheduleFrom DESC) AS RN,
	--S.SchedulingId As Id, CAST(S.ScheduleFrom as date) As AppointmentDate, CONVERT(nvarchar(5),S.ScheduleFrom,114) As TimeFrom
	--,CONVERT(nvarchar(5),S.ScheduleTo,114) As TimeTill
	--,CAST(S.AssociatedId as bigint) As PatientId,CAST(S.PhysicianId as bigint) As ClinicianId
	--,(Select TOP 1 PhysicianName From Physician P Where P.Id=S.PhysicianId) As ClinicianName
	--,S.TypeOfVisit,S.ExtValue1 As DepartmentId,S.RoomAssigned,S.EquipmentAssigned,S.[Location]
	--,CAST(S.TypeOfProcedure as int) As AppointmentTypeId, ISNULL(S.Comments,'') As [Description]
	--,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue='4903' 
	--And GlobalCodeValue=S.[Status]) As [Status]
	--,ISNULL(S.TypeOfVisit,'') As AppointmentName
	--,S.CreatedBy As UserId
	--,IsFavorite=(Case
	--	WHEN Exists (Select 1 From FavoriteClinician Where PatientId=@pPatientId And ClinicianId=S.PhysicianId)
	--	THEN Cast(1 As bit)
	--	Else Cast(0 As bit)
	-- End)

	--From Scheduling S
	--Where
	--IsActive=1
	--And S.AssociatedId=@pPatientId
	--And (@pUserId=0 OR FacilityId=(Select TOP 1 FacilityId From [Users] Where UserId=@pUserId))
	--And (ISNULL(@pId,0)=0 OR S.SchedulingId=@pId)
	--And (@pOnlyUpcoming=0 OR S.ScheduleFrom >= @LocalDateTime)
	--Order by S.ScheduleFrom Desc
	--For Json Path),'[]')
	--For Json Path
END





