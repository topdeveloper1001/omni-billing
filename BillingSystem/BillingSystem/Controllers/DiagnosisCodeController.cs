using System.Linq;
using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class DiagnosisCodeController : BaseController
    {
        private readonly IDiagnosisCodeService _service;
        private readonly IDiagnosisService _dService;

        public DiagnosisCodeController(IDiagnosisCodeService service, IDiagnosisService dService)
        {
            _service = service;
            _dService = dService;
        }


        /// <summary>
        /// Get the details of the DiagnosisCode View in the Model DiagnosisCode such as DiagnosisCodeList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model DiagnosisCode to be passed to View DiagnosisCode
        /// </returns>
        public ActionResult DiagnosisCodeMain()
        {
            //Get the Entity list
            var diagnosisCodeList = _service.GetListOnDemand(1, Helpers.DefaultRecordCount, Helpers.DefaultDiagnosisTableNumber);

            //Intialize the View Model i.e. DiagnosisCodeView which is binded to Main View Index.cshtml under DiagnosisCode
            var diagnosisCodeView = new DiagnosisCodeView
            {
                DiagnosisCodeList = diagnosisCodeList,
                CurrentDiagnosisCode = new Model.DiagnosisCode(),
                UserId = Helpers.GetLoggedInUserId()
            };

            //Pass the View Model in ActionResult to View DiagnosisCode
            return View(diagnosisCodeView);
        }

        /// <summary>
        /// Bind all the DiagnosisCode list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the DiagnosisCode list object
        /// </returns>
        [HttpPost]
        public ActionResult BindDiagnosisCodeList()
        {
            //Get the facilities list
            var diagnosisCodeList = _service.GetDiagnosisCode(Helpers.DefaultDiagnosisTableNumber);
            var diagnosisCodeView = new DiagnosisCodeView
            {
                DiagnosisCodeList = diagnosisCodeList,
                CurrentDiagnosisCode = new Model.DiagnosisCode(),
                UserId = Helpers.GetLoggedInUserId()
            };
            //Pass the ActionResult with List of DiagnosisCodeViewModel object to Partial View DiagnosisCodeList
            return PartialView(PartialViews.DiagnosisCodeList, diagnosisCodeView);

        }

        public ActionResult BindDiagnosisCodeListNew(string blockNumber, bool showInActive)
        {
            var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);

            //Get the facilities list
            var diagnosisCodeList = _service.GetDiagnosisCodeData(showInActive, Helpers.DefaultDiagnosisTableNumber).OrderByDescending(i => i.DiagnosisTableNumberId).Take(takeValue).ToList();
            var diagnosisCodeView = new DiagnosisCodeView
            {
                DiagnosisCodeList = diagnosisCodeList,
                CurrentDiagnosisCode = new Model.DiagnosisCode(),
                UserId = Helpers.GetLoggedInUserId()
            };
            //Pass the ActionResult with List of DiagnosisCodeViewModel object to Partial View DiagnosisCodeList
            return PartialView(PartialViews.DiagnosisCodeList, diagnosisCodeView);
        }

        public JsonResult RebindDiagnosisCodeList(int blockNumber, string tableNumber)
        {
            var recordCount = Helpers.DefaultRecordCount;
            var list = _service.GetListOnDemand(blockNumber, recordCount, Helpers.DefaultDiagnosisTableNumber);
            var jsonResult = new
            {
                list,
                NoMoreData = list.Count < recordCount,
                UserId = Helpers.GetLoggedInUserId()
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Add New or Update the DiagnosisCode based on if we pass the DiagnosisCode ID in the DiagnosisCodeViewModel object.
        /// </summary>
        /// <param name="DiagnosisCodeModel">pass the details of DiagnosisCode in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of DiagnosisCode row
        /// </returns>
        public ActionResult SaveDiagnosisCode(Model.DiagnosisCode DiagnosisCodeModel)
        {
            //Initialize the newId variable 
            var newId = -1;

            //Check if DiagnosisCodeViewModel 
            if (DiagnosisCodeModel != null)
            {
                if (DiagnosisCodeModel.DiagnosisTableNumberId > 0)
                {
                    DiagnosisCodeModel.CreatedBy = Helpers.GetLoggedInUserId();
                    DiagnosisCodeModel.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    DiagnosisCodeModel.ModifiedBy = Helpers.GetLoggedInUserId();
                    DiagnosisCodeModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    DiagnosisCodeModel.CreatedBy = Helpers.GetLoggedInUserId();
                    DiagnosisCodeModel.CreatedDate = Helpers.GetInvariantCultureDateTime();

                }
                //Call the AddDiagnosisCode Method to Add / Update current DiagnosisCode
                newId = _service.AddUptdateDiagnosisCode(DiagnosisCodeModel, Helpers.DefaultDiagnosisTableNumber);

            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current DiagnosisCode in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetDiagnosisCode(int id)
        {
            //Call the AddDiagnosisCode Method to Add / Update current DiagnosisCode
            var currentDiagnosisCode = _service.GetDiagnosisCodeByID(id);

            //If the view is shown in ViewMode only, then ViewBag.ViewOnly is set to true otherwise false.
            //ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);

            //Pass the ActionResult with the current DiagnosisCodeViewModel object as model to PartialView DiagnosisCodeAddEdit
            return PartialView(PartialViews.DiagnosisCodeAddEdit, currentDiagnosisCode);
        }


        /// <summary>
        /// Delete the current DiagnosisCode based on the DiagnosisCode ID passed in the DiagnosisCodeModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteDiagnosisCode(int id)
        {        //Get DiagnosisCode model object by current DiagnosisCode ID
            var currentDiagnosisCode = _service.GetDiagnosisCodeByID(id);

            //Check If DiagnosisCode model is not null
            if (currentDiagnosisCode != null)
            {
                currentDiagnosisCode.IsDeleted = true;
                currentDiagnosisCode.DeletedBy = Helpers.GetLoggedInUserId();
                currentDiagnosisCode.DeletedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current DiagnosisCode
                var result = _service.AddUptdateDiagnosisCode(currentDiagnosisCode, Helpers.DefaultDiagnosisTableNumber);

                //return deleted ID of current DiagnosisCode as Json Result to the Ajax Call.
                return Json(result);
            }


            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the DiagnosisCode View Model and pass it to DiagnosisCodeAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetDiagnosisCodeForm()
        {
            //Intialize the new object of DiagnosisCode ViewModel
            var diagnosisCodeViewModel = new Model.DiagnosisCode();

            //Pass the View Model as DiagnosisCodeViewModel to PartialView DiagnosisCodeAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.DiagnosisCodeAddEdit, diagnosisCodeViewModel);
        }


        /// <summary>
        /// Exports the diagnosis code to excel.
        /// </summary>
        /// <returns></returns>
        //public ActionResult ExportDiagnosisCodeToExcel()
        //{
        //    var diagnosisCodetable = new System.Data.DataTable("DiagnosisCodeData");
        //    diagnosisCodetable.Columns.Add("DiagnosisTableNumberId", typeof(string));
        //    diagnosisCodetable.Columns.Add("DiagnosisTableNumber", typeof(string));
        //    diagnosisCodetable.Columns.Add("DiagnosisTableName", typeof(string));
        //    diagnosisCodetable.Columns.Add("DiagnosisCode ", typeof(string));
        //    diagnosisCodetable.Columns.Add("ShortDescription", typeof(string));
        //    diagnosisCodetable.Columns.Add("DiagnosisMediumDescription", typeof(string));
        //    diagnosisCodetable.Columns.Add("DiagnosisFullDescription", typeof(string));
        //    diagnosisCodetable.Columns.Add("DiagnosisWeight", typeof(string)); 
        //    diagnosisCodetable.Columns.Add("CodeEffectiveDate", typeof(DateTime));
        //    diagnosisCodetable.Columns.Add("CodeExpiryDate", typeof(DateTime));
        //    diagnosisCodetable.Columns.Add("DiagnosisDiseaseGroup", typeof(string));
        //    diagnosisCodetable.Columns.Add("DiagnosisDiseaseCategory", typeof(string));
        //    diagnosisCodetable.Columns.Add("DiagnosisDiseaseChapter", typeof(string));


        //    var _service = new _service(Helpers.DefaultDiagnosisTableNumber);
        //    //Get the facilities list
        //    var objDiagnosisCodeData = _service.GetDiagnosisCode();
        //    foreach (var item in objDiagnosisCodeData)
        //    {
        //        var model = new DiagnosisCode
        //        {
        //            DiagnosisTableNumberId = item.DiagnosisTableNumberId,
        //            DiagnosisTableNumber = item.DiagnosisTableNumber,
        //            DiagnosisTableName = item.DiagnosisTableName,
        //            DiagnosisCode1 = item.DiagnosisCode1,
        //            ShortDescription = item.ShortDescription,
        //            DiagnosisMediumDescription = item.DiagnosisMediumDescription,
        //            DiagnosisFullDescription = item.DiagnosisFullDescription,
        //            DiagnosisWeight = item.DiagnosisWeight,
        //            DiagnosisEffectiveStartDate = item.DiagnosisEffectiveStartDate,
        //            DiagnosisEffectiveEndDate = item.DiagnosisEffectiveEndDate,
        //            DiagnosisDiseaseGroup = item.DiagnosisDiseaseGroup,
        //            DiagnosisDiseaseCategory = item.DiagnosisDiseaseCategory,
        //            DiagnosisDiseaseChapter = item.DiagnosisDiseaseChapter
        //        };

        //        diagnosisCodetable.Rows.Add(model.DiagnosisTableNumberId, model.DiagnosisTableNumber, model.DiagnosisTableName, model.DiagnosisCode1, model.ShortDescription,
        //            model.DiagnosisMediumDescription, model.DiagnosisFullDescription, model.DiagnosisWeight, model.DiagnosisEffectiveStartDate,
        //           model.DiagnosisEffectiveEndDate, model.DiagnosisDiseaseGroup, model.DiagnosisDiseaseCategory, model.DiagnosisDiseaseChapter 
        //            );
        //    }
        //    var grid = new GridView { DataSource = diagnosisCodetable };
        //    grid.DataBind();
        //    Response.ClearContent();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment; filename=DiagnosisCode.xls");
        //    Response.ContentType = "application/ms-excel";
        //    Response.Charset = "";
        //    var sw = new StringWriter();
        //    var htw = new HtmlTextWriter(sw);
        //    grid.RenderControl(htw);
        //    Response.Output.Write(sw.ToString());
        //    Response.Flush();
        //    Response.End();
        //    return RedirectToAction("DiagnosisCodeMain");
        //}

        /// <summary>
        /// Imports the diagnosis code excel data.
        /// </summary>
        /// <param name="DiagnosisFile">The diagnosis file.</param>
        /// <returns></returns>
        public ActionResult ImportDiagnosisCodeExcelData(HttpPostedFileBase DiagnosisFile)
        {
            if (DiagnosisFile != null)
            {
                string savedFileName = "~/UploadedExcelDocuments/" + DiagnosisFile.FileName;
                string OriginalPath = Server.MapPath(savedFileName);
                DiagnosisFile.SaveAs(OriginalPath);
                //DataSet dtset = Import(OriginalPath);

                // var excel = new ExcelQueryFactory();

                //var details = from x in excel.Worksheet<DiagnosisCode>() select x;
                ////excel.Worksheet<DiagnosisCode>() select x;
                //if (details != null)
                //{
                //    ViewBag.check = "File Uploaded Successfully";
                //}
                //else
                //{
                //    ViewBag.check = "issue with data. Please check the Excel";
                //}
            }
            return RedirectToAction("DiagnosisCodeMain");
        }





        //public ActionResult ExportDiagnosisCodeToExcel()
        //{
        //    var workbook = new HSSFWorkbook();
        //    var sheet = workbook.CreateSheet("CPTCodeExcel");
        //    var format = workbook.CreateDataFormat();
        //    sheet.CreateFreezePane(0, 1, 0, 1);
        //    sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:O1"));
        //    // Add header labels
        //    var rowIndex = 0;
        //    var row = sheet.CreateRow(rowIndex);
        //    row.CreateCell(0).SetCellValue("Diagnosis Table Number Id");
        //    row.CreateCell(1).SetCellValue("Diagnosis Table Number");
        //    row.CreateCell(2).SetCellValue("Diagnosis Table Name");
        //    row.CreateCell(3).SetCellValue("Diagnosis Code");
        //    row.CreateCell(4).SetCellValue("Short Description");
        //    row.CreateCell(5).SetCellValue("Diagnosis Medium Description");
        //    row.CreateCell(6).SetCellValue("Diagnosis Full Description");
        //    row.CreateCell(7).SetCellValue("Diagnosis Weight");
        //    row.CreateCell(8).SetCellValue("Diagnosis Effective StartDate");
        //    row.CreateCell(9).SetCellValue("Diagnosis Effective EndDate");
        //    row.CreateCell(10).SetCellValue("Diagnosis Disease Group");


        //    row.CreateCell(11).SetCellValue("Diagnosis Disease Category");
        //    row.CreateCell(12).SetCellValue("Diagnosis Disease Chapter");


        //    rowIndex++;
        //    using (var _dService = new DiagnosisBal(Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber))
        //    {
        //        //Get the facilities list
        //        var objCptCodesData = _dService.GetAllDiagnosisCodes();
        //        //Get the facilities list
        //        var cellStyle = workbook.CreateCellStyle();
        //        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

        //        foreach (var item in objCptCodesData)
        //        {
        //            row = sheet.CreateRow(rowIndex);
        //            row.CreateCell(0).SetCellType(CellType.Numeric);
        //            row.CreateCell(0).CellStyle = cellStyle;
        //            row.CreateCell(0).SetCellValue(Convert.ToDouble(item.DiagnosisTableNumberId));
        //            row.CreateCell(1).SetCellValue(item.DiagnosisTableNumber);
        //            row.CreateCell(2).SetCellValue(item.DiagnosisTableName);
        //            row.CreateCell(3).SetCellValue(item.DiagnosisCode1);
        //            //row.CreateCell(4).SetCellType(CellType.Numeric);
        //            //row.CreateCell(4).CellStyle = cellStyle;
        //            row.CreateCell(4).SetCellValue((item.ShortDescription));
        //            row.CreateCell(5).SetCellValue(item.DiagnosisMediumDescription);
        //            row.CreateCell(6).SetCellValue(item.DiagnosisFullDescription);
        //            row.CreateCell(7).SetCellValue(item.DiagnosisWeight.ToString());
        //            row.CreateCell(8).SetCellValue(item.DiagnosisEffectiveStartDate.ToString());
        //            row.CreateCell(9).SetCellValue((item.DiagnosisEffectiveEndDate).ToString());
        //            row.CreateCell(10).SetCellValue((item.DiagnosisDiseaseGroup));
        //            row.CreateCell(11).SetCellValue((item.DiagnosisDiseaseCategory));
        //            row.CreateCell(12).SetCellValue((item.DiagnosisDiseaseChapter));

        //            rowIndex++;
        //        }
        //    }
        //    using (var exportData = new MemoryStream())
        //    {
        //        var cookie = new HttpCookie("Downloaded", "True");
        //        Response.Cookies.Add(cookie);
        //        workbook.Write(exportData);
        //        var saveAsFileName = string.Format("DiagnosisCodesExcel-{0:d}.xls", DateTime.Now).Replace("/", "-");
        //        return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
        //    }
        //}


        public ActionResult ExportDiagnosisCodeToExcel(string searchText, string tableNumber)
        {

            var facilityId = Helpers.GetDefaultFacilityId();
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("CPTCodeExcel");
            var format = workbook.CreateDataFormat();
            sheet.CreateFreezePane(0, 1, 0, 1);
            sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:O1"));
            // Add header labels
            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("Diagnosis Table Number Id");
            row.CreateCell(1).SetCellValue("Diagnosis Table Number");
            row.CreateCell(2).SetCellValue("Diagnosis Table Name");
            row.CreateCell(3).SetCellValue("Diagnosis Code");
            row.CreateCell(4).SetCellValue("Short Description");
            row.CreateCell(5).SetCellValue("Diagnosis Medium Description");
            row.CreateCell(6).SetCellValue("Diagnosis Full Description");
            row.CreateCell(7).SetCellValue("Diagnosis Weight");
            row.CreateCell(8).SetCellValue("Diagnosis Effective StartDate");
            row.CreateCell(9).SetCellValue("Diagnosis Effective EndDate");
            row.CreateCell(10).SetCellValue("Diagnosis Disease Group");


            row.CreateCell(11).SetCellValue("Diagnosis Disease Category");
            row.CreateCell(12).SetCellValue("Diagnosis Disease Chapter");


            rowIndex++;

            //Get the facilities list
            var objCptCodesData = searchText != null ? _dService.ExportFilteredDiagnosisCodes(searchText, tableNumber) : _dService.GetAllDiagnosisCodes(facilityId, Helpers.DefaultDiagnosisTableNumber);
            //Get the facilities list
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

            foreach (var item in objCptCodesData)
            {
                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellType(CellType.Numeric);
                row.CreateCell(0).CellStyle = cellStyle;
                row.CreateCell(0).SetCellValue(Convert.ToDouble(item.DiagnosisTableNumberId));
                row.CreateCell(1).SetCellValue(item.DiagnosisTableNumber);
                row.CreateCell(2).SetCellValue(item.DiagnosisTableName);
                row.CreateCell(3).SetCellValue(item.DiagnosisCode1);
                //row.CreateCell(4).SetCellType(CellType.Numeric);
                //row.CreateCell(4).CellStyle = cellStyle;
                row.CreateCell(4).SetCellValue((item.ShortDescription));
                row.CreateCell(5).SetCellValue(item.DiagnosisMediumDescription);
                row.CreateCell(6).SetCellValue(item.DiagnosisFullDescription);
                row.CreateCell(7).SetCellValue(item.DiagnosisWeight.ToString());
                row.CreateCell(8).SetCellValue(item.DiagnosisEffectiveStartDate.ToString());
                row.CreateCell(9).SetCellValue((item.DiagnosisEffectiveEndDate).ToString());
                row.CreateCell(10).SetCellValue((item.DiagnosisDiseaseGroup));
                row.CreateCell(11).SetCellValue((item.DiagnosisDiseaseCategory));
                row.CreateCell(12).SetCellValue((item.DiagnosisDiseaseChapter));

                rowIndex++;

            }
            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                Response.Cookies.Add(cookie);
                var currentDateTime = Helpers.GetInvariantCultureDateTime();
                workbook.Write(exportData);
                var saveAsFileName = string.Format("DiagnosisCodesExcel-{0:d}.xls", currentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }

    }
}
