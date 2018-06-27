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
        private readonly IPatientInfoService _piService;
        private readonly IUsersService _uService;

        public ManualDashboardController(IFacilityStructureService fsService, IPatientInfoService piService, IUsersService uService)
        {
            _fsService = fsService;
            _piService = piService;
            _uService = uService;
        }

        /// <summary>
        /// Get the details of the ManualDashboard View in the Model ManualDashboard such as ManualDashboardList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model ManualDashboard to be passed to View ManualDashboard
        /// </returns>
        public ActionResult Index()
        {
            //Initialize the ManualDashboard BAL object
            using (var bal = new ManualDashboardBal())
            {
                var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
                var loggedinfacilityId = Helpers.GetDefaultFacilityId();
                var facilitybal = new FacilityBal();
                var corporateFacilitydetail = facilitybal.GetFacilityById(loggedinfacilityId);
                var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID != 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();
                //Get the Entity list
                /*var list = bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                        facilityid, Convert.ToString(DateTime.Now.Year));
                list = list.Where(x => x.IsActive == true).ToList();
                var orderByExpression = HtmlExtensions.GetOrderByExpression<ManualDashboardCustomModel>("Indicators");*/
                //var data = HtmlExtensions.OrderByDir<ManualDashboardCustomModel>(list, "ASC", orderByExpression);
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
        }

        /// <summary>
        /// Gets the manual dashboard list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetManualDashboardList()
        {
            List<ManualDashboardCustomModel> list;
            using (var bal = new ManualDashboardBal())
            {
                var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
                var loggedinfacilityId = Helpers.GetDefaultFacilityId();
                var facilitybal = new FacilityBal();
                var corporateFacilitydetail = facilitybal.GetFacilityById(loggedinfacilityId);
                var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID == null
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();
                //Get the Entity list
                list = bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                        facilityid, Convert.ToString(CurrentDateTime.Year));
                list = list.Where(x => x.IsActive == true).ToList();
                var orderByExpression = HtmlExtensions.GetOrderByExpression<ManualDashboardCustomModel>("Indicators");
                list = HtmlExtensions.OrderByDir<ManualDashboardCustomModel>(list, "ASC", orderByExpression);

            }
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
            using (var bal = new ManualDashboardBal())
            {
                //Call the AddManualDashboard Method to Add / Update current ManualDashboard
                var current = bal.GetManualDashboardDataByIndicatorNumber(corporateId, facilityId,
                    year, indicatorNumber, budgetType, subCategory1, subCategory2);

                if (current != null)
                {
                    var objIsLocked = GetIsLockedProperties(Convert.ToInt32(current.FacilityId),
                        Convert.ToInt32(current.CorporateId), Convert.ToInt32(current.BudgetType),
                        Convert.ToInt32(current.Year));
                    var jsonData = new { current, locked = objIsLocked };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }

                //Pass the ActionResult with the current ManualDashboardViewModel object as model to PartialView ManualDashboardAddEdit
                return Json(new ManualDashboardCustomModel(), JsonRequestBehavior.AllowGet);
            }
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
            using (var bal = new DashboardIndicatorDataBal())
            {
                //Call the AddManualDashboard Method to Add / Update current ManualDashboard
                var indicatorsDatalst = bal.DeleteManualDashboardDetails(corporateId, facilityId,
                     indicatorNumber, budgetType, year, subCategory1, subCategory2);
                //Pass the ActionResult with the current ManualDashboardViewModel object as model to PartialView ManualDashboardAddEdit
            }
            using (var manualDashboardData = new ManualDashboardBal())
            {
                var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
                var list = userisAdmin
                            ? manualDashboardData.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                                0, Convert.ToString(CurrentDateTime.Year))
                                : manualDashboardData.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                                Helpers.GetDefaultFacilityId(), Convert.ToString(CurrentDateTime.Year));
                return PartialView(PartialViews.ManualDashboardList, list);
            }
        }

        /// <summary>
        /// Delete the current ManualDashboard based on the ManualDashboard ID passed in the ManualDashboardModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteManualDashboard(int id)
        {
            using (var bal = new ManualDashboardBal())
            {
                //Get ManualDashboard model object by current ManualDashboard ID
                var model = bal.GetManualDashboardById(id);
                var userId = Helpers.GetLoggedInUserId();
                var list = new List<ManualDashboardCustomModel>();
                var currentDate = Helpers.GetInvariantCultureDateTime();

                //Check If ManualDashboard model is not null
                if (model != null)
                {
                    model.CorporateId = Helpers.GetSysAdminCorporateID();
                    model.IsActive = false;
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDate;
                    //Update Operation of current ManualDashboard
                    list = bal.SaveManualDashboard(model);
                    //return deleted ID of current ManualDashboard as Json Result to the Ajax Call.
                }
                return PartialView(PartialViews.ManualDashboardList, list);
            }
            //Pass the ActionResult with List of ManualDashboardViewModel object to Partial View ManualDashboardList
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
            using (var bal = new ManualDashboardBal())
            {
                //Call the AddManualDashboard Method to Add / Update current ManualDashboard
                if (id != 0)
                    return Json(true, JsonRequestBehavior.AllowGet);

                var list = bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                    Helpers.GetDefaultFacilityId(), Convert.ToString(CurrentDateTime.Year));
                var selectedValue =
                    !list.Any(x => x.Indicators == indicatorNumber && x.BudgetType == budgetType && x.Year == year);
                return Json(selectedValue, JsonRequestBehavior.AllowGet);
            }
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
            using (var bal = new ManualDashboardBal())
            {
                var list = bal.RebindManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), facilityId, year, indicator, owner);
                return PartialView(PartialViews.ManualDashboardList, list);
            }
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

            using (var bal = new ManualDashboardBal())
            {
                var ExportList = bal.RebindManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), facilityId, year,
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
            using (var bal = new ManualDashboardBal())
            {
                var list = facilityid == 0
                    ? bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), 0,
                        Convert.ToString(CurrentDateTime.Year))
                    : bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                        facilityid, Convert.ToString(CurrentDateTime.Year));
                return list;
            }
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

            using (var manualDashboardBal = new ManualDashboardBal())
            {
                var expireDate = CurrentDateTime.AddDays(expiryDays);
                var ut = manualDashboardBal.GetUserToken(Helpers.GetLoggedInUsername(), expireDate);

                var userId = Helpers.GetLoggedInUserId();
                var facId = Helpers.GetDefaultFacilityId();
                BackgroundJob.Enqueue(() => SendUserTokenAndSave(ut.TokenNumber, expireDate, userId, facId).Wait());
            }
            var saveAsFileName = string.Format("ManualDashboardDataTemplate-{0:d}.xlsm", CurrentDateTime)
                .Replace("/", "-");
            return File(serverPath, "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
        }

        /// <summary>
        /// Sends the user token and save.
        /// </summary>
        /// <param name="usertoken">The usertoken.</param>
        /// <returns></returns>
        public async Task<bool> SendUserTokenAndSave(string usertoken, DateTime? expiryDate, int loggedInUserId, int facilityId)
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
            using (var bal = new ManualDashboardBal())
            {
                var userisAdmin = Helpers.GetLoggedInUserIsAdmin();

                /*
                 * Changes by: Amit Jain
                 * On: 16072015
                 * Purpose: Binding has been changed according to those filters.
                 */
                //Get the Entity list
                //var list = userisAdmin
                //    ? bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                //        Convert.ToInt32(facilityId), Convert.ToString(DateTime.Now.Year))
                //    : bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                //        Helpers.GetDefaultFacilityId(), Convert.ToString(DateTime.Now.Year));

                var list = userisAdmin
                    ? bal.RebindManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                        Convert.ToInt32(facilityId), year, indicator, owner)
                    : bal.RebindManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
                        Helpers.GetDefaultFacilityId(), year, indicator, owner);

                //Intialize the View Model i.e. DashboardIndicatorsView which is binded to Main View Index.cshtml under DashboardIndicators
                var orderByExpression = HtmlExtensions.GetOrderByExpression<ManualDashboardCustomModel>(sort);
                var data = HtmlExtensions.OrderByDir(list, sortdir, orderByExpression);

                //Pass the View Model in ActionResult to View DashboardIndicators
                return PartialView(PartialViews.ManualDashboardList, data);
            }
        }

        /// <summary>
        /// Binds the indicators active inactive.
        /// </summary>
        /// <param name="showInActive">The show in active.</param>
        /// <returns></returns>
        public ActionResult BindGridActiveInactive(bool showInActive)
        {
            using (var bal = new ManualDashboardBal())
            {
                var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
                //Get the Entity list
                var list = userisAdmin ? bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), 0, CurrentDateTime.Year.ToString()) :
                    bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), CurrentDateTime.Year.ToString());
                //list = list.Where(x => x.IsActive == !showInActive).ToList();
                list = list.Where(x => x.IsActive == showInActive).ToList();
                //Intialize the View Model i.e. DashboardIndicatorsView which is binded to Main View Index.cshtml under DashboardIndicators
                var orderByExpression = HtmlExtensions.GetOrderByExpression<ManualDashboardCustomModel>("Indicators");
                var data = HtmlExtensions.OrderByDir<ManualDashboardCustomModel>(list, "ASC", orderByExpression);
                //Pass the View Model in ActionResult to View DashboardIndicators
                return PartialView(PartialViews.ManualDashboardList, data);
            }
        }

        /// <summary>
        /// Gets the ownership list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOwnershipList()
        {
            var list = new List<SelectListItem>();
            using (var bal = new ManualDashboardBal())
            {
                var olist = bal.GetOwnershipList(Helpers.GetSysAdminCorporateID());
                if (olist.Any())
                    list.AddRange(olist.Select(it => new SelectListItem
                    {
                        Text = it,
                        Value = it
                    }));
            }
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
            using (var bal = new ManualDashboardBal())
            {
                var iList = bal.GetIndicatorsList(Helpers.GetSysAdminCorporateID(), ownership);
                if (iList.Any())
                {
                    list.AddRange(iList.Select(i => new SelectListItem
                    {
                        Text = string.Format("{0} - {1}", i.Dashboard, i.IndicatorNumber),
                        Value = i.IndicatorNumber
                    }));
                }
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
            using (var bal = new ManualDashboardBal())
            {
                var iList = bal.GetIndicatorsList(Helpers.GetSysAdminCorporateID(), ownership);
                if (iList.Any())
                {
                    list.AddRange(iList.Select(i => new SelectListItem
                    {
                        Text = string.Format("{0} ({1})", i.Dashboard, i.IndicatorNumber),
                        Value = i.IndicatorNumber
                    }));
                }
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
            using (var bal = new IndicatorDataCheckListBal())
            {
                var ind = bal.GetIndicatorDataCheckListSingle(facilityId, corporateId, budgetType, year);
                return ind;
            }
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
            using (var dBal = new ManualDashboardBal())
            {
                var indicatorsObjList = dBal.GetIndicatorsList(corporateid, string.Empty);
                if (indicatorsObjList.Any())
                {
                    listIndicators.AddRange(indicatorsObjList.Select(item => new DropdownListData
                    {
                        Text = item.Dashboard,
                        Value = item.IndicatorNumber
                    }));
                }
                var olist = dBal.GetOwnershipList(corporateid);
                listOwnership.AddRange(olist.Select(item => new DropdownListData
                {
                    Text = item,
                    Value = item
                }));
            }
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
            using (var bal = new ManualDashboardBal())
            {
                var list = bal.RebindManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(), facilityId, year, indicator, owner);
                list = list.Where(x => x.IsActive == showInActive).ToList();
                return PartialView(PartialViews.ManualDashboardList, list);
            }
        }



        /*
         * Below is the Old Code for saving Manual Dashboard Data 
         On: 06 June, 2016 
         WHY: To avoid the 12 times iteration of save data*/

        //public ActionResult SaveManualDashboard(ManualDashboard model)
        //{
        //    //Initialize the newId variable 
        //    var userId = Helpers.GetLoggedInUserId();
        //    var currentDate = Helpers.GetInvariantCultureDateTime();
        //    //var list = new List<ManualDashboardCustomModel>();
        //    var corporateid = Helpers.GetSysAdminCorporateID();
        //    //var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
        //    //Check if Model is not null 
        //    if (model != null)
        //    {
        //        var facilityid = model.FacilityId; //Helpers.GetDefaultFacilityId();
        //        //using (var transScope = new TransactionScope())
        //        //{
        //        using (var bal = new ManualDashboardBal())
        //        {
        //            model.CorporateId = corporateid;
        //            model.CreatedBy = userId;
        //            model.CreatedDate = currentDate;

        //            //Call the AddManualDashboard Method to Add / Update current ManualDashboard
        //            var indicatorsData = bal.GetIndicatorsDataForIndicatorNumber(
        //                Convert.ToInt32(model.CorporateId), Convert.ToInt32(model.FacilityId),
        //                Convert.ToString(model.Year),
        //                Convert.ToString(model.Indicators), Convert.ToString(model.BudgetType), Convert.ToString(model.SubCategory1), Convert.ToString(model.SubCategory2));
        //            indicatorsData = indicatorsData.OrderBy(x => x.Month).ToList();
        //            DashboardIndicatorData indicatorData = null;

        //            for (var i = 1; i <= 12; i++)
        //            {
        //                var statisticData = i == 1
        //                    ? model.M1
        //                    : i == 2
        //                        ? model.M2
        //                        : i == 3
        //                            ? model.M3
        //                            : i == 4
        //                                ? model.M4
        //                                : i == 5
        //                                    ? model.M5
        //                                    : i == 6
        //                                        ? model.M6
        //                                        : i == 7
        //                                            ? model.M7
        //                                            : i == 8
        //                                                ? model.M8
        //                                                : i == 9
        //                                                    ? model.M9
        //                                                    : i == 10
        //                                                        ? model.M10
        //                                                        : i == 11
        //                                                            ? model.M11
        //                                                            : i == 12 ? model.M12 : "0.00";

        //                indicatorData = new DashboardIndicatorData
        //                {
        //                    IndicatorNumber = Convert.ToString(model.Indicators),
        //                    Month = i,
        //                    CorporateId = corporateid,
        //                    FacilityId = facilityid,
        //                    DepartmentNumber = model.ExternalValue1,
        //                    IsActive = model.IsActive,
        //                    Year = Convert.ToString(model.Year),
        //                    CreatedBy = userId,
        //                    CreatedDate = currentDate,
        //                    SubCategory1 = model.SubCategory1,
        //                    SubCategory2 = model.SubCategory2,
        //                    StatisticData = statisticData,
        //                    ExternalValue1 = Convert.ToString(model.BudgetType),
        //                    ExternalValue2 = model.OtherDescription,
        //                    ExternalValue3 = "1",
        //                    IndicatorId = 1,
        //                };

        //                if (indicatorsData.Count >= i)
        //                {
        //                    indicatorData = indicatorsData[i - 1];
        //                    indicatorData.StatisticData = statisticData;
        //                    indicatorData.IsActive = model.IsActive;
        //                    indicatorData.ExternalValue2 = model.OtherDescription;
        //                    indicatorData.SubCategory1 = !string.IsNullOrEmpty(model.SubCategory1) ? model.SubCategory1 : "0";
        //                    indicatorData.SubCategory2 = !string.IsNullOrEmpty(model.SubCategory2) ? model.SubCategory2 : "0";
        //                    indicatorData.DepartmentNumber = model.ExternalValue1;
        //                }

        //                using (var indicatorDataBal = new DashboardIndicatorDataBal())
        //                {
        //                    indicatorData.CreatedBy = Helpers.GetLoggedInUserId();
        //                    indicatorDataBal.SaveDashboardIndicatorDataCustom(indicatorData);
        //                    indicatorDataBal.SaveIndicators(new DashboardIndicators
        //                    {
        //                        FacilityId = model.FacilityId,
        //                        CorporateId = corporateid,//Upadate by Krishna on 17072015
        //                        Description = model.OtherDescription,
        //                        OwnerShip = model.OwnerShip,
        //                        IndicatorNumber = Convert.ToString(model.Indicators),
        //                        Defination = model.Defination,
        //                    });
        //                }
        //            }

        //            //Add by shashank to check the Special case for the Indicator i.e. is the Target/Budget is static for indicator 
        //            //.... Should only Call for Dashboard type = Budget (Externalvalue1='1')
        //            if (indicatorData != null && !string.IsNullOrEmpty(indicatorData.IndicatorNumber) && indicatorData.ExternalValue1 == "1")
        //            {
        //                using (var ibal = new DashboardIndicatorDataBal())
        //                    ibal.SetStaticBudgetTarget(indicatorData);
        //            }

        //            //Added for saving in the table ManualDashboard
        //            if (indicatorData != null && !string.IsNullOrEmpty(indicatorData.IndicatorNumber))
        //            {
        //                using (var ibal = new DashboardIndicatorDataBal())
        //                    ibal.UpdateIndicatorsDataInManualDashboard(indicatorData);
        //            }

        //            //list = userisAdmin
        //            //    ? bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
        //            //        Convert.ToInt32(model.FacilityId), Convert.ToString(DateTime.Now.Year))
        //            //        : bal.GetManualIndicatorDashboardDataList(Helpers.GetSysAdminCorporateID(),
        //            //        Helpers.GetDefaultFacilityId(), Convert.ToString(DateTime.Now.Year));
        //            //transScope.Complete();
        //        }
        //        //}
        //    }
        //    //Pass the ActionResult with List of ManualDashboardViewModel object to Partial View ManualDashboardList
        //    //return PartialView(PartialViews.ManualDashboardList, list);
        //    return Json("1", JsonRequestBehavior.AllowGet);
        //}


        //New Code




        public ActionResult SaveManualDashboard(ManualDashboard model)
        {
            var result = string.Empty;
            if (model != null)
            {
                model.CorporateId = Helpers.GetSysAdminCorporateID();
                model.Defination = string.Empty;

                using (var bal = new ManualDashboardBal())
                    result = bal.SaveIndicatorsPlusData(model);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
