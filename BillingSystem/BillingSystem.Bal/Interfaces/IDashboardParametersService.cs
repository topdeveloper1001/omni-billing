using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDashboardParametersService
    {
        List<DashboardParametersCustomModel> DeleteDashboardParameters(DashboardParameters model);
        DashboardParameters GetDashboardParametersById(int? Id);
        List<DashboardParametersCustomModel> GetDashboardParameters(int corpoarteId, int facilityId);
        List<DashboardParametersRangeCustomModel> GetParametersListByDashboard(int corpoarteId, int facilityId, string dashboardtype);
        List<DashboardParametersCustomModel> SaveDashboardParameters(DashboardParameters model);
    }
}