using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class RuleStepView
    {
     
        public RuleStep CurrentRuleStep { get; set; }
        public List<RuleStepCustomModel> RuleStepList { get; set; }
        public string RuleMasterStepdesc { get; set; }
    }
}
