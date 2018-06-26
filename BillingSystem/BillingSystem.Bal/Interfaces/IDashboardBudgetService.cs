using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDashboardBudgetService
    {
        int DeleteDashBoradBudget(DashboardBudget model);
        List<DashboardBudgetCustomModel> GetDashboardBudget(int corporateid, int facilityid);
        DashboardBudget GetDashboardBudgetById(int? dashboardBudgetId);
        List<ExternalDashboardModel> GetDashboardDataBalanceSheetList(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department);
        List<ExternalDashboardModel> GetDashBoardDataStatList(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department);
        List<DashboardBudgetReportCustomModel> GetDBBudgetActual(int facilityId, int corporateId, string bugetFor, string fiscalyear);
        List<DashboardChargesCustomModel> GetDBChargesChartDashBoard(int facilityId, int corporateId, string bugetFor, string fiscalyear);
        List<DashboardChargesCustomModel> GetDBChargesDashBoard(int facilityId, int corporateId, string bugetFor, string fiscalyear);
        List<ExternalDashboardModel> GetExecutiveDashboardBalanceSheet(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department);
        List<ManualDashboardCustomModel> GetManualDashBoard(int facilityId, int corporateId, string bugetFor, int fiscalyear, int facilityType, int segment, int Department);
        List<ExternalDashboardModel> GetManualDashBoardStatData(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department);
        List<ExternalDashboardModel> GetManualDashBoardStatDataAcute(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department);
        List<ExternalDashboardModel> GetManualDashBoardStatSingleData(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department);
        List<ManualDashboardCustomModel> GetManualDashBoardV1(int facilityId, int corporateId, string bugetFor, int fiscalyear, int facilityType, int segment, int Department);
        List<PatientFallStats> GetPatientFallRate(int facilityId, int corporateId, string bugetFor, int fiscalyear, int facilityType, int segment, int Department);
        List<PatientFallStats> GetPatientFallRateV1(int facilityId, int corporateId, string bugetFor, int fiscalyear, int facilityType, int segment, int Department);
        List<ManualDashboardCustomModel> GetSubCategoryCharts(int facilityId, int corporateId, string bugetFor, int fiscalyear, int facilityType, int segment, int Department);
        List<ManualDashboardCustomModel> GetSubCategoryChartsPayorMix(int facilityId, int corporateId, string bugetFor, int fiscalyear, int facilityType, int segment, int Department);
        int SaveDashboardBudget(DashboardBudget model);
        bool SetDashBoardChargesActuals(int facilityId, int corporateId, string fiscalyear);
        bool SetDashBoardCounterActuals(int facilityId, int corporateId, string fiscalyear);
    }
}