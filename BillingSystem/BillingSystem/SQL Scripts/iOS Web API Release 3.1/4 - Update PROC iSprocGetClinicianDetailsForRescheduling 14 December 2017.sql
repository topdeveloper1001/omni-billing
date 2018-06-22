
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
	,CountryId int,FacilityId bigint,SpecialtyId nvarchar(10),SpecialtyName nvarchar(100),CountryName nvarchar(100),StateName nvarchar(100),CityName nvarchar(100))

	INSERT INTO @TClinicians (ClinicianId,ClinicianName,SpecialtyId,SpecialtyName,CityId,StateId,CountryId,CountryName,StateName,CityName)
	Select P.Id, P.PhysicianName,ISNULL(CAST(P.FacultySpeciality as bigint),0) As SpecialtyId
	,(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeCategoryValue='1121' And P.FacultySpeciality=G.GlobalCodeValue) As SpecialtyName
	,CAST(F.FacilityCity As int) As CityId
	,CAST(F.FacilityState As int) As StateId
	,CAST(F.CountryID As int) As CountryId
	,(Select TOP 1 C.CountryName From Country C Where C.CountryId=F.CountryID) As CountryName
	,(Select TOP 1 S.StateName From [State] S Where S.StateId=F.FacilityState) As StateName
	,(Select TOP 1 C1.[Name] From City C1 Where C1.CityId=F.FacilityCity) As CityName
	From Physician P
	INNER JOIN Facility F ON P.FacilityId=F.FacilityId
	Where P.Id=@pClinicianId

	Select DISTINCT T.ClinicianId,T.ClinicianName,T.CityId, T.CityName,T.StateName,T.StateId, T.CountryName,T.CountryId,T.SpecialtyId,T.SpecialtyName
	,AppointmentTypes=(
	Select CA.AppointmentTypeId
	,Title= (Select TOP 1 A.[Name] From AppointmentTypes A Where A.Id=CA.AppointmentTypeId)
	From ClinicianAppointmentType CA 
	Where CA.ClinicianId=T.ClinicianId FOR JSON PATH
	)
	From @TClinicians T
	FOR JSON PATH, ROOT('ClinicanDetail')
End