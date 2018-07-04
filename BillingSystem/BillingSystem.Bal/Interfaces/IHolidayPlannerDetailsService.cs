using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IHolidayPlannerDetailsService
    {
        bool DeleteHolidayEvent(int id);
        int GetExistingHolidayPlannerDetailsId(int holidayId, string eventId);
        List<HolidayPlannerDetailsCustomModel> GetHolidayPlannerDetails();
        HolidayPlannerDetails GetHolidayPlannerDetailsByID(int? HolidayPlannerDetailsId);
        int SaveHolidayPlannerDetails(HolidayPlannerDetails model);
    }
}