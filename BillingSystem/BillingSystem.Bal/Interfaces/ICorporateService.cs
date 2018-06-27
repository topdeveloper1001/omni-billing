using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ICorporateService
    {
        int AddUptdateCorporate(Corporate corporate);
        bool CheckDefaultTableNumber(string defaultTableNumber, int corporateId);
        bool CheckDuplicateCorporate(string name, int id);
        bool CheckDuplicateCorporateNumber(string number, int id);
        void CreateDefaultCorporateItem(int corporateId, string corporateName);
        bool DeleteCorporateData(string corporateId);
        List<Corporate> GetAllCorporate();
        List<Corporate> GetCorporate(int cId);
        Corporate GetCorporateById(int? CorporateId);
        List<Corporate> GetCorporateDDL(int cId);
        List<DropdownListData> GetCorporateDropdownData(int cId);
        string GetCorporateNameById(int? corporateId);
    }
}