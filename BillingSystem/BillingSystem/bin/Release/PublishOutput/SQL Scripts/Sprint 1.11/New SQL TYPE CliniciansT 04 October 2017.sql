IF TYPE_ID(N'CliniciansT') IS NOT NULL
	DROP TYPE dbo.CliniciansT

GO
-- Create the data type
CREATE TYPE CliniciansT As Table 
(
ClinicianId bigint,
ClinicianName nvarchar(100),
OpeningTime nvarchar(10),
ClosingTime nvarchar(10),
DepartmentId bigint,
DepartmentName nvarchar(100)
)
GO
