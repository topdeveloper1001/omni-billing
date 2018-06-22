// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrubHeaderCustomModel.cs" company="Spadez">
//   Omni Healthcare
// </copyright>
// <summary>
//   The scrub header custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The scrub header custom model.
    /// </summary>
    [NotMapped]
    public class ScrubHeaderCustomModel : ScrubHeader
    {
        /// <summary>
        /// Gets or sets the assigned by user.
        /// </summary>
        public string AssignedByUser { get; set; }

        /// <summary>
        /// Gets or sets the assigned to user.
        /// </summary>
        public string AssignedToUser { get; set; }

        /// <summary>
        /// Gets or sets the executed by user.
        /// </summary>
        public string ExecutedByUser { get; set; }

        /// <summary>
        /// Gets or sets the patient name.
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// Gets or sets the encounter patient type.
        /// </summary>
        public string EncounterPatientType { get; set; }

        /// <summary>
        /// Gets or sets the encounter number.
        /// </summary>
        public string EncounterNumber { get; set; }

        /// <summary>
        /// Gets or sets the bill header status.
        /// </summary>
        public string BillHeaderStatus { get; set; }

        /// <summary>
        /// Gets or sets the bill penality.
        /// </summary>
        public string BillPenality { get; set; }

        /// <summary>
        /// Gets or sets the bill days left to e claim.
        /// </summary>
        public int? BillDaysLeftToEClaim { get; set; }

        /// <summary>
        /// Gets or sets the section.
        /// </summary>
        public int? Section { get; set; }

        /// <summary>
        /// Gets or sets the rule code.
        /// </summary>
        /// <value>
        /// The rule code.
        /// </value>
        public string RuleCode { get; set; }

        /// <summary>
        /// Gets or sets the rule description.
        /// </summary>
        /// <value>
        /// The rule description.
        /// </value>
        public string RuleDescription { get; set; }

        /// <summary>
        /// Gets or sets the bill number.
        /// </summary>
        /// <value>
        /// The bill number.
        /// </value>
        public string BillNumber { get; set; }

        public DateTime? EncounterEndTime { get; set; }
    }
}
