// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TPXMLParsedDataCustomModel.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The tpxml parsed data custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The tpxml parsed data custom model.
    /// </summary>
    [NotMapped]
    public class TPXMLParsedDataCustomModel : TPXMLParsedData
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the bill number.
        /// </summary>
        public string BillNumber { get; set; }

        /// <summary>
        /// Gets or sets the corporate name.
        /// </summary>
        public string CorporateName { get; set; }

        /// <summary>
        /// Gets or sets the data status.
        /// </summary>
        public string DataStatus { get; set; }

        /// <summary>
        /// Gets or sets the diagnosis type.
        /// </summary>
        public string DiagnosisType { get; set; }

        /// <summary>
        /// Gets or sets the encounter end type.
        /// </summary>
        public string EncounterEndType { get; set; }

        /// <summary>
        /// Gets or sets the encounter number.
        /// </summary>
        public string EncounterNumber { get; set; }

        /// <summary>
        /// Gets or sets the encounter start type.
        /// </summary>
        public string EncounterStartType { get; set; }

        /// <summary>
        /// Gets or sets the encounter type.
        /// </summary>
        public string EncounterType { get; set; }

        /// <summary>
        /// Gets or sets the facility name.
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the order clinical id str.
        /// </summary>
        public string OrderClinicalIdStr { get; set; }

        /// <summary>
        /// Gets or sets the order executing clinical id str.
        /// </summary>
        public string OrderExecutingClinicalIdStr { get; set; }

        /// <summary>
        /// Gets or sets the patient name.
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// Gets or sets the type of the activity.
        /// </summary>
        /// <value>
        /// The type of the activity.
        /// </value>
        public string ActivityType { get; set; }

        /// <summary>
        /// Gets or sets the insurance company.
        /// </summary>
        /// <value>
        /// The insurance company.
        /// </value>
        public string InsuranceCompany { get; set; }
        #endregion
    }
}