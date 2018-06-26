using System;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDashboardService
    {
        List<BedOccupancyCustomModel> GetBedOccupencyByFloorData(int corporateId, int facilityId);
        List<ClaimDenialPercentage> GetClaimDenialPercent();
        List<BedOccupancyCustomModel> GetDashboardChartData(int corporateId, int facilityId);
        List<BillScrubberTrend> GetDenialsCodedByPhysicians(int facilityId);
        List<BedOccupancyCustomModel> GetEncounterTypeData(int corporateId, int facilityId, string displayType, DateTime fromDate, DateTime tillDate);
        List<DashboardChargesCustomModel> GetHighChartsBillingTrendData(int facilityId, int corporateId, string displayType, string fiscalyear);
        List<DashboardChargesCustomModel> GetHighChartsRegistrationProductivityData(int facilityId, int corporateId, string displayType, string fiscalyear, string graphtype);
        IEnumerable<PatientBillingTrend> GetInPatientBillingTrendData(int facilityId, DateTime? tillDate);
        List<PatientBillingTrend> GetInPatientDischarges(string displayType);
        List<PatientBillingTrend> GetOutPatientVisits(string displayType);
        IEnumerable<RegistrationProductivity> GetRegistrationProductivityData(int facilityId, int displayType, DateTime? tillDate);
    }
}