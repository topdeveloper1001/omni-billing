using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class DRGCodesRepository : GenericRepository<DRGCodes>
    {
        public DRGCodesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
