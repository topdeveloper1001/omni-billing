// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarePlanController.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// </Screen Owner>
// Shashank Modified on : Feb 09 2016
// </Screen Owner>
// <summary>
//   The holiday planner controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace BillingSystem.Controllers
{
    #region System Referneces
    using System.Web.Mvc;
    #endregion

    #region Project Referneces
    using Bal.BusinessAccess;
    using Common;
    using Model;
    using Models;
    #endregion

    /// <summary>
    /// CarePlan controller.
    /// </summary>
    public class CarePlanController : Controller
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the CarePlan list
        /// </summary>
        /// <returns>action result with the partial view containing the CarePlan list object</returns>
        [HttpPost]
        public ActionResult BindCarePlanList(int val, string sort, string sortdir)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int faclityId = Helpers.GetDefaultFacilityId();

            // Initialize the CarePlan BAL object
            using (var carePlanBal = new CarePlanBal())
            {
                // Get the facilities list
                // var carePlanList = carePlanBal.GetCarePlan();
                var carePlanList = carePlanBal.GetActiveCarePlan(corporateId, faclityId, Convert.ToBoolean(val));
                if (carePlanList.Count > 0)
                {
                    carePlanList = carePlanList.OrderBy(string.Format("{0} {1}", sort, sortdir)).ToList();
                }
                // Pass the ActionResult with List of CarePlanViewModel object to Partial View CarePlanList
                return PartialView(PartialViews.CarePlanList, carePlanList);
            }
        }

        /// <summary>
        /// Delete the current CarePlan based on the CarePlan ID passed in the CarePlanModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="inActive"></param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteCarePlan(int id, bool inActive)
        {
            using (var bal = new CarePlanBal())
            {
                // Get CarePlan model object by current CarePlan ID
                var currentCarePlan = bal.GetCarePlanById(id);
                var userId = Helpers.GetLoggedInUserId();

                // Check If CarePlan model is not null
                if (currentCarePlan != null)
                {
                    currentCarePlan.IsActive = false;
                    int result = inActive
                        ? bal.SaveCarePlan(currentCarePlan, inActive ? 1 : 0)
                        : bal.DeleteCarePlan(currentCarePlan);

                    // return deleted ID of current CarePlan as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Get the details of the CarePlan View in the Model CarePlan such as CarePlanList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model CarePlan to be passed to View
        ///     CarePlan
        /// </returns>
        public ActionResult Index()
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int faclityId = Helpers.GetDefaultFacilityId();

            // Initialize the CarePlan BAL object
            var carePlanBal = new CarePlanBal();

            // Get the Entity list
            // var carePlanList = carePlanBal.GetCarePlan();
            var carePlanList = carePlanBal.GetActiveCarePlan(corporateId, faclityId, true);

            // Intialize the View Model i.e. CarePlanView which is binded to Main View Index.cshtml under CarePlan
            var carePlanView = new CarePlanView { CarePlanList = carePlanList, CurrentCarePlan = new Careplan() };

            // Pass the View Model in ActionResult to View CarePlan
            return View(carePlanView);
        }

        /// <summary>
        /// Reset the CarePlan View Model and pass it to CarePlanAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetCarePlanForm()
        {
            // Intialize the new object of CarePlan ViewModel
            var carePlanViewModel = new Careplan();

            // Pass the View Model as CarePlanViewModel to PartialView CarePlanAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.CarePlanAddEdit, carePlanViewModel);
        }

        /// <summary>
        /// Add New or Update the CarePlan based on if we pass the CarePlan ID in the CarePlanViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of CarePlan row
        /// </returns>
        public ActionResult SaveCarePlan(Careplan model)
        {
            // Initialize the newId variable 
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            int newId = -1;
            var dateTime = Helpers.GetInvariantCultureDateTime();
            int userId = Helpers.GetLoggedInUserId();

            var cBal = new CarePlanBal();
            var check = cBal.CheckDuplicateCarePlanNumber(model.Id, model.PlanNumber, facilityId, corporateId);

            // Check if Model is not null 
            if (check)
            {
                return Json("-1");
            }
            else
            {
                using (var bal = new CarePlanBal())
                {
                    if (model.Id > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = dateTime;
                        model.FacilityId = facilityId;
                        model.CorporateId = corporateId;
                    }
                    else
                    {
                        model.FacilityId = facilityId;
                        model.CorporateId = corporateId;
                        model.CreatedBy = userId;
                        model.CreatedDate = dateTime;
                    }

                    // Call the AddCarePlan Method to Add / Update current CarePlan
                    newId = bal.SaveCarePlan(model, 2);
                }
            }

            return Json(newId);
        }

        /// <summary>
        /// Gets the care plan data.
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetCarePlanData(int id)
        {
            using (var bal = new CarePlanBal())
            {
                var currentCarePlan = bal.GetCarePlanById(id);
                var jsonResult =
                    new
                        {
                            currentCarePlan.DiagnosisAssociated,
                            currentCarePlan.EncounterPatientType,
                            currentCarePlan.IsActive,
                            currentCarePlan.PlanDescription,
                            currentCarePlan.PlanLength,
                            currentCarePlan.PlanLengthType,
                            currentCarePlan.PlanNumber,
                            currentCarePlan.Id,
                            currentCarePlan.Name
                        };

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the task number.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetTaskNumber()
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            using (var bal = new CarePlanBal())
            {
                int carePlanNumber = bal.GetTaskNumber(facilityId, corporateId);
                return Json(carePlanNumber);
            }
        }

        #endregion
    }
}