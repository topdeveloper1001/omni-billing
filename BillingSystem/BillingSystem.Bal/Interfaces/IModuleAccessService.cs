using System;
using System.Collections.Generic;
using System.Data;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IModuleAccessService
    {
        int DeleteEntry(ModuleAccess moduleAccess);
        List<ModuleAccess> GetModuleAccess();
        ModuleAccess GetModuleAccessByID(int? moduleAccessId);
        List<ModuleAccess> GetModulesAccessList(int corporateId, int? facilityid);
        int Save(DataTable dt, int corporateid, int facilityId, DateTime? currentDate, int loggedinUserId = 0);
    }
}