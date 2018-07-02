using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Models;
using System.Web.Mvc;

namespace BillingSystem.Controllers
{
    public class DeptTimmingController : BaseController
    {
        private readonly IDeptTimmingService _service;

        public DeptTimmingController(IDeptTimmingService service)
        {
            _service = service;
        }


        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the DeptTimming list
        /// </summary>
        /// <returns>action result with the partial view containing the DeptTimming list object</returns>
        [HttpPost]
        public ActionResult BindDeptTimmingList()
        {
            // Get the facilities list
            var deptTimmingList = _service.GetDeptTimming();

            // Pass the ActionResult with List of DeptTimmingViewModel object to Partial View DeptTimmingList
            return PartialView(PartialViews.DeptTimmingList, deptTimmingList);
        }

        /// <summary>
        /// Delete the current DeptTimming based on the DeptTimming ID passed in the DeptTimmingModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteDeptTimming(int id)
        {
            // Get DeptTimming model object by current DeptTimming ID
            var currentDeptTimming = _service.GetDeptTimmingById(id);
            var userId = Helpers.GetLoggedInUserId();

            // Check If DeptTimming model is not null
            if (currentDeptTimming != null)
            {
                currentDeptTimming.IsActive = false;

                // Update Operation of current DeptTimming
                int result = _service.SaveDeptTimming(currentDeptTimming);

                // return deleted ID of current DeptTimming as Json Result to the Ajax Call.
                return Json(result);
            }

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Get the details of the current DeptTimming in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetDeptTimming(int id)
        {
            // Call the AddDeptTimming Method to Add / Update current DeptTimming
            var currentDeptTimming = _service.GetDeptTimmingById(id);

            // Pass the ActionResult with the current DeptTimmingViewModel object as model to PartialView DeptTimmingAddEdit
            return PartialView(PartialViews.DeptTimmingAddEdit, currentDeptTimming);
        }

        /// <summary>
        /// Get the details of the DeptTimming View in the Model DeptTimming such as DeptTimmingList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model DeptTimming to be passed to View
        ///     DeptTimming
        /// </returns>
        public ActionResult DeptTimmingMain()
        {
            // Get the Entity list
            var deptTimmingList = _service.GetDeptTimming();

            // Intialize the View Model i.e. DeptTimmingView which is binded to Main View Index.cshtml under DeptTimming
            var deptTimmingView = new DeptTimmingView
            {
                DeptTimmingList = deptTimmingList,
                CurrentDeptTimming = new DeptTimming()
            };

            // Pass the View Model in ActionResult to View DeptTimming
            return View(deptTimmingView);
        }

        /// <summary>
        /// Reset the DeptTimming View Model and pass it to DeptTimmingAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetDeptTimmingForm()
        {
            // Intialize the new object of DeptTimming ViewModel
            var deptTimmingViewModel = new DeptTimming();

            // Pass the View Model as DeptTimmingViewModel to PartialView DeptTimmingAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.DeptTimmingAddEdit, deptTimmingViewModel);
        }

        /// <summary>
        /// Add New or Update the DeptTimming based on if we pass the DeptTimming ID in the DeptTimmingViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of DeptTimming row
        /// </returns>
        public ActionResult SaveDeptTimming(DeptTimming model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                // Call the AddDeptTimming Method to Add / Update current DeptTimming
                newId = _service.SaveDeptTimming(model);
            }

            return Json(newId);
        }


        #endregion
    }
}
