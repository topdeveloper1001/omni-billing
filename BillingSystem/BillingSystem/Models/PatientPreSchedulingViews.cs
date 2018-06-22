// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreSchedulingLinkView.cs" company="Spadez">
//   Omniheakthcare
// </copyright>
// <summary>
//   The pre scheduling link view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Models
{
    /// <summary>
    ///     The pre scheduling link view.
    /// </summary>
    public class PatientPreSchedulingView
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the corporate id.
        /// </summary>
        public int CorporateId { get; set; }

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        public int FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the user object.
        /// </summary>
        /// <value>
        /// The user object.
        /// </value>
        public Model.Users UserObj { get; set; }
        #endregion
    }
}