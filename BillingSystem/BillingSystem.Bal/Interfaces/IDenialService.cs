using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDenialService
    {
        int AddUpdateDenial(Denial Denial);
        List<DenialCodeCustomModel> BindDenialCodes(int takeValue);
        List<Denial> GetAuthorizationDenialsCode();
        List<DenialCodeCustomModel> GetDenial();
        Denial GetDenialById(int id);
        List<DenialCodeCustomModel> GetFilteredDenialCodes(string text);
        List<GlobalCodes> GetGlobalCodesById(string id);
        List<DenialCodeCustomModel> GetListOnDemand(int blockNumber, int blockSize);
    }
}