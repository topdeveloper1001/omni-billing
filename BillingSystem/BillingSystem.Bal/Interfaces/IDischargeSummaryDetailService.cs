using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDischargeSummaryDetailService
    {
        bool CheckIfRecordAlreadyAdded(string id, string typeId);
        List<DischargeSummaryDetailCustomModel> DeleteDischargeDetail(int id, string typeId);
        List<DischargeSummaryDetailCustomModel> GetDischargeSummaryDetailListByTypeId(string typeId);
        List<DischargeSummaryDetailCustomModel> SaveDischargeSummaryDetail(DischargeSummaryDetail model);
    }
}