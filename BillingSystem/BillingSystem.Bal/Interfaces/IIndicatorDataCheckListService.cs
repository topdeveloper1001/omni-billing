using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IIndicatorDataCheckListService
    {
        bool DeleteIndicatorDataCheckList(string corporateId, string facilityId, int budgetType, int year, int month);
        List<IndicatorDataCheckListCustomModel> GetDataFromIndicatorDataCheckList(int corporateid, int facilityid, int budgetType, int year, string month);
        List<int> GetDefaultMonthAndYearByFacilityId(int facilityId, int corporateId);
        IndicatorDataCheckList GetIndicatorDataCheckListById(int? id);
        List<IndicatorDataCheckListCustomModel> GetIndicatorDataCheckListList(int corporateid, int facilityid);
        IndicatorDataCheckList GetIndicatorDataCheckListSingle(int facilityId, int corporateId, int budgetType, int year);
        List<IndicatorDataCheckListCustomModel> SaveIndicatorDataCheckList(IndicatorDataCheckList model);
        List<IndicatorDataCheckListCustomModel> SaveIndicatorDataCheckListInDB(IndicatorDataCheckList model);
    }
}