using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPreliminaryBillService
    {
        List<BedTransactionCustomModel> GetBedTransactionByEncounterID(int? encounterId);
        BedTransactionCustomModel GetBedTransactionDetail(BedTransactionCustomModel obj);
        List<BedTransactionCustomModel> GetBedTransactionList(int encounterId);
    }
}