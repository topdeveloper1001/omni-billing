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
    using System.Web.Mvc;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Models;

    public class HolidayPlannerDetailsController : BaseController
    {
        private readonly IHolidayPlannerDetailsService _service;

        public HolidayPlannerDetailsController(IHolidayPlannerDetailsService service)
        {
            _service = service;
        }


        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the HolidayPlannerDetails list
        /// </summary>
        /// <returns>action result with the partial view containing the HolidayPlannerDetails list object</returns>
        [HttpPost]
        public ActionResult BindHolidayPlannerDetailsList()
        {
            var lst = _service.GetHolidayPlannerDetails();
            return this.PartialView(PartialViews.HolidayPlannerDetailsList, lst);
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
            var currentHolidayPlannerDetails = _service.GetHolidayPlannerDetailsByID(id);
            int userId = Helpers.GetLoggedInUserId();

            if (currentHolidayPlannerDetails != null)
            {
                var result = _service.SaveHolidayPlannerDetails(currentHolidayPlannerDetails);
                return this.Json(result);
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
            var currentHolidayPlannerDetails = _service.GetHolidayPlannerDetailsByID(id);
            return this.PartialView(PartialViews.HolidayPlannerDetailsAddEdit, currentHolidayPlannerDetails);
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
            var lst = _service.GetHolidayPlannerDetails();
            var holidayPlannerDetailsView = new HolidayPlannerDetailsView
            {
                HolidayPlannerDetailsList = lst,
                CurrentHolidayPlannerDetails = new HolidayPlannerDetails()
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
                newId = _service.SaveHolidayPlannerDetails(model);
            }

            return this.Json(newId);
        }

        #endregion


        public ActionResult DeleteHolidayPlannerEvent(int id)
        {
            bool isDeleted = false;
            isDeleted = _service.DeleteHolidayEvent(id);

            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }
    }
}