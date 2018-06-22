// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FutureOpenOrderCustomModel.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The future open order custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace BillingSystem.Model.CustomModel
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    using BillingSystem.Model.Model;

    /// <summary>
    /// The future open order custom model.
    /// </summary>
    [NotMapped]
    public class FutureOpenOrderCustomModel : FutureOpenOrder
    {
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
        public Nullable<DateTime> BirthDate { get; set; }
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
    }
}