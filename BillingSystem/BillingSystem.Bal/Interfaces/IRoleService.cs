using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IRoleService
    {
        int AddUpdateRole(Role role);
        bool CheckDuplicateRole(int roleId, string roleName);
        List<Role> GetAllRoles(int corporateId);
        List<Role> GetAllRolesByCorporateFacility(int corporateId, int facilityId, int portalId = 0);
        List<Role> GetFacilityRolesByCorporateIdFacilityId(int corporateId, int facilityId, int portalId);
        List<Role> GetPhysicianRolesByCorporateId(int corporateId);
        Role GetRoleById(int roleID);
        Role GetRoleByRoleName(string roleName);
        int GetRoleIdByFacilityAndName(string roleName, int cId, int fId);
        string GetRoleNameById(int roleID);
        List<Role> GetRolesByCorporateId(int corporateId);
        List<Role> GetRolesByCorporateIdFacilityId(int corporateId, int facilityId);
        List<DropdownListData> GetRolesByFacility(int facilityId);
    }
}