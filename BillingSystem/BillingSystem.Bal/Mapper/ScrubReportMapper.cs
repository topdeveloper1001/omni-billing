// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrubReportMapper.cs" company="Spadez ">
//   Omni healthCare
// </copyright>
// <summary>
//   Defines the ScrubReportMapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.Mapper
{
    using System;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The scrub report mapper.
    /// </summary>
    public class ScrubReportMapper : Mapper<ScrubReport, ScrubReportCustomModel>
    {
        /// <summary>
        /// The map model to view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ScrubReportCustomModel"/>.
        /// </returns>
        public override ScrubReportCustomModel MapModelToViewModel(ScrubReport model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new GlobalCodeBal())
                {
                    var ruleStep = bal.GetRuleStepDetailsById(Convert.ToInt32(vm.RuleStepID));
                    var ruleMaster = bal.GetRuleMasterDetailsById(Convert.ToInt32(vm.RuleMasterID));
                    var rhsTooltip = ruleStep.RHSC;
                    var lhsToolTip = ruleStep.LHSC;
                    var ruleCode = ruleMaster.RuleCode + "  (Rule Step number: " + ruleStep.RuleStepNumber + " )";

                    // Case for Direct (3) and Query String (4) Values
                    if (string.IsNullOrEmpty(lhsToolTip))
                    {
                        lhsToolTip = "Direct Value Entered by User";
                    }

                    // Case for Direct (3) and Query String (4) Values
                    if (string.IsNullOrEmpty(rhsTooltip))
                    {
                        rhsTooltip = ruleStep.RHSFrom == 4
                                         ? "Calculated Value based on Custom-Query"
                                         : "Direct Value Entered by User";
                    }

                    // Case for Count Function Type (Column: QueryFunctionType in RuleStep Table)
                    if (!string.IsNullOrEmpty(ruleStep.QueryFunctionType)
                        && ruleStep.QueryFunctionType.Trim().Equals("2"))
                    {
                        lhsToolTip = "Calculated Value based on Custom-Query";
                    }

                    vm.CompareTypeText = bal.GetNameByGlobalCodeValue(Convert.ToString(vm.CompareType), Convert.ToString((int)GlobalCodeCategoryValue.DataComparer));
                    vm.RuleMasterDesc = ruleStep.RuleStepDescription;
                    vm.RuleStepDesc = bal.GetDescByRuleStepId(Convert.ToInt32(vm.RuleStepID));
                    vm.ErrorText = bal.GetDescByErrorId(Convert.ToInt32(vm.ErrorID));
                    vm.LhsTooltip = lhsToolTip;
                    vm.RhsTooltip = rhsTooltip;
                    vm.ConStartDesc = bal.GetNameByGlobalCodeValue(Convert.ToString(model.ConStart), Convert.ToString((int)GlobalCodeCategoryValue.ConditionStart));
                    vm.ConEndDesc = bal.GetNameByGlobalCodeValue(Convert.ToString(model.ConEnd), Convert.ToString((int)GlobalCodeCategoryValue.ConditionEnd));
                    vm.RuleStepsValue = ruleCode;
                }
            }

            return vm;
        }

    }
}
