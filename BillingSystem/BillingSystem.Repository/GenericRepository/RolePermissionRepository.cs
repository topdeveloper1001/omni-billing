using BillingSystem.Model;


namespace BillingSystem.Repository.GenericRepository
{
    public class RolePermissionRepository : GenericRepository<RolePermission>
    {
        public RolePermissionRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }

    }
}
