using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class EncounterCustomModel
    {
        public int EncounterID { get; set; }
        public string EncounterNumber { get; set; }
        public DateTime? EncounterRegistrationDate { get; set; }
        public DateTime? EncounterStartTime { get; set; }
        public DateTime? EncounterInpatientAdmitDate { get; set; }
        public string EncounterRegistrationPersonnel { get; set; }
        public string PersonEmiratesIDNumber { get; set; }
        public int? PatientID { get; set; }
        public int? EncounterFacilityID { get; set; }
        public string EncounterFacility { get; set; }
        public int? EncounterStartType { get; set; }
        public int? EncounterConfidentialityLevel { get; set; }
        public string EncounterProvideLocation { get; set; }
        public string EncounterAmbulatoryCondition { get; set; }
        public int? EncounterModeofArrival { get; set; }
        public string EncounterPrimaryReferralSource { get; set; }
        public string EncounterReferralSource { get; set; }
        public string EncounterReferringHospital { get; set; }
        public string EncounterReferringClinic { get; set; }
        public string EncounterTransferHospital { get; set; }
        public int? EncounterPatientType { get; set; }
        public string EncounterMedicalService { get; set; }
        public int? EncounterServiceCategory { get; set; }
        public int? EncounterAdmitType { get; set; }
        public string EncounterAdmitReason { get; set; }
        public int? EncounterAccidentRelated { get; set; }
        public string EncounterAccidentType { get; set; }
        public DateTime? EncounterAccidentDate { get; set; }
        public bool? EncounterPoliceInvolved { get; set; }
        public int? EncounterSpecimenType { get; set; }
        public string EncounterTrackingGroup { get; set; }
        public string EncounterNurseAmbulatory { get; set; }
        public int? EncounterType { get; set; }
        public string EncounterAccomodation { get; set; }
        public string EncounterAccomodationRequest { get; set; }
        public string EncounterAccomodationReason { get; set; }
        public string EncounterIsolation { get; set; }
        public string EncounterTransferSource { get; set; }
        public string EncounterReferringFacilityAccess { get; set; }
        public string EncounterReferringMRN { get; set; }
        public string EncounterDischargePlan { get; set; }
        public string EncounterTransferDestination { get; set; }
        public string EncounterDischargeLocation { get; set; }
        public DateTime? EncounterEndTime { get; set; }
        public string EncounterDischargeDisposition { get; set; }
        public string EncounterDischargePersonnelID { get; set; }
        public string EncounterAcuteCareFacilitynonUAE { get; set; }
        public DateTime? EncounterDeceasedDate { get; set; }
        public string EncounterComment { get; set; }
        public int? EncounterEndType { get; set; }
        public string EncounterEligibilityIDPayer { get; set; }
        public decimal? Charges { get; set; }
        public decimal? Payment { get; set; }
        public int? EncounterSpecialty { get; set; }
        public string EncounterLocation { get; set; }
        public string WorkingDiagnosis { get; set; }
        public int? EncounterPhysicianType { get; set; }
        public int? EncounterAttendingPhysician { get; set; }
        public int? CorporateID { get; set; }
        public bool? IsAutoClosed { get; set; }
        public bool? HomeCareRecurring { get; set; }
        public bool? IsMCContractBaseRateApplied { get; set; }

        public PatientInfo PatientInfo { get; set; }
        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string WaitTime { get; set; }
        public string PatientState { get; set; }
        public string Triage { get; set; }

        public DateTime? ENMCreateDate { get; set; }
        public string FacilityName { get; set; }
        public string PatientName { get; set; }
        public string PersonMedicalRecordNumber { get; set; }
        public string EncounterState { get; set; }
        public string patientBedId { get; set; }// added to get the or save patient Bed id
        public string patientBedStartDate { get; set; }
        public string patientBedExpectedEndDate { get; set; }
        public string patientBedService { get; set; }
        public string patientBedEndDate { get; set; }
        public string BedName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string EncounterTypeName { get; set; }
        public string EncounterPatientTypeName { get; set; }
        public string EncounterPatientTypecheck { get; set; }
        public string BedId { get; set; }
        public int? Age { get; set; }

        public string EncounterSpecialityName { get; set; }
        public string EncounterModeOfArrivalName { get; set; }
        public string EncounterServiceCategoryName { get; set; }
        public string EncounterPhysicianTypeName { get; set; }
        public string EncounterPhysicianName { get; set; }
        public string EncounterAdmitTypeName { get; set; }
        public string PatientIsVIP { get; set; }

        public string WaitingTime { get; set; }

        public string TriageValue { get; set; }
        public string PatientStageName { get; set; }


        public string InsuranceCompanyName { get; set; }
        public string InsuranceCompanyPhoneNumber { get; set; }
        public string InsuranceCompanyFaxNumber { get; set; }
        public string InsuranceCompanyAddress { get; set; }

        public Authorization EncounterAuthorization { get; set; }
        public List<AuthorizationCustomModel> EncounterAuthorizationList { get; set; }

        public List<DocumentsTemplates> AuthDocs { get; set; } = new List<DocumentsTemplates>();
        public string OverrideBedType { get; set; }

        public List<BillDetailCustomModel> EncounterBedTransaction { get; set; }
        public bool IsPrimaryDiagnosisDone { get; set; }

        public string ErrorStatus { get; set; }
        public bool IsEncounterAuthorized { get; set; }
        public string PrimaryDiagnosisDescription { get; set; }

        public string FloorName { get; set; }
        public string DepartmentName { get; set; }
        public string Room { get; set; }
        public string BedRateApplicable { get; set; }
        public DateTime? BedAssignedOn { get; set; }

        public string VirtuallyDischarge { get; set; }
        public string VirtuallyDischargeOn { get; set; }


        public bool EhrViewAccessible { get; set; }
        public bool PatientInfoAccessible { get; set; }
        public bool EncounterViewAccessible { get; set; }

        public bool IsDRGExist { get; set; }
        public string AverageLengthofStay { get; set; }
        public string ExpectedLengthofStay { get; set; }
        public bool ActualMoreThanExpected { get; set; }
        public string PhysicianName { get; set; }
        public int BillHeaderId { get; set; }

        public int TriageSortingValue { get; set; }
    }

    [NotMapped]
    public class EncounterExtension
    {
        public string Title { get; set; }
        public int Code { get; set; }
        public string XAxis { get; set; }
        public int YAxis { get; set; }
    }
    [NotMapped]
    public class EncounterEndCheckReturnStatus
    {
        public int RetStatus { get; set; }
    }
    [NotMapped]
    public class CMODashboardModel
    {
        public string PatientIsVIP { get; set; }
        public int CID { get; set; }
        public int FID { get; set; }
        public int EncounterId { get; set; }
        public int PatientId { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public DateTime? PersonBirthDate { get; set; }
        public string EncounterNumber { get; set; }
        public DateTime? EncounterStartTime { get; set; }
        public string EncounterType { get; set; }
        public int EncounterPatientType { get; set; }
        public string PrimaryDiagnosis { get; set; }
        public string BedAssigned { get; set; }
        public string BedServiceCode { get; set; }
        public int? ALOS { get; set; }
        public string ELOS { get; set; }
        public string DiagnosisStatus { get; set; }
        public string LabResultStatus { get; set; }
        public string MedicationStatus { get; set; }
        public string VitalStatus { get; set; }
        public string NurseTaskStatus { get; set; }
        public bool IsDRGExist { get; set; }
    }
}
