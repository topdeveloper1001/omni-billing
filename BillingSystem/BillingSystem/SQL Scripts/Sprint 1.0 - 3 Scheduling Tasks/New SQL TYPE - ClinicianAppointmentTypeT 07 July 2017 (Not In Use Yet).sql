
IF TYPE_ID(N'ClinicianAppointmentTypeT') IS NOT NULL
	DROP TYPE ClinicianAppointmentTypeT


-- Create the data type
CREATE TYPE ClinicianAppointmentTypeT AS TABLE
(
	[Id] [bigint],
	[ClinicianId] [bigint],
	[AppointmentTypeId] [bigint]
)