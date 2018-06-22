using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class DashboardRemarkRepository : GenericRepository<DashboardRemark>
    {
        public DashboardRemarkRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
