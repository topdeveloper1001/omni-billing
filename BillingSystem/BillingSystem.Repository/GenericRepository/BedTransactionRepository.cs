using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class BedTransactionRepository : GenericRepository<BedTransaction>
    {
        public BedTransactionRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
