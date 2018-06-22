using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    public class RuleMasterController : BaseController
    {
        /// <summary>
        /// Get the details of the RuleMaster View in the Model RuleMaster such as RuleMasterList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model RuleMaster to be passed to View RuleMaster
        /// </returns>
        public ActionResult Index()
        {
            //Initialize the RuleMaster BAL object
            var ruleMasterBal = new RuleMasterBal(Helpers.DefaultBillEditRuleTableNumber);
            var ruleMasterList = ruleMasterBal.GetRuleMasterList();

            //Intialize the View Model i.e. RuleMasterView which is binded to Main View Index.cshtml under RuleMaster
            var ruleMasterView = new RuleMasterView
            {
                RuleMasterList = ruleMasterList,
                CurrentRuleMaster = new RuleMaster
                {
                    IsActive = true,
                    EffectiveEndDate = CurrentDateTime,
                    EffectiveStartDate = CurrentDateTime
                },
                RuleStepView = new RuleStepView
                {
                    CurrentRuleStep = new RuleStep
                    {
                        IsActive = true,
                        EffectiveEndDate = CurrentDateTime,
                        EffectiveStartDate = CurrentDateTime
                    },
                    RuleStepList = new List<RuleStepCustomModel>()
                }
            };

            //Pass the View Model in ActionResult to View RuleMaster
            return View(ruleMasterView);
        }

        /// <summary>
        /// Bind all the RuleMaster list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the RuleMaster list object
        /// </returns>
        //[HttpPost]
        //public ActionResult BindRuleMasterList(string tn, bool notActive =  false)
        //{
        //    //Initialize the RuleMaster BAL object
        //    using (var ruleMasterBal = new RuleMasterBal(!string.IsNullOrEmpty(tn) ? tn : Helpers.DefaultBillEditRuleTableNumber))
        //    {
        //        //Get the Rule Master list
        //        var ruleMasterList = ruleMasterBal.GetRuleMasterList(notActive);

        //        //Pass the ActionResult with List of RuleMasterViewModel object to Partial View RuleMasterList
        //        return PartialView(PartialViews.RuleMasterList, ruleMasterList);
        //    }
        //}
        [HttpPost]
        public ActionResult BindRuleMasterList(string tn, bool notActive = false)
        {
            //Initialize the RuleMaster BAL object
            using (var ruleMasterBal = new RuleMasterBal(!string.IsNullOrEmpty(tn) ? tn : Helpers.DefaultBillEditRuleTableNumber))
            {
                //Get the Rule Master list
                var ruleMasterList = ruleMasterBal.GetRuleMasterList(notActive);

                //Pass the ActionResult with List of RuleMasterViewModel object to Partial View RuleMasterList
                return PartialView(PartialViews.RuleMasterList, ruleMasterList);
            }
        }

     
        /// <summary>
        /// Add New or Update the RuleMaster based on if we pass the RuleMaster ID in the RuleMasterViewModel object.
        /// </summary>
        /// <param name="model">pass the details of RuleMaster in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of RuleMaster row
        /// </returns>
        public ActionResult SaveRuleMaster(RuleMaster model)
        {
            //Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            //Check if RuleMasterViewModel 
            if (model != null)
            {
                using (var ruleMasterBal = new RuleMasterBal(Helpers.DefaultBillEditRuleTableNumber))
                {
                    if (model.RuleMasterID > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = currentDateTime;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = currentDateTime;
                    }
                    //Call the AddRuleMaster Method to Add / Update current RuleMaster
                    newId = ruleMasterBal.AddUptdateRuleMaster(model);
                }
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current RuleMaster in the view model by ID
        /// </summary>
        /// <param name="rmId">The rm identifier.</param>
        /// <returns></returns>
        public ActionResult GetRuleMaster(int rmId)
        {
            using (var ruleMasterBal = new RuleMasterBal(Helpers.DefaultBillEditRuleTableNumber))
            {
                //Call the AddRuleMaster Method to Add / Update current RuleMaster
                var currentRuleMaster = ruleMasterBal.GetRuleMasterById(rmId);

                //Pass the ActionResult with the current RuleMasterViewModel object as model to PartialView RuleMasterAddEdit
                return PartialView(PartialViews.RuleMasterAddEdit, currentRuleMaster);
            }
        }

        /// <summary>
        /// Delete the current RuleMaster based on the RuleMaster ID passed in the RuleMasterModel
        /// </summary>
        /// <param name="RuleMasterID">The rule master identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteRuleMaster(int RuleMasterID)
        {
            using (var ruleMasterBal = new RuleMasterBal(Helpers.DefaultBillEditRuleTableNumber))
            {
                //Get RuleMaster model object by current RuleMaster ID
                var currentRuleMaster = ruleMasterBal.GetRuleMasterById(RuleMasterID);

                var result = ruleMasterBal.DeleteRuleMaster(currentRuleMaster);
                //return deleted ID of current DashboardBudget as Json Result to the Ajax Call.
                return Json(result);
            }
        }

        /// <summary>
        /// Reset the RuleMaster View Model and pass it to RuleMasterAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetRuleMasterForm()
        {
            //Intialize the new object of RuleMaster ViewModel
            var ruleMasterViewModel = new Model.RuleMaster();

            //Pass the View Model as RuleMasterViewModel to PartialView RuleMasterAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.RuleMasterAddEdit, ruleMasterViewModel);
        }

        /// <summary>
        /// Get the details of the RuleStep View in the Model RuleStep such as RuleStepList, list of countries etc.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model RuleStep to be passed to View RuleStep
        /// </returns>
        public ActionResult RuleStepView(int? ruleMasterId)
        {
            //Initialize the RuleStep BAL object
            var ruleStepBal = new RuleStepBal();

            //Get the Entity list
            var ruleStepList = ruleStepBal.GetRuleStepsList(Convert.ToInt32(ruleMasterId));

            //Intialize the View Model i.e. RuleStepView which is binded to Main View Index.cshtml under RuleStep
            var ruleStepView = new RuleStepView
            {
                RuleStepList = ruleStepList,
                CurrentRuleStep = new RuleStep { IsActive = true, RuleMasterID = ruleMasterId }
            };

            //Pass the View Model in ActionResult to View RuleStep
            return View(ruleStepView);
        }
    }
}
