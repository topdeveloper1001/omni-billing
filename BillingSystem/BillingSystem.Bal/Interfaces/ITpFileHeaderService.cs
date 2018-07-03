using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ITpFileHeaderService
    {
        string GetFormattedXmlStringByXFileId(int id);
        IEnumerable<TPFileHeaderCustomModel> TpFileHeaderList(int corporateId);
    }
}