using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace BillingSystem.Controllers
{
    public class DRGCodesController : BaseController
    {
        private readonly IDRGCodesService _service;

        public DRGCodesController(IDRGCodesService service)
        {
            _service = service;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the DRGCodes list
        /// </summary>
        /// <returns>
        ///     action result with the partial view containing the DRGCodes list object
        /// </returns>
        public ActionResult BindDRGCodesList()
        {
            // Get the facilities list
            List<DRGCodes> drgCodesList = _service.GetDrgCodes(Helpers.DefaultDrgTableNumber);
            var drgCodesView = new DRGCodesView
            {
                DRGCodesList = drgCodesList,
                CurrentDRGCodes = new DRGCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };

            // Pass the ActionResult with List of DRGCodesViewModel object to Partial View DRGCodesList
            return this.PartialView(PartialViews.DRGCodesList, drgCodesView);
        }

        /// <summary>
        /// The bind drg codes list new.
        /// </summary>
        /// <param name="blockNumber">
        /// The block number.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult BindDRGCodesListNew(string blockNumber, bool showInActive)
        {
            int takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);

            // Get the facilities list
            List<DRGCodes> drgCodesList =
                //_service.GetDrgCodes().OrderByDescending(f => f.DRGCodesId ).Take(takeValue).ToList();
                _service.GetActiveInactiveDrgCodes(showInActive, Helpers.DefaultDrgTableNumber).OrderByDescending(f => f.DRGCodesId).Take(takeValue).ToList();

            var drgCodesView = new DRGCodesView
            {
                DRGCodesList = drgCodesList,
                CurrentDRGCodes = new DRGCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };

            // Pass the ActionResult with List of DRGCodesViewModel object to Partial View DRGCodesList
            return this.PartialView(PartialViews.DRGCodesList, drgCodesView);
        }

        /// <summary>
        /// DRGs the codes.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DRGCodes()
        {
            // Get the facilities list
            // var drgCodesList = _service.GetDrgCodes();
            List<DRGCodes> drgCodesList = _service.GetDrgCodesListOnDemand(1, Helpers.DefaultRecordCount, Helpers.DefaultDrgTableNumber);

            // Intialize the View Model i.e. DRGCodesView which is binded to Main View Index.cshtml under DRGCodes
            var drgCodesView = new DRGCodesView
            {
                DRGCodesList = drgCodesList,
                CurrentDRGCodes = new DRGCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };

            // Pass the View Model in ActionResult to View DRGCodes
            return View(drgCodesView);
        }

        /// <summary>
        /// Delete the current DRGCodes based on the DRGCodes ID passed in the SharedViewModel
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteDRGCodes(CommonModel model)
        {
            // Get DRGCodes model object by current DRGCodes ID
            DRGCodes currentDrgCodes = _service.GetDrgCodesById(Convert.ToInt32(model.Id));

            // Check If DRGCodes model is not null
            if (currentDrgCodes != null)
            {
                currentDrgCodes.IsDeleted = true;
                currentDrgCodes.DeletedBy = Helpers.GetLoggedInUserId();
                currentDrgCodes.DeletedDate = Helpers.GetInvariantCultureDateTime();

                // Update Operation of current DRGCodes
                int result = _service.SaveDrgCode(currentDrgCodes, Helpers.DefaultDrgTableNumber);

                // return deleted ID of current DRGCodes as Json Result to the Ajax Call.
                return this.Json(result);
            }

            // Return the Json result as Action Result back JSON Call Success
            return this.Json(null);
        }

        /// <summary>
        /// The export drg codes to excel.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ExportDRGCodesToExcel(string searchText, string tableNumber)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("DRGCodesData");
            IDataFormat format = workbook.CreateDataFormat();
            sheet.CreateFreezePane(0, 1, 0, 1);
            sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:O1"));

            // Add header labels
            int rowIndex = 0;
            IRow row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("DRGCodes Id");
            row.CreateCell(1).SetCellValue("Code Table Number");
            row.CreateCell(2).SetCellValue("Code Table Description");
            row.CreateCell(3).SetCellValue("Code Numbering");
            row.CreateCell(4).SetCellValue("Code Description");
            row.CreateCell(5).SetCellValue("Code Price");
            row.CreateCell(6).SetCellValue("Code DRGWeight");
            row.CreateCell(7).SetCellValue("Code Effective Date");
            row.CreateCell(8).SetCellValue("Code Expiry Date");
            row.CreateCell(9).SetCellValue("Alos");
            row.CreateCell(10).SetCellValue("Application Rule");

            rowIndex++;

            // Get the facilities list
            List<DRGCodes> onjDrgCodesData = searchText != null
                                                 ? _service.ExportDRGCodes(searchText, tableNumber)
                                                 : _service.GetDrgCodes(Helpers.DefaultDrgTableNumber);

            // Get the facilities list
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

            foreach (DRGCodes item in onjDrgCodesData)
            {
                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellType(CellType.Numeric);
                row.CreateCell(0).CellStyle = cellStyle;
                row.CreateCell(0).SetCellValue(Convert.ToDouble(item.DRGCodesId));
                row.CreateCell(1).SetCellValue(item.CodeTableNumber);
                row.CreateCell(2).SetCellValue(item.CodeTableDescription);
                row.CreateCell(3).SetCellValue(item.CodeNumbering);

                row.CreateCell(4).SetCellValue(item.CodeDescription);

                row.CreateCell(5).CellStyle = cellStyle;
                row.CreateCell(5).SetCellValue(item.CodePrice);
                row.CreateCell(6).CellStyle = cellStyle;
                row.CreateCell(6).SetCellValue(item.CodeDRGWeight);
                row.CreateCell(7).SetCellValue(item.CodeEffectiveDate.ToString());
                row.CreateCell(8).SetCellValue(item.CodeExpiryDate.ToString());

                row.CreateCell(9).SetCellValue(item.Alos.ToString());
                row.CreateCell(10).SetCellValue(item.ApplicationRule);

                rowIndex++;
            }


            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                this.Response.Cookies.Add(cookie);
                workbook.Write(exportData);
                string saveAsFileName = string.Format("DRGCodesDataExcel-{0:d}.xls", currentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }

        /// <summary>
        /// Get the details of the current DRGCodes in the view model by ID
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetDRGCodes(DRGCodes model)
        {
            // Call the AddDRGCodes Method to Add / Update current DRGCodes
            DRGCodes currentDrgCodes = _service.GetDrgCodesById(Convert.ToInt32(model.DRGCodesId));

            // Pass the ActionResult with the current DRGCodesViewModel object as model to PartialView DRGCodesAddEdit
            return this.PartialView(PartialViews.DRGCodesAddEdit, currentDrgCodes);
        }


        /// <summary>
        /// Exports the DRG codes to excel.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <summary>
        /// Imports the DRG codes excel data.
        /// </summary>
        /// <param name="DRGFile">
        /// The DRG file.
        /// </param>
        /// <returns>
        /// </returns>
        public ActionResult ImportDRGCodesExcelData(HttpPostedFileBase DRGFile)
        {
            if (DRGFile != null)
            {
                string savedFileName = "~/UploadedExcelDocuments/" + DRGFile.FileName;
                DRGFile.SaveAs(this.Server.MapPath(savedFileName));
            }

            return this.RedirectToAction("DRGCodes");
        }

        /// <summary>
        /// The rebind bind drg codes list.
        /// </summary>
        /// <param name="blockNumber">
        /// The block number.
        /// </param>
        /// <param name="tableNumber">
        /// The table number.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult RebindBindDRGCodesList(int blockNumber, string tableNumber)
        {
            int recordCount = Helpers.DefaultRecordCount;

            List<DRGCodes> list = _service.GetDrgCodesListOnDemand(blockNumber, recordCount, Helpers.DefaultDrgTableNumber);
            var jsonResult =
                new { list, NoMoreData = list.Count < recordCount, UserId = Helpers.GetLoggedInUserId() };
            return this.Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Reset the DRGCodes View Model and pass it to DRGCodesAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ResetDRGCodesForm()
        {
            // Intialize the new object of DRGCodes ViewModel
            var drgCodesViewModel = new DRGCodes();

            // Pass the View Model as DRGCodesViewModel to PartialView DRGCodesAddEdit just to update the AddEdit partial view.
            return this.PartialView(PartialViews.DRGCodesAddEdit, drgCodesViewModel);
        }

        /// <summary>
        /// Add New or Update the DRGCodes based on if we pass the DRGCodes ID in the DRGCodesViewModel object.
        /// </summary>
        /// <param name="drgCodesModel">
        /// pass the details of DRGCodes in the view model
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of DRGCodes row
        /// </returns>
        public ActionResult SaveDrgDetails(DRGCodes drgCodesModel)
        {
            // Initialize the newId variable 
            int newId = -1;

            // Check if DRGCodesViewModel 
            if (drgCodesModel != null)
            {
                //drgCodesModel.IsActive = true;
                drgCodesModel.IsDeleted = false;
                if (drgCodesModel.DRGCodesId > 0)
                {
                    drgCodesModel.ModifiedBy = Helpers.GetLoggedInUserId();
                    drgCodesModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    drgCodesModel.CreatedBy = Helpers.GetLoggedInUserId();
                    drgCodesModel.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    //drgCodesModel.IsActive = true;
                }

                // Call the AddDRGCodes Method to Add / Update current DRGCodes
                newId = _service.SaveDrgCode(drgCodesModel, Helpers.DefaultDrgTableNumber);

            }

            return this.Json(newId);
        }

        #endregion


        public ActionResult DRGActiveInActiveCodesList(string blockNumber, bool showInActive)
        {
            int takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);

            // Get the facilities list
            List<DRGCodes> drgCodesList =
                _service.GetActiveInactiveDrgCodes(showInActive, Helpers.DefaultDrgTableNumber).OrderByDescending(f => f.DRGCodesId).Take(takeValue).ToList();
            var drgCodesView = new DRGCodesView
            {
                DRGCodesList = drgCodesList,
                CurrentDRGCodes = new DRGCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };

            // Pass the ActionResult with List of DRGCodesViewModel object to Partial View DRGCodesList
            return this.PartialView(PartialViews.DRGCodesList, drgCodesView);
        }
    }

}