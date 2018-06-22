IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocSaveClinicianAppointmentTypeAssignments') 
  DROP PROCEDURE SprocSaveClinicianAppointmentTypeAssignments;

Go 

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE SprocSaveClinicianAppointmentTypeAssignments
(
@ClinicianAppointmentType ClinicianAppointmentTypeT ReadOnly,
@LoggedInUserId bigint
)
AS
BEGIN
	DECLARE @LocalDateTime datetime, @TimeZone nvarchar(50), @FId bigint

	Select TOP 1 @FId=FacilityId From [Users] Where UserId=@LoggedInUserId
	set @TimeZone=(Select TimeZ from Facility Where Facilityid = @FId)
	Set @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,GETDATE()))

	INSERT INTO [dbo].[ClinicianAppointmentType] ([ClinicianId]
    ,[AppointmentTypeId],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted])
	Select A.ClinicianId,A.AppointmentTypeId,A.CreatedBy,@LocalDateTime,null,null,0 From @ClinicianAppointmentType A 
	Where Not Exists 
	(Select * From [dbo].[ClinicianAppointmentType] C 
		Where A.ClinicianId=C.ClinicianId And A.AppointmentTypeId=C.AppointmentTypeId)


	Exec SprocGetClinicianAppointmentTypes @FId,@LoggedInUserId,0
END
GO
