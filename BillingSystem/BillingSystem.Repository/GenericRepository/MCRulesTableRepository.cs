using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class MCRulesTableRepository : GenericRepository<MCRulesTable>
    {
        public MCRulesTableRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
