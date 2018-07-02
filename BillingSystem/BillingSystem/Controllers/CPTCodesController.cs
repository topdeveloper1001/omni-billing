using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using System.Data;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{


    /// <summary>
    /// The cpt codes controller.
    /// </summary>
    public class CPTCodesController : BaseController
    {
        private readonly ICarePlanService _cpService;
        private readonly ICPTCodesService _service;
        private readonly IGlobalCodeCategoryService _gcService;

        public CPTCodesController(ICarePlanService cpService, ICPTCodesService service, IGlobalCodeCategoryService gcService)
        {
            _cpService = cpService;
            _service = service;
            _gcService = gcService;
        }


        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ViewResult Index()
        {
            //using (var bal = new CPTCodesBal(Helpers.DefaultCptTableNumber))
            //{
            //    var vm = new CPTCodesView
            //    {
            //        CPTCodesList = bal.GetCPTCodes(),
            //        CurrentCPTCode = new CPTCodes { IsActive = true, IsDeleted = false, CPTCodesId = 0 }
            //    };
            //    return View(vm);
            //}
            return View();
        }

        /// <summary>
        /// Gets the CPT codes list.
        /// </summary>
        /// <param name="sidx">The sidx.</param>
        /// <param name="sord">The sord.</param>
        /// <param name="page">The page.</param>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public JsonResult GetCptCodesList(string sidx, string sord, int? page, int? rows)
        {
            var list = _service.GetCPTCodes(Helpers.DefaultCptTableNumber);
            var pageIndex = page.HasValue ? page.Value : 1;
            var pageSize = rows == null ? 0 : Convert.ToInt32(rows);

            if (!string.IsNullOrEmpty(sidx))
            {
                if (sidx == "CodeTableNumber" && sord == "desc")
                    list = list.OrderByDescending(e => e.CodeTableNumber).ToList();
                if (sidx == "CodeTableNumber" && sord == "asc")
                    list = list.OrderBy(e => e.CodeTableNumber).ToList();
                if (sidx == "CodeNumbering" && sord == "asc")
                    list = list.OrderBy(e => e.CodeNumbering).ToList();
                if (sidx == "CodeNumbering" && sord == "desc")
                    list = list.OrderByDescending(e => e.CodeNumbering).ToList();
                if (sidx == "CodeDescription" && sord == "asc")
                    list = list.OrderBy(e => e.CodeDescription).ToList();
                if (sidx == "CodeDescription" && sord == "desc")
                    list = list.OrderByDescending(e => e.CodeDescription).ToList();
                if (sidx == "CodePrice" && sord == "asc")
                    list = list.OrderBy(e => e.CodePrice).ToList();
                if (sidx == "CodePrice" && sord == "desc")
                    list = list.OrderByDescending(e => e.CodePrice).ToList();
                if (sidx == "CodeAnesthesiaBaseUnit" && sord == "asc")
                    list = list.OrderBy(e => e.CodeAnesthesiaBaseUnit).ToList();
                if (sidx == "CodeAnesthesiaBaseUnit" && sord == "desc")
                    list = list.OrderByDescending(e => e.CodeAnesthesiaBaseUnit).ToList();
                if (sidx == "CodeGroup" && sord == "asc")
                    list = list.OrderBy(e => e.CodeGroup).ToList();
                if (sidx == "CodeGroup" && sord == "desc")
                    list = list.OrderByDescending(e => e.CodeGroup).ToList();
            }

            var totalRecords = list.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = pageIndex,
                records = totalRecords,
                rows = (
                    from data in list
                    select new
                    {
                        i = data.CPTCodesId,
                        cell = new dynamic[]
                        {
                                data.CPTCodesId,
                                data.CodeTableNumber,
                                data.CodeNumbering,
                                data.CodeDescription,
                                data.CodePrice,
                                data.CodeAnesthesiaBaseUnit,
                                data.CodeGroup
                        }
                    }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        ///     CPTs the codes.
        /// </summary>
        /// <returns></returns>
        public ActionResult CPTCodes()
        {  //Get the facilities list
            var list = _service.GetCptCodesListOnDemand(1, Helpers.DefaultRecordCount, Helpers.DefaultCptTableNumber);

            //Intialize the View Model i.e. CPTCodesView which is binded to Main View Index.cshtml under CPTCodes
            var viewData = new CPTCodesView
            {
                CPTCodesList = list,
                CurrentCPTCode = new CPTCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };

            //Pass the View Model in ActionResult to View CPTCodes
            return View(viewData);

        }

        /// <summary>
        ///     Saves the CPT codes.
        /// </summary>
        /// <param name="model">The CPT codes model.</param>
        /// <returns></returns>
        public ActionResult SaveCPTCodes(CPTCodes model)
        {
            //Initialize the newId variable 
            var newId = -1;

            //Check if CPTCodesViewModel 
            if (model != null)
            {
                if (model.CPTCodesId > 0)
                {
                    model.ModifiedBy = Helpers.GetLoggedInUserId();
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    model.IsDeleted = false;
                }
                //Call the AddCPTCodes Method to Add / Update current CPTCodes
                newId = _service.AddUpdateCPTCodes(model, Helpers.DefaultCptTableNumber);

            }
            return Json(newId);
        }

        /// <summary>
        ///     Bind all the CPTCodes list
        /// </summary>
        /// <returns>
        ///     action result with the partial view containing the CPTCodes list object
        /// </returns>
        public ActionResult BindCPTCodesList()
        {   //Get the facilities list
            var cptCodesList = _service.GetCPTCodes(Helpers.DefaultCptTableNumber);
            var viewData = new CPTCodesView
            {
                CPTCodesList = cptCodesList,
                CurrentCPTCode = new CPTCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };
            //Pass the ActionResult with List of CPTCodesViewModel object to Partial View CPTCodesList
            return PartialView(PartialViews.CPTCodesList, viewData);

        }
        /// <summary>
        ///     Bind all the CPTCodes list
        /// </summary>
        /// <returns>
        ///     action result with the partial view containing the CPTCodes list object
        /// </returns>
        public ActionResult BindCPTCodesListNew(string blockNumber, bool showInActive)
        {
            var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);
            //Get the facilities list
            var cptCodesList = _service.GetCPTCodesData(showInActive, Helpers.DefaultCptTableNumber).OrderByDescending(f => f.CPTCodesId).Take(takeValue).ToList();
            var viewData = new CPTCodesView
            {
                CPTCodesList = cptCodesList,
                CurrentCPTCode = new CPTCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };
            //Pass the ActionResult with List of CPTCodesViewModel object to Partial View CPTCodesList
            return PartialView(PartialViews.CPTCodesList, viewData);

        }
        /// <summary>
        ///     Gets the CPT codes.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetCPTCodes(string id)
        {
            var current = _service.GetCPTCodesById(Convert.ToInt32(id));
            return PartialView(PartialViews.CPTCodesAddEdit, current);

        }

        /// <summary>
        ///     Reset the CPTCodes View Model and pass it to CPTCodesAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetCPTCodesForm()
        {
            //Intialize the new object of CPTCodes ViewModel
            var cptCodesViewModel = new CPTCodes();

            //Pass the View Model as CPTCodesViewModel to PartialView CPTCodesAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.CPTCodesAddEdit, cptCodesViewModel);
        }

        /// <summary>
        ///     Delete the current CPTCodes based on the CPTCodes ID passed in the SharedViewModel
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult DeleteCPTCodes(CommonModel model)
        {
            //Get CPTCodes model object by current CPTCodes ID
            var currentCptCodes = _service.GetCPTCodesById(Convert.ToInt32(model.Id));

            //Check If CPTCodes model is not null
            if (currentCptCodes != null)
            {
                currentCptCodes.IsDeleted = true;
                currentCptCodes.DeletedBy = Helpers.GetLoggedInUserId();
                currentCptCodes.DeletedDate = Helpers.GetInvariantCultureDateTime();
                currentCptCodes.IsActive = false;

                //Update Operation of current CPTCodes
                var result = _service.AddUpdateCPTCodes(currentCptCodes, Helpers.DefaultCptTableNumber);

                //return deleted ID of current CPTCodes as Json Result to the Ajax Call.
                return Json(result);
            }


            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        ///     Gets the service main categories.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetServiceMainCategories()
        {
            var startRange = Convert.ToInt32(GlobalCodeCategoryValue.CPTCodestartRange);
            var finishRange = Convert.ToInt32(GlobalCodeCategoryValue.CPTCodesFinishRange);

            var list = _gcService.GetGlobalCodeCategoriesRange(startRange, finishRange);
            return Json(list);

        }

        /// <summary>
        ///     Exports the CPT codes to excel.
        /// </summary>
        /// <returns></returns>
        //public ActionResult ExportCPTCodesToExcel()
        //{
        //    var cptCodestable = new DataTable("CPTCodesData");
        //    cptCodestable.Columns.Add("CPTCodesId", typeof(string));
        //    cptCodestable.Columns.Add("CodeTableNumber", typeof(string));
        //    cptCodestable.Columns.Add("CodeTableDescription", typeof(string));
        //    cptCodestable.Columns.Add("CodeNumbering ", typeof(string));
        //    cptCodestable.Columns.Add("CodeDescription", typeof(string));
        //    cptCodestable.Columns.Add("CodePrice", typeof(string));
        //    cptCodestable.Columns.Add("CodeAnesthesiaBaseUnit", typeof(string));
        //    cptCodestable.Columns.Add("CodeEffectiveDate", typeof(string));
        //    cptCodestable.Columns.Add("CodeExpiryDate", typeof(string));
        //    cptCodestable.Columns.Add("CodeBasicProductApplicationRule", typeof(string));
        //    cptCodestable.Columns.Add("CodeOtherProductsApplicationRule", typeof(string));
        //    cptCodestable.Columns.Add("CodeServiceMainCategory", typeof(string));
        //    cptCodestable.Columns.Add("CodeCPTCodesSubCategory", typeof(string));
        //    cptCodestable.Columns.Add("CodeUSCLSChapter", typeof(string));
        //    cptCodestable.Columns.Add("CodeCPTMUEValues", typeof(string));
        //    cptCodestable.Columns.Add("CodeGroup", typeof(string));

        //    var cptCodesBal = new CPTCodesBal(Helpers.DefaultCptTableNumber);
        //    //Get the facilities list
        //    var objCptCodesData = cptCodesBal.GetCPTCodes();


        //    foreach (var item in objCptCodesData)
        //    {
        //        var model = new CPTCodes
        //        {
        //            CPTCodesId = item.CPTCodesId,
        //            CodeTableNumber = item.CodeTableNumber,
        //            CodeTableDescription = item.CodeTableDescription,
        //            CodeNumbering = item.CodeNumbering,
        //            CodeDescription = item.CodeDescription,
        //            CodePrice = item.CodePrice,
        //            CodeAnesthesiaBaseUnit = item.CodeAnesthesiaBaseUnit,
        //            CodeEffectiveDate = item.CodeEffectiveDate,
        //            CodeExpiryDate = item.CodeExpiryDate,
        //            CodeBasicProductApplicationRule = item.CodeBasicProductApplicationRule,
        //            CodeOtherProductsApplicationRule = item.CodeOtherProductsApplicationRule,
        //            CodeServiceMainCategory = item.CodeServiceMainCategory,
        //            CodeServiceCodeSubCategory = item.CodeServiceCodeSubCategory,
        //            CodeUSCLSChapter = item.CodeUSCLSChapter,
        //            CodeCPTMUEValues = item.CodeCPTMUEValues,
        //            CodeGroup = item.CodeGroup
        //        };
        //        cptCodestable.Rows.Add(model.CPTCodesId, model.CodeTableNumber, model.CodeTableDescription,
        //            model.CodeNumbering, model.CodeDescription, model.CodePrice, model.CodeEffectiveDate,
        //            model.CodeExpiryDate, model.CodeBasicProductApplicationRule, model.CodeOtherProductsApplicationRule,
        //            model.CodeServiceMainCategory, model.CodeServiceCodeSubCategory, model.CodeUSCLSChapter,
        //            model.CodeCPTMUEValues, model.CodeGroup);
        //    }

        //    var grid = new GridView { DataSource = cptCodestable };
        //    grid.DataBind();

        //    Response.ClearContent();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment; filename=CPTCodes.xls");
        //    Response.ContentType = "application/ms-excel";
        //    Response.Charset = "";
        //    var sw = new StringWriter();
        //    var htw = new HtmlTextWriter(sw);
        //    grid.RenderControl(htw);
        //    Response.Output.Write(sw.ToString());
        //    Response.Flush();
        //    Response.End();

        //    return null; // RedirectToAction("CPTCodes");
        //}


        /// <summary>
        ///     Bind all the CPTCodes list
        /// </summary>
        /// <returns>
        ///     action result with the partial view containing the CPTCodes list object
        /// </returns>
        public JsonResult RebindCptCodesList(int blockNumber, string tableNumber)
        {
            var recordCount = Helpers.DefaultRecordCount;

            var list = _service.GetCptCodesListOnDemand(blockNumber, recordCount, Helpers.DefaultCptTableNumber);
            var jsonResult = new
            {
                list,
                NoMoreData = list.Count < recordCount,
                UserId = Helpers.GetLoggedInUserId()
            };

            //Pass the ActionResult with List of CPTCodesViewModel object to Partial View CPTCodesList
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }


        /// <summary>
        /// Exports the service code to excel.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="tableNumber">The table number.</param>
        /// <returns></returns>
        //public ActionResult ExportCPTCodesToExcel(string searchText, string tableNumber)
        //{
        //    var workbook = new HSSFWorkbook();
        //    var sheet = workbook.CreateSheet("CPTCodeExcel");
        //    var format = workbook.CreateDataFormat();
        //    sheet.CreateFreezePane(0, 1, 0, 1);
        //    sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:O1"));
        //    var rowIndex = 0;
        //    var row = sheet.CreateRow(rowIndex);
        //    row.CreateCell(0).SetCellValue("Code Table Number");
        //    row.CreateCell(1).SetCellValue("Code Table Description");
        //    row.CreateCell(2).SetCellValue("Code Numbering");
        //    row.CreateCell(3).SetCellValue("Code Description");
        //    row.CreateCell(4).SetCellValue("Code Price");
        //    row.CreateCell(5).SetCellValue("Code Anesthesia Base Unit");
        //    row.CreateCell(6).SetCellValue("Code Effective Date");
        //    row.CreateCell(7).SetCellValue("Code Expiry Date");
        //    //row.CreateCell(8).SetCellValue("Code Basic Product Application Rule");
        //    //row.CreateCell(9).SetCellValue("Code Other Products Application Rule");
        //    //row.CreateCell(10).SetCellValue("Code Service Main Category");
        //    //row.CreateCell(11).SetCellValue("Code Service Code Sub Category");
        //    //row.CreateCell(12).SetCellValue("Code USCLS Chapter");
        //    //row.CreateCell(13).SetCellValue("Code CPTMUE Values"); // CodeGroup
        //    row.CreateCell(8).SetCellValue("CodeGroup"); // CodeGroup
        //    rowIndex++;
        //    using (var cptCodesBal = new CPTCodesBal(Helpers.DefaultCptTableNumber))
        //    {
        //        // Get the facilities list
        //        var objCptCodesData = searchText != null
        //                                  ? cptCodesBal.GetFilteredCodeExportToExcel(searchText, tableNumber)
        //                                  : cptCodesBal.GetCPTCodes();

        //        // Get the facilities list
        //        var cellStyle = workbook.CreateCellStyle();
        //        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

        //        foreach (var item in objCptCodesData)
        //        {
        //            row = sheet.CreateRow(rowIndex);
        //            row.CreateCell(0).SetCellType(CellType.Numeric);
        //            row.CreateCell(0).CellStyle = cellStyle;
        //            row.CreateCell(0).SetCellValue(Convert.ToDouble(item.CodeTableNumber));
        //            row.CreateCell(1).SetCellValue(item.CodeTableDescription);
        //            row.CreateCell(2).SetCellValue(item.CodeNumbering);
        //            row.CreateCell(3).SetCellValue(item.CodeDescription);
        //            row.CreateCell(4).SetCellType(CellType.Numeric);
        //            row.CreateCell(4).CellStyle = cellStyle;

        //            // row.CreateCell(4).SetCellValue(Convert.ToDouble(item.CodePrice));
        //            row.CreateCell(4).SetCellValue((item.CodePrice));
        //            row.CreateCell(5).SetCellValue(item.CodeAnesthesiaBaseUnit);
        //            row.CreateCell(6).SetCellValue(item.CodeEffectiveDate.ToString());
        //            row.CreateCell(7).SetCellValue(item.CodeExpiryDate.ToString());
        //            //row.CreateCell(8).SetCellValue(item.CodeBasicProductApplicationRule);
        //            //row.CreateCell(9).SetCellValue((item.CodeOtherProductsApplicationRule));
        //            //row.CreateCell(10).SetCellValue((item.CodeServiceMainCategory).ToString());
        //            //row.CreateCell(11).SetCellValue(Convert.ToString(item.CodeServiceCodeSubCategory));
        //            //row.CreateCell(12).SetCellValue((item.CodeUSCLSChapter));
        //            //row.CreateCell(13).SetCellValue((item.CodeCPTMUEValues));
        //            row.CreateCell(8).SetCellValue((item.CodeGroup));
        //            rowIndex++;
        //        }
        //    }

        //    using (var exportData = new MemoryStream())
        //    {
        //        var cookie = new HttpCookie("Downloaded", "True");
        //        Response.Cookies.Add(cookie);
        //        workbook.Write(exportData);
        //        var saveAsFileName = string.Format("CPTCodesExcel-{0:d}.xls", Helpers.GetInvariantCultureDateTime()).Replace("/", "-");
        //        return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
        //    }
        //}

        public ActionResult BindActiveInActiveCptCodesList(string blockNumber, bool showInActive)
        {
            var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);
            //Get the facilities list
            var cptCodesList = _service.GetCPTCodesData(showInActive, Helpers.DefaultCptTableNumber).OrderByDescending(f => f.CPTCodesId).Take(takeValue).ToList();
            var viewData = new CPTCodesView
            {
                CPTCodesList = cptCodesList,
                CurrentCPTCode = new CPTCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };
            //Pass the ActionResult with List of CPTCodesViewModel object to Partial View CPTCodesList
            return PartialView(PartialViews.CPTCodesList, viewData);

        }


        public ActionResult GetCptCodesOnEdit(string id)
        {
            var current = _service.GetCPTCodesById(Convert.ToInt32(id));
            var jsonData = new
            {
                current.CPTCodesId,
                current.CTPCodeRangeValue,
                current.CodeAnesthesiaBaseUnit,
                current.CodeBasicProductApplicationRule,
                current.CodeCPTMUEValues,
                current.CodeDescription,
                CodeEffectiveDate = current.CodeEffectiveDate.GetShortDateString3(),
                CodeExpiryDate = current.CodeExpiryDate.GetShortDateString3(),
                current.CodeGroup,
                current.CodeNumbering,
                current.CodeOtherProductsApplicationRule,
                current.CodePrice,
                current.CodeServiceCodeSubCategory,
                current.CodeServiceMainCategory,
                current.CodeTableDescription,
                current.CodeTableNumber,
                current.CodeUSCLSChapter,
                current.IsActive

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}