using System.Collections.Generic;
using System.Data;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IRoleTabsService
    {
        int AddUpdateRolePermission(List<RoleTabs> roleTabsList);
        int AddUpdateRolePermissionSP(DataTable dt, long userId, long corporaIdId, long facilityId);
        bool CheckIfTabNameAccessibleToGivenRole(string tabName, string controllerName, string actionName, int roleId);
        List<TabsAccessible> CheckIfTabsAccessibleToGivenRole(int roleId, IEnumerable<Tabs> tabs);
        List<RoleTabs> GetAllRoleTabsPermissions();
        List<RoleTabs> GetRoleTabsByRoleId(int? roleId);
    }
}