
-- Drop stored procedure if it already exists
IF OBJECT_ID('iSprocGetCliniciansAndSpecialtiesByAppType','P') IS NOT NULL
   DROP PROCEDURE iSprocGetCliniciansAndSpecialtiesByAppType
GO

CREATE PROCEDURE iSprocGetCliniciansAndSpecialtiesByAppType
(
@pAppointmentTypeId bigint,
@pCityId bigint=null
)
As
Begin
	
	SET @pCityId=ISNULL(@pCityId,0)

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
	Where P.Id IN (Select DISTINCT A.ClinicianId From ClinicianAppointmentType A Where A.AppointmentTypeId=@pAppointmentTypeId)
	AND (@pCityId=0 OR F.FacilityCity=@pCityId)


	Select Specialities=
	(
	Select DISTINCT T1.SpecialtyId As Id,T1.SpecialtyName As [Name] From @TClinicians T1
	For JSON PATH
	),
	Clinicians=
	(
	Select DISTINCT T2.ClinicianId,T2.ClinicianName,T2.CityId,T2.StateId,T2.CountryId,T2.SpecialtyId,T2.SpecialtyName
	From @TClinicians T2
	For JSON PATH
	)
	For Json PATH, Root ('CliniciansData')



	--Select DISTINCT SpecialtyId As Id,SpecialtyName As [Name] From @TClinicians


	--Select DISTINCT ClinicianId,ClinicianName,CityId,StateId,CountryId,SpecialtyId,SpecialtyName From @TClinicians
End