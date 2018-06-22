// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DenialCodeCustomModel.cs" company="Spadez">
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
    public class DenialCodeCustomModel : Denial
    {
        public string DenialTypeStr { get; set; }

        public string DenialStatusStr { get; set; }

      
    }
}