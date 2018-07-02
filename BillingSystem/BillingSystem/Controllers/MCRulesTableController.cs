using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class MCRulesTableController : BaseController
    {
        private readonly IMCRulesTableService _service;
        private readonly IFacilityService _fService;
        private readonly IMcContractService _mcService;

        public MCRulesTableController(IMCRulesTableService service, IFacilityService fService, IMcContractService mcService)
        {
            _service = service;
            _fService = fService;
            _mcService = mcService;
        }


        /// <summary>
        /// Get the details of the MCRulesTable View in the Model MCRulesTable such as MCRulesTableList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model MCRulesTable to be passed to View MCRulesTable
        /// </returns>
        public ActionResult MCRulesTableMain()
        {
            var list = _service.GetMCRulesTableList();
            var viewModel = new MCRulesTableView
            {
                MCRulesTableList = list,
                CurrentMCRulesTable = new MCRulesTable()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Add New or Update the MCRulesTable based on if we pass the MCRulesTable ID in the MCRulesTableViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of MCRulesTable row
        /// </returns>
        public ActionResult SaveMCRulesTable(MCRulesTable model)
        {
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = _fService.GetInvariantCultureDateTime(Helpers.GetDefaultFacilityId());
            var list = new List<MCRulesTableCustomModel>();

            //Check if Model is not null 
            if (model != null)
            {
                if (model.ManagedCareRuleId > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDate;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDate;
                }

                //Call the AddMCRulesTable Method to Add / Update current MCRulesTable
                list = _service.SaveMCRulesTable(model);
            }

            //Pass the ActionResult with List of MCRulesTableViewModel object to Partial View MCRulesTableList
            return PartialView(PartialViews.MCRulesTableList, list);
        }

        /// <summary>
        /// Get the details of the current MCRulesTable in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetMCRulesTableDetails(int id)
        {
            var current = _service.GetMCRulesTableByID(id);
            return Json(current, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete the current MCRulesTable based on the MCRulesTable ID passed in the MCRulesTableModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteMCRulesTable(int id)
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var result = _service.DeleteMCRulesTable(id, corporateId, facilityId);
            return Json(true);
        }

        /// <summary>
        /// Maximums the rule step number.
        /// </summary>
        /// <param name="RuleSetNumber">The rule set number.</param>
        /// <returns></returns>
        public ActionResult MaxRuleStepNumber(int RuleSetNumber)
        {
            var mcrulestepMaxNumber = _service.GetMaxRuleStepNumber(RuleSetNumber);
            return Json(mcrulestepMaxNumber, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Bind all the RuleStep list
        /// </summary>
        /// <param name="McContractID">The mc contract identifier.</param>
        /// <returns>
        /// action result with the partial view containing the RuleStep list object
        /// </returns>
        [HttpPost]
        public ActionResult BindRuleStepList(int McContractID)
        {
            var mcContractobj = _mcService.GetMcContractDetail(McContractID);
            //Get the Entity list
            var mcruleStepList = _service.GetMcRulesListByRuleSetId(Convert.ToInt32(mcContractobj.MCCode));
            //Pass the ActionResult with List of RuleStepViewModel object to Partial View RuleStepList
            return PartialView(PartialViews.MCRulesTableList, mcruleStepList);
        }

        public ActionResult BindRuleStepListObj(int McContractID)
        {
            var mcruleStepList = _service.GetMcRulesListByRuleSetId(McContractID);
            return PartialView(PartialViews.MCRulesTableList, mcruleStepList);
        }
    }
}
