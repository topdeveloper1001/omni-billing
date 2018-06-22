using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class BillActivityRepository : GenericRepository<BillActivity>
    {
        public BillActivityRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
