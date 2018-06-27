using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{

    public interface IDashboardDisplayOrderService
    {
        DashboardDisplayOrder GetDashboardDisplayOrderByID(int id);
        List<DashboardDisplayOrderCustomModel> GetDashboardDisplayOrderList(int? corporateid, long facilityId = 0);
        List<DashboardDisplayOrderCustomModel> SaveDashboardDisplayOrder(DashboardDisplayOrder m, bool isDeleted = false);
    }
}