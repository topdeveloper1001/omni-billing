using System.Collections.Generic;
using System.Data;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ICPTCodesService
    {
        int AddUpdateCPTCodes(CPTCodes model, string CptTableNumber);
        bool CheckIfCptCodeExistsInRange(string value, int startRange, int endRange, string CptTableNumber);
        IEnumerable<CPTCodes> GetAllCptCodes(string CptTableNumber);
        List<CPTCodes> GetCodesByRange(int startRange, int endRange, string CptTableNumber);
        List<ExportCodesData> GetCodesDataToExport(long cId, long fId, long userId, string tableNumber, string codeType, string searchText, out string columns, string tableName = "");
        string GetCPTCodeDescription(string codeid, string CptTableNumber);
        List<CPTCodes> GetCPTCodes(string CptTableNumber);
        CPTCodes GetCPTCodesByCode(string code, string CptTableNumber);
        CPTCodes GetCPTCodesById(int id);
        CPTCodesCustomModel GetCPTCodesCustomById(int id, string CptTableNumber);
        List<CPTCodes> GetCPTCodesData(bool showInActive, string CptTableNumber);
        List<CPTCodes> GetCptCodesListByMueValue(string mueValue, string CptTableNumber);
        List<CPTCodes> GetCptCodesListOnDemand(int blockNumber, int blockSize, string CptTableNumber);
        List<CPTCodesCustomModel> GetCPTCustomCodesByRange(int startRange, int endRange, string CptTableNumber);
        List<CPTCodes> GetFilteredCodeExportToExcel(string text, string tableNumber);
        List<CPTCodes> GetFilteredCodes(string text, string CptTableNumber);
        List<CPTCodes> GetFilteredCptCodes(string text, string tableNumber);
        string GetOrderCodeDescbyCode(string code, string CptTableNumber);
        bool ImportAndSaveCodesToDatabase(string codeType, long cId, long fId, string tno, string tname, long loggedinUserId, DataTable dt);
    }
}