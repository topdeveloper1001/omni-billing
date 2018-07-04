using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDashboardIndicatorsService
    {
        bool CheckDuplicateSortOrder(int sortOrder, int indicatorId);
        bool DeleteIndicator(DashboardIndicatorsCustomModel vm);
        DashboardIndicatorsCustomModel GetDashboardIndicatorsById(int id, long corporateId = 0);
        DashboardIndicatorsCustomModel GetDashboardIndicatorsByNumber(string number, long corporateId);
        //IEnumerable<DashboardIndicatorsCustomModel> GetDashboardIndicatorsDataList(int corporateId, int? showinactive);
        List<DashboardIndicatorsCustomModel> GetDashboardIndicators(long corporateId, string sort, string sortdir, bool status, long facilityId = 0);
        List<DashboardIndicatorsCustomModel> GetDashboardIndicatorsListByCorporate(int corporateId, int facilityId);
        string GetIndicatorNextNumber(int corporateId);
        bool IsIndicatorExist(string indicatorNumber, int id, int corporateId, string subCategory1, string subCategory2);
        List<DashboardIndicatorsCustomModel> SaveDashboardIndicators(DashboardIndicatorsCustomModel vm);
        bool UpdateIndicatorsOtherDetail(DashboardIndicatorsCustomModel vm);
    }
}