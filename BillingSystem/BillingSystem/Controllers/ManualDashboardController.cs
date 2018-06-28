using System;
using System.IO;
using System.Linq;
using System.Web;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using Hangfire;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Threading.Tasks;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class ManualDashboardController : BaseController
    {
        private readonly IFacilityStructureService _fsService;
        private readonly IDashboardIndicatorDataService _diService;
        private readonly IPatientInfoService _piService;
        private readonly IUsersService _uService;
        private readonly IManualDashboardService _service;
        private readonly IIndicatorDataCheckListService _iService;

        public ManualDashboardController(IManualDashboardService service, IDashboardIndicatorDataService diService, IFacilityStructureService fsService
            , IPatientInfoService piService, IUsersService uService, IIndicatorDataCheckListService iService)
        {
            _fsService = fsService;
            _piService = piService;
            _uService = uService;
            _diService = diService;
            _service = service;
            _iService = iService;
        }

        /// <summary>
        /// Get the details of the ManualDashboard View in the Model ManualDashboard such as ManualDashboardList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model ManualDashboard to be passed to View ManualDashboard
        /// </returns>
        public ActionResult Index()
        {
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            var facilitybal = new FacilityBal();
            var corporateFacilitydetail = facilitybal.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID != 0
                ? loggedinfacilityId : Helpers.GetFacilityIdNextDefaultCororateFacility();

            var data = new List<ManualDashboardCustomModel>();
            //Intialize the View Model i.e. ManualDashboardView which is binded to Main View Index.cshtml under ManualDashboard
            var viewModel = new ManualDashboardView
            {
                ManualDashboardList = data,
                CurrentManualDashboard = new ManualDashboardCustomModel(),
                IsAdmin = userisAdmin,
                CFacilityId = facilityid
            };

            //Pass the View Model in ActionResult to View ManualDashboard
            return View(viewModel);
        }

        /// <summary>
        /// Gets the manual dashboard list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetManualDashboardList()
        {
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var loggedinfacilityId = Helpers.GetDefaultFacilityId();
            var facilitybal = new FacilityBal();
            var corporateFacilitydetail = facilitybal.GetFacilityById(loggedinfacilityId);
            var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == 0
                ? loggedinfacilityId : Helpers.GetFacilityIdNextDefaultCororateFacility();

            //Get the Entity list
            var list = _service.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                     facilityid, Convert.ToString(CurrentDateTime.Year));
            list = list.Where(x => x.IsActive == true).ToList();
            var orderByExpression = HtmlExtensions.GetOrderByExpression<ManualDashboardCustomModel>("Indicators");
            list = HtmlExtensions.OrderByDir<ManualDashboardCustomModel>(list, "ASC", orderByExpression);
            return PartialView(PartialViews.ManualDashboardList, list);
        }



        /// <summary>
        /// Get the details of the current ManualDashboard in the view model by ID
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="corporateId"></param>
        /// <param name="year"></param>
        /// <param name="indicatorNumber"></param>
        /// <param name="budgetType"></param>
        /// <param name="subCategory1"></param>
        /// <param name="subCategory2"></param>
        /// <returns></returns>
        public JsonResult GetManualDashboardDetails(int facilityId, int corporateId, string year, string indicatorNumber, string budgetType, string subCategory1, string subCategory2)
        {
            //Call the AddManualDashboard Method to Add / Update current ManualDashboard
            var current = _service.GetManualDashboardDataByIndicatorNumber(corporateId, facilityId,
                year, indicatorNumber, budgetType, subCategory1, subCategory2);

            if (current != null)
            {
                var objIsLocked = GetIsLockedProperties(current.FacilityId.GetValueOrDefault(),
                    current.CorporateId.GetValueOrDefault(), current.BudgetType.GetValueOrDefault()
                    , current.Year.GetValueOrDefault());
                var jsonData = new { current, locked = objIsLocked };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

            //Pass the ActionResult with the current ManualDashboardViewModel object as model to PartialView ManualDashboardAddEdit
            return Json(new ManualDashboardCustomModel(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the manual dashboard details.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="year">The year.</param>
        /// <param name="indicatorNumber">The indicator number.</param>
        /// <param name="budgetType">Type of the budget.</param>
        /// <returns></returns>
        public ActionResult DeleteManualDashboardDetails(int facilityId, int corporateId, string year, string indicatorNumber, string budgetType, string subCategory1, string subCategory2)
        {
            var status = _diService.DeleteManualDashboardDetails(corporateId, facilityId,
                     indicatorNumber, budgetType, year, subCategory1, subCategory2);

            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var list = userisAdmin
                        ? _service.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                            0, Convert.ToString(CurrentDateTime.Year))
                            : _service.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                            Helpers.GetDefaultFacilityId(), Convert.ToString(CurrentDateTime.Year));
            return PartialView(PartialViews.ManualDashboardList, list);
        }

        /// <summary>
        /// Delete the current ManualDashboard based on the ManualDashboard ID passed in the ManualDashboardModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteManualDashboard(int id)
        {
            //Get ManualDashboard model object by current ManualDashboard ID
            var vm = _service.GetManualDashboardById(id);
            var userId = Helpers.GetLoggedInUserId();
            var list = new List<ManualDashboardCustomModel>();
            var currentDate = Helpers.GetInvariantCultureDateTime();

            //Check If ManualDashboard model is not null
            if (vm != null)
            {
                vm.CorporateId = Helpers.GetSysAdminCorporateID();
                vm.IsActive = false;
                vm.CreatedBy = userId;
                vm.CreatedDate = currentDate;
                list = _service.SaveManualDashboard(vm);
            }
            return PartialView(PartialViews.ManualDashboardList, list);
        }

        /// <summary>
        /// Determines whether [is data exist] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="indicatorNumber">The indicators.</param>
        /// <param name="budgetType">Type of the budget.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public ActionResult IsDataExist(int id, int indicatorNumber, int budgetType, int facilityId, int year)
        {
            if (id != 0)
                return Json(true, JsonRequestBehavior.AllowGet);

            var list = _service.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                Helpers.GetDefaultFacilityId(), Convert.ToString(CurrentDateTime.Year));
            var selectedValue =
                !list.Any(x => x.Indicators == indicatorNumber && x.BudgetType == budgetType && x.Year == year);
            return Json(selectedValue, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Rebinds the grid with facility.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="year"></param>
        /// <param name="indicator"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public ActionResult RebindGridWithFacility(int facilityId, int year, int indicator, string owner)
        {
            var list = _service.RebindManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), facilityId, year, indicator, owner);
            return PartialView(PartialViews.ManualDashboardList, list);
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public ActionResult ExportToExcel(int facilityId, int year, int indicator, string owner)
        {
            //Thread.Sleep(10000);
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("ManualDashboardData");
            var format = workbook.CreateDataFormat();
            sheet.CreateFreezePane(0, 1, 0, 1);
            sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:W1"));
            //sheet.SetAutoFilter()
            // Add header labels
            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("BudgetType");
            row.CreateCell(1).SetCellValue("Indicators");
            row.CreateCell(2).SetCellValue("Indicator Numbers");
            row.CreateCell(3).SetCellValue("SubCategory1");
            row.CreateCell(4).SetCellValue("SubCategory2");
            row.CreateCell(5).SetCellValue("Frequency");
            row.CreateCell(6).SetCellValue("Corporate");
            row.CreateCell(7).SetCellValue("Facility");
            row.CreateCell(8).SetCellValue("OwnerShip");
            row.CreateCell(9).SetCellValue("Year");
            row.CreateCell(10).SetCellValue("Jan");
            row.CreateCell(11).SetCellValue("Feb");
            row.CreateCell(12).SetCellValue("Mar");
            row.CreateCell(13).SetCellValue("Apr");
            row.CreateCell(14).SetCellValue("May");
            row.CreateCell(15).SetCellValue("Jun");
            row.CreateCell(16).SetCellValue("Jul");
            row.CreateCell(17).SetCellValue("Aug");
            row.CreateCell(18).SetCellValue("Sep");
            row.CreateCell(19).SetCellValue("Oct");
            row.CreateCell(20).SetCellValue("Nov");
            row.CreateCell(21).SetCellValue("Dec");
            row.CreateCell(22).SetCellValue("OtherDescription");
            rowIndex++;
            // CEll STYLE 
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

            var ExportList = _service.RebindManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), facilityId, year,
                     indicator, owner);
            foreach (var item in ExportList)
            {

                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(item.BudgetTypeStr);
                row.CreateCell(1).SetCellValue(item.IndicatorTypeStr);
                row.CreateCell(2).SetCellType(CellType.Numeric);
                row.CreateCell(2).CellStyle = cellStyle;
                row.CreateCell(2).SetCellValue(Convert.ToDouble(item.Indicators));
                row.CreateCell(3).SetCellValue(item.SubCategoryValue1Str);
                row.CreateCell(4).SetCellValue(item.SubCategoryValue2Str);
                row.CreateCell(5).SetCellValue(item.FrequencyTypeStr);
                row.CreateCell(6).SetCellValue(item.CorporateStr);
                row.CreateCell(7).SetCellValue(item.FacilityStr);
                row.CreateCell(8).SetCellValue(item.OwnerShip);

                row.CreateCell(9).SetCellType(CellType.Numeric);
                row.CreateCell(9).CellStyle = cellStyle;
                row.CreateCell(9).SetCellValue(Convert.ToDouble(item.Year));

                row.CreateCell(10).SetCellType(CellType.Numeric);
                row.CreateCell(10).CellStyle = cellStyle;
                row.CreateCell(10).SetCellValue(Convert.ToDouble(item.M1));

                row.CreateCell(11).SetCellType(CellType.Numeric);
                row.CreateCell(11).CellStyle = cellStyle;
                row.CreateCell(11).SetCellValue(Convert.ToDouble(item.M2));

                row.CreateCell(12).SetCellType(CellType.Numeric);
                row.CreateCell(12).CellStyle = cellStyle;
                row.CreateCell(12).SetCellValue(Convert.ToDouble(item.M3));

                row.CreateCell(13).SetCellType(CellType.Numeric);
                row.CreateCell(13).CellStyle = cellStyle;
                row.CreateCell(13).SetCellValue(Convert.ToDouble(item.M4));

                row.CreateCell(14).SetCellType(CellType.Numeric);
                row.CreateCell(14).CellStyle = cellStyle;
                row.CreateCell(14).SetCellValue(Convert.ToDouble(item.M5));

                row.CreateCell(15).SetCellType(CellType.Numeric);
                row.CreateCell(15).CellStyle = cellStyle;
                row.CreateCell(15).SetCellValue(Convert.ToDouble(item.M6));

                row.CreateCell(16).SetCellType(CellType.Numeric);
                row.CreateCell(16).CellStyle = cellStyle;
                row.CreateCell(16).SetCellValue(Convert.ToDouble(item.M7));

                row.CreateCell(17).SetCellType(CellType.Numeric);
                row.CreateCell(17).CellStyle = cellStyle;
                row.CreateCell(17).SetCellValue(Convert.ToDouble(item.M8));

                row.CreateCell(18).SetCellType(CellType.Numeric);
                row.CreateCell(18).CellStyle = cellStyle;
                row.CreateCell(18).SetCellValue(Convert.ToDouble(item.M9));

                row.CreateCell(19).SetCellType(CellType.Numeric);
                row.CreateCell(19).CellStyle = cellStyle;
                row.CreateCell(19).SetCellValue(Convert.ToDouble(item.M10));

                row.CreateCell(20).SetCellType(CellType.Numeric);
                row.CreateCell(20).CellStyle = cellStyle;
                row.CreateCell(20).SetCellValue(Convert.ToDouble(item.M11));

                row.CreateCell(21).SetCellType(CellType.Numeric);
                row.CreateCell(21).CellStyle = cellStyle;
                row.CreateCell(21).SetCellValue(Convert.ToDouble(item.M12));

                row.CreateCell(22).SetCellValue(item.OtherDescription);
                rowIndex++;
            }

            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                Response.Cookies.Add(cookie);
                workbook.Write(exportData);
                string saveAsFileName = string.Format("ManualDashboardData-{0:d}.xls", CurrentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }

        /// <summary>
        /// Gets the manual dashboard data.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        private List<ManualDashboardCustomModel> GetManualDashboardData(int facilityid)
        {
            var list = facilityid == 0
                    ? _service.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), 0,
                        Convert.ToString(CurrentDateTime.Year))
                    : _service.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                        facilityid, Convert.ToString(CurrentDateTime.Year));
            return list;
        }

        /// <summary>
        /// Downloads the import excel file.
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadImportExcelFile()
        {
            var virtualPath = CommonConfig.ManualDashboardExcelTemplateFilePath;
            var serverPath = Server.MapPath(virtualPath);
            var corporateId = Helpers.GetSysAdminCorporateID();
            int expiryDays;
            using (var gBal = new GlobalCodeBal())
            {
                var result = gBal.GetIndicatorSettingsByCorporateId(Convert.ToString(corporateId));
                expiryDays = !string.IsNullOrEmpty(result.GlobalCodeName)
                    ? Convert.ToInt32(result.GlobalCodeName)
                    : 90;
            }

            var cookie = new HttpCookie("Downloaded", "True");
            Response.Cookies.Add(cookie);

            var expireDate = CurrentDateTime.AddDays(expiryDays);
            var ut = _service.GetUserToken(Helpers.GetLoggedInUsername(), expireDate);

            var userId = Helpers.GetLoggedInUserId();
            var facId = Helpers.GetDefaultFacilityId();
            BackgroundJob.Enqueue(() => SendUserTokenAndSaveAsync(ut.TokenNumber, expireDate, userId, facId).Wait());

            var saveAsFileName = string.Format("ManualDashboardDataTemplate-{0:d}.xlsm", CurrentDateTime)
                .Replace("/", "-");
            return File(serverPath, "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
        }

        /// <summary>
        /// Sends the user token and save.
        /// </summary>
        /// <param name="usertoken">The usertoken.</param>
        /// <returns></returns>
        public async Task<bool> SendUserTokenAndSaveAsync(string usertoken, DateTime? expiryDate, int loggedInUserId, int facilityId)
        {
            var msgBody = ResourceKeyValues.GetFileText("usertokentoaccess");
            Users userCm = null;

            var currentDate = Helpers.GetInvariantCultureDateTime();
            userCm = _uService.GetUserById(loggedInUserId);
            var facilityname = _piService.GetFacilityNameByFacilityId(facilityId);
            if (!string.IsNullOrEmpty(msgBody) && userCm != null)
            {
                userCm.UserToken = usertoken;
                msgBody = msgBody.Replace("{User}", userCm.UserName)
                   .Replace("{Facility-Name}", facilityname).Replace("{CodeValue}", usertoken).Replace("{TokenGeneratedon}", currentDate.ToShortDateString()).
                   Replace("{TokenExpireOn}", expiryDate.Value.ToShortDateString());
            }

            var status = false;
            if (userCm != null && !string.IsNullOrEmpty(userCm.Email))
            {
                var emailInfo = new EmailInfo
                {
                    VerificationTokenId = "",
                    PatientId = 0,
                    Email = userCm.Email, //"er.amitjain@hotmail.com",//"krishan.kumar@spadezgroup.com",//
                    Subject = ResourceKeyValues.GetKeyValue("usertokenemailsubject"),
                    VerificationLink = "",
                    MessageBody = msgBody
                };
                status = await MailHelper.SendEmailAsync(emailInfo);
                if (status)
                    userCm.UserToken = usertoken;
            }
            return status;
        }

        /// <summary>
        /// Binds the indicators by order.
        /// </summary>
        /// <param name="sort">The sort.</param>
        /// <param name="sortdir">The sortdir.</param>
        /// <returns></returns>
        public ActionResult BindManualDashboardIndicatorsDataByOrder(string sort, string sortdir, string facilityId, int year, int indicator, string owner)
        {
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();

            var list = userisAdmin
                ? _service.RebindManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                    Convert.ToInt32(facilityId), year, indicator, owner)
                : _service.RebindManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                    Helpers.GetDefaultFacilityId(), year, indicator, owner);

            //Intialize the View Model i.e. DashboardIndicatorsView which is binded to Main View Index.cshtml under DashboardIndicators
            var orderByExpression = HtmlExtensions.GetOrderByExpression<ManualDashboardCustomModel>(sort);
            var data = HtmlExtensions.OrderByDir(list, sortdir, orderByExpression);

            //Pass the View Model in ActionResult to View DashboardIndicators
            return PartialView(PartialViews.ManualDashboardList, data);
        }

        /// <summary>
        /// Binds the indicators active inactive.
        /// </summary>
        /// <param name="showInActive">The show in active.</param>
        /// <returns></returns>
        public ActionResult BindGridActiveInactive(bool showInActive)
        {
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            //Get the Entity list
            var list = userisAdmin ? _service.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), 0, CurrentDateTime.Year.ToString()) :
                _service.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), CurrentDateTime.Year.ToString());
            //list = list.Where(x => x.IsActive == !showInActive).ToList();
            list = list.Where(x => x.IsActive == showInActive).ToList();
            //Intialize the View Model i.e. DashboardIndicatorsView which is binded to Main View Index.cshtml under DashboardIndicators
            var orderByExpression = HtmlExtensions.GetOrderByExpression<ManualDashboardCustomModel>("Indicators");
            var data = HtmlExtensions.OrderByDir<ManualDashboardCustomModel>(list, "ASC", orderByExpression);
            //Pass the View Model in ActionResult to View DashboardIndicators
            return PartialView(PartialViews.ManualDashboardList, data);
        }

        /// <summary>
        /// Gets the ownership list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOwnershipList()
        {
            var list = new List<SelectListItem>();
            var olist = _service.GetOwnershipList(Helpers.GetSysAdminCorporateID());
            if (olist.Any())
                list.AddRange(olist.Select(it => new SelectListItem
                {
                    Text = it,
                    Value = it
                }));
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the indicators list.
        /// </summary>
        /// <param name="ownership">The ownership.</param>
        /// <returns></returns>
        public ActionResult GetIndicatorsList(string ownership = "")
        {
            var list = new List<SelectListItem>();
            var iList = _service.GetIndicatorsList(Helpers.GetSysAdminCorporateID(), ownership);
            if (iList.Any())
            {
                list.AddRange(iList.Select(i => new SelectListItem
                {
                    Text = string.Format("{0} - {1}", i.Dashboard, i.IndicatorNumber),
                    Value = i.IndicatorNumber
                }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Gets the indicators list.
        /// </summary>
        /// <param name="ownership">The ownership.</param>
        /// <returns></returns>
        public ActionResult GetDashboardIndicatorsList(string ownership = "")
        {
            var list = new List<SelectListItem>();
            var iList = _service.GetIndicatorsList(Helpers.GetSysAdminCorporateID(), ownership);
            if (iList.Any())
            {
                list.AddRange(iList.Select(i => new SelectListItem
                {
                    Text = string.Format("{0} ({1})", i.Dashboard, i.IndicatorNumber),
                    Value = i.IndicatorNumber
                }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Gets the years list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetYearsList()
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = Convert.ToString(CurrentDateTime.Year),
                    Value = Convert.ToString(CurrentDateTime.Year)
                },
                new SelectListItem
                {
                    Text = Convert.ToString(CurrentDateTime.Year - 1),
                    Value = Convert.ToString(CurrentDateTime.Year - 1)
                }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the is locked properties.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="budgetType">Type of the budget.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public IndicatorDataCheckList GetIsLockedProperties(int facilityId, int corporateId, int budgetType, int year)
        {
            var m = _iService.GetIndicatorDataCheckListSingle(facilityId, corporateId, budgetType, year);
            return m;
        }



        public ActionResult BindAllListsInManualDashboardData()
        {
            var facilityid = Convert.ToString(Helpers.GetDefaultFacilityId());
            var corporateid = Helpers.GetSysAdminCorporateID();


            #region Get Global Codes
            /*----------------Get Global Codes start here---------------------------*/

            /* Categories List:
             * 3112: BudgetType Dropdowndata
             * 4343: FormatType
             * 4344: Frequency 
             * 4345: DashboardType
             * 4346: KPICategory
             * 4347 Dashboard Sub-Category1 
             * 4352: Dashboard Sub-Category2             * 
             */
            var categories = new List<string> { "3112", "4343", "4344", "4345", "4346", "4347", "4351" };
            List<DropdownListData> list;
            var listIndicators = new List<DropdownListData>();
            var listDepartments = new List<DropdownListData>();
            var listOwnership = new List<DropdownListData>();
            var listYears = new List<DropdownListData>();
            List<DropdownListData> listFacilities;

            using (var bal = new GlobalCodeBal())
                list = bal.GetListByCategoriesRange(categories);

            /*-----------------Get Global Codes end here--------------------------*/

            #endregion


            #region Get Indicators List
            /*----------Get Indicators and Ownership data start here--------------*/
            var indicatorsObjList = _service.GetIndicatorsList(corporateid, string.Empty);
            if (indicatorsObjList.Any())
            {
                listIndicators.AddRange(indicatorsObjList.Select(item => new DropdownListData
                {
                    Text = item.Dashboard,
                    Value = item.IndicatorNumber
                }));
            }
            var olist = _service.GetOwnershipList(corporateid);
            listOwnership.AddRange(olist.Select(item => new DropdownListData
            {
                Text = item,
                Value = item
            }));
            /*----------Get Indicators and Ownership data end here--------------*/

            #endregion


            #region Get Departments Data
            /*-----------Get Departments Data Start here----------------------*/
            var deps = _fsService.GetFacilityDepartments(corporateid, facilityid);
            if (deps.Count > 0)
            {
                listDepartments.AddRange(
                    deps.Where(x => !string.IsNullOrEmpty(x.ExternalValue1))
                        .Select(item => new DropdownListData
                        {
                            //Text = item.ExternalValue1,
                            Value = Convert.ToString(item.ExternalValue1),
                            Text =
                                Convert.ToString(item.ExternalValue1) + @" (Department Name :" +
                                item.FacilityStructureName + @" )",
                        }));
            }

            /*-----------Get Departments Data End here----------------------*/

            #endregion


            #region Get Facilities List
            /*-----------Get Facilities Data start here----------------------*/
            using (var fBal = new FacilityBal())
                listFacilities = fBal.GetFacilitiesForDashboards(Convert.ToInt32(facilityid), corporateid,
                              Helpers.GetLoggedInUserIsAdmin());
            /*-----------Get Facilities Data end here----------------------*/

            #endregion


            #region Get Years List
            /*--------------------Get Years List start here----------------------*/
            var currentYear = Convert.ToString(CurrentDateTime.Year);
            var lastYear = Convert.ToString(CurrentDateTime.Year - 1);
            listYears = new List<DropdownListData>
            {
                new DropdownListData
                {
                    Text = currentYear,
                    Value = currentYear
                },
                new DropdownListData
                {
                    Text = lastYear,
                    Value = lastYear
                }
            };
            /*--------------------Get Years List end here----------------------*/

            #endregion

            var jsonData = new
            {
                listBudgetType = list.Where(g => g.ExternalValue1.Equals("3112")).ToList(),
                listFormatType = list.Where(g => g.ExternalValue1.Equals("4343")).ToList(),
                listFrequency = list.Where(g => g.ExternalValue1.Equals("4344")).ToList(),
                listDashboardType = list.Where(g => g.ExternalValue1.Equals("4345")).ToList(),
                listKPICategory = list.Where(g => g.ExternalValue1.Equals("4346")).ToList(),
                listSubCat1 = list.Where(g => g.ExternalValue1.Equals("4347")).ToList(),
                listSubCat2 = list.Where(g => g.ExternalValue1.Equals("4351")).ToList(),
                listIndicators,
                listFacilities,
                listDepartments,
                listOwnership,
                listYears
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RebindGriActiveInActive(int facilityId, int year, int indicator, string owner, bool showInActive)
        {
            var list = _service.RebindManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), facilityId, year, indicator, owner);
            list = list.Where(x => x.IsActive == showInActive).ToList();
            return PartialView(PartialViews.ManualDashboardList, list);
        }



        public ActionResult SaveManualDashboard(ManualDashboard model)
        {
            var result = string.Empty;
            if (model != null)
            {
                model.CorporateId = Helpers.GetSysAdminCorporateID();
                model.Defination = string.Empty;

                result = _service.SaveIndicatorsPlusData(model);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
