// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CareplanCustomModel.cs" company="Spadez">
//   OmniHEalthcare
// </copyright>
// <summary>
//   The careplan custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The careplan custom model.
    /// </summary>
    [NotMapped]
    public class CareplanCustomModel : Careplan
    {
        public string EncounterType { get; set; }
    }
}