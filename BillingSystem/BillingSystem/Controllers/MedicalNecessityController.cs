// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerController.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    /// <summary>
    ///     MedicalNecessity controller.
    /// </summary>
    public class MedicalNecessityController : Controller
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the MedicalNecessity list
        /// </summary>
        /// <returns>action result with the partial view containing the MedicalNecessity list object</returns>
        [HttpPost]
        public ActionResult BindMedicalNecessityList()
        {
            // Initialize the MedicalNecessity BAL object
            using (var medicalNecessityBal = new MedicalNecessityBal(Helpers.DefaultDiagnosisTableNumber))
            {
                // Get the facilities list
                List<MedicalNecessityCustomModel> medicalNecessityList = medicalNecessityBal.GetMedicalNecessity();

                // Pass the ActionResult with List of MedicalNecessityViewModel object to Partial View MedicalNecessityList
                return PartialView(PartialViews.MedicalNecessityList, medicalNecessityList);
            }
        }

        /// <summary>
        ///     Delete the current MedicalNecessity based on the MedicalNecessity ID passed in the MedicalNecessityModel
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult DeleteMedicalNecessity(int id)
        {
            using (var bal = new MedicalNecessityBal(Helpers.DefaultDiagnosisTableNumber))
            {
                // Get MedicalNecessity model object by current MedicalNecessity ID
                var currentMedicalNecessity = bal.GetMedicalNecessityById(id);
                // Check If MedicalNecessity model is not null
                if (currentMedicalNecessity != null)
                {
                    currentMedicalNecessity.IsActive = false;

                    // currentMedicalNecessity.ModifiedBy = userId;
                    // currentMedicalNecessity.ModifiedDate = DateTime.Now;

                    // Update Operation of current MedicalNecessity
                    int result = bal.SaveMedicalNecessity(currentMedicalNecessity);

                    // return deleted ID of current MedicalNecessity as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        ///     Get the details of the current MedicalNecessity in the view model by ID
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetMedicalNecessity(int id)
        {
            using (var bal = new MedicalNecessityBal(Helpers.DefaultDiagnosisTableNumber))
            {
                // Call the AddMedicalNecessity Method to Add / Update current MedicalNecessity
                MedicalNecessity currentMedicalNecessity = bal.GetMedicalNecessityById(id);

                // Pass the ActionResult with the current MedicalNecessityViewModel object as model to PartialView MedicalNecessityAddEdit
                return PartialView(PartialViews.MedicalNecessityAddEdit, currentMedicalNecessity);
            }
        }

        /// <summary>
        ///     Get the details of the MedicalNecessity View in the Model MedicalNecessity such as MedicalNecessityList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        ///     returns the actionresult in the form of current object of the Model MedicalNecessity to be passed to View
        ///     MedicalNecessity
        /// </returns>
        public ActionResult MedicalNecessityMain()
        {
            // Initialize the MedicalNecessity BAL object
            var medicalNecessityBal = new MedicalNecessityBal(Helpers.DefaultDiagnosisTableNumber);

            // Get the Entity list
            List<MedicalNecessityCustomModel> medicalNecessityList = medicalNecessityBal.GetMedicalNecessity();

            // Intialize the View Model i.e. MedicalNecessityView which is binded to Main View Index.cshtml under MedicalNecessity
            var medicalNecessityView = new MedicalNecessityView
            {
                MedicalNecessityList = medicalNecessityList,
                CurrentMedicalNecessity = new MedicalNecessity()
            };

            // Pass the View Model in ActionResult to View MedicalNecessity
            return View(medicalNecessityView);
        }

        /// <summary>
        ///     Reset the MedicalNecessity View Model and pass it to MedicalNecessityAddEdit Partial View.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetMedicalNecessityForm()
        {
            // Intialize the new object of MedicalNecessity ViewModel
            var medicalNecessityViewModel = new MedicalNecessity();

            // Pass the View Model as MedicalNecessityViewModel to PartialView MedicalNecessityAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.MedicalNecessityAddEdit, medicalNecessityViewModel);
        }

        /// <summary>
        ///     Add New or Update the MedicalNecessity based on if we pass the MedicalNecessity ID in the MedicalNecessityViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <returns>
        ///     returns the newly added or updated ID of MedicalNecessity row
        /// </returns>
        public ActionResult SaveMedicalNecessity(MedicalNecessity model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new MedicalNecessityBal(Helpers.DefaultDiagnosisTableNumber))
                {
                    if (model.Id > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    }

                    // Call the AddMedicalNecessity Method to Add / Update current MedicalNecessity
                    newId = bal.SaveMedicalNecessity(model);
                }
            }

            return Json(newId);
        }


        public ActionResult GetMedicalNecesstiyById(int id)
        {
            var bal = new MedicalNecessityBal(Helpers.DefaultDiagnosisTableNumber);
            MedicalNecessity currentMedicalNecessity = bal.GetMedicalNecessityById(id);
            var jsonData = new
            {
                currentMedicalNecessity.Id,
                currentMedicalNecessity.IsActive,
                currentMedicalNecessity.ICD9Code,
                currentMedicalNecessity.BillingCode,
                currentMedicalNecessity.BillingCodeType
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSearchData(string text)
        {
            using (var bal = new MedicalNecessityBal(Helpers.DefaultDiagnosisTableNumber))
            {
                var list = bal.GetMedicalNecessity();
                var newList = list.Where(x => x.Description.ToLower().Trim().Contains(text)).ToList();
                return Json(newList, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Index()
        {
            
            return View(new MedicalNecessityView());
        }
        #endregion
    }
}