using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IXFileHeaderService
    {
        string GetRemittanceFormattedXmlStringByFileId(int id);
        List<XFileHeaderCustomModel> GetXFileHeader();
        List<XFileHeaderCustomModel> GetXFileHeaderByCId(int cId, int fId);
        XFileHeader GetXFileHeaderByID(int? XFileHeaderId);
        int SaveXFileHeader(XFileHeader model);
    }
}