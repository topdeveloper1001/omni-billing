﻿using System.Web.Helpers;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Models;
using BillingSystem.Model;
using System;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class InsurancePolicesController : BaseController
    {
        private readonly IInsurancePolicesService _service;
        private readonly IMcContractService _mcService;

        public InsurancePolicesController(IInsurancePolicesService service, IMcContractService mcService)
        {
            _service = service;
            _mcService = mcService;
        }


        /// <summary>
        /// Get the details of the InsurancePolices View in the Model InsurancePolices such as InsurancePolicesList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model InsurancePolices to be passed to View InsurancePolices
        /// </returns>
        public ActionResult InsurancePolices()
        {
            var list = _service.GetInsurancePolicyListByFacility(Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId(), Helpers.GetLoggedInUserId());
            var vm = new InsurancePolicesView
            {
                InsurancePolicesList = list,
                CurrentInsurancePolices = new InsurancePolices()
            };
            return View(vm);
        }

        public ActionResult ListView()
        {
            var vm = _service.GetInsurancePolicyListByFacility(Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId(), Helpers.GetLoggedInUserId());
            return PartialView("UserControls/_PolicesList", vm);
        }

        /// <summary>
        /// Add New or Update the InsurancePolices based on if we pass the InsurancePolices ID in the InsurancePolicesViewModel object.
        /// </summary>
        /// <param name="model">pass the details of InsurancePolices in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of InsurancePolices row
        /// </returns>
        public ActionResult SaveInsurancePolices(InsurancePolices model)
        {
            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            model.IsActive = true;
            model.IsDeleted = false;
            model.CorporateId = Helpers.GetDefaultCorporateId();
            model.FacilityId = Helpers.GetDefaultFacilityId();

            if (model.InsurancePolicyId > 0)
            {
                model.ModifiedBy = userId;
                model.ModifiedDate = currentDateTime;
            }
            else
            {
                model.CreatedBy = userId;
                model.CreatedDate = currentDateTime;
            }
            //Call the AddInsurancePolices Method to Add / Update current InsurancePolices
            var list = _service.AddUpdateInsurancePolices(model);

            //Pass the ActionResult with List of InsurancePolicesViewModel object to Partial View InsurancePolicesList
            return PartialView(PartialViews.PolicesList, list);
        }

        /// <summary>
        /// Get the details of the current InsurancePolices in the view model by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetInsurancePolicesById(int id)
        {
            var current = _service.GetInsurancePolicyById(id);

            var jsonResult = new
            {
                current.InsuranceCompanyId,
                current.InsurancePlanId,
                current.InsurancePolicyId,
                current.IsActive,
                current.IsDeleted,
                current.McContractCode,
                current.ModifiedBy,
                current.PlanName,
                current.PlanNumber,
                PolicyBeginDate = Convert.ToDateTime(current.PolicyBeginDate).ToString("d"),
                PolicyEndDate = Convert.ToDateTime(current.PolicyEndDate).ToString("d"),
                current.PolicyDescription,
                current.PolicyHolderName,
                current.PolicyNumber,
                current.PolicyName,
            };


            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete the current InsurancePolices based on the InsurancePolices ID passed in the SharedViewModel
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteInsurancePolices(int policyId)
        {
            var list = Enumerable.Empty<InsurancePolicyCustomModel>();
            var m = _service.GetInsurancePolicyById(policyId);

            if (m != null)
            {
                m.IsDeleted = true;
                m.DeletedBy = Helpers.GetLoggedInUserId();
                m.DeletedDate = Helpers.GetInvariantCultureDateTime();

                list = _service.AddUpdateInsurancePolices(m);
            }

            //Pass the ActionResult with List of InsurancePolicesViewModel object to Partial View InsurancePolicesList
            return PartialView(PartialViews.PolicesList, list);
        }

        /// <summary>
        /// Gets the insurance plan by company.
        /// </summary>
        /// <param name="companyId">The insurance company identifier.</param>
        /// <returns></returns>
        public JsonResult GetInsurancePlanByCompany(int companyId)
        {
            var list = new List<SelectListItem>();
            var list2 = _service.GetInsurancePlanByCompanyId(companyId);
            if (list2.Count > 0)
            {
                list.AddRange(list2.Select(item => new SelectListItem
                {
                    Text = item.PlanName,
                    Value = Convert.ToString(item.InsurancePlanId)
                }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Validates the policy name policy number.
        /// </summary>
        /// <param name="policyName">Name of the policy.</param>
        /// <param name="policyNumber">The policy number.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult ValidatePolicyNamePolicyNumber(string policyName, string policyNumber, int id, int insuranceCompanyId, int planId)
        {
            var result = _service.ValidatePolicyNamePolicyNumber(policyName, policyNumber, id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the policy name by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetPolicyNameById(int id)
        {
            var result = _service.GetInsurancePolicyById(id);
            return Json(result != null ? result.PolicyName : string.Empty);
        }

        public JsonResult BindMcContracts()
        {
            var list = new List<DropdownListData>();
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var data = _mcService.GetManagedCareByFacility(corporateId, facilityId, Helpers.GetLoggedInUserId());
            if (data.Any())
            {
                list.AddRange(data.Select(item => new DropdownListData
                {
                    Text = item.MCCode,
                    Value = Convert.ToString(item.MCContractID)
                }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInsurancePoliceData()
        {
            var list = _service.GetInsurancePolicyList();
            return PartialView(PartialViews.PolicesList, list);
        }
    }
}
