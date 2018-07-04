
using BillingSystem.Model.Model;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    public class PaymentTypeDetailController : Controller
    {
        private readonly IPaymentTypeDetailService _service;

        public PaymentTypeDetailController(IPaymentTypeDetailService service)
        {
            _service = service;
        }


        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the PaymentTypeDetail list
        /// </summary>
        /// <returns>action result with the partial view containing the PaymentTypeDetail list object</returns>
        [HttpPost]
        public ActionResult BindPaymentTypeDetailList()
        {
            var paymentTypeDetailList = _service.GetPaymentTypeDetail();
            return PartialView(PartialViews.PaymentTypeDetailList, paymentTypeDetailList);
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
            var currentPaymentTypeDetail = _service.GetPaymentTypeDetailById(id);
            // Check If PaymentTypeDetail model is not null
            if (currentPaymentTypeDetail != null)
            {
                int result = _service.SavePaymentTypeDetail(currentPaymentTypeDetail);
                return Json(result);
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
            var currentPaymentTypeDetail = _service.GetPaymentTypeDetailById(id);
            return PartialView(PartialViews.PaymentTypeDetailAddEdit, currentPaymentTypeDetail);
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
            // Get the Entity list
            var paymentTypeDetailList = _service.GetPaymentTypeDetail();

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
                newId = _service.SavePaymentTypeDetail(model);
            }

            return Json(newId);
        }

        #endregion
    }
}
