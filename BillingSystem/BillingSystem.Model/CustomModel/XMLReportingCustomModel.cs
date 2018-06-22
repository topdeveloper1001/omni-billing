// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XMLReportingCustomModel.cs" company="SpadeZ">
//   Omnihealthcare
// </copyright>
// <summary>
//   The xml reporting custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System;

    /// <summary>
    /// The xml reporting custom model.
    /// </summary>
    public class XMLReportingCustomModel
    {
    }

    /// <summary>
    /// The xml reporting batch report.
    /// </summary>
    public class XmlReportingBatchReport
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the batch number.
        /// </summary>
        public string BatchNumber { get; set; }

        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        public int? Claims { get; set; }

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int? CorporateID { get; set; }

        /// <summary>
        /// Gets or sets the corporate name.
        /// </summary>
        public string CorporateName { get; set; }

        /// <summary>
        /// Gets or sets the date received.
        /// </summary>
        public DateTime? DateReceived { get; set; }

        /// <summary>
        /// Gets or sets the facility name.
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the gross.
        /// </summary>
        public decimal Gross { get; set; }

        /// <summary>
        /// Gets or sets the sender id.
        /// </summary>
        public string SenderID { get; set; }

        #endregion
    }

    public class XmlReportingInitialClaimErrorReport
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the batch number.
        /// </summary>
        public string BatchNumber { get; set; }

        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        public int? Claims { get; set; }

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int? CorporateID { get; set; }

        /// <summary>
        /// Gets or sets the corporate name.
        /// </summary>
        public string CorporateName { get; set; }

        /// <summary>
        /// Gets or sets the date received.
        /// </summary>
        public DateTime? DateReceived { get; set; }

        /// <summary>
        /// Gets or sets the facility name.
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the gross.
        /// </summary>
        public decimal Gross { get; set; }

        /// <summary>
        /// Gets or sets the sender id.
        /// </summary>
        public string SenderID { get; set; }

        /// <summary>
        /// Gets or sets the clinician identifier.
        /// </summary>
        /// <value>
        /// The clinician identifier.
        /// </value>
        public string ClinicianID { get; set; }

        /// <summary>
        /// Gets or sets the claims with error.
        /// </summary>
        /// <value>
        /// The claims with error.
        /// </value>
        public int? ClaimsWithError { get; set; }

        /// <summary>
        /// Gets or sets the gross charges with error.
        /// </summary>
        /// <value>
        /// The gross charges with error.
        /// </value>
        public decimal? GrossChargesWithError { get; set; }

        /// <summary>
        /// Gets or sets the initialize error percentage.
        /// </summary>
        /// <value>
        /// The initialize error percentage.
        /// </value>
        public decimal? InitErrorPercentage { get; set; }

        /// <summary>
        /// Gets or sets the error revenue percentage.
        /// </summary>
        /// <value>
        /// The error revenue percentage.
        /// </value>
        public decimal? ErrorRevenuePercentage { get; set; }
        #endregion
    }
}