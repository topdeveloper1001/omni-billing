using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IATCCodesService
    {
        int DeleteATCCode(ATCCodes model);
        List<ATCCodesCustomModel> GetATCCodes(string text = "");
        ATCCodes GetATCCodesByID(int? ATCCodesId);
        int SaveATCCodes(ATCCodes model);
    }
}