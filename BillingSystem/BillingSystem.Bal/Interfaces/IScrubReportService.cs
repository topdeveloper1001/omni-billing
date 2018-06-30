using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IScrubReportService
    {
        int AddUpdateScrubReport(ScrubReport scrubreportobj);
        bool ApplyScrubBillToSpecificBillHeaderId(int billHeaderId, int loggedUserId);
        string AssignUserToScrubHeaderForBillEdit(int assignedTo, int scrubHeaderId, int loggedUserId, DateTime assignedDate);
        List<ScrubHeaderCustomModel> GetErrorDetailByRuleCode(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate);
        List<ScrubHeaderCustomModel> GetErrorSummaryByRuleCode(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate);
        List<ScrubHeaderCustomModel> GetScrubHeaderByEncounter(int encounterid);
        ScrubEditTrack GetScrubHeaderById(int scrubHeaderid);
        List<ScrubHeaderCustomModel> GetScrubHeaderList(int corporateId, int facilityId, int billHeaderId, int userId, bool createscrub);
        List<ScrubHeaderCustomModel> GetScrubHeaderListWorkQueues(int corporateId, int facilityId, int billHeaderId, int userId, bool createscrub);
        List<ScrubReportCustomModel> GetScrubReport(int scrubHeaderId, int reportType);
        ScrubReport GetScrubReportById(int scrubReportId);
        ScrubReportCustomModel GetScrubReportDetailById(int scrubReportId, string BillEditRuleTableNumber);
        List<ScrubHeaderCustomModel> GetScrubSummaryList(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate);
        int SetCorrectedDiagnosis(int corporateid, int facilityid, int patientid, int encounterid, int loggedinUserId, string diagnosisCode);
        bool UpdateScrubReportDetailWithCorrection(int scrubReportId, int scrubHeaderId, string lhsValue, string rhsValue, int loggedinUserId, int corporateId, int facilityid, string correctionCodeId);
    }
}