using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IHCPCSCodesService
    {
        int AddHCPCSCodes(HCPCSCodes model, string HcpcsTableNumber);
        List<HCPCSCodes> ExportHCPCSCodes(string text, string tableNumber);
        List<HCPCSCodes> GetActiveInActiveHCPCSCodes(bool showInActive, string HcpcsTableNumber);
        List<HCPCSCodes> GetFilteredHCPCSCodes(string text, string HcpcsTableNumber);
        string GetHCPCSCodeDescription(string codeid, string HcpcsTableNumber);
        List<HCPCSCodes> GetHCPCSCodes(string HcpcsTableNumber);
        HCPCSCodes GetHCPCSCodesById(int id);
        List<HCPCSCodes> GetHCPCSCodesFilterData(string text, string tableNumber);
        List<HCPCSCodes> GetHCPCSCodesListOnDemand(int blockNumber, int blockSize, string HcpcsTableNumber);
    }
}