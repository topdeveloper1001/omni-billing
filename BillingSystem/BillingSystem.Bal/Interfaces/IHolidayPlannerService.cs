using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IHolidayPlannerService
    {
        List<HolidayPlannerCustomModel> GetHolidayPlanner(int corporateId);
        HolidayPlannerCustomModel GetHolidayPlannerByCurrentSelection(int facilityId, int corporateId, int year, string itemTypeId, string itemId);
        HolidayPlanner GetHolidayPlannerById(int? HolidayPlannerId);
        HolidayPlannerCustomModel SaveHolidayPlanner(HolidayPlannerCustomModel vm);
    }
}