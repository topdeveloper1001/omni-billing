using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IFacilityDepartmentService
    {
        FacilityDepartment GetFacilityDepartmentById(int? facilityDepartmentId);
        List<FacilityDepartmentCustomModel> GetFacilityDepartmentList(int corpoarteId, int facilityId, bool showInActive);
        List<FacilityDepartmentCustomModel> SaveFacilityDepartment(FacilityDepartment model);
    }
}