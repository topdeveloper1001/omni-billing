using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class PatientPreSchedulingRepository : GenericRepository<PatientPreScheduling>
    {
        public PatientPreSchedulingRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
