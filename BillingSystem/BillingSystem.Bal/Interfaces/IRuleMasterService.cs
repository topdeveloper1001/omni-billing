using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IRuleMasterService
    {
        int AddUptdateRuleMaster(RuleMaster ruleMaster, string BillEditRuleTableNumber);
        bool DeleteMultipleRules(List<int> ids);
        int DeleteRuleMaster(RuleMaster model);
        RuleMaster GetRuleMasterById(int? ruleMasterId, string BillEditRuleTableNumber);
        RuleMasterCustomModel GetRuleMasterCustomModelById(int? ruleMasterId);
        List<RuleMasterCustomModel> GetRuleMasterList(string BillEditRuleTableNumber, bool isActive = true);

    }
}