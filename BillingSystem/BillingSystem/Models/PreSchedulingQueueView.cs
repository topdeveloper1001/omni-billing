// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreSchedulingQueueView.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <Screen owner>
// Shashank (Created On : Feb 04 2016)
// </Screen owner>
// <summary>
//   The patient scheduler portal controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Models
{
    using System.Collections.Generic;

    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The pre scheduling queue view.
    /// </summary>
    public class PreSchedulingQueueView
    {
        /// <summary>
        /// Gets or sets the pre scheduling list.
        /// </summary>
        /// <value>
        /// The pre scheduling list.
        /// </value>
        public List<SchedulingCustomModel> PreSchedulingList { get; set; }
    }
}