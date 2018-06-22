using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class DischargeSummaryDetailRepository : GenericRepository<DischargeSummaryDetail>
    {
        public DischargeSummaryDetailRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
