// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlanner.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace BillingSystem.Model
{
    /// <summary>
    /// The holiday planner.
    /// </summary>
    public class HolidayPlanner
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the corporate id.
        /// </summary>
        public int? CorporateId { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the facility id.
        /// </summary>
        public int? FacilityId { get; set; }

        /// <summary>
        ///     Gets or sets the holiday planner id.
        /// </summary>
        public int HolidayPlannerId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///     Gets or sets the item id.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        ///     Gets or sets the itemtype id.
        /// </summary>
        public string ItemtypeId { get; set; }

        /// <summary>
        ///     Gets or sets the year.
        /// </summary>
        public int? Year { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        #endregion
    }




}