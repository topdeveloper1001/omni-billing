// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientPreSchedulingCustomModel.cs" company="Spadez">
//   Omniheralthcare
// </copyright>
// <summary>
//   The patient pre scheduling custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The patient pre scheduling custom model.
    /// </summary>
    [NotMapped]
    public class PatientPreSchedulingCustomModel : PatientPreScheduling
    {
    }
}