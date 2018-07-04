using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class OrderActivityController : BaseController
    {
        private readonly IOrderActivityService _service;

        public OrderActivityController(IOrderActivityService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get the details of the OrderActivity View in the Model OrderActivity such as OrderActivityList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model OrderActivity to be passed to View OrderActivity
        /// </returns>
        public ActionResult OrderActivityMain()
        {

            //Get the Entity list
            var orderActivityList = _service.GetOrderActivityCustom(Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber);

            //Intialize the View Model i.e. OrderActivityView which is binded to Main View Index.cshtml under OrderActivity
            var orderActivityView = new OrderActivityView
            {
                OrderActivityList = orderActivityList,
                CurrentOrderActivity = new Model.OrderActivity()
            };

            //Pass the View Model in ActionResult to View OrderActivity
            return View(orderActivityView);
        }

        /// <summary>
        /// Bind all the OrderActivity list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the OrderActivity list object
        /// </returns>
        [HttpPost]
        public ActionResult BindOrderActivityList()
        {
            //Get the facilities list
            var orderActivityList = _service.GetOrderActivityCustom(Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber);

            //Pass the ActionResult with List of OrderActivityViewModel object to Partial View OrderActivityList
            return PartialView(PartialViews.OrderActivityList, orderActivityList);
        }


        /// <summary>
        /// Add New or Update the OrderActivity based on if we pass the OrderActivity ID in the OrderActivityViewModel object.
        /// </summary>
        /// <param name="OrderActivityModel">pass the details of OrderActivity in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of OrderActivity row
        /// </returns>
        public ActionResult SaveOrderActivity(Model.OrderActivity OrderActivityModel)
        {
            //Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();
            //Check if OrderActivityViewModel 
            if (OrderActivityModel != null)
            {
                if (OrderActivityModel.OrderActivityID > 0)
                {
                    OrderActivityModel.ModifiedBy = userId;
                    OrderActivityModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                //Call the AddOrderActivity Method to Add / Update current OrderActivity
                newId = _service.AddUptdateOrderActivity(OrderActivityModel);
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current OrderActivity in the view model by ID
        /// </summary>
        /// <param name="OrderActivityID">The order activity identifier.</param>
        /// <returns></returns>
        public ActionResult GetOrderActivity(int OrderActivityID)
        {
            var currentOrderActivity = _service.GetOrderActivityByID(Convert.ToInt32(OrderActivityID));
            return PartialView(PartialViews.OrderActivityAddEdit, currentOrderActivity);
        }

        /// <summary>
        /// Delete the current OrderActivity based on the OrderActivity ID passed in the OrderActivityModel
        /// </summary>
        /// <param name="OrderActivityID">The order activity identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteOrderActivity(int OrderActivityID)
        {
            var currentOrderActivity = _service.GetOrderActivityByID(Convert.ToInt32(OrderActivityID));
            var userId = Helpers.GetLoggedInUserId();
            if (currentOrderActivity != null)
            {
                currentOrderActivity.IsActive = false;
                currentOrderActivity.ModifiedBy = userId;
                currentOrderActivity.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current OrderActivity
                var result = _service.AddUptdateOrderActivity(currentOrderActivity);

                //return deleted ID of current OrderActivity as Json Result to the Ajax Call.
                return Json(result);
            }
            return Json(null);
        }

        /// <summary>
        /// Reset the OrderActivity View Model and pass it to OrderActivityAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetOrderActivityForm()
        {
            //Intialize the new object of OrderActivity ViewModel
            var orderActivityViewModel = new Model.OrderActivity();

            //Pass the View Model as OrderActivityViewModel to PartialView OrderActivityAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.OrderActivityAddEdit, orderActivityViewModel);
        }
    }
}
