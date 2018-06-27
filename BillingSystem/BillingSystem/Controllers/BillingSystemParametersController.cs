﻿using BillingSystem.Models;
using BillingSystem.Common;
using System.Web.Mvc;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class BillingSystemParametersController : BaseController
    {
        private readonly IBillingSystemParametersService _service;
        /// <summary>
        /// Get the details of the BillingSystemParameters View in the Model BillingSystemParameters such as BillingSystemParametersList, list of countries etc.
        /// </summary>
        /// <returns>returns the actionresult in the form of current object of the Model BillingSystemParameters to be passed to View BillingSystemParameters</returns>
        public ActionResult Index()
        {
            //Intialize the View Model i.e. BillingSystemParametersView which is binded to Main View Index.cshtml under BillingSystemParameters
            var viewModel = new BillingSystemParametersView
            {
                //BillingSystemParametersList = list,
                CurrentBillingSystemParameters =
                    new BillingSystemParameters
                    {
                        IsActive = true,
                        CorporateId = Helpers.GetSysAdminCorporateID(),
                        Id = 0
                    }
            };

            //Pass the View Model in ActionResult to View BillingSystemParameters
            return View(viewModel);
        }

        /// <summary>
        /// Add New or Update the BillingSystemParameters based on if we pass the BillingSystemParameters ID in the BillingSystemParametersViewModel object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns the newly added or updated ID of BillingSystemParameters row</returns>
        public JsonResult SaveBillingSystemParameters(BillingSystemParameters model)
        {
            var id = -1;
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();

            //Check if Model is not null
            if (model != null)
            {


                if (model.Id > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDate;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDate;
                    //model.CorporateId = model.CorporateId;
                    model.IsActive = true;
                }

                //Call the AddBillingSystemParameters Method to Add / Update current BillingSystemParameters
                id = _service.SaveBillingSystemParameters(model);

            }
            return Json(id);
        }

        /// <summary>
        /// Get the details of the current BillingSystemParameters in the view model by ID 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBillingSystemParametersDetails(string facilityNumber, int corporateId)
        {
            //Call the AddBillingSystemParameters Method to Add / Update current BillingSystemParameters
            var data = _service.GetDetailsByCorporateAndFacility(corporateId, facilityNumber);

            var jsonResult = new
            {
                data.Id,
                EffectiveDate = data.EffectiveDate.GetShortDateString1(),
                EndDate = data.EndDate.GetShortDateString1(),
                OupatientCloseBillsTime = data.OupatientCloseBillsTime.GetTimeStringFromDateTime(),
                data.FacilityNumber,
                data.BillHoldDays,
                data.ARGLacct,
                data.MgdCareGLacct,
                data.BadDebtGLacct,
                data.SmallBalanceGLacct,
                data.SmallBalanceAmount,
                data.SmallBalanceWriteoffDays,
                data.ERCloseBillsHours,
                data.IsActive,
                data.ExternalValue1,
                data.ExternalValue2,
                data.ExternalValue3,
                data.ExternalValue4,
                data.CorporateId,
                data.CPTTableNumber,
                data.ServiceCodeTableNumber,
                data.DrugTableNumber,
                data.DRGTableNumber,
                data.DiagnosisTableNumber,
                data.HCPCSTableNumber,
                data.BillEditRuleTableNumber,
                data.DefaultCountry
            };

            //Pass the ActionResult with the current BillingSystemParametersViewModel object as model to PartialView BillingSystemParametersAddEdit
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }
    }
}
