using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class ProjectsMapper : Mapper<Projects, ProjectsCustomModel>
    {
        public override ProjectsCustomModel MapModelToViewModel(Projects model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new ProjectsBal())
                {
                    vm.ProjectType = !string.IsNullOrEmpty(vm.ExternalValue2) ? bal.GetNameByGlobalCodeValue(vm.ExternalValue2,
                        Convert.ToString((int)GlobalCodeCategoryValue.DashboardProjectsType)) : string.Empty;
                    vm.ProjectStatus = !string.IsNullOrEmpty(vm.ExternalValue3) ? bal.GetNameByGlobalCodeValue(vm.ExternalValue3,
                       Convert.ToString((int)GlobalCodeCategoryValue.ExternalDashboardColors)) : string.Empty;

                    vm.FacilityName = bal.GetFacilityNameByFacilityId(Convert.ToInt32(vm.FacilityId));

                    int ur;
                    if (int.TryParse(vm.UserResponsible, out ur))
                        vm.Responsible = bal.GetNameByUserId(ur);
                }
            }
            return vm;
        }
    }
}
