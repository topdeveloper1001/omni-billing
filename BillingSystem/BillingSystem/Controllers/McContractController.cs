using System;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using BillingSystem.Common;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using System.Linq;

namespace BillingSystem.Controllers
{
    public class McContractController : BaseController
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var bal = new McContractBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var list = bal.GetManagedCareByFacility(corporateId, facilityId, Helpers.GetLoggedInUserId());
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
            using (var bal = new McContractBal())
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

                var list = bal.SaveContract(model);
                return PartialView(PartialViews.McContractListView, list);
            }
        }

        /// <summary>
        /// Gets the mc contract detail.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetMcContractDetail(int id)
        {
            using (var bal = new McContractBal())
            {
                var model = bal.GetMcContractDetail(id);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Deletes the mc contract.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteMcContract(int id)
        {
            using (var bal = new McContractBal())
            {
                var corporateId = Helpers.GetDefaultCorporateId();
                var facilityId = Helpers.GetDefaultFacilityId();
                var result = bal.DeleteContract(id, corporateId, facilityId, Helpers.GetLoggedInUserId());
                return PartialView(PartialViews.McContractListView, result);
            }
        }

        /// <summary>
        /// Mcs the rule step view.
        /// </summary>
        /// <param name="McContractID">The mc contract identifier.</param>
        /// <returns></returns>
        public ActionResult MCRuleStepView(int? McContractID)
        {
            //Initialize the RuleStep BAL object
            var mcruleStepBal = new MCRulesTableBal();

            //Get the Entity list
            var mcruleStepList = mcruleStepBal.GetMcRulesListByRuleSetId(Convert.ToInt32(McContractID));

            //Intialize the View Model i.e. RuleStepView which is binded to Main View Index.cshtml under RuleStep
            var mcruleStepView = new MCRulesTableView
            {
                MCRulesTableList = mcruleStepList,
                CurrentMCRulesTable = new MCRulesTable() { IsActive = true, RuleSetNumber = McContractID }
            };

            //Pass the View Model in ActionResult to View RuleStep
            return View(mcruleStepView);
        }

        /// <summary>
        /// Get the details of the current RuleMaster in the view model by ID
        /// </summary>
        /// <param name="rmId">The rm identifier.</param>
        /// <returns></returns>
        public ActionResult GetRuleMasterById(int rmId)
        {
            using (var ruleMasterBal = new McContractBal())
            {
                //Call the AddRuleMaster Method to Add / Update current RuleMaster
                var currentRuleMaster = ruleMasterBal.GetMcContractDetail(rmId);

                //Pass the ActionResult with the current RuleMasterViewModel object as model to PartialView RuleMasterAddEdit
                return Json(currentRuleMaster, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the mc overview string.
        /// </summary>
        /// <param name="mcCode">The mc code.</param>
        /// <returns></returns>
        public ActionResult GetMcOverviewString(int mcCode)
        {
            var mcruleStepBal = new McContractBal();
            //Get the Entity list
            var mcruleStepList = mcruleStepBal.GetMCOverview(Convert.ToInt32(mcCode));
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
            var bal = new McContractBal();
            var list = bal.GetManagedCareByFacility(corporateId, facilityId, Helpers.GetLoggedInUserId());
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
