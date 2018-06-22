// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyRooster.cs" company="">
//   
// </copyright>
// <summary>
//   The faculty rooster.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The faculty rooster.
    /// </summary>
    public class FacultyRooster
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the faculty id.
        /// </summary>
        public int? FacultyId { get; set; }

        /// <summary>
        /// Gets or sets the faculty type.
        /// </summary>
        public int? FacultyType { get; set; }

        /// <summary>
        /// Gets or sets the dept id.
        /// </summary>
        public int? DeptId { get; set; }

        /// <summary>
        /// Gets or sets the availability type.
        /// </summary>
        public string AvailabilityType { get; set; }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the working day.
        /// </summary>
        public int? WorkingDay { get; set; }

        /// <summary>
        /// Gets or sets the week number.
        /// </summary>
        public int? WeekNumber { get; set; }

        /// <summary>
        /// Gets or sets the from date.
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Gets or sets the to date.
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        public int? FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int? CorporateId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the recurreance date to.
        /// </summary>
        public DateTime? RecurreanceDateTo { get; set; }

        /// <summary>
        /// Gets or sets the recurreance date from.
        /// </summary>
        public DateTime? RecurreanceDateFrom { get; set; }

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
    }
}