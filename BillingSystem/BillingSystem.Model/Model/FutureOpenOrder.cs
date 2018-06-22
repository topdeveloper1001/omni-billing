// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FutureOpenOrder.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The future open order.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Model.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The future open order.
    /// </summary>
    public class FutureOpenOrder
    {
        /// <summary>
        /// Gets or sets the future open order id.
        /// </summary>
        [Key]
        public int FutureOpenOrderID { get; set; }

        /// <summary>
        /// Gets or sets the open order prescribed date.
        /// </summary>
        public DateTime? OpenOrderPrescribedDate { get; set; }

        /// <summary>
        /// Gets or sets the physician id.
        /// </summary>
        public int? PhysicianID { get; set; }

        /// <summary>
        /// Gets or sets the patient id.
        /// </summary>
        public int? PatientID { get; set; }

        /// <summary>
        /// Gets or sets the encounter id.
        /// </summary>
        public int? EncounterID { get; set; }

        /// <summary>
        /// Gets or sets the diagnosis code.
        /// </summary>
        public string DiagnosisCode { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the category id.
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub category id.
        /// </summary>
        public int? SubCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the order type.
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// Gets or sets the order code.
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the frequency code.
        /// </summary>
        public string FrequencyCode { get; set; }

        /// <summary>
        /// Gets or sets the period days.
        /// </summary>
        public string PeriodDays { get; set; }

        /// <summary>
        /// Gets or sets the order notes.
        /// </summary>
        public string OrderNotes { get; set; }

        /// <summary>
        /// Gets or sets the order status.
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// Gets or sets the is activity schecduled.
        /// </summary>
        public bool? IsActivitySchecduled { get; set; }

        /// <summary>
        /// Gets or sets the activity schecduled on.
        /// </summary>
        public DateTime? ActivitySchecduledOn { get; set; }

        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the item strength.
        /// </summary>
        public string ItemStrength { get; set; }

        /// <summary>
        /// Gets or sets the item dosage.
        /// </summary>
        public string ItemDosage { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the is deleted.
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the deleted by.
        /// </summary>
        public int? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the deleted date.
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int? CorporateID { get; set; }

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        public int? FacilityID { get; set; }
    }
}