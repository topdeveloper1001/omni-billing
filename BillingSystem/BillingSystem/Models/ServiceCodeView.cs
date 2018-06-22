// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCodeView.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <Screen Owner>
// Nitin Yadav Modified on : feb 09 2014
// </Screen Owner>
// <summary>
//   The service code view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------




namespace BillingSystem.Models
{
    using System.Collections.Generic;

    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The service code view.
    /// </summary>
    public class ServiceCodeView
    {
        /// <summary>
        /// Gets or sets the service code list.
        /// </summary>
        public List<ServiceCode> ServiceCodeList { get; set; }

        /// <summary>
        /// Gets or sets the current service code.
        /// </summary>
        public ServiceCode CurrentServiceCode { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public int UserId { get; set; }
    }

    /// <summary>
    /// The service code view model.
    /// </summary>
    public class ServiceCodeViewModel
    {
        /// <summary>
        /// Gets or sets the service code list.
        /// </summary>
        public List<ServiceCodeCustomModel> ServiceCodeList { get; set; }

        /// <summary>
        /// Gets or sets the current service code.
        /// </summary>
        public ServiceCode CurrentServiceCode { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public int UserId { get; set; }

        
    }
}