using System.Collections.Generic;
using System.Data;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IRoleTabsService
    {
        //int AddUpdateRolePermission(List<RoleTabs> roleTabsList);
        int SaveRoleTabs(DataTable dt, long userId, long corporaIdId, long facilityId, long portalKey = 1);
        bool CheckIfTabNameAccessibleToGivenRole(string tabName, string controllerName, string actionName, int roleId);
        List<TabsAccessible> CheckIfTabsAccessibleToGivenRole(int roleId, IEnumerable<Tabs> tabs);
        List<RoleTabs> GetRoleTabsByRoleId(int? roleId);
    }
}