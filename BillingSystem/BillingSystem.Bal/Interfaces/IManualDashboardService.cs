using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IManualDashboardService
    {
        List<DashboardIndicatorData> GetIndicatorsDataForIndicatorNumber(int corporateId, int facilityid, string year, string indicatorNumber, string budgetType, string subCategory1, string subCategory2);
        List<DashboardIndicators> GetIndicatorsList(int corporateId, string ownership = "");
        ManualDashboard GetManualDashboardById(int? id);
        ManualDashboardCustomModel GetManualDashboardDataByIndicatorNumber(int corporateId, int facilityid, string year, string indicatorNumber, string budgetType, string subCategory1, string subCategory2);
        List<ManualDashboardCustomModel> GetManualDashboardList(int corporateId, int facilityId = 0);
        List<ManualDashboardCustomModel> GetManualIndicatorDashboardDataList(int corporateId, int facilityid, string year);
        List<string> GetOwnershipList(int corporateId);
        UserTokenCustomModel GetUserToken(string username, DateTime expiryDate);
        List<ManualDashboardCustomModel> RebindManualIndicatorDashboardDataList(int corporateId, int facilityid, int year, int indicator, string ownership);
        string SaveIndicatorsPlusData(ManualDashboard model);
        List<ManualDashboardCustomModel> SaveManualDashboard(ManualDashboard model);
    }
}