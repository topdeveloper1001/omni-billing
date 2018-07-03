
namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;
    using BillingSystem.Common.Common;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// The scrub report controller.
    /// </summary>
    public class ScrubReportController : BaseController
    {
        private readonly IScrubReportService _service;
        private readonly IUsersService _uService;
        private readonly IRuleStepService _rsService;
        private readonly IGlobalCodeService _gService;

        public ScrubReportController(IScrubReportService service, IUsersService uService, IRuleStepService rsService, IGlobalCodeService gService)
        {
            _service = service;
            _uService = uService;
            _rsService = rsService;
            _gService = gService;
        }

        /// <summary>
        /// Get the details of the ScrubReport View in the Model ScrubReport such as ScrubReportList, list of countries etc.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="generateScrub">generateScrub</param>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model ScrubReport to be passed to View ScrubReport
        /// </returns>
        public ActionResult Index(int? billHeaderId, bool? generateScrub)
        {
            if (billHeaderId == null)
            {
                billHeaderId = 0;
            }


            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();
            var generateScrubStatus = generateScrub != null && (bool)generateScrub;

            // Get the Entity list
            var headerList = _service.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);

            // Intialize the View Model i.e. ScrubReportView which is binded to Main View Index.cshtml under ScrubReport
            var scrubReportView = new ScrubReportView
            {
                ScrubHeaderList = headerList,
                ScrubReportList = new List<ScrubReportCustomModel>(),
                BillHeaderId = Convert.ToInt32(billHeaderId),
            };

            // Pass the View Model in ActionResult to View ScrubReport
            return View(scrubReportView);
        }

        /// <summary>
        /// Gets the scrub report.
        /// </summary>
        /// <param name="scrubHeaderId">The scrub header identifier.</param>
        /// <param name="reportType">Type of the report.</param>
        /// <returns></returns>
        public ActionResult GetScrubReport(int scrubHeaderId, int reportType)
        {
            var list = _service.GetScrubReport(scrubHeaderId, reportType);
            ViewBag.BillHeaderID = scrubHeaderId;
            return PartialView(PartialViews.ScrubReportList, list);
        }

        /// <summary>
        /// Generates the srubb report.
        /// </summary>
        /// <param name="billheaderid">The billheaderid.</param>
        /// <param name="isAllShown"></param>
        /// <returns></returns>
        public ActionResult GenerateScrubReport(int billheaderid, bool isAllShown)
        {

            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();

            if (isAllShown)
                billheaderid = 0;

            //Get the Entity list
            var headerList = _service.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billheaderid), userId, true);
            //return RedirectToAction("Index", new { billHeaderId = billheaderid });
            return PartialView(PartialViews.ScrubHeaderList, headerList);
        }

        /// <summary>
        /// Gets the scrub report detail.
        /// </summary>
        /// <param name="scrubReportId">The scrub report identifier.</param>
        /// <returns></returns>
        public ActionResult GetScrubReportDetail(int scrubReportId)
        {
            var result = _service.GetScrubReportDetailById(scrubReportId, Helpers.DefaultBillEditRuleTableNumber);
            return PartialView(PartialViews.BillCorrectionView, result);
        }

        /// <summary>
        /// Bills the edit corrections.
        /// </summary>
        /// <param name="scrubReportId">The scrub report identifier.</param>
        /// <param name="lhsValue">The LHS value.</param>
        /// <param name="rhsValue">The RHS value.</param>
        /// <param name="correctionCodeId">The correction code identifier(GlobalCategoryValue : 0101).</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult BillEditCorrections(int scrubReportId, string lhsValue, string rhsValue, string correctionCodeId, int patientId, int encounterId)
        {
            var loggedinUser = Helpers.GetLoggedInUserId();
            var result = _service.GetScrubReportDetailById(scrubReportId, Helpers.DefaultBillEditRuleTableNumber);
            var scrubHeaderId = Convert.ToInt32(result.ScrubHeaderID);
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();

            // ..... Toupdate the diagnosis for correction in bill edits
            var returnStatus = 100;
            var ruleStepId = Convert.ToInt32(result.RuleStepID);

            // var scrubHeaderbal = new ScrubHeader()
            var ruleStepObj = _rsService.GetRuleStepByID(ruleStepId);
            var scrubHeaderobj = _service.GetScrubHeaderById(scrubHeaderId);
            if (ruleStepObj != null)
            {
                var ruleStepLhst = ruleStepObj.LHST;
                if (ruleStepLhst.Contains("Diagnosis"))
                {
                    returnStatus = _service.SetCorrectedDiagnosis(
                        corporateid,
                        facilityid,
                        Convert.ToInt32(scrubHeaderobj.PatientID),
                        Convert.ToInt32(scrubHeaderobj.EncounterID),
                        loggedinUser,
                        lhsValue);
                }
            }

            // Update Query
            if (returnStatus == 100)
            {
                var updatedStatus = _service.UpdateScrubReportDetailWithCorrection(scrubReportId, scrubHeaderId, lhsValue, rhsValue, Helpers.GetLoggedInUserId(), corporateid, facilityid, correctionCodeId);
                returnStatus = updatedStatus ? 101 : returnStatus;
            }
            var scrubreportObj = _service.GetScrubReportById(scrubReportId);
            scrubreportObj.ExtValue4 = correctionCodeId;
            _service.AddUpdateScrubReport(scrubreportObj);

            // var list = _service.GetScrubReport(scrubHeaderId, 999);
            ViewBag.BillHeaderID = scrubHeaderId;

            // return PartialView(PartialViews.ScrubReportList, list);
            return Json(returnStatus.ToString());
        }


        /// <summary>
        /// Assigns the user to scrub header.
        /// </summary>
        /// <param name="assignedTo">The assigned to.</param>
        /// <param name="scrubHeaderId">The scrub header identifier.</param>
        /// <returns></returns>
        public ActionResult AssignUserToScrubHeader(int assignedTo, int scrubHeaderId)
        {
            var loggedinUserId = Helpers.GetLoggedInUserId();
            var res = _service.AssignUserToScrubHeaderForBillEdit(assignedTo, scrubHeaderId, loggedinUserId, Helpers.GetInvariantCultureDateTime());
            var jsonResult = new { AssignedToUser = res, AssignedByUser = _uService.GetNameByUserId(loggedinUserId), AssignedBy = loggedinUserId };
            return Json(jsonResult);
        }

        /// <summary>
        /// Rescrubs the after bill edit.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="allShown">if set to <c>true</c> [all shown].</param>
        /// <returns></returns>
        public ActionResult RescrubAfterBillEdit(int billHeaderId, bool allShown)
        {
            var userId = Helpers.GetLoggedInUserId();

            //Apply Scrub
            _service.ApplyScrubBillToSpecificBillHeaderId(Convert.ToInt32(billHeaderId), userId);

            if (allShown)
                billHeaderId = 0;

            return RedirectToAction("Index", new { billHeaderId, generateScrub = false });
        }

        #region Work Queues

        public ActionResult BillEditWorkQueuesView(int? billHeaderId, bool? generateScrub)
        {
            if (billHeaderId == null)
                billHeaderId = 0;


            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();
            var generateScrubStatus = generateScrub != null && (bool)generateScrub;

            //Get the Entity list
            var headerList = _service.GetScrubHeaderListWorkQueues(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);



            //Intialize the View Model i.e. ScrubReportView which is binded to Main View Index.cshtml under ScrubReport
            var scrubReportView = new ScrubReportView
            {
                ScrubHeaderList = headerList,
                ScrubReportList = new List<ScrubReportCustomModel>(),
                BillHeaderId = Convert.ToInt32(billHeaderId),
                BillsCount = headerList.Count()
            };

            //Pass the View Model in ActionResult to View ScrubReport
            return View(scrubReportView);
        }

        #endregion

        /// <summary>
        /// Gets the correction code list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCorrectionCodeList()
        {
            var globalcodeList = _gService.GetGCodesListByCategoryValue("0101").ToList();
            var filteredList = globalcodeList.Select(item => new
            {
                Value = item.GlobalCodeValue,
                Text = string.Format("{0}-{1}", item.GlobalCodeName, item.Description),
            }).ToList();
            return Json(filteredList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the scurb reportby scrub report identifier.
        /// </summary>
        /// <param name="scrubreportId">The scrubreport identifier.</param>
        /// <returns></returns>
        public ActionResult GetScurbReportbyScrubReportId(int scrubreportId)
        {
            var result = _service.GetScrubReportDetailById(scrubreportId, Helpers.DefaultBillEditRuleTableNumber);
            var scrubHeaderId = Convert.ToInt32(result.ScrubHeaderID);
            var list = _service.GetScrubReport(scrubHeaderId, 999);
            return PartialView(PartialViews.ScrubReportList, list);
        }

        #region Sorting Events


        /// <summary>
        /// Sorts the clean claims.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="generateScrub">The generate scrub.</param>
        /// <returns></returns>
        public ActionResult SortCleanClaims(int? billHeaderId, bool? generateScrub)
        {
            if (billHeaderId == null)
                billHeaderId = 0;
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();
            var generateScrubStatus = generateScrub != null && (bool)generateScrub;

            var headerList = _service.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);
            headerList =
                headerList.Where(
                    a =>
                    a.Status != null
                    && (a.Section == Convert.ToInt32(ReviewSummarySections.Approved)
                        && ((int)a.Status == 0 || (int)a.Status == 99))).ToList();
            return PartialView(PartialViews.ScrubHeaderListF1, headerList);

        }

        /// <summary>
        /// Sorts the claims corrected.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="generateScrub">The generate scrub.</param>
        /// <returns></returns>
        public ActionResult SortClaimsCorrected(int? billHeaderId, bool? generateScrub)
        {
            if (billHeaderId == null)
                billHeaderId = 0;
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();
            var generateScrubStatus = generateScrub != null && (bool)generateScrub;

            var headerList = _service.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);
            headerList =
                headerList.Where(a => a.Status != null && (a.Section == Convert.ToInt32(ReviewSummarySections.BillEditErrors) && ((int)a.Status == 0 || (int)a.Status == 99))).ToList();
            return PartialView(PartialViews.ScrubHeaderList, headerList);

        }

        /// <summary>
        /// Sorts the claimswith potential edit.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="generateScrub">The generate scrub.</param>
        /// <returns></returns>
        public ActionResult SortClaimswithPotentialEdit(int? billHeaderId, bool? generateScrub)
        {
            if (billHeaderId == null)
                billHeaderId = 0;
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();
            var generateScrubStatus = generateScrub != null && (bool)generateScrub;

            var headerList = _service.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);
            headerList =
                headerList.Where(
                    a =>
                    a.Status != null && (a.Section != Convert.ToInt32(ReviewSummarySections.Denials))
                    && ((int)a.Status == 1 || (int)a.Status == 2)).ToList();
            return PartialView(PartialViews.ScrubHeaderListWithBillEditsWithError, headerList);

        }

        /// <summary>
        /// Sorts the claimsdeniedwith errors.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="generateScrub">The generate scrub.</param>
        /// <returns></returns>
        public ActionResult SortClaimsdeniedwithErrors(int? billHeaderId, bool? generateScrub)
        {
            if (billHeaderId == null)
                billHeaderId = 0;
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();
            var generateScrubStatus = generateScrub != null && (bool)generateScrub;

            var headerList = _service.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);
            headerList =
                headerList.Where(a => a.Status != null && (a.Section == Convert.ToInt32(ReviewSummarySections.Denials) && ((int)a.Status == 1 || (int)a.Status == 2 || (int)a.Status == 0))).ToList();
            return PartialView(PartialViews.ScrubHeaderListWithDenials, headerList);

        }

        #endregion
    }
}