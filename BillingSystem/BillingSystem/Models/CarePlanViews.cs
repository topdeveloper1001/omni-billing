// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarePlanViews.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The care plan view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Models
{
    using System.Collections.Generic;

    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The care plan view.
    /// </summary>
    public class CarePlanView
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the care plan list.
        /// </summary>
        public List<CareplanCustomModel> CarePlanList { get; set; }

        /// <summary>
        /// Gets or sets the current care plan.
        /// </summary>
        public Careplan CurrentCarePlan { get; set; }

        #endregion
    }
}