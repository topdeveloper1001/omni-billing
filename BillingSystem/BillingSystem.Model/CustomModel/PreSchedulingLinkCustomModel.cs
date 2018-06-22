// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreSchedulingLinkCustomModel.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <owner>
// Shashank (Created on : 1st of Feb 2016)
// </owner>
// <summary>
//   The pre scheduling link.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The pre scheduling link custom model.
    /// </summary>
    [NotMapped]
    public class PreSchedulingLinkCustomModel : PreSchedulingLink
    {
        /// <summary>
        /// Gets or sets the name of the facility.
        /// </summary>
        /// <value>
        /// The name of the facility.
        /// </value>
        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the name of the corporate.
        /// </summary>
        /// <value>
        /// The name of the corporate.
        /// </value>
        public string CorporateName { get; set; }
    }
}