using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDRGCodesService
    {
        List<DRGCodes> ExportDRGCodes(string text, string tableNumber);
        List<DRGCodes> GetActiveInactiveDrgCodes(bool showInActive, string DrgTableNumber);
        string GetDrgCodeById(int drgId, string DrgTableNumber);
        List<DRGCodes> GetDrgCodes(string DrgTableNumber);
        DRGCodes GetDrgCodesById(int id);
        List<DRGCodes> GetDRGCodesFiltered(string text, string tableNumber);
        List<DRGCodes> GetDrgCodesListOnDemand(int blockNumber, int blockSize, string DrgTableNumber);
        DRGCodes GetDrgCodesobjByCodeValue(string drgcode, string DrgTableNumber);
        string GetDrgDescriptionByCode(string code, string DrgTableNumber);
        List<DRGCodes> GetFilteredDRGCodes(string text, string DrgTableNumber);
        int SaveDrgCode(DRGCodes model, string DrgTableNumber);
    }
}