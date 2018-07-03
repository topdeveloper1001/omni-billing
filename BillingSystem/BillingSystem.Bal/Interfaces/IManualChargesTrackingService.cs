using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IManualChargesTrackingService
    {
        int AddUptdateManualChargesTracking(ManualChargesTracking model);
        ManualChargesTracking GetManualChargesTrackingByID(int? ManualChargesTrackingId);
        List<ManualChargesTrackingCustomModel> GetManualChargesTrackingList(int facilityid, int corporateid);
        List<ManualChargesTrackingCustomModel> GetManualChargesTrackingListDateRange(int facilityid, int corporateid, DateTime fromDate, DateTime tilldate);
    }
}