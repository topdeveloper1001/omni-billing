using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class BedChargesRepository : GenericRepository<BedCharges>
    {
        public BedChargesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
