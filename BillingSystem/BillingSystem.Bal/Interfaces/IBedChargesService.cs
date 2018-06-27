using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IBedChargesService
    {
        bool CheckBedChargeExist(int encounterid, int patietid, DateTime? datestart);
        int DeleteBedCharges(BedCharges model);
        BedCharges GetBedChargesByID(int? BedChargesId);
        List<BedChargesCustomModel> GetBedChargesList();
        List<BedChargesCustomModel> SaveBedCharges(BedCharges model);
        bool SaveBedChargesList(List<BedCharges> model);
    }
}