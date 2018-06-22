using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class ProjectTargetsMapper : Mapper<ProjectTargets, ProjectTargetsCustomModel>
    {
        public override ProjectTargetsCustomModel MapModelToViewModel(ProjectTargets model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new BaseBal())
                {
                    vm.TargetPercentageValueStr = bal.GetNameByGlobalCodeValue(Convert.ToString(model.TargetedCompletionValue),
                   Convert.ToString((int)GlobalCodeCategoryValue.ProjectsPercentageValue));
                }
            }
            return vm;
        }
    }
}
