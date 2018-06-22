using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class RuleMasterView
    {
        public RuleMaster CurrentRuleMaster { get; set; }
        public List<RuleMasterCustomModel> RuleMasterList { get; set; }
        public RuleStepView RuleStepView { get; set; }
    }
}
