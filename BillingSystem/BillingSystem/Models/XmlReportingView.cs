// -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="XmlReportingView.cs" company="Spadez">
// Omnihealthcare
//     </copyright>
// <summary>
// The xml reporting view. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------  

namespace BillingSystem.Models
{
    using System;

    /// <summary>
    /// The xml reporting view. 
    /// </summary>
    public class XmlReportingView
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the corporate id. 
        /// </summary>
        public int CorporateId { get; set; }

        /// <summary>
        /// Gets or sets the from date. 
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Gets or sets the reporting type. 
        /// </summary>
        public int ReportingType { get; set; }

        /// <summary>
        /// Gets or sets the reporting type action. 
        /// </summary>
        public string ReportingTypeAction { get; set; }

        /// <summary>
        /// Gets or sets the title. 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the to date. 
        /// </summary>
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Gets or sets the user id. 
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the view type. 
        /// </summary>
        public string ViewType { get; set; }

        #endregion Public Properties
    }
}