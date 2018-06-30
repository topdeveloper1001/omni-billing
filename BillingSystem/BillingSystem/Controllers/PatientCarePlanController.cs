// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerController.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace BillingSystem.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// PatientCarePlan controller.
    /// </summary>
    public class PatientCarePlanController : Controller
    {
        private readonly ICarePlanTaskService _cpService;
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the PatientCarePlan list
        /// </summary>
        /// <returns>action result with the partial view containing the PatientCarePlan list object</returns>
        [HttpPost]
        public ActionResult BindPatientCarePlanList()
        {
            // Initialize the PatientCarePlan BAL object
            using (var patientCarePlanBal = new PatientCarePlanService())
            {
                // Get the facilities list
                var patientCarePlanList = patientCarePlanBal.GetPatientCarePlan();

                // Pass the ActionResult with List of PatientCarePlanViewModel object to Partial View PatientCarePlanList
                return this.PartialView(PartialViews.PatientCarePlanList, patientCarePlanList);
            }
        }

        /// <summary>
        /// Delete the current PatientCarePlan based on the PatientCarePlan ID passed in the PatientCarePlanModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeletePatientCarePlan(int id)
        {
            using (var bal = new PatientCarePlanService())
            {
                // Get PatientCarePlan model object by current PatientCarePlan ID
                var currentPatientCarePlan = bal.GetPatientCarePlanById(id);
                var userId = Helpers.GetLoggedInUserId();

                // Check If PatientCarePlan model is not null
                if (currentPatientCarePlan != null)
                {
                    currentPatientCarePlan.IsActive = false;

                    currentPatientCarePlan.ModifiedBy = userId;
                    currentPatientCarePlan.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                    // Update Operation of current PatientCarePlan
                    int result = bal.SavePatientCarePlan(currentPatientCarePlan);

                    // return deleted ID of current PatientCarePlan as Json Result to the Ajax Call.
                    return this.Json(result);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return this.Json(null);
        }

        /// <summary>
        /// Get the details of the current PatientCarePlan in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetPatientCarePlan(int id)
        {
            using (var bal = new PatientCarePlanService())
            {
                // Call the AddPatientCarePlan Method to Add / Update current PatientCarePlan
                PatientCarePlan currentPatientCarePlan = bal.GetPatientCarePlanById(id);

                // Pass the ActionResult with the current PatientCarePlanViewModel object as model to PartialView PatientCarePlanAddEdit
                return this.PartialView(PartialViews.PatientCarePlanAddEdit, currentPatientCarePlan);
            }
        }

        /// <summary>
        /// Get the details of the PatientCarePlan View in the Model PatientCarePlan such as PatientCarePlanList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model PatientCarePlan to be passed to View
        ///     PatientCarePlan
        /// </returns>
        public ActionResult PatientCarePlanMain()
        {
            // Initialize the PatientCarePlan BAL object
            var patientCarePlanBal = new PatientCarePlanService();

            // Get the Entity list
            var patientCarePlanList = patientCarePlanBal.GetPatientCarePlan();

            // Intialize the View Model i.e. PatientCarePlanView which is binded to Main View Index.cshtml under PatientCarePlan
            var patientCarePlanView = new PatientCarePlanView
            {
                PatientCarePlanList = patientCarePlanList,
                CurrentPatientCarePlan = new PatientCarePlan(),
                CurrentCarePlanTask = new CarePlanTask()
            };

            // Pass the View Model in ActionResult to View PatientCarePlan
            return View(patientCarePlanView);
        }

        /// <summary>
        /// Reset the PatientCarePlan View Model and pass it to PatientCarePlanAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetPatientCarePlanForm()
        {
            // Intialize the new object of PatientCarePlan ViewModel
            var patientCarePlanViewModel = new PatientCarePlan();

            // Pass the View Model as PatientCarePlanViewModel to PartialView PatientCarePlanAddEdit just to update the AddEdit partial view.
            return this.PartialView(PartialViews.PatientCarePlanAddEdit, patientCarePlanViewModel);
        }

        /// <summary>
        /// Add New or Update the PatientCarePlan based on if we pass the PatientCarePlan ID in the PatientCarePlanViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of PatientCarePlan row
        /// </returns>
        //public ActionResult SavePatientCarePlan(PatientCarePlan model)
        //{
        //    // Initialize the newId variable 
        //    var corporateId = Helpers.GetSysAdminCorporateID();
        //    var facilityId = Helpers.GetDefaultFacilityId();
        //    var dateTime = Helpers.GetInvariantCultureDateTime();
        //    int newId = -1;
        //    int userId = Helpers.GetLoggedInUserId();

        //    // Check if Model is not null 
        //    if (model != null)
        //    {
        //        using (var bal = new PatientCarePlanBal())
        //        {
        //            if (model.Id > 0)
        //            {
        //                model.ModifiedBy = userId;
        //                model.ModifiedDate = dateTime;

        //            }
        //            else
        //            {
        //                model.IsActive = true;
        //                model.CorporateId = corporateId;
        //                model.FacilityId = facilityId;
        //                model.CreatedDate = dateTime;
        //                model.CreatedBy = userId;

        //            }
        //            // Call the AddPatientCarePlan Method to Add / Update current PatientCarePlan
        //            newId = bal.SavePatientCarePlan(model);
        //        }
        //    }

        //    return Json(newId);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="careId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult BindCarePlanTask(int careId, List<string> code)
        {
            var careTaskList = code == null ? _cpService.BindCarePlanTask(careId) : _cpService.BindCarePlanTask(careId).Where(e => !code.Contains(e.Id.ToString())).ToList();
            if (careTaskList.Count > 0)
            {
                var list = new List<SelectListItem>();
                list.AddRange(careTaskList.Select(item => new SelectListItem
                {
                    Text = string.Format("{0} - {1} (Plan:{2})", item.TaskNumber, item.TaskName, item.CarePlan),
                    Value = item.Id.ToString()
                }));

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return Json(0);
        }


        //public ActionResult BindCarePlanTask(int careId)
        //{
        //    var cBal = new CarePlanTaskBal();
        //    var careTaskList = cBal.BindCarePlanTask(careId);
        //    if (careTaskList.Count > 0)
        //    {
        //        var list = new List<SelectListItem>();
        //        list.AddRange(careTaskList.Select(item => new SelectListItem
        //        {
        //            Text = string.Format("{0} - {1} (Plan:{2})", item.TaskNumber, item.TaskName, item.CarePlan),
        //            Value = item.TaskNumber.ToString()
        //        }));

        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(0);
        //}
        #endregion
    }
}
