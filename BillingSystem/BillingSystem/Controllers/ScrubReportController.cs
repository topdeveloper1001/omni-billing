// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrubReportController.cs" company="Spadez">
//   OMNI
// </copyright>
// <summary>
//   The scrub report controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Common.Common;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// The scrub report controller.
    /// </summary>
    public class ScrubReportController : BaseController
    {
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

            // Initialize the ScrubReport BAL object
            var scrubReportBal = new ScrubReportBal();

            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();
            var generateScrubStatus = generateScrub != null && (bool)generateScrub;

            // Get the Entity list
            var headerList = scrubReportBal.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);

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
            using (var bal = new ScrubReportBal())
            {
                var list = bal.GetScrubReport(scrubHeaderId, reportType);
                ViewBag.BillHeaderID = scrubHeaderId;
                return PartialView(PartialViews.ScrubReportList, list);
            }
        }

        /// <summary>
        /// Generates the srubb report.
        /// </summary>
        /// <param name="billheaderid">The billheaderid.</param>
        /// <param name="isAllShown"></param>
        /// <returns></returns>
        public ActionResult GenerateScrubReport(int billheaderid, bool isAllShown)
        {
            var scrubReportBal = new ScrubReportBal();

            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();

            if (isAllShown)
                billheaderid = 0;

            //Get the Entity list
            var headerList = scrubReportBal.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billheaderid), userId, true);
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
            using (var bal = new ScrubReportBal(Helpers.DefaultBillEditRuleTableNumber))
            {
                var result = bal.GetScrubReportDetailById(scrubReportId);
                return PartialView(PartialViews.BillCorrectionView, result);
            }
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
            using (var bal = new ScrubReportBal(Helpers.DefaultBillEditRuleTableNumber))
            {
                var loggedinUser = Helpers.GetLoggedInUserId();
                var result = bal.GetScrubReportDetailById(scrubReportId);
                var scrubHeaderId = Convert.ToInt32(result.ScrubHeaderID);
                var corporateid = Helpers.GetSysAdminCorporateID();
                var facilityid = Helpers.GetDefaultFacilityId();

                // ..... Toupdate the diagnosis for correction in bill edits
                var returnStatus = 100;
                var ruleStepId = Convert.ToInt32(result.RuleStepID);
                var ruleStepBal = new RuleStepBal();

                // var scrubHeaderbal = new ScrubHeader()
                var ruleStepObj = ruleStepBal.GetRuleStepByID(ruleStepId);
                var scrubHeaderobj = bal.GetScrubHeaderById(scrubHeaderId);
                if (ruleStepObj != null)
                {
                    var ruleStepLhst = ruleStepObj.LHST;
                    if (ruleStepLhst.Contains("Diagnosis"))
                    {
                        returnStatus = bal.SetCorrectedDiagnosis(
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
                    var updatedStatus = bal.UpdateScrubReportDetailWithCorrection(scrubReportId, scrubHeaderId, lhsValue, rhsValue, Helpers.GetLoggedInUserId(), corporateid, facilityid, correctionCodeId);
                    returnStatus = updatedStatus ? 101 : returnStatus;
                }
                var scrubreportObj = bal.GetScrubReportById(scrubReportId);
                scrubreportObj.ExtValue4 = correctionCodeId;
                bal.AddUpdateScrubReport(scrubreportObj);

                // var list = bal.GetScrubReport(scrubHeaderId, 999);
                ViewBag.BillHeaderID = scrubHeaderId;

                // return PartialView(PartialViews.ScrubReportList, list);
                return Json(returnStatus.ToString());
            }
        }

        /// <summary>
        /// Assigns the user to scrub header.
        /// </summary>
        /// <param name="assignedTo">The assigned to.</param>
        /// <param name="scrubHeaderId">The scrub header identifier.</param>
        /// <returns></returns>
        public ActionResult AssignUserToScrubHeader(int assignedTo, int scrubHeaderId)
        {
            using (var bal = new ScrubReportBal())
            {
                var loggedinUserId = Helpers.GetLoggedInUserId();
                var res = bal.AssignUserToScrubHeaderForBillEdit(assignedTo, scrubHeaderId, loggedinUserId, Helpers.GetInvariantCultureDateTime());
                var jsonResult = new { AssignedToUser = res, AssignedByUser = bal.GetNameByUserId(loggedinUserId), AssignedBy = loggedinUserId };
                return Json(jsonResult);
            }
        }

        /// <summary>
        /// Rescrubs the after bill edit.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="allShown">if set to <c>true</c> [all shown].</param>
        /// <returns></returns>
        public ActionResult RescrubAfterBillEdit(int billHeaderId, bool allShown)
        {
            using (var bal = new ScrubReportBal())
            {
                var userId = Helpers.GetLoggedInUserId();

                //Apply Scrub
                bal.ApplyScrubBillToSpecificBillHeaderId(Convert.ToInt32(billHeaderId), userId);

                if (allShown)
                    billHeaderId = 0;

                return RedirectToAction("Index", new { billHeaderId, generateScrub = false });
            }
        }

        #region Work Queues

        public ActionResult BillEditWorkQueuesView(int? billHeaderId, bool? generateScrub)
        {
            if (billHeaderId == null)
                billHeaderId = 0;

            //Initialize the ScrubReport BAL object
            var scrubReportBal = new ScrubReportBal();

            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();
            var generateScrubStatus = generateScrub != null && (bool)generateScrub;

            //Get the Entity list
            var headerList = scrubReportBal.GetScrubHeaderListWorkQueues(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);
           

            
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
            using (var globalCodeBal = new GlobalCodeBal())
            {
                var globalcodeList = globalCodeBal.GetGCodesListByCategoryValue("0101").ToList();
                var filteredList = globalcodeList.Select(item => new
                {
                    Value = item.GlobalCodeValue,
                    Text = string.Format("{0}-{1}", item.GlobalCodeName, item.Description),
                }).ToList();
                return Json(filteredList, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the scurb reportby scrub report identifier.
        /// </summary>
        /// <param name="scrubreportId">The scrubreport identifier.</param>
        /// <returns></returns>
        public ActionResult GetScurbReportbyScrubReportId(int scrubreportId)
        {
            using (var bal = new ScrubReportBal(Helpers.DefaultBillEditRuleTableNumber))
            {
                var result = bal.GetScrubReportDetailById(scrubreportId);
                var scrubHeaderId = Convert.ToInt32(result.ScrubHeaderID);
                var list = bal.GetScrubReport(scrubHeaderId, 999);
                return PartialView(PartialViews.ScrubReportList, list);
            }
        }

        #region Sorting Events

        //public ActionResult SortClaimsNeedingApprovalGrid(int? billHeaderId, bool? generateScrub)
        //{

        //    if (billHeaderId == null)
        //        billHeaderId = 0;
        //    var corporateId = Helpers.GetSysAdminCorporateID();
        //    var facilityId = Helpers.GetDefaultFacilityId();
        //    var userId = Helpers.GetLoggedInUserId();
        //    var generateScrubStatus = generateScrub != null && (bool)generateScrub;
        //    using (var scrubReportBal = new ScrubReportBal())
        //    {
        //        var headerList = scrubReportBal.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);
        //        return PartialView(PartialViews.ScrubHeaderList, headerList);
        //    }
        //}


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
            using (var scrubReportBal = new ScrubReportBal())
            {
                var headerList = scrubReportBal.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);
                headerList =
                    headerList.Where(
                        a =>
                        a.Status != null
                        && (a.Section == Convert.ToInt32(ReviewSummarySections.Approved)
                            && ((int)a.Status == 0 || (int)a.Status == 99))).ToList();
                return PartialView(PartialViews.ScrubHeaderListF1, headerList);
            }
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
            using (var scrubReportBal = new ScrubReportBal())
            {
                var headerList = scrubReportBal.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);
                headerList =
                    headerList.Where(a => a.Status != null && (a.Section == Convert.ToInt32(ReviewSummarySections.BillEditErrors) && ((int)a.Status == 0 || (int)a.Status == 99))).ToList();
                return PartialView(PartialViews.ScrubHeaderList, headerList);
            }
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
            using (var scrubReportBal = new ScrubReportBal())
            {
                var headerList = scrubReportBal.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);
                headerList =
                    headerList.Where(
                        a =>
                        a.Status != null && (a.Section != Convert.ToInt32(ReviewSummarySections.Denials))
                        && ((int)a.Status == 1 || (int)a.Status == 2)).ToList();
                return PartialView(PartialViews.ScrubHeaderListWithBillEditsWithError, headerList);
            }
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
            using (var scrubReportBal = new ScrubReportBal())
            {
                var headerList = scrubReportBal.GetScrubHeaderList(corporateId, facilityId, Convert.ToInt32(billHeaderId), userId, generateScrubStatus);
                headerList =
                    headerList.Where(a => a.Status != null && (a.Section == Convert.ToInt32(ReviewSummarySections.Denials) && ((int)a.Status == 1 || (int)a.Status == 2 || (int)a.Status == 0))).ToList();
                return PartialView(PartialViews.ScrubHeaderListWithDenials, headerList);
            }
        }
        
        #endregion
    }
}