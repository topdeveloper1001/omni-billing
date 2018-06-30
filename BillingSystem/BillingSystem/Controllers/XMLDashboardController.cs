// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XMLDashboardController.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The xml dashboard controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;

    /// <summary>
    /// The xml dashboard controller.
    /// </summary>
    public class XMLDashboardController : BaseController
    {
        private readonly IDashboardBudgetService _dbService;

        public XMLDashboardController(IDashboardBudgetService dbService)
        {
            _dbService = dbService;
        }

        // GET: /XMLDashboard/
        #region Public Methods and Operators

        /// <summary>
        /// The bill scrubber.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult BillScrubber()
        {
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var section5RemarksList = new List<DashboardRemarkCustomModel>();
            var section6RemarksList = new List<DashboardRemarkCustomModel>();
            var section7RemarksList = new List<DashboardRemarkCustomModel>();
            var section8RemarksList = new List<DashboardRemarkCustomModel>();
            var section9RemarksList = new List<DashboardRemarkCustomModel>();
            var section10RemarksList = new List<DashboardRemarkCustomModel>();

            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            var facilitybal = new FacilityService();
            var corporateFacilitydetail = facilitybal.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == null
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            using (var bal = new DashboardRemarkBal())
            {
                var corporateid = Helpers.GetSysAdminCorporateID();
                var allRemarksList = bal.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                    Convert.ToInt32(12));
                if (allRemarksList != null && allRemarksList.Count > 0)
                {
                    section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("1")).ToList();
                    section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("2")).ToList();
                    section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("3")).ToList();
                    section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("4")).ToList();
                    section5RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("5")).ToList();
                    section6RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("6")).ToList();
                    section7RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("7")).ToList();
                    section8RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("8")).ToList();
                    section9RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("9")).ToList();
                    section10RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("10")).ToList();
                }
                var dashboardview = new XMLBillScrubberDashboardView
                {
                    FacilityId = facilityid,
                    DashboardType = 12,
                    Title = Helpers.ExternalDashboardTitleView("11"),
                    Section1RemarksList = section1RemarksList,
                    Section2RemarksList = section2RemarksList,
                    Section3RemarksList = section3RemarksList,
                    Section4RemarksList = section4RemarksList,
                    Section5RemarksList = section5RemarksList,
                    Section6RemarksList = section6RemarksList,
                    Section7RemarksList = section7RemarksList,
                    Section8RemarksList = section8RemarksList,
                    Section9RemarksList = section9RemarksList,
                    Section10RemarksList = section10RemarksList,
                };
                return View(dashboardview);
            }
        }


        /// <summary>
        /// Bills the scrubber graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult BillScrubberGraphsData(int facilityId, int month, int facilityType, int segment, int department)
        {
            var cId = Helpers.GetSysAdminCorporateID();
            var customDate = month == 0
                ? CurrentDateTime.ToShortDateString()
                : Convert.ToDateTime(month + "/" + month + "/" + CurrentDateTime.Year).ToShortDateString();
            var curentYear = CurrentDateTime.Year;
            const string rcmDashboardYtd = "2000,2001,2002,2003,2004,2005,2012,2013";
            const string indicatorsData = "2006,2007,2008,2009,2010,2011";

            if (facilityId == 0)
                facilityId = 9999;

            var manualDashboardList = _dbService.GetManualDashBoardStatData(facilityId, cId, rcmDashboardYtd,
                customDate, facilityType, segment, department);

            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, cId, indicatorsData,
                curentYear, facilityType, segment, department);

            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();

            var TotalDollarBilledClaims = manualDashboardList.Where(x => x.IndicatorNumber.Equals("2000")).ToList();
            var TotalDollarDeniedClaims = manualDashboardList.Where(x => x.IndicatorNumber.Equals("2001")).ToList();
            var GrossDenialRate = manualDashboardList.Where(x => x.IndicatorNumber.Equals("2002")).ToList();
            var DollarAmountTechnicalEdits = manualDashboardList.Where(x => x.IndicatorNumber.Equals("2003")).ToList();
            var DenialsbyReasonforDenial = manualDashboardList.Where(x => x.IndicatorNumber.Equals("2004")).ToList();
            var DollarClaimsResubmittedbyDenialByMonth = manualDashboardList.Where(x => x.IndicatorNumber.Equals("2005")).ToList();
            var TotalDollarClaimsResubmitted = manualDashboardData.Where(x => x.Indicators == 2006).ToList();
            var TotalResubmissionDollarsCollected = manualDashboardData.Where(x => x.Indicators == 2007 && x.SubCategory1.Equals("27")).ToList();
            var TotalResubmissionInpatientDollarsCollected = manualDashboardData.Where(x => x.Indicators == 2007 && x.SubCategory1.Equals("142")).ToList();
            var TotalResubmissionOutpatientDollarsCollected = manualDashboardData.Where(x => x.Indicators == 2007 && x.SubCategory1.Equals("143")).ToList();
            var DollarPercentofDenialsResubmitted = manualDashboardData.Where(x => x.Indicators == 2008).ToList();
            var PercentofResubmissionsCollected = manualDashboardData.Where(x => x.Indicators == 2009).ToList();
            var TotalCashCollected = manualDashboardData.Where(x => x.Indicators == 2010).ToList();
            var AverageAmountCollectedPerResubmission = manualDashboardData.Where(x => x.Indicators == 2011).ToList();

            // ----YTD
            var RevenueCollectedByDenialCodeYearToDate = manualDashboardList.Where(x => x.IndicatorNumber.Equals("2012")).ToList();
            //Pie chart should be in 100% that's why we have implemented the below functionality
            var PercentRevenueCollectedByDenialCodeYTD2013 = manualDashboardList.Where(x => x.IndicatorNumber.Equals("2013")).ToList();
            var sum = PercentRevenueCollectedByDenialCodeYTD2013.Sum(x => Convert.ToDecimal(x.CYTA));

            if (sum > 1)
            {
                foreach (var item in PercentRevenueCollectedByDenialCodeYTD2013)
                    item.CYTA = item.CYTA > 0 ? (Convert.ToDecimal(item.CYTA) / sum) : item.CYTA;
            }

            var PercentRevenueCollectedByDenialCodeYTD = PercentRevenueCollectedByDenialCodeYTD2013;

            var jsonResult = new
            {
                TotalDollarBilledClaims,
                TotalDollarDeniedClaims,
                GrossDenialRate,
                DollarAmountTechnicalEdits,
                DenialsbyReasonforDenial,
                DollarClaimsResubmittedbyDenialByMonth,
                TotalDollarClaimsResubmitted,
                TotalResubmissionDollarsCollected,
                TotalResubmissionInpatientDollarsCollected,
                TotalResubmissionOutpatientDollarsCollected,
                DollarPercentofDenialsResubmitted,
                PercentofResubmissionsCollected,
                TotalCashCollected,
                AverageAmountCollectedPerResubmission,
                RevenueCollectedByDenialCodeYearToDate,
                PercentRevenueCollectedByDenialCodeYTD
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}