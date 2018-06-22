using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class DrugInteractionsRepository : GenericRepository<DrugInteractions>
    {
        public DrugInteractionsRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
