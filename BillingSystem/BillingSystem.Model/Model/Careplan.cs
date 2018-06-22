// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Careplan.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The careplan.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The careplan.
    /// </summary>
    public class Careplan
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int CorporateId { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the diagnosis associated.
        /// </summary>
        public string DiagnosisAssociated { get; set; }

        /// <summary>
        /// Gets or sets the encounter patient type.
        /// </summary>
        public string EncounterPatientType { get; set; }

        /// <summary>
        /// Gets or sets the ext value 1.
        /// </summary>
        public string ExtValue1 { get; set; }

        /// <summary>
        /// Gets or sets the ext value 2.
        /// </summary>
        public string ExtValue2 { get; set; }

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        public int FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public int ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the plan description.
        /// </summary>
        public string PlanDescription { get; set; }

        /// <summary>
        /// Gets or sets the plan length.
        /// </summary>
        public string PlanLength { get; set; }

        /// <summary>
        /// Gets or sets the plan length type.
        /// </summary>
        public string PlanLengthType { get; set; }

        /// <summary>
        /// Gets or sets the plan number.
        /// </summary>
        public string PlanNumber { get; set; }

        public string Name { get; set; }

        #endregion
    }
}