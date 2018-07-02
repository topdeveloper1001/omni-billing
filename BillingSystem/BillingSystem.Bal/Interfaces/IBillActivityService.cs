using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IBillActivityService
    {
        bool CheckForDuplicateTableSet(int id, string tableNumber, string typeId);
        List<BillingCodeTableSet> GetTableNumbersList(string typeId);
        bool DeleteBillActivityFromBill(int id);
        bool DeleteDiagnosisTypeBillActivity(int encounterId, int patientId, string actCode, int userId);
        List<BillDetailCustomModel> GetBillActivitiesByBillHeaderId(int billHeaderId);
        List<BillDetailCustomModel> GetBillActivitiesByEncounterId(int encounterId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        List<BillPdfFormatCustomModel> GetBillPdfFormat(int billHeaderId);
        string GetCodeDescription(string orderCode, string orderType, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        int GetEncounterIdByBillActivityId(int billactivityId);
        string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "");
        int GetPatientIdByBillActivityId(int billactivityId);
    }
}