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
    using System.Collections.Generic;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Model.Model;
    using BillingSystem.Models;

    /// <summary>
    /// FutureOrderActivity controller.
    /// </summary>
    public class FutureOrderActivityController : Controller
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the FutureOrderActivity list
        /// </summary>
        /// <returns>action result with the partial view containing the FutureOrderActivity list object</returns>
        [HttpPost]
        public ActionResult BindFutureOrderActivityList()
        {
            // Initialize the FutureOrderActivity BAL object
            using (var FutureOrderActivityBal = new FutureOrderActivityBal())
            {
                // Get the facilities list
                var FutureOrderActivityList = FutureOrderActivityBal.GetFutureOrderActivity();

                // Pass the ActionResult with List of FutureOrderActivityViewModel object to Partial View FutureOrderActivityList
                //return this.PartialView(PartialViews.FutureOrderActivityList, FutureOrderActivityList);
                return null;
            }
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
            using (var bal = new FutureOrderActivityBal())
            {
                // Get FutureOrderActivity model object by current FutureOrderActivity ID
                var currentFutureOrderActivity = bal.GetFutureOrderActivityById(id);
                var userId = Helpers.GetLoggedInUserId();

                // Check If FutureOrderActivity model is not null
                if (currentFutureOrderActivity != null)
                {
                    currentFutureOrderActivity.IsActive = false;

                    // currentFutureOrderActivity.ModifiedBy = userId;
                    // currentFutureOrderActivity.ModifiedDate = DateTime.Now;

                    // Update Operation of current FutureOrderActivity
                    int result = bal.SaveFutureOrderActivity(currentFutureOrderActivity);

                    // return deleted ID of current FutureOrderActivity as Json Result to the Ajax Call.
                    return this.Json(result);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return this.Json(null);
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
            using (var bal = new FutureOrderActivityBal())
            {
                // Call the AddFutureOrderActivity Method to Add / Update current FutureOrderActivity
                FutureOrderActivity currentFutureOrderActivity = bal.GetFutureOrderActivityById(id);

                // Pass the ActionResult with the current FutureOrderActivityViewModel object as model to PartialView FutureOrderActivityAddEdit
                //return this.PartialView(PartialViews.FutureOrderActivityAddEdit, currentFutureOrderActivity);
                return null;
            }
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
            // Initialize the FutureOrderActivity BAL object
            var FutureOrderActivityBal = new FutureOrderActivityBal();

            // Get the Entity list
            var FutureOrderActivityList = FutureOrderActivityBal.GetFutureOrderActivity();

            // Intialize the View Model i.e. FutureOrderActivityView which is binded to Main View Index.cshtml under FutureOrderActivity
            var FutureOrderActivityView = new FutureOrderActivityView
                                         {
                                             FutureOrderActivityList = FutureOrderActivityList, 
                                             CurrentFutureOrderActivity = new FutureOrderActivity()
                                         };

            // Pass the View Model in ActionResult to View FutureOrderActivity
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
                using (var bal = new FutureOrderActivityBal())
                {
                    if (model.FutureOrderActivityID > 0)
                    {
                        // model.ModifiedBy = userId;
                        // model.ModifiedDate = DateTime.Now;
                    }

                    // Call the AddFutureOrderActivity Method to Add / Update current FutureOrderActivity
                    newId = bal.SaveFutureOrderActivity(model);
                }
            }

            return this.Json(newId);
        }

        #endregion
    }
}
