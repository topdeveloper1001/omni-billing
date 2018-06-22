// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FutureOrderActivity.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The future order activity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------





namespace BillingSystem.Model.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The future order activity.
    /// </summary>
    public class FutureOrderActivity
    {
        /// <summary>
        /// Gets or sets the future order activity id.
        /// </summary>
        [Key]
        public int FutureOrderActivityID { get; set; }

        /// <summary>
        /// Gets or sets the order type.
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// Gets or sets the order code.
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// Gets or sets the order category id.
        /// </summary>
        public int OrderCategoryID { get; set; }

        /// <summary>
        /// Gets or sets the order sub category id.
        /// </summary>
        public int OrderSubCategoryID { get; set; }

        /// <summary>
        /// Gets or sets the order activity status.
        /// </summary>
        public int OrderActivityStatus { get; set; }

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int CorporateID { get; set; }

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        public int FacilityID { get; set; }

        /// <summary>
        /// Gets or sets the patient id.
        /// </summary>
        public int PatientID { get; set; }

        /// <summary>
        /// Gets or sets the encounter id.
        /// </summary>
        public int EncounterID { get; set; }

        /// <summary>
        /// Gets or sets the medical record number.
        /// </summary>
        public string MedicalRecordNumber { get; set; }

        /// <summary>
        /// Gets or sets the order id.
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// Gets or sets the order by.
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// Gets or sets the order activity quantity.
        /// </summary>
        public decimal OrderActivityQuantity { get; set; }

        /// <summary>
        /// Gets or sets the order schedule date.
        /// </summary>
        public DateTime OrderScheduleDate { get; set; }

        /// <summary>
        /// Gets or sets the planned by.
        /// </summary>
        public int PlannedBy { get; set; }

        /// <summary>
        /// Gets or sets the planned date.
        /// </summary>
        public DateTime PlannedDate { get; set; }

        /// <summary>
        /// Gets or sets the planned for.
        /// </summary>
        public int PlannedFor { get; set; }

        /// <summary>
        /// Gets or sets the executed by.
        /// </summary>
        public int ExecutedBy { get; set; }

        /// <summary>
        /// Gets or sets the executed date.
        /// </summary>
        public DateTime ExecutedDate { get; set; }

        /// <summary>
        /// Gets or sets the executed quantity.
        /// </summary>
        public decimal ExecutedQuantity { get; set; }

        /// <summary>
        /// Gets or sets the result value min.
        /// </summary>
        public decimal ResultValueMin { get; set; }

        /// <summary>
        /// Gets or sets the result value max.
        /// </summary>
        public decimal ResultValueMax { get; set; }

        /// <summary>
        /// Gets or sets the result uom.
        /// </summary>
        public int ResultUOM { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public int ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}