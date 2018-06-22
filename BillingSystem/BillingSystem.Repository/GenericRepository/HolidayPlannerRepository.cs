using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class HolidayPlannerRepository : GenericRepository<HolidayPlanner>
    {
        public HolidayPlannerRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
