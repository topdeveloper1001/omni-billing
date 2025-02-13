﻿using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Web.Mvc;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class ManagedCareController : BaseController
    {
        private readonly IManagedCareService _service;
        private readonly IInsurancePlansService _ipService;
        private readonly IInsuranceCompanyService _icService;

        public ManagedCareController(IManagedCareService service, IInsurancePlansService ipService, IInsuranceCompanyService icService)
        {
            _service = service;
            _ipService = ipService;
            _icService = icService;
        }


        /// <summary>
        /// Get the details of the ManagedCare View in the Model ManagedCare such as ManagedCareList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model ManagedCare to be passed to View ManagedCare
        /// </returns>
        public ActionResult Index()
        {
            //Get the Entity list
            var corporateId = Helpers.GetDefaultCorporateId();
            var managedCareList = _service.GetManagedCareListByCorporate(corporateId);

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
            //Get the facilities list
            var corporateId = Helpers.GetDefaultCorporateId();
            var managedCareList = _service.GetManagedCareListByCorporate(corporateId);

            //Pass the ActionResult with List of ManagedCareViewModel object to Partial View ManagedCareList
            return PartialView(PartialViews.ManagedCareList, managedCareList);
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
                newId = _service.AddUptdateManagedCare(model);
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
            //Call the AddManagedCare Method to Add / Update current ManagedCare
            var currentManagedCare = _service.GetManagedCareByID(Convert.ToInt32(managedCareId));

            return PartialView(PartialViews.ManagedCareAddEdit, currentManagedCare);
        }

        /// <summary>
        /// Delete the current ManagedCare based on the ManagedCare ID passed in the ManagedCareModel
        /// </summary>
        /// <param name="managedCareId">The managed care identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteManagedCare(int managedCareId)
        {
            var currentManagedCare = _service.GetManagedCareByID(Convert.ToInt32(managedCareId));
            var userId = Helpers.GetLoggedInUserId();
            if (currentManagedCare != null)
            {
                currentManagedCare.IsActive = false;
                currentManagedCare.ModifiedBy = userId;
                currentManagedCare.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current ManagedCare
                var result = _service.AddUptdateManagedCare(currentManagedCare);

                //return deleted ID of current ManagedCare as Json Result to the Ajax Call.
                return Json(result);
            }
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
            var list = _icService.GetInsuranceCompanies(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
            return Json(list);
        }

        /// <summary>
        /// Binds the insurance plans by company identifier.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns></returns>
        public ActionResult BindInsurancePlansByCompanyId(int companyId)
        {
            var list = _ipService.GetInsurancePlansByCompanyId(companyId, CurrentDateTime);
            return Json(list);
        }
    }
}
