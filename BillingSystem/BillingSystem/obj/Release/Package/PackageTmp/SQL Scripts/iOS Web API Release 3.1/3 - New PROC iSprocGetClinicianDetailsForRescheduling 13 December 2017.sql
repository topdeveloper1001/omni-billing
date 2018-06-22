
-- Drop stored procedure if it already exists
IF OBJECT_ID('iSprocGetClinicianDetailsForRescheduling','P') IS NOT NULL
   DROP PROCEDURE iSprocGetClinicianDetailsForRescheduling
GO

CREATE PROCEDURE iSprocGetClinicianDetailsForRescheduling
(
@pClinicianId bigint
)
As
Begin
	
	Declare @TClinicians As Table (ClinicianId bigint,ClinicianName nvarchar(200),CityId int,StateId int
	,CountryId int,FacilityId bigint,SpecialtyId nvarchar(10),SpecialtyName nvarchar(100))

	INSERT INTO @TClinicians (ClinicianId,ClinicianName,SpecialtyId,SpecialtyName,CityId,StateId,CountryId)
	Select P.Id, P.PhysicianName,ISNULL(CAST(P.FacultySpeciality as bigint),0) As SpecialtyId
	,(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeCategoryValue='1121' And P.FacultySpeciality=G.GlobalCodeValue) As SpecialtyName
	,CAST(F.FacilityCity As int) As CityId
	,CAST(F.FacilityState As int) As StateId
	,CAST(F.CountryID As int) As CountryId
	From Physician P
	INNER JOIN Facility F ON P.FacilityId=F.FacilityId
	Where P.Id=@pClinicianId

	Select DISTINCT T.ClinicianId,T.ClinicianName,T.CityId,T.StateId,T.CountryId,T.SpecialtyId,T.SpecialtyName
	,AppointmentTypes=(
	Select CA.AppointmentTypeId
	,Title= (Select TOP 1 A.[Name] From AppointmentTypes A Where A.Id=CA.AppointmentTypeId)
	From ClinicianAppointmentType CA 
	Where CA.ClinicianId=T.ClinicianId FOR JSON PATH
	)
	From @TClinicians T
	FOR JSON PATH, ROOT('ClinicanDetail')
End