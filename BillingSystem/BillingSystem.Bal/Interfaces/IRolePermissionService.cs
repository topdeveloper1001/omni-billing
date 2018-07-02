using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IRolePermissionService
    {
        int AddUpdateRolePermission(List<RolePermission> rolePermissionList);
        List<RolePermission> GetAllRolePermissions();
        List<RolePermission> GetRolePermissionByRoleId(int? roleId);
    }
}