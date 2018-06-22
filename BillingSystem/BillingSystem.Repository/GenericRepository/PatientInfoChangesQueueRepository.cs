using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class PatientInfoChangesQueueRepository : GenericRepository<PatientInfoChangesQueue>
    {
        public PatientInfoChangesQueueRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
