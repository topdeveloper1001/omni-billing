using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IRuleStepService
    {
        int DeleteRuleStep(RuleStep model);
        string GetErrorCodeByErrorId(int errorId);
        int GetMaxRuleStepNumber(int ruleStepMasterId);
        string GetPreviewRuleStepResult(int ruleMasterId);
        RuleStep GetRuleStepByID(int? ruleStepId);
        List<RuleStepCustomModel> GetRuleStepsList(int ruleMasterId);
        int SaveRuleStep(RuleStep model);
    }
}