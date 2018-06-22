// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrubHeaderCustomModel.cs" company="Spadez">
//   Omni Healthcare
// </copyright>
// <summary>
//   The scrub header custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The scrub header custom model.
    /// </summary>
    [NotMapped]
    public class PhysicianActivityCustomModel:FacilityStructure
    {
        /// <summary>
        /// Gets or sets the assigned by user.
        /// </summary>
        public DateTime? OrderCloseDate { get; set; }

        /// <summary>
        /// Gets or sets the assigned to user.
        /// </summary>
        public string PhysicianName { get; set; }

        /// <summary>
        /// Gets or sets the executed by user.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the patient name.
        /// </summary>
        public string PatientName { get; set; }

       
        /// <summary>
        /// Gets or sets the encounter number.
        /// </summary>
        public string CodeDescription { get; set; }

        /// <summary>
        /// Gets or sets the bill header status.
        /// </summary>
        public decimal Qunatity { get; set; }

        /// <summary>
        /// Gets or sets the bill penality.
        /// </summary>
        public decimal TotalGrossCharges { get; set; }

      

        public decimal TotalActivityCost { get; set; }

        public string ActivityCode { get; set; }

       

        
    }
}
