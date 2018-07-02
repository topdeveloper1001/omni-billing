using System;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using BillingSystem.Common;
using System.Web.Mvc;
using BillingSystem.Model;
using System.Linq;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class McContractController : BaseController
    {
        private readonly IMcContractService _service;
        private readonly IMCRulesTableService _mrService;

        public McContractController(IMcContractService service, IMCRulesTableService mrService)
        {
            _service = service;
            _mrService = mrService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var list = _service.GetManagedCareByFacility(corporateId, facilityId, Helpers.GetLoggedInUserId());
            var mcContractView = new McContractView
            {
                ContractList = list.ToList(),
                CurrentContract = new MCContract { BCIsActive = true },
                McRuleStepView =
                    new MCRulesTableView()
                    {
                        CurrentMCRulesTable =
                            new MCRulesTable()
                            {
                                IsActive = true,
                            },
                        MCRulesTableList = new List<MCRulesTableCustomModel>()
                    },
                CurrentMCOrderCodeRates = new MCOrderCodeRates(),
                MCOrderCodeRatesList = new List<MCOrderCodeRatesCustomModel>()
            };
            return View(mcContractView);
        }

        /// <summary>
        /// Saves the mc contract.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult SaveMcContract(MCContract model)
        {
            if (model.MCContractID > 0)
            {
                model.BCModifiedBy = Helpers.GetLoggedInUserId();
                model.BCModifiedDate = Helpers.GetInvariantCultureDateTime();
            }
            else
            {
                model.BCCreatedBy = Helpers.GetLoggedInUserId();
                model.BCCreatedDate = Helpers.GetInvariantCultureDateTime();
            }

            model.CorporateId = Helpers.GetDefaultCorporateId();
            model.FacilityId = Helpers.GetDefaultFacilityId();

            var list = _service.SaveContract(model);
            return PartialView(PartialViews.McContractListView, list);
        }

        /// <summary>
        /// Gets the mc contract detail.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetMcContractDetail(int id)
        {
            var model = _service.GetMcContractDetail(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the mc contract.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteMcContract(int id)
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var result = _service.DeleteContract(id, corporateId, facilityId, Helpers.GetLoggedInUserId());
            return PartialView(PartialViews.McContractListView, result);
        }

        /// <summary>
        /// Mcs the rule step view.
        /// </summary>
        /// <param name="McContractID">The mc contract identifier.</param>
        /// <returns></returns>
        public ActionResult MCRuleStepView(int? McContractID)
        {
            var mcruleStepList = _mrService.GetMcRulesListByRuleSetId(Convert.ToInt32(McContractID));

            var mcruleStepView = new MCRulesTableView
            {
                MCRulesTableList = mcruleStepList,
                CurrentMCRulesTable = new MCRulesTable() { IsActive = true, RuleSetNumber = McContractID }
            };

            return View(mcruleStepView);
        }

        /// <summary>
        /// Get the details of the current RuleMaster in the view model by ID
        /// </summary>
        /// <param name="rmId">The rm identifier.</param>
        /// <returns></returns>
        public ActionResult GetRuleMasterById(int rmId)
        {
            //Call the AddRuleMaster Method to Add / Update current RuleMaster
            var currentRuleMaster = _service.GetMcContractDetail(rmId);
            return Json(currentRuleMaster, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Gets the mc overview string.
        /// </summary>
        /// <param name="mcCode">The mc code.</param>
        /// <returns></returns>
        public ActionResult GetMcOverviewString(int mcCode)
        {
            var mcruleStepList = _service.GetMCOverview(Convert.ToInt32(mcCode));
            return Json(mcruleStepList);
        }

        /// <summary>
        /// Gets the mc contracts.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMcContracts()
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var list = _service.GetManagedCareByFacility(corporateId, facilityId, Helpers.GetLoggedInUserId());
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
