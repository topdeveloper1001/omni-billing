// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ELMAH_Error.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The elma h_ error.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The elma h_ error.
    /// </summary>
    public class ELMAH_Error
    {
        /// <summary>
        /// Gets or sets the error id.
        /// </summary>
        [Key]
        public Guid ErrorId { get; set; }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the time utc.
        /// </summary>
        public DateTime TimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Gets or sets the all xml.
        /// </summary>
        public string AllXml { get; set; }
    }
}