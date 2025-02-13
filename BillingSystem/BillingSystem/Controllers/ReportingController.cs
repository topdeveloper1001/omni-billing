﻿
namespace BillingSystem.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;
    using System.Web.Mvc;
    using Common;
    using Common.Common;
    using Model;
    using Model.CustomModel;
    using Models;

    using Microsoft.Reporting.WebForms;

    using RazorPDF;
    using BillingSystem.Bal.Interfaces;
    using AutoMapper;

    #endregion

    /// <summary>
    ///     Code cleaned on 22 Apeil 2016 by Shashank
    /// </summary>
    public class ReportingController : BaseController
    {
        private readonly IEncounterService _eService;
        private readonly IBillActivityService _baService;
        private readonly IBillHeaderService _bhService;
        private readonly IFacilityStructureService _fsService;
        private readonly IPatientInfoService _piService;
        private readonly IUsersService _uService;
        private readonly IReportingService _service;
        private readonly IFacilityService _fService;
        private readonly IPhysicianService _phService;
        private readonly IScrubReportService _srService;
        private readonly IXmlReportingService _xrService;
        private readonly IMapper _mapper;

        public ReportingController(IMapper mapper, IEncounterService eService, IBillActivityService baService
            , IBillHeaderService bhService, IFacilityStructureService fsService, IPatientInfoService piService
            , IUsersService uService, IReportingService service, IFacilityService fService
            , IPhysicianService phService, IScrubReportService srService, IXmlReportingService xrService)
        {
            _eService = eService;
            _baService = baService;
            _bhService = bhService;
            _fsService = fsService;
            _piService = piService;
            _uService = uService;
            _service = service;
            _fService = fService;
            _phService = phService;
            _srService = srService;
            _xrService = xrService;

            _mapper = mapper;
        }


        /// <summary>
        ///     Indexes the specified reporting identifier.
        /// </summary>
        /// <param name="reportingId">
        ///     The reporting identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult Index(int? reportingId)
        {
            int reportingTypeId = Convert.ToInt32(reportingId);
            var reporting = new ReportingView
            {
                FromDate = Helpers.GetFirstDayofCurrentMonth(),
                ToDate = Helpers.GetLastDayOfCurrentMonth(),
                ReportingType = reportingTypeId,
                Title = Helpers.ReportingTitleView(Convert.ToString(reportingTypeId)),
                ReportingTypeAction =
                                        Helpers.GetReportingTypeAction(Convert.ToString(reportingTypeId)),
                UserId = Helpers.GetLoggedInUserId(),
                ShowAllRecords = false,
                ViewType = "Y",
                CorporateId = Helpers.GetDefaultCorporateId()
            };
            return View(reporting);
        }

        /// <summary>
        ///     Gets the type of the list by reporting.
        /// </summary>
        /// <param name="reportingTypeId">
        ///     The reporting type identifier.
        /// </param>
        /// <param name="fromDate">
        ///     From date.
        /// </param>
        /// <param name="tillDate">
        ///     The till date.
        /// </param>
        /// <param name="isAll">
        ///     if set to <c> true </c> [is all].
        /// </param>
        /// <param name="userId">
        ///     The user Id.
        /// </param>
        /// <param name="displayby">
        ///     The displayby.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetListByReportingType(
            string reportingTypeId,
            DateTime fromDate,
            DateTime tillDate,
            bool isAll,
            int? userId,
            int? displayby)
        {
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingTypeId);
            int useridnotnull = userId == null ? 0 : Convert.ToInt32(userId);
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            if (isAll)
            {
                userId = 0;
            }

            switch (reportingType)
            {
                case ReportingType.UserLoginActivity:

                    // var userLoginList = _service.GetUserLoginActivityList(fromDate, tillDate, isAll, useridnotnull);
                    // return PartialView(PartialViews.UserLoginActivityView, userLoginList);
                    corporateId = Helpers.GetDefaultCorporateId();
                    List<LoginActivityReportCustomModel> userLoginList = _service.GetLoginTimeDayNightShift(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate,
                        Convert.ToInt32(userId));
                    return PartialView(PartialViews.LoginTrackingDayNightShiftReport, userLoginList);

                case ReportingType.PasswordDisablesLog:

                    // corporateId = Helpers.GetDefaultCorporateId();
                    List<AuditLogCustomModel> pwdDisabledLogs = _service.GetPasswordDisableLog_SP(
                        fromDate,
                        tillDate,
                        isAll,
                        corporateId,
                        facilityId);
                    return PartialView(PartialViews.PasswordDisablesLogView, pwdDisabledLogs);

                case ReportingType.PasswordChangeLog:
                    List<AuditLogCustomModel> passwordChangeLogs = _service.GetPasswordChangeLog_SP(
                        fromDate,
                        tillDate,
                        isAll,
                        corporateId,
                        facilityId);
                    return PartialView(PartialViews.PasswordChangeLogView, passwordChangeLogs);

                case ReportingType.DailyChargeReport:
                    List<LoginTrackingCustomModel> dailyChargeReport = _service.GetUserLoginActivityList(
                        fromDate,
                        tillDate,
                        useridnotnull);
                    return PartialView(PartialViews.DailyChargeReportView, dailyChargeReport);

                case ReportingType.ChargesByPayorReport:
                    List<LoginTrackingCustomModel> chargesByPayorReport = _service.GetUserLoginActivityList(
                        fromDate,
                        tillDate,
                        useridnotnull);
                    return PartialView(PartialViews.ChargesByPayorReportView, chargesByPayorReport);

                case ReportingType.RevenueForecastReport:
                    List<RevenueForecast> revenueReport = _service.GetRevenueForecastFacility(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate);
                    return PartialView(PartialViews.RevenueForecastFacilityView, revenueReport);

                case ReportingType.ClaimTransactionDetailReport:
                    List<BillTransmissionReportCustomModel> claimtransactionDetails =
                        _service.GetClaimTransDetails(corporateId, facilityId, fromDate, tillDate, displayby);
                    return PartialView(PartialViews.ClaimTransDetailReport, claimtransactionDetails);

                case ReportingType.DenialReport:
                    List<DenialReportCustomModel> denialreportDetail = _service.GetDenialCodesReport(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate,
                        displayby);
                    return PartialView(PartialViews.DenialReport, denialreportDetail);

                case ReportingType.JournalEntrySupport:
                    List<JournalEntrySupportReportCustomModel> journalEntrySupportReportDetail =
                        _service.GetJournalEntrySupport(corporateId, facilityId, fromDate, tillDate, displayby);
                    return PartialView(PartialViews.JournalEntrySupportReport, journalEntrySupportReportDetail);

                case ReportingType.JournalEntrySupportMCD:
                    List<JournalEntrySupportReportCustomModel> journalEntrySupportReportDetail1 =
                        _service.GetJournalEntrySupport(corporateId, facilityId, fromDate, tillDate, displayby);
                    return PartialView(
                        PartialViews.JournalEntryManageCareDiscount,
                        journalEntrySupportReportDetail1);

                case ReportingType.JournalEntrySupportWO:
                    List<JournalEntrySupportReportCustomModel> journalEntrySupportReportDetail2 =
                        _service.GetJournalEntrySupport(corporateId, facilityId, fromDate, tillDate, displayby);
                    return PartialView(PartialViews.JournalEnterBadDebtWriteoffs, journalEntrySupportReportDetail2);

                case ReportingType.CorrectionLog:
                    List<ScrubEditTrackCustomModel> correctionLogReport = _service.GetBillEditCorrectionLogs(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate,
                        isAll);
                    return PartialView(PartialViews.ScrubEditTrackListReport, correctionLogReport);

                case ReportingType.SchedulingActivityLog:
                    List<AuditLogCustomModel> schedulingLogReport = _service.GetSchedulingLogActivity(fromDate, tillDate, corporateId, facilityId);
                    return PartialView(PartialViews.SchedulingLog, schedulingLogReport);

                default:
                    break;
            }

            return PartialView(PartialViews.RoleList, null);
        }

        /// <summary>
        ///     Exports to PDF.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="isAll">if set to <c>true</c> [is all].</param>
        /// <param name="reportingId">The reporting identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="viewtype">The viewtype.</param>
        /// <returns></returns>
        public ActionResult ExportToPDF(
            DateTime fromDate,
            DateTime? tillDate,
            bool isAll,
            string reportingId,
            int? userId,
            string viewtype)
        {
            // var dtFrom = Helpers.ParseValueToInvariantDateTime(fromDate);
            // var dtTill = Helpers.ParseValueToInvariantDateTime(tillDate);
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            DateTime tillDateNew = tillDate ?? Helpers.GetInvariantCultureDateTime();
            int useridNew = userId ?? 0;
            PdfResult pdf = null;
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingId);

            if (isAll)
            {
                userId = 0;
            }

            switch (reportingType)
            {
                case ReportingType.UserLoginActivity:

                    pdf = new PdfResult(
                        _service.GetUserLoginActivityList(fromDate, tillDateNew, Convert.ToInt32(userId)),
                        "ExportToPDF");
                    break;

                case ReportingType.PasswordDisablesLog:

                    // corporateId = Helpers.GetDefaultCorporateId();
                    pdf =
                        new PdfResult(
                            _service.GetPasswordDisableLog_SP(fromDate, tillDateNew, isAll, corporateId, facilityId),
                            "ExportToPDF");
                    break;

                case ReportingType.PasswordChangeLog:
                    pdf =
                        new PdfResult(
                            _service.GetPasswordChangeLog_SP(fromDate, tillDateNew, isAll, corporateId, facilityId),
                            "ExportToPDF");
                    break;

                case ReportingType.DailyChargeReport:
                    pdf = new PdfResult(_service.GetUserLoginActivityList(fromDate, tillDateNew, useridNew), "ExportToPDF");
                    break;

                case ReportingType.ChargesByPayorReport:
                    pdf = new PdfResult(_service.GetUserLoginActivityList(fromDate, tillDateNew, useridNew), "ExportToPDF");
                    break;

                case ReportingType.PayorWiseAgeingReport:
                    pdf =
                        new PdfResult(
                            _service.GetAgeingReport(corporateId, facilityId, fromDate, Convert.ToInt32(reportingId)),
                            "ExportToPDF");
                    break;

                case ReportingType.PatientWiseAgeingReport:
                    pdf =
                        new PdfResult(
                            _service.GetAgeingReport(corporateId, facilityId, fromDate, Convert.ToInt32(reportingId)),
                            "ExportToPDF");
                    break;

                case ReportingType.DepartmentWiseAgeingReport:
                    pdf =
                        new PdfResult(
                            _service.GetAgeingReport(corporateId, facilityId, fromDate, Convert.ToInt32(reportingId)),
                            "ExportToPDF");
                    break;

                case ReportingType.DepartmentWiseReconciliationReport:
                case ReportingType.PayorWiseReconciliationReport:
                case ReportingType.PatientWiseReconciliationReport:
                    pdf =
                        new PdfResult(
                            _service.GetReconciliationReport(
                                corporateId,
                                facilityId,
                                fromDate,
                                viewtype,
                                Convert.ToInt32(reportingId)),
                            "ExportToPDF");
                    pdf.ViewBag.ViewType = viewtype;
                    break;

                case ReportingType.DepartmentUtilization:
                    pdf =
                        new PdfResult(
                            _service.GetDepartmentUtilizationReport(
                                corporateId,
                                fromDate,
                                tillDate,
                                2,
                                facilityId,
                                0,
                                Convert.ToInt32(0)),
                            "ExportToPDF");
                    break;

                case ReportingType.PhysicianActivityReport:
                    pdf = new PdfResult(
                        _service.GetPhysicianActivityReport(corporateId, facilityId, fromDate, tillDate, 0),
                        "ExportToPDF");
                    break;

                case ReportingType.PhysicianUtilization:
                    pdf =
                        new PdfResult(
                            _service.GetPhysicianUtilizationReport(
                                corporateId,
                                fromDate,
                                tillDate,
                                1,
                                facilityId,
                                0,
                                Convert.ToInt32(0)),
                            "ExportToPDF");
                    break;
                case ReportingType.ClaimTransactionDetailReport:
                    pdf =
                        new PdfResult(
                            _service.GetClaimTransDetails(
                                corporateId,
                                facilityId,
                                fromDate,
                                tillDate,
                                Convert.ToInt32(viewtype)),
                            "ExportToPDF");
                    break;
                default:
                    pdf = new PdfResult(_service.GetUserLoginActivityList(fromDate, tillDateNew, useridNew), "ExportToPDF");
                    break;
            }

            pdf.ViewBag.ReportingID = reportingId;
            pdf.ViewBag.Title = Helpers.ReportingTitleView(reportingId);
            return pdf;
        }

        /// <summary>
        ///     Exports to excel.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="isAll">if set to <c>true</c> [is all].</param>
        /// <param name="reportingId">The reporting identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="viewtype">The viewtype.</param>
        /// <returns></returns>
        public ActionResult ExportToExcel(
            DateTime fromDate,
            DateTime? tillDate,
            bool isAll,
            string reportingId,
            int? userId,
            string viewtype)
        {
            Response.AddHeader("Content-Type", "application/vnd.ms-excel.xls");
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingId);
            Response.AddHeader(
                "content-disposition",
                string.Format(
                    "attachment; filename={0}",
                    reportingType + "-" + CurrentDateTime.ToShortDateString() + ".xls"));

            int corporateid = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetDefaultFacilityId();
            DateTime tillDateNew = tillDate ?? Helpers.GetInvariantCultureDateTime();
            int useridNew = userId ?? 0;
            DateTime selecteddate = fromDate;

            if (isAll)
            {
                userId = 0;
            }

            switch (reportingType)
            {
                case ReportingType.UserLoginActivity:
                    List<LoginTrackingCustomModel> userLoginList = _service.GetUserLoginActivityList(
                        fromDate,
                        tillDateNew,
                        useridNew);
                    return PartialView(PartialViews.UserLoginActivityView, userLoginList);

                case ReportingType.PasswordDisablesLog:

                    // corporateid = Helpers.GetDefaultCorporateId();
                    List<AuditLogCustomModel> pwdDisabledLogs = _service.GetPasswordDisableLog_SP(
                        fromDate,
                        tillDateNew,
                        isAll,
                        corporateid,
                        facilityid);
                    return PartialView(PartialViews.PasswordDisablesLogView, pwdDisabledLogs);

                case ReportingType.PasswordChangeLog:
                    List<AuditLogCustomModel> passwordChangeLogs = _service.GetPasswordChangeLog_SP(
                        fromDate,
                        tillDateNew,
                        isAll,
                        corporateid,
                        facilityid);
                    return PartialView(PartialViews.PasswordChangeLogView, passwordChangeLogs);

                case ReportingType.DailyChargeReport:
                    List<LoginTrackingCustomModel> dailyChargeReport = _service.GetUserLoginActivityList(
                        fromDate,
                        tillDateNew,
                        useridNew);
                    return PartialView(PartialViews.DailyChargeReportView, dailyChargeReport);

                case ReportingType.ChargesByPayorReport:
                    List<LoginTrackingCustomModel> chargesByPayorReport = _service.GetUserLoginActivityList(
                        fromDate,
                        tillDateNew,
                        useridNew);
                    return PartialView(PartialViews.ChargesByPayorReportView, chargesByPayorReport);

                // ...................Ageing Report starts here
                case ReportingType.PayorWiseAgeingReport: // .........By Payor wise
                    List<AgingReportCustomModel> ageingPayorReportData = _service.GetAgeingReport(
                        corporateid,
                        facilityid,
                        selecteddate,
                        Convert.ToInt32(reportingId));
                    return PartialView(PartialViews.PayorWiseAgeingReport, ageingPayorReportData);

                case ReportingType.PatientWiseAgeingReport: // .........By Patient wise
                    List<AgingReportCustomModel> ageingPatientReportData = _service.GetAgeingReport(
                        corporateid,
                        facilityid,
                        selecteddate,
                        Convert.ToInt32(reportingId));
                    return PartialView(PartialViews.PatientWiseAgeingReport, ageingPatientReportData);

                case ReportingType.DepartmentWiseAgeingReport: // .........By Department wise
                    List<AgingReportCustomModel> ageingDepartmentReportData = _service.GetAgeingReport(
                        corporateid,
                        facilityid,
                        selecteddate,
                        Convert.ToInt32(reportingId));
                    return PartialView(PartialViews.DepartmentWiseAgeingReport, ageingDepartmentReportData);

                // ................Ageing Report End Here

                // ...................Reconciliation Report starts here
                case ReportingType.DepartmentWiseReconciliationReport:
                case ReportingType.PayorWiseReconciliationReport:
                case ReportingType.PatientWiseReconciliationReport:
                    List<ReconcilationReportCustomModel> reconciliationReportData =
                        _service.GetReconciliationReport(
                            corporateid,
                            facilityid,
                            selecteddate,
                            viewtype,
                            Convert.ToInt32(reportingId));
                    switch (viewtype)
                    {
                        case "M": // Monthly View
                            return PartialView(PartialViews.ReconcilationARMonthWise, reconciliationReportData);

                        case "Y": // Yearly View
                            return PartialView(PartialViews.ReconcilationARYearWise, reconciliationReportData);

                        default: // Weekly View
                            return PartialView(PartialViews.ReconcilationARWeekWise, reconciliationReportData);
                    }

                // ................Reconciliation Report End Here
                case ReportingType.CorrectionLog:
                    List<ScrubEditTrackCustomModel> correctionLogReport = _service.GetBillEditCorrectionLogs(
                        corporateid,
                        facilityid,
                        fromDate,
                        tillDate,
                        isAll);
                    return PartialView(PartialViews.ScrubEditTrackListReport, correctionLogReport);

                case ReportingType.ChargeReport:
                    List<ChargesReportCustomModel> chargeReportIP = _service.GetChargesReport(
                        corporateid,
                        facilityid,
                        fromDate,
                        tillDate,
                        Convert.ToDecimal(0.00),
                        1);
                    List<ChargesReportCustomModel> chargeReportOP = _service.GetChargesReport(
                        corporateid,
                        facilityid,
                        fromDate,
                        tillDate,
                        Convert.ToDecimal(0.00),
                        2);
                    List<ChargesReportCustomModel> chargeReportER = _service.GetChargesReport(
                        corporateid,
                        facilityid,
                        fromDate,
                        tillDate,
                        Convert.ToDecimal(0.00),
                        3);
                    var chargesReportViews = new ChargesReportViews
                    {
                        ERChargesList = chargeReportER,
                        IPChargesList = chargeReportIP,
                        OPChargesList = chargeReportOP
                    };
                    return PartialView(PartialViews.ChargesReport, chargesReportViews);

                case ReportingType.ChargeDetailReport:
                    List<ChargesReportCustomModel> chargeDetailReportIP = _service.GetChargesDetailReport(
                        corporateid,
                        facilityid,
                        fromDate,
                        tillDate,
                        Convert.ToDecimal(0.00),
                        1,
                        3);
                    List<ChargesReportCustomModel> chargeDetailReportOP = _service.GetChargesDetailReport(
                        corporateid,
                        facilityid,
                        fromDate,
                        tillDate,
                        Convert.ToDecimal(0.00),
                        2,
                        3);
                    List<ChargesReportCustomModel> chargeDetailReportER = _service.GetChargesDetailReport(
                        corporateid,
                        facilityid,
                        fromDate,
                        tillDate,
                        Convert.ToDecimal(0.00),
                        3,
                        3);
                    var chargesDetailReportViews = new ChargesReportViews
                    {
                        ERChargesList = chargeDetailReportER,
                        IPChargesList = chargeDetailReportIP,
                        OPChargesList = chargeDetailReportOP
                    };
                    return PartialView(PartialViews.ChargesDetailReport, chargesDetailReportViews);

                case ReportingType.DepartmentUtilization:
                    List<PhysicianDepartmentUtilizationCustomModel> departmentUtilizationReport =
                        _service.GetDepartmentUtilizationReport(corporateid, fromDate, tillDate, 2, facilityid, 0, 0);
                    return PartialView(PartialViews.DepartmentUtilization, departmentUtilizationReport);

                case ReportingType.PhysicianActivityReport:
                    List<PhysicianActivityCustomModel> physicianActivityReport =
                        _service.GetPhysicianActivityReport(corporateid, facilityid, fromDate, tillDate, 0);
                    return PartialView(PartialViews.PhysicianActivityReport, physicianActivityReport);

                case ReportingType.PhysicianUtilization:
                    List<PhysicianDepartmentUtilizationCustomModel> physicianUtilizationReport =
                        _service.GetPhysicianUtilizationReport(corporateid, fromDate, tillDate, 1, facilityid, 0, 0);
                    return PartialView(PartialViews.PhysicianUtilization, physicianUtilizationReport);

                case ReportingType.ErrorSummary:
                    List<ScrubHeaderCustomModel> errorSummaryList =
                        _srService.GetErrorSummaryByRuleCode(corporateid, facilityid, fromDate, tillDate);
                    return PartialView(PartialViews.ErrorSummary, errorSummaryList);

                case ReportingType.ErrorDetail:
                    List<ScrubHeaderCustomModel> errorList =
                        _srService.GetErrorDetailByRuleCode(corporateid, facilityid, fromDate, tillDate);
                    return PartialView(PartialViews.ErrorDetail, errorList);

                case ReportingType.ScrubbeSummary:
                    List<ScrubHeaderCustomModel> scrubList = _srService.GetScrubSummaryList(
                        corporateid,
                        facilityid,
                        fromDate,
                        tillDate);
                    return PartialView(PartialViews.ScrubberSummary, scrubList);

                case ReportingType.ClaimTransactionDetailReport:
                    List<BillTransmissionReportCustomModel> claimtransactionDetails =
                        _service.GetClaimTransDetails(
                            corporateid,
                            facilityid,
                            fromDate,
                            tillDate,
                            Convert.ToInt32(viewtype));
                    return PartialView(PartialViews.ClaimTransDetailReport, claimtransactionDetails);

                case ReportingType.DenialReport:
                    List<DenialReportCustomModel> denialreportDetail = _service.GetDenialCodesReport(
                        corporateid,
                        facilityid,
                        fromDate,
                        tillDate,
                        Convert.ToInt32(viewtype));
                    return PartialView(PartialViews.DenialReport, denialreportDetail);
                case ReportingType.JournalEntrySupport:
                    List<JournalEntrySupportReportCustomModel> journalEntrySupportReportDetail =
                        _service.GetJournalEntrySupport(corporateid, facilityid, fromDate, tillDate, Convert.ToInt32(1));
                    return PartialView(PartialViews.JournalEntrySupportReport, journalEntrySupportReportDetail);
            }

            List<LoginTrackingCustomModel> list = _service.GetUserLoginActivityList(fromDate, tillDateNew, useridNew);
            return PartialView(PartialViews.UserLoginActivityView, list);
        }

        /// <summary>
        ///     Gets the type of the list by reporting.
        /// </summary>
        /// <param name="reportingTypeId">
        ///     The reporting type identifier.
        /// </param>
        /// <param name="date">
        ///     The date.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult AgeingReport(string reportingTypeId, DateTime? date)
        {
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingTypeId);
            int corporateid = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetDefaultFacilityId();
            DateTime selecteddate = date ?? Helpers.GetInvariantCultureDateTime();
            List<AgingReportCustomModel> ageingReportData = _service.GetAgeingReport(
                corporateid,
                facilityid,
                selecteddate,
                Convert.ToInt32(reportingTypeId));
            switch (reportingType)
            {
                case ReportingType.PayorWiseAgeingReport:
                    return PartialView(PartialViews.PayorWiseAgeingReport, ageingReportData);

                case ReportingType.PatientWiseAgeingReport:
                    return PartialView(PartialViews.PatientWiseAgeingReport, ageingReportData);

                case ReportingType.DepartmentWiseAgeingReport:
                    return PartialView(PartialViews.DepartmentWiseAgeingReport, ageingReportData);

                default:
                    break;
            }

            return PartialView(PartialViews.RoleList, null);
        }

        /// <summary>
        ///     Reconciliations the report.
        /// </summary>
        /// <param name="reportingTypeId">
        ///     The reporting type identifier.
        /// </param>
        /// <param name="date">
        ///     The date.
        /// </param>
        /// <param name="viewtype">
        ///     The viewtype.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ReconciliationReport(string reportingTypeId, DateTime? date, string viewtype)
        {
            int corporateid = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetDefaultFacilityId();
            DateTime selecteddate = date ?? Helpers.GetInvariantCultureDateTime();
            List<ReconcilationReportCustomModel> reconciliationReportData = _service.GetReconciliationReport(
                corporateid,
                facilityid,
                selecteddate,
                viewtype,
                Convert.ToInt32(reportingTypeId));
            switch (viewtype)
            {
                case "M":
                    return PartialView(PartialViews.ReconcilationARMonthWise, reconciliationReportData);

                case "Y":
                    return PartialView(PartialViews.ReconcilationARYearWise, reconciliationReportData);

                case "W":
                    return PartialView(PartialViews.ReconcilationARWeekWise, reconciliationReportData);

                default:
                    break;
            }

            return PartialView(PartialViews.RoleList, null);
        }

        // public ActionResult UnclosedEncountersReport()
        // {
        // var facilityId = Helpers.GetDefaultFacilityId();
        // var corporateId = Helpers.GetSysAdminCorporateID();
        // using (var _service = new EncounterBal())
        // {
        // var list = _service.GetUnclosedEncounters(facilityId, corporateId);
        // return PartialView(PartialViews.UnclosedEncountersView, list);
        // }
        // }

        /// <summary>
        ///     Gets the bill format.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetBillFormat()
        {
            return PartialView(PartialViews.BillFormat, null);
        }

        /// <summary>
        ///     Gets the revenue report by patient identifier.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetRevenueReportByPatientId(int patientId)
        {
            List<RevenueForecast> revenueReport = _service.GetRevenueForecastFacilityByPatient(patientId);
            return PartialView(PartialViews.RevenueForecastFacilityViewByPatient, revenueReport);
        }

        /// <summary>
        ///     Gets the bill with details format.
        /// </summary>
        /// <param name="billHeaderId">
        ///     The bill Header Id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetBillWithDetailsFormat(int billHeaderId)
        {
            BillHeaderCustomModel billheaderObj = _bhService.GetBillHeaderById(billHeaderId);
            int facilityId = billheaderObj.FacilityID ?? Helpers.GetDefaultFacilityId();
            var facilityObj = _fService.GetFacilityById(facilityId);

            int patientId = billheaderObj.PatientID ?? 0;
            PatientInfo patientinfoObj = _piService.GetPatientInfoById(patientId);

            int encounterId = billheaderObj.EncounterID ?? 0;
            Encounter encounterObj = _eService.GetEncounterByEncounterId(encounterId);


            var billActivtiesList = _baService.GetBillActivitiesByBillHeaderId(billHeaderId);
            var billFormatView = new BillFormatDetailView
            {
                BillDetails = billActivtiesList.Any() ? billActivtiesList
                                                 : new List<BillDetailCustomModel>(),
                BillHeaderDeatils = _mapper.Map<BillHeader>(billheaderObj),
                EncounterDetails = encounterObj ?? new Encounter(),
                FacilityDetails = facilityObj ?? new Facility(),
                PatientDetails = patientinfoObj ?? new PatientInfo()
            };
            return PartialView("../Reporting/" + PartialViews.BillFormat, billFormatView);
        }

        /// <summary>
        ///     Gets the bill with details PDF format.
        /// </summary>
        /// <param name="billHeaderId">
        ///     The bill header identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetBillWithDetailsPdfFormat1(int billHeaderId)
        {
            PdfResult pdf = null;

            var billheaderObj = _bhService.GetBillHeaderById(billHeaderId);
            int facilityId = billheaderObj.FacilityID ?? Helpers.GetDefaultFacilityId();
            var facilityObj = _fService.GetFacilityById(facilityId);

            int patientId = billheaderObj.PatientID ?? 0;
            PatientInfo patientinfoObj = _piService.GetPatientInfoById(patientId);

            int encounterId = billheaderObj.EncounterID ?? 0;
            Encounter encounterObj = _eService.GetEncounterByEncounterId(encounterId);


            List<BillDetailCustomModel> billActivtiesList = _baService.GetBillActivitiesByBillHeaderId(billHeaderId);

            var billFormatView = new BillFormatDetailView
            {
                BillDetails =
                                             billActivtiesList.Any()
                                                 ? billActivtiesList
                                                 : new List<BillDetailCustomModel>(),
                BillHeaderDeatils = _mapper.Map<BillHeader>(billheaderObj),
                EncounterDetails = encounterObj ?? new Encounter(),
                FacilityDetails = facilityObj ?? new Facility(),
                PatientDetails = patientinfoObj ?? new PatientInfo()
            };
            pdf = new PdfResult(billFormatView, "BillExportToPDF");
            return pdf;
        }

        /// <summary>
        ///     Logins the detail view report.
        /// </summary>
        /// <param name="userid">
        ///     The userid.
        /// </param>
        /// <param name="tillDate">
        ///     The till date.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult LoginDetailViewReport(int userid, DateTime tillDate)
        {
            int corporateid = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetDefaultFacilityId();
            var userLoginList = _service.GetUserLoginActivityDetailList(userid, tillDate);
            return PartialView(PartialViews.UserLoginActivityView, userLoginList);
        }

        /// <summary>
        ///     Gets the type of the list by reporting.
        /// </summary>
        /// <param name="payorId">
        ///     The payor identifier.
        /// </param>
        /// <param name="date">
        ///     The date.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult AgeingReportByPayor(string payorId, DateTime? date)
        {
            int corporateid = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetDefaultFacilityId();
            DateTime selecteddate = date ?? Helpers.GetInvariantCultureDateTime();
            var ageingReportData = _service.GetPatientAgeingPayorWise(
                corporateid,
                facilityid,
                selecteddate,
                payorId);
            return PartialView(PartialViews.PatientWiseAgeingReport, ageingReportData);
        }

        /// <summary>
        ///     Gets the charges reports data.
        /// </summary>
        /// <param name="reportingTypeId">
        ///     The reporting type identifier.
        /// </param>
        /// <param name="fromDate">
        ///     From date.
        /// </param>
        /// <param name="tillDate">
        ///     The till date.
        /// </param>
        /// <param name="departmentNumber">
        ///     The department number.
        /// </param>
        /// <param name="payorId">
        ///     The payor Id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetChargesReportsData(
            string reportingTypeId,
            DateTime? fromDate,
            DateTime? tillDate,
            string departmentNumber,
            int payorId)
        {
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingTypeId);
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            decimal departmentnumber = !string.IsNullOrEmpty(departmentNumber)
                                           ? Convert.ToDecimal(departmentNumber)
                                           : Convert.ToDecimal(0.00);
            switch (reportingType)
            {
                case ReportingType.ChargeReport:
                    List<ChargesReportCustomModel> chargeReportIP = _service.GetChargesReport(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate,
                        departmentnumber,
                        1);
                    List<ChargesReportCustomModel> chargeReportOP = _service.GetChargesReport(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate,
                        departmentnumber,
                        2);
                    List<ChargesReportCustomModel> chargeReportER = _service.GetChargesReport(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate,
                        departmentnumber,
                        3);
                    var chargesReportViews = new ChargesReportViews
                    {
                        ERChargesList = chargeReportER,
                        IPChargesList = chargeReportIP,
                        OPChargesList = chargeReportOP
                    };
                    return PartialView(PartialViews.ChargesReport, chargesReportViews);

                case ReportingType.ChargeDetailReport:
                    List<ChargesReportCustomModel> chargeDetailReportIP = _service.GetChargesDetailReport(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate,
                        departmentnumber,
                        1,
                        payorId);
                    List<ChargesReportCustomModel> chargeDetailReportOP = _service.GetChargesDetailReport(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate,
                        departmentnumber,
                        2,
                        payorId);
                    List<ChargesReportCustomModel> chargeDetailReportER = _service.GetChargesDetailReport(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate,
                        departmentnumber,
                        3,
                        payorId);
                    var chargesDetailReportViews = new ChargesReportViews
                    {
                        ERChargesList = chargeDetailReportER,
                        IPChargesList = chargeDetailReportIP,
                        OPChargesList = chargeDetailReportOP
                    };
                    return PartialView(PartialViews.ChargesDetailReport, chargesDetailReportViews);
            }

            return null;
        }

        /// <summary>
        ///     TODO The get physician activity report.
        /// </summary>
        /// <param name="reportingTypeId">
        ///     TODO The reporting type id.
        /// </param>
        /// <param name="fromDate">
        ///     TODO The from date.
        /// </param>
        /// <param name="tillDate">
        ///     TODO The till date.
        /// </param>
        /// <param name="facilityId">
        ///     TODO The facility id.
        /// </param>
        /// <param name="physicianId">
        ///     TODO The physician id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetPhysicianActivityReport(
            string reportingTypeId,
            DateTime? fromDate,
            DateTime? tillDate,
            int facilityId,
            int physicianId)
        {
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingTypeId);
            int corporateId = Helpers.GetSysAdminCorporateID();
            switch (reportingType)
            {
                case ReportingType.PhysicianActivityReport:
                    List<PhysicianActivityCustomModel> physicianActivityReport =
                        _service.GetPhysicianActivityReport(corporateId, facilityId, fromDate, tillDate, physicianId);
                    return PartialView(PartialViews.PhysicianActivityReport, physicianActivityReport);

                case ReportingType.PhysicianUtilization:
                    List<PhysicianDepartmentUtilizationCustomModel> physicianUtilizationReport =
                        _service.GetPhysicianUtilizationReport(
                            corporateId,
                            fromDate,
                            tillDate,
                            1,
                            facilityId,
                            physicianId,
                            0);
                    return PartialView(PartialViews.PhysicianUtilization, physicianUtilizationReport);
            }

            return Content("There is error while sending request to Server! Please try some time.");
        }

        /// <summary>
        ///     TODO The get department utilization report.
        /// </summary>
        /// <param name="fromDate">
        ///     TODO The from date.
        /// </param>
        /// <param name="tillDate">
        ///     TODO The till date.
        /// </param>
        /// <param name="facilityId">
        ///     TODO The facility id.
        /// </param>
        /// <param name="departmentId">
        ///     TODO The department id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetDepartmentUtilizationReport(
            DateTime? fromDate,
            DateTime? tillDate,
            int facilityId,
            int departmentId)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            var departmentUtilization = _service.GetDepartmentUtilizationReport(corporateId, fromDate, tillDate, 2, facilityId, 0, departmentId);
            return PartialView(PartialViews.DepartmentUtilization, departmentUtilization);
        }

        /// <summary>
        ///     Gets the future charge report.
        /// </summary>
        /// <param name="fromDate">
        ///     From date.
        /// </param>
        /// <param name="tillDate">
        ///     The till date.
        /// </param>
        /// <param name="facilityId">
        ///     The facility identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetFutureChargeReport(DateTime? fromDate, DateTime? tillDate, int facilityId)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            var futureChargeReport = _service.GetFutureChargeReport(
                corporateId,
                fromDate,
                tillDate,
                facilityId);
            return PartialView(PartialViews.FutureOpenOrders, futureChargeReport);
        }

        /// <summary>
        ///     TODO The get physicians by corporate and facility.
        /// </summary>
        /// <param name="facilityId">
        ///     TODO The facility id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetPhysiciansByCorporateAndFacility(int facilityId)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            List<Physician> physicianList = _phService.GetPhysicianByCorporateIdandfacilityId(corporateId, facilityId);
            return Json(physicianList);
        }

        /// <summary>
        ///     TODO The get scrubber and error summary report.
        /// </summary>
        /// <param name="reportingTypeId">
        ///     TODO The reporting type id.
        /// </param>
        /// <param name="fromDate">
        ///     TODO The from date.
        /// </param>
        /// <param name="tillDate">
        ///     TODO The till date.
        /// </param>
        /// <param name="facilityId">
        ///     TODO The facility id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetScrubberAndErrorSummaryReport(
            string reportingTypeId,
            DateTime? fromDate,
            DateTime? tillDate,
            int facilityId)
        {
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingTypeId);
            int corporateId = Helpers.GetSysAdminCorporateID();
            facilityId = facilityId != 0 ? facilityId : Helpers.GetDefaultFacilityId();
            switch (reportingType)
            {
                case ReportingType.ScrubbeSummary:

                    List<ScrubHeaderCustomModel> scrubList = _srService.GetScrubSummaryList(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate);
                    return PartialView(PartialViews.ScrubberSummary, scrubList);

                case ReportingType.ErrorDetail:
                    List<ScrubHeaderCustomModel> errorList = _srService.GetErrorDetailByRuleCode(
                        corporateId,
                        facilityId,
                        fromDate,
                        tillDate);
                    return PartialView(PartialViews.ErrorDetail, errorList);

                case ReportingType.ErrorSummary:
                    List<ScrubHeaderCustomModel> errorSummaryList =
                        _srService.GetErrorSummaryByRuleCode(corporateId, facilityId, fromDate, tillDate);
                    return PartialView(PartialViews.ErrorSummary, errorSummaryList);
            }

            return Content("There is error while sending request to Server! Please try some time.");
        }

        /// <summary>
        ///     TODO The get detail user transction report.
        /// </summary>
        /// <param name="fromDate">
        ///     TODO The from date.
        /// </param>
        /// <param name="tillDate">
        ///     TODO The till date.
        /// </param>
        /// <param name="facilityId">
        ///     TODO The facility id.
        /// </param>
        /// <param name="transctionId">
        ///     TODO The transction id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetDetailUserTransctionReport(
            DateTime? fromDate,
            DateTime? tillDate,
            int facilityId,
            int transctionId,
            int encounterId)
        {
            return PartialView(PartialViews.DetailUserTransction, new ChargesReportViews());
        }

        /// <summary>
        ///     TODO The get users by corporate id and facility id.
        /// </summary>
        /// <param name="facilityId">
        ///     TODO The facility id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetUsersByCorporateIdAndFacilityId(int facilityId)
        {
            var list = new List<DropdownListData>();
            int corporateId = Helpers.GetDefaultCorporateId();

            // var usersList = _service.GetUsersByCorporateId(corporateId);
            List<Users> usersList = _uService.GetUsersByCorporateandFacilityId(corporateId, facilityId);
            list.AddRange(
                usersList.Select(
                    item =>
                    new DropdownListData
                    {
                        Text = string.Format("{0} {1}", item.FirstName, item.LastName),
                        Value = item.UserID.ToString()
                    }));


            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     TODO The get encounter by user id.
        /// </summary>
        /// <param name="userId">
        ///     TODO The user id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetEncounterByUserId(int userId)
        {
            var list = new List<DropdownListData>();

            List<Encounter> encounterList = _eService.GetEncounterByUserId(userId);
            list.AddRange(encounterList.Select(item => new DropdownListData { Text = item.EncounterNumber, Value = item.EncounterID.ToString() }));


            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     TODO The get facility deapartments.
        /// </summary>
        /// <param name="facilityId">
        ///     TODO The facility id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetFacilityDeapartments(string facilityId)
        {
            var list = new List<SelectListItem>();
            List<FacilityStructure> facilityDepartments = _fsService.GetFacilityDepartments(Helpers.GetSysAdminCorporateID(), facilityId);
            if (facilityDepartments.Any())
            {
                list.AddRange(
                    facilityDepartments.Select(
                        item =>
                        new SelectListItem
                        {
                            Text = string.Format(" {0} ", item.FacilityStructureName),
                            Value = Convert.ToString(item.FacilityStructureId)
                        }));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     TODO The get physician activity report data.
        /// </summary>
        /// <param name="reportingTypeId">
        ///     TODO The reporting type id.
        /// </param>
        /// <param name="fromDate">
        ///     TODO The from date.
        /// </param>
        /// <param name="tillDate">
        ///     TODO The till date.
        /// </param>
        /// <param name="facilityId">
        ///     TODO The facility id.
        /// </param>
        /// <param name="physicianId">
        ///     TODO The physician id.
        /// </param>
        /// <param name="sort">
        ///     TODO The sort.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetPhysicianActivityReportData(
            string reportingTypeId,
            DateTime? fromDate,
            DateTime? tillDate,
            int facilityId,
            int physicianId,
            string sort)
        {
            return null;
        }

        /// <summary>
        /// PDFs the reports.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="isAll">if set to <c>true</c> [is all].</param>
        /// <param name="reportingId">The reporting identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="viewtype">The viewtype.</param>
        /// <returns></returns>
        public ActionResult PDFReports(
            DateTime fromDate,
            DateTime? tillDate,
            bool isAll,
            string reportingId,
            int? userId,
            string viewtype)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingId);
            var rpTitle = Helpers.ReportingTitleView(Convert.ToString(reportingId));
            try
            {
                #region reportPath

                LocalReport lr = new LocalReport();
                lr.SetBasePermissionsForSandboxAppDomain(new PermissionSet(PermissionState.Unrestricted));
                string path = "";
                ReportDataSource ReportDataSourceOrderDetail = null;
                ReportDataSource ReportDataSourceOrderDetail2 = null;
                ReportDataSource ReportDataSourceOrderDetail3 = null;
                switch (reportingType)
                {
                    case ReportingType.ClaimTransactionDetailReport:
                        path = Path.Combine(Server.MapPath("~/Reports"), "BillTransmissionReport.rdlc");
                        var claimtransactionDetails = _service.GetClaimTransDetails(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate,
                            Convert.ToInt32(viewtype));
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", claimtransactionDetails);
                        break;
                    case ReportingType.PayorWiseAgeingReport:
                        path = Path.Combine(Server.MapPath("~/Reports"), "PayorWiseAgeing.rdlc");
                        var dataset = _service.GetAgeingReport(
                            corporateId,
                            facilityId,
                            fromDate,
                            Convert.ToInt32(reportingId));
                        if (dataset.Count > 0) dataset.RemoveAt(dataset.Count - 1);
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", dataset);
                        break;
                    case ReportingType.PatientWiseAgeingReport:
                        path = Path.Combine(Server.MapPath("~/Reports"), "PatientWiseAgeing.rdlc");
                        var patientdataset = _service.GetAgeingReport(
                            corporateId,
                            facilityId,
                            fromDate,
                            Convert.ToInt32(reportingId));
                        if (patientdataset.Count > 0) patientdataset.RemoveAt(patientdataset.Count - 1);
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", patientdataset);
                        break;
                    case ReportingType.CorrectionLog:
                        path = Path.Combine(Server.MapPath("~/Reports"), "CorrectionLog.rdlc");
                        var correctionLogReport = _service.GetBillEditCorrectionLogs(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate,
                            isAll);
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", correctionLogReport);
                        break;
                    case ReportingType.ErrorSummary:
                        path = Path.Combine(Server.MapPath("~/Reports"), "ErrorSummary.rdlc");
                        var scrubList = _srService.GetErrorSummaryByRuleCode(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate);
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", scrubList);
                        break;
                    case ReportingType.ErrorDetail:
                        path = Path.Combine(Server.MapPath("~/Reports"), "ErrorDetail.rdlc");
                        var scrubErrorDetailList = _srService.GetErrorDetailByRuleCode(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate);
                        scrubErrorDetailList =
                            scrubErrorDetailList.Where(x => !x.RuleDescription.Equals("TOTAL")).ToList();
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", scrubErrorDetailList);
                        break;
                    case ReportingType.ScrubbeSummary:
                        path = Path.Combine(Server.MapPath("~/Reports"), "ScrubSummary.rdlc");
                        var scrubbersummary = _srService.GetScrubSummaryList(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate);
                        scrubbersummary = scrubbersummary.Where(x => !x.EncounterNumber.Equals("TOTAL")).ToList();
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", scrubbersummary);
                        break;
                    case ReportingType.DenialReport:
                        path = Path.Combine(Server.MapPath("~/Reports"), "DenialCode.rdlc");
                        var denialreportDetail = _service.GetDenialCodesReport(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate,
                            Convert.ToInt32(viewtype));
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", denialreportDetail);
                        break;
                    case ReportingType.ChargeDetailReport:
                        path = Path.Combine(Server.MapPath("~/Reports"), "ChargesDetailReport.rdlc");
                        List<ChargesReportCustomModel> chargeDetailReportIP = _service.GetChargesDetailReport(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate,
                            Convert.ToDecimal(userId),
                            1,
                            Convert.ToInt32(viewtype));
                        List<ChargesReportCustomModel> chargeDetailReportOP = _service.GetChargesDetailReport(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate,
                            Convert.ToDecimal(userId),
                            2,
                            Convert.ToInt32(viewtype));
                        List<ChargesReportCustomModel> chargeDetailReportER = _service.GetChargesDetailReport(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate,
                            Convert.ToDecimal(userId),
                            3,
                            Convert.ToInt32(viewtype));
                        chargeDetailReportIP =
                            chargeDetailReportIP.Where(x => !x.EncounterNumber.Equals("TOTAL")).ToList();
                        chargeDetailReportOP =
                            chargeDetailReportOP.Where(x => !x.EncounterNumber.Equals("TOTAL")).ToList();
                        chargeDetailReportER =
                            chargeDetailReportER.Where(x => !x.EncounterNumber.Equals("TOTAL")).ToList();
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", chargeDetailReportIP);
                        ReportDataSourceOrderDetail2 = new ReportDataSource("DataSet2", chargeDetailReportOP);
                        ReportDataSourceOrderDetail3 = new ReportDataSource("DataSet3", chargeDetailReportER);
                        break;
                    case ReportingType.ChargeReport:
                        path = Path.Combine(Server.MapPath("~/Reports"), "ChargesReport.rdlc");
                        List<ChargesReportCustomModel> chargeReportIP = _service.GetChargesReport(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate,
                            Convert.ToDecimal(userId),
                            1);
                        List<ChargesReportCustomModel> chargeReportOP = _service.GetChargesReport(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate,
                            Convert.ToDecimal(userId),
                            2);
                        List<ChargesReportCustomModel> chargeReportER = _service.GetChargesReport(
                            corporateId,
                            facilityId,
                            fromDate,
                            tillDate,
                            Convert.ToDecimal(userId),
                            3);
                        chargeReportIP = chargeReportIP.Where(x => !x.ActivityType.Equals("TOTAL")).ToList();
                        chargeReportOP = chargeReportOP.Where(x => !x.ActivityType.Equals("TOTAL")).ToList();
                        chargeReportER = chargeReportER.Where(x => !x.ActivityType.Equals("TOTAL")).ToList();
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", chargeReportIP);
                        ReportDataSourceOrderDetail2 = new ReportDataSource("DataSet2", chargeReportOP);
                        ReportDataSourceOrderDetail3 = new ReportDataSource("DataSet3", chargeReportER);
                        break;

                    case ReportingType.PhysicianActivityReport:
                        path = Path.Combine(Server.MapPath("~/Reports"), "PhysicianActivityReport.rdlc");
                        var physicianActivityReport = _service.GetPhysicianActivityReport(
                            corporateId,
                            Convert.ToInt32(userId),
                            fromDate,
                            tillDate,
                            Convert.ToInt32(viewtype));
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", physicianActivityReport);
                        break;
                    case ReportingType.JournalEntrySupport:
                        path = Path.Combine(Server.MapPath("~/Reports"), "JournalEntrySupportChargesAccReport.rdlc");
                        List<JournalEntrySupportReportCustomModel> journalEntrySupportReportDetail =
                            _service.GetJournalEntrySupport(corporateId, facilityId, fromDate, tillDate, Convert.ToInt32(1));
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", journalEntrySupportReportDetail);
                        break;
                    case ReportingType.PayorWiseReconciliationReport:
                        if (viewtype == "M")
                        {
                            path = Path.Combine(
                                Server.MapPath("~/Reports"),
                                "PayorWiseReconciliationReportMonthly.rdlc");
                            List<ReconcilationReportCustomModel> reconciliationReportData =
                                _service.GetReconciliationReport(
                                    corporateId,
                                    facilityId,
                                    fromDate,
                                    viewtype,
                                    Convert.ToInt32(reportingId));
                            ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", reconciliationReportData);
                        }
                        else if (viewtype == "Y")
                        {
                            path = Path.Combine(Server.MapPath("~/Reports"), "PayorWiseReconciliationReport.rdlc");
                            List<ReconcilationReportCustomModel> reconciliationReportData =
                                _service.GetReconciliationReport(
                                    corporateId,
                                    facilityId,
                                    fromDate,
                                    viewtype,
                                    Convert.ToInt32(reportingId));
                            ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", reconciliationReportData);
                        }
                        else if (viewtype == "W")
                        {
                            path = Path.Combine(
                                Server.MapPath("~/Reports"),
                                "PatientWiseReconciliationReportWeekly.rdlc");
                            List<ReconcilationReportCustomModel> reconciliationPatinetReportDataW =
                                _service.GetReconciliationReport_Weekly(
                                    corporateId,
                                    facilityId,
                                    fromDate,
                                    viewtype,
                                    Convert.ToInt32(reportingId));
                            ReportDataSourceOrderDetail = new ReportDataSource(
                                "DataSet1",
                                reconciliationPatinetReportDataW);
                        }
                        break;
                    case ReportingType.PatientWiseReconciliationReport:
                        if (viewtype == "M")
                        {
                            path = Path.Combine(
                                Server.MapPath("~/Reports"),
                                "PatientWiseReconciliationReportMonthly.rdlc");
                            List<ReconcilationReportCustomModel> reconciliationPatinetReportDataM =
                                _service.GetReconciliationReport_Monthly(
                                    corporateId,
                                    facilityId,
                                    fromDate,
                                    viewtype,
                                    Convert.ToInt32(reportingId));
                            ReportDataSourceOrderDetail = new ReportDataSource(
                                "DataSet1",
                                reconciliationPatinetReportDataM);
                        }
                        else if (viewtype == "Y")
                        {
                            path = Path.Combine(Server.MapPath("~/Reports"), "PatientWiseReconciliationReport.rdlc");
                            List<ReconcilationReportCustomModel> reconciliationPatinetReportDataY =
                                _service.GetReconciliationReport(
                                    corporateId,
                                    facilityId,
                                    fromDate,
                                    viewtype,
                                    Convert.ToInt32(reportingId));
                            ReportDataSourceOrderDetail = new ReportDataSource(
                                "DataSet1",
                                reconciliationPatinetReportDataY);
                        }
                        else if (viewtype == "W")
                        {
                            path = Path.Combine(
                                Server.MapPath("~/Reports"),
                                "PatientWiseReconciliationReportWeekly.rdlc");
                            List<ReconcilationReportCustomModel> reconciliationPatinetReportDataW =
                                _service.GetReconciliationReport_Weekly(
                                    corporateId,
                                    facilityId,
                                    fromDate,
                                    viewtype,
                                    Convert.ToInt32(reportingId));
                            ReportDataSourceOrderDetail = new ReportDataSource(
                                "DataSet1",
                                reconciliationPatinetReportDataW);
                        }
                        break;
                    case ReportingType.PasswordDisablesLog:
                        path = Path.Combine(Server.MapPath("~/Reports"), "PasswordDisableLog.rdlc");
                        List<AuditLogCustomModel> pwdDisabledLogs = _service.GetPasswordDisableLog_SP(
                            fromDate,
                            Convert.ToDateTime(tillDate),
                            isAll,
                            corporateId,
                            facilityId);
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", pwdDisabledLogs);
                        break;
                    case ReportingType.PasswordChangeLog:
                        path = Path.Combine(Server.MapPath("~/Reports"), "PasswordChangeLog.rdlc");
                        List<AuditLogCustomModel> passwordChangeLogs = _service.GetPasswordChangeLog_SP(
                            fromDate,
                            Convert.ToDateTime(tillDate),
                            isAll,
                            corporateId,
                            facilityId);
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", passwordChangeLogs);
                        break;
                    case ReportingType.UserLoginActivity:
                        path = Path.Combine(Server.MapPath("~/Reports"), "UserActivityLog.rdlc");
                        List<LoginTrackingCustomModel> userLoginList = _service.GetUserLoginActivityList_SP(
                            fromDate,
                            tillDate,
                            Convert.ToInt32(userId),
                            corporateId,
                            facilityId);
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", userLoginList);
                        break;
                    case ReportingType.PhysicianUtilization:
                        path = Path.Combine(Server.MapPath("~/Reports"), "PhysicianUtilizationReport.rdlc");
                        List<PhysicianDepartmentUtilizationCustomModel> physicianUtilizationReport =
                            _service.GetPhysicianUtilizationReport(
                                corporateId,
                                fromDate,
                                tillDate,
                                1,
                                facilityId,
                                Convert.ToInt32(viewtype),
                                0);
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", physicianUtilizationReport);
                        break;
                    case ReportingType.DepartmentUtilization:
                        path = Path.Combine(Server.MapPath("~/Reports"), "PhysicianUtilizationReport.rdlc");
                        List<PhysicianDepartmentUtilizationCustomModel> departmentUtilization =
                            _service.GetDepartmentUtilizationReport(
                                corporateId,
                                fromDate,
                                tillDate,
                                2,
                                facilityId,
                                0,
                                Convert.ToInt32(viewtype));
                        ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", departmentUtilization);
                        break;
                }
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                #endregion reportPath

                #region DynamicReportParameter

                var rpReportName = new ReportParameter("pReportname", rpTitle);
                if (reportingType == ReportingType.DenialReport)
                {
                    var rpReportType = new ReportParameter("pViewType", viewtype);
                    lr.SetParameters(rpReportType);
                }

                #endregion DynamicReportParameter

                #region AddDataSources

                lr.DataSources.Add(ReportDataSourceOrderDetail);
                if (reportingType == ReportingType.ChargeDetailReport || reportingType == ReportingType.ChargeReport)
                {
                    lr.DataSources.Add(ReportDataSourceOrderDetail2);
                    lr.DataSources.Add(ReportDataSourceOrderDetail3);
                }
                lr.SetParameters(rpReportName);

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                #endregion AddDataSources

                #region deviceInfo

                string deviceInfo = "<DeviceInfo>" + "  <OutputFormat>PDF</OutputFormat>"
                                    + "  <PageWidth>8in</PageWidth>" + //12.64835in previous
                                    "  <PageHeight>12in</PageHeight>" + //11.69in previous
                                    "  <MarginTop>0.5in</MarginTop>" + "  <MarginLeft>0.3in</MarginLeft>"
                                    + "  <MarginRight>0.3in</MarginRight>" + "  <MarginBottom>0.5in</MarginBottom>"
                                    + "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = lr.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                #endregion deviceInfo

                return File(renderedBytes, mimeType);
            }
            catch (Exception ex)
            {
                return Json(
                    ex.Message + "<BR/>" + ex.InnerException + "\n\n" + ex.StackTrace,
                    JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the bill with details PDF format.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public ActionResult GetBillWithDetailsPdfFormat(int billHeaderId)
        {
            try
            {
                #region reportPath

                LocalReport lr = new LocalReport();
                lr.SetBasePermissionsForSandboxAppDomain(new PermissionSet(PermissionState.Unrestricted));
                string path = "";
                ReportDataSource ReportDataSourceOrderDetail = null;
                path = Path.Combine(Server.MapPath("~/Reports"), "BillPdfFormat.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                #endregion reportPath

                #region Data Detail

                List<BillDetailCustomModel> billActivtiesList =
                    _baService.GetBillActivitiesByBillHeaderId(billHeaderId);
                List<BillPdfFormatCustomModel> otherBillFormatData = _baService.GetBillPdfFormat(billHeaderId);
                ReportDataSourceOrderDetail = new ReportDataSource("DataSet1", billActivtiesList);
                var ReportDataSourceOrderDetail2 = new ReportDataSource("DataSet2", otherBillFormatData);

                #endregion DataDetail

                #region DynamicReportParameter

                //var rpReportName = new ReportParameter("pReportname", rpTitle);

                #endregion DynamicReportParameter

                #region AddDataSources

                lr.DataSources.Add(ReportDataSourceOrderDetail);
                lr.DataSources.Add(ReportDataSourceOrderDetail2);

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                #endregion AddDataSources

                #region deviceInfo

                string deviceInfo = "<DeviceInfo>" + "  <OutputFormat>PDF</OutputFormat>"
                                    + "  <PageWidth>8in</PageWidth>" + //12.64835in previous
                                    "  <PageHeight>12in</PageHeight>" + //11.69in previous
                                                                        // "  <PageHeight>14.71909in</PageHeight>" +
                                    "  <MarginTop>0.5in</MarginTop>" + "  <MarginLeft>0.3in</MarginLeft>"
                                    + "  <MarginRight>0.3in</MarginRight>" + "  <MarginBottom>0.5in</MarginBottom>"
                                    + "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = lr.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                #endregion deviceInfo

                return File(renderedBytes, mimeType);
            }
            catch (Exception)
            {
                return Json("Report Does not Exist", JsonRequestBehavior.AllowGet);
            }
        }

        #region XmlReportingReports

        /// <summary>
        ///     XMLs the reporting.
        /// </summary>
        /// <param name="reportingId">
        ///     The reporting identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult XMLReporting(int? reportingId)
        {
            int reportingTypeId = Convert.ToInt32(reportingId);
            var reporting = new XmlReportingView
            {
                FromDate = Helpers.GetFirstDayofCurrentMonth(),
                ToDate = Helpers.GetLastDayOfCurrentMonth(),
                ReportingType = reportingTypeId,
                Title =
                                        Helpers.XmlReportingTitleView(Convert.ToString(reportingTypeId)),

                // ReportingTypeAction = Helpers.GetReportingTypeAction(Convert.ToString(reportingTypeId)),
                UserId = Helpers.GetLoggedInUserId(),
                ViewType = "Y",
                CorporateId = Helpers.GetDefaultCorporateId()
            };
            return View(reporting);
        }

        /// <summary>
        ///     Gets the XML batch report.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetXMLBatchReport()
        {
            int fID = Helpers.GetDefaultFacilityId();
            int cID = Helpers.GetSysAdminCorporateID();
            List<XmlReportingBatchReport> listToReturn = _xrService.GetBatchReort(cID, fID);
            return PartialView(PartialViews.XMLBillingBatchReport, listToReturn);
        }

        /// <summary>
        ///     Gets the XML initial claim error report.
        /// </summary>
        /// <param name="startdate">
        ///     The startdate.
        /// </param>
        /// <param name="enddate">
        ///     The enddate.
        /// </param>
        /// <param name="encType">
        ///     Type of the enc.
        /// </param>
        /// <param name="clinicalId">
        ///     The clinical identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetXMLInitialClaimErrorReport(
            DateTime startdate,
            DateTime enddate,
            string encType,
            string clinicalId)
        {
            int fID = Helpers.GetDefaultFacilityId();
            int cID = Helpers.GetSysAdminCorporateID();
            List<XmlReportingInitialClaimErrorReport> listToReturn = _xrService.GetInitialClaimErrorReport(
                cID,
                fID,
                startdate,
                enddate,
                encType,
                clinicalId);
            return PartialView(PartialViews.XMLBillingInitialClaimErrorReport, listToReturn);
        }

        #endregion XmlReportingReports

        #region Sorting By Krishna on 21082015

        /// <summary>
        ///     Sorts the password log grid.
        /// </summary>
        /// <param name="fromDate">
        ///     From date.
        /// </param>
        /// <param name="tillDate">
        ///     The till date.
        /// </param>
        /// <param name="isAll">
        ///     if set to <c> true </c> [is all].
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortPasswordLogGrid(DateTime fromDate, DateTime tillDate, bool isAll)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetDefaultFacilityId();
            List<AuditLogCustomModel> passwordChangeLogs = _service.GetPasswordChangeLog_SP(
                fromDate,
                tillDate,
                isAll,
                corporateId,
                facilityid);
            return PartialView(PartialViews.PasswordChangeLogView, passwordChangeLogs);
        }

        /// <summary>
        ///     Sorts the password disable log grid.
        /// </summary>
        /// <param name="fromDate">
        ///     From date.
        /// </param>
        /// <param name="tillDate">
        ///     The till date.
        /// </param>
        /// <param name="isAll">
        ///     if set to <c> true </c> [is all].
        /// </param>
        /// <param name="userId">
        ///     The user identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortPasswordDisableLogGrid(DateTime fromDate, DateTime tillDate, bool isAll, int? userId)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetDefaultFacilityId();

            // corporateId = Helpers.GetDefaultCorporateId();
            int useridnotnull = userId == null ? 0 : Convert.ToInt32(userId);

            corporateId = Helpers.GetDefaultCorporateId();
            List<AuditLogCustomModel> pwdDisabledLogs = _service.GetPasswordDisableLog_SP(
                fromDate,
                tillDate,
                isAll,
                corporateId,
                facilityid);
            return PartialView(PartialViews.PasswordDisablesLogView, pwdDisabledLogs);
        }

        /// <summary>
        ///     Sorts the user log activty grid.
        /// </summary>
        /// <param name="fromDate">
        ///     From date.
        /// </param>
        /// <param name="tillDate">
        ///     The till date.
        /// </param>
        /// <param name="isAll">
        ///     The is All.
        /// </param>
        /// <param name="userId">
        ///     The user identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortUserLogActivityGrid(DateTime fromDate, DateTime tillDate, bool isAll, int? userId)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            if (isAll)
            {
                userId = 0;
            }

            corporateId = Helpers.GetDefaultCorporateId();
            List<LoginActivityReportCustomModel> userLoginList = _service.GetLoginTimeDayNightShift(
                corporateId,
                facilityId,
                fromDate,
                tillDate,
                Convert.ToInt32(userId));
            return PartialView(PartialViews.LoginTrackingDayNightShiftReport, userLoginList);
        }

        /// <summary>
        ///     TODO The sort daily charge report grid.
        /// </summary>
        /// <param name="fromDate">
        ///     TODO The from date.
        /// </param>
        /// <param name="tillDate">
        ///     TODO The till date.
        /// </param>
        /// <param name="departmentNumber">
        ///     TODO The department number.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortDailyChargeReportGrid(DateTime fromDate, DateTime tillDate, string departmentNumber)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();

            decimal departmentnumber = !string.IsNullOrEmpty(departmentNumber)
                                           ? Convert.ToDecimal(departmentNumber)
                                           : Convert.ToDecimal(0.00);

            List<ChargesReportCustomModel> chargeDetailReportIP = _service.GetChargesDetailReport(
                corporateId,
                facilityId,
                fromDate,
                tillDate,
                departmentnumber,
                1,
                3);

            return PartialView(PartialViews.ChargeDetailReport, chargeDetailReportIP);
        }

        /// <summary>
        ///     TODO The sort collection logt grid.
        /// </summary>
        /// <param name="fromDate">
        ///     TODO The from date.
        /// </param>
        /// <param name="tillDate">
        ///     TODO The till date.
        /// </param>
        /// <param name="isAll">
        ///     TODO The is all.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortCollectionLogtGrid(DateTime fromDate, DateTime tillDate, bool isAll)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            List<ScrubEditTrackCustomModel> correctionLogReport = _service.GetBillEditCorrectionLogs(
                corporateId,
                facilityId,
                fromDate,
                tillDate,
                isAll);
            return PartialView(PartialViews.ScrubEditTrackListReport, correctionLogReport);
        }

        /// <summary>
        ///     TODO The sort claim trans report grid.
        /// </summary>
        /// <param name="fromDate">
        ///     TODO The from date.
        /// </param>
        /// <param name="tillDate">
        ///     TODO The till date.
        /// </param>
        /// <param name="displayby">
        ///     TODO The displayby.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortClaimTransReportGrid(DateTime fromDate, DateTime tillDate, int displayby)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();

            List<BillTransmissionReportCustomModel> claimtransactionDetails = _service.GetClaimTransDetails(
                corporateId,
                facilityId,
                fromDate,
                tillDate,
                displayby);
            return PartialView(PartialViews.ClaimTransDetailReport, claimtransactionDetails);
        }

        /// <summary>
        ///     TODO The sort revenu forcast report grid.
        /// </summary>
        /// <param name="fromDate">
        ///     TODO The from date.
        /// </param>
        /// <param name="tillDate">
        ///     TODO The till date.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortRevenuForcastReportGrid(DateTime fromDate, DateTime tillDate)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();

            List<RevenueForecast> revenueReport = _service.GetRevenueForecastFacility(
                corporateId,
                facilityId,
                fromDate,
                tillDate);
            return PartialView(PartialViews.RevenueForecastFacilityView, revenueReport);
        }

        /// <summary>
        ///     TODO The journal entry support report grid.
        /// </summary>
        /// <param name="fromDate">
        ///     TODO The from date.
        /// </param>
        /// <param name="tillDate">
        ///     TODO The till date.
        /// </param>
        /// <param name="displayby">
        ///     TODO The displayby.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult JournalEntrySupportReportGrid(DateTime fromDate, DateTime tillDate, int displayby)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();

            List<JournalEntrySupportReportCustomModel> journalEntrySupportReportDetail =
                _service.GetJournalEntrySupport(corporateId, facilityId, fromDate, tillDate, displayby);
            return PartialView(PartialViews.JournalEntrySupportReport, journalEntrySupportReportDetail);
        }

        /// <summary>
        ///     Sorts the denial code grid.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="displayby">The displayby.</param>
        /// <returns></returns>
        public ActionResult SortDenialCodeGrid(DateTime fromDate, DateTime tillDate, int displayby)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetDefaultFacilityId();
            List<DenialReportCustomModel> denialreportDetail = _service.GetDenialCodesReport(
                corporateId,
                facilityid,
                fromDate,
                tillDate,
                displayby);
            return PartialView(PartialViews.DenialReport, denialreportDetail);
        }

        /// <summary>
        ///     Sorts the ip charges report grid.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="departmentNumber">The department number.</param>
        /// <param name="payorId">The payor identifier.</param>
        /// <returns></returns>
        public ActionResult SortIPChargesReportGrid(
            DateTime? fromDate,
            DateTime? tillDate,
            string departmentNumber,
            int payorId)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            decimal departmentnumber = !string.IsNullOrEmpty(departmentNumber)
                                           ? Convert.ToDecimal(departmentNumber)
                                           : Convert.ToDecimal(0.00);
            List<ChargesReportCustomModel> chargeDetailReportIP = _service.GetChargesDetailReport(
                corporateId,
                facilityId,
                fromDate,
                tillDate,
                departmentnumber,
                1,
                payorId);
            return PartialView(PartialViews.IPChargesDetailReport, chargeDetailReportIP);
        }

        /// <summary>
        ///     Sorts the op charges report grid.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="departmentNumber">The department number.</param>
        /// <param name="payorId">The payor identifier.</param>
        /// <returns></returns>
        public ActionResult SortOPChargesReportGrid(
            DateTime? fromDate,
            DateTime? tillDate,
            string departmentNumber,
            int payorId)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            decimal departmentnumber = !string.IsNullOrEmpty(departmentNumber)
                                           ? Convert.ToDecimal(departmentNumber)
                                           : Convert.ToDecimal(0.00);

            List<ChargesReportCustomModel> chargeDetailReportOP = _service.GetChargesDetailReport(
                corporateId,
                facilityId,
                fromDate,
                tillDate,
                departmentnumber,
                2,
                payorId);
            return PartialView(PartialViews.OPChargesDetailReport, chargeDetailReportOP);
        }

        /// <summary>
        ///     Sorts the ep charges report grid.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="departmentNumber">The department number.</param>
        /// <param name="payorId">The payor identifier.</param>
        /// <returns></returns>
        public ActionResult SortEPChargesReportGrid(
            DateTime? fromDate,
            DateTime? tillDate,
            string departmentNumber,
            int payorId)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();
            decimal departmentnumber = !string.IsNullOrEmpty(departmentNumber)
                                           ? Convert.ToDecimal(departmentNumber)
                                           : Convert.ToDecimal(0.00);

            List<ChargesReportCustomModel> chargeDetailReportER = _service.GetChargesDetailReport(
                corporateId,
                facilityId,
                fromDate,
                tillDate,
                departmentnumber,
                3,
                payorId);
            return PartialView(PartialViews.ERChargesDetailReport, chargeDetailReportER);
        }

        #endregion Sorting By Krishna on 21082015
    }
}