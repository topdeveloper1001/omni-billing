using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDashboardTransactionCounterService
    {
        int DeleteDashboardTransactionCounter(DashboardTransactionCounter model);
        List<DashboardTransactionCounterCustomModel> GetDashboardTrancationData(int corporateId, int facilityId);
        List<DashboardTransactionCounterCustomModel> GetDashboardTransactionCounter(int corporateid, int facilityid);
        DashboardTransactionCounter GetDashboardTransactionCounterById(int? DashboardTransactionCounterId);
        int SaveDashboardTransactionCounter(DashboardTransactionCounter model);
    }
}