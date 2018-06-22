// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindClaimsModel.cs" company="Spadez">
//   OmniHealtchare
// </copyright>
// <summary>
//   The find claims model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Models
{
    using System;
    using System.Collections.Generic;

    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The find claims model.
    /// </summary>
    public class FindClaimsModel
    {
        /// <summary>
        /// Gets or sets the claims list.
        /// </summary>
        /// <value>
        /// The claims list.
        /// </value>
        public List<BillHeaderCustomModel> ClaimsList { get; set; }

        /// <summary>
        /// Gets or sets the month start date.
        /// </summary>
        /// <value>
        /// The month start date.
        /// </value>
        public DateTime MonthStartDate { get; set; }

        /// <summary>
        /// Gets or sets the month end date.
        /// </summary>
        /// <value>
        /// The month end date.
        /// </value>
        public DateTime MonthEndDate { get; set; }
    }
}