using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class McContractView
    {
        public MCContract CurrentContract { get; set; }
        public List<McContractCustomModel> ContractList { get; set; }
        public MCRulesTableView McRuleStepView { get; set; }
        public MCOrderCodeRates CurrentMCOrderCodeRates { get; set; }
        public List<MCOrderCodeRatesCustomModel> MCOrderCodeRatesList { get; set; }
    }
}
