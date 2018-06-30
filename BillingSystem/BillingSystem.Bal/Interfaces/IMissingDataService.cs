using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IMissingDataService
    {
        List<BillHeaderCustomModel> GetAllXMLBillHeaderList(int corporateId, int facilityId);
        List<MissingDataCustomModel> GetXMLMissingData(int corporateid, int facilityid);
    }
}