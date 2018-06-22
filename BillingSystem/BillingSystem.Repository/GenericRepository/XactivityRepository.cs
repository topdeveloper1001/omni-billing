using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class XactivityRepository : GenericRepository<XActivity>
    {
        public XactivityRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
