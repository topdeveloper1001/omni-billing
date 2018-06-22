using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class MCOrderCodeRatesRepository : GenericRepository<MCOrderCodeRates>
    {
        public MCOrderCodeRatesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
