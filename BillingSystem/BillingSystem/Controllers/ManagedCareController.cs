using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    public class ManagedCareController : BaseController
    {
        /// <summary>
        /// Get the details of the ManagedCare View in the Model ManagedCare such as ManagedCareList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model ManagedCare to be passed to View ManagedCare
        /// </returns>
        public ActionResult Index()
        {
            //Initialize the ManagedCare BAL object
            var managedCareBal = new ManagedCareBal();

            //Get the Entity list
            var corporateId = Helpers.GetDefaultCorporateId();
            var managedCareList = managedCareBal.GetManagedCareListByCorporate(corporateId);

            //Intialize the View Model i.e. ManagedCareView which is binded to Main View Index.cshtml under ManagedCare
            var managedCareView = new ManagedCareView
            {
                ManagedCareList = managedCareList,
                CurrentManagedCare = new ManagedCare { IsActive = true }
            };

            //Pass the View Model in ActionResult to View ManagedCare
            return View(managedCareView);
        }

        /// <summary>
        /// Bind all the ManagedCare list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the ManagedCare list object
        /// </returns>
        [HttpPost]
        public ActionResult BindManagedCareList()
        {
            //Initialize the ManagedCare BAL object
            using (var managedCareBal = new ManagedCareBal())
            {
                //Get the facilities list
                var corporateId = Helpers.GetDefaultCorporateId();
                var managedCareList = managedCareBal.GetManagedCareListByCorporate(corporateId);

                //Pass the ActionResult with List of ManagedCareViewModel object to Partial View ManagedCareList
                return PartialView(PartialViews.ManagedCareList, managedCareList);
            }
        }

        /// <summary>
        /// Add New or Update the ManagedCare based on if we pass the ManagedCare ID in the ManagedCareViewModel object.
        /// </summary>
        /// <param name="model">pass the details of ManagedCare in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of ManagedCare row
        /// </returns>
        public ActionResult SaveManagedCare(ManagedCare model)
        {
            var currentLocalTime = Helpers.GetInvariantCultureDateTime();
            //Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();

            //Check if ManagedCareViewModel 
            if (model != null)
            {
                using (var bal = new ManagedCareBal())
                {
                    model.CorporateID = corporateId;
                    model.FacilityID = facilityId;
                    if (model.ManagedCareID > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = currentLocalTime;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = currentLocalTime;
                    }

                    //Call the AddManagedCare Method to Add / Update current ManagedCare
                    newId = bal.AddUptdateManagedCare(model);
                }
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current ManagedCare in the view model by ID
        /// </summary>
        /// <param name="managedCareId">The managed care identifier.</param>
        /// <returns></returns>
        public ActionResult GetManagedCare(int managedCareId)
        {
            using (var managedCareBal = new ManagedCareBal())
            {
                //Call the AddManagedCare Method to Add / Update current ManagedCare
                var currentManagedCare = managedCareBal.GetManagedCareByID(Convert.ToInt32(managedCareId));

                //If the view is shown in ViewMode only, then ViewBag.ViewOnly is set to true otherwise false.
                //ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);

                //Pass the ActionResult with the current ManagedCareViewModel object as model to PartialView ManagedCareAddEdit
                return PartialView(PartialViews.ManagedCareAddEdit, currentManagedCare);
            }
        }

        /// <summary>
        /// Delete the current ManagedCare based on the ManagedCare ID passed in the ManagedCareModel
        /// </summary>
        /// <param name="managedCareId">The managed care identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteManagedCare(int managedCareId)
        {
            using (var managedCareBal = new ManagedCareBal())
            {
                //Get ManagedCare model object by current ManagedCare ID
                var currentManagedCare = managedCareBal.GetManagedCareByID(Convert.ToInt32(managedCareId));
                var userId = Helpers.GetLoggedInUserId();
                //Check If ManagedCare model is not null
                if (currentManagedCare != null)
                {
                    currentManagedCare.IsActive = false;
                    currentManagedCare.ModifiedBy = userId;
                    currentManagedCare.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current ManagedCare
                    var result = managedCareBal.AddUptdateManagedCare(currentManagedCare);

                    //return deleted ID of current ManagedCare as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the ManagedCare View Model and pass it to ManagedCareAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetManagedCareForm()
        {
            //Intialize the new object of ManagedCare ViewModel
            var model = new ManagedCare { IsActive = true };

            //Pass the View Model as ManagedCareViewModel to PartialView ManagedCareAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.ManagedCareAddEdit, model);
        }

        /// <summary>
        /// Binds the insurance companies.
        /// </summary>
        /// <returns></returns>
        public ActionResult BindInsuranceCompanies()
        {
            using (var insBal = new InsuranceCompanyBal())
            {
                var list = insBal.GetInsuranceCompanies(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
                return Json(list);
            }
        }

        /// <summary>
        /// Binds the insurance plans by company identifier.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns></returns>
        public ActionResult BindInsurancePlansByCompanyId(int companyId)
        {
            using (var insBal = new InsurancePlansBal())
            {
                var list = insBal.GetInsurancePlansByCompanyId(companyId, CurrentDateTime);
                return Json(list);
            }
        }
    }
}
