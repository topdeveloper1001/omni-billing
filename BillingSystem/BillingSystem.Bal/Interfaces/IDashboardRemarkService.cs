using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDashboardRemarkService
    {
        DashboardRemark GetDashboardRemarkByID(int? dashboardRemarkId);
        List<DashboardRemarkCustomModel> GetDashboardRemarkList(int corporateid);
        List<DashboardRemarkCustomModel> GetDashboardRemarkList(int corporateid, int facilityid);
        List<DashboardRemarkCustomModel> GetDashboardRemarkListByDashboardType(int corporateid, int facilityid, int dashboardTypeId);
        List<DashboardRemarkCustomModel> GetDashboardRemarkListByDashboardTypeAndMonth(int corporateid, int facilityid, int dashboardTypeId, int month);
        List<DashboardRemarkCustomModel> SaveDashboardRemark(DashboardRemark model);
    }
}