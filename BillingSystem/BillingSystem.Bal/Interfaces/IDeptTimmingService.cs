using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDeptTimmingService
    {
        string GetTimingAddedById(int id);
        int DeleteDepartmentTiming(int facilityStructureId);
        List<DeptTimming> GetDepTimingsById(int departmenId);
        List<DeptTimmingCustomModel> GetDeptTimming();
        List<DeptTimmingCustomModel> GetDeptTimmingByDepartmentId(int departmenId);
        List<DeptTimmingCustomModel> GetDeptTimmingByDepartmentId1(int departmenId);
        DeptTimming GetDeptTimmingById(int? deptTimmingId);
        int SaveDeptTimming(DeptTimming m);
        int SaveDeptTimmingList(List<DeptTimming> m);
    }
}