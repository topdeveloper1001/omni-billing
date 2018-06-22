using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class ManagedCareRepository : GenericRepository<ManagedCare>
    {
        public ManagedCareRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
