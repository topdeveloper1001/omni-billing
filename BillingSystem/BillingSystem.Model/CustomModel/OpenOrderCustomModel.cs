using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    using System.Collections.Generic;

    [NotMapped]
    public class OpenOrderCustomModel
    {
        public int OpenOrderID { get; set; }
        public DateTime? OpenOrderPrescribedDate { get; set; }
        public int? PhysicianID { get; set; }
        public int? PatientID { get; set; }
        public int? EncounterID { get; set; }
        public string DiagnosisCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public string OrderType { get; set; }
        public string OrderCode { get; set; }
        public decimal? Quantity { get; set; }
        public string FrequencyCode { get; set; }
        public string PeriodDays { get; set; }
        public string OrderNotes { get; set; }
        public string OrderStatus { get; set; }
        public bool? IsActivitySchecduled { get; set; }
        public DateTime? ActivitySchecduledOn { get; set; }
        public string ItemName { get; set; }
        public string ItemStrength { get; set; }
        public string ItemDosage { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? CorporateID { get; set; }
        public int? FacilityID { get; set; }
        public bool? IsApproved { get; set; }
        public string EV1 { get; set; }
        public string EV2 { get; set; }
        public string EV3 { get; set; }
        public string EV4 { get; set; }

        public string FacilityName { get; set; }
        public string PatientName { get; set; }
        public string PersonEmiratesIDNumber { get; set; }
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
        public string encounterPatientTypercheck { get; set; }


        public int OrderCount { get; set; }
        public string OrderTypeName { get; set; }
        public string OrderDescription { get; set; }
        public string DiagnosisDescription { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public int? UserDefinedDescriptionId { get; set; }
        public string UserDefinedDescription { get; set; }
        public string Status { get; set; }
        public string FrequencyText { get; set; }

        public Int64 ClaimId { get; set; }
        public Int64 ActivityId { get; set; }

        public bool MulitpleOrderActivites { get; set; }

        public string ActivityCode { get; set; }
        public string SpecimenTypeStr { get; set; }
        public int RoomPrice { get; set; }
        public string TabId { get; set; }
    }
    [NotMapped]
    public class DrugReactionCustomModel
    {
        public bool PatientIsAllergicToDrug { get; set; }
        public bool PatientCurrentMedicationAllergy { get; set; }
        public bool CurrentMedication { get; set; }
        public string ReactionType { get; set; }
        public string WarningText { get; set; }
        public string DrugAllergyName { get; set; }
        public string AllergyFromType { get; set; }
        public string CurrentMedicationMessage { get; set; }
    }

    public class OpenOrdersData
    {
        public List<OpenOrderCustomModel> MostRecentOrder { get; set; }
        public List<OpenOrderCustomModel> PreviousOrders { get; set; }
    }
}
