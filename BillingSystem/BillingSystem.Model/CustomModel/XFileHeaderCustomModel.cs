// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XFileHeaderCustomModel.cs" company="SPadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The x file header custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The x file header custom model.
    /// </summary>
    [NotMapped]
    public class XFileHeaderCustomModel : XFileHeader
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether status bit.
        /// </summary>
        public bool StatusBit { get; set; }

        /// <summary>
        /// Gets or sets the status str.
        /// </summary>
        public string StatusStr { get; set; }

        #endregion
    }
}