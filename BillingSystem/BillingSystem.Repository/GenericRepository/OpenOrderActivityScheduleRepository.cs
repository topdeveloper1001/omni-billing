using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class OpenOrderActivityScheduleRepository: GenericRepository<OpenOrderActivitySchedule>
    {
        public OpenOrderActivityScheduleRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
