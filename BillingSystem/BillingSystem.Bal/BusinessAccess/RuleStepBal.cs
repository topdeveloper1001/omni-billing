using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class RuleStepBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<RuleStepCustomModel> GetRuleStepsList(int ruleMasterId)
        {
            var list = new List<RuleStepCustomModel>();
            using (var RuleStepRep = UnitOfWork.RuleStepRepository)
            {
                var lstRuleStep = RuleStepRep.Where(a => a.IsActive != null && (bool)a.IsActive && (int)a.RuleMasterID == ruleMasterId).ToList();
                if (lstRuleStep.Count > 0)
                {
                    list.AddRange(lstRuleStep.Select(item => new RuleStepCustomModel
                    {
                        CompareType = item.CompareType,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        DataType = item.DataType,
                        DirectValue = item.DirectValue,
                        EffectiveEndDate = item.EffectiveEndDate,
                        EffectiveStartDate = item.EffectiveStartDate,
                        //ErrorCode = item.ErrorCode,
                        ErrorID = item.ErrorID,
                        ErrorType = item.ErrorType,
                        IsActive = item.IsActive,
                        LHSC = item.LHSC,
                        LHSK = item.LHSK,
                        LHST = item.LHST,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        QueryString = item.QueryString,
                        RHSC = item.RHSC,
                        RHSFrom = item.RHSFrom,
                        RHSK = item.RHSK,
                        RHST = item.RHST,
                        RuleCode = item.RuleCode,
                        RuleMasterID = item.RuleMasterID,
                        RuleStepDescription = item.RuleStepDescription,
                        RuleStepID = item.RuleStepID,
                        RuleStepNumber = item.RuleStepNumber,
                        //DataTypeText = GetNameByGlobalCodeId(Convert.ToInt32(item.DataType)),
                        DataTypeText =
                            GetNameByGlobalCodeValue(item.DataType.ToString(),
                                Convert.ToInt32(GlobalCodeCategoryValue.DataTypes).ToString()),
                        CompareTypeText =
                            GetNameByGlobalCodeValue(item.CompareType.ToString(),
                                Convert.ToInt32(GlobalCodeCategoryValue.DataComparer).ToString()),
                        ErrorCode = GetErrorCodeByErrorId(Convert.ToInt32(item.ErrorID)),
                        RuleMasterCode = GetRuleCodeByRuleMasterId(Convert.ToInt32(item.RuleMasterID)),
                        RHSFromText = GetRHSFromTextByValue(Convert.ToString(item.RHSFrom)),
                        SelectedSectionString = item.LHST + " - " + item.LHSC,
                        CompareSectionString = item.RHSFrom == 4 ? item.QueryText :
                            item.RHSFrom == 1
                                ? item.RHST + " - " + item.RHSC
                                : item.RHSFrom == 2
                                    ? item.DirectValue
                                    : item.RHSFrom == 3 ? item.DirectValue + " and  " + item.QueryString : "NA",
                        ConEnd = item.ConEnd,
                        ConStart = item.ConStart,
                        ConditionEndString =
                            GetNameByGlobalCodeValue(item.ConEnd.ToString(),
                                Convert.ToInt32(GlobalCodeCategoryValue.ConditionEnd).ToString()),
                        ConditionStartString =
                            GetNameByGlobalCodeValue(item.ConStart.ToString(),
                                Convert.ToInt32(GlobalCodeCategoryValue.ConditionStart).ToString()),
                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveRuleStep(RuleStep model)
        {
            using (var rep = UnitOfWork.RuleStepRepository)
            {
                if (model.RuleStepID > 0)
                {
                    var current = rep.GetSingle(model.RuleStepID);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(model, model.RuleStepID);
                }
                else
                    rep.Create(model);
                return model.RuleStepID;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="ruleStepId">The rule step identifier.</param>
        /// <returns></returns>
        public RuleStep GetRuleStepByID(int? ruleStepId)
        {
            using (var rep = UnitOfWork.RuleStepRepository)
            {
                var model = rep.Where(x => x.RuleStepID == ruleStepId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Gets the rule code by rule master identifier.
        /// </summary>
        /// <modifiedby-purpose>
        /// Shashank : to display the Desc with rule Code
        /// </modifiedby-purpose>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        private string GetRuleCodeByRuleMasterId(int ruleMasterId)
        {
            using (var rep = UnitOfWork.RuleMasterRepository)
            {
                var model = rep.Where(r => r.RuleMasterID == ruleMasterId).FirstOrDefault();
                return model != null ? model.RuleCode + " - " + model.RuleDescription : string.Empty;
            }
        }

        /// <summary>
        /// Gets the error code by error identifier.
        /// </summary>
        /// <modifiedby-purpose>
        /// Shashank : to display the Desc with Error Code
        /// </modifiedby-purpose>
        /// <param name="errorId">The error identifier.</param>
        /// <returns></returns>
        public string GetErrorCodeByErrorId(int errorId)
        {
            using (var rep = UnitOfWork.ErrorMasterRepository)
            {
                var model = rep.Where(r => r.ErrorMasterID == errorId).FirstOrDefault();
                return model != null ? model.ErrorCode + " - " + model.ErrorDescription : string.Empty;
            }
        }

        /// <summary>
        /// Gets the RHS from text by value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string GetRHSFromTextByValue(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var rhsFrom = (RHSFrom)Enum.Parse(typeof(RHSFrom), value);
                switch (rhsFrom)
                {
                    case RHSFrom.Table:
                        return RHSFrom.Table.ToString();
                    case RHSFrom.DirectValue:
                        return RHSFrom.DirectValue.ToString();
                    case RHSFrom.RangeValue:
                        return RHSFrom.RangeValue.ToString();
                    case RHSFrom.CustomQuery:
                        return RHSFrom.CustomQuery.ToString();
                    default:
                        break;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the maximum rule step number.
        /// </summary>
        /// <param name="ruleStepMasterId">The rule step master identifier.</param>
        /// <returns></returns>
        public int GetMaxRuleStepNumber(int ruleStepMasterId)
        {
            using (var rep = UnitOfWork.RuleStepRepository)
            {
                var model = rep.Where(x => x.RuleMasterID == ruleStepMasterId).ToList().LastOrDefault();
                return model != null ? Convert.ToInt32(model.RuleStepNumber) + 1 : 1;
            }
        }

        /// <summary>
        /// Gets the preview rule step result.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        public string GetPreviewRuleStepResult(int ruleMasterId)
        {
            using (var rep = UnitOfWork.RuleStepRepository)
            {
                var ruleStepPreviewObj = rep.GetPreviewRuleStepResult(ruleMasterId);
                var firstFileString = ruleStepPreviewObj.FirstOrDefault();
                return firstFileString != null
                    ? !string.IsNullOrEmpty(firstFileString.PreviewRule)
                        ? firstFileString.PreviewRule.Replace("/n", "<br/><br/>")
                        : ""
                    : "";
            }
        }


        /// <summary>
        /// Deletes the rule master.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int DeleteRuleStep(RuleStep model)
        {
            using (var rep = UnitOfWork.ScrubReportRepository)
            {
                var list = rep.Where(s => s.RuleStepID == model.RuleStepID).ToList();
                rep.Delete(list);
            }
            using (var rep = UnitOfWork.RuleStepRepository)
            {
                rep.Delete(model);
            }
            return model.RuleStepID;
        }
    }
}
