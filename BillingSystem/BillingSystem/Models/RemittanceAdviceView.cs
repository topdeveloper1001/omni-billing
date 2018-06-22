// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemittanceAdviceView.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The remittance advice view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Models
{
    using System.Collections.Generic;

    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The remittance advice view.
    /// </summary>
    public class RemittanceAdviceView
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the files uploaded.
        /// </summary>
        public List<XFileHeaderCustomModel> FilesUploaded { get; set; }

        /// <summary>
        /// Gets or sets the x advice xml data.
        /// </summary>
        public List<XAdviceXMLParsedDataCustomModel> XAdviceXMLData { get; set; }

        #endregion
    }
}