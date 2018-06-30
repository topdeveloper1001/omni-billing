// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerDetailsController.cs" company="">
//   
// </copyright>
// <summary>
//   The holiday planner details controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// The holiday planner details controller.
    /// </summary>
    public class HolidayPlannerDetailsController : BaseController
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the HolidayPlannerDetails list
        /// </summary>
        /// <returns>action result with the partial view containing the HolidayPlannerDetails list object</returns>
        [HttpPost]
        public ActionResult BindHolidayPlannerDetailsList()
        {
            // Initialize the HolidayPlannerDetails BAL object
            using (var holidayPlannerDetailsBal = new HolidayPlannerDetailsService())
            {
                // Get the facilities list
                var holidayPlannerDetailsList =
                    holidayPlannerDetailsBal.GetHolidayPlannerDetails();

                // Pass the ActionResult with List of HolidayPlannerDetailsViewModel object to Partial View HolidayPlannerDetailsList
                return this.PartialView(PartialViews.HolidayPlannerDetailsList, holidayPlannerDetailsList);
            }
        }

        /// <summary>
        /// Delete the current HolidayPlannerDetails based on the HolidayPlannerDetails ID passed in the
        ///     HolidayPlannerDetailsModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteHolidayPlannerDetails(int id)
        {
            using (var bal = new HolidayPlannerDetailsService())
            {
                // Get HolidayPlannerDetails model object by current HolidayPlannerDetails ID
                var currentHolidayPlannerDetails = bal.GetHolidayPlannerDetailsByID(id);
                int userId = Helpers.GetLoggedInUserId();

                // Check If HolidayPlannerDetails model is not null
                if (currentHolidayPlannerDetails != null)
                {
                    // currentHolidayPlannerDetails.IsActive = false;
                    // currentHolidayPlannerDetails.ModifiedBy = userId;
                    // currentHolidayPlannerDetails.ModifiedDate = DateTime.Now;

                    // Update Operation of current HolidayPlannerDetails
                    var result = bal.SaveHolidayPlannerDetails(currentHolidayPlannerDetails);

                    // return deleted ID of current HolidayPlannerDetails as Json Result to the Ajax Call.
                    return this.Json(result);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return this.Json(null);
        }

        /// <summary>
        /// Get the details of the current HolidayPlannerDetails in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetHolidayPlannerDetails(int id)
        {
            using (var bal = new HolidayPlannerDetailsService())
            {
                // Call the AddHolidayPlannerDetails Method to Add / Update current HolidayPlannerDetails
                HolidayPlannerDetails currentHolidayPlannerDetails = bal.GetHolidayPlannerDetailsByID(id);

                // Pass the ActionResult with the current HolidayPlannerDetailsViewModel object as model to PartialView HolidayPlannerDetailsAddEdit
                return this.PartialView(PartialViews.HolidayPlannerDetailsAddEdit, currentHolidayPlannerDetails);
            }
        }

        /// <summary>
        /// Get the details of the HolidayPlannerDetails View in the Model HolidayPlannerDetails such as
        ///     HolidayPlannerDetailsList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model HolidayPlannerDetails to be passed to View
        ///     HolidayPlannerDetails
        /// </returns>
        public ActionResult HolidayPlannerDetailsMain()
        {
            // Initialize the HolidayPlannerDetails BAL object
            var holidayPlannerDetailsBal = new HolidayPlannerDetailsService();

            // Get the Entity list
            var holidayPlannerDetailsList =
                holidayPlannerDetailsBal.GetHolidayPlannerDetails();

            // Intialize the View Model i.e. HolidayPlannerDetailsView which is binded to Main View Index.cshtml under HolidayPlannerDetails
            var holidayPlannerDetailsView = new HolidayPlannerDetailsView
                                                {
                                                    HolidayPlannerDetailsList =
                                                        holidayPlannerDetailsList, 
                                                    CurrentHolidayPlannerDetails =
                                                        new HolidayPlannerDetails()
                                                };

            // Pass the View Model in ActionResult to View HolidayPlannerDetails
            return View(holidayPlannerDetailsView);
        }

        /// <summary>
        /// Reset the HolidayPlannerDetails View Model and pass it to HolidayPlannerDetailsAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetHolidayPlannerDetailsForm()
        {
            // Intialize the new object of HolidayPlannerDetails ViewModel
            var holidayPlannerDetailsViewModel = new HolidayPlannerDetails();

            // Pass the View Model as HolidayPlannerDetailsViewModel to PartialView HolidayPlannerDetailsAddEdit just to update the AddEdit partial view.
            return this.PartialView(PartialViews.HolidayPlannerDetailsAddEdit, holidayPlannerDetailsViewModel);
        }

        /// <summary>
        /// Add New or Update the HolidayPlannerDetails based on if we pass the HolidayPlannerDetails ID in the
        ///     HolidayPlannerDetailsViewModel object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of HolidayPlannerDetails row
        /// </returns>
        public ActionResult SaveHolidayPlannerDetails(HolidayPlannerDetails model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new HolidayPlannerDetailsService())
                {
                    if (model.Id > 0)
                    {
                        // model.ModifiedBy = userId;
                        // model.ModifiedDate = DateTime.Now;
                    }

                    // Call the AddHolidayPlannerDetails Method to Add / Update current HolidayPlannerDetails
                    newId = bal.SaveHolidayPlannerDetails(model);
                }
            }

            return this.Json(newId);
        }

        #endregion


        public ActionResult DeleteHolidayPlannerEvent(int id)
        {
            bool isDeleted = false;
            using (var bal = new HolidayPlannerDetailsService())
            {
                isDeleted = bal.DeleteHolidayEvent(id);
            }

            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }
    }
}