using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class RoleRepository : GenericRepository<Role>
    {
        public RoleRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
