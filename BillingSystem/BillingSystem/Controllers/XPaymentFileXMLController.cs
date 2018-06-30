// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XPaymentFileXMLController.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The x payment file xml controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// The x payment file xml controller.
    /// </summary>
    public class XPaymentFileXMLController : BaseController
    {
        #region Public Methods and Operators

        /// <summary>
        /// Get the details of the current XPaymentFileXML in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetXPaymentFileXML(int id)
        {
            using (var bal = new XPaymentFileXMLService())
            {
                // Call the AddXPaymentFileXML Method to Add / Update current XPaymentFileXML
                XPaymentFileXML currentXPaymentFileXml = bal.GetXPaymentFileXMLByID(id);

                // Pass the ActionResult with the current XPaymentFileXMLViewModel object as model to PartialView XPaymentFileXMLAddEdit
                return this.PartialView(PartialViews.XPaymentFileXMLAddEdit, currentXPaymentFileXml);
            }
        }

        /// <summary>
        /// Reset the XPaymentFileXML View Model and pass it to XPaymentFileXMLAddEdit Partial View.
        /// </summary>
        /// <param name="shared">
        /// pass the input parameters such as ID
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ResetXPaymentFileXMLForm()
        {
            // Intialize the new object of XPaymentFileXML ViewModel
            var xPaymentFileXmlViewModel = new XPaymentFileXML();

            // Pass the View Model as XPaymentFileXMLViewModel to PartialView XPaymentFileXMLAddEdit just to update the AddEdit partial view.
            return this.PartialView(PartialViews.XPaymentFileXMLAddEdit, xPaymentFileXmlViewModel);
        }

        /// <summary>
        /// Get the details of the XPaymentFileXML View in the Model XPaymentFileXML such as XPaymentFileXMLList, list of
        ///     countries etc.
        /// </summary>
        /// <param name="claimId">
        /// The claim Id.
        /// </param>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model XPaymentFileXML to be passed to View
        ///     XPaymentFileXML
        /// </returns>
        public ActionResult XPaymentFileXMLMain(int claimId)
        {
            // Initialize the XPaymentFileXML BAL object
            var xPaymentFileXmlBal = new XPaymentFileXMLService();

            // Get the Entity list
            List<XPaymentFileXMLCustomModel> xPaymentFileXmlList =
                xPaymentFileXmlBal.GetXPaymentFileXML(claimId).ToList();

            // Intialize the View Model i.e. XPaymentFileXMLView which is binded to Main View Index.cshtml under XPaymentFileXML
            var xPaymentFileXmlView = new XPaymentFileXMLView { XPaymentFileXMLList = xPaymentFileXmlList, };

            // Pass the View Model in ActionResult to View XPaymentFileXML
            return View(xPaymentFileXmlView);
        }

        #endregion
    }
}