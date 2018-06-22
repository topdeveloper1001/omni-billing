// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarePlanTaskCustomModel.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The care plan task custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The care plan task custom model.
    /// </summary>
    [NotMapped]
    public class PatientCarePlanCustomModel : PatientCarePlan
    {
        public string CarePlanName { get; set; }

        public string CarePlanNumber { get; set; }

        public string CarePlanTaskName { get; set; }

        public string CarePlanTaskNumber { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Text { get; set; }

        public string Value { get; set; }

        public string PatientCarePrimaryIdList { get; set; }

        public List<PatientCarePlanTaskCustomModel> PatientCarePlanList { get; set; }
    }


    public class PatientCarePlanTaskCustomModel
    {
        public int Id { get; set; }

        public string TaskId { get; set; }

        public string CarePlanId { get; set; }

        public string CareTaskName { get; set; }

        public string CareTaskNumber { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }

}