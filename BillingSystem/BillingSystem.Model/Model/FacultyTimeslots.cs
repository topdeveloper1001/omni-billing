// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyTimeslots.cs" company="Spadez">
//   OmniHealthCare 
// </copyright>
// <summary>
//   Faculty Timeslots Model class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///  Faculty Timeslots Model class
    /// </summary>
    public class FacultyTimeslots
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        [Key]
        public int ID { get; set; }

        /// <summary>
        ///     Gets or sets the type of the faculty.
        /// </summary>
        /// <value>
        ///     The type of the faculty.
        /// </value>
        public string FacultyType { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        ///     Gets or sets the week day.
        /// </summary>
        /// <value>
        ///     The week day.
        /// </value>
        public string WeekDay { get; set; }

        /// <summary>
        /// Gets or sets the available date from.
        /// </summary>
        public DateTime? AvailableDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the available date till.
        /// </summary>
        public DateTime? AvailableDateTill { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        public int FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int CorporateId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is available.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is available; otherwise, <c>false</c>.
        /// </value>
        public string SlotAvailability { get; set; }

        /// <summary>
        /// Gets or sets the color of the slot.
        /// </summary>
        /// <value>
        /// The color of the slot.
        /// </value>
        public string SlotColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the slot text.
        /// </summary>
        /// <value>
        /// The color of the slot text.
        /// </value>
        public string SlotTextColor { get; set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is recurring.
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// Gets or sets the type of the rec_.
        /// </summary>
        /// <value>
        /// The type of the rec_.
        /// </value>
        public string RecType { get; set; }

        /// <summary>
        /// Gets or sets the rec_ pattern.
        /// </summary>
        /// <value>
        /// The rec_ pattern.
        /// </value>
        public string RecPattern { get; set; }

        /// <summary>
        /// Gets or sets the record eventlength.
        /// </summary>
        /// <value>
        /// The record eventlength.
        /// </value>
        public long RecEventlength { get; set; }

        /// <summary>
        /// Gets or sets the record event p identifier.
        /// </summary>
        /// <value>
        /// The record event p identifier.
        /// </value>
        public int RecEventPId { get; set; }

        /// <summary>
        /// Gets or sets the recurring date from.
        /// </summary>
        public DateTime? RecurringDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the recurring date till.
        /// </summary>
        public DateTime? RecurringDateTill { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        #endregion
    }
}