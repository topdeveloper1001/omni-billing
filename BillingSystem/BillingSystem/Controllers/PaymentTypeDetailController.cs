// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerController.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BillingSystem.Model.Model;

namespace BillingSystem.Controllers
{
    using System.Web.Mvc;

    using Bal.BusinessAccess;
    using Common;
    using Models;

    /// <summary>
    /// PaymentTypeDetail controller.
    /// </summary>
    public class PaymentTypeDetailController : Controller
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the PaymentTypeDetail list
        /// </summary>
        /// <returns>action result with the partial view containing the PaymentTypeDetail list object</returns>
        [HttpPost]
        public ActionResult BindPaymentTypeDetailList()
        {
            // Initialize the PaymentTypeDetail BAL object
            using (var paymentTypeDetailBal = new PaymentTypeDetailBal())
            {
                // Get the facilities list
                var paymentTypeDetailList = paymentTypeDetailBal.GetPaymentTypeDetail();

                // Pass the ActionResult with List of PaymentTypeDetailViewModel object to Partial View PaymentTypeDetailList
                return PartialView(PartialViews.PaymentTypeDetailList, paymentTypeDetailList);
            }
        }

        /// <summary>
        /// Delete the current PaymentTypeDetail based on the PaymentTypeDetail ID passed in the PaymentTypeDetailModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeletePaymentTypeDetail(int id)
        {
            using (var bal = new PaymentTypeDetailBal())
            {
                // Get PaymentTypeDetail model object by current PaymentTypeDetail ID
                var currentPaymentTypeDetail = bal.GetPaymentTypeDetailById(id);
                //var userId = Helpers.GetLoggedInUserId();

                // Check If PaymentTypeDetail model is not null
                if (currentPaymentTypeDetail != null)
                {
                    //currentPaymentTypeDetail.IsActive = false;

                    // currentPaymentTypeDetail.ModifiedBy = userId;
                    // currentPaymentTypeDetail.ModifiedDate = DateTime.Now;

                    // Update Operation of current PaymentTypeDetail
                    int result = bal.SavePaymentTypeDetail(currentPaymentTypeDetail);

                    // return deleted ID of current PaymentTypeDetail as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Get the details of the current PaymentTypeDetail in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetPaymentTypeDetail(int id)
        {
            using (var bal = new PaymentTypeDetailBal())
            {
                // Call the AddPaymentTypeDetail Method to Add / Update current PaymentTypeDetail
                PaymentTypeDetail currentPaymentTypeDetail = bal.GetPaymentTypeDetailById(id);

                // Pass the ActionResult with the current PaymentTypeDetailViewModel object as model to PartialView PaymentTypeDetailAddEdit
                return PartialView(PartialViews.PaymentTypeDetailAddEdit, currentPaymentTypeDetail);
            }
        }

        /// <summary>
        /// Get the details of the PaymentTypeDetail View in the Model PaymentTypeDetail such as PaymentTypeDetailList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model PaymentTypeDetail to be passed to View
        ///     PaymentTypeDetail
        /// </returns>
        public ActionResult PaymentTypeDetailMain()
        {
            // Initialize the PaymentTypeDetail BAL object
            var paymentTypeDetailBal = new PaymentTypeDetailBal();

            // Get the Entity list
            var paymentTypeDetailList = paymentTypeDetailBal.GetPaymentTypeDetail();

            // Intialize the View Model i.e. PaymentTypeDetailView which is binded to Main View Index.cshtml under PaymentTypeDetail
            var paymentTypeDetailView = new PaymentTypeDetailView
                                         {
                                             PaymentTypeDetailList = paymentTypeDetailList, 
                                             CurrentPaymentTypeDetail = new PaymentTypeDetail()
                                         };

            // Pass the View Model in ActionResult to View PaymentTypeDetail
            return View(paymentTypeDetailView);
        }

        /// <summary>
        /// Reset the PaymentTypeDetail View Model and pass it to PaymentTypeDetailAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetPaymentTypeDetailForm()
        {
            // Intialize the new object of PaymentTypeDetail ViewModel
            var paymentTypeDetailViewModel = new PaymentTypeDetail();

            // Pass the View Model as PaymentTypeDetailViewModel to PartialView PaymentTypeDetailAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.PaymentTypeDetailAddEdit, paymentTypeDetailViewModel);
        }

        /// <summary>
        /// Add New or Update the PaymentTypeDetail based on if we pass the PaymentTypeDetail ID in the PaymentTypeDetailViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of PaymentTypeDetail row
        /// </returns>
        public ActionResult SavePaymentTypeDetail(PaymentTypeDetail model)
        {
            // Initialize the newId variable 
            int newId = -1;
            //int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new PaymentTypeDetailBal())
                {
                    if (model.Id > 0)
                    {
                        // model.ModifiedBy = userId;
                        // model.ModifiedDate = DateTime.Now;
                    }

                    // Call the AddPaymentTypeDetail Method to Add / Update current PaymentTypeDetail
                    newId = bal.SavePaymentTypeDetail(model);
                }
            }

            return Json(newId);
        }

        #endregion
    }
}
