using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IFacilityRoleService
    {
        int AddUpdateFacilityRole(FacilityRole model);
        int AddUpdateFacilityRoleCustomModel(FacilityRoleCustomModel vm);
        bool CheckCorporateExist(int corporateId);
        bool CheckIfExists(int roleId, int facilityId, int corporateId, int facilityRoleId);
        bool CheckIfRoleAssigned(int roleId, int facilityRoleId);
        bool CheckRoleIsAssignOrNot(int roleId, int facilityId, int corporateId);
        List<FacilityRoleCustomModel> GetActiveInActiveRecords(bool showInActive, int corporateId, int facilityId);
        FacilityRole GetFacilityRoleById(int facilityRoleId);
        List<FacilityRoleCustomModel> GetFacilityRoleListByAdminUser(int corporateId, int facilityId, int roleId);
        List<FacilityRoleCustomModel> GetFacilityRoleListByFacility(int corporateId, int facilityId, int roleId);
        List<FacilityRoleCustomModel> GetFacilityRoleListCustom(int corporateId, int facilityId, int roleId, int portalId, bool showInActive = true);
        List<FacilityRoleCustomModel> GetFacilityRoleListData(int corporateId, int facilityId, int roleId, bool showInActive);
        FacilityRole GetFacilityRoleModelById(int facilityRoleId);
        IEnumerable<FacilityRole> GetFacilityRolesByRoleId(int roleId);
        List<FacilityRoleCustomModel> GetUserTypeRoleDropDown(int corporateId, int facilityId, bool scheduledApplied);
        List<FacilityRoleCustomModel> GetUserTypeRoleDropDownInTaskPlan(int corporateId, int facilityId, bool carePlanAccessible);
        bool IsSchedulingApplied(int roleId);
        int SaveFacilityRole(FacilityRoleCustomModel vm, long userId, DateTime currentDate);
        bool SaveFacilityRoleIfNotExists(FacilityRole model);
    }
}