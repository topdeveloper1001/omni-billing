// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerCustomModel.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The holiday planner custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The holiday planner custom model.
    /// </summary>
    [NotMapped]
    public class HolidayPlannerCustomModel : HolidayPlanner
    {
        public List<HolidayPlannerDetailsCustomModel> TimeSlots { get; set; }
    }
}