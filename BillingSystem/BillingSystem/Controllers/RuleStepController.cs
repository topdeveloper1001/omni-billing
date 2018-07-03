using BillingSystem.Common.Common;
using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class RuleStepController : BaseController
    {
        private readonly IRuleStepService _service;
        private readonly IRuleMasterService _rmService;
        private readonly IErrorMasterService _erService;
        private readonly IGlobalCodeService _gService;

        public RuleStepController(IRuleStepService service, IRuleMasterService rmService, IErrorMasterService erService, IGlobalCodeService gService)
        {
            _service = service;
            _rmService = rmService;
            _erService = erService;
            _gService = gService;
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
            // Get the Entity list
            var ruleStepList = _service.GetRuleStepsList(Convert.ToInt32(ruleMasterId));
            var rulemasterObj = _rmService.GetRuleMasterById(Convert.ToInt32(ruleMasterId), Helpers.DefaultBillEditRuleTableNumber);

            // Intialize the View Model i.e. RuleStepView which is binded to Main View Index.cshtml under RuleStep
            var ruleStepView = new RuleStepView
            {
                RuleStepList = ruleStepList,
                CurrentRuleStep =
                                           new RuleStep
                                           {
                                               IsActive = true,
                                               RuleMasterID = ruleMasterId,
                                               CreatedBy = Helpers.GetLoggedInUserId(),
                                               CreatedDate = Helpers.GetInvariantCultureDateTime(),
                                               EffectiveEndDate = rulemasterObj.EffectiveEndDate,
                                               EffectiveStartDate =
                                                       rulemasterObj.EffectiveStartDate
                                           },
                RuleMasterStepdesc =
                                           "( " + rulemasterObj.RuleMasterID + " - "
                                           + rulemasterObj.RuleDescription + " )"
            };

            // Pass the View Model in ActionResult to View RuleStep
            return View(ruleStepView);
        }

        /// <summary>
        /// Bind all the RuleStep list
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns>
        /// action result with the partial view containing the RuleStep list object
        /// </returns>
        [HttpPost]
        public ActionResult BindRuleStepList(int ruleMasterId)
        {
            var ruleStepList = _service.GetRuleStepsList(ruleMasterId);
            return PartialView(PartialViews.RuleStepList, ruleStepList);
        }

        /// <summary>
        /// Add New or Update the RuleStep based on if we pass the RuleStep ID in the RuleStepViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of RuleStep row
        /// </returns>
        public ActionResult SaveRuleStep(RuleStep model)
        {
            // Initialize the newId variable 
            var newId = -1;
            var list = new List<RuleStepCustomModel>();
            var userId = Helpers.GetLoggedInUserId();
            var ruleMasterStepObj = _rmService.GetRuleMasterCustomModelById(model.RuleMasterID);

            // Check if Model is not null 
            if (model != null)
            {
                if (model.RuleStepID > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }
                if (model.RHSFrom == 2 && model.DataType == Convert.ToInt32(RuleStepDataType.STRING))
                {
                    if (!string.IsNullOrEmpty(model.DirectValue) && model.DirectValue.Contains(","))
                    {
                        var splitDirectvalue = model.DirectValue.Replace(",", "','");
                        splitDirectvalue = "'" + splitDirectvalue + "'";
                        model.DirectValue = splitDirectvalue;
                    }
                }

                model.EffectiveStartDate = ruleMasterStepObj.EffectiveStartDate;
                model.EffectiveEndDate = ruleMasterStepObj.EffectiveEndDate;

                // Call the AddRuleStep Method to Add / Update current RuleStep
                newId = _service.SaveRuleStep(model);
                list = _service.GetRuleStepsList(Convert.ToInt32(model.RuleMasterID));
            }

            return PartialView(PartialViews.RuleStepList, list);
        }

        /// <summary>
        /// Get the details of the current RuleStep in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetRuleStep(int id)
        {
            // Call the AddRuleStep Method to Add / Update current RuleStep
            var currentRuleStep = _service.GetRuleStepByID(id);
            if (currentRuleStep.RHSFrom == 2 && currentRuleStep.DataType == Convert.ToInt32(RuleStepDataType.STRING))
            {
                if (!string.IsNullOrEmpty(currentRuleStep.DirectValue) && currentRuleStep.DirectValue.Contains("','"))
                {
                    var splitDirectvalue = currentRuleStep.DirectValue.Replace("','", ",");
                    splitDirectvalue = splitDirectvalue.Replace("'", string.Empty);
                    currentRuleStep.DirectValue = splitDirectvalue;
                }
            }

            // Pass the ActionResult with the current RuleStepViewModel object as model to PartialView RuleStepAddEdit
            return PartialView(PartialViews.RuleStepAddEdit, currentRuleStep);
        }
        /// <summary>
        /// Delete the current RuleStep based on the RuleStep ID passed in the RuleStepModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteRuleStep(int id)
        {
            var currentRuleStep = _service.GetRuleStepByID(id);

            var result = _service.DeleteRuleStep(currentRuleStep);
            return Json(result);
        }

        /// <summary>
        /// Reset the RuleStep View Model and pass it to RuleStepAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetRuleStepForm()
        {
            // Intialize the new object of RuleStep ViewModel
            var ruleStepViewModel = new Model.RuleStep();

            // Pass the View Model as RuleStepViewModel to PartialView RuleStepAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.RuleStepAddEdit, ruleStepViewModel);
        }

        /// <summary>
        /// Gets the rule step dropdown data.
        /// </summary>
        /// <param name="startRange">The start range.</param>
        /// <param name="endRange">The end range.</param>
        /// <returns></returns>
        //public JsonResult GetRuleStepDropdownData(int startRange, int endRange)
        //{
        //    var lst = new List<string>() {"1", "2", "4", "5", "6", "7"};
        //    using (var _service = new GlobalCodeBal())
        //    {
        //        var globalCodes = _service.GetGlobalCodesByCategoriesRange(startRange, endRange);
        //        if (globalCodes.Count > 0)
        //        {
        //            var list = new List<DropdownListData>();
        //            list.AddRange(
        //                globalCodes.Select(
        //                    item =>
        //                        new DropdownListData
        //                        {
        //                            Text = item.GlobalCodes.GlobalCodeName,
        //                            Value = Convert.ToString(item.GlobalCodes.GlobalCodeValue),
        //                            ExternalValue1 = item.GlobalCodes.GlobalCodeCategoryValue.Trim(),
        //                            SortOrder = item.GlobalCodes.SortOrder,
        //                        }));
        //            var dataTypes = list.Where(a => a.ExternalValue1.Trim().Contains("14000")).OrderBy(n => n.SortOrder).ToList();
        //            var compareTypes = list.Where(a => a.ExternalValue1.Trim().Contains("14100")).OrderBy(n => n.SortOrder).ToList();
        //            var tList = list.Where(a => a.ExternalValue1.Trim().Contains("14200")).OrderBy(n => n.SortOrder).ToList();
        //            var conditionStartList = list.Where(a => a.ExternalValue1.Trim().Contains("14101")).ToList();
        //            var conditionEndList = list.Where(a => a.ExternalValue1.Trim().Contains("14102")).ToList();
        //            var expCompareTypes = compareTypes.Where(i => i.Value != null && lst.Contains(i.Value)).ToList();
        //            var jsonResult = new
        //            {
        //                DataTypes = dataTypes,
        //                CompareTypes = compareTypes,
        //                TablesList = tList,
        //                conditionStartList = conditionStartList,
        //                conditionEndList = conditionEndList,
        //                ExpressionTypes = expCompareTypes
        //            };
        //            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return Json(null);
        //}
        public JsonResult GetRuleStepDropdownData()
        {
            var list = new List<DropdownListData>();

            var categories = new List<string> { "14000", "14100", "14200", "14101", "14102" };
            var lst = new List<string>() { "1", "2", "4", "5", "6", "7" };
            list = _gService.GetListByCategoriesRange(categories);
            var dataTypes = list.Where(a => a.ExternalValue1.Trim().Contains("14000")).OrderBy(n => n.Text).ToList();
            var compareTypes = list.Where(a => a.ExternalValue1.Trim().Contains("14100")).OrderByDescending(n => n.Text).ToList();
            var tList = list.Where(a => a.ExternalValue1.Trim().Contains("14200")).OrderBy(n => n.Text).ToList();
            var conditionStartList = list.Where(a => a.ExternalValue1.Trim().Contains("14101")).ToList();
            var conditionEndList = list.Where(a => a.ExternalValue1.Trim().Contains("14102")).ToList();
            var expCompareTypes = compareTypes.Where(i => i.Value != null && lst.Contains(i.Value)).ToList();
            var jsonResult = new
            {
                DataTypes = dataTypes,
                CompareTypes = compareTypes,
                TablesList = tList,
                conditionStartList = conditionStartList,
                conditionEndList = conditionEndList,
                ExpressionTypes = expCompareTypes
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Gets the errors list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetErrorsList()
        {
            var list = new List<DropdownListData>();
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var errorsList = _erService.GetErrorListByCorporateAndFacilityId(corporateId, facilityId, false);
            if (errorsList.Count > 0)
                list.AddRange(errorsList.Select(item => new DropdownListData { Text = string.Format("{0} - {1}", item.ErrorCode, item.ErrorDescription), Value = Convert.ToString(item.ErrorMasterID), ExternalValue1 = item.ErrorDescription }));
            return Json(list);
        }

        /// <summary>
        /// Gets the rule master by identifier.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        public ActionResult GetRuleMasterById(int ruleMasterId)
        {
            var ruleStepList = _rmService.GetRuleMasterCustomModelById(ruleMasterId);

            //Pass the ActionResult with List of RuleStepViewModel object to Partial View RuleStepList
            return Json(ruleStepList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the rule step number.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        public ActionResult GetRuleStepNumber(int ruleMasterId)
        {
            var rulestepMaxNumber = _service.GetMaxRuleStepNumber(ruleMasterId);

            return Json(rulestepMaxNumber, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the preview rule step result.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        public ActionResult GetPreviewRuleStepResult(int ruleMasterId)
        {
            var ruleStepPreviewString = _service.GetPreviewRuleStepResult(ruleMasterId);
            return Json(ruleStepPreviewString, JsonRequestBehavior.AllowGet);
        }
    }
}
