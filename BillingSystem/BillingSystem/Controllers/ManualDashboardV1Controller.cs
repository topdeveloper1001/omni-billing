using System;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Threading.Tasks;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class ManualDashboardV1Controller : BaseController
    {
        private readonly IPatientInfoService _piService;
        private readonly IUsersService _uService;
        private readonly IDashboardIndicatorDataService _diService;

        public ManualDashboardV1Controller(IPatientInfoService piService, IUsersService uService, IDashboardIndicatorDataService diService)
        {
            _piService = piService;
            _uService = uService;
            _diService = diService;
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
                var facilitybal = new FacilityService();
                var corporateFacilitydetail = facilitybal.GetFacilityById(loggedinfacilityId);
                var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID != 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();

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
                var facilitybal = new FacilityService();
                var corporateFacilitydetail = facilitybal.GetFacilityById(loggedinfacilityId);
                var facilityid = corporateFacilitydetail != null && corporateFacilitydetail.LoggedInID != 0
                    ? loggedinfacilityId
                     : Helpers.GetFacilityIdNextDefaultCororateFacility();

                //Get the Entity list
                list = bal.GetManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(),
                        facilityid, Convert.ToString(CurrentDateTime.Year));
                list = list.Where(x => x.IsActive == true).ToList();
                var orderByExpression = HtmlExtensions.GetOrderByExpression<ManualDashboardCustomModel>("Indicators");
                list = HtmlExtensions.OrderByDir<ManualDashboardCustomModel>(list, "ASC", orderByExpression);

            }
            return PartialView(PartialViews.ManualDashboardList, list);
        }

        /// <summary>
        /// Add New or Update the ManualDashboard based on if we pass the ManualDashboard ID in the ManualDashboardViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of ManualDashboard row
        /// </returns>
        public ActionResult SaveManualDashboard(ManualDashboard model)
        {
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<ManualDashboardCustomModel>();
            var facilityid = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            //Check if Model is not null 
            if (model != null)
            {
                using (var transScope = new TransactionScope())
                {
                    using (var bal = new ManualDashboardBal())
                    {
                        model.CorporateId = corporateid;
                        model.CreatedBy = userId;
                        model.CreatedDate = currentDate;

                        using (var b = new BaseBal())
                        {
                            b.SaveIndicators(new DashboardIndicators
                            {
                                FacilityId = model.FacilityId,
                                CorporateId = corporateid,//Upadate by Krishna on 17072015
                                Description = model.OtherDescription,
                                OwnerShip = model.OwnerShip,
                                IndicatorNumber = Convert.ToString(model.Indicators),
                                Defination = model.Defination,
                            });
                        }


                        bal.UpdateIndicatorV1(model);

                        //Add by shashank to check the Special case for the Indicator i.e. is the Target/Budget is static for indicator 
                        //.... Should only Call for Dashboard type = Budget (Externalvalue1='1')
                        //if (indicatorData != null && !string.IsNullOrEmpty(indicatorData.IndicatorNumber) && indicatorData.ExternalValue1 == "1")
                        //{
                        //    using (var ibal = new DashboardIndicatorDataBal())
                        //        ibal.SetStaticBudgetTarget(indicatorData);
                        //}

                        ////Added for saving in the table ManualDashboard
                        //if (indicatorData != null && !string.IsNullOrEmpty(indicatorData.IndicatorNumber))
                        //{
                        //    using (var ibal = new DashboardIndicatorDataBal())
                        //        ibal.UpdateIndicatorsDataInManualDashboard(indicatorData);
                        //}

                        list = userisAdmin
                            ? bal.GetManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(),
                                Convert.ToInt32(model.FacilityId), Convert.ToString(CurrentDateTime.Year))
                                : bal.GetManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(),
                                Helpers.GetDefaultFacilityId(), Convert.ToString(CurrentDateTime.Year));
                        transScope.Complete();
                    }
                }
            }
            //Pass the ActionResult with List of ManualDashboardViewModel object to Partial View ManualDashboardList
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
        /// <returns></returns>
        public JsonResult GetManualDashboardDetails(int facilityId, int corporateId, string year, string indicatorNumber, string budgetType)
        {
            using (var bal = new ManualDashboardBal())
            {
                //Call the AddManualDashboard Method to Add / Update current ManualDashboard
                var current = bal.GetManualDashboardDataByIndicatorNumberV1(corporateId, facilityId,
                    year, indicatorNumber, budgetType);

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
        public ActionResult DeleteManualDashboardDetails(int facilityId, int corporateId, string year, string indicatorNumber, string budgetType)
        {
            var indicatorsDatalst = _diService.DeleteManualDashboardDetails(corporateId, facilityId,
                     indicatorNumber, budgetType, year, "", "");

            using (var manualDashboardData = new ManualDashboardBal())
            {
                var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
                var list = userisAdmin
                            ? manualDashboardData.GetManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(),
                                0, Convert.ToString(CurrentDateTime.Year))
                                : manualDashboardData.GetManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(),
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

                var list = bal.GetManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(),
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
                var list = bal.RebindManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(), facilityId, year, indicator, owner);
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
                var exportList = bal.RebindManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(), facilityId,
                    year,
                    indicator, owner);

                foreach (var item in exportList)
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
                    ? bal.GetManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(), 0,
                        Convert.ToString(CurrentDateTime.Year))
                    : bal.GetManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(),
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
            var cookie = new HttpCookie("Downloaded", "True");
            Response.Cookies.Add(cookie);

            var corporateId = Helpers.GetSysAdminCorporateID();
            int expiryDays;
            using (var gBal = new GlobalCodeService())
            {
                var result = gBal.GetIndicatorSettingsByCorporateId(Convert.ToString(corporateId));
                expiryDays = !string.IsNullOrEmpty(result.GlobalCodeName)
                    ? Convert.ToInt32(result.GlobalCodeName)
                    : 90;
            }

            using (var manualDashboardBal = new ManualDashboardBal())
            {
                var expireDate = CurrentDateTime.AddDays(expiryDays);
                var manualDashboardController = manualDashboardBal.GetUserToken(Helpers.GetLoggedInUsername(), expireDate);
                SendUserTokenAndSave(manualDashboardController.TokenNumber, expireDate).Wait();
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
        private async Task<bool> SendUserTokenAndSave(string usertoken, DateTime? expiryDate)
        {
            var msgBody = ResourceKeyValues.GetFileText("usertokentoaccess");
            Users userCm = null;
            var currentDate = Helpers.GetInvariantCultureDateTime();
            userCm = _uService.GetUserById(Convert.ToInt32(Helpers.GetLoggedInUserId()));
            var facilityname = _piService.GetFacilityNameByFacilityId(Convert.ToInt32(Helpers.GetDefaultFacilityId()));
            if (!string.IsNullOrEmpty(msgBody) && userCm != null)
            {
                userCm.UserToken = usertoken;
                msgBody = msgBody.Replace("{User}", userCm.UserName)
                   .Replace("{Facility-Name}", facilityname).Replace("{CodeValue}", usertoken).Replace("{TokenGeneratedon}", currentDate.ToShortDateString()).
                   Replace("{TokenExpireOn}", expiryDate.Value.ToShortDateString());
            }
            var emailInfo = new EmailInfo
            {
                VerificationTokenId = "",
                PatientId = 0,
                Email = userCm.Email,
                Subject = ResourceKeyValues.GetKeyValue("usertokenemailsubject"),
                VerificationLink = "",
                MessageBody = msgBody
            };
            var status = await MailHelper.SendEmailAsync(emailInfo);
            if (status)
                userCm.UserToken = usertoken;
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
                    ? bal.RebindManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(),
                        Convert.ToInt32(facilityId), year, indicator, owner)
                    : bal.RebindManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(),
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
                var list = userisAdmin ? bal.GetManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(), 0, CurrentDateTime.Year.ToString()) :
                    bal.GetManualIndicatorDashboardDataListV1(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), CurrentDateTime.Year.ToString());
                list = list.Where(x => x.IsActive == !showInActive).ToList();
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
            using (var bal = new IndicatorDataCheckListService())
            {
                var ind = bal.GetIndicatorDataCheckListSingle(facilityId, corporateId, budgetType, year);
                return ind;
            }
        }
    }
}
