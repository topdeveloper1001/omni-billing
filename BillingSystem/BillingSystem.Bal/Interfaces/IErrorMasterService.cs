using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IErrorMasterService
    {
        int AddUptdateErrorMaster(ErrorMaster model);
        List<ErrorMasterCustomModel> GetErrorListByCorporateAndFacilityId(int corporateId, int facilityId, bool? showInactive);
        ErrorMaster GetErrorMasterById(int? errorMasterId);
        IEnumerable<ErrorMasterCustomModel> GetSearchedDenialsList(string text);
    }
}