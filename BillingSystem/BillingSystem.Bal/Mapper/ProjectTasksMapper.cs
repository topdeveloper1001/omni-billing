using System;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal.Mapper
{
    public class ProjectTasksMapper : Mapper<ProjectTasks, ProjectTasksCustomModel>
    {
        public override ProjectTasksCustomModel MapModelToViewModel(ProjectTasks model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new ProjectsBal())
                {
                    vm.StatusColor = !string.IsNullOrEmpty(vm.ExternalValue2)
                        ? bal.GetNameByGlobalCodeValue(vm.ExternalValue2,
                            Convert.ToString((int)GlobalCodeCategoryValue.ExternalDashboardColors))
                        : string.Empty;
                    var pr = bal.GetProjectDetailsByNumber(vm.ProjectNumber);
                    if (pr != null)
                    {
                        vm.ProjectName = pr.Name;
                        vm.KpiTargetDate = pr.EstCompletionDate.HasValue ? pr.EstCompletionDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                        if (pr.ExternalValue2 != null)
                            vm.ProjectTypeId = int.Parse(pr.ExternalValue2);
                        int ur;
                        if (int.TryParse(vm.UserResponsible, out ur))
                            vm.Responsible = bal.GetNameByUserId(ur);
                    }
                    vm.FacilityName = bal.GetFacilityNameByFacilityId(Convert.ToInt32(vm.FacilityId));

                    if (!string.IsNullOrEmpty(vm.ExternalValue2))
                    {
                        var colors = (ExternalDashboardColor)Enum.Parse(typeof(ExternalDashboardColor), vm.ExternalValue2);
                        switch (colors)
                        {
                            case ExternalDashboardColor.Green:
                                vm.ColorImage = "/images/circleGreen19x19.png";
                                break;
                            case ExternalDashboardColor.Yellow:
                                vm.ColorImage = "~/images/circleYellow19x19.png";
                                break;
                            case ExternalDashboardColor.Red:
                                vm.ColorImage = "~/images/circleRed19x19.png";
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            return vm;
        }
    }
}
