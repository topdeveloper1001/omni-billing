// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentCustomModel.cs" company="SPadez">
//   Omni Healthcare
// </copyright>
// <summary>
//   The payment custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    using BillingSystem.Model.Model;
    using System.Collections.Generic;

    /// <summary>
    /// The payment custom model.
    /// </summary>
    [NotMapped]
    public class PaymentCustomModel : Payment
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the activity type code.
        /// </summary>
        public string ActivityTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the activity type name.
        /// </summary>
        public string ActivityTypeName { get; set; }

        /// <summary>
        /// Gets or sets the bill header description.
        /// </summary>
        public string BillHeaderDescription { get; set; }

        /// <summary>
        /// Gets or sets the current payment type detail.
        /// </summary>
        public PaymentTypeDetail CurrentPaymentTypeDetail { get; set; }

        /// <summary>
        /// Gets or sets the denial code description.
        /// </summary>
        public string DenialCodeDescription { get; set; }

        /// <summary>
        /// Gets or sets the encounter number.
        /// </summary>
        public string EncounterNumber { get; set; }

        /// <summary>
        /// Gets or sets the ptd card holder name.
        /// </summary>
        public string PTDCardHolderName { get; set; }

        /// <summary>
        /// Gets or sets the ptd card number.
        /// </summary>
        public string PTDCardNumber { get; set; }

        /// <summary>
        /// Gets or sets the ptd expiry month.
        /// </summary>
        public string PTDExpiryMonth { get; set; }

        /// <summary>
        /// Gets or sets the ptd expiry year.
        /// </summary>
        public string PTDExpiryYear { get; set; }

        /// <summary>
        /// Gets or sets the ptd payment type.
        /// </summary>
        public string PTDPaymentType { get; set; }

        /// <summary>
        /// Gets or sets the ptd security number.
        /// </summary>
        public string PTDSecurityNumber { get; set; }

        /// <summary>
        /// Gets or sets the pay by patient name.
        /// </summary>
        public string PayByPatientName { get; set; }

        /// <summary>
        /// Gets or sets the pay for patient name.
        /// </summary>
        public string PayForPatientName { get; set; }

        /// <summary>
        /// Gets or sets the payed date.
        /// </summary>
        public string PayedDate { get; set; }

        /// <summary>
        /// Gets or sets the payment type name.
        /// </summary>
        public string PaymentTypeName { get; set; }

        /// <summary>
        /// Gets or sets the pay status string.
        /// </summary>
        /// <value>
        /// The pay status string.
        /// </value>
        public string PayStatusStr { get; set; }

        #endregion
    }



    [NotMapped]
    public class PaymentViewDetail
    {
        public IEnumerable<PaymentCustomModel> PaymentsList { get; set; }
        public PaymentDetailsCustomModel PaymentDetails { get; set; }
        public List<DropdownListData> PatientsList { get; set; }
        public bool Success { get; set; } = false;
    }
}