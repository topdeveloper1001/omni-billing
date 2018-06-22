using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using WebGrease;

namespace BillingSystem.Controllers
{
    public class MCRulesTableController : BaseController
    {
        /// <summary>
        /// Get the details of the MCRulesTable View in the Model MCRulesTable such as MCRulesTableList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model MCRulesTable to be passed to View MCRulesTable
        /// </returns>
        public ActionResult MCRulesTableMain()
        {
            //Initialize the MCRulesTable BAL object
            using (var bal = new MCRulesTableBal())
            {
                //Get the Entity list
                var list = bal.GetMCRulesTableList();

                //Intialize the View Model i.e. MCRulesTableView which is binded to Main View Index.cshtml under MCRulesTable
                var viewModel = new MCRulesTableView
                {
                    MCRulesTableList = list,
                    CurrentMCRulesTable = new MCRulesTable()
                };

                //Pass the View Model in ActionResult to View MCRulesTable
                return View(viewModel);
            }
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
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<MCRulesTableCustomModel>();

            //Check if Model is not null 
            if (model != null)
            {
                using (var bal = new MCRulesTableBal())
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
                    list = bal.SaveMCRulesTable(model);
                    //list = bal.GetMCRulesTableList();
                }
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
            using (var bal = new MCRulesTableBal())
            {
                //Call the AddMCRulesTable Method to Add / Update current MCRulesTable
                var current = bal.GetMCRulesTableByID(id);
                //Pass the ActionResult with the current MCRulesTableViewModel object as model to PartialView MCRulesTableAddEdit
                return Json(current, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete the current MCRulesTable based on the MCRulesTable ID passed in the MCRulesTableModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteMCRulesTable(int id)
        {
            using (var bal = new MCRulesTableBal())
            {
                var corporateId = Helpers.GetDefaultCorporateId();
                var facilityId = Helpers.GetDefaultFacilityId();
                var result = bal.DeleteMCRulesTable(id, corporateId, facilityId);
                return Json(true);
            }
        }

        /// <summary>
        /// Maximums the rule step number.
        /// </summary>
        /// <param name="RuleSetNumber">The rule set number.</param>
        /// <returns></returns>
        public ActionResult MaxRuleStepNumber(int RuleSetNumber)
        {
            using (var mcruleStepBal = new MCRulesTableBal())
            {
                var mcrulestepMaxNumber = mcruleStepBal.GetMaxRuleStepNumber(RuleSetNumber);
                return Json(mcrulestepMaxNumber, JsonRequestBehavior.AllowGet);
            }   
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
            var mcruleStepBal = new MCRulesTableBal();
            var mcContractBal = new McContractBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var mcContractobj = mcContractBal.GetMcContractDetail(McContractID);
            //Get the Entity list
            var mcruleStepList = mcruleStepBal.GetMcRulesListByRuleSetId(Convert.ToInt32(mcContractobj.MCCode));
            //Pass the ActionResult with List of RuleStepViewModel object to Partial View RuleStepList
            return PartialView(PartialViews.MCRulesTableList, mcruleStepList);
        }

        public ActionResult BindRuleStepListObj(int McContractID)
        {
            var mcruleStepBal = new MCRulesTableBal();
            //Get the Entity list
            var mcruleStepList = mcruleStepBal.GetMcRulesListByRuleSetId(McContractID);
            //Pass the ActionResult with List of RuleStepViewModel object to Partial View RuleStepList
            return PartialView(PartialViews.MCRulesTableList, mcruleStepList);
        }
    }
}
