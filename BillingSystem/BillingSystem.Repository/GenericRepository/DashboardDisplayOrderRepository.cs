using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class DashboardDisplayOrderRepository : GenericRepository<DashboardDisplayOrder>
    {
        public DashboardDisplayOrderRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
