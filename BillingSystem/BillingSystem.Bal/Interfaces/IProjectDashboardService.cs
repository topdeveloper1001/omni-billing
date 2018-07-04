using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IProjectDashboardService
    {
        ProjectDashboard GetProjectDashboardById(int? id);
        List<ProjectDashboardCustomModel> GetProjectDashboardData(int corporateid, int facilityid, string year);
        List<ProjectDashboardCustomModel> GetProjectDashboardList(int corporateid, int facilityid);
        List<ProjectDashboardCustomModel> SaveProjectDashboard(ProjectDashboard model);
    }
}