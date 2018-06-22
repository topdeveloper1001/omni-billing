// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientSchedulerPortalView.cs" company="SPadez">
//   Omnihealthcare
// </copyright>
// <Screen owner>
// Shashank Created On 2nd Feb 2016
// </Screen owner>
// <summary>
//   The patient scheduler portal view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Models
{
    using System.Collections.Generic;

    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The patient scheduler portal view.
    /// </summary>
    public class PatientSchedulerPortalView
    {
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        public int PatientId { get; set; }

        /// <summary>
        /// Gets or sets the facility identifier.
        /// </summary>
        /// <value>
        /// The facility identifier.
        /// </value>
        public int FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the corporate identifier.
        /// </summary>
        /// <value>
        /// The corporate identifier.
        /// </value>
        public int CorporateId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is previous encounter.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is previous encounter; otherwise, <c>false</c>.
        /// </value>
        public bool IsPreviousEncounter { get; set; }

        /// <summary>
        /// Gets or sets the name of the previous encounter physician.
        /// </summary>
        /// <value>
        /// The name of the previous encounter physician.
        /// </value>
        public string PreviousEncounterPhysicianName { get; set; }

        /// <summary>
        /// Gets or sets the previous encounter physician identifier.
        /// </summary>
        /// <value>
        /// The previous encounter physician identifier.
        /// </value>
        public string PreviousEncounterPhysicianId { get; set; }
    }
}