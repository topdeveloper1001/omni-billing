using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class HolidayPlannerDetailsRepository : GenericRepository<HolidayPlannerDetails>
    {
        public HolidayPlannerDetailsRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
