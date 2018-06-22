IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetPatientDetails')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetPatientDetails

GO

/****** Object:  StoredProcedure [dbo].[SprocGetPatientDetails]    Script Date: 5/3/2018 9:43:06 PM ******/
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
CREATE PROCEDURE [dbo].[SprocGetPatientDetails]
(
@PId bigint,
@EId bigint=0,
@ShowEncounters bit=1
)
AS
BEGIN
	Declare @LocalTime datetime--, @TimeZone nvarchar(50)
	
	
	Declare @EPatientType int=0, @EncounterAttendingPhysician bigint, @FId int
	Declare @PatientBedInfo Table (PId int, PatientName nvarchar(100), BedName nvarchar(100),Room nvarchar(100), DepartmentName nvarchar(100), FloorName nvarchar(100), 
	BedAssignedOn Date, patientBedService nvarchar(20), BedRateApplicable nvarchar(10))

	

	--Get Most Recent Encounter Details.
	Select TOP 1 @EId=E.EncounterID, @EPatientType = EncounterPatientType, @EncounterAttendingPhysician = EncounterAttendingPhysician
	,@FId = EncounterFacilityID
	From Encounter E Where E.PatientID = @PId And (@EId=0 OR EncounterID=@EId)
	--Order by EncounterInpatientAdmitDate
	Order by EncounterEndTime

	Set @LocalTime = (Select dbo.GetCurrentDatetimeByEntity(@FId))

	---Get Bed Info incase of Encounter is of IP (InPatient).
	If @EPatientType = 2
	Begin 
		INSERT INTO @PatientBedInfo
		EXEC SPROC_GetPatientBedInformation @PId
	End

	Select * From PatientInfo Where PatientID = @PId And ISNULL(IsDeleted,0)=0

	;WITH PatientDetails (PatientName,FacilityName,CorporateName,ProfilePicImagePath,DocumentTemplateId
	  ,BedAssignedOn,BedName,BedRateApplicable,DepartmentName,FloorName,patientBedService,Room
	  ,PhysicianId,PrimaryPhysician,OrdersPendingToAdminister)
	As
	(
	Select P1.PatientName,
	
	(Select TOP 1 F.FacilityName From Facility F Where F.FacilityId = P1.FacilityId And F.IsActive=1 And ISNULL(F.IsDeleted,0)=0) As FacilityName,
	(Select TOP 1 C.CorporateName From Corporate C Where C.CorporateID = P1.CorporateId And ISNULL(C.IsDeleted,0)=0) As CorporateName,

	(Case 
	 When ISNULL(D.FilePath,'') != ''
		THEN D.FilePath
	 ELSE
		'/images/BlankProfilePic.png'
	End) As ProfilePicImagePath,
	ISNULL(D.DocumentsTemplatesID,'') As DocumentTemplateId,
	ISNULL(B.BedAssignedOn,'') As BedAssignedOn,
	ISNULL(B.BedName,'') As BedName,
	ISNULL(B.BedRateApplicable,'') As BedRateApplicable,
	ISNULL(B.DepartmentName,'') As DepartmentName,
	ISNULL(B.FloorName,'') As FloorName,
	ISNULL(B.patientBedService,'') As patientBedService,
	ISNULL(B.Room,'') As Room,

	ISNULL(P.Id,'') As PhysicianId,
	ISNULL(P.PhysicianName,'') As PrimaryPhysician,
	
	(Select Count(1) From OrderActivity OA Where 
	OA.EncounterID = @EId And OA.OrderScheduleDate <= @LocalTime And OA.OrderActivityStatus = 1) As OrdersPendingToAdminister

	From
	(
	Select P.PatientID, P.FacilityId,P.CorporateId, (P.PersonFirstName + ' ' + P.PersonLastName) As PatientName
	From PatientInfo P Where P.PatientID = @PId And ISNULL(P.IsDeleted,0)=0 
	) P1
	LEFT OUTER JOIN DocumentsTemplates D ON P1.PatientID = D.AssociatedID And D.AssociatedType = 1
	LEFT OUTER JOIN @PatientBedInfo B ON P1.PatientID = B.PId
	LEFT OUTER JOIN Physician P ON P.Id = @EncounterAttendingPhysician
	)

	--Select * FROM PatientDetails


	Select PatientName,FacilityName,CorporateName,ProfilePicImagePath,DocumentTemplateId
	  ,BedAssignedOn,BedName,BedRateApplicable,DepartmentName,FloorName,patientBedService,Room
	  ,PhysicianId,PrimaryPhysician, Cast(OrdersPendingToAdminister as bit) as  OrdersPendingToAdminister From PatientDetails


	IF @ShowEncounters=1
		Select * From Encounter Where EncounterID = @EId
END

GO


