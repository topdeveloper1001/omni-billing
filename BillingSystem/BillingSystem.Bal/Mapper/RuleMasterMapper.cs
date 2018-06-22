using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class RuleMasterMapper : Mapper<RuleMaster, RuleMasterCustomModel>
    {
        public override RuleMasterCustomModel MapModelToViewModel(RuleMaster model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new GlobalCodeBal())
                {
                    vm.RuleSpecifiedForString = bal.GetNameByGlobalCodeValue(model.RuleSpecifiedFor,
                        Convert.ToString((int)GlobalCodeCategoryValue.RulesSpecifiedfor));
                    vm.RoleIdString = bal.GetRoleNameByRoleId(Convert.ToInt32(model.RoleId));
                    vm.CorporateString = bal.GetCorporateNameFromId(Convert.ToInt32(model.CorporateID));
                    vm.FacilityString = bal.GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityID));
                    vm.RuleTypeString = Convert.ToInt32(model.RuleType) == 1 ? "Normal" : "Other";
                }
            }
            return vm;
        }
    }
}
