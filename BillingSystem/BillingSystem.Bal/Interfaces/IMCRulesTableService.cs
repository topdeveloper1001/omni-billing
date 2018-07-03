using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IMCRulesTableService
    {
        bool DeleteMCRulesTable(int id, int corporateId, int facilityId);
        int GetMaxRuleStepNumber(int RuleSetNumber);
        List<MCRulesTableCustomModel> GetMcRulesListByRuleSetId(int ruleSetNumber);
        MCRulesTable GetMCRulesTableByID(int? MCRulesTableId);
        List<MCRulesTableCustomModel> GetMCRulesTableList();
        List<MCRulesTableCustomModel> SaveMCRulesTable(MCRulesTable model);
    }
}