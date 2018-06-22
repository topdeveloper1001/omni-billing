using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace BillingSystem.Model.CustomModel
{
    /// <summary>
    /// The scrub header custom model.
    /// </summary>
    [NotMapped]
    public  class PhysicianDepartmentUtilizationCustomModel
    {
        /// <summary>
        /// Gets or sets the assigned by user.
        /// </summary>
        public DateTime? ScheduledDate { get; set; }
       
        /// <summary>
        /// Gets or sets the patient name.
        /// </summary>
        public string Name { get; set; }

       
        /// <summary>
        /// Total of Appointment Slots Available
        /// </summary>
        public decimal TSTotal { get; set; }

        /// <summary>
        /// Gets or sets Total of Appointment Slots Confirmed
        /// </summary>
        public decimal TSConfirmed { get; set; }

        /// <summary>
        /// Gets or sets Total of Appointment Slots Done
        /// </summary>
        public decimal TSDone { get; set; }



        /// <summary>
        /// Gets or sets Total of Appointment Slots Cancelled/No Shows
        /// </summary>
        /// <value>
        /// The ts cancelled.
        /// </value>
        public decimal TSCancelled { get; set; }

        /// <summary>
        /// Gets or sets Overall Confirmed Utilization
        /// </summary>
        /// <value>
        /// The pre confirmed.
        /// </value>
        public decimal PerConfirmed { get; set; }


        /// <summary>
        /// Gets or sets Overall Done Utilization
        /// </summary>
        /// <value>
        /// The pre done.
        /// </value>
        public decimal PerDone { get; set; }
        /// <summary>
        /// Gets or sets Overall Cancelled Utilization
        /// </summary>
        /// <value>
        /// The pre cancelled.
        /// </value>
        public decimal PerCancelled { get; set; }

      
        
    }
}
