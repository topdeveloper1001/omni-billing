using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IMCOrderCodeRatesService
    {
        int DeleteMCOrderCodeRates(int id);
        MCOrderCodeRates GetMCOrderCodeRatesByID(int? mcOrderCodeRatesId);
        List<MCOrderCodeRatesCustomModel> GetMCOrderCodeRatesList();
        List<MCOrderCodeRatesCustomModel> GetMcOrderCodeRatesListByMcCode(int mcCode);
        List<MCOrderCodeRatesCustomModel> SaveMCOrderCodeRates(MCOrderCodeRates model);
    }
}