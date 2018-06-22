using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class SystemConfigurationRepository : GenericRepository<SystemConfiguration>
    {
        public SystemConfigurationRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
 
        }

        public SystemConfigurationRepository()
            : base(new BillingEntities())
        {
            AutoSave = true;
        }
    }
    
}
