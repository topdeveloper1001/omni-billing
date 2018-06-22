// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BillDetailsView.cs" company="Spadez">
//   Omniheathcare
// </copyright>
// <summary>
//   The bill details view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Models
{
    using System.Collections.Generic;

    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The bill details view.
    /// </summary>
    public class BillDetailsView
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the bill activity list.
        /// </summary>
        public List<BillDetailCustomModel> BillActivityList { get; set; }

        /// <summary>
        /// Gets or sets the bill header.
        /// </summary>
        public BillHeaderCustomModel BillHeader { get; set; }

        /// <summary>
        /// Gets or sets the bill header list.
        /// </summary>
        public List<BillHeaderCustomModel> BillHeaderList { get; set; }

        /// <summary>
        /// Gets or sets the encounter id.
        /// </summary>
        public int EncounterId { get; set; }

        /// <summary>
        /// Gets or sets the patient id.
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// Gets or sets the patient info.
        /// </summary>
        public PatientInfoCustomModel PatientInfo { get; set; }

        /// <summary>
        /// Gets or sets the query string id.
        /// </summary>
        public int? QueryStringId { get; set; }

        /// <summary>
        /// Gets or sets the query string type id.
        /// </summary>
        public int? QueryStringTypeId { get; set; }

        /// <summary>
        /// Gets or sets the payer wise bill header list.
        /// </summary>
        /// <value>
        /// The payer wise bill header list.
        /// </value>
        public List<BillHeaderCustomModel> PayerWiseBillHeaderList { get; set; }



        /// <summary>
        /// Gets or sets the in patient ListView.
        /// </summary>
        /// <value>
        /// The in patient ListView.
        /// </value>
        public List<BillHeaderCustomModel> InPatientListView { get; set; }

        /// <summary>
        /// Gets or sets the out patient ListView.
        /// </summary>
        /// <value>
        /// The out patient ListView.
        /// </value>
        public List<BillHeaderCustomModel> OutPatientListView { get; set; }

        /// <summary>
        /// Gets or sets the er patient ListView.
        /// </summary>
        /// <value>
        /// The er patient ListView.
        /// </value>
        public List<BillHeaderCustomModel> ErPatientListView { get; set; }


        #endregion
    }
}