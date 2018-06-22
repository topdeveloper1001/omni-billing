using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class ErrorMasterRepository : GenericRepository<ErrorMaster>
    {
        public ErrorMasterRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
