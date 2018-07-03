using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IXAdviceXMLParsedDataService
    {
        List<XAdviceXMLParsedData> GetXAdviceXMLParsedData(int corporateId, int facilityId);
        List<XAdviceXMLParsedDataCustomModel> GetXAdviceXmlParsedDataById(int corporateId, int facilityId, int fileId);
        List<XAdviceXMLParsedDataCustomModel> GetXAdviceXMLParsedDataCustom(int corporateId, int facilityId);
        List<TPXMLParsedDataCustomModel> GetXMLFileData(int corporateId, int facilityId);
    }
}