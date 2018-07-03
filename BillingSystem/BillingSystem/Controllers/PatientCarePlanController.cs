using System.Linq;

namespace BillingSystem.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Models;

    /// <summary>
    /// PatientCarePlan controller.
    /// </summary>
    public class PatientCarePlanController : Controller
    {
        private readonly IPatientCarePlanService _service;

        private readonly ICarePlanTaskService _cpService;

        public PatientCarePlanController(IPatientCarePlanService service, ICarePlanTaskService cpService)
        {
            _service = service;
            _cpService = cpService;
        }
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the PatientCarePlan list
        /// </summary>
        /// <returns>action result with the partial view containing the PatientCarePlan list object</returns>
        [HttpPost]
        public ActionResult BindPatientCarePlanList()
        {
            var patientCarePlanList = _service.GetPatientCarePlan();
            return this.PartialView(PartialViews.PatientCarePlanList, patientCarePlanList);
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
            var currentPatientCarePlan = _service.GetPatientCarePlanById(id);
            var userId = Helpers.GetLoggedInUserId();

            // Check If PatientCarePlan model is not null
            if (currentPatientCarePlan != null)
            {
                currentPatientCarePlan.IsActive = false;

                currentPatientCarePlan.ModifiedBy = userId;
                currentPatientCarePlan.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                // Update Operation of current PatientCarePlan
                int result = _service.SavePatientCarePlan(currentPatientCarePlan);

                // return deleted ID of current PatientCarePlan as Json Result to the Ajax Call.
                return this.Json(result);
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
            var currentPatientCarePlan = _service.GetPatientCarePlanById(id);
            return this.PartialView(PartialViews.PatientCarePlanAddEdit, currentPatientCarePlan);
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

            // Get the Entity list
            var patientCarePlanList = _service.GetPatientCarePlan();

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
        #endregion
    }
}
