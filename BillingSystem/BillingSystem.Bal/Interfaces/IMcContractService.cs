using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IMcContractService
    {
        IEnumerable<McContractCustomModel> DeleteContract(int id, int corporateId, int facilityId, long userId);
        string GetManageCareName(string MCCode);
        IEnumerable<McContractCustomModel> GetManagedCareByFacility(int corporateId, int facilityId, long userId);
        McContractCustomModel GetMcContractDetail(int id);
        string GetMCOverview(int MCCode);
        IEnumerable<McContractCustomModel> SaveContract(MCContract m);
    }
}