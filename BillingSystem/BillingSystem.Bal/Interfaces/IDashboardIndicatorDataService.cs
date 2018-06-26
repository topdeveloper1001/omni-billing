using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDashboardIndicatorDataService
    {
        bool BulkActiveDashboardIndicatorData(string indicatorNumber, int corporateid);
        bool BulkInactiveDashboardIndicatorData(string indicatorNumber, int corporateid);
        List<DashboardIndicatorDataCustomModel> BulkSaveDashboardIndicatorData(List<DashboardIndicatorDataCustomModel> vmList, int currentFacilityId, int currentCorporateId);
        bool DeleteManualDashboardDetails(int corporateid, int facilityId, string indicatorNumber, string budgetType, string year, string subCategory1, string subCategory2);
        bool GenerateIndicatorEffects(DashboardIndicators model);
        DashboardIndicatorData GetCurrentById(int id);
        List<DashboardIndicatorDataCustomModel> GetDashboardIndicatorDataList(int corporateid, int facilityId);
        List<DashboardIndicatorData> GetDashboardIndicatorDataListByIndicatorNumberType(int corporateid, int facilityId, string indicatorNumber, string budgetType, string year);
        List<DashboardIndicatorDataCustomModel> SaveDashboardIndicatorData(DashboardIndicatorData model);
        int SaveDashboardIndicatorDataCustom(DashboardIndicatorData model);
        bool SetStaticBudgetTarget(DashboardIndicatorData model);
        bool SetStaticBudgetTargetIndciators(DashboardIndicators model);
        bool UpdateCalculateIndicatorUpdate(DashboardIndicators model);
        bool UpdateDashBoardIndicatorDataStatus(int corporateid, int facilityId, string indicatorNumber, string budgetType, string year, bool status, string type, string subCategory1, string subCategory2);
        bool UpdateIndicatorsDataInManualDashboard(DashboardIndicatorData model);
    }
}