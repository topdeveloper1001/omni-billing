using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class ManualChargesTrackingRepository : GenericRepository<ManualChargesTracking>
    {
        public ManualChargesTrackingRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
