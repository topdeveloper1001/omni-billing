// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarePlanTask.cs" company="Spadez">
//   OmniHealthCare
// </copyright>
// <summary>
//   The care plan task.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The care plan task.
    /// </summary>
    public class CarePlanTask
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the activity type.
        /// </summary>
        public string ActivityType { get; set; }

        /// <summary>
        /// Gets or sets the care plan id.
        /// </summary>
        public int? CarePlanId { get; set; }

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int? CorporateId { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public string EndTime { get; set; }

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
        public int? FacilityId { get; set; }

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
        /// Gets or sets a value indicating whether is recurring.
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the rec t ime interval type.
        /// </summary>
        public string RecTImeIntervalType { get; set; }

        /// <summary>
        /// Gets or sets the rec time interval.
        /// </summary>
        public string RecTimeInterval { get; set; }

        /// <summary>
        /// Gets or sets the recurrance type.
        /// </summary>
        public string RecurranceType { get; set; }

        /// <summary>
        /// Gets or sets the responsible user type.
        /// </summary>
        public int? ResponsibleUserType { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets the task description.
        /// </summary>
        public string TaskDescription { get; set; }

        /// <summary>
        /// Gets or sets the task number.
        /// </summary>
        public string TaskNumber { get; set; }

        public string TaskName { get; set; }

        public int? PatientId { get; set; }

        public int? EncounterId { get; set; }

        #endregion
    }
}