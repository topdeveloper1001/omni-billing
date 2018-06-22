using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class PatientInfoCustomModel
    {
        public int Id { get; set; }
        public PatientInfo PatientInfo { get; set; }
        public bool PatientSex { get; set; }
        public bool PatientMaritalStatus { get; set; }
        public bool? PatientIsVIP { get; set; }
        public int PersonAge { get; set; }
        public string ProfilePicImagePath { get; set; }
        public int? DocumentTemplateId { get; set; }
        public string PatientName { get; set; }
        public string AttendingPhysician { get; set; }
        public Encounter CurrentEncounter { get; set; }
        public string FacilityName { get; set; }
        public int CorporateId { get; set; }
        public string CorporateName { get; set; }
        public string PrimaryPhysician { get; set; }
        public int PhysicianId { get; set; }
        public bool IsAuthorizationExist { get; set; }
        public bool IsEncounterExist { get; set; }
        public int DenailEncounterID { get; set; }

        public string FloorName { get; set; }
        public string DepartmentName { get; set; }
        public string Room { get; set; }
        public string BedRateApplicable { get; set; }
        public DateTime? BedAssignedOn { get; set; }
        public string patientBedService { get; set; }
        public string BedName { get; set; }

        public bool PatientInfoAccessible { get; set; }
        public bool EhrViewAccessible { get; set; }
        public bool TransactionsViewAccessible { get; set; }
        public bool AuthorizationViewAccessible { get; set; }
        public bool BillHeaderViewAccessible { get; set; }
        public bool OrdersPendingToAdminister { get; set; }
        public bool SchedularViewAccessible { get; set; }

        public string Text { get; set; }
        public string Value { get; set; }

        public int? PatientActiveEncounterFacilityId { get; set; }

    }
    [NotMapped]
    public class ImageInfoModel
    {
        public string FileName { get; set; }
        public string ImageUrl { get; set; }
    }

    [NotMapped]
    public class PatientInfoXReturnPaymentCustomModel
    {
        public string IsPersonVIP { get; set; }
        public int PatientID { get; set; }
        public int EncounterID { get; set; }
        public DateTime EncounterStartTime { get; set; }
        public DateTime? EncounterEndTime { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public string PersonEmiratesIDNumber { get; set; }
        public string PersonPassportNumber { get; set; }
        public DateTime PersonBirthdate { get; set; }
        public string PhoneNo { get; set; }
        public int? AActivityID { get; set; }
        public Int32 BillHeaderID { get; set; }
        public string DenialCode { get; set; }
        public string AADenialCode { get; set; }
        public string BillNumber { get; set; }
        public string Status { get; set; }
        public string EncounterNumber { get; set; }
        public string BillStatus { get; set; }
        public string EncounterType { get; set; }
        public string EStartFormatted { get; set; }
        public string EEndFormatted { get; set; }
        public string VirtualDischarge { get; set; }
    }

    [NotMapped]
    public class DropdownListData
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public string CategoryValue { get; set; }
        public string ExternalValue1 { get; set; }
        public string ExternalValue2 { get; set; }
        public string ExternalValue3 { get; set; }
        public int? SortOrder { get; set; }
        public long Id { get; set; }
    }

    [NotMapped]
    public class RegisterPatientCustomModel
    {
        public string PersonFirstName { get; set; }
        public string PersonSecondName { get; set; }
        public string PersonPhoneNumber { get; set; }
        public string PersonEmailId { get; set; }
        public string PersonDateOfBirth { get; set; }
        public string PersonGender { get; set; }
        public string EmirateId { get; set; }
        public int CorporateId { get; set; }
        public int FacilityId { get; set; }
    }
}
