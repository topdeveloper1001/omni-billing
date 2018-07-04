using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IXMLBillingService
    {
        int AddUptdateXFileHeader(XFileHeader XFileHeader);
        string GetFormattedXmlStringByXFileId(int id);
        int GetLatestXFileHeaderId();
        List<XFileHeader> GetXFileHeader(int facilityId, int corporateId);
        XFileHeader GetXFileHeaderByID(int? XFileHeaderId);
        List<XFileHeaderCustomModel> GetXFileHeaderCModel(int facilityId, int corporateId);
        XFileXML GetXmlStringById(int id);
        bool ImportXmlBills(string xml, string fullPath, bool sFlag);
        string RemittanceXMLParser(string xml, string fullPath, bool sFlag, int corporateId, int facilityId);
        string XMLBillFileParser(string xml, string fullPath, bool sFlag, int corporateId, int facilityId, string batchNumber, bool? executeDetails = false, long? loggedinUserId = 0);
    }
}