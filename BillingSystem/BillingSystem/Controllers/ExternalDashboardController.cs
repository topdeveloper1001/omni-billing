using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class ExternalDashboardController : BaseController
    {
        private readonly IFacilityStructureService _fsService;
        private readonly IUsersService _uService;
        private readonly IDashboardBudgetService _dbService;
        private readonly IFacilityService _fService;
        private readonly IIndicatorDataCheckListService _iService;
        private readonly IDashboardRemarkService _drService;
        private readonly IEncounterService _eService;
        private readonly IGlobalCodeService _gService;
        private readonly IProjectTasksService _ptService;

        public ExternalDashboardController(IFacilityStructureService fsService, IUsersService uService, IDashboardBudgetService dbService, IFacilityService fService, IIndicatorDataCheckListService iService, IDashboardRemarkService drService, IEncounterService eService, IGlobalCodeService gService, IProjectTasksService ptService)
        {
            _fsService = fsService;
            _uService = uService;
            _dbService = dbService;
            _fService = fService;
            _iService = iService;
            _drService = drService;
            _eService = eService;
            _gService = gService;
            _ptService = ptService;
        }

        private static List<ExternalDashboardModel> ExternalDashboardLocalList { get; set; }

        #region Common Dashboard Class

        public class DropdownDataClass
        {
            public List<DropdownListData> dList { get; set; }
            public List<DropdownListData> ftList { get; set; }
            public List<DropdownListData> fList { get; set; }
            public List<DropdownListData> rList { get; set; }
            public List<DropdownListData> mList { get; set; }
            public int defaultYear { get; set; }
            public string defaultMonth { get; set; }
            public string facilityid { get; set; }
        }


        public DropdownDataClass GetDashboardDropdownData(int facilityId)
        {
            var categories = new List<string> { "4242", "4141", "903" };
            var corporateid = Helpers.GetSysAdminCorporateID();
            var dList = new List<DropdownListData>();
            var ftList = new List<DropdownListData>();
            var fList = new List<DropdownListData>();
            var rList = new List<DropdownListData>();
            var mList = new List<DropdownListData>();

            //Get Departments data
            dList = _fsService.GetRevenueDepartments(corporateid,
               facilityId == 0 ? Convert.ToString(Helpers.GetDefaultFacilityId()) : Convert.ToString(facilityId));
            var currentdt = _fService.GetInvariantCultureDateTime(facilityId);
            var defaultYear = currentdt.Year;
            var defaultMonth = currentdt.Month;

            //Get Facility data
            fList = _fService.GetFacilitiesForDashboards(facilityId, corporateid, Helpers.GetLoggedInUserIsAdmin());

            var list = _gService.GetListByCategoriesRange(categories);
            ftList = list.Where(f => f.ExternalValue1.Equals("4242")).ToList();
            rList = list.Where(f => f.ExternalValue1.Equals("4141")).ToList();
            mList = list.Where(f => f.ExternalValue1.Equals("903")).OrderBy(f => int.Parse(f.Value)).ToList();

            var defaults = _iService.GetDefaultMonthAndYearByFacilityId(facilityId == 0 ? Helpers.GetDefaultFacilityId() : facilityId
                , corporateid);
            if (defaults.Count == 2)
            {
                defaultYear = defaults[0] > 0 ? defaults[0] : defaultYear;
                defaultMonth = defaults[1] > 0 ? defaults[1] : defaultMonth;
            }

            var data = new DropdownDataClass
            {
                dList = dList,
                fList = fList,
                ftList = ftList,
                rList = rList,
                mList = mList,
                defaultYear = defaultYear,
                defaultMonth = Convert.ToString(defaultMonth),
                facilityid = Convert.ToString(facilityId)
            };

            return data;
        }

        #endregion

        #region Executive Summary Dashboards
        /// <summary>
        /// Executives the dashboard view.
        /// </summary>
        /// <returns></returns>
        public ActionResult ExecutiveDashboardView()
        {
            var dashboardview = new ExecutiveDashboardView
            {
                FacilityId = 0,
                DashboardType = 1,
                Title = Helpers.ExternalDashboardTitleView("1")
            };
            return View(dashboardview);
        }



        /// <summary>
        /// Executives the dashboard with month.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        public ActionResult ExecutiveDashboardFilters(int? facilityID, int month, int facilityType, int segment,
            int Department, string type)
        {
            #region Method Main Code

            if (month == 0)
                month = 12;
            #region Local variables Declaration
            List<ExternalDashboardModel> section1List;
            List<ExternalDashboardModel> section5List;
            List<ExternalDashboardModel> section10List;
            var balanceSheetSectionList = new List<ExternalDashboardModel>();
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
            #endregion


            var facilityId = facilityID ?? 9999;
            facilityId = facilityId == 0 ? 9999 : facilityId;

            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;
            var customDate = Convert.ToDateTime(month + "/" + month + "/" + currentYear).ToShortDateString();

            var corporateid = Helpers.GetSysAdminCorporateID();
            var indicatorNumbers = "101,102,103,104,105,106,144,108,109,242,110,111,113,114,155,115,116,117,118,119,120,121,122,124,125,126,127,128,129,130,131,162,145,260,261,280,281,282,283,284,285,286,287,288,609,610";
            var manualDashboardList = _dbService.GetManualDashBoardStatData(facilityId,
                    corporateid, indicatorNumbers, customDate, facilityType, segment, Department);
            ExternalDashboardLocalList = manualDashboardList;
            var balanceSheetData = _dbService.GetExecutiveDashboardBalanceSheet(facilityId,
               corporateid, "", customDate, 0, 0, 0);

            #region Statistics Section Code/ listing for Grid
            var section1Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection1Stat)).OfType<object>().ToList();
            var section5Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection5Stat)).OfType<object>().ToList();
            var section10Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection10Stat)).OfType<object>().ToList();
            var balanceSheetEnumlist = Enum.GetValues(typeof(ExecutiveDashboardBalanceSheetStat)).OfType<object>().ToList();

            var section1ListOrders = ExecutiveDashboard1ListSortOrder();//-------------Get the Sort Ordering list for Section 1
            var sectionVolume5ListOrders = ExecutiveDashboardListSortOrder();//-------------Get the Sort Ordering list for Section 4-5
            var sectionVolume10ListOrders = ExecutiveDashboardSection10ListSortOrder();//-------------Get the Sort Ordering list for Section 10
            var sectionBalanceSheetList = ExecutiveDashboardBalanceSheetListSortOrder();//-------------Get the Sort Ordering list for Section 12

            var section1 = section1Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = ((int)enumValue).ToString() });
            var section5 = section5Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = ((int)enumValue).ToString() });
            var section10 = section10Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = ((int)enumValue).ToString() });
            var balanceSheetSection = balanceSheetEnumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = Convert.ToString((int)enumValue) });
            section1List = manualDashboardList != null
                ? manualDashboardList.Where(x => section1.Any(r => r.Value == x.IndicatorNumber.Trim()))
                    .ToList()
                    .OrderBy(x => section1ListOrders[x.IndicatorNumber.Trim()])
                    .ToList()
                : new List<ExternalDashboardModel>();
            var adccontroller = section1List.Where(x => x.IndicatorNumber == "105").ToList();
            section1List =
                section1List.Where(x => (x.IndicatorNumber != "105")).ToList();
            section1List.Add(adccontroller.FirstOrDefault(x => x.IndicatorNumber == "105"));
            section5List = manualDashboardList != null ? manualDashboardList.Where(x => section5.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList().OrderBy(x => sectionVolume5ListOrders[x.IndicatorNumber.Trim()]).ToList()
                : new List<ExternalDashboardModel>();
            section10List = manualDashboardList != null ? manualDashboardList.Where(x => section10.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList().OrderBy(x => sectionVolume10ListOrders[x.IndicatorNumber.Trim()]).ToList() : new List<ExternalDashboardModel>();
            balanceSheetSectionList = balanceSheetData != null
              ? balanceSheetData.Where(x => balanceSheetSection.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList()
              .OrderBy(x => sectionBalanceSheetList[x.IndicatorNumber.Trim()]).ToList() : new List<ExternalDashboardModel>();
            #endregion

            #region Remarks Section Listing
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, Convert.ToInt32(facilityID),
                    Convert.ToInt32(1));
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
            #endregion
            #region Dashboard View Initialization
            var dashboardview = new ExecutiveDashboardView
            {
                FacilityId = facilityId,
                DashboardType = 1,
                Title = Helpers.ExternalDashboardTitleView(type),
                Section1List = section1List,
                Section5List = section5List,
                Section10List = section10List,
                BalanceSheetList = balanceSheetSectionList,
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList,
                Section7RemarksList = section7RemarksList,
                Section8RemarksList = section8RemarksList,
                Section9RemarksList = section9RemarksList,
                Section10RemarksList = section10RemarksList
            };
            #endregion
            //return View(dashboardview);
            return PartialView(PartialViews.ExecutiveDashboardPView, dashboardview);
            #endregion
        }

        /// <summary>
        /// Executives the dashboard graph list.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult ExecutiveDashboardGraphList(int facilityId, int month, int facilityType, int segment, int department, string type)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            //var execDashboardData = "144,156,120,110,121,159,247,103,277,165,127,143,109,122";
            const string execDashboardData = "144,156,120,110,121,159,247,103,277,143,109,122,244,245,1316";
            var corporateId = Helpers.GetSysAdminCorporateID();
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateId,
                Convert.ToString(execDashboardData),
                currentYear, facilityType, segment, department);
            var adcChart =
                manualDashboardData.Where(x => x.Indicators == 144)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList(); //ADCChart
            var adcServiceCodeChart = new List<ManualDashboardCustomModel>();
            adcServiceCodeChart.AddRange(
                manualDashboardData.Where(
                    x => x.Indicators == 156 && x.Year == currentYear && x.BudgetType == 2 && x.SubCategory2 != "0")
                    .ToList().OrderBy(x => x.SubCategory2));
            var swbChart =
                manualDashboardData.Where(x => x.Indicators == 120)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var netRevenueChart =
                manualDashboardData.Where(x => x.Indicators == 110)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var indirectNetRevenueChart =
                manualDashboardData.Where(x => x.Indicators == 121)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var payorMixChart =
                manualDashboardData.Where(x => x.Indicators == 159)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var referalPayorChart =
                manualDashboardData.Where(x => x.Indicators == 247)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var inpatientDays = manualDashboardData.Where(x => x.Indicators == 103)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var nursingHoursPPD = manualDashboardData.Where(x => x.Indicators == 277)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var cashCollectionChart1 = new List<ManualDashboardCustomModel>();

            var cashFlowIndicators = new List<int?> { 143, 244, 245, 1316 };

            var cashFlowList =
                manualDashboardData.Where(
                    x => x.Indicators != null && cashFlowIndicators.Contains(x.Indicators) && x.BudgetType == 2 && x.Year == currentYear).OrderBy(c => c.Indicators)
                    .ToList();
            cashCollectionChart1.AddRange(cashFlowList);

            var bedOccupancyChartYTD = new List<ExternalDashboardModel>();
            var ebitdaChartYTD = new List<ExternalDashboardModel>();
            if (ExternalDashboardLocalList != null && ExternalDashboardLocalList.Any())
            {
                bedOccupancyChartYTD =
                 ExternalDashboardLocalList.Where(x => x.IndicatorNumber == "109")
                    .ToList();
                ebitdaChartYTD =
                 ExternalDashboardLocalList.Where(x => x.IndicatorNumber == "122")
                     .ToList();
            }
            var jsonResult = new
            {
                adcChart,
                adcServiceCodeChart,
                //getOpEncounterChart,
                swbChart,
                netRevenueChart,
                indirectNetRevenueChart,
                payorMixChart,
                referalPayorChart,
                cashCollectionChart1,
                inpatientDays,
                nursingHoursPPD,
                bedOccupancyChartYTD,
                ebitdaChartYTD
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Executives the dashboard graph list.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult ExecutiveDashboardGraphListV1(int facilityId, int month, int facilityType, int segment, int department, string type)
        {
            const string execDashboardData = "144,156,120,110,121,159,247,103,277,143,109,122,244,245,1316";
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var manualDashboardData = _dbService.GetManualDashBoardV1(facilityId, corporateId,
                Convert.ToString(execDashboardData),
                currentYear, facilityType, segment, department);
            var adcChart =
                manualDashboardData.Where(x => x.Indicators == 144)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList(); //ADCChart
            var adcServiceCodeChart = new List<ManualDashboardCustomModel>();
            adcServiceCodeChart.AddRange(
                manualDashboardData.Where(
                    x => x.Indicators == 156 && x.Year == currentYear && x.BudgetType == 2 && x.SubCategory2 != "0")
                    .ToList().OrderBy(x => x.SubCategory2));
            var swbChart =
                manualDashboardData.Where(x => x.Indicators == 120)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var netRevenueChart =
                manualDashboardData.Where(x => x.Indicators == 110)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var indirectNetRevenueChart =
                manualDashboardData.Where(x => x.Indicators == 121)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var payorMixChart =
                manualDashboardData.Where(x => x.Indicators == 159)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var referalPayorChart =
                manualDashboardData.Where(x => x.Indicators == 247)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var inpatientDays = manualDashboardData.Where(x => x.Indicators == 103)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var nursingHoursPPD = manualDashboardData.Where(x => x.Indicators == 277)
                    .OrderBy(x => x.Year)
                    .ThenByDescending(x => x.BudgetType).ToList();
            var cashCollectionChart1 = new List<ManualDashboardCustomModel>();

            var cashFlowIndicators = new List<int?> { 143, 244, 245, 1316 };
            //cashCollectionChart1.AddRange(manualDashboardData.Where(x => x.Indicators == 143 && x.BudgetType == 2 && x.Year == curentYear));
            //cashCollectionChart1.AddRange(manualDashboardData.Where(x => x.Indicators == 165 && x.BudgetType == 2 && x.Year == curentYear));
            //cashCollectionChart1.AddRange(manualDashboardData.Where(x => x.Indicators == 127 && x.BudgetType == 2 && x.Year == curentYear));

            var cashFlowList =
                manualDashboardData.Where(
                    x => x.Indicators != null && cashFlowIndicators.Contains(x.Indicators) && x.BudgetType == 2 && x.Year == currentYear).OrderBy(c => c.Indicators)
                    .ToList();
            cashCollectionChart1.AddRange(cashFlowList);

            var bedOccupancyChartYTD = new List<ExternalDashboardModel>();
            var ebitdaChartYTD = new List<ExternalDashboardModel>();
            if (ExternalDashboardLocalList != null && ExternalDashboardLocalList.Any())
            {
                bedOccupancyChartYTD =
                 ExternalDashboardLocalList.Where(x => x.IndicatorNumber == "109")
                    .ToList();
                ebitdaChartYTD =
                 ExternalDashboardLocalList.Where(x => x.IndicatorNumber == "122")
                     .ToList();
            }
            var jsonResult = new
            {
                adcChart,
                adcServiceCodeChart,
                //getOpEncounterChart,
                swbChart,
                netRevenueChart,
                indirectNetRevenueChart,
                payorMixChart,
                referalPayorChart,
                cashCollectionChart1,
                inpatientDays,
                nursingHoursPPD,
                bedOccupancyChartYTD,
                ebitdaChartYTD
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Executives the dashboard v1.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        public ActionResult ExecutiveDashboardV1(int? month)
        {
            #region Local variables Declaration
            var section1List = new List<ExternalDashboardModel>();
            var section5List = new List<ExternalDashboardModel>();
            var section10List = new List<ExternalDashboardModel>();
            var balanceSheetSectionList = new List<ExternalDashboardModel>();

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
            #endregion

            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID != 0
                ? loggedinfacilityId
                 : 0;
            var corporateid = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityid);

            #region Main Code 
            #region Stats Section List objects
            var customdateVal = currentDateTime.ToShortDateString();
            if (month == null)
            {
                var monthVal = currentDateTime.Month;
                var yearVal = monthVal == 1 ? currentDateTime.AddYears(-1).Year : currentDateTime.Year;
                customdateVal = (Convert.ToDateTime("06/06/" + yearVal)).ToShortDateString();
            }
            var customDate = month == null
                ? customdateVal : (Convert.ToDateTime("06/06/" + currentDateTime.Year)).ToShortDateString();
            var manualDashboardList = _dbService.GetDashBoardDataStatList(facilityid,
                corporateid, "", customDate, 0, 0, 0);

            var balanceSheetData = _dbService.GetDashboardDataBalanceSheetList(facilityid,
                corporateid, "", customDate, 0, 0, 0);

            var section1Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection1Stat)).OfType<object>().ToList();
            var section5Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection5Stat)).OfType<object>().ToList();
            var section10Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection10Stat)).OfType<object>().ToList();
            var balanceSheetEnumlist = Enum.GetValues(typeof(ExecutiveDashboardBalanceSheetStat)).OfType<object>().ToList();
            var section1 = section1Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = Convert.ToString((int)enumValue) });
            var section5 = section5Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = Convert.ToString((int)enumValue) });
            var section10 = section10Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = Convert.ToString((int)enumValue) });
            var balanceSheetSection = balanceSheetEnumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = Convert.ToString((int)enumValue) });

            var sectionVolume5ListOrders = ExecutiveDashboardListSortOrder();
            var section10ListOrder = ExecutiveDashboardSection10ListSortOrder();
            section1List = manualDashboardList != null
                ? manualDashboardList.Where(x => section1.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList()
                : new List<ExternalDashboardModel>();
            var adccontroller = section1List.Where(x => x.IndicatorNumber == "105").ToList();
            section1List =
                section1List.Where(x => (x.IndicatorNumber != "105")).ToList();
            section1List.Add(adccontroller.FirstOrDefault(x => x.IndicatorNumber == "105"));
            section5List = manualDashboardList != null
                ? manualDashboardList.Where(x => section5.Any(r => r.Value == x.IndicatorNumber.Trim()))
                    .ToList().OrderBy(x => sectionVolume5ListOrders[x.IndicatorNumber.Trim()]).ToList()
                : new List<ExternalDashboardModel>();
            section10List = manualDashboardList != null
                ? manualDashboardList.Where(x => section10.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList()
                .OrderBy(x => section10ListOrder[x.IndicatorNumber.Trim()]).ToList()
                : new List<ExternalDashboardModel>();
            balanceSheetSectionList = balanceSheetData != null
               ? balanceSheetData.Where(x => balanceSheetSection.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList()
               : new List<ExternalDashboardModel>();
            #endregion

            #region Remarks Data
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                            Convert.ToInt32(1));
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
            #endregion

            #region Executive Dashboard Assignment/ initialization
            var dashboardview = new ExecutiveDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 1,
                Title = Helpers.ExternalDashboardTitleView("1"),
                Section1List = section1List,
                Section5List = section5List,
                Section10List = section10List,
                BalanceSheetList = balanceSheetSectionList,
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList,
                Section7RemarksList = section7RemarksList,
                Section8RemarksList = section8RemarksList,
                Section9RemarksList = section9RemarksList,
                Section10RemarksList = section10RemarksList
            };
            #endregion
            return View(dashboardview);

            #endregion
        }

        /// <summary>
        /// Executives the dashboard filters v1.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult ExecutiveDashboardFiltersV1(int? facilityID, int month, int facilityType, int segment,
            int Department, string type)
        {
            #region Method Main Code

            #region Local variables Declaration
            List<ExternalDashboardModel> section1List;
            List<ExternalDashboardModel> section5List;
            List<ExternalDashboardModel> section10List;
            var balanceSheetSectionList = new List<ExternalDashboardModel>();
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
            #endregion


            var facilityId = facilityID ?? 9999;
            facilityId = facilityId == 0 ? 9999 : facilityId;
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var customDate = Convert.ToDateTime(month + "/" + month + "/" + currentDateTime.Year).ToShortDateString();

            var corporateid = Helpers.GetSysAdminCorporateID();

            var indicatorNumbers = "101,102,103,104,105,106,144,108,109,242,110,111,113,114,155,115,116,117,118,119,120,121,122,124,125,126,127,128,129,130,131,162,145,260,261,280,281,282,283,284,285,286,287,288,609,610";
            var manualDashboardList = _dbService.GetDashBoardDataStatList(facilityId,
                    corporateid, indicatorNumbers, customDate, facilityType, segment, Department);
            ExternalDashboardLocalList = manualDashboardList;
            var balanceSheetData = _dbService.GetDashboardDataBalanceSheetList(facilityId,
                corporateid, "", customDate, 0, 0, 0);

            #region Statistics Section Code/ listing for Grid
            var section1Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection1Stat)).OfType<object>().ToList();
            var section5Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection5Stat)).OfType<object>().ToList();
            var section10Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection10Stat)).OfType<object>().ToList();
            var balanceSheetEnumlist = Enum.GetValues(typeof(ExecutiveDashboardBalanceSheetStat)).OfType<object>().ToList();

            var section1ListOrders = ExecutiveDashboard1ListSortOrder();//-------------Get the Sort Ordering list for Section 1
            var sectionVolume5ListOrders = ExecutiveDashboardListSortOrder();//-------------Get the Sort Ordering list for Section 4-5
            var sectionVolume10ListOrders = ExecutiveDashboardSection10ListSortOrder();//-------------Get the Sort Ordering list for Section 10
            var sectionBalanceSheetList = ExecutiveDashboardBalanceSheetListSortOrder();//-------------Get the Sort Ordering list for Section 12

            var section1 = section1Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = ((int)enumValue).ToString() });
            var section5 = section5Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = ((int)enumValue).ToString() });
            var section10 = section10Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = ((int)enumValue).ToString() });
            var balanceSheetSection = balanceSheetEnumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = Convert.ToString((int)enumValue) });
            section1List = manualDashboardList != null
                ? manualDashboardList.Where(x => section1.Any(r => r.Value == x.IndicatorNumber.Trim()))
                    .ToList()
                    .OrderBy(x => section1ListOrders[x.IndicatorNumber.Trim()])
                    .ToList()
                : new List<ExternalDashboardModel>();
            var adccontroller = section1List.Where(x => x.IndicatorNumber == "105").ToList();
            section1List =
                section1List.Where(x => (x.IndicatorNumber != "105")).ToList();
            section1List.Add(adccontroller.FirstOrDefault(x => x.IndicatorNumber == "105"));
            section5List = manualDashboardList != null ? manualDashboardList.Where(x => section5.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList().OrderBy(x => sectionVolume5ListOrders[x.IndicatorNumber.Trim()]).ToList()
                : new List<ExternalDashboardModel>();
            section10List = manualDashboardList != null ? manualDashboardList.Where(x => section10.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList().OrderBy(x => sectionVolume10ListOrders[x.IndicatorNumber.Trim()]).ToList() : new List<ExternalDashboardModel>();
            balanceSheetSectionList = balanceSheetData != null
              ? balanceSheetData.Where(x => balanceSheetSection.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList()
              .OrderBy(x => sectionBalanceSheetList[x.IndicatorNumber.Trim()]).ToList() : new List<ExternalDashboardModel>();
            #endregion

            #region Remarks Section Listing
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, Convert.ToInt32(facilityID),
                     Convert.ToInt32(1));
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
            #endregion

            #region Dashboard View Initialization
            var dashboardview = new ExecutiveDashboardView
            {
                FacilityId = facilityId,
                DashboardType = 1,
                Title = Helpers.ExternalDashboardTitleView(type),
                Section1List = section1List,
                Section5List = section5List,
                Section10List = section10List,
                BalanceSheetList = balanceSheetSectionList,
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList,
                Section7RemarksList = section7RemarksList,
                Section8RemarksList = section8RemarksList,
                Section9RemarksList = section9RemarksList,
                Section10RemarksList = section10RemarksList
            };
            #endregion
            //return View(dashboardview);
            return PartialView(PartialViews.ExecutiveDashboardPView, dashboardview);
            #endregion
        }

        /// <summary>
        /// Executives the dashboard.
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 3600, VaryByParam = "month", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult ExecutiveDashboard(int? month)
        {
            #region Local variables Declaration
            var section1List = new List<ExternalDashboardModel>();
            var section5List = new List<ExternalDashboardModel>();
            var section10List = new List<ExternalDashboardModel>();
            var balanceSheetSectionList = new List<ExternalDashboardModel>();

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
            #endregion

            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                ? loggedinfacilityId
                 : 0;
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityid);
            var corporateid = Helpers.GetSysAdminCorporateID();
            #region Main Code

            #region Stats Section List objects
            var customdateVal = currentDateTime.ToShortDateString();
            if (month == null)
            {
                var monthVal = currentDateTime.Month;
                var yearVal = monthVal == 1 ? currentDateTime.AddYears(-1).Year : currentDateTime.Year;
                customdateVal = (Convert.ToDateTime("06/06/" + yearVal)).ToShortDateString();
            }
            var customDate = month == null
                ? customdateVal : (Convert.ToDateTime("06/06/" + currentDateTime.Year)).ToShortDateString();
            var manualDashboardList = _dbService.GetManualDashBoardStatData(facilityid,
                corporateid, "", customDate, 0, 0, 0);

            var balanceSheetData = _dbService.GetExecutiveDashboardBalanceSheet(facilityid,
                corporateid, "", customDate, 0, 0, 0);

            var section1Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection1Stat)).OfType<object>().ToList();
            var section5Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection5Stat)).OfType<object>().ToList();
            var section10Enumlist = Enum.GetValues(typeof(ExecutiveDashboardSection10Stat)).OfType<object>().ToList();
            var balanceSheetEnumlist = Enum.GetValues(typeof(ExecutiveDashboardBalanceSheetStat)).OfType<object>().ToList();
            var section1 = section1Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = Convert.ToString((int)enumValue) });
            var section5 = section5Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = Convert.ToString((int)enumValue) });
            var section10 = section10Enumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = Convert.ToString((int)enumValue) });
            var balanceSheetSection = balanceSheetEnumlist.Select(enumValue => new SelectListItem { Text = enumValue.ToString(), Value = Convert.ToString((int)enumValue) });

            var sectionVolume5ListOrders = ExecutiveDashboardListSortOrder();
            var section10ListOrder = ExecutiveDashboardSection10ListSortOrder();
            section1List = manualDashboardList != null
                ? manualDashboardList.Where(x => section1.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList()
                : new List<ExternalDashboardModel>();
            var adccontroller = section1List.Where(x => x.IndicatorNumber == "105").ToList();
            section1List =
                section1List.Where(x => (x.IndicatorNumber != "105")).ToList();
            section1List.Add(adccontroller.FirstOrDefault(x => x.IndicatorNumber == "105"));
            section5List = manualDashboardList != null
                ? manualDashboardList.Where(x => section5.Any(r => r.Value == x.IndicatorNumber.Trim()))
                    .ToList().OrderBy(x => sectionVolume5ListOrders[x.IndicatorNumber.Trim()]).ToList()
                : new List<ExternalDashboardModel>();
            section10List = manualDashboardList != null
                ? manualDashboardList.Where(x => section10.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList()
                .OrderBy(x => section10ListOrder[x.IndicatorNumber.Trim()]).ToList()
                : new List<ExternalDashboardModel>();
            balanceSheetSectionList = balanceSheetData != null
               ? balanceSheetData.Where(x => balanceSheetSection.Any(r => r.Value == x.IndicatorNumber.Trim())).ToList()
               : new List<ExternalDashboardModel>();
            #endregion

            #region Remarks Data
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                            Convert.ToInt32(1));
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
            #endregion

            #region Executive Dashboard Assignment/ initialization
            var dashboardview = new ExecutiveDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 1,
                Title = Helpers.ExternalDashboardTitleView("1"),
                Section1List = section1List,
                Section5List = section5List,
                Section10List = section10List,
                BalanceSheetList = balanceSheetSectionList,
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList,
                Section7RemarksList = section7RemarksList,
                Section8RemarksList = section8RemarksList,
                Section9RemarksList = section9RemarksList,
                Section10RemarksList = section10RemarksList
            };
            #endregion
            return View(dashboardview);

            #endregion
        }


        #endregion


        #region Executive Key Performance Dashboard

        /// <summary>
        /// Executives the key performance.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        public ActionResult ExecutiveKeyPerformance()
        {
            var dashboardview = GetKpiData(0);
            return View(dashboardview);
        }

        private ExecutiveKeyPerformanceView GetKpiData(int facilityId)
        {
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var mainList = GetExecutiveKeyPerformanceList(facilityId);
            var corporateid = Helpers.GetSysAdminCorporateID();
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityId,
                Convert.ToInt32(10));

            var strategicType = Convert.ToString((int)DashboardProjectType.Strategic);
            var financialType = Convert.ToString((int)DashboardProjectType.Financial);
            var opType = Convert.ToString((int)DashboardProjectType.Operational);
            var indType = Convert.ToString((int)DashboardProjectType.Individual);

            var strategicExec = mainList.Where(g => g.ExternalValue2.Trim().Equals(strategicType)).ToList();
            var financialExec = mainList.Where(g => g.ExternalValue2.Trim().Equals(financialType)).ToList();
            var individualExec = mainList.Where(g => g.ExternalValue2.Trim().Equals(indType)).ToList();
            var opExec = mainList.Where(g => g.ExternalValue2.Trim().Equals(opType)).ToList();

            if (allRemarksList != null && allRemarksList.Count > 0)
            {
                section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals(strategicType)).ToList();
                section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals(opType)).ToList();
                section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals(financialType)).ToList();
                section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals(indType)).ToList();
            }
            var dbView = new ExecutiveKeyPerformanceView
            {
                FacilityId = facilityId,
                DashboardType = 10,
                StrategicKpiList = strategicExec,
                FinancialKpiList = financialExec,
                IndividualKpiList = individualExec,
                OperationalKpiList = opExec,
                Title = Helpers.ExternalDashboardTitleView("10"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
            };
            return dbView;
        }

        private List<ProjectsCustomModel> GetExecutiveKeyPerformanceList(int facilityId)
        {
            var list = _ptService.GetProjectsForExecKpiDashboard(Helpers.GetSysAdminCorporateID(), facilityId);
            if (list.Count > 0)
            {
                foreach (var pr in list)
                {
                    if (pr.Milestones != null && pr.Milestones.Any())
                    {
                        pr.Milestones.ForEach(cc =>
                          cc.ColorImage = Url.Content(cc.ColorImage));
                    }
                }
            }
            return list;
        }

        public JsonResult BindKPIDropdownData()
        {
            var data = GetDashboardDropdownData(0);
            var jsonResult = new
            {
                data.dList,
                data.fList,
                data.ftList,
                data.rList,
                data.mList,
                data.defaultYear,
                defaultMonth = Convert.ToString(data.defaultMonth),
                facilityid = Convert.ToString(data.facilityid)
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Patient Acquisition / Clinical Compliance Dashboard

        /// <summary>
        /// Clinical Compliance Dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult ClinicalCompliance(int? type)
        {
            if (type != null && Convert.ToInt32(type) > 0)
            {
                var section1RemarksList = new List<DashboardRemarkCustomModel>();
                var section2RemarksList = new List<DashboardRemarkCustomModel>();
                var loggedinfacilityId = Helpers.GetDefaultFacilityId();
                var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
                var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();
                var corporateid = Helpers.GetSysAdminCorporateID();
                var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid, Convert.ToInt32(type));
                if (allRemarksList != null && allRemarksList.Count > 0)
                {
                    section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("1")).ToList();
                    section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("2")).ToList();
                }

                var dashboardview = new ExecutiveDashboardView
                {
                    FacilityId = facilityid,
                    DashboardType = 4,
                    Title = Helpers.ExternalDashboardTitleView("10"),
                    Section1RemarksList = section1RemarksList,
                    Section2RemarksList = section2RemarksList,
                };
                return View(dashboardview);
            }
            return View("Index");
        }

        public JsonResult BindPatientAcquisitionData(int facilityId)
        {
            var data = GetDashboardDropdownData(facilityId);
            var graphsData = GetPatientAcquisitionGraphData(facilityId, 0, 0, 0, 0);
            var jsonResult = new
            {
                data.dList,
                data.fList,
                data.ftList,
                data.rList,
                data.mList,
                data.defaultYear,
                defaultMonth = Convert.ToString(data.defaultMonth),
                facilityid = Convert.ToString(facilityId),
                graphsData.conversionRate,
                graphsData.patientinFunnel,
                graphsData.timefromFunneltoBed,
                graphsData.lostfromFunnel
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public PatientAcquisitionData GetPatientAcquisitionGraphData(int facilityId, int month, int facilityType, int segment, int department)
        {
            const string ClinicalComplianceGrpahsArray = "720,721,722,723";

            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateId, Convert.ToString(ClinicalComplianceGrpahsArray), currentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();

            var conversionRate = manualDashboardData.Where(x => x.Indicators == 720).ToList();
            var patientinFunnel = manualDashboardData.Where(x => x.Indicators == 721).ToList();
            var timefromFunneltoBed = manualDashboardData.Where(x => x.Indicators == 722).ToList();
            var lostfromFunnel = manualDashboardData.Where(x => x.Indicators == 723).ToList();


            var result = new PatientAcquisitionData
            {
                conversionRate = conversionRate,
                patientinFunnel = patientinFunnel,
                timefromFunneltoBed = timefromFunneltoBed,
                lostfromFunnel = lostfromFunnel
            };
            return result;
        }

        public JsonResult RebindPatientAcquisitionData(int facilityId, int month, int facilityType, int segment, int department)
        {
            var graphsData = GetPatientAcquisitionGraphData(facilityId, month, 0, 0, 0);

            var jsonResult = new
            {
                graphsData.conversionRate,
                graphsData.patientinFunnel,
                graphsData.timefromFunneltoBed,
                graphsData.lostfromFunnel
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public class PatientAcquisitionData
        {
            public List<ManualDashboardCustomModel> conversionRate { get; set; }
            public List<ManualDashboardCustomModel> patientinFunnel { get; set; }
            public List<ManualDashboardCustomModel> timefromFunneltoBed { get; set; }
            public List<ManualDashboardCustomModel> lostfromFunnel { get; set; }
        }


        #endregion


        #region Clinical / Clinical Quality Dashboard
        /// <summary>
        /// Clinical Quality Dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult ClinicalQuality(int? type)
        {
            if (type != null && Convert.ToInt32(type) > 0)
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
                var loggedinfacilityId = Helpers.GetDefaultFacilityId();
                var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
                var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();
                var corporateid = Helpers.GetSysAdminCorporateID();
                var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid, Convert.ToInt32(type));
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
                }

                var dashboardview = new ExecutiveDashboardView
                {
                    FacilityId = facilityid,
                    DashboardType = 4,
                    Title = Helpers.ExternalDashboardTitleView("3"),
                    Section1RemarksList = section1RemarksList,
                    Section2RemarksList = section2RemarksList,
                    Section3RemarksList = section3RemarksList,
                    Section4RemarksList = section4RemarksList,
                    Section5RemarksList = section5RemarksList,
                    Section6RemarksList = section6RemarksList,
                    Section7RemarksList = section7RemarksList,
                    Section8RemarksList = section8RemarksList,
                    Section9RemarksList = section9RemarksList,
                };
                return View(dashboardview);
            }
            return View("Index");
        }

        public ClinicalData GetClinicalGraphsData(int facilityId, int month, int facilityType, int segment, int department)
        {
            const string clinicalGrpahsArray = "750,174,830,832,758,756,180,181,186,175,752,754,188,189,190,191,192,1312,1313,1314,1311,1309,1310";
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateId,
                Convert.ToString(clinicalGrpahsArray),
                currentYear, facilityType, segment, department);

            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();

            var totalsentinelevents = manualDashboardData.Where(x => x.Indicators == 750).ToList();
            var patienFallRate = manualDashboardData.Where(x => x.Indicators == 174).ToList();
            var totalNearmiss = manualDashboardData.Where(x => x.Indicators == 830).ToList();
            var totaladverseincidents = manualDashboardData.Where(x => x.Indicators == 832).ToList();

            var TotalMedicationErrors = manualDashboardData.Where(x => x.Indicators == 758).ToList();
            var totalIncidentsReports = manualDashboardData.Where(x => x.Indicators == 756).ToList();
            var mdroRate = manualDashboardData.Where(x => x.Indicators == 180).ToList();
            var mrsaRate = manualDashboardData.Where(x => x.Indicators == 181).ToList();

            var handHygieneCompliance = manualDashboardData.Where(x => x.Indicators == 186).ToList();

            var pressureUlcerIncidentRate = manualDashboardData.Where(x => x.Indicators == 175).ToList();
            var averageFIMScorePAR = manualDashboardData.Where(x => x.Indicators == 752).ToList();
            var averageFIMScoreLTC = manualDashboardData.Where(x => x.Indicators == 754).ToList();

            var inappropriateAntiBioticUsageRate = manualDashboardData.Where(x => x.Indicators == 188).ToList();
            var therapyInitialAssessmentProtocolCompliance = manualDashboardData.Where(x => x.Indicators == 189).ToList();

            var manualHandlingRiskAssessmentProtocolCompliance = manualDashboardData.Where(x => x.Indicators == 190).ToList();
            var standardizedOutcomeMeasureProtocol = manualDashboardData.Where(x => x.Indicators == 191).ToList();
            var Incidents = manualDashboardData.Where(x => x.Indicators == 192).ToList();
            var nonMedicationRelatedIncidents = manualDashboardData.Where(x => x.Indicators == 1312 && x.Year == currentYear).ToList();
            var typeOfIncidents = manualDashboardData.Where(x => x.Indicators == 1313 && x.Year == currentYear).ToList();
            //var categoryofIncidents = manualDashboardData.Where(x => x.Indicators == 1314).ToList();  //categoryofIncidents
            var medicationErrors = manualDashboardData.Where(x => x.Indicators == 1311 && x.Year == currentYear).ToList();  //categoryofIncidents

            var customDataToreturn = new List<ManualDashboardCustomModel>();
            var manualDashboardData1 = manualDashboardData.Where(x => x.Indicators == 1309 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData2 = manualDashboardData.Where(x => x.Indicators == 1310 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData3 = manualDashboardData.Where(x => x.Indicators == 1314 && x.ExternalValue3 == "1").ToList();
            customDataToreturn.Add(manualDashboardData1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            customDataToreturn.Add(manualDashboardData3.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            customDataToreturn.Add(manualDashboardData2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            var categoryofIncidents = customDataToreturn;


            var data = new ClinicalData
            {
                totalsentinelevents = totalsentinelevents,
                patienFallRate = patienFallRate,
                totalNearmiss = totalNearmiss,
                totaladverseincidents = totaladverseincidents,
                TotalMedicationErrors = TotalMedicationErrors,
                totalIncidentsReports = totalIncidentsReports,
                mdroRate = mdroRate,
                mrsaRate = mrsaRate,
                handHygieneCompliance = handHygieneCompliance,
                pressureUlcerIncidentRate = pressureUlcerIncidentRate,
                averageFIMScorePAR = averageFIMScorePAR,
                averageFIMScoreLTC = averageFIMScoreLTC,
                inappropriateAntiBioticUsageRate = inappropriateAntiBioticUsageRate,
                therapyInitialAssessmentProtocolCompliance = therapyInitialAssessmentProtocolCompliance,
                manualHandlingRiskAssessmentProtocolCompliance = manualHandlingRiskAssessmentProtocolCompliance,
                standardizedOutcomeMeasureProtocol = standardizedOutcomeMeasureProtocol,
                Incidents = Incidents,
                nonMedicationRelatedIncidents = nonMedicationRelatedIncidents,
                typeOfIncidents = typeOfIncidents,
                medicationErrors = medicationErrors,
                categoryofIncidents = categoryofIncidents
            };
            return data;

        }

        public JsonResult BindAllClinicalDataOnLoad(int facilityId)
        {
            var data = GetDashboardDropdownData(facilityId);
            var graphsData = GetClinicalGraphsData(facilityId, 0, 0, 0, 0);

            var jsonResult = new
            {
                data.dList,
                data.fList,
                data.ftList,
                data.rList,
                data.mList,
                data.defaultYear,
                defaultMonth = Convert.ToString(data.defaultMonth),
                facilityid = Convert.ToString(facilityId),
                graphsData.totalsentinelevents,
                graphsData.patienFallRate,
                graphsData.totalNearmiss,
                graphsData.totaladverseincidents,
                graphsData.TotalMedicationErrors,
                graphsData.totalIncidentsReports,
                graphsData.mdroRate,
                graphsData.mrsaRate,
                graphsData.handHygieneCompliance,
                graphsData.pressureUlcerIncidentRate,
                graphsData.averageFIMScorePAR,
                graphsData.averageFIMScoreLTC,
                graphsData.inappropriateAntiBioticUsageRate,
                graphsData.therapyInitialAssessmentProtocolCompliance,
                graphsData.manualHandlingRiskAssessmentProtocolCompliance,
                graphsData.standardizedOutcomeMeasureProtocol,
                graphsData.Incidents,
                graphsData.nonMedicationRelatedIncidents,
                graphsData.typeOfIncidents,
                graphsData.medicationErrors,
                graphsData.categoryofIncidents
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RebindClinicalGraphs(int facilityId, int month, int facilityType, int segment, int department)
        {
            var graphsData = GetClinicalGraphsData(facilityId, month, facilityType, segment, department);

            var jsonResult = new
            {
                graphsData.totalsentinelevents,
                graphsData.patienFallRate,
                graphsData.totalNearmiss,
                graphsData.totaladverseincidents,
                graphsData.TotalMedicationErrors,
                graphsData.totalIncidentsReports,
                graphsData.mdroRate,
                graphsData.mrsaRate,
                graphsData.handHygieneCompliance,
                graphsData.pressureUlcerIncidentRate,
                graphsData.averageFIMScorePAR,
                graphsData.averageFIMScoreLTC,
                graphsData.inappropriateAntiBioticUsageRate,
                graphsData.therapyInitialAssessmentProtocolCompliance,
                graphsData.manualHandlingRiskAssessmentProtocolCompliance,
                graphsData.standardizedOutcomeMeasureProtocol,
                graphsData.Incidents,
                graphsData.nonMedicationRelatedIncidents,
                graphsData.typeOfIncidents,
                graphsData.medicationErrors,
                graphsData.categoryofIncidents
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public class ClinicalData
        {
            public List<ManualDashboardCustomModel> totalsentinelevents { get; set; }
            public List<ManualDashboardCustomModel> patienFallRate { get; set; }
            public List<ManualDashboardCustomModel> totalNearmiss { get; set; }
            public List<ManualDashboardCustomModel> totaladverseincidents { get; set; }
            public List<ManualDashboardCustomModel> TotalMedicationErrors { get; set; }
            public List<ManualDashboardCustomModel> totalIncidentsReports { get; set; }
            public List<ManualDashboardCustomModel> mdroRate { get; set; }
            public List<ManualDashboardCustomModel> mrsaRate { get; set; }
            public List<ManualDashboardCustomModel> handHygieneCompliance { get; set; }
            public List<ManualDashboardCustomModel> pressureUlcerIncidentRate { get; set; }
            public List<ManualDashboardCustomModel> averageFIMScorePAR { get; set; }
            public List<ManualDashboardCustomModel> averageFIMScoreLTC { get; set; }
            public List<ManualDashboardCustomModel> inappropriateAntiBioticUsageRate { get; set; }
            public List<ManualDashboardCustomModel> therapyInitialAssessmentProtocolCompliance { get; set; }
            public List<ManualDashboardCustomModel> manualHandlingRiskAssessmentProtocolCompliance { get; set; }
            public List<ManualDashboardCustomModel> standardizedOutcomeMeasureProtocol { get; set; }
            public List<ManualDashboardCustomModel> Incidents { get; set; }
            public List<ManualDashboardCustomModel> nonMedicationRelatedIncidents { get; set; }
            public List<ManualDashboardCustomModel> typeOfIncidents { get; set; }
            public List<ManualDashboardCustomModel> medicationErrors { get; set; }
            public List<ManualDashboardCustomModel> categoryofIncidents { get; set; }
        }
        #endregion


        #region Financial Management Dashboard
        /// <summary>
        /// Cams the financial MGT dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult CamFinancialMGTDashboard()
        {
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var section5RemarksList = new List<DashboardRemarkCustomModel>();
            var section6RemarksList = new List<DashboardRemarkCustomModel>();
            var section7RemarksList = new List<DashboardRemarkCustomModel>();
            var section8RemarksList = new List<DashboardRemarkCustomModel>();
            var section9RemarksList = new List<DashboardRemarkCustomModel>();
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                    Convert.ToInt32(5));
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
            }
            var dashboardview = new CamFinancialMGTDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 5,
                Title = Helpers.ExternalDashboardTitleView("4"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList,
                Section7RemarksList = section7RemarksList,
                Section8RemarksList = section8RemarksList,
                Section9RemarksList = section9RemarksList,
            };
            return View(dashboardview);
        }

        public ActionResult BindAllFinancialManagementDataOnLoad(int facilityId)
        {
            var graphsData = GetFinancialManagementGraphsData(facilityId, 0, 0, 0, 0);
            var data = GetDashboardDropdownData(facilityId);
            var jsonResult = new
            {
                data.dList,
                data.fList,
                data.ftList,
                data.rList,
                data.mList,
                data.defaultYear,
                defaultMonth = Convert.ToString(data.defaultMonth),
                facilityid = Convert.ToString(facilityId),
                graphsData.netRevenue,
                graphsData.swbDirect,
                graphsData.otherDirect,
                graphsData.otherGAExpenses,
                graphsData.facilityRentandUtilities,
                graphsData.otherdirectpatientrelatedcosts,
                graphsData.consumablesPPD,
                graphsData.pharmacyPPD,
                graphsData.fBPPD,
                graphsData.newmarketdevelopmentSWB,
                graphsData.marketingBDCosts,
                graphsData.newMarketDevelopmentOtherCosts,
                graphsData.deprandAmort,
                graphsData.NursePatientRatio,
                graphsData.healthCareassistantPatientratio,
                graphsData.therapistPatientratio,
                graphsData.physicianPatientratio
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);


        }

        public FinancialManagementData GetFinancialManagementGraphsData(int facilityId, int month, int facilityType, int segment, int department)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;
            var corporateid = Helpers.GetSysAdminCorporateID();

            const string FinancialMgtIndicators = "110,111,113,115,280,259,256,257,258,260,281,282,117,142,1010,1015,1020";
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateid, Convert.ToString(FinancialMgtIndicators), currentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
            {
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            }

            var netRevenue = manualDashboardData.Where(x => x.Indicators == 110).ToList();
            var swbDirect = manualDashboardData.Where(x => x.Indicators == 111).ToList();
            var otherDirect = manualDashboardData.Where(x => x.Indicators == 113).ToList();
            var otherGAExpenses = manualDashboardData.Where(x => x.Indicators == 115).ToList();
            var facilityRentandUtilities = manualDashboardData.Where(x => x.Indicators == 280).ToList();
            var otherdirectpatientrelatedcosts = manualDashboardData.Where(x => x.Indicators == 259).ToList();
            var consumablesPPD = manualDashboardData.Where(x => x.Indicators == 256).ToList();
            var pharmacyPPD = manualDashboardData.Where(x => x.Indicators == 257).ToList();
            var fBPPD = manualDashboardData.Where(x => x.Indicators == 258).ToList();
            var newmarketdevelopmentSWB = manualDashboardData.Where(x => x.Indicators == 260).ToList();
            var marketingBDCosts = manualDashboardData.Where(x => x.Indicators == 281).ToList();
            var newMarketDevelopmentOtherCosts = manualDashboardData.Where(x => x.Indicators == 282).ToList();
            var deprandAmort = manualDashboardData.Where(x => x.Indicators == 117).ToList();
            var NursePatientRatio = manualDashboardData.Where(x => x.Indicators == 142 && x.SubCategory1 == "0").ToList();
            var healthCareassistantPatientratio = manualDashboardData.Where(x => x.Indicators == 1010).ToList();
            var therapistPatientratio = manualDashboardData.Where(x => x.Indicators == 1015).ToList();
            var physicianPatientratio = manualDashboardData.Where(x => x.Indicators == 1020).ToList();

            var data = new FinancialManagementData
            {
                netRevenue = netRevenue,
                swbDirect = swbDirect,
                otherDirect = otherDirect,
                otherGAExpenses = otherGAExpenses,
                facilityRentandUtilities = facilityRentandUtilities,
                otherdirectpatientrelatedcosts = otherdirectpatientrelatedcosts,
                consumablesPPD = consumablesPPD,
                pharmacyPPD = pharmacyPPD,
                fBPPD = fBPPD,
                newmarketdevelopmentSWB = newmarketdevelopmentSWB,
                marketingBDCosts = marketingBDCosts,
                newMarketDevelopmentOtherCosts = newMarketDevelopmentOtherCosts,
                deprandAmort = deprandAmort,
                NursePatientRatio = NursePatientRatio,
                healthCareassistantPatientratio = healthCareassistantPatientratio,
                therapistPatientratio = therapistPatientratio,
                physicianPatientratio = physicianPatientratio
            };
            return data;
        }

        public JsonResult RebindFinancialManagementGraphs(int facilityId, int month, int facilityType, int segment, int department)
        {
            var graphsData = GetFinancialManagementGraphsData(facilityId, month, facilityType, segment, department);

            var jsonResult = new
            {
                facilityid = Convert.ToString(facilityId),
                graphsData.netRevenue,
                graphsData.swbDirect,
                graphsData.otherDirect,
                graphsData.otherGAExpenses,
                graphsData.facilityRentandUtilities,
                graphsData.otherdirectpatientrelatedcosts,
                graphsData.consumablesPPD,
                graphsData.pharmacyPPD,
                graphsData.fBPPD,
                graphsData.newmarketdevelopmentSWB,
                graphsData.marketingBDCosts,
                graphsData.newMarketDevelopmentOtherCosts,
                graphsData.deprandAmort,
                graphsData.NursePatientRatio,
                graphsData.healthCareassistantPatientratio,
                graphsData.therapistPatientratio,
                graphsData.physicianPatientratio
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        public class FinancialManagementData
        {
            public List<ManualDashboardCustomModel> netRevenue { get; set; }
            public List<ManualDashboardCustomModel> swbDirect { get; set; }
            public List<ManualDashboardCustomModel> otherDirect { get; set; }
            public List<ManualDashboardCustomModel> otherGAExpenses { get; set; }
            public List<ManualDashboardCustomModel> facilityRentandUtilities { get; set; }
            public List<ManualDashboardCustomModel> otherdirectpatientrelatedcosts { get; set; }
            public List<ManualDashboardCustomModel> consumablesPPD { get; set; }
            public List<ManualDashboardCustomModel> pharmacyPPD { get; set; }
            public List<ManualDashboardCustomModel> fBPPD { get; set; }
            public List<ManualDashboardCustomModel> newmarketdevelopmentSWB { get; set; }
            public List<ManualDashboardCustomModel> marketingBDCosts { get; set; }
            public List<ManualDashboardCustomModel> newMarketDevelopmentOtherCosts { get; set; }
            public List<ManualDashboardCustomModel> deprandAmort { get; set; }
            public List<ManualDashboardCustomModel> NursePatientRatio { get; set; }
            public List<ManualDashboardCustomModel> healthCareassistantPatientratio { get; set; }
            public List<ManualDashboardCustomModel> therapistPatientratio { get; set; }
            public List<ManualDashboardCustomModel> physicianPatientratio { get; set; }
        }
        #endregion


        #region RCM Dashboard For Cambridge
        /// <summary>
        /// Cambridge RCMs the dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult CamRCMDashboard()
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
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                Convert.ToInt32(6));
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
            var dashboardview = new RCMDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 6,
                Title = Helpers.ExternalDashboardTitleView("5"),
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

        /// <summary>
        /// RCMs the graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult RCMGraphsDataUpdated(int facilityId, int month, int facilityType, int segment, int department)
        {

            var cId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;


            var customDate = month == 0
                ? currentDateTime.ToShortDateString()
                : Convert.ToDateTime(month + "/" + month + "/" + currentYear).ToShortDateString();
            var RCMDashboardData = "11,10,108,290,925,230,1000,1005,160,161";
            var indicatorsYtd = "103,270,700,701,910,242,106,229,222";

            if (facilityId == 0)
                facilityId = 9999;

            var manualDashboardList = _dbService.GetManualDashBoardStatData(facilityId, cId, indicatorsYtd,
                customDate, facilityType, segment, department);

            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, cId,
                Convert.ToString(RCMDashboardData),
                currentYear, facilityType, segment, department);

            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();

            var idgReveune = manualDashboardList.Where(x => x.IndicatorNumber.Equals("103")).ToList();
            var idnRevenue = manualDashboardList.Where(x => x.IndicatorNumber.Equals("700")).ToList();
            var outOnPassDays = manualDashboardList.Where(x => x.IndicatorNumber.Equals("701")).ToList();
            var dischargePatientDays = manualDashboardList.Where(x => x.IndicatorNumber.Equals("910") && x.SubCategoryValue1Str.Equals("0") && x.SubCategoryValue2Str.Equals("0")).ToList();
            var pdServiceCodeRevenue = manualDashboardList.Where(x => x.IndicatorNumber.Equals("242")).ToList();

            var averageLengthOfStay = manualDashboardList.Where(x => x.IndicatorNumber.Equals("106")).ToList();

            var totalOperatingBedsRevenue = manualDashboardData.Where(x => x.Indicators == 108).ToList();
            var netarbalanceRevenue = manualDashboardData.Where(x => x.Indicators == 290).ToList();
            var opRevenue = manualDashboardData.Where(x => x.Indicators == 161).ToList();
            var ipRevenue = manualDashboardData.Where(x => x.Indicators == 160).ToList();
            var payorMix = manualDashboardData.Where(x => x.Indicators == 925).ToList();
            var percentagebilledReveune = manualDashboardData.Where(x => x.Indicators == 230).ToList();
            var accountSubmittedClaims = manualDashboardData.Where(x => x.Indicators == 1000).ToList();
            var claimsResubmissionPercentage = manualDashboardData.Where(x => x.Indicators == 1005).ToList();
            var revenueByCategory = manualDashboardList.Where(x => x.IndicatorNumber.Equals("222")).ToList();
            var revenueByServiceCode = manualDashboardList.Where(x => x.IndicatorNumber.Equals("229")).ToList();


            var jsonResult = new
            {
                idgReveune,
                idnRevenue,
                outOnPassDays,
                dischargePatientDays,
                pdServiceCodeRevenue,
                averageLengthOfStay,
                totalOperatingBedsRevenue,
                netarbalanceRevenue,
                opRevenue,
                ipRevenue,
                payorMix,
                percentagebilledReveune,
                accountSubmittedClaims,
                claimsResubmissionPercentage,
                revenueByCategory,
                revenueByServiceCode
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }
        #endregion


        #region Case Management Dashboard for Cambridge



        /// <summary>
        /// Cases the management graphs data_ new.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult CaseManagementGraphsData_New(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var customDate = (Convert.ToDateTime(month + "/" + month + "/" + currentYear)).ToShortDateString();
            var corporateid = Helpers.GetSysAdminCorporateID();

            const string caseManagementIndicators = "101,102,141,900,104,901,902,903,904";
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateid,
                caseManagementIndicators, currentYear,
                facilityType, segment, department);

            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();


            var admissions = manualDashboardData.Where(x => x.Indicators == 101).ToList();
            var discharges = manualDashboardData.Where(x => x.Indicators == 102).ToList();
            var outpatientEncounters = manualDashboardData.Where(x => x.Indicators == 104).ToList();
            var totalOPAppointments = manualDashboardData.Where(x => x.Indicators == 902).ToList();

            var adminssionbyReferalSource =
                manualDashboardData.Where(
                    x => x.Indicators == 141 && x.BudgetType == 2 && x.Year == currentYear).OrderBy(x => x.SubCategory1).ToList();

            var dischargesbyDisposition =
                manualDashboardData.Where(
                    x => x.Indicators == 900 && x.BudgetType == 2 && x.Year == currentYear).OrderBy(x => x.SubCategory1).ToList();

            var oPEncountersbyType =
               manualDashboardData.Where(
                   x => x.Indicators == 901 && x.SubCategory1 != "0" && x.BudgetType == 2 && x.Year == currentYear).OrderBy(x => x.SubCategory1).ToList();

            var oPSchedulingTypes = new List<ManualDashboardCustomModel>();
            var moPSchedulingTypes1 = manualDashboardData.Where(x => x.Indicators == 903 && x.ExternalValue3 == "1").ToList();
            var moPSchedulingTypes2 = manualDashboardData.Where(x => x.Indicators == 904 && x.ExternalValue3 == "1").ToList();
            oPSchedulingTypes.Add(moPSchedulingTypes1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            oPSchedulingTypes.Add(moPSchedulingTypes2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));

            var ytdReportForSelectedMonth = _dbService.GetManualDashBoardStatData(
                facilityId != 0 ? facilityId : 9999,
                corporateid,
                "141,900",
                customDate,
                facilityType,
                segment,
                department);
            var adminssionbyReferalSourceYtd =
                ytdReportForSelectedMonth.Where(x => x.IndicatorNumber == "141").ToList();
            var dischargesbyDispositionYtd =
               ytdReportForSelectedMonth.Where(x => x.IndicatorNumber == "900").ToList();

            var jsonResult =
                new
                {
                    admissions,
                    discharges,
                    outpatientEncounters,
                    totalOPAppointments,
                    adminssionbyReferalSource,
                    dischargesbyDisposition,
                    oPEncountersbyType,
                    oPSchedulingTypes,
                    adminssionbyReferalSourceYtd,
                    dischargesbyDispositionYtd
                };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        #endregion


        /// <summary>
        /// Executives the key performance filters.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <returns></returns>
        public ActionResult ExecutiveKeyPerformanceFilters(int? facilityID, int facilityType, int segment)
        {
            //var section1RemarksList = new List<DashboardRemarkCustomModel>();
            //var section2RemarksList = new List<DashboardRemarkCustomModel>();
            //var section3RemarksList = new List<DashboardRemarkCustomModel>();

            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var facilityId = facilityID ?? facilityid;

            var dbView = GetKpiData(facilityId);
            return PartialView(PartialViews.ExecutiveKeyPerfomDashboardPview, dbView);

        }

        /// <summary>
        /// Clinicals the compliance_ backup.
        /// Created on Feb 10 2016 - Shashank
        /// Not in used But keep it as backup
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult ClinicalCompliance_Backup(int? type)
        {
            if (type != null && Convert.ToInt32(type) > 0)
            {
                var section1RemarksList = new List<DashboardRemarkCustomModel>();
                var section2RemarksList = new List<DashboardRemarkCustomModel>();
                var section3RemarksList = new List<DashboardRemarkCustomModel>();
                var section4RemarksList = new List<DashboardRemarkCustomModel>();
                var section5RemarksList = new List<DashboardRemarkCustomModel>();
                var section6RemarksList = new List<DashboardRemarkCustomModel>();
                var loggedinfacilityId = Helpers.GetDefaultFacilityId();
                var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
                var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();

                var corporateid = Helpers.GetSysAdminCorporateID();
                var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid, Convert.ToInt32(type));
                if (allRemarksList != null && allRemarksList.Count > 0)
                {
                    section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("1")).ToList();
                    section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("2")).ToList();
                    section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("3")).ToList();
                    section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("4")).ToList();
                    section5RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("5")).ToList();
                    section6RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("6")).ToList();
                }

                var dashboardview = new ExecutiveDashboardView
                {
                    FacilityId = facilityid,
                    DashboardType = 4,
                    Title = Helpers.ExternalDashboardTitleView("10"),
                    Section1RemarksList = section1RemarksList,
                    Section2RemarksList = section2RemarksList,
                    Section3RemarksList = section3RemarksList,
                    Section4RemarksList = section4RemarksList,
                    Section5RemarksList = section5RemarksList,
                    Section6RemarksList = section6RemarksList,
                };
                return View(dashboardview);
            }
            return View("Index");
        }

        /// <summary>
        /// Kpis the dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult KPIDashboard()
        {
            var patinetFallList = new List<PatientFallStats>();
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
            var section11RemarksList = new List<DashboardRemarkCustomModel>();
            var section12RemarksList = new List<DashboardRemarkCustomModel>();
            var section13RemarksList = new List<DashboardRemarkCustomModel>();

            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();

            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityid);

            var customDate = currentDateTime.Year;
            patinetFallList = _dbService.GetPatientFallRate(facilityid, Helpers.GetSysAdminCorporateID(), "", customDate, 0, 0, 0);

            var corporateid = Helpers.GetSysAdminCorporateID();
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                Convert.ToInt32(2));
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
                section11RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("11")).ToList();
                section12RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("12")).ToList();
                section13RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("13")).ToList();
            }
            var dashboardview = new KPISummaryDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 2,
                Title = Helpers.ExternalDashboardTitleView("2"),
                PatinetFallList = patinetFallList,
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
                Section11RemarksList = section11RemarksList,
                Section12RemarksList = section12RemarksList,
                Section13RemarksList = section13RemarksList
            };
            return View(dashboardview);
        }

        /// <summary>
        /// Gets the kpi dashboard filtered.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult GetKpiDashboardFiltered(int? facilityID, int month, int facilityType, int segment,
            int Department, string type)
        {
            #region Local Variables Declaration
            var patinetFallList = new List<PatientFallStats>();
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
            var section11RemarksList = new List<DashboardRemarkCustomModel>();
            var section12RemarksList = new List<DashboardRemarkCustomModel>();
            var section13RemarksList = new List<DashboardRemarkCustomModel>();
            #endregion
            var currentDateTime = _fService.GetInvariantCultureDateTime(Convert.ToInt32(facilityID));
            #region patinetFallList Data
            var customDate = currentDateTime.Year;
            patinetFallList = _dbService.GetPatientFallRate(Convert.ToInt32(facilityID),
                Helpers.GetSysAdminCorporateID(), "", customDate, facilityType, segment, Department);

            #endregion

            #region KPI Dashboard Filtered Data Section
            var loggedinfacilityId = Convert.ToInt32(facilityID);
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                ? loggedinfacilityId
                 : 17;
            #region Remarks Section
            var corporateid = Helpers.GetSysAdminCorporateID();

            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                Convert.ToInt32(2));
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
                section11RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("11")).ToList();
                section12RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("12")).ToList();
                section13RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("13")).ToList();
            }
            #endregion
            #region Dashboard Binding

            var dashboardview = new KPISummaryDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 2,
                Title = Helpers.ExternalDashboardTitleView("2"),
                PatinetFallList = patinetFallList,
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
                Section11RemarksList = section11RemarksList,
                Section12RemarksList = section12RemarksList,
                Section13RemarksList = section13RemarksList
            };
            #endregion
            return PartialView(PartialViews.KPIDashboardPView, dashboardview);
            #endregion
        }

        /// <summary>
        /// RCMs the dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult RCMDashboard()
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
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                Convert.ToInt32(6));
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
            }
            var dashboardview = new RCMDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 6,
                Title = Helpers.ExternalDashboardTitleView("5"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList,
                Section7RemarksList = section7RemarksList,
                Section8RemarksList = section8RemarksList,
                Section9RemarksList = section9RemarksList,
            };
            return View(dashboardview);
        }

        /// <summary>
        /// Cases the management dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult FinancialMGTDashboard()
        {
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            //var _fService = new FacilityService();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();

            var corporateid = Helpers.GetSysAdminCorporateID();
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var section5RemarksList = new List<DashboardRemarkCustomModel>();
            var section6RemarksList = new List<DashboardRemarkCustomModel>();
            var section7RemarksList = new List<DashboardRemarkCustomModel>();
            var section8RemarksList = new List<DashboardRemarkCustomModel>();

            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                    Convert.ToInt32(5));
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
            }
            var dashboardview = new FinancialMGTDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 5,
                Title = Helpers.ExternalDashboardTitleView("4"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList,
                Section7RemarksList = section7RemarksList,
                Section8RemarksList = section8RemarksList,
            };
            return View(dashboardview);
        }

        /// <summary>
        /// Cases the management dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult HRDashboard()
        {
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            //var _fService = new FacilityService();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var section5RemarksList = new List<DashboardRemarkCustomModel>();
            var section6RemarksList = new List<DashboardRemarkCustomModel>();

            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                    Convert.ToInt32(8));
            if (allRemarksList != null && allRemarksList.Count > 0)
            {
                section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("1")).ToList();
                section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("2")).ToList();
                section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("3")).ToList();
                section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("4")).ToList();
                section5RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("5")).ToList();
                section6RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("6")).ToList();
            }
            var dashboardview = new HRDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 8,
                Title = Helpers.ExternalDashboardTitleView("7"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList
            };
            return View(dashboardview);
        }

        /// <summary>
        /// Cases the management dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult CaseManagementDashboard()
        {
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var section5RemarksList = new List<DashboardRemarkCustomModel>();

            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                    Convert.ToInt32(7));
            if (allRemarksList != null && allRemarksList.Count > 0)
            {
                section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("1")).ToList();
                section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("2")).ToList();
                section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("3")).ToList();
                section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("4")).ToList();
                section5RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("5")).ToList();
            }
            var dashboardview = new CaseMgtDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 7,
                Title = Helpers.ExternalDashboardTitleView("6"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList
            };
            return View(dashboardview);
        }

        /// <summary>
        /// Gets the manual dashboard data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult GetManualDashboardData(int facilityID, int month, int facilityType, int segment, int Department, string type)
        {
            var manualDashboardData = new List<ManualDashboardCustomModel>();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityID);
            var curentYear = currentDateTime.Year;
            if (type == "124")
            {
                var totalOperatingExpenditures = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "165", curentYear, facilityType, segment, Department);
                var netCapitalExpendureOperations =
                    _dbService.GetManualDashBoard(facilityID,
                        Helpers.GetSysAdminCorporateID(), "127", curentYear, facilityType, segment, Department);
                var netCashCollection = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "143", curentYear, facilityType, segment, Department);
                manualDashboardData.AddRange(netCashCollection.Where(x => x.BudgetType == 2 && x.Year == curentYear));
                manualDashboardData.AddRange(totalOperatingExpenditures.Where(x => x.BudgetType == 2 && x.Year == curentYear));
                manualDashboardData.AddRange(netCapitalExpendureOperations.Where(x => x.BudgetType == 2 && x.Year == curentYear));
            }
            else
            {
                if (type == "156") //.....Average Daily census By Service Code
                {
                    manualDashboardData = _dbService.GetManualDashBoard(facilityID, corporateId,
                           Convert.ToString("242"),
                           curentYear, facilityType, segment, Department);

                    var averageDailyCencus = GetADCByServiceCodePerMonth(manualDashboardData).OrderBy(x => x.SubCategory1);
                    return Json(averageDailyCencus.Any() ? averageDailyCencus : null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    manualDashboardData = _dbService.GetManualDashBoard(facilityID, corporateId,
                        Convert.ToString(type),
                        curentYear, facilityType, segment, Department);
                    if (type == "242")
                    {
                        var patientDays = GetPatientDaysAll(manualDashboardData);
                        return Json(patientDays.Any() ? patientDays : null, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            return Json(manualDashboardData.Any() ? manualDashboardData : null, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Gets the statistics data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult GetStatisticsData(int facilityID, int month, int facilityType, int segment, int Department, string type)
        {
            var dashboardTypeid = Convert.ToInt32(type);
            var manualDashboardData = new List<ManualDashboardCustomModel>();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityID);
            var curentYear = currentDateTime.Year;
            if (type == "1")
            {
                var addmissionslist = _dbService.GetManualDashBoard(facilityID,
                        Helpers.GetSysAdminCorporateID(), "101", curentYear, facilityType, segment, Department);
                var dischargeslist = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "102", curentYear, facilityType, segment, Department);
                var inaptientDayslist = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "103", curentYear, facilityType, segment, Department);
                var opEncountersList = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "104", curentYear, facilityType, segment, Department);
                var netrevunelist = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "110", curentYear, facilityType, segment, Department);
                var occupencyRateList = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "109", curentYear, facilityType, segment, Department);
                var netreveunelist = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "120", curentYear, facilityType, segment, Department);
                var inderectreveunelist = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "121", curentYear, facilityType, segment, Department);
                var netIncomeLosslist = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "119", curentYear, facilityType, segment, Department);
                var netCashFromOperations = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "124", curentYear, facilityType, segment, Department);
                var netCapitalExpendureOperations = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "127", curentYear, facilityType, segment, Department);
                var netcashOperations = _dbService.GetManualDashBoard(facilityID,
                    Helpers.GetSysAdminCorporateID(), "143", curentYear, facilityType, segment, Department);
                manualDashboardData.AddRange(addmissionslist);
                manualDashboardData.AddRange(dischargeslist);
                manualDashboardData.AddRange(inaptientDayslist);
                manualDashboardData.AddRange(opEncountersList);
                manualDashboardData.AddRange(netrevunelist);
                manualDashboardData.AddRange(occupencyRateList);
                manualDashboardData.AddRange(netreveunelist);
                manualDashboardData.AddRange(inderectreveunelist);
                manualDashboardData.AddRange(netIncomeLosslist);
                manualDashboardData.AddRange(netCashFromOperations);
                manualDashboardData.AddRange(netCapitalExpendureOperations);
                manualDashboardData.AddRange(netcashOperations);

            }
            return PartialView(PartialViews.ExternalDashboardList, manualDashboardData);
        }

        /// <summary>
        /// Gets the manual dashboard stat data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult GetManualDashboardStatData(int facilityID, int month, int facilityType, int segment,
            int Department, string type)
        {
            var manualDashboardData = new List<ExternalDashboardModel>();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityID);
            var curentYear = currentDateTime.Year;
            if (type == "1")
            {
                var customDate = Convert.ToDateTime("01/" + month + "/" + curentYear).ToShortDateString();
                var addmissionslist = _dbService.GetManualDashBoardStatData(facilityID, Helpers.GetSysAdminCorporateID(), "101", customDate, facilityType, segment, Department);
            }
            return PartialView(PartialViews.VolumeExecutiveStats, manualDashboardData);
        }

        /// <summary>
        /// Gets the year to date data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult GetYearToDateData(int facilityID, int month, int facilityType, int segment,
            int Department, string type)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityID);
            var curentYear = currentDateTime.Year;

            var customDate = month == 0
                    ? currentDateTime.ToShortDateString()
                    : Convert.ToDateTime(month + "/" + month + "/" + curentYear).ToShortDateString();

            var manualDashboardList = _dbService.GetManualDashBoardStatData(facilityID,
              Helpers.GetSysAdminCorporateID(), "", customDate, facilityType, segment, Department);
            if (type == "109")
            {
                var bedoccupancyRate = manualDashboardList != null
                    ? manualDashboardList.SingleOrDefault(x => x.IndicatorNumber == "109")
                    : null;
                var ebitdaMargin = manualDashboardList != null
                    ? manualDashboardList.SingleOrDefault(x => x.IndicatorNumber == "122")
                    : null;
                var jsonResult = new
                {
                    bedoccupancyRate,
                    ebitdaMargin,
                };
                return Json(jsonResult ?? null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var datatoreturn = manualDashboardList != null
                    ? manualDashboardList.SingleOrDefault(x => x.IndicatorNumber == type)
                    : null;

                return Json(datatoreturn ?? null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the sub category charts.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult GetSubCategoryCharts(int facilityID, int month, int facilityType, int segment,
            int Department, string type)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityID);
            var currentYear = currentDateTime.Year;


            var manualDashboardData = _dbService.GetSubCategoryCharts(facilityID, corporateId,
                Convert.ToString(type), currentYear, facilityType, segment, Department);

            if (type == "156" || type == "159" || type == "142" || type == "141")
            {
                if (manualDashboardData.Any())
                {
                    var newIndicatorLine = new ManualDashboardCustomModel
                    {
                        M1 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M1)).ToString(),
                        M2 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M2)).ToString(),
                        M3 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M3)).ToString(),
                        M4 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M4)).ToString(),
                        M5 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M5)).ToString(),
                        M6 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M6)).ToString(),
                        M7 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M7)).ToString(),
                        M8 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M8)).ToString(),
                        M9 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M9)).ToString(),
                        M10 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M10)).ToString(),
                        M11 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M11)).ToString(),
                        M12 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M12)).ToString(),
                    };
                    var lastrow = newIndicatorLine;
                    foreach (var item in manualDashboardData)
                    {
                        item.M1 = lastrow.M1 != "0.00"
                            ? ((Convert.ToDecimal(item.M1) / Convert.ToDecimal(lastrow.M1)) * 100).ToString()
                            : "0.00";
                        item.M2 = lastrow.M2 != "0.00"
                            ? ((Convert.ToDecimal(item.M2) / Convert.ToDecimal(lastrow.M2)) * 100).ToString()
                            : "0.00";
                        item.M3 = lastrow.M3 != "0.00"
                            ? ((Convert.ToDecimal(item.M3) / Convert.ToDecimal(lastrow.M3)) * 100).ToString()
                            : "0.00";
                        item.M4 = lastrow.M4 != "0.00"
                            ? ((Convert.ToDecimal(item.M4) / Convert.ToDecimal(lastrow.M4)) * 100).ToString()
                            : "0.00";
                        item.M5 = lastrow.M5 != "0.00"
                            ? ((Convert.ToDecimal(item.M5) / Convert.ToDecimal(lastrow.M5)) * 100).ToString()
                            : "0.00";
                        item.M6 = lastrow.M6 != "0.00"
                            ? ((Convert.ToDecimal(item.M6) / Convert.ToDecimal(lastrow.M6)) * 100).ToString()
                            : "0.00";
                        item.M7 = lastrow.M7 != "0.00"
                            ? ((Convert.ToDecimal(item.M7) / Convert.ToDecimal(lastrow.M7)) * 100).ToString()
                            : "0.00";
                        item.M8 = lastrow.M8 != "0.00"
                            ? ((Convert.ToDecimal(item.M8) / Convert.ToDecimal(lastrow.M8)) * 100).ToString()
                            : "0.00";
                        item.M9 = lastrow.M9 != "0.00"
                            ? ((Convert.ToDecimal(item.M9) / Convert.ToDecimal(lastrow.M9)) * 100).ToString()
                            : "0.00";
                        item.M10 = lastrow.M10 != "0.00"
                            ? ((Convert.ToDecimal(item.M10) / Convert.ToDecimal(lastrow.M10)) * 100).ToString()
                            : "0.00";
                        item.M11 = lastrow.M11 != "0.00"
                            ? ((Convert.ToDecimal(item.M11) / Convert.ToDecimal(lastrow.M11)) * 100).ToString()
                            : "0.00";
                        item.M12 = lastrow.M12 != "0.00"
                            ? ((Convert.ToDecimal(item.M12) / Convert.ToDecimal(lastrow.M12)) * 100).ToString()
                            : "0.00";
                    }
                }
            }
            manualDashboardData = manualDashboardData.All(
                 x =>
                     x.M1 == "0.00" && x.M2 == "0.00" && x.M3 == "0.00" && x.M4 == "0.00" && x.M5 == "0.00" &&
                     x.M6 == "0.00" && x.M7 == "0.00" && x.M8 == "0.00" && x.M9 == "0.00" && x.M10 == "0.00" &&
                     x.M11 == "0.00" && x.M12 == "0.00")
                 ? null
                 : manualDashboardData;
            return Json(manualDashboardData ?? null, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Views the section narratives.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <param name="type">The type.</param>
        /// <param name="viewAll">if set to <c>true</c> [view all].</param>
        /// <param name="sectionType">Type of the section.</param>
        /// <returns></returns>
        public ActionResult ViewSectionNarratives(int facilityID, int month, int facilityType, int segment,
            int department, int type, bool viewAll, string sectionType)
        {
            var listtoReturn = new List<DashboardRemarkCustomModel>();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var remarksList = viewAll
                    ? _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityID, type)
                    : _drService.GetDashboardRemarkListByDashboardTypeAndMonth(corporateid, facilityID, type, month);

            if (sectionType == "2")
            {
                var section2Enumlist = Enum.GetValues(typeof(Section2Narrations)).OfType<object>().ToList();
                var section2 = section2Enumlist.Select(enumValue => new SelectListItem
                {
                    Text = enumValue.ToString(),
                    Value = Convert.ToString((int)enumValue),
                });
                listtoReturn =
                    remarksList.Where(
                        x => section2.Any(r => r.Value == x.DashboardSection.Trim())).ToList();
            }
            if (sectionType == "6")
            {
                var section6Enumlist = Enum.GetValues(typeof(Section6Narrations)).OfType<object>().ToList();

                var section6 = section6Enumlist.Select(enumValue => new SelectListItem
                {
                    Text = enumValue.ToString(),
                    Value = Convert.ToString((int)enumValue),
                });
                listtoReturn =
                    remarksList.Where(
                        x => section6.Any(r => r.Value == x.DashboardSection.Trim())).ToList();
            }
            if (sectionType == "6")
            {
                var section11Enumlist = Enum.GetValues(typeof(Section11Narrations)).OfType<object>().ToList();
                var section11 = section11Enumlist.Select(enumValue => new SelectListItem
                {
                    Text = enumValue.ToString(),
                    Value = Convert.ToString((int)enumValue),
                });
                listtoReturn =
                    remarksList.Where(
                        x => section11.Any(r => r.Value == x.DashboardSection.Trim())).ToList();
            }
            return PartialView(PartialViews.RemarksList, listtoReturn);
        }

        /// <summary>
        /// Views the section narratives.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="type">The type.</param>
        /// <param name="viewAll">if set to <c>true</c> [view all].</param>
        /// <param name="sectionType">Type of the section.</param>
        /// <returns></returns>
        public ActionResult ViewRemarkdsListByDashboardSection(int facilityId, int month, int type, bool viewAll, string sectionType)
        {
            var corporateid = Helpers.GetSysAdminCorporateID();
            var list = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityId, type);
            if (list.Count > 0)
            {
                list = viewAll
                    ? list.Where(s => s.DashboardSection.Equals(sectionType)).ToList()
                    : list.Where(s => s.DashboardSection.Equals(sectionType) && s.Month == month).ToList();
            }
            return PartialView(PartialViews.RemarksList, list);
        }

        /// <summary>
        /// Clinicals the graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult ClinicalGraphsData(int facilityId, int month, int facilityType, int segment, int department)
        {
            var clinicalGrpahsArray = "172,177,176,166,166,167,168,169,170,171,174,175,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,1312,1313,1314,1311,1309,1310";

            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var curentYear = currentDateTime.Year;

            var corporateId = Helpers.GetSysAdminCorporateID();
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateId,
                Convert.ToString(clinicalGrpahsArray),
                curentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var hamList = manualDashboardData.Where(x => x.Indicators == 172).ToList();
            var fallRiskList = manualDashboardData.Where(x => x.Indicators == 177).ToList();
            var painManagementList = manualDashboardData.Where(x => x.Indicators == 176).ToList();
            var dischargeToComunityrate = manualDashboardData.Where(x => x.Indicators == 166).ToList();

            var lostReferals = manualDashboardData.Where(x => x.Indicators == 167).ToList();
            var transferOutRate = manualDashboardData.Where(x => x.Indicators == 168).ToList();
            var nursingStaffCompetency = manualDashboardData.Where(x => x.Indicators == 169).ToList();
            var nursingDepartmentOrientation = manualDashboardData.Where(x => x.Indicators == 170).ToList();

            var patientIdentification = manualDashboardData.Where(x => x.Indicators == 171).ToList();
            var patientFallRatewithInjury = manualDashboardData.Where(x => x.Indicators == 174).ToList();
            var pressureUlcerIncidentRate = manualDashboardData.Where(x => x.Indicators == 175).ToList();
            var fallRiskAssessmentProtocolComplianceRate = manualDashboardData.Where(x => x.Indicators == 177).ToList();

            var compliancetoPressureUlcerPreventionProtocol = manualDashboardData.Where(x => x.Indicators == 178).ToList();
            var sbarProtocolComplianceRate = manualDashboardData.Where(x => x.Indicators == 179).ToList();
            var mdroRate = manualDashboardData.Where(x => x.Indicators == 180).ToList();
            var mrsaRate = manualDashboardData.Where(x => x.Indicators == 181).ToList();

            var esblRate = manualDashboardData.Where(x => x.Indicators == 182).ToList();
            var lrtiRate = manualDashboardData.Where(x => x.Indicators == 183).ToList();
            var cautiRate = manualDashboardData.Where(x => x.Indicators == 184).ToList();
            var bsiRate = manualDashboardData.Where(x => x.Indicators == 185).ToList();

            var handHygineComplainceRate = manualDashboardData.Where(x => x.Indicators == 186).ToList();
            var staffVaccinationRate = manualDashboardData.Where(x => x.Indicators == 187).ToList();
            var inappropriateAntiBioticUsageRate = manualDashboardData.Where(x => x.Indicators == 188).ToList();
            var therapyInitialAssessmentProtocolCompliance = manualDashboardData.Where(x => x.Indicators == 189).ToList();

            var manualHandlingRiskAssessmentProtocolCompliance = manualDashboardData.Where(x => x.Indicators == 190).ToList();
            var standardizedOutcomeMeasureProtocol = manualDashboardData.Where(x => x.Indicators == 191).ToList();
            var Incidents = manualDashboardData.Where(x => x.Indicators == 192).ToList();
            var nonMedicationRelatedIncidents = manualDashboardData.Where(x => x.Indicators == 1312 && x.Year == curentYear).ToList();
            var typeOfIncidents = manualDashboardData.Where(x => x.Indicators == 1313 && x.Year == curentYear).ToList();
            //var categoryofIncidents = manualDashboardData.Where(x => x.Indicators == 1314).ToList();  //categoryofIncidents
            var medicationErrors = manualDashboardData.Where(x => x.Indicators == 1311 && x.Year == curentYear).ToList();  //categoryofIncidents

            var customDataToreturn = new List<ManualDashboardCustomModel>();
            var manualDashboardData1 = manualDashboardData.Where(x => x.Indicators == 1309 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData2 = manualDashboardData.Where(x => x.Indicators == 1310 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData3 = manualDashboardData.Where(x => x.Indicators == 1314 && x.ExternalValue3 == "1").ToList();
            customDataToreturn.Add(manualDashboardData1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == curentYear));
            customDataToreturn.Add(manualDashboardData3.FirstOrDefault(x => x.BudgetType == 2 && x.Year == curentYear));
            customDataToreturn.Add(manualDashboardData2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == curentYear));
            var categoryofIncidents = customDataToreturn;

            var jsonResult = new
            {
                hamList,
                fallRiskList,
                painManagementList,
                dischargeToComunityrate,
                lostReferals,
                transferOutRate,
                nursingStaffCompetency,
                nursingDepartmentOrientation,
                patientIdentification,
                patientFallRatewithInjury,
                pressureUlcerIncidentRate,
                fallRiskAssessmentProtocolComplianceRate,
                compliancetoPressureUlcerPreventionProtocol,
                sbarProtocolComplianceRate,
                mdroRate,
                mrsaRate,
                esblRate,
                lrtiRate,
                cautiRate,
                bsiRate,
                handHygineComplainceRate,
                staffVaccinationRate,
                inappropriateAntiBioticUsageRate,
                therapyInitialAssessmentProtocolCompliance,
                manualHandlingRiskAssessmentProtocolCompliance,
                standardizedOutcomeMeasureProtocol,
                Incidents,
                nonMedicationRelatedIncidents,
                typeOfIncidents,
                categoryofIncidents,
                medicationErrors
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// RCMs the graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult RCMGraphsData(int facilityId, int month, int facilityType, int segment, int department)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            //const string rcmDashboardData = "1308,221,222,224,144,101,102,225,226,227,228,161,242,229,230,231,232,159,270,131";
            const string rcmDashboardData = "1308,221,222,224,144,101,102,225,108,227,228,161,242,229,230,231,232,159,270,131";

            var customDate = month == 0 ? currentDateTime.ToShortDateString() : Convert.ToDateTime(month + "/" + month + "/" + currentYear).ToShortDateString();
            var manualDashboardList = _dbService.GetManualDashBoardStatData(facilityId,
                Helpers.GetSysAdminCorporateID(), "270", customDate, facilityType, segment, department);

            var corporateId = Helpers.GetSysAdminCorporateID();
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateId,
                Convert.ToString(rcmDashboardData),
                currentYear, facilityType, segment, department);

            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var ipReveune = manualDashboardData.Where(x => x.Indicators == 1308).ToList();
            var reveunePPD = manualDashboardData.Where(x => x.Indicators == 221).ToList();
            var revenueIP = manualDashboardData.Where(x => x.Indicators == 222).ToList();

            var revenueDaman = revenueIP != null && revenueIP.Any()
                ? revenueIP.Where(x => x.SubCategory1 == "2").ToList()
                : revenueIP;
            var reveuneRoyalFamily = revenueIP != null && revenueIP.Any()
                ? revenueIP.Where(x => x.SubCategory1 == "31").ToList()
                : revenueIP; //Revenue Royal Family

            var reveuneInpatinetOther = manualDashboardData.Where(x => x.Indicators == 224).ToList();

            var averageDailyCencus = manualDashboardData.Where(x => x.Indicators == 144).ToList();
            var newAdmissions = manualDashboardData.Where(x => x.Indicators == 101).ToList();
            var plannedDischarges = manualDashboardData.Where(x => x.Indicators == 102).ToList();
            var dischargesUnplanned = manualDashboardData.Where(x => x.Indicators == 225).ToList();

            var acuteOutsPatients = manualDashboardData.Where(x => x.Indicators == 108).ToList();
            var acuteOutsDays = manualDashboardData.Where(x => x.Indicators == 227).ToList();
            var therapeuticLeaves = manualDashboardData.Where(x => x.Indicators == 228).ToList();
            var opRevenue = manualDashboardData.Where(x => x.Indicators == 161).ToList();
            var patientDaysList = manualDashboardData.Where(x => x.Indicators == 242).ToList();
            var patientDays = GetPatientDaysAll(patientDaysList);
            var serviceCodeDistributionbyBilledClaims = manualDashboardData.Where(x => x.Indicators == 229).ToList();
            var percentagebilledReveune = manualDashboardData.Where(x => x.Indicators == 230).ToList();
            var accountSubmittedClaims = manualDashboardData.Where(x => x.Indicators == 231).ToList();
            var numberSubmittedClaims = manualDashboardData.Where(x => x.Indicators == 232).ToList();

            var payorMix = GetSubCategoryChartsPayorMix(facilityId, month, facilityType, segment, department, "159");//payor Mix
            var claimsResubmissionPercentage = manualDashboardList.Where(x => x.IndicatorNumber == "270").ToList();
            var arDays = manualDashboardData.Where(x => x.Indicators == 131).ToList(); //GetGraphsData(facilityId, month, facilityType, segment, department, ""); //manualDashboardList.Where(x => x.IndicatorNumber == "131").ToList(); 


            var jsonResult = new
            {
                ipReveune,
                reveunePPD,
                revenueDaman,
                reveuneRoyalFamily,
                reveuneInpatinetOther,
                averageDailyCencus,
                arDays,
                newAdmissions,
                plannedDischarges,
                dischargesUnplanned,
                acuteOutsPatients,
                acuteOutsDays,
                therapeuticLeaves,
                opRevenue,
                patientDays,
                serviceCodeDistributionbyBilledClaims,
                percentagebilledReveune,
                accountSubmittedClaims,
                numberSubmittedClaims,
                payorMix,
                claimsResubmissionPercentage
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cases the management graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult CaseManagementGraphsData(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var corporateid = Helpers.GetSysAdminCorporateID();
            var caseManagementIndicators = "227,233,234,235,236,237,238,239,240,255";
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateid,
                Convert.ToString(caseManagementIndicators),
                currentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var acuteOutDays = manualDashboardData.Where(x => x.Indicators == 227).ToList();
            var acuteOut = manualDashboardData.Where(x => x.Indicators == 233).ToList();
            var therapueticLeaves = manualDashboardData.Where(x => x.Indicators == 234).ToList();
            var presenceInitialAssessment = manualDashboardData.Where(x => x.Indicators == 235).ToList();
            var presenceMDTdocumentation = manualDashboardData.Where(x => x.Indicators == 236).ToList();
            var presenceDischargedDocumentation = manualDashboardData.Where(x => x.Indicators == 237).ToList();
            var dischargeDisposition = manualDashboardData.Where(x => x.Indicators == 238).ToList();
            var numberUnplannedDischarges = manualDashboardData.Where(x => x.Indicators == 239).ToList();
            var postDischargeFollowup = manualDashboardData.Where(x => x.Indicators == 240).ToList();
            var lostReferral = manualDashboardData.Where(x => x.Indicators == 255).ToList();
            var jsonResult = new
            {
                acuteOutDays,
                acuteOut,
                therapueticLeaves,
                presenceInitialAssessment,
                presenceMDTdocumentation,
                presenceDischargedDocumentation,
                dischargeDisposition,
                numberUnplannedDischarges,
                postDischargeFollowup,
                lostReferral
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the financial MGT graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult GetFinancialMGTGraphsData(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var curentYear = currentDateTime.Year;

            var customDate = (Convert.ToDateTime(month + "/" + month + "/" + curentYear)).ToShortDateString();
            //var manualDashboardList = _dbService.GetManualDashBoardStatData(facilityId,
            //    Helpers.GetSysAdminCorporateID(), "", customDate, 0, 0, 0);
            var corporateid = Helpers.GetSysAdminCorporateID();
            var financialMgtIndicators = "110,111,112,113,155,115,117,118,125,126,127,129,130,240,243,244,245,255";
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateid,
                Convert.ToString(financialMgtIndicators),
                curentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var netRevenue = manualDashboardData.Where(x => x.Indicators == 110).ToList();
            var swbDirect = manualDashboardData.Where(x => x.Indicators == 111).ToList();
            var supplies = manualDashboardData.Where(x => x.Indicators == 112).ToList();
            var otherDirect = manualDashboardData.Where(x => x.Indicators == 113).ToList();
            var swbinDirect = manualDashboardData.Where(x => x.Indicators == 155).ToList();
            var otherIndirectCosts = manualDashboardData.Where(x => x.Indicators == 115).ToList();
            var deprAndAmort = manualDashboardData.Where(x => x.Indicators == 117).ToList();
            var interest = manualDashboardData.Where(x => x.Indicators == 118).ToList();
            var changesWorkingCapital = manualDashboardData.Where(x => x.Indicators == 125).ToList();
            var otherAdjustments = manualDashboardData.Where(x => x.Indicators == 126).ToList();
            var capitalExpenditures = manualDashboardData.Where(x => x.Indicators == 127).ToList();
            var cashInBank = manualDashboardData.Where(x => x.Indicators == 129).ToList();
            var daysTotalExpendituresInCash = manualDashboardData.Where(x => x.Indicators == 130).ToList();
            var totalCashCollected = manualDashboardData.Where(x => x.Indicators == 243).ToList();
            var totalPayablesSalaries = manualDashboardData.Where(x => x.Indicators == 244).ToList();
            var totalCapitalPaymentsMade = manualDashboardData.Where(x => x.Indicators == 245).ToList();

            var jsonResult = new
            {
                netRevenue,
                swbDirect,
                supplies,
                otherDirect,
                swbinDirect,
                otherIndirectCosts,
                deprAndAmort,
                interest,
                changesWorkingCapital,
                otherAdjustments,
                capitalExpenditures,
                cashInBank,
                daysTotalExpendituresInCash,
                totalCashCollected,
                totalPayablesSalaries,
                totalCapitalPaymentsMade
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the financial MGT graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult GetHRGraphsData(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var curentYear = currentDateTime.Year;

            var customDate = (Convert.ToDateTime(month + "/" + month + "/" + curentYear)).ToShortDateString();
            //var manualDashboardList = _dbService.GetManualDashBoardStatData(facilityId,
            //    Helpers.GetSysAdminCorporateID(), "", customDate, 0, 0, 0);

            var corporateid = Helpers.GetSysAdminCorporateID();
            var hrGraphsIndicators = "254,246,247,248,211,250,251,252,253,277";
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateid,
                Convert.ToString(hrGraphsIndicators),
                curentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var FTEs = manualDashboardData.Where(x => x.Indicators == 254).ToList();
            var headcount = manualDashboardData.Where(x => x.Indicators == 246).ToList();
            var overtimeHours = manualDashboardData.Where(x => x.Indicators == 247).ToList();
            var attritionRate = manualDashboardData.Where(x => x.Indicators == 248).ToList();
            var employeeEngagementScore = manualDashboardData.Where(x => x.Indicators == 211).ToList();
            var administrationTotalStaff = manualDashboardData.Where(x => x.Indicators == 250).ToList();
            var timetakenToRecruitVacantPos = manualDashboardData.Where(x => x.Indicators == 251).ToList();
            var productiveHours = manualDashboardData.Where(x => x.Indicators == 252).ToList();
            var unproductiveHours = manualDashboardData.Where(x => x.Indicators == 253).ToList();
            var nursingHoursPPD = manualDashboardData.Where(x => x.Indicators == 277).ToList();
            var jsonResult = new
            {
                FTEs,
                headcount,
                overtimeHours,
                attritionRate,
                employeeEngagementScore,
                administrationTotalStaff,
                timetakenToRecruitVacantPos,
                productiveHours,
                unproductiveHours,
                nursingHoursPPD
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the kpi graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult GetKPIGraphsData(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var customDate = (Convert.ToDateTime(month + "/" + month + "/" + currentYear)).ToShortDateString();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var hrGraphsIndicators = "254,246,172,174,247,248,255,236,237,233,234,1312,1313,1311,1314,1309,1310,227";
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateid,
                Convert.ToString(hrGraphsIndicators),
                currentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var FTEs = manualDashboardData.Where(x => x.Indicators == 254).ToList();
            var headcount = manualDashboardData.Where(x => x.Indicators == 246).ToList();
            var highAlertMedicationChart = manualDashboardData.Where(x => x.Indicators == 172).ToList();
            var patientFallRateGraph = manualDashboardData.Where(x => x.Indicators == 174).ToList();
            var overtimeHours = manualDashboardData.Where(x => x.Indicators == 247).ToList();
            var attritionRate = manualDashboardData.Where(x => x.Indicators == 248).ToList();
            var lostReferral = manualDashboardData.Where(x => x.Indicators == 255).ToList();
            var presenceMDTdocumentation = manualDashboardData.Where(x => x.Indicators == 236).ToList();
            var acuteOutDays = manualDashboardData.Where(x => x.Indicators == 227).ToList();
            var acuteOut = manualDashboardData.Where(x => x.Indicators == 233).ToList();
            var therapueticLeaves = manualDashboardData.Where(x => x.Indicators == 234).ToList();
            var nonMedicationRelatedIncidents = manualDashboardData.Where(x => x.Indicators == 1312 && x.Year == currentYear && x.BudgetType == 2).ToList();
            var typeOfIncidents = manualDashboardData.Where(x => x.Indicators == 1313 && x.Year == currentYear).ToList();

            var customDataToreturn = new List<ManualDashboardCustomModel>();
            var manualDashboardData1 = manualDashboardData.Where(x => x.Indicators == 1309 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData2 = manualDashboardData.Where(x => x.Indicators == 1310 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData3 = manualDashboardData.Where(x => x.Indicators == 1314 && x.ExternalValue3 == "1").ToList();
            customDataToreturn.Add(manualDashboardData1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            customDataToreturn.Add(manualDashboardData3.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            customDataToreturn.Add(manualDashboardData2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            var categoryofIncidents = customDataToreturn;

            var medicationErrors = manualDashboardData.Where(x => x.Indicators == 1311 && x.Year == currentYear).ToList();  //categoryofIncidents

            var jsonResult = new
            {
                FTEs,
                headcount,
                highAlertMedicationChart,
                patientFallRateGraph,
                overtimeHours,
                attritionRate,
                lostReferral,
                presenceMDTdocumentation,
                acuteOutDays,
                acuteOut,
                therapueticLeaves,
                nonMedicationRelatedIncidents,
                typeOfIncidents,
                categoryofIncidents,
                medicationErrors
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Executives the key performance.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        public ActionResult ProjectsDashboard()
        {
            //const int dashboardType = 9;
            //var section1RemarksList = new List<DashboardRemarkCustomModel>();
            //var section2RemarksList = new List<DashboardRemarkCustomModel>();
            //var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var corporateid = Helpers.GetSysAdminCorporateID();
            //const int facilityid = 0;
            var userId = Helpers.GetLoggedInUserId();

            var users = _uService.GetNonAdminUsersByCorporate(corporateid);
            var ifAny = users.Any(u => u.UserID == userId);
            if (!ifAny)
                userId = 0;


            var dashboardview = GetProjectsData(0, userId);
            return View(dashboardview);

        }

        /// <summary>
        /// Executives the key performance filters.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult ProjectsDashboardFilters(int? facilityID, int facilityType, int segment, string userId)
        {
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            //var _fService = new FacilityService();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var facilityId = facilityID ?? facilityid;

            var dashboardview = GetProjectsData(facilityId, Convert.ToInt32(userId));
            return PartialView(PartialViews.ProjectsDashView, dashboardview);

        }

        public JsonResult GetProjectTaskComments(int taskId)
        {
            var comments = _ptService.GetProjectTaskCommentById(taskId);
            return Json(comments, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Downloads the snapshot.
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadSnapshot()
        {
            var fileBytes = System.IO.File.ReadAllBytes(Convert.ToString(Session["AttachedSnapShot"]));
            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                Response.Cookies.Add(cookie);
                exportData.Write(fileBytes, 0, fileBytes.Length);
                return File(exportData.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "Report.png");
            }
        }

        #region New Version of the Dashboard Database value

        /// <summary>
        /// Gets the manual dashboard data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult GetManualDashboardDataV1(int facilityID, int month, int facilityType, int segment, int Department, string type)
        {
            var manualDashboardData = new List<ManualDashboardCustomModel>();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityID);
            var currentYear = currentDateTime.Year;

            if (type == "124")
            {
                var totalOperatingExpenditures = _dbService.GetManualDashBoardV1(facilityID,
                    Helpers.GetSysAdminCorporateID(), "165", currentYear, facilityType, segment, Department);
                var netCapitalExpendureOperations = _dbService.GetManualDashBoardV1(facilityID, Helpers.GetSysAdminCorporateID(), "127", currentYear, facilityType, segment, Department);
                var netCashCollection = _dbService.GetManualDashBoardV1(facilityID, Helpers.GetSysAdminCorporateID(), "143", currentYear, facilityType, segment, Department);
                manualDashboardData.AddRange(netCashCollection.Where(x => x.BudgetType == 2 && x.Year == currentYear));
                manualDashboardData.AddRange(totalOperatingExpenditures.Where(x => x.BudgetType == 2 && x.Year == currentYear));
                manualDashboardData.AddRange(netCapitalExpendureOperations.Where(x => x.BudgetType == 2 && x.Year == currentYear));
            }
            else
            {
                if (type == "156") //.....Average Daily census By Service Code
                {
                    manualDashboardData = _dbService.GetManualDashBoardV1(facilityID, corporateId,
                           Convert.ToString("242"),
                           currentYear, facilityType, segment, Department);

                    var averageDailyCencus = GetADCByServiceCodePerMonth(manualDashboardData).OrderBy(x => x.SubCategory1);
                    return Json(averageDailyCencus.Any() ? averageDailyCencus : null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    manualDashboardData = _dbService.GetManualDashBoardV1(facilityID, corporateId,
                        Convert.ToString(type),
                        currentYear, facilityType, segment, Department);
                    if (type == "242")
                    {
                        var patientDays = GetPatientDaysAll(manualDashboardData);
                        return Json(patientDays.Any() ? patientDays : null, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            return Json(manualDashboardData.Any() ? manualDashboardData : null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Executives the dashboard view v1.
        /// </summary>
        /// <returns></returns>
        public ActionResult ExecutiveDashboardViewV1()
        {
            var dashboardview = new ExecutiveDashboardView
            {
                FacilityId = 0,
                DashboardType = 1,
                Title = Helpers.ExternalDashboardTitleView("1")
            };
            return View(dashboardview);
        }


        /// <summary>
        /// Clinical Quality Dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult ClinicalQualityV1(int? type)
        {
            if (type != null && Convert.ToInt32(type) > 0)
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
                var loggedinfacilityId = Helpers.GetDefaultFacilityId();
                //var _fService = new FacilityService();
                var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
                var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();
                var corporateid = Helpers.GetSysAdminCorporateID();

                var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid, Convert.ToInt32(type));
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
                }

                var dashboardview = new ExecutiveDashboardView
                {
                    FacilityId = facilityid,
                    DashboardType = 4,
                    Title = Helpers.ExternalDashboardTitleView("3"),
                    Section1RemarksList = section1RemarksList,
                    Section2RemarksList = section2RemarksList,
                    Section3RemarksList = section3RemarksList,
                    Section4RemarksList = section4RemarksList,
                    Section5RemarksList = section5RemarksList,
                    Section6RemarksList = section6RemarksList,
                    Section7RemarksList = section7RemarksList,
                    Section8RemarksList = section8RemarksList,
                    Section9RemarksList = section9RemarksList,
                };
                return View(dashboardview);
            }
            return View("Index");
        }

        /// <summary>
        /// Clinical Compliance Dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult ClinicalComplianceV1(int? type)
        {
            if (type != null && Convert.ToInt32(type) > 0)
            {
                var section1RemarksList = new List<DashboardRemarkCustomModel>();
                var section2RemarksList = new List<DashboardRemarkCustomModel>();
                var section3RemarksList = new List<DashboardRemarkCustomModel>();
                var section4RemarksList = new List<DashboardRemarkCustomModel>();
                var section5RemarksList = new List<DashboardRemarkCustomModel>();
                var section6RemarksList = new List<DashboardRemarkCustomModel>();
                var loggedinfacilityId = Helpers.GetDefaultFacilityId();
                //var _fService = new FacilityService();
                var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
                var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();
                var corporateid = Helpers.GetSysAdminCorporateID();

                var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid, Convert.ToInt32(type));
                if (allRemarksList != null && allRemarksList.Count > 0)
                {
                    section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("1")).ToList();
                    section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("2")).ToList();
                    section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("3")).ToList();
                    section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("4")).ToList();
                    section5RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("5")).ToList();
                    section6RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("6")).ToList();
                }

                var dashboardview = new ExecutiveDashboardView
                {
                    FacilityId = facilityid,
                    DashboardType = 4,
                    Title = Helpers.ExternalDashboardTitleView("10"),
                    Section1RemarksList = section1RemarksList,
                    Section2RemarksList = section2RemarksList,
                    Section3RemarksList = section3RemarksList,
                    Section4RemarksList = section4RemarksList,
                    Section5RemarksList = section5RemarksList,
                    Section6RemarksList = section6RemarksList,
                };
                return View(dashboardview);
            }
            return View("Index");
        }

        /// <summary>
        /// Kpis the dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult KPIDashboardV1()
        {
            var patinetFallList = new List<PatientFallStats>();
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
            var section11RemarksList = new List<DashboardRemarkCustomModel>();
            var section12RemarksList = new List<DashboardRemarkCustomModel>();
            var section13RemarksList = new List<DashboardRemarkCustomModel>();

            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            //var _fService = new FacilityService();

            var currentDateTime = _fService.GetInvariantCultureDateTime(loggedinfacilityId);
            var currentYear = currentDateTime.Year;


            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();
            patinetFallList = _dbService.GetPatientFallRateV1(facilityid, Helpers.GetSysAdminCorporateID(), "", currentYear, 0, 0, 0);
            var corporateid = Helpers.GetSysAdminCorporateID();
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                Convert.ToInt32(2));
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
                section11RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("11")).ToList();
                section12RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("12")).ToList();
                section13RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("13")).ToList();
            }
            var dashboardview = new KPISummaryDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 2,
                Title = Helpers.ExternalDashboardTitleView("2"),
                PatinetFallList = patinetFallList,
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
                Section11RemarksList = section11RemarksList,
                Section12RemarksList = section12RemarksList,
                Section13RemarksList = section13RemarksList
            };
            return View(dashboardview);
        }

        /// <summary>
        /// RCMs the dashboard v1.
        /// </summary>
        /// <returns></returns>
        public ActionResult RCMDashboardV1()
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
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            //var _fService = new FacilityService();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                Convert.ToInt32(6));

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
            }
            var dashboardview = new RCMDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 6,
                Title = Helpers.ExternalDashboardTitleView("5"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList,
                Section7RemarksList = section7RemarksList,
                Section8RemarksList = section8RemarksList,
                Section9RemarksList = section9RemarksList,
            };
            return View(dashboardview);
        }

        /// <summary>
        /// Financials the MGT dashboard v1.
        /// </summary>
        /// <returns></returns>
        public ActionResult FinancialMGTDashboardV1()
        {
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            //var _fService = new FacilityService();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var section5RemarksList = new List<DashboardRemarkCustomModel>();
            var section6RemarksList = new List<DashboardRemarkCustomModel>();
            var section7RemarksList = new List<DashboardRemarkCustomModel>();
            var section8RemarksList = new List<DashboardRemarkCustomModel>();
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                    Convert.ToInt32(5));

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
            }
            var dashboardview = new FinancialMGTDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 5,
                Title = Helpers.ExternalDashboardTitleView("4"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList,
                Section7RemarksList = section7RemarksList,
                Section8RemarksList = section8RemarksList,
            };
            return View(dashboardview);
        }

        /// <summary>
        /// Hrs the dashboard v1.
        /// </summary>
        /// <returns></returns>
        public ActionResult HRDashboardV1()
        {
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            //var _fService = new FacilityService();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var section5RemarksList = new List<DashboardRemarkCustomModel>();

            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                    Convert.ToInt32(8));
            if (allRemarksList != null && allRemarksList.Count > 0)
            {
                section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("1")).ToList();
                section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("2")).ToList();
                section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("3")).ToList();
                section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("4")).ToList();
                section5RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("5")).ToList();
            }
            var dashboardview = new HRDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 8,
                Title = Helpers.ExternalDashboardTitleView("7"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList
            };
            return View(dashboardview);
        }

        /// <summary>
        /// Cases the management dashboard v1.
        /// </summary>
        /// <returns></returns>
        public ActionResult CaseManagementDashboardV1()
        {
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            //var _fService = new FacilityService();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var section5RemarksList = new List<DashboardRemarkCustomModel>();

            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                    Convert.ToInt32(7));
            if (allRemarksList != null && allRemarksList.Count > 0)
            {
                section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("1")).ToList();
                section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("2")).ToList();
                section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("3")).ToList();
                section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("4")).ToList();
                section5RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("5")).ToList();
            }
            var dashboardview = new CaseMgtDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 7,
                Title = Helpers.ExternalDashboardTitleView("6"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList
            };
            return View(dashboardview);
        }

        /// <summary>
        /// Clinicals the graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult ClinicalGraphsDataV1(int facilityId, int month, int facilityType, int segment, int department)
        {
            var clinicalGrpahsArray = "172,177,176,166,166,167,168,169,170,171,174,175,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,1312,1313,1314,1311,1309,1310";
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateId,
                Convert.ToString(clinicalGrpahsArray),
                currentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var hamList = manualDashboardData.Where(x => x.Indicators == 172).ToList();
            var fallRiskList = manualDashboardData.Where(x => x.Indicators == 177).ToList();
            var painManagementList = manualDashboardData.Where(x => x.Indicators == 176).ToList();
            var dischargeToComunityrate = manualDashboardData.Where(x => x.Indicators == 166).ToList();

            var lostReferals = manualDashboardData.Where(x => x.Indicators == 167).ToList();
            var transferOutRate = manualDashboardData.Where(x => x.Indicators == 168).ToList();
            var nursingStaffCompetency = manualDashboardData.Where(x => x.Indicators == 169).ToList();
            var nursingDepartmentOrientation = manualDashboardData.Where(x => x.Indicators == 170).ToList();

            var patientIdentification = manualDashboardData.Where(x => x.Indicators == 171).ToList();
            var patientFallRatewithInjury = manualDashboardData.Where(x => x.Indicators == 174).ToList();
            var pressureUlcerIncidentRate = manualDashboardData.Where(x => x.Indicators == 175).ToList();
            var fallRiskAssessmentProtocolComplianceRate = manualDashboardData.Where(x => x.Indicators == 177).ToList();

            var compliancetoPressureUlcerPreventionProtocol = manualDashboardData.Where(x => x.Indicators == 178).ToList();
            var sbarProtocolComplianceRate = manualDashboardData.Where(x => x.Indicators == 179).ToList();
            var mdroRate = manualDashboardData.Where(x => x.Indicators == 180).ToList();
            var mrsaRate = manualDashboardData.Where(x => x.Indicators == 181).ToList();

            var esblRate = manualDashboardData.Where(x => x.Indicators == 182).ToList();
            var lrtiRate = manualDashboardData.Where(x => x.Indicators == 183).ToList();
            var cautiRate = manualDashboardData.Where(x => x.Indicators == 184).ToList();
            var bsiRate = manualDashboardData.Where(x => x.Indicators == 185).ToList();

            var handHygineComplainceRate = manualDashboardData.Where(x => x.Indicators == 186).ToList();
            var staffVaccinationRate = manualDashboardData.Where(x => x.Indicators == 187).ToList();
            var inappropriateAntiBioticUsageRate = manualDashboardData.Where(x => x.Indicators == 188).ToList();
            var therapyInitialAssessmentProtocolCompliance = manualDashboardData.Where(x => x.Indicators == 189).ToList();

            var manualHandlingRiskAssessmentProtocolCompliance = manualDashboardData.Where(x => x.Indicators == 190).ToList();
            var standardizedOutcomeMeasureProtocol = manualDashboardData.Where(x => x.Indicators == 191).ToList();
            var Incidents = manualDashboardData.Where(x => x.Indicators == 192).ToList();
            var nonMedicationRelatedIncidents = manualDashboardData.Where(x => x.Indicators == 1312 && x.Year == currentYear).ToList();
            var typeOfIncidents = manualDashboardData.Where(x => x.Indicators == 1313 && x.Year == currentYear).ToList();
            //var categoryofIncidents = manualDashboardData.Where(x => x.Indicators == 1314).ToList();  //categoryofIncidents
            var medicationErrors = manualDashboardData.Where(x => x.Indicators == 1311 && x.Year == currentYear).ToList();  //categoryofIncidents

            var customDataToreturn = new List<ManualDashboardCustomModel>();
            var manualDashboardData1 = manualDashboardData.Where(x => x.Indicators == 1309 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData2 = manualDashboardData.Where(x => x.Indicators == 1310 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData3 = manualDashboardData.Where(x => x.Indicators == 1314 && x.ExternalValue3 == "1").ToList();
            customDataToreturn.Add(manualDashboardData1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            customDataToreturn.Add(manualDashboardData3.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            customDataToreturn.Add(manualDashboardData2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            var categoryofIncidents = customDataToreturn;

            var jsonResult = new
            {
                hamList,
                fallRiskList,
                painManagementList,
                dischargeToComunityrate,
                lostReferals,
                transferOutRate,
                nursingStaffCompetency,
                nursingDepartmentOrientation,
                patientIdentification,
                patientFallRatewithInjury,
                pressureUlcerIncidentRate,
                fallRiskAssessmentProtocolComplianceRate,
                compliancetoPressureUlcerPreventionProtocol,
                sbarProtocolComplianceRate,
                mdroRate,
                mrsaRate,
                esblRate,
                lrtiRate,
                cautiRate,
                bsiRate,
                handHygineComplainceRate,
                staffVaccinationRate,
                inappropriateAntiBioticUsageRate,
                therapyInitialAssessmentProtocolCompliance,
                manualHandlingRiskAssessmentProtocolCompliance,
                standardizedOutcomeMeasureProtocol,
                Incidents,
                nonMedicationRelatedIncidents,
                typeOfIncidents,
                categoryofIncidents,
                medicationErrors
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// RCMs the graphs data v1.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult RCMGraphsDataV1(int facilityId, int month, int facilityType, int segment, int department)
        {
            var RCMDashboardData = "1308,221,222,224,144,101,102,225,226,227,228,161,242,229,230,231,232,159,270,131";

            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var customDate = month == 0
                ? currentDateTime.ToShortDateString()
                : Convert.ToDateTime(month + "/" + month + "/" + currentYear).ToShortDateString();
            var manualDashboardList = _dbService.GetManualDashBoardStatData(facilityId,
                Helpers.GetSysAdminCorporateID(), "270", customDate, facilityType, segment, department);

            var corporateId = Helpers.GetSysAdminCorporateID();
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateId,
                Convert.ToString(RCMDashboardData),
                currentYear, facilityType, segment, department);

            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var ipReveune = manualDashboardData.Where(x => x.Indicators == 1308).ToList();
            var reveunePPD = manualDashboardData.Where(x => x.Indicators == 221).ToList();
            var revenueIP = manualDashboardData.Where(x => x.Indicators == 222).ToList();

            var revenueDaman = revenueIP != null && revenueIP.Any()
                ? revenueIP.Where(x => x.SubCategory1 == "2").ToList()
                : revenueIP;
            var reveuneRoyalFamily = revenueIP != null && revenueIP.Any()
                ? revenueIP.Where(x => x.SubCategory1 == "31").ToList()
                : revenueIP; //Revenue Royal Family

            var reveuneInpatinetOther = manualDashboardData.Where(x => x.Indicators == 224).ToList();

            var averageDailyCencus = manualDashboardData.Where(x => x.Indicators == 144).ToList();
            var newAdmissions = manualDashboardData.Where(x => x.Indicators == 101).ToList();
            var plannedDischarges = manualDashboardData.Where(x => x.Indicators == 102).ToList();
            var dischargesUnplanned = manualDashboardData.Where(x => x.Indicators == 225).ToList();

            var acuteOutsPatients = manualDashboardData.Where(x => x.Indicators == 226).ToList();
            var acuteOutsDays = manualDashboardData.Where(x => x.Indicators == 227).ToList();
            var therapeuticLeaves = manualDashboardData.Where(x => x.Indicators == 228).ToList();
            var opRevenue = manualDashboardData.Where(x => x.Indicators == 161).ToList();
            var patientDaysList = manualDashboardData.Where(x => x.Indicators == 242).ToList();
            var patientDays = GetPatientDaysAll(patientDaysList);
            var serviceCodeDistributionbyBilledClaims = manualDashboardData.Where(x => x.Indicators == 229).ToList();
            var percentagebilledReveune = manualDashboardData.Where(x => x.Indicators == 230).ToList();
            var accountSubmittedClaims = manualDashboardData.Where(x => x.Indicators == 231).ToList();
            var numberSubmittedClaims = manualDashboardData.Where(x => x.Indicators == 232).ToList();

            var payorMix = GetSubCategoryChartsPayorMix(facilityId, month, facilityType, segment, department, "159");//payor Mix
            var claimsResubmissionPercentage = manualDashboardList.Where(x => x.IndicatorNumber == "270").ToList();
            var arDays = manualDashboardData.Where(x => x.Indicators == 131).ToList(); //GetGraphsData(facilityId, month, facilityType, segment, department, ""); //manualDashboardList.Where(x => x.IndicatorNumber == "131").ToList(); 

            var jsonResult = new
            {
                ipReveune,
                reveunePPD,
                revenueDaman,
                reveuneRoyalFamily,
                reveuneInpatinetOther,
                averageDailyCencus,
                arDays,
                newAdmissions,
                plannedDischarges,
                dischargesUnplanned,
                acuteOutsPatients,
                acuteOutsDays,
                therapeuticLeaves,
                opRevenue,
                patientDays,
                serviceCodeDistributionbyBilledClaims,
                percentagebilledReveune,
                accountSubmittedClaims,
                numberSubmittedClaims,
                payorMix,
                claimsResubmissionPercentage
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cases the management graphs data v1.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult CaseManagementGraphsDataV1(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var corporateid = Helpers.GetSysAdminCorporateID();
            var caseManagementIndicators = "227,233,234,235,236,237,238,239,240,255";
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateid,
                Convert.ToString(caseManagementIndicators), currentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var acuteOutDays = manualDashboardData.Where(x => x.Indicators == 227).ToList();
            var acuteOut = manualDashboardData.Where(x => x.Indicators == 233).ToList();
            var therapueticLeaves = manualDashboardData.Where(x => x.Indicators == 234).ToList();
            var presenceInitialAssessment = manualDashboardData.Where(x => x.Indicators == 235).ToList();
            var presenceMDTdocumentation = manualDashboardData.Where(x => x.Indicators == 236).ToList();
            var presenceDischargedDocumentation = manualDashboardData.Where(x => x.Indicators == 237).ToList();
            var dischargeDisposition = manualDashboardData.Where(x => x.Indicators == 238).ToList();
            var numberUnplannedDischarges = manualDashboardData.Where(x => x.Indicators == 239).ToList();
            var postDischargeFollowup = manualDashboardData.Where(x => x.Indicators == 240).ToList();
            var lostReferral = manualDashboardData.Where(x => x.Indicators == 255).ToList();
            var jsonResult = new
            {
                acuteOutDays,
                acuteOut,
                therapueticLeaves,
                presenceInitialAssessment,
                presenceMDTdocumentation,
                presenceDischargedDocumentation,
                dischargeDisposition,
                numberUnplannedDischarges,
                postDischargeFollowup,
                lostReferral
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the financial MGT graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult GetFinancialMGTGraphsDataV1(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var corporateid = Helpers.GetSysAdminCorporateID();
            var financialMgtIndicators = "110,111,112,113,155,115,117,118,125,126,127,129,130,240,243,244,245,255";
            var manualDashboardData = _dbService.GetManualDashBoardV1(facilityId, corporateid,
                Convert.ToString(financialMgtIndicators),
                currentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var netRevenue = manualDashboardData.Where(x => x.Indicators == 110).ToList();
            var swbDirect = manualDashboardData.Where(x => x.Indicators == 111).ToList();
            var supplies = manualDashboardData.Where(x => x.Indicators == 112).ToList();
            var otherDirect = manualDashboardData.Where(x => x.Indicators == 113).ToList();
            var swbinDirect = manualDashboardData.Where(x => x.Indicators == 155).ToList();
            var otherIndirectCosts = manualDashboardData.Where(x => x.Indicators == 115).ToList();
            var deprAndAmort = manualDashboardData.Where(x => x.Indicators == 117).ToList();
            var interest = manualDashboardData.Where(x => x.Indicators == 118).ToList();
            var changesWorkingCapital = manualDashboardData.Where(x => x.Indicators == 125).ToList();
            var otherAdjustments = manualDashboardData.Where(x => x.Indicators == 126).ToList();
            var capitalExpenditures = manualDashboardData.Where(x => x.Indicators == 127).ToList();
            var cashInBank = manualDashboardData.Where(x => x.Indicators == 129).ToList();
            var daysTotalExpendituresInCash = manualDashboardData.Where(x => x.Indicators == 130).ToList();
            var totalCashCollected = manualDashboardData.Where(x => x.Indicators == 243).ToList();
            var totalPayablesSalaries = manualDashboardData.Where(x => x.Indicators == 244).ToList();
            var totalCapitalPaymentsMade = manualDashboardData.Where(x => x.Indicators == 245).ToList();

            var jsonResult = new
            {
                netRevenue,
                swbDirect,
                supplies,
                otherDirect,
                swbinDirect,
                otherIndirectCosts,
                deprAndAmort,
                interest,
                changesWorkingCapital,
                otherAdjustments,
                capitalExpenditures,
                cashInBank,
                daysTotalExpendituresInCash,
                totalCashCollected,
                totalPayablesSalaries,
                totalCapitalPaymentsMade
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the hr graphs data v1.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult GetHRGraphsDataV1(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var corporateid = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var hrGraphsIndicators = "254,246,247,248,211,250,251,252,253,277";
            var manualDashboardData = _dbService.GetManualDashBoardV1(facilityId, corporateid,
                Convert.ToString(hrGraphsIndicators),
                currentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var FTEs = manualDashboardData.Where(x => x.Indicators == 254).ToList();
            var headcount = manualDashboardData.Where(x => x.Indicators == 246).ToList();
            var overtimeHours = manualDashboardData.Where(x => x.Indicators == 247).ToList();
            var attritionRate = manualDashboardData.Where(x => x.Indicators == 248).ToList();
            var employeeEngagementScore = manualDashboardData.Where(x => x.Indicators == 211).ToList();
            var administrationTotalStaff = manualDashboardData.Where(x => x.Indicators == 250).ToList();
            var timetakenToRecruitVacantPos = manualDashboardData.Where(x => x.Indicators == 251).ToList();
            var productiveHours = manualDashboardData.Where(x => x.Indicators == 252).ToList();
            var unproductiveHours = manualDashboardData.Where(x => x.Indicators == 253).ToList();
            var nursingHoursPPD = manualDashboardData.Where(x => x.Indicators == 277).ToList();
            var jsonResult = new
            {
                FTEs,
                headcount,
                overtimeHours,
                attritionRate,
                employeeEngagementScore,
                administrationTotalStaff,
                timetakenToRecruitVacantPos,
                productiveHours,
                unproductiveHours,
                nursingHoursPPD
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the kpi graphs data v1.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult GetKPIGraphsDataV1(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var corporateid = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            var hrGraphsIndicators = "254,246,172,174,247,248,255,236,237,233,234,1312,1313,1311,1314,1309,1310,227";
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateid,
                Convert.ToString(hrGraphsIndicators),
                currentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            var FTEs = manualDashboardData.Where(x => x.Indicators == 254).ToList();
            var headcount = manualDashboardData.Where(x => x.Indicators == 246).ToList();
            var highAlertMedicationChart = manualDashboardData.Where(x => x.Indicators == 172).ToList();
            var patientFallRateGraph = manualDashboardData.Where(x => x.Indicators == 174).ToList();
            var overtimeHours = manualDashboardData.Where(x => x.Indicators == 247).ToList();
            var attritionRate = manualDashboardData.Where(x => x.Indicators == 248).ToList();
            var lostReferral = manualDashboardData.Where(x => x.Indicators == 255).ToList();
            var presenceMDTdocumentation = manualDashboardData.Where(x => x.Indicators == 236).ToList();
            var acuteOutDays = manualDashboardData.Where(x => x.Indicators == 227).ToList();
            var acuteOut = manualDashboardData.Where(x => x.Indicators == 233).ToList();
            var therapueticLeaves = manualDashboardData.Where(x => x.Indicators == 234).ToList();
            var nonMedicationRelatedIncidents = manualDashboardData.Where(x => x.Indicators == 1312 && x.Year == currentYear && x.BudgetType == 2).ToList();
            var typeOfIncidents = manualDashboardData.Where(x => x.Indicators == 1313 && x.Year == currentYear).ToList();

            var customDataToreturn = new List<ManualDashboardCustomModel>();
            var manualDashboardData1 = manualDashboardData.Where(x => x.Indicators == 1309 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData2 = manualDashboardData.Where(x => x.Indicators == 1310 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData3 = manualDashboardData.Where(x => x.Indicators == 1314 && x.ExternalValue3 == "1").ToList();
            customDataToreturn.Add(manualDashboardData1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            customDataToreturn.Add(manualDashboardData3.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            customDataToreturn.Add(manualDashboardData2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            var categoryofIncidents = customDataToreturn;

            var medicationErrors = manualDashboardData.Where(x => x.Indicators == 1311 && x.Year == currentYear).ToList();  //categoryofIncidents

            var jsonResult = new
            {
                FTEs,
                headcount,
                highAlertMedicationChart,
                patientFallRateGraph,
                overtimeHours,
                attritionRate,
                lostReferral,
                presenceMDTdocumentation,
                acuteOutDays,
                acuteOut,
                therapueticLeaves,
                nonMedicationRelatedIncidents,
                typeOfIncidents,
                categoryofIncidents,
                medicationErrors
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportToExcel(int? type, int? month, int? facilityId)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(Convert.ToInt32(facilityId));
            var workbook = new HSSFWorkbook();
            #region Excel Creation and Formatting
            var sheet = workbook.CreateSheet("ExternalDashboardData");
            sheet.CreateFreezePane(0, 2, 0, 2);
            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            var font = workbook.CreateFont();
            var format = workbook.CreateDataFormat();
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.Color = IndexedColors.Black.Index;
            style.SetFont(font);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 8, 15));
            sheet.SetColumnWidth(0, 2500);
            sheet.SetColumnWidth(1, 2500);
            sheet.SetColumnWidth(2, 2500);
            sheet.SetColumnWidth(3, 2500);
            sheet.SetColumnWidth(4, 2500);
            sheet.SetColumnWidth(5, 2500);
            sheet.SetColumnWidth(6, 2500);
            sheet.SetColumnWidth(7, 10000);
            sheet.SetColumnWidth(8, 2500);
            sheet.SetColumnWidth(9, 2500);
            sheet.SetColumnWidth(10, 2500);
            sheet.SetColumnWidth(11, 2500);
            sheet.SetColumnWidth(12, 2500);
            sheet.SetColumnWidth(13, 2500);
            sheet.SetColumnWidth(14, 2500);
            var columnpercentagestyle = workbook.CreateCellStyle();
            var columnstyle = workbook.CreateCellStyle();
            var formatIdfromatter = HSSFDataFormat.GetBuiltinFormat("#,##0");
            columnstyle.DataFormat = formatIdfromatter == -1 ? format.GetFormat("#,##0") : formatIdfromatter;
            var formatIdpercentage = HSSFDataFormat.GetBuiltinFormat("0.0%");
            columnpercentagestyle.DataFormat = formatIdpercentage == -1 ? format.GetFormat("0.0%") : formatIdpercentage;
            sheet.SetDefaultColumnStyle(0, columnstyle);
            sheet.SetDefaultColumnStyle(1, columnstyle);
            sheet.SetDefaultColumnStyle(2, columnstyle);
            sheet.SetDefaultColumnStyle(3, columnpercentagestyle);
            sheet.SetDefaultColumnStyle(4, columnstyle);
            sheet.SetDefaultColumnStyle(5, columnstyle);
            sheet.SetDefaultColumnStyle(6, columnpercentagestyle);
            sheet.SetDefaultColumnStyle(7, style);
            sheet.SetDefaultColumnStyle(8, columnstyle);
            sheet.SetDefaultColumnStyle(9, columnstyle);
            sheet.SetDefaultColumnStyle(10, columnstyle);
            sheet.SetDefaultColumnStyle(11, columnpercentagestyle);
            sheet.SetDefaultColumnStyle(12, columnstyle);
            sheet.SetDefaultColumnStyle(13, columnstyle);
            sheet.SetDefaultColumnStyle(14, columnpercentagestyle);
            // Add header labels
            var rowIndex = 0;
            #region Header Row 1
            var row1 = sheet.CreateRow(rowIndex);
            row1.CreateCell(0).SetCellValue("Current Month");
            row1.GetCell(0).CellStyle = style;
            row1.CreateCell(1).SetCellValue("");
            row1.CreateCell(2).SetCellValue("");
            row1.CreateCell(3).SetCellValue("");
            row1.CreateCell(4).SetCellValue("");
            row1.CreateCell(5).SetCellValue("");
            row1.CreateCell(6).SetCellValue("");
            row1.CreateCell(7).SetCellValue("");
            row1.CreateCell(8).SetCellValue("Current Year");
            row1.GetCell(8).CellStyle = style;
            row1.CreateCell(9).SetCellValue("");
            row1.CreateCell(10).SetCellValue("");
            row1.CreateCell(11).SetCellValue("");
            row1.CreateCell(12).SetCellValue("");
            row1.CreateCell(13).SetCellValue("");
            row1.CreateCell(14).SetCellValue("");
            #endregion

            #region Header row 2
            rowIndex++;
            var row = sheet.CreateRow(rowIndex);
            var style1 = workbook.CreateCellStyle();
            style1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
            style1.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Blue.Index;
            style1.FillPattern = FillPattern.SolidForeground;
            style1.BorderTop = BorderStyle.Medium;
            style1.BorderBottom = BorderStyle.Medium;
            style1.BorderLeft = BorderStyle.Medium;
            style1.BorderRight = BorderStyle.Medium;
            style1.SetFont(font);
            row.CreateCell(0).SetCellValue("Actual");
            row.GetCell(0).CellStyle = style1;
            row.CreateCell(1).SetCellValue("Budget");
            row.GetCell(1).CellStyle = style1;
            row.CreateCell(2).SetCellValue("Variance");
            row.GetCell(2).CellStyle = style1;
            row.CreateCell(3).SetCellValue("% Var");
            row.GetCell(3).CellStyle = style1;
            row.CreateCell(4).SetCellValue("Prior Year (PY)");
            row.GetCell(4).CellStyle = style1;
            row.CreateCell(5).SetCellValue("PY Variance");
            row.GetCell(5).CellStyle = style1;
            row.CreateCell(6).SetCellValue("PY % Var");
            row.GetCell(6).CellStyle = style1;
            row.CreateCell(7).SetCellValue("Description");
            row.GetCell(7).CellStyle = style1;
            row.CreateCell(8).SetCellValue("Actual");
            row.GetCell(8).CellStyle = style1;
            row.CreateCell(9).SetCellValue("Budget");
            row.GetCell(9).CellStyle = style1;
            row.CreateCell(10).SetCellValue("Variance");
            row.GetCell(10).CellStyle = style1;
            row.CreateCell(11).SetCellValue("% Var");
            row.GetCell(11).CellStyle = style1;
            row.CreateCell(12).SetCellValue("Prior Year (PY)");
            row.GetCell(12).CellStyle = style1;
            row.CreateCell(13).SetCellValue("PY Variance");
            row.GetCell(13).CellStyle = style1;
            row.CreateCell(14).SetCellValue("PY % Var");
            row.GetCell(14).CellStyle = style1;
            rowIndex++;
            #endregion
            #endregion
            #region Data Rows
            var facilityid = facilityId == null
                ? Helpers.GetFacilityIdNextDefaultCororateFacility()
                : facilityId == 0 ? 9999 : facilityId;
            var corporateid = Helpers.GetSysAdminCorporateID();
            //var currentDateTime = _eService.GetInvariantCultureDateTime();
            var currentYear = currentDateTime.Year;

            var indicatorNumbers = string.Empty;
            var sectionVolume5ListOrders = ExecutiveDashboardListSortOrder();
            var section10ListOrder = ExecutiveDashboardSection10ListSortOrder();
            if (type == 1)
            {
                var enumRessult = Helpers.GetEnumList<ExecutiveDashboardSection1Stat>();
                var resultList = enumRessult.Where(x => Enum.IsDefined(typeof(ExecutiveDashboardSection1Stat), x))
                    .Select(x => (int)Enum.Parse(typeof(ExecutiveDashboardSection1Stat), x, true)).ToList();
                indicatorNumbers = (String.Join(",", resultList));
            }
            else if (type == 2)
            {
                var enumRessult = Helpers.GetEnumList<ExecutiveDashboardSection5Stat>();
                var resultList = new List<string>(sectionVolume5ListOrders.Keys);
                indicatorNumbers = (String.Join(",", resultList));
            }
            else if (type == 3)
            {
                var enumRessult = Helpers.GetEnumList<ExecutiveDashboardSection10Stat>();
                var resultList = new List<string>(section10ListOrder.Keys);
                indicatorNumbers = (String.Join(",", resultList));
            }

            var customDate = month == null
                ? currentDateTime.ToShortDateString()
                : (Convert.ToDateTime(month + "/" + month + "/" + currentYear)).ToShortDateString();

            var manualDashboardList = _dbService.GetManualDashBoardStatData(Convert.ToInt32(facilityid),
                corporateid, indicatorNumbers, customDate, 0, 0, 0);
            manualDashboardList = type == 2
                ? manualDashboardList.OrderBy(x => sectionVolume5ListOrders[x.IndicatorNumber.Trim()]).ToList()
                : type == 3
                    ? manualDashboardList.OrderBy(x => section10ListOrder[x.IndicatorNumber.Trim()]).ToList()
                    : manualDashboardList;
            //----------------- Data Formattter


            var cellGreenStyle = workbook.CreateCellStyle();
            cellGreenStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
            cellGreenStyle.FillPattern = FillPattern.SolidForeground;
            cellGreenStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

            var cellRedStyle = workbook.CreateCellStyle();
            cellRedStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            cellRedStyle.FillPattern = FillPattern.SolidForeground;
            cellRedStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

            var cellYellowStyle = workbook.CreateCellStyle();
            cellYellowStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
            cellYellowStyle.FillPattern = FillPattern.SolidForeground;
            cellYellowStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

            var cellGreenStylepercent = workbook.CreateCellStyle();
            cellGreenStylepercent.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
            cellGreenStylepercent.FillPattern = FillPattern.SolidForeground;
            cellGreenStylepercent.DataFormat = formatIdpercentage == -1 ? format.GetFormat("0.0%") : formatIdpercentage;
            cellGreenStylepercent.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

            var cellRedStylepercent = workbook.CreateCellStyle();
            cellRedStylepercent.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            cellRedStylepercent.FillPattern = FillPattern.SolidForeground;
            cellRedStylepercent.DataFormat = formatIdpercentage == -1 ? format.GetFormat("0.0%") : formatIdpercentage;
            cellRedStylepercent.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

            var cellYellowStylepercent = workbook.CreateCellStyle();
            cellYellowStylepercent.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
            cellYellowStylepercent.FillPattern = FillPattern.SolidForeground;
            cellYellowStylepercent.DataFormat = formatIdpercentage == -1 ? format.GetFormat("0.0%") : formatIdpercentage;
            cellYellowStylepercent.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

            #region Data Formmatter

            var cellStyle = workbook.CreateCellStyle();
            var cellStylepercentage = workbook.CreateCellStyle();
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0");
            var formatId = HSSFDataFormat.GetBuiltinFormat("0.00%");
            cellStylepercentage.DataFormat = formatId == -1 ? format.GetFormat("0.00%") : formatId;

            #endregion
            //--------Data formatter ends

            foreach (var item in manualDashboardList)
            {
                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellType(CellType.Numeric);
                if (item.FormatType == 1)
                {
                    row.GetCell(0).CellStyle = cellStyle;
                    row.GetCell(0).SetCellValue(Convert.ToDouble(item.CMA));
                }
                else
                {
                    row.GetCell(0).CellStyle = cellStylepercentage;
                    row.GetCell(0).SetCellValue(Convert.ToDouble(item.CMA) / 100);
                }
                row.CreateCell(1).SetCellType(CellType.Numeric);
                if (item.FormatType == 1)
                {
                    row.GetCell(1).CellStyle = cellStyle;
                    row.GetCell(1).SetCellValue(Convert.ToDouble(item.CMB));
                }
                else
                {
                    row.GetCell(1).CellStyle = cellStylepercentage;
                    row.GetCell(1).SetCellValue(Convert.ToDouble(item.CMB) / 100);
                }
                row.CreateCell(2).SetCellType(CellType.Numeric);
                row.CreateCell(2).CellStyle = cellStyle;
                //switch (item.CMVarColor)
                //{
                //    case "1":
                //        row.GetCell(2).CellStyle = cellGreenStyle;
                //        break;
                //    case "2":
                //        row.GetCell(2).CellStyle = cellYellowStyle;
                //        break;
                //    case "3":
                //        row.GetCell(2).CellStyle = cellRedStyle;
                //        break;
                //}
                row.GetCell(2).SetCellValue(Convert.ToDouble(item.CMVar));
                row.CreateCell(3).SetCellType(CellType.Numeric);
                row.GetCell(3).CellStyle = cellStylepercentage;
                //switch (item.CMVarPercentColor)
                //{
                //    case "1":
                //        row.GetCell(3).CellStyle = cellGreenStylepercent;
                //        break;
                //    case "2":
                //        row.GetCell(3).CellStyle = cellYellowStylepercent;
                //        break;
                //    case "3":
                //        row.GetCell(3).CellStyle = cellRedStylepercent;
                //        break;
                //}
                row.GetCell(3).SetCellValue(Convert.ToDouble(item.CMVarPercentage));
                row.CreateCell(4).SetCellType(CellType.Numeric);
                if (item.FormatType == 1)
                {
                    row.GetCell(4).CellStyle = cellStyle;
                    row.GetCell(4).SetCellValue(Convert.ToDouble(item.PMA));
                }
                else
                {
                    row.GetCell(4).CellStyle = cellStylepercentage;
                    row.GetCell(4).SetCellValue(Convert.ToDouble(item.PMA) / 100);
                }
                row.CreateCell(5).SetCellType(CellType.Numeric);
                row.GetCell(5).CellStyle = cellStyle;
                //switch (item.PMAPercentColor)
                //{
                //    case "1":
                //        row.GetCell(5).CellStyle = cellGreenStyle;
                //        break;
                //    case "2":
                //        row.GetCell(5).CellStyle = cellYellowStyle;
                //        break;
                //    case "3":
                //        row.GetCell(5).CellStyle = cellRedStyle;
                //        break;
                //}
                row.GetCell(5).SetCellValue(Convert.ToDouble(item.PMVar));
                row.CreateCell(6).SetCellType(CellType.Numeric);
                row.GetCell(6).CellStyle = cellStylepercentage;
                //switch (item.PMAPercentColor)
                //{
                //    case "1":
                //        row.GetCell(6).CellStyle = cellGreenStylepercent;
                //        break;
                //    case "2":
                //        row.GetCell(6).CellStyle = cellYellowStylepercent;
                //        break;
                //    case "3":
                //        row.GetCell(6).CellStyle = cellRedStylepercent;
                //        break;
                //}
                row.GetCell(6).SetCellValue(Convert.ToDouble(item.PMVarPercentage));
                row.CreateCell(7).SetCellType(CellType.String);
                row.GetCell(7).SetCellValue((item.DashBoard));
                row.CreateCell(8).SetCellType(CellType.Numeric);
                if (item.FormatType == 1)
                {
                    row.GetCell(8).CellStyle = cellStyle;
                    row.GetCell(8).SetCellValue(Convert.ToDouble(item.CYTA));
                }
                else
                {
                    row.GetCell(8).CellStyle = cellStylepercentage;
                    row.GetCell(8).SetCellValue(Convert.ToDouble(item.CYTA) / 100);
                }
                row.CreateCell(9).SetCellType(CellType.Numeric);
                if (item.FormatType == 1)
                {
                    row.GetCell(9).CellStyle = cellStyle;
                    row.GetCell(9).SetCellValue(Convert.ToDouble(item.CYTB));
                }
                else
                {
                    row.GetCell(9).CellStyle = cellStylepercentage;
                    row.GetCell(9).SetCellValue(Convert.ToDouble(item.CYTB) / 100);
                }
                row.CreateCell(10).SetCellType(CellType.Numeric);
                row.GetCell(10).CellStyle = cellStyle;
                //switch (item.CYTAVarColor)
                //{
                //    case "1":
                //        row.GetCell(10).CellStyle = cellGreenStyle;
                //        break;
                //    case "2":
                //        row.GetCell(10).CellStyle = cellYellowStyle;
                //        break;
                //    case "3":
                //        row.GetCell(10).CellStyle = cellRedStyle;
                //        break;
                //}
                row.GetCell(10).SetCellValue(Convert.ToDouble(item.CYTBVar));
                row.CreateCell(11).SetCellType(CellType.Numeric);
                row.GetCell(11).CellStyle = cellStylepercentage;
                //switch (item.CYTBPercentColor)
                //{
                //    case "1":
                //        row.GetCell(11).CellStyle = cellGreenStylepercent;
                //        break;
                //    case "2":
                //        row.GetCell(11).CellStyle = cellYellowStylepercent;
                //        break;
                //    case "3":
                //        row.GetCell(11).CellStyle = cellRedStylepercent;
                //        break;
                //}
                row.GetCell(11).SetCellValue(Convert.ToDouble(item.CYTBVarPercentage));
                row.CreateCell(12).SetCellType(CellType.Numeric);
                if (item.FormatType == 1)
                {
                    row.GetCell(12).CellStyle = cellStyle;
                    row.GetCell(12).SetCellValue(Convert.ToDouble(item.PYTA));
                }
                else
                {
                    row.GetCell(12).CellStyle = cellStylepercentage;
                    row.GetCell(12).SetCellValue(Convert.ToDouble(item.PYTA) / 100);
                }
                row.CreateCell(13).SetCellType(CellType.Numeric);
                row.GetCell(13).CellStyle = cellStyle;
                //switch (item.PYTAColor)
                //{
                //    case "1":
                //        row.GetCell(13).CellStyle = cellGreenStyle;
                //        break;
                //    case "2":
                //        row.GetCell(13).CellStyle = cellYellowStyle;
                //        break;
                //    case "3":
                //        row.GetCell(13).CellStyle = cellRedStyle;
                //        break;
                //}
                row.GetCell(13).SetCellValue(Convert.ToInt32(item.PYTBVar));
                row.CreateCell(14).SetCellType(CellType.Numeric);
                row.GetCell(14).CellStyle = cellStylepercentage;
                //switch (item.PYTAPercentColor)
                //{
                //    case "1":
                //        row.GetCell(14).CellStyle = cellGreenStylepercent;
                //        break;
                //    case "2":
                //        row.GetCell(14).CellStyle = cellYellowStylepercent;
                //        break;
                //    case "3":
                //        row.GetCell(14).CellStyle = cellRedStylepercent;
                //        break;
                //}
                row.GetCell(14).SetCellValue(Convert.ToInt32(item.PYTBVarPercentage));
                rowIndex++;
            }
            #endregion

            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                Response.Cookies.Add(cookie);
                workbook.Write(exportData);
                var saveAsFileName = type == 1 ? string.Format("ExecutiveDashboard_VolumeData-{0:d}.xls", currentDateTime).Replace("/", "-") :
                    type == 2 ? string.Format("ExecutiveDashboard_IncomeData-{0:d}.xls", currentDateTime).Replace("/", "-") :
                        type == 3 ? string.Format("ExecutiveDashboard_CashFlowData-{0:d}.xls", currentDateTime).Replace("/", "-") :
                            string.Format("ExecutiveDashboardData-{0:d}.xls", currentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }


        /// <summary>
        /// Clinicals the compliance graphs data v1.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult ClinicalComplianceGraphsDataV1(int facilityId, int month, int facilityType, int segment, int department)
        {
            const string ClinicalComplianceGrpahsArray = "720,721,722,723";
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;


            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateId,
                Convert.ToString(ClinicalComplianceGrpahsArray), currentYear, facilityType, segment, department);
            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();

            var conversionRate = manualDashboardData.Where(x => x.Indicators == 720).ToList();
            var patientinFunnel = manualDashboardData.Where(x => x.Indicators == 721).ToList();
            var timefromFunneltoBed = manualDashboardData.Where(x => x.Indicators == 722).ToList();
            var lostfromFunnel = manualDashboardData.Where(x => x.Indicators == 723).ToList();


            var jsonResult = new
            {
                conversionRate,
                patientinFunnel,
                timefromFunneltoBed,
                lostfromFunnel
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Clinicals the graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult ClinicalGraphsData_New(int facilityId, int month, int facilityType, int segment, int department)
        {
            // var clinicalGrpahsArray = "172,177,176,166,166,167,168,169,170,171,174,175,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,1312,1313,1314,1311,1309,1310";
            const string clinicalGrpahsArray = "750,174,830,832,758,756,180,181,186,175,752,754,188,189,190,191,192,1312,1313,1314,1311,1309,1310";
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;
            var manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateId,
                Convert.ToString(clinicalGrpahsArray),
                currentYear, facilityType, segment, department);

            if (manualDashboardData.Any())
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();

            var totalsentinelevents = manualDashboardData.Where(x => x.Indicators == 750).ToList();
            var patienFallRate = manualDashboardData.Where(x => x.Indicators == 174).ToList();
            var totalNearmiss = manualDashboardData.Where(x => x.Indicators == 830).ToList();
            var totaladverseincidents = manualDashboardData.Where(x => x.Indicators == 832).ToList();

            var TotalMedicationErrors = manualDashboardData.Where(x => x.Indicators == 758).ToList();
            var totalIncidentsReports = manualDashboardData.Where(x => x.Indicators == 756).ToList();
            var mdroRate = manualDashboardData.Where(x => x.Indicators == 180).ToList();
            var mrsaRate = manualDashboardData.Where(x => x.Indicators == 181).ToList();

            var handHygieneCompliance = manualDashboardData.Where(x => x.Indicators == 186).ToList();

            var pressureUlcerIncidentRate = manualDashboardData.Where(x => x.Indicators == 175).ToList();
            var averageFIMScorePAR = manualDashboardData.Where(x => x.Indicators == 752).ToList();
            var averageFIMScoreLTC = manualDashboardData.Where(x => x.Indicators == 754).ToList();

            var inappropriateAntiBioticUsageRate = manualDashboardData.Where(x => x.Indicators == 188).ToList();
            var therapyInitialAssessmentProtocolCompliance = manualDashboardData.Where(x => x.Indicators == 189).ToList();

            var manualHandlingRiskAssessmentProtocolCompliance = manualDashboardData.Where(x => x.Indicators == 190).ToList();
            var standardizedOutcomeMeasureProtocol = manualDashboardData.Where(x => x.Indicators == 191).ToList();
            var Incidents = manualDashboardData.Where(x => x.Indicators == 192).ToList();
            var nonMedicationRelatedIncidents = manualDashboardData.Where(x => x.Indicators == 1312 && x.Year == currentYear).ToList();
            var typeOfIncidents = manualDashboardData.Where(x => x.Indicators == 1313 && x.Year == currentYear).ToList();
            //var categoryofIncidents = manualDashboardData.Where(x => x.Indicators == 1314).ToList();  //categoryofIncidents
            var medicationErrors = manualDashboardData.Where(x => x.Indicators == 1311 && x.Year == currentYear).ToList();  //categoryofIncidents

            var customDataToreturn = new List<ManualDashboardCustomModel>();
            var manualDashboardData1 = manualDashboardData.Where(x => x.Indicators == 1309 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData2 = manualDashboardData.Where(x => x.Indicators == 1310 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData3 = manualDashboardData.Where(x => x.Indicators == 1314 && x.ExternalValue3 == "1").ToList();
            customDataToreturn.Add(manualDashboardData1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            customDataToreturn.Add(manualDashboardData3.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            customDataToreturn.Add(manualDashboardData2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            var categoryofIncidents = customDataToreturn;


            var jsonResult = new
            {
                totalsentinelevents,
                patienFallRate,
                totalNearmiss,
                totaladverseincidents,
                TotalMedicationErrors,
                totalIncidentsReports,
                mdroRate,
                mrsaRate,
                handHygieneCompliance,
                pressureUlcerIncidentRate,
                averageFIMScorePAR,
                averageFIMScoreLTC,
                inappropriateAntiBioticUsageRate,
                therapyInitialAssessmentProtocolCompliance,
                manualHandlingRiskAssessmentProtocolCompliance,
                standardizedOutcomeMeasureProtocol,
                Incidents,
                nonMedicationRelatedIncidents,
                typeOfIncidents,
                medicationErrors,
                categoryofIncidents
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the hr graphs data_ new.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult GetHRGraphsData_New(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var corporateid = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            const string HrGraphsIndicators = "1320,800,802,132,133,134,135,136,137,818,250,820,251,860,862,252,253,822";

            var manualDashboardData = _dbService.GetManualDashBoard(
                facilityId,
                corporateid,
                Convert.ToString(HrGraphsIndicators),
                currentYear,
                facilityType,
                segment,
                department);

            if (manualDashboardData.Any())
            {
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            }

            var physicianPositions = new List<ManualDashboardCustomModel>();
            var manualDashboardData1 = manualDashboardData.Where(x => x.Indicators == 1320 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData2 = manualDashboardData.Where(x => x.Indicators == 800 && x.ExternalValue3 == "1").ToList();
            var manualDashboardData3 = manualDashboardData.Where(x => x.Indicators == 802 && x.ExternalValue3 == "1").ToList();
            physicianPositions.Add(manualDashboardData1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            physicianPositions.Add(manualDashboardData2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            physicianPositions.Add(manualDashboardData3.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));

            var clinicianPositions = new List<ManualDashboardCustomModel>();
            var mclinicianPositions1 = manualDashboardData.Where(x => x.Indicators == 132 && x.ExternalValue3 == "1").ToList();
            var mclinicianPositions2 = manualDashboardData.Where(x => x.Indicators == 133 && x.ExternalValue3 == "1").ToList();
            var mclinicianPositions3 = manualDashboardData.Where(x => x.Indicators == 134 && x.ExternalValue3 == "1").ToList();
            clinicianPositions.Add(mclinicianPositions1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            clinicianPositions.Add(mclinicianPositions2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            clinicianPositions.Add(mclinicianPositions3.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));


            var administrativePositions = new List<ManualDashboardCustomModel>();
            var mAdministrativePositions1 = manualDashboardData.Where(x => x.Indicators == 135 && x.ExternalValue3 == "1").ToList();
            var mAdministrativePositions2 = manualDashboardData.Where(x => x.Indicators == 136 && x.ExternalValue3 == "1").ToList();
            var mAdministrativePositions3 = manualDashboardData.Where(x => x.Indicators == 137 && x.ExternalValue3 == "1").ToList();
            administrativePositions.Add(mAdministrativePositions1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            administrativePositions.Add(mAdministrativePositions2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            administrativePositions.Add(mAdministrativePositions3.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));

            var totalPositions = manualDashboardData.Where(x => x.Indicators == 818).OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ThenBy(m1 => m1.BudgetType).ToList();
            var administrationtotalStaff = manualDashboardData.Where(x => x.Indicators == 250).ToList();
            var emiratizationRate = manualDashboardData.Where(x => x.Indicators == 820).OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ThenBy(m1 => m1.BudgetType).ToList();
            var timeTakentoRecruitVacantPosition = manualDashboardData.Where(x => x.Indicators == 251).OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ThenBy(m1 => m1.BudgetType).ToList();

            var attritionRate = new List<ManualDashboardCustomModel>();
            var mattritionRate1 = manualDashboardData.Where(x => x.Indicators == 860 && x.ExternalValue3 == "1").ToList();
            var mattritionRate2 = manualDashboardData.Where(x => x.Indicators == 862 && x.ExternalValue3 == "1").ToList();
            attritionRate.Add(mattritionRate1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
            attritionRate.Add(mattritionRate2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));

            var overtimeRate = manualDashboardData.Where(x => x.Indicators == 822).OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ThenBy(m1 => m1.BudgetType).ToList();
            var productiveHours = manualDashboardData.Where(x => x.Indicators == 252).OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ThenBy(m1 => m1.BudgetType).ToList();
            var unproductiveHours = manualDashboardData.Where(x => x.Indicators == 253).OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ThenBy(m1 => m1.BudgetType).ToList();

            var jsonResult = new
            {
                physicianPositions,
                clinicianPositions,
                administrativePositions,
                totalPositions,
                administrationtotalStaff,
                emiratizationRate,
                timeTakentoRecruitVacantPosition,
                attritionRate,
                productiveHours,
                unproductiveHours,
                overtimeRate
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cams the get financial MGT graphs data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <returns></returns>
        public ActionResult CamGetFinancialMGTGraphsData(int facilityId, int month, int facilityType, int segment,
            int department)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;


            var corporateid = Helpers.GetSysAdminCorporateID();
            const string FinancialMgtIndicators = "110,111,113,115,280,259,256,257,258,260,281,282,117,142,1010,1015,1020";
            var manualDashboardData = _dbService.GetManualDashBoard(
                facilityId,
                corporateid,
                Convert.ToString(FinancialMgtIndicators),
                currentYear,
                facilityType,
                segment,
                department);
            if (manualDashboardData.Any())
            {
                manualDashboardData = manualDashboardData.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ToList();
            }

            var netRevenue = manualDashboardData.Where(x => x.Indicators == 110).ToList();
            var swbDirect = manualDashboardData.Where(x => x.Indicators == 111).ToList();
            var otherDirect = manualDashboardData.Where(x => x.Indicators == 113).ToList();
            var otherGAExpenses = manualDashboardData.Where(x => x.Indicators == 115).ToList();
            var facilityRentandUtilities = manualDashboardData.Where(x => x.Indicators == 280).ToList();
            var otherdirectpatientrelatedcosts = manualDashboardData.Where(x => x.Indicators == 259).ToList();
            var consumablesPPD = manualDashboardData.Where(x => x.Indicators == 256).ToList();
            var pharmacyPPD = manualDashboardData.Where(x => x.Indicators == 257).ToList();
            var fBPPD = manualDashboardData.Where(x => x.Indicators == 258).ToList();
            var newmarketdevelopmentSWB = manualDashboardData.Where(x => x.Indicators == 260).ToList();
            var marketingBDCosts = manualDashboardData.Where(x => x.Indicators == 281).ToList();
            var newMarketDevelopmentOtherCosts = manualDashboardData.Where(x => x.Indicators == 282).ToList();
            var deprandAmort = manualDashboardData.Where(x => x.Indicators == 117).ToList();
            var NursePatientRatio = manualDashboardData.Where(x => x.Indicators == 142 && x.SubCategory1 == "0").ToList();
            var healthCareassistantPatientratio = manualDashboardData.Where(x => x.Indicators == 1010).ToList();
            var therapistPatientratio = manualDashboardData.Where(x => x.Indicators == 1015).ToList();
            var physicianPatientratio = manualDashboardData.Where(x => x.Indicators == 1020).ToList();

            var jsonResult = new
            {
                netRevenue,
                swbDirect,
                otherDirect,
                otherGAExpenses,
                facilityRentandUtilities,
                otherdirectpatientrelatedcosts,
                consumablesPPD,
                pharmacyPPD,
                fBPPD,
                newmarketdevelopmentSWB,
                marketingBDCosts,
                newMarketDevelopmentOtherCosts,
                deprandAmort,
                NursePatientRatio,
                healthCareassistantPatientratio,
                therapistPatientratio,
                physicianPatientratio
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        #region Private Methods of Controller

        /// <summary>
        /// Gets the local sub category charts.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private List<ManualDashboardCustomModel> GetLocalSubCategoryCharts(int facilityID, int month, int facilityType, int segment,
            int Department, string type)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityID);
            var currentYear = currentDateTime.Year;

            var manualDashboardData = _dbService.GetSubCategoryCharts(
                facilityID,
                corporateId,
                Convert.ToString(type),
                currentYear,
                facilityType,
                segment,
                Department);

            // ......>Pie chart data Builder
            if (type == "156" || type == "159" || type == "142" || type == "141" || type == "229" || type == "222" || type == "141")
            {
                if (manualDashboardData.Any())
                {
                    manualDashboardData = manualDashboardData.Where(x => x.BudgetType == 2).ToList();
                    var newIndicatorLine = new ManualDashboardCustomModel
                    {
                        M1 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M1)).ToString(),
                        M2 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M2)).ToString(),
                        M3 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M3)).ToString(),
                        M4 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M4)).ToString(),
                        M5 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M5)).ToString(),
                        M6 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M6)).ToString(),
                        M7 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M7)).ToString(),
                        M8 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M8)).ToString(),
                        M9 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M9)).ToString(),
                        M10 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M10)).ToString(),
                        M11 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M11)).ToString(),
                        M12 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M12)).ToString(),
                    };
                    var lastrow = newIndicatorLine;
                    foreach (var item in manualDashboardData)
                    {
                        item.M1 = lastrow.M1 != "0.00"
                            ? ((Convert.ToDecimal(item.M1) / Convert.ToDecimal(lastrow.M1)) * 100).ToString()
                            : "0.00";
                        item.M2 = lastrow.M2 != "0.00"
                            ? ((Convert.ToDecimal(item.M2) / Convert.ToDecimal(lastrow.M2)) * 100).ToString()
                            : "0.00";
                        item.M3 = lastrow.M3 != "0.00"
                            ? ((Convert.ToDecimal(item.M3) / Convert.ToDecimal(lastrow.M3)) * 100).ToString()
                            : "0.00";
                        item.M4 = lastrow.M4 != "0.00"
                            ? ((Convert.ToDecimal(item.M4) / Convert.ToDecimal(lastrow.M4)) * 100).ToString()
                            : "0.00";
                        item.M5 = lastrow.M5 != "0.00"
                            ? ((Convert.ToDecimal(item.M5) / Convert.ToDecimal(lastrow.M5)) * 100).ToString()
                            : "0.00";
                        item.M6 = lastrow.M6 != "0.00"
                            ? ((Convert.ToDecimal(item.M6) / Convert.ToDecimal(lastrow.M6)) * 100).ToString()
                            : "0.00";
                        item.M7 = lastrow.M7 != "0.00"
                            ? ((Convert.ToDecimal(item.M7) / Convert.ToDecimal(lastrow.M7)) * 100).ToString()
                            : "0.00";
                        item.M8 = lastrow.M8 != "0.00"
                            ? ((Convert.ToDecimal(item.M8) / Convert.ToDecimal(lastrow.M8)) * 100).ToString()
                            : "0.00";
                        item.M9 = lastrow.M9 != "0.00"
                            ? ((Convert.ToDecimal(item.M9) / Convert.ToDecimal(lastrow.M9)) * 100).ToString()
                            : "0.00";
                        item.M10 = lastrow.M10 != "0.00"
                            ? ((Convert.ToDecimal(item.M10) / Convert.ToDecimal(lastrow.M10)) * 100).ToString()
                            : "0.00";
                        item.M11 = lastrow.M11 != "0.00"
                            ? ((Convert.ToDecimal(item.M11) / Convert.ToDecimal(lastrow.M11)) * 100).ToString()
                            : "0.00";
                        item.M12 = lastrow.M12 != "0.00"
                            ? ((Convert.ToDecimal(item.M12) / Convert.ToDecimal(lastrow.M12)) * 100).ToString()
                            : "0.00";
                    }
                }
            }
            // ......>Bar chart data Builder
            else if (type == "192")
            {
                manualDashboardData = manualDashboardData.Where(x => x.BudgetType == 1).ToList();
            }
            return (manualDashboardData.Any() ? manualDashboardData : null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="month"></param>
        /// <param name="facilityType"></param>
        /// <param name="segment"></param>
        /// <param name="Department"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<ManualDashboardCustomModel> GetSubCategoryChartsPayorMix(int facilityID, int month, int facilityType, int segment,
            int Department, string type)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityID);
            var currentYear = currentDateTime.Year;

            var manualDashboardData = _dbService.GetSubCategoryChartsPayorMix(facilityID, corporateId,
                Convert.ToString(type),
                currentYear, facilityType, segment, Department);
            //......>Pie chart data Builder
            if (type == "156" || type == "159" || type == "142" || type == "141" || type == "229" || type == "222")
            {
                if (manualDashboardData.Any())
                {
                    manualDashboardData = manualDashboardData.Where(x => x.BudgetType == 2).ToList();
                    var newIndicatorLine = new ManualDashboardCustomModel
                    {
                        M1 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M1)).ToString(),
                        M2 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M2)).ToString(),
                        M3 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M3)).ToString(),
                        M4 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M4)).ToString(),
                        M5 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M5)).ToString(),
                        M6 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M6)).ToString(),
                        M7 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M7)).ToString(),
                        M8 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M8)).ToString(),
                        M9 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M9)).ToString(),
                        M10 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M10)).ToString(),
                        M11 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M11)).ToString(),
                        M12 = manualDashboardData.Sum(x => Convert.ToDecimal(x.M12)).ToString(),
                    };
                    var lastrow = newIndicatorLine;
                    foreach (var item in manualDashboardData)
                    {
                        item.M1 = lastrow.M1 != "0.00" && lastrow.M1 != "0.0000"
                            ? ((Convert.ToDecimal(item.M1) / Convert.ToDecimal(lastrow.M1)) * 100).ToString()
                            : "0.00";
                        item.M2 = lastrow.M2 != "0.00" && lastrow.M2 != "0.0000"
                            ? ((Convert.ToDecimal(item.M2) / Convert.ToDecimal(lastrow.M2)) * 100).ToString()
                            : "0.00";
                        item.M3 = lastrow.M3 != "0.00" && lastrow.M3 != "0.0000"
                            ? ((Convert.ToDecimal(item.M3) / Convert.ToDecimal(lastrow.M3)) * 100).ToString()
                            : "0.00";
                        item.M4 = lastrow.M4 != "0.00" && lastrow.M4 != "0.0000"
                            ? ((Convert.ToDecimal(item.M4) / Convert.ToDecimal(lastrow.M4)) * 100).ToString()
                            : "0.00";
                        item.M5 = lastrow.M5 != "0.00" && lastrow.M5 != "0.0000"
                            ? ((Convert.ToDecimal(item.M5) / Convert.ToDecimal(lastrow.M5)) * 100).ToString()
                            : "0.00";
                        item.M6 = lastrow.M6 != "0.00" && lastrow.M6 != "0.0000"
                            ? ((Convert.ToDecimal(item.M6) / Convert.ToDecimal(lastrow.M6)) * 100).ToString()
                            : "0.00";
                        item.M7 = lastrow.M7 != "0.0000" && lastrow.M7 != "0.0000"
                            ? ((Convert.ToDecimal(item.M7) / Convert.ToDecimal(lastrow.M7)) * 100).ToString()
                            : "0.0000";
                        item.M8 = lastrow.M8 != "0.0000" && lastrow.M8 != "0.0000"
                            ? ((Convert.ToDecimal(item.M8) / Convert.ToDecimal(lastrow.M8)) * 100).ToString()
                            : "0.0000";
                        item.M9 = lastrow.M9 != "0.0000" && lastrow.M9 != "0.0000"
                            ? ((Convert.ToDecimal(item.M9) / Convert.ToDecimal(lastrow.M9)) * 100).ToString()
                            : "0.0000";
                        item.M10 = lastrow.M10 != "0.0000" && lastrow.M10 != "0.0000"
                            ? ((Convert.ToDecimal(item.M10) / Convert.ToDecimal(lastrow.M10)) * 100).ToString()
                            : "0.0000";
                        item.M11 = lastrow.M11 != "0.0000" && lastrow.M11 != "0.0000"
                            ? ((Convert.ToDecimal(item.M11) / Convert.ToDecimal(lastrow.M11)) * 100).ToString()
                            : "0.0000";
                        item.M12 = lastrow.M12 != "0.0000" && lastrow.M12 != "0.0000"
                            ? ((Convert.ToDecimal(item.M12) / Convert.ToDecimal(lastrow.M12)) * 100).ToString()
                            : "0.0000";
                    }
                }
            }
            //......>Bar chart data Builder
            else if (type == "192")
            {
                manualDashboardData = manualDashboardData.Where(x => x.BudgetType == 1).ToList();
            }
            return (manualDashboardData.Any() ? manualDashboardData : null);
        }
        /// <summary>
        /// Gets the local year to date data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private ExternalDashboardModel GetLocalYearToDateData(int facilityID, int month, int facilityType, int segment,
           int Department, string type)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityID);
            var currentYear = currentDateTime.Year;

            var customDate = month == 0
                ? currentDateTime.ToShortDateString()
                : Convert.ToDateTime(month + "/" + month + "/" + currentYear).ToShortDateString();
            var manualDashboardList = _dbService.GetManualDashBoardStatData(facilityID,
                Helpers.GetSysAdminCorporateID(), "", customDate, facilityType, segment, Department);
            var datatoreturn = manualDashboardList != null
                ? manualDashboardList.SingleOrDefault(x => x.IndicatorNumber == type)
                : null;
            return (datatoreturn ?? null);
        }

        /// <summary>
        /// Executives the dashboard list sort order.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> ExecutiveDashboardListSortOrder()
        {
            var list2Orders = new Dictionary<string, int>
                {
                    {"110", 0},//NetRevenue
                    {"111", 1},//SWB
                    {"609", 2},//Consumables
                    {"280", 3},//FacilityRentUtilities
                    {"113", 4},//OtherDirect
                    {"114", 5},//OperatingMargin
                    {"155", 6},//SWBIndirect
                    {"281", 7},//MarketingBDCosts
                    {"115", 8},//IndirectCosts
                    {"610", 9},//ExpansionPreOpeningCosts
                    {"116", 10},//EBITDA
                    {"260", 11},//NewMarketDevelopmentCosts
                    {"282", 12},//NewMarketDevelopmentOtherCosts
                    {"117", 13},//DeprandAmort
                    {"118", 14},//Interest
                    {"119", 15},
                    {"120", 16},
                    {"121", 17},
                    {"145", 18},
                    {"122", 19},
                    {"162", 20},
                    {"261", 21}
                };
            return list2Orders;
        }

        /// <summary>
        /// Executives the dashboard list sort order.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> ExecutiveDashboard1ListSortOrder()
        {
            var list1Orders = new Dictionary<string, int>
            {
                {"101", 1},
                {"102", 2},
                {"103", 3},
                {"104", 4},
                {"108", 5},
                {"109", 6},
                {"144", 7},
                {"242", 8},
                {"106", 9},
                {"105", 10}
            };
            return list1Orders;
        }

        /// <summary>
        /// Executives the dashboard list sort order.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> ExecutiveDashboardSection10ListSortOrder()
        {
            var list10Orders = new Dictionary<string, int>
            {
                {"124", 0},
                {"125", 1},
                {"126", 2},
                {"283", 3},
                {"127", 4},
                {"284", 5},
                {"285", 6},
                {"286", 7},
                {"287", 8},
                {"288", 9},
                {"128", 10},
                {"129", 11},
                {"130", 12},
                {"131", 13},
            };
            return list10Orders;
        }

        /// <summary>
        /// Executives the dashboard list sort order.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> ExecutiveDashboardBalanceSheetListSortOrder()
        {
            var list10Orders = new Dictionary<string, int>
            {
                {"289", 0},
                {"290", 1},
                {"291", 2},
                {"293", 3},
                {"1317", 4},
                {"1318", 5},
                {"294", 6},
                {"1319", 7},
                {"600", 8},
                {"601", 9},
                {"602", 10},
                {"603", 11},
                {"295", 12},
                {"296", 13},
                {"298", 14},
                {"604", 15},
                {"605", 16},
                {"300", 17},
                {"606", 18},
                {"607", 19},
                {"302", 20},
                {"303", 21},
                {"304", 22},
                {"608", 23},
                {"305", 24},
                {"306", 25},
            };
            return list10Orders;
        }

        /// <summary>
        /// Gets the patient days all.
        /// </summary>
        /// <param name="patientDaysList">The patient days list.</param>
        /// <returns></returns>
        private List<ManualDashboardCustomModel> GetPatientDaysAll(List<ManualDashboardCustomModel> patientDaysList)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(Helpers.GetDefaultFacilityId());
            var currentYear = currentDateTime.Year;

            var newPatientDays = new List<ManualDashboardCustomModel>();
            //......>Pie chart data Builder
            if (patientDaysList.Any())
            {
                var patientDaysListCurrentYearBudget =
                    patientDaysList.Where(x => x.BudgetType == 1 && x.Year == currentYear).ToList();
                var patientDaysListCurrentYearActual =
                    patientDaysList.Where(x => x.BudgetType == 2 && x.Year == currentYear).ToList();
                var patientDaysListPreviousYearActual =
                    patientDaysList.Where(x => x.BudgetType == 2 && x.Year == currentYear - 1).ToList();
                var patientDaysListCurrentYearBudgetfirst = patientDaysListCurrentYearBudget.FirstOrDefault() ??
                                                            null;
                var patientDaysListCurrentYearActualfirst = patientDaysListCurrentYearActual.FirstOrDefault() ??
                                                            null;
                var patientDaysListPreviousYearActualfirst = patientDaysListPreviousYearActual.FirstOrDefault() ??
                                                             null;
                if (patientDaysListCurrentYearBudgetfirst != null)
                {
                    patientDaysListCurrentYearBudgetfirst.M1 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M1)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M2 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M2)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M3 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M3)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M4 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M4)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M5 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M5)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M6 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M6)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M7 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M7)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M8 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M8)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M9 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M9)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M10 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M10)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M11 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M11)).ToString();
                    patientDaysListCurrentYearBudgetfirst.M12 =
                        patientDaysListCurrentYearBudget.Sum(x => Convert.ToDecimal(x.M12)).ToString();
                }
                if (patientDaysListCurrentYearActualfirst != null)
                {
                    patientDaysListCurrentYearActualfirst.M1 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M1)).ToString();
                    patientDaysListCurrentYearActualfirst.M2 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M2)).ToString();
                    patientDaysListCurrentYearActualfirst.M3 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M3)).ToString();
                    patientDaysListCurrentYearActualfirst.M4 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M4)).ToString();
                    patientDaysListCurrentYearActualfirst.M5 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M5)).ToString();
                    patientDaysListCurrentYearActualfirst.M6 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M6)).ToString();
                    patientDaysListCurrentYearActualfirst.M7 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M7)).ToString();
                    patientDaysListCurrentYearActualfirst.M8 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M8)).ToString();
                    patientDaysListCurrentYearActualfirst.M9 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M9)).ToString();
                    patientDaysListCurrentYearActualfirst.M10 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M10)).ToString();
                    patientDaysListCurrentYearActualfirst.M11 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M11)).ToString();
                    patientDaysListCurrentYearActualfirst.M12 =
                        patientDaysListCurrentYearActual.Sum(x => Convert.ToDecimal(x.M12)).ToString();
                }
                if (patientDaysListPreviousYearActualfirst != null)
                {
                    patientDaysListPreviousYearActualfirst.M1 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M1)).ToString();
                    patientDaysListPreviousYearActualfirst.M2 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M2)).ToString();
                    patientDaysListPreviousYearActualfirst.M3 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M3)).ToString();
                    patientDaysListPreviousYearActualfirst.M4 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M4)).ToString();
                    patientDaysListPreviousYearActualfirst.M5 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M5)).ToString();
                    patientDaysListPreviousYearActualfirst.M6 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M6)).ToString();
                    patientDaysListPreviousYearActualfirst.M7 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M7)).ToString();
                    patientDaysListPreviousYearActualfirst.M8 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M8)).ToString();
                    patientDaysListPreviousYearActualfirst.M9 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M9)).ToString();
                    patientDaysListPreviousYearActualfirst.M10 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M10)).ToString();
                    patientDaysListPreviousYearActualfirst.M11 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M11)).ToString();
                    patientDaysListPreviousYearActualfirst.M12 =
                        patientDaysListPreviousYearActual.Sum(x => Convert.ToDecimal(x.M12)).ToString();
                }
                if (patientDaysListPreviousYearActualfirst != null)
                    newPatientDays.Add(patientDaysListPreviousYearActualfirst);
                if (patientDaysListCurrentYearActualfirst != null)
                    newPatientDays.Add(patientDaysListCurrentYearActualfirst);
                if (patientDaysListCurrentYearBudgetfirst != null)
                    newPatientDays.Add(patientDaysListCurrentYearBudgetfirst);
            }
            //......>Bar chart data Builder
            return newPatientDays;
        }

        /// <summary>
        /// Gets the adc by service code all.
        /// </summary>
        /// <param name="patientDaysList">The patient days list.</param>
        /// <returns></returns>
        private List<ManualDashboardCustomModel> GetADCByServiceCodePerMonth(List<ManualDashboardCustomModel> patientDaysList)
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(Helpers.GetDefaultFacilityId());
            var currentYear = currentDateTime.Year;

            var patientDaysForeachMonth = patientDaysList.Where(x => x.BudgetType == 2 && x.Year == currentYear).ToList();
            foreach (var item in patientDaysForeachMonth)
            {
                item.M1 = (Convert.ToDecimal(item.M1) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 1))).ToString();
                item.M2 = (Convert.ToDecimal(item.M2) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 2))).ToString();
                item.M3 = (Convert.ToDecimal(item.M3) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 3))).ToString();
                item.M4 = (Convert.ToDecimal(item.M4) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 4))).ToString();
                item.M5 = (Convert.ToDecimal(item.M5) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 5))).ToString();
                item.M6 = (Convert.ToDecimal(item.M6) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 6))).ToString();
                item.M7 = (Convert.ToDecimal(item.M7) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 7))).ToString();
                item.M8 = (Convert.ToDecimal(item.M8) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 8))).ToString();
                item.M9 = (Convert.ToDecimal(item.M9) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 9))).ToString();
                item.M10 = (Convert.ToDecimal(item.M10) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 10))).ToString();
                item.M11 = (Convert.ToDecimal(item.M11) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 11))).ToString();
                item.M12 = (Convert.ToDecimal(item.M12) / Convert.ToDecimal(DateTime.DaysInMonth(currentYear, 12))).ToString();
            }
            return patientDaysForeachMonth;
        }

        /// <summary>
        /// Gets the manual dashboard data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="month">The month.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="department">The department.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private List<ManualDashboardCustomModel> GetGraphsData(int facilityId, int month, int facilityType, int segment, int department, string type)
        {
            var manualDashboardData = new List<ManualDashboardCustomModel>();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _fService.GetInvariantCultureDateTime(facilityId);
            var currentYear = currentDateTime.Year;

            if (type == "124")
            {
                var totalOperatingExpenditures = _dbService.GetManualDashBoard(facilityId,
                    Helpers.GetSysAdminCorporateID(), "165", currentYear, facilityType, segment, department);
                var netCapitalExpendureOperations =
                    _dbService.GetManualDashBoard(facilityId,
                        Helpers.GetSysAdminCorporateID(), "127", currentYear, facilityType, segment, department);
                var netCashCollection = _dbService.GetManualDashBoard(facilityId,
                    Helpers.GetSysAdminCorporateID(), "143", currentYear, facilityType, segment, department);
                manualDashboardData.AddRange(netCashCollection.Where(x => x.BudgetType == 2 && x.Year == currentYear));
                manualDashboardData.AddRange(totalOperatingExpenditures.Where(x => x.BudgetType == 2 && x.Year == currentYear));
                manualDashboardData.AddRange(netCapitalExpendureOperations.Where(x => x.BudgetType == 2 && x.Year == currentYear));
            }
            else if (type == "121")
            {
                var sumofFields = _dbService.GetManualDashBoard(facilityId, corporateId, Convert.ToString(type),
                   currentYear, facilityType, segment, department);
                var fieldtoDivide = _dbService.GetManualDashBoard(facilityId, corporateId, Convert.ToString("110"),
                   currentYear, facilityType, segment, department);
                foreach (var item in sumofFields)
                {
                    var fd = fieldtoDivide.FirstOrDefault(x => x.BudgetType == item.BudgetType && x.Year == item.Year);
                    if (fd != null)
                    {
                        item.M1 = fd.M1 == "0.00" ? (Convert.ToDecimal(item.M1) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M1) / Convert.ToDecimal(fd.M1)).ToString();
                        item.M2 = fd.M2 == "0.00" ? (Convert.ToDecimal(item.M2) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M2) / Convert.ToDecimal(fd.M2)).ToString();
                        item.M3 = fd.M3 == "0.00" ? (Convert.ToDecimal(item.M3) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M3) / Convert.ToDecimal(fd.M3)).ToString();
                        item.M4 = fd.M4 == "0.00" ? (Convert.ToDecimal(item.M4) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M4) / Convert.ToDecimal(fd.M4)).ToString();
                        item.M5 = fd.M5 == "0.00" ? (Convert.ToDecimal(item.M5) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M5) / Convert.ToDecimal(fd.M5)).ToString();
                        item.M6 = fd.M6 == "0.00" ? (Convert.ToDecimal(item.M6) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M6) / Convert.ToDecimal(fd.M6)).ToString();
                        item.M7 = fd.M7 == "0.00" ? (Convert.ToDecimal(item.M7) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M7) / Convert.ToDecimal(fd.M7)).ToString();
                        item.M8 = fd.M8 == "0.00" ? (Convert.ToDecimal(item.M8) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M8) / Convert.ToDecimal(fd.M8)).ToString();
                        item.M9 = fd.M9 == "0.00" ? (Convert.ToDecimal(item.M9) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M9) / Convert.ToDecimal(fd.M9)).ToString();
                        item.M10 = fd.M10 == "0.00" ? (Convert.ToDecimal(item.M10) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M10) / Convert.ToDecimal(fd.M10)).ToString();
                        item.M11 = fd.M11 == "0.00" ? (Convert.ToDecimal(item.M11) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M11) / Convert.ToDecimal(fd.M11)).ToString();
                        item.M12 = fd.M12 == "0.00" ? (Convert.ToDecimal(item.M12) / Convert.ToDecimal(1)).ToString() : (Convert.ToDecimal(item.M12) / Convert.ToDecimal(fd.M12)).ToString();
                    }
                }
                manualDashboardData.AddRange(sumofFields);
            }
            else if (type == "1314")
            {
                var customDataToreturn = new List<ManualDashboardCustomModel>();
                var manualDashboardData1 = _dbService.GetManualDashBoard(facilityId, corporateId,
                   Convert.ToString("1309"),
                   currentYear, facilityType, segment, department);
                var manualDashboardData2 = _dbService.GetManualDashBoard(facilityId, corporateId,
                   Convert.ToString("1310"),
                   currentYear, facilityType, segment, department);
                var manualDashboardData3 = _dbService.GetManualDashBoard(facilityId, corporateId,
                   Convert.ToString("1314"),
                   currentYear, facilityType, segment, department);

                customDataToreturn.Add(manualDashboardData1.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
                customDataToreturn.Add(manualDashboardData3.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
                customDataToreturn.Add(manualDashboardData2.FirstOrDefault(x => x.BudgetType == 2 && x.Year == currentYear));
                return customDataToreturn;
            }
            else
            {
                manualDashboardData = _dbService.GetManualDashBoard(facilityId, corporateId,
                    Convert.ToString(type),
                    currentYear, facilityType, segment, department);
            }
            return manualDashboardData;
        }




        private List<ProjectsCustomModel> GetProjectsDashboardData(int facilityId, string responsibleUserId)
        {
            var list = _ptService.GetProjectsDashboardData(Helpers.GetSysAdminCorporateID(), facilityId, responsibleUserId);
            if (list.Count > 0)
            {
                foreach (var pr in list)
                {
                    if (pr.Milestones != null && pr.Milestones.Any())
                    {
                        pr.Milestones.ForEach(cc =>
                          cc.ColorImage = Url.Content(cc.ColorImage));
                    }
                }
            }
            return list;
        }

        #endregion


        /// <summary>
        /// Cases the management dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult CaseManagementDashboard1()
        {
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            //var _fService = new FacilityService();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var section5RemarksList = new List<DashboardRemarkCustomModel>();
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                    Convert.ToInt32(7));
            if (allRemarksList != null && allRemarksList.Count > 0)
            {
                section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("1")).ToList();
                section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("2")).ToList();
                section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("3")).ToList();
                section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("4")).ToList();
                section5RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("5")).ToList();
            }
            var dashboardview = new CaseMgtDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 7,
                Title = Helpers.ExternalDashboardTitleView("6"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList
            };
            return View(dashboardview);
        }

        /// <summary>
        /// Clinical Quality Dashboard.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult ClinicalQuality1(int? type)
        {
            if (type != null && Convert.ToInt32(type) > 0)
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
                var loggedinfacilityId = Helpers.GetDefaultFacilityId();
                //var _fService = new FacilityService();
                var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
                var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();
                var corporateid = Helpers.GetSysAdminCorporateID();

                var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid, Convert.ToInt32(type));
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
                }

                var dashboardview = new ExecutiveDashboardView
                {
                    FacilityId = facilityid,
                    DashboardType = 4,
                    Title = Helpers.ExternalDashboardTitleView("3"),
                    Section1RemarksList = section1RemarksList,
                    Section2RemarksList = section2RemarksList,
                    Section3RemarksList = section3RemarksList,
                    Section4RemarksList = section4RemarksList,
                    Section5RemarksList = section5RemarksList,
                    Section6RemarksList = section6RemarksList,
                    Section7RemarksList = section7RemarksList,
                    Section8RemarksList = section8RemarksList,
                    Section9RemarksList = section9RemarksList,
                };
                return View(dashboardview);
            }
            return View("Index");
        }

        /// <summary>
        /// Hrs the dashboard1.
        /// </summary>
        /// <returns></returns>
        public ActionResult HRDashboard1()
        {
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            //var _fService = new FacilityService();
            var corporateFacilitydetail = _fService.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                   ? loggedinfacilityId
                    : Helpers.GetFacilityIdNextDefaultCororateFacility();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var section5RemarksList = new List<DashboardRemarkCustomModel>();
            var section6RemarksList = new List<DashboardRemarkCustomModel>();

            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityid,
                    Convert.ToInt32(8));
            if (allRemarksList != null && allRemarksList.Count > 0)
            {
                section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("1")).ToList();
                section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("2")).ToList();
                section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("3")).ToList();
                section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("4")).ToList();
                section5RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("5")).ToList();
                section6RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals("6")).ToList();
            }
            var dashboardview = new HRDashboardView
            {
                FacilityId = facilityid,
                DashboardType = 8,
                Title = Helpers.ExternalDashboardTitleView("7"),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                Section5RemarksList = section5RemarksList,
                Section6RemarksList = section6RemarksList
            };
            return View(dashboardview);
        }

        private ProjectsDashboardView GetProjectsData(int facilityId, int userId)
        {
            const int dashboardType = 9;
            var section1RemarksList = new List<DashboardRemarkCustomModel>();
            var section2RemarksList = new List<DashboardRemarkCustomModel>();
            var section3RemarksList = new List<DashboardRemarkCustomModel>();
            var section4RemarksList = new List<DashboardRemarkCustomModel>();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var mainList = GetProjectsDashboardData(facilityId, userId == 0 ? string.Empty : Convert.ToString(userId));
            var allRemarksList = _drService.GetDashboardRemarkListByDashboardType(corporateid, facilityId, dashboardType);

            var strategicType = Convert.ToString((int)DashboardProjectType.Strategic);
            var financialType = Convert.ToString((int)DashboardProjectType.Financial);
            var indType = Convert.ToString((int)DashboardProjectType.Individual);
            var opType = Convert.ToString((int)DashboardProjectType.Operational);

            var strategicExec = mainList.Where(g => g.ExternalValue2.Trim().Equals(strategicType)).ToList();
            var financialExec = mainList.Where(g => g.ExternalValue2.Trim().Equals(financialType)).ToList();
            var individualExec = mainList.Where(g => g.ExternalValue2.Trim().Equals(indType)).ToList();
            var opExec = mainList.Where(g => g.ExternalValue2.Trim().Equals(opType)).ToList();

            if (allRemarksList != null && allRemarksList.Count > 0)
            {
                section1RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals(strategicType)).ToList();
                section2RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals(opType)).ToList();
                section3RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals(financialType)).ToList();
                section4RemarksList = allRemarksList.Where(s => s.DashboardSection.Equals(indType)).ToList();
            }
            var dashboardview = new ProjectsDashboardView
            {
                FacilityId = facilityId,
                DashboardType = dashboardType,
                StrategicKpiList = strategicExec,
                FinancialKpiList = financialExec,
                IndividualKpiList = individualExec,
                OperationalKpiList = opExec,
                Title = Helpers.ExternalDashboardTitleView(Convert.ToString(dashboardType)),
                Section1RemarksList = section1RemarksList,
                Section2RemarksList = section2RemarksList,
                Section3RemarksList = section3RemarksList,
                Section4RemarksList = section4RemarksList,
                ResponsibleUserId = userId
            };
            return dashboardview;
        }
    }
}