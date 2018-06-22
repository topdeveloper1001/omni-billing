using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class BillingCodeTableSetRepository : GenericRepository<BillingCodeTableSet>
    {
        public BillingCodeTableSetRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
