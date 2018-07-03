using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ITPXMLParsedDataService
    {
        List<TPFileHeaderCustomModel> DeleteAndThenGetXmlFileData(int corporateId, int facilityId, long fileId, bool? withDetails);
        bool DeleteXMLParsedData(int cid, int fid);
        bool ExecuteXmlFileDetails(int corporateId, int facilityid, long fileId);
        List<TPXMLParsedDataCustomModel> GetXmlParsedData(long tpFileId);
        List<TPFileHeaderCustomModel> TPXMLFilesListCIDFID(int corporateid, int facilityId);
        List<TPXMLParsedDataCustomModel> TPXMLParsedDataList(int tpFileHeaderId);
        List<TPXMLParsedDataCustomModel> TPXMLParsedDataListCIDFID(int corporateid, int facilityId);
    }
}