using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IUserRoleService
    {
        int AddUpdateUserRole(IEnumerable<UserRole> userRoles);
        bool CheckIfExists(int userId, int roleId);
        bool CheckRoleExist(int roleId);
        int DeleteRoleWithUser(int userId);
        List<UserRole> GetUserIdByCorporateAndRoleTypeId(int corporateId, List<Role> roleIds);
        List<UserRole> GetUserRolesByCorporateFacilityAndUserId(int userId, int corporateId, int facilityId);
        List<UserRole> GetUserRolesByUserId(int userId);
        int SaveUserRole(UserRole model);
    }
}