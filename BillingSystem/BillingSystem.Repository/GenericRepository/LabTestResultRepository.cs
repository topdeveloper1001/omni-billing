using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class LabTestResultRepository : GenericRepository<LabTestResult>
    {
        public LabTestResultRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
