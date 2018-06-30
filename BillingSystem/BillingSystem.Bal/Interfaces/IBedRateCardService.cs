using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IBedRateCardService
    {
        int AddUpdateBedRateCard(BedRateCard model);
        string GetBedRateByBedTypeId(int id);
        BedRateCard GetBedRateCardById(int id);
        List<BedRateCardCustomModel> GetBedRateCardsList(string serviceCodeTableNumber);
        List<BedRateCardCustomModel> GetBedRateCardsList(string serviceCodeTableNumber, int corporateId, int facilityId);
        IEnumerable<BedRateCardCustomModel> GetBedRateCardsListByBedType(string bedTypeid, bool nonChargeable);
     }
}