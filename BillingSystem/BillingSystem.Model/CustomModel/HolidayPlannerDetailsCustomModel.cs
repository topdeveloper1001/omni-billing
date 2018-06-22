// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerDetailsCustomModel.cs" company="SPadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   Defines the HolidayPlannerDetailsCustomModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// </summary>
    [NotMapped]
    public class HolidayPlannerDetailsCustomModel : HolidayPlannerDetails
    {
        public string end_date { get; set; }

        /// <summary>
        ///     Gets or sets the start_date.
        /// </summary>
        public string start_date { get; set; }

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>
        ///     The text.
        /// </value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public string color { get; set; }


        public string rec_type { get; set; }

        /// <summary>
        /// Gets or sets the rec_pattern.
        /// </summary>
        /// <value>
        /// The rec_pattern.
        /// </value>
        public string rec_pattern { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is _timed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if _timed; otherwise, <c>false</c>.
        /// </value>
        public bool _timed { get; set; }

        /// <summary>
        /// Gets or sets the event_length.
        /// </summary>
        /// <value>
        /// The event_length.
        /// </value>
        public long event_length { get; set; }

        /// <summary>
        /// Gets or sets the event_pid.
        /// </summary>
        /// <value>
        /// The event_pid.
        /// </value>
        public int event_pid { get; set; }

        public bool full_day { get; set; }
    }
}
