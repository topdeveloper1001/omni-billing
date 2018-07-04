using System;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IReportingService
    {
        List<AgingReportCustomModel> GetAgeingReport(int corporateid, int facilityId, DateTime? date, int reportType);
        List<ScrubEditTrackCustomModel> GetBillEditCorrectionLogs(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate, bool displayall);
        List<ChargesReportCustomModel> GetChargesDetailReport(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate, decimal? departmentNumber, int type, int payorId);
        List<ChargesReportCustomModel> GetChargesReport(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate, decimal? departmentNumber, int type);
        List<BillTransmissionReportCustomModel> GetClaimTransDetails(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate, int? displayby);
        List<DenialReportCustomModel> GetDenialCodesReport(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate, int? displayby);
        List<PhysicianDepartmentUtilizationCustomModel> GetDepartmentUtilizationReport(int corporateId, DateTime? fromDate, DateTime? tillDate, int displayflag, int facilityId, int physicianId, int departmentId);
        List<FutureOpenOrderCustomModel> GetFutureChargeReport(int corporateId, DateTime? fromDate, DateTime? tillDate, int facilityId);
        List<JournalEntrySupportReportCustomModel> GetJournalEntrySupport(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate, int? displayby);
        List<LoginActivityReportCustomModel> GetLoginTimeDayNightShift(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate, int userId);
        List<AuditLogCustomModel> GetPasswordChangeLog(DateTime fromDate, DateTime tillDate, bool isAll, int corporateId);
        List<AuditLogCustomModel> GetPasswordChangeLog_SP(DateTime fromDate, DateTime tillDate, bool isAll, int corporateId, int facilityId);
        List<AuditLogCustomModel> GetPasswordDisabledLog(DateTime fromDate, DateTime tillDate, int userId, int corporateId, bool isAll);
        List<AuditLogCustomModel> GetPasswordDisableLog_SP(DateTime fromDate, DateTime tillDate, bool isAll, int corporateId, int facilityId);
        List<AgingReportCustomModel> GetPatientAgeingPayorWise(int corporateid, int facilityId, DateTime? date, string companyId);
        List<PhysicianActivityCustomModel> GetPhysicianActivityReport(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate, int physicianId);
        List<PhysicianDepartmentUtilizationCustomModel> GetPhysicianUtilizationReport(int corporateId, DateTime? fromDate, DateTime? tillDate, int displayflag, int facilityId, int physicianId, int departmentId);
        List<ReconcilationReportCustomModel> GetReconciliationReport(int corporateid, int facilityId, DateTime? date, string viewtype, int reportType);
        List<ReconcilationReportCustomModel> GetReconciliationReport_Monthly(int corporateid, int facilityId, DateTime? date, string viewtype, int reportType);
        List<ReconcilationReportCustomModel> GetReconciliationReport_Weekly(int corporateid, int facilityId, DateTime? date, string viewtype, int reportType);
        List<RevenueForecast> GetRevenueForecastFacility(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate);
        List<RevenueForecast> GetRevenueForecastFacilityByPatient(int patientId);
        List<AuditLogCustomModel> GetSchedulingLogActivity(DateTime? fromDate, DateTime? tillDate, int corporateId, int facilityId);
        List<ScrubHeaderCustomModel> GetScrubberAndErrorSummaryReport(string reportingTypeId, DateTime? fromDate, DateTime? tillDate, int facilityId);
        List<LoginTrackingCustomModel> GetUserLoginActivityDetailList(int userId, DateTime tillDate);
        List<LoginTrackingCustomModel> GetUserLoginActivityList(DateTime fromDate, DateTime tillDate, int userId);
        List<LoginTrackingCustomModel> GetUserLoginActivityList_SP(DateTime? fromDate, DateTime? tillDate, int userId, int corporateId, int facilityId);
    }
}