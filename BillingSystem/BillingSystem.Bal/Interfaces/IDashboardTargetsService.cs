using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDashboardTargetsService
    {
        DashboardTargets GetDashboardTargetsById(int? dashboardTargetsId);
        List<DashboardTargetsCustomModel> GetDashboardTargetsList(int cId, int fId);
        List<DashboardTargetsCustomModel> SaveDashboardTargets(DashboardTargets model);
    }
}