// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientPreScheduling.cs" company="Spadez">
//   Omnihealhcare
// </copyright>
// <summary>
//   The patient pre scheduling.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The patient pre scheduling.
    /// </summary>
    public class PatientPreScheduling
    {
        /// <summary>
        /// Gets or sets the patient pre scheduling id.
        /// </summary>
        [Key]
        public int PatientPreSchedulingId { get; set; }

        /// <summary>
        /// Gets or sets the patient id.
        /// </summary>
        public int? PatientId { get; set; }

        /// <summary>
        /// Gets or sets the encounter id.
        /// </summary>
        public int? EncounterId { get; set; }

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        public int? FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the physician id.
        /// </summary>
        public string PhysicianId { get; set; }

        /// <summary>
        /// Gets or sets the physician speciality.
        /// </summary>
        public string PhysicianSpeciality { get; set; }

        /// <summary>
        /// Gets or sets the type of procedure.
        /// </summary>
        public string TypeOfProcedure { get; set; }

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int? CorporateId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the deleted by.
        /// </summary>
        public int? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the deleted date.
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Gets or sets the ext value 1.
        /// </summary>
        public string ExtValue1 { get; set; }

        /// <summary>
        /// Gets or sets the ext value 2.
        /// </summary>
        public string ExtValue2 { get; set; }

        /// <summary>
        /// Gets or sets the ext value 3.
        /// </summary>
        public string ExtValue3 { get; set; }

        /// <summary>
        /// Gets or sets the ext value 4.
        /// </summary>
        public string ExtValue4 { get; set; }

        /// <summary>
        /// Gets or sets the ext value 5.
        /// </summary>
        public string ExtValue5 { get; set; }

        /// <summary>
        /// Gets or sets the preferred date.
        /// </summary>
        public DateTime? PreferredDate { get; set; }

        /// <summary>
        /// Gets or sets the preferred time slots.
        /// </summary>
        public string PreferredTimeSlots { get; set; }
    }
}