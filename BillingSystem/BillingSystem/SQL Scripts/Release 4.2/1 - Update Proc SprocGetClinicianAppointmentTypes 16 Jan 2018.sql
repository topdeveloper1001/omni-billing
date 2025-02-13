IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetClinicianAppointmentTypes')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE SprocGetClinicianAppointmentTypes
GO
/****** Object:  StoredProcedure [dbo].[SprocGetClinicianAppointmentTypes]    Script Date: 16-01-2018 15:19:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetClinicianAppointmentTypes]
(
@FId bigint,
@LoggedInUserId bigint,
@ClinicianId bigint
)
AS
BEGIN
	IF @ClinicianId=0				--Get ALL
	Begin
		;With Physicians 
		As
		(
		Select Id, PhysicianName From
		Physician P
		INNER JOIN [Role] R ON P.UserType = R.RoleId
		Where P.FacilityId = @FId
		)

		Select C.*,P.PhysicianName As ClinicianName
		,(Select TOP 1 [Name] From AppointmentTypes Where Id=C.AppointmentTypeId) As AppointmentType 
		From ClinicianAppointmentType C 
		INNER JOIN Physician P ON C.ClinicianId = P.Id
		Where P.Id IN (Select Id From Physicians)
		And C.IsDeleted=0
	End
	Else		--Get SINGLE
	Begin
		Select C.*
		,(Select TOP 1 P.PhysicianName From Physician P Where P.Id=C.ClinicianId) As ClinicianName
		,(Select TOP 1 [Name] From AppointmentTypes Where Id=C.AppointmentTypeId) As AppointmentType 
		From ClinicianAppointmentType C 
		Where C.ClinicianId=@ClinicianId
		And C.IsDeleted=0
	End
END
