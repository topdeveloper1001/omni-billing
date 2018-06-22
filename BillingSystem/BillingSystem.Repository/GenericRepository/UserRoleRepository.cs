using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class UserRoleRepository : GenericRepository<UserRole>
    {
        public UserRoleRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
