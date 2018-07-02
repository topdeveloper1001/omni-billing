// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerController.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System.Web.Mvc;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;
    using BillingSystem.Model.Model;
    using BillingSystem.Models;

    /// <summary>
    /// FutureOrderActivity controller.
    /// </summary>
    public class FutureOrderActivityController : Controller
    {
        private readonly IFutureOrderActivityService _service;

        public FutureOrderActivityController(IFutureOrderActivityService service)
        {
            _service = service;
        }


        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the FutureOrderActivity list
        /// </summary>
        /// <returns>action result with the partial view containing the FutureOrderActivity list object</returns>
        [HttpPost]
        public ActionResult BindFutureOrderActivityList()
        {
            // Get the facilities list
            var FutureOrderActivityList = _service.GetFutureOrderActivity();

            // Pass the ActionResult with List of FutureOrderActivityViewModel object to Partial View FutureOrderActivityList
            //return this.PartialView(PartialViews.FutureOrderActivityList, FutureOrderActivityList);
            return null;
        }

        /// <summary>
        /// Delete the current FutureOrderActivity based on the FutureOrderActivity ID passed in the FutureOrderActivityModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteFutureOrderActivity(int id)
        {
            // Get FutureOrderActivity model object by current FutureOrderActivity ID
            var currentFutureOrderActivity = _service.GetFutureOrderActivityById(id);
            var userId = Helpers.GetLoggedInUserId();

            // Check If FutureOrderActivity model is not null
            if (currentFutureOrderActivity != null)
            {
                currentFutureOrderActivity.IsActive = false;

                // currentFutureOrderActivity.ModifiedBy = userId;
                // currentFutureOrderActivity.ModifiedDate = DateTime.Now;

                // Update Operation of current FutureOrderActivity
                int result = _service.SaveFutureOrderActivity(currentFutureOrderActivity);

                // return deleted ID of current FutureOrderActivity as Json Result to the Ajax Call.
                return Json(result);
            }

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Get the details of the current FutureOrderActivity in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetFutureOrderActivity(int id)
        {
            FutureOrderActivity currentFutureOrderActivity = _service.GetFutureOrderActivityById(id);
            return null;
        }

        /// <summary>
        /// Get the details of the FutureOrderActivity View in the Model FutureOrderActivity such as FutureOrderActivityList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model FutureOrderActivity to be passed to View
        ///     FutureOrderActivity
        /// </returns>
        public ActionResult FutureOrderActivityMain()
        {
            var FutureOrderActivityList = _service.GetFutureOrderActivity();
            var FutureOrderActivityView = new FutureOrderActivityView
            {
                FutureOrderActivityList = FutureOrderActivityList,
                CurrentFutureOrderActivity = new FutureOrderActivity()
            };

            return null;
        }

        /// <summary>
        /// Reset the FutureOrderActivity View Model and pass it to FutureOrderActivityAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetFutureOrderActivityForm()
        {
            // Intialize the new object of FutureOrderActivity ViewModel
            var FutureOrderActivityViewModel = new FutureOrderActivity();

            // Pass the View Model as FutureOrderActivityViewModel to PartialView FutureOrderActivityAddEdit just to update the AddEdit partial view.
            //return this.PartialView(PartialViews.FutureOrderActivityAddEdit, FutureOrderActivityViewModel);
            return null;
        }

        /// <summary>
        /// Add New or Update the FutureOrderActivity based on if we pass the FutureOrderActivity ID in the FutureOrderActivityViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of FutureOrderActivity row
        /// </returns>
        public ActionResult SaveFutureOrderActivity(FutureOrderActivity model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                newId = _service.SaveFutureOrderActivity(model);
            }

            return this.Json(newId);
        }

        #endregion
    }
}
