
namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Service Code Controller
    /// </summary>
    public class ServiceCodeController : BaseController
    {
        private readonly IServiceCodeService _service;
        private readonly IGlobalCodeCategoryService _gcService;

        public ServiceCodeController(IServiceCodeService service, IGlobalCodeCategoryService gcService)
        {
            _service = service;
            _gcService = gcService;
        }


        #region Public Methods and Operators

        /// <summary>
        /// The bind service code list new.
        /// </summary>
        /// <param name="blockNumber">The block number.</param>
        /// <param name="tn">The tn.</param>
        /// <param name="showInActive">if set to <c>true</c> [show in active].</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindServiceCodeListNew(string blockNumber, string tn, bool showInActive)
        {
            tn = string.IsNullOrEmpty(tn) ? Helpers.DefaultServiceCodeTableNumber : tn;

            int takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);

            // Get the facilities list
            var serviceCodeList =
                _service.GetServiceCodesActiveInActive(showInActive, tn).OrderByDescending(f => f.ServiceCodeId).Take(takeValue).ToList();
            var serviceCodeView = new ServiceCodeViewModel
            {
                ServiceCodeList = serviceCodeList,
                CurrentServiceCode = new ServiceCode(),
                UserId = Helpers.GetLoggedInUserId()
            };

            // Pass the ActionResult with List of ServiceCodeViewModel object to Partial View ServiceCodeList
            return this.PartialView(PartialViews.ServiceCodeList, serviceCodeView);
        }

        /// <summary>
        /// Delete the current ServiceCode based on the ServiceCode ID passed in the SharedViewModel
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteServiceCode(CommonModel model)
        {
            // Get ServiceCode model object by current ServiceCode ID
            ServiceCode currentServiceCode = _service.GetServiceCodeById(Convert.ToInt32(model.Id));

            // Check If ServiceCode model is not null
            if (currentServiceCode != null)
            {
                currentServiceCode.IsDeleted = true;
                currentServiceCode.DeletedBy = Helpers.GetLoggedInUserId();
                currentServiceCode.DeletedDate = Helpers.GetInvariantCultureDateTime();
                currentServiceCode.IsActive = true;

                // Update Operation of current ServiceCode
                int result = _service.AddUpdateServiceCode(currentServiceCode, Helpers.DefaultServiceCodeTableNumber);

                // return deleted ID of current ServiceCode as Json Result to the Ajax Call.
                return this.Json(result);
            }

            // Return the Json result as Action Result back JSON Call Success
            return this.Json(null);
        }

        /// <summary>
        /// Exports the service code to excel.
        /// </summary>
        /// <param name="searchText">
        /// The search Text.
        /// </param>
        /// <param name="tableNumber"></param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ExportServiceCodeToExcel(string searchText, string tableNumber)
        {
            var workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("ServiceCodeExcel");
            workbook.CreateDataFormat();
            sheet.CreateFreezePane(0, 1, 0, 1);
            sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:L1"));

            // Add header labels
            int rowIndex = 0;
            IRow row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("Service Code Table Number");
            row.CreateCell(1).SetCellValue("Service Code Table Description");
            row.CreateCell(2).SetCellValue("Service Code Value");
            row.CreateCell(3).SetCellValue("Service Code Description");
            row.CreateCell(4).SetCellValue("Service Code Price");
            row.CreateCell(5).SetCellValue("Service Code Effective Date");
            row.CreateCell(6).SetCellValue("Service Expiry Date");
            row.CreateCell(7).SetCellValue("Service Code Basic Application Rule");
            row.CreateCell(8).SetCellValue("Service Code Other Application Rule");
            row.CreateCell(9).SetCellValue("Service Code Service Code Main");
            row.CreateCell(10).SetCellValue("Service Service Code Sub");
            rowIndex++;
            // Get the facilities list
            var objServiceCodesData = searchText != null
                                                        ? _service.ExportServiceCodes(searchText, tableNumber)
                                                        : _service.GetServiceCodes(Helpers.DefaultServiceCodeTableNumber);
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

            foreach (ServiceCode item in objServiceCodesData)
            {
                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellType(CellType.Numeric);
                row.CreateCell(0).CellStyle = cellStyle;
                row.CreateCell(0).SetCellValue(Convert.ToDouble(item.ServiceCodeTableNumber));
                row.CreateCell(1).SetCellValue(item.ServiceCodeTableDescription);
                row.CreateCell(2).SetCellValue(item.ServiceCodeValue);
                row.CreateCell(3).SetCellValue(item.ServiceCodeDescription);
                row.CreateCell(4).SetCellType(CellType.Numeric);
                row.CreateCell(4).CellStyle = cellStyle;
                row.CreateCell(4).SetCellValue(Convert.ToDouble(item.ServiceCodePrice));
                row.CreateCell(5).SetCellValue(item.ServiceCodeEffectiveDate.ToString());
                row.CreateCell(6).SetCellValue(item.ServiceExpiryDate.ToString());
                row.CreateCell(7).SetCellValue(item.ServiceCodeBasicApplicationRule);
                row.CreateCell(8).SetCellValue(item.ServiceCodeOtherApplicationRule);
                row.CreateCell(9).SetCellType(CellType.Numeric);
                row.CreateCell(9).CellStyle = cellStyle;

                // row.CreateCell(9).SetCellValue(Convert.ToDouble(item.ServiceCodeServiceCodeMain));
                row.CreateCell(9).SetCellValue(item.ServiceCodeServiceCodeMain);
                row.CreateCell(10).SetCellType(CellType.Numeric);
                row.CreateCell(10).CellStyle = cellStyle;

                // row.CreateCell(10).SetCellValue(Convert.ToDouble(item.ServiceServiceCodeSub));
                row.CreateCell(10).SetCellValue(item.ServiceServiceCodeSub);
                rowIndex++;

            }

            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                this.Response.Cookies.Add(cookie);
                workbook.Write(exportData);
                string saveAsFileName = string.Format("ServiceCodeExcel-{0:d}.xls", CurrentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }

        /// <summary>
        /// Gets the service code.
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult GetServiceCode(string id)
        {

            ServiceCode current = _service.GetServiceCodeById(Convert.ToInt32(id));
            var jsonResult =
                new
                {
                    current.ServiceCodeId,
                    current.ServiceCodeValue,
                    current.ServiceCodeDescription,
                    current.ServiceCodePrice,
                    ServiceCodeEffectiveDate = current.ServiceCodeEffectiveDate.GetShortDateString3(),
                    ServiceExpiryDate = current.ServiceExpiryDate.GetShortDateString3(),
                    current.ServiceCodeBasicApplicationRule,
                    current.ServiceCodeOtherApplicationRule,
                    current.ServiceCodeServiceCodeMain,
                    current.ServiceServiceCodeSub,
                    current.CanOverRide
                };
            return this.Json(jsonResult, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Gets the service main categories.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetServiceMainCategories()
        {
            int startRange = Convert.ToInt32(GlobalCodeCategoryValue.ServiceCodeStartRange);
            int finishRange = Convert.ToInt32(GlobalCodeCategoryValue.ServiceCodeFinishRange);

            var list = _gcService.GetGlobalCodeCategoriesRange(startRange, finishRange);
            return this.Json(list);
        }

        /// <summary>
        /// Rebinds the bind service code list.
        /// </summary>
        /// <param name="blockNumber">The block number.</param>
        /// <param name="tableNumber">The table number.</param>
        /// <returns>
        /// The <see cref="JsonResult" />.
        /// </returns>
        public JsonResult RebindBindServiceCodeList(int blockNumber, string tableNumber)
        {
            int recordCount = Helpers.DefaultRecordCount;

            List<ServiceCodeCustomModel> list = _service.GetServiceCodesListOnDemandCustom(blockNumber, recordCount, tableNumber);
            var jsonResult =
                new { list, NoMoreData = list.Count < recordCount, UserId = Helpers.GetLoggedInUserId() };
            return this.Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the service code.
        /// </summary>
        /// <param name="model">
        /// The service code model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult SaveServiceCode(ServiceCode model)
        {
            // Initialize the newId variable 
            int newId = -1;

            // Check if ServiceCodeViewModel 
            if (model != null)
            {
                if (model.ServiceCodeId > 0)
                {
                    model.ModifiedBy = Helpers.GetLoggedInUserId();
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }

                // Call the AddServiceCode Method to Add / Update current ServiceCode
                newId = _service.AddUpdateServiceCode(model, Helpers.DefaultServiceCodeTableNumber);

            }

            return this.Json(newId);
        }

        /// <summary>
        /// Services the code.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ServiceCode()
        { 
            // Intialize the View Model i.e. ServiceCodeView which is binded to Main View Index.cshtml under ServiceCode
            var serviceCodeView = new ServiceCodeViewModel
            {
                ServiceCodeList = _service.GetServiceCodesListOnDemandCustom(1, Helpers.DefaultRecordCount, Helpers.DefaultServiceCodeTableNumber),
                CurrentServiceCode = new ServiceCode(),
                UserId = Helpers.GetLoggedInUserId()
            };

            // Pass the View Model in ActionResult to View ServiceCode
            return View(serviceCodeView);
        }

        public ActionResult GetServiceCodesList()
        {
             return this.Json(_service.GetServiceCodesList(Helpers.DefaultServiceCodeTableNumber), JsonRequestBehavior.AllowGet);
        }



        #endregion
    }
}