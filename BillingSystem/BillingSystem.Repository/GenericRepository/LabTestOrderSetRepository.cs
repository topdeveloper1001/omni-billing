using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class LabTestOrderSetRepository : GenericRepository<LabTestOrderSet>
    {
        public LabTestOrderSetRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
