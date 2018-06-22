// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CareplanCustomModel.cs" company="Spadez">
//   OmniHEalthcare
// </copyright>
// <summary>
//   The careplan custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The careplan custom model.
    /// </summary>
    [NotMapped]
    public class PatientEvaluationSetCustomModel : PatientEvaluationSet
    {

        public string EncounterNumber { get; set; }
        public DateTime? ENMStartdate { get; set; }
        public string PhysicianName { get; set; }

        public string DocumentName { get; set; }
        public string CompletedBy { get; set; }
    }
}