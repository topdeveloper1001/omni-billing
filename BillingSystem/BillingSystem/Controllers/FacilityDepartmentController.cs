﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    public class FacilityDepartmentController : BaseController
    {
        private readonly IFacilityDepartmentService _service;
        private readonly IFacilityStructureService _fsService;

        public FacilityDepartmentController(IFacilityDepartmentService service, IFacilityStructureService fsService)
        {
            _service = service;
            _fsService = fsService;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Binds the account dropdowns.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult BindAccountDropdowns()
        {

            int facilityid = Helpers.GetDefaultFacilityId();
            int corporateid = Helpers.GetSysAdminCorporateID();

            // Get the Entity list
            List<FacilityStructure> facilityDepartments = _fsService.GetFacilityDepartments(
                corporateid,
                facilityid.ToString());
            var listrevenueAccount = new List<SelectListItem>();
            var listGeneralGlAccount = new List<SelectListItem>();
            if (facilityDepartments.Count > 0)
            {
                listrevenueAccount.AddRange(
                    facilityDepartments.Where(x => !string.IsNullOrEmpty(x.ExternalValue1))
                        .Select(
                            item => new SelectListItem
                            {
                                // Text = item.ExternalValue1,
                                Value = Convert.ToString(item.ExternalValue1),
                                Text =
                                                Convert.ToString(item.ExternalValue1)
                                                + @" (Department Name :" + item.FacilityStructureName
                                                + @" )",
                            }));
                listGeneralGlAccount.AddRange(
                    facilityDepartments.Where(x => !string.IsNullOrEmpty(x.ExternalValue2))
                        .Select(
                            item => new SelectListItem
                            {
                                // Text = item.ExternalValue2,
                                Text =
                                                Convert.ToString(item.ExternalValue2)
                                                + @" (Department Name :" + item.FacilityStructureName
                                                + @" )",
                                Value = Convert.ToString(item.ExternalValue2),
                            }));
                var jsonResult =
                    new { reveuneAccount = listrevenueAccount, generalLederAccount = listGeneralGlAccount, };
                return this.Json(jsonResult, JsonRequestBehavior.AllowGet);
            }

            return this.Json(null);

        }

        /// <summary>
        /// Delete the current FacilityDepartment based on the FacilityDepartment ID passed in the FacilityDepartmentModel
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteFacilityDepartment(int id)
        {
            var list = new List<FacilityDepartmentCustomModel>();
            // Get FacilityDepartment model object by current FacilityDepartment ID
            FacilityDepartment model = _service.GetFacilityDepartmentById(id);
            int userId = Helpers.GetLoggedInUserId();
            DateTime currentDate = Helpers.GetInvariantCultureDateTime();
            int corporateid = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetSysAdminCorporateID();

            // Check If FacilityDepartment model is not null
            if (model != null)
            {
                model.ModifiedBy = userId;
                model.ModifiedDate = currentDate;
                model.IsActive = false;

                // Update Operation of current FacilityDepartment
                List<FacilityDepartmentCustomModel> result = _service.SaveFacilityDepartment(model);
                list = _service.GetFacilityDepartmentList(corporateid, facilityid, true);

                // return deleted ID of current FacilityDepartment as Json Result to the Ajax Call.
                return this.Json(result);
            }

            // Pass the ActionResult with List of FacilityDepartmentViewModel object to Partial View FacilityDepartmentList
            return this.PartialView(PartialViews.FacilityDepartmentList, list);
        }

        /// <summary>
        /// Get the details of the current FacilityDepartment in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult GetFacilityDepartmentDetails(int id)
        {
            var m = _service.GetFacilityDepartmentById(id);
            return this.Json(m);
        }

        /// <summary>
        /// Gets the facility departments.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetFacilityDepartments(bool showInActive)
        {
            int facilityid = Helpers.GetDefaultFacilityId();
            int corporateid = Helpers.GetSysAdminCorporateID();

            // Get the Entity list
            List<FacilityDepartmentCustomModel> list = _service.GetFacilityDepartmentList(corporateid, facilityid, showInActive);
            return this.PartialView(PartialViews.FacilityDepartmentList, list);
        }

        /// <summary>
        ///     Get the details of the FacilityDepartment View in the Model FacilityDepartment such as FacilityDepartmentList, list
        ///     of countries etc.
        /// </summary>
        /// <returns>
        ///     returns the actionresult in the form of current object of the Model FacilityDepartment to be passed to View
        ///     FacilityDepartment
        /// </returns>
        public ActionResult Index()
        {
            int facilityid = Helpers.GetDefaultFacilityId();
            int corporateid = Helpers.GetSysAdminCorporateID();

            // Get the Entity list
            List<FacilityDepartmentCustomModel> list = _service.GetFacilityDepartmentList(corporateid, facilityid, true);

            // Intialize the View Model i.e. FacilityDepartmentView which is binded to Main View Index.cshtml under FacilityDepartment
            var viewModel = new FacilityDepartmentView
            {
                FacilityDepartmentList = list,
                CurrentFacilityDepartment = new FacilityDepartment()
            };

            // Pass the View Model in ActionResult to View FacilityDepartment
            return View(viewModel);
        }

        /// <summary>
        /// Add New or Update the FacilityDepartment based on if we pass the FacilityDepartment ID in the
        ///     FacilityDepartmentViewModel object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of FacilityDepartment row
        /// </returns>
        public ActionResult SaveFacilityDepartment(FacilityDepartment model)
        {
            // Initialize the newId variable 
            int userId = Helpers.GetLoggedInUserId();
            DateTime currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<FacilityDepartmentCustomModel>();
            int facilityid = Helpers.GetDefaultFacilityId();
            int corporateid = Helpers.GetSysAdminCorporateID();

            // Check if Model is not null 
            if (model != null)
            {
                model.CorporateId = corporateid;
                model.FacilityId = facilityid;
                model.ExternalValue1 = model.ExternalValue1.Split(':')[1].Replace(")", string.Empty);
                model.ExternalValue2 = model.ExternalValue2.Split(':')[1].Replace(")", string.Empty);
                if (model.Id > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDate;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDate;
                }

                // Call the AddFacilityDepartment Method to Add / Update current FacilityDepartment
                list = _service.SaveFacilityDepartment(model);
            }

            // Pass the ActionResult with List of FacilityDepartmentViewModel object to Partial View FacilityDepartmentList
            return this.PartialView(PartialViews.FacilityDepartmentList, list);
        }

        #endregion
    }
}