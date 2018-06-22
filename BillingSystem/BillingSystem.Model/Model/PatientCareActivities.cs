// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientCareActivities.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The patient care activities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The patient care activities.
    /// </summary>
    public class PatientCareActivities
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the task number.
        /// </summary>
        public string TaskNumber { get; set; }

        /// <summary>
        /// Gets or sets the task name.
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the activity type.
        /// </summary>
        public string ActivityType { get; set; }

        /// <summary>
        /// Gets or sets the user type.
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        public int? FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int? CorporateId { get; set; }

        /// <summary>
        /// Gets or sets the administrative on.
        /// </summary>
        public DateTime? AdministrativeOn { get; set; }

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
        /// Gets or sets the modeified date.
        /// </summary>
        public DateTime? ModeifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the patient id.
        /// </summary>
        public int? PatientId { get; set; }

        /// <summary>
        /// Gets or sets the encounter id.
        /// </summary>
        public int? EncounterId { get; set; }

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
    }
}