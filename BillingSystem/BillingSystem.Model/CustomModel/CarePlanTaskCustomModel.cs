// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarePlanTaskCustomModel.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The care plan task custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The care plan task custom model.
    /// </summary>
    [NotMapped]
    public class CarePlanTaskCustomModel : CarePlanTask
    {
        public string ResponsibleUser { get; set; }

        public string Activity { get; set; }

        public string Occurrence { get; set; }

        public string RecurranceTimeIntervalType { get; set; }

        public string CarePlan { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}