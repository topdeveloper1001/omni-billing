// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreSchedulingLink.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <owner>
// Shashank (Created on : 1st of Feb 2016)
// </owner>
// <summary>
//   The pre scheduling link.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The pre scheduling link.
    /// </summary>
    public class PreSchedulingLink
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        public int? FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int? CorporateId { get; set; }

        /// <summary>
        /// Gets or sets the public link url.
        /// </summary>
        public string PublicLinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the short url.
        /// </summary>
        public string ShortURL { get; set; }

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
        /// Gets or sets the modifiedby.
        /// </summary>
        public int? Modifiedby { get; set; }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

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