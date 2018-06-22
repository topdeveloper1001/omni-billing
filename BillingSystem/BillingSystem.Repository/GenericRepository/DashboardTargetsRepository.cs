using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class DashboardTargetsRepository : GenericRepository<DashboardTargets>
    {
        public DashboardTargetsRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
