using System.Collections.Generic;
using System.Data;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDrugService
    {
        int AddUptdateDrug(Drug model, string DrugTableNumber);
        List<Drug> ExportFilteredDrugCodes(string text, string tableNumber);
        Drug GetCurrentDrugByCode(string code, string DrugTableNumber);
        Drug GetDrugByID(int? drugId);
        string GetDRUGCodeDescription(string codeid, string DrugTableNumber);
        List<Drug> GetDrugList(string DrugTableNumber);
        List<Drug> GetDrugListbyBrandCode(string brandCode, string DrugTableNumber);
        List<Drug> GetDrugListbyDrugCode(string drugCode, string DrugTableNumber);
        List<Drug> GetDrugListByDrugView(string ViewVal, string DrugTableNumber);
        List<Drug> GetDrugListOnDemand(int blockNumber, int blockSize, string viewVal, string DrugTableNumber);
        List<Drug> GetFilteredDrugCodes(string text, string DrugTableNumber);
        List<Drug> GetFilteredDrugCodesData(string text, string drugStatus, string tableNumber);
        List<Drug> GetFilteredDrugCodesStatus(string text, string drugStatus, string DrugTableNumber);
        string ImportDrugCodesToDB(DataTable dt, int loggedInUser, string tableNumber, string type);
    }
}