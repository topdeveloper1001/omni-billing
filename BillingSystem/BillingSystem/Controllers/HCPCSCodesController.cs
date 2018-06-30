using System.Web;
using System.Linq;
using BillingSystem.Models;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using System;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace BillingSystem.Controllers
{
    public class HCPCSCodesController : BaseController
    {
        //public ActionResult HCPCSCodes(SharedViewModel shared)
        //{
        //    var objHCPCSCodesComm = new HCPCSCodesCommunicator();
        //    var objHCPCSCodesData = objHCPCSCodesComm.GetHCPCSCodes(shared);
        //    var HCPCSCodesModel = new HCPCSCodesView
        //    {
        //        HCPCSCodesList = objHCPCSCodesData,
        //        HCPCSCodesViewModel = new HCPCSCodesViewModel(),
        //        CurrentHCPCSCodes = new HCPCSCodesViewModel()
        //    };

        //    var objGlobalCodeComm = new GlobalCodeCommunicator();
        //    shared.StartRange = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.ServiceCodeStartRange);
        //    shared.FinishRange = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.ServiceCodeFinishRange);
        //    var objGlobalCodeData = objGlobalCodeComm.GetGlobalCodeCategoriesRange(shared);
        //    HCPCSCodesModel.CurrentHCPCSCodes.LstServiceMainCategory = objGlobalCodeData;
        //    HCPCSCodesModel.CurrentHCPCSCodes.LstServiceSubCategory = new List<GlobalCodeViewModel>();
        //    return View(HCPCSCodesModel);
        //}

        //public ActionResult SaveHCPCSCodes(HCPCSCodesViewModel HCPCSCodesviewModel)
        //{
        //    var newId = -1;
        //    if (HCPCSCodesviewModel != null)
        //    {
        //        var objHCPCSCodesComm = new HCPCSCodesCommunicator();
        //        newId = objHCPCSCodesComm.AddHCPCSCodes(HCPCSCodesviewModel);
        //    }
        //    return Json(newId);
        //}

        ///// <summary>
        ///// Bind all the HCPCSCodes list 
        ///// </summary>
        ///// <returns>action result with the partial view containing the HCPCSCodes list object</returns>
        //[HttpPost]
        //public ActionResult BindHCPCSCodesList()
        //{
        //    var objHCPCSCodesComm = new HCPCSCodesCommunicator();
        //    var HCPCSCodesList = objHCPCSCodesComm.GetHCPCSCodes(null);
        //    return PartialView(PartialViews.HCPCSCodesList, HCPCSCodesList);
        //}

        //public ActionResult GetHCPCSCodes(SharedViewModel shared)
        //{
        //    var objHCPCSCodesComm = new HCPCSCodesCommunicator();
        //    var objHCPCSCodesData = objHCPCSCodesComm.GetHCPCSCodesById(shared);
        //    if (objHCPCSCodesData != null)
        //        return PartialView("UserControls/_HCPCSCodesAddEdit", objHCPCSCodesData);
        //    return null;
        //}

        //public ActionResult ExportHCPCSCodesToExcel()
        //{

        //    var HCPCSCodestable = new System.Data.DataTable("HCPCSCodesData");

        //    HCPCSCodestable.Columns.Add("HCPCSCodesId", typeof(string));
        //    HCPCSCodestable.Columns.Add("CodeTableNumber", typeof(string));
        //    HCPCSCodestable.Columns.Add("CodeTableDescription", typeof(string));
        //    HCPCSCodestable.Columns.Add("CodeNumbering ", typeof(string));
        //    HCPCSCodestable.Columns.Add("CodeDescription", typeof(string));
        //    HCPCSCodestable.Columns.Add("CodePrice", typeof(string));
        //    HCPCSCodestable.Columns.Add("CodeEffectiveDate", typeof(DateTime));
        //    HCPCSCodestable.Columns.Add("CodeExpiryDate", typeof(DateTime));
        //    HCPCSCodestable.Columns.Add("CodeBasicProductApplicationRule", typeof(string));
        //    HCPCSCodestable.Columns.Add("CodeOtherProductsApplicationRule", typeof(string));
        //    HCPCSCodestable.Columns.Add("CodeServiceMainCategory", typeof(string));
        //    HCPCSCodestable.Columns.Add("CodeServiceCodeSubCategory", typeof(string));
        //    HCPCSCodestable.Columns.Add("CodeCPTMUEValues", typeof(string));

        //    var objHCPCSCodesComm = new HCPCSCodesCommunicator();
        //    var objHCPCSCodesData = objHCPCSCodesComm.GetHCPCSCodes(null);
        //    foreach (var item in objHCPCSCodesData)
        //    {
        //        HCPCSCodesViewModel model = new HCPCSCodesViewModel();
        //        model.HCPCSCodesId = item.HCPCSCodesId;
        //        model.CodeTableNumber = item.CodeTableNumber;
        //        model.CodeTableDescription = item.CodeTableDescription;
        //        model.CodeNumbering = item.CodeNumbering;
        //        model.CodeDescription = item.CodeDescription;
        //        model.CodePrice = item.CodePrice;
        //        model.CodeEffectiveDate = item.CodeEffectiveDate;
        //        model.CodeExpiryDate = item.CodeExpiryDate;
        //        model.CodeBasicProductApplicationRule = item.CodeBasicProductApplicationRule;
        //        model.CodeOtherProductsApplicationRule = item.CodeOtherProductsApplicationRule;
        //        model.CodeServiceMainCategory = item.CodeServiceMainCategory;
        //        model.CodeServiceCodeSubCategory = item.CodeServiceCodeSubCategory;
        //        model.CodeCPTMUEValues = item.CodeCPTMUEValues;
        //        HCPCSCodestable.Rows.Add(model.HCPCSCodesId, model.CodeTableNumber, model.CodeTableDescription, model.CodeNumbering, model.CodeDescription, model.CodePrice, model.CodeEffectiveDate, model.CodeExpiryDate, model.CodeBasicProductApplicationRule, model.CodeOtherProductsApplicationRule, model.CodeServiceMainCategory, model.CodeServiceCodeSubCategory, model.CodeCPTMUEValues);
        //    }

        //    var grid = new GridView();
        //    grid.DataSource = HCPCSCodestable;
        //    grid.DataBind();

        //    Response.ClearContent();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment; filename=HCPCSCodes.xls");
        //    Response.ContentType = "application/ms-excel";
        //    Response.Charset = "";
        //    StringWriter sw = new StringWriter();
        //    HtmlTextWriter htw = new HtmlTextWriter(sw);
        //    grid.RenderControl(htw);
        //    Response.Output.Write(sw.ToString());
        //    Response.Flush();
        //    Response.End();
        //    return RedirectToAction("ServiceCode");
        //}

        ////public ActionResult ImportServiceCodeExcelData(ServiceCodeDTO model)
        ////{
        ////    if (logic.UserLoggedONot() == false)
        ////    {
        ////        return RedirectToAction("UserLogin", new RouteValueDictionary(new { controller = "Home", action = "UserLogin" }));
        ////    }

        ////    if (model.ImportExcelfile != null)
        ////    {
        ////        if (model.ImportExcelfile.ContentLength > 0)
        ////        {
        ////            logic.ImportServiceCodeData(model);
        ////            ViewBag.Message = "File uploaded successfully.";
        ////            return RedirectToAction("ServiceCode");
        ////        }
        ////    }
        ////    return RedirectToAction("ServiceCode");
        ////}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult BindSubCategories(string categoryId)
        //{
        //    if (categoryId.Contains("\n"))
        //        categoryId = categoryId.Replace("\n", string.Empty);

        //    using (var globalComm = new ServiceCodeCommunicator())
        //    {
        //        var list = globalComm.GetGlobalCodesById(categoryId);
        //        return Json(list);
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Reset the HCPCSCodes View Model and pass it to HCPCSCodesAddEdit Partial View. 
        ///// </summary>
        ///// <param name="shared">pass the input parameters such as ID</param>
        ///// <returns></returns>
        //public ActionResult ResetHCPCSCodesForm(SharedViewModel shared)
        //{
        //    //Bind the GlobalCode dropdown data
        //    var objGlobalCodeComm = new GlobalCodeCommunicator();
        //    shared.StartRange = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.ServiceCodeStartRange);
        //    shared.FinishRange = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.ServiceCodeFinishRange);

        //    //Intialize the new object of HCPCSCodes ViewModel
        //    var HCPCSCodesViewModel = new HCPCSCodesViewModel
        //    {
        //        LstServiceMainCategory = objGlobalCodeComm.GetGlobalCodeCategoriesRange(shared),
        //        LstServiceSubCategory = new List<GlobalCodeViewModel>()
        //    };

        //    //Pass the View Model as HCPCSCodesViewModel to PartialView HCPCSCodesAddEdit just to update the AddEdit partial view.
        //    return PartialView(PartialViews.HCPCSCodesAddEdit, HCPCSCodesViewModel);
        //}

        ///// <summary>
        ///// Delete the current HCPCSCodes based on the HCPCSCodes ID passed in the SharedViewModel
        ///// </summary>
        ///// <param name="shared"></param>
        ///// <returns></returns>
        //public ActionResult DeleteHCPCSCodes(SharedViewModel shared)
        //{
        //    //Intialize the Deleted ID variable 
        //    var deletedId = -1;

        //    //Update Operation of current HCPCSCodes
        //    var objHCPCSCodesComm = new HCPCSCodesCommunicator();
        //    deletedId = objHCPCSCodesComm.DeleteHCPCSCodes(shared);

        //    //Return the Json result as Action Result back JSON Call Success
        //    return Json(deletedId);
        //}

        /// <summary>
        /// HCPCSs the codes.
        /// </summary>
        /// <returns></returns>
        public ActionResult HCPCSCodes()
        {
            //Initialize the HCPCSCodes Communicator object
            var hcpcsCodesBal = new HCPCSCodesService(Helpers.DefaultHcPcsTableNumber);

            //Get the facilities list
            var hcpcsCodesList = hcpcsCodesBal.GetHCPCSCodesListOnDemand(1, Helpers.DefaultRecordCount);

            //Intialize the View Model i.e. HCPCSCodesView which is binded to Main View Index.cshtml under HCPCSCodes
            var hcpcsCodesView = new HCPCSCodesView
            {
                HCPCSCodesList = hcpcsCodesList,
                CurrentHCPCSCodes = new HCPCSCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };

            //Pass the View Model in ActionResult to View HCPCSCodes
            return View(hcpcsCodesView);
        }

        public ActionResult SortHCPCSCodes()
        {
            var viewpath = string.Format("../HCPCSCodes/{0}", PartialViews.HCPCSCodesList);
            var hcpcsCodesBal = new HCPCSCodesService(Helpers.DefaultHcPcsTableNumber);
            var hcpcsCodesList = hcpcsCodesBal.GetHCPCSCodesListOnDemand(1, Helpers.DefaultRecordCount);
            var hcpcsCodesView = new HCPCSCodesView
            {
                HCPCSCodesList = hcpcsCodesList,
                CurrentHCPCSCodes = new HCPCSCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };
            return PartialView(PartialViews.HCPCSCodesList, hcpcsCodesView);
        }

        /// <summary>
        /// Saves the HCPCS codes.
        /// </summary>
        /// <param name="HCPCSCodesModel">The HCPCS codes model.</param>
        /// <returns></returns>
        public ActionResult SaveHCPCSCodes(HCPCSCodes HCPCSCodesModel)
        {
            //Initialize the newId variable 
            var newId = -1;

            //Check if HCPCSCodesViewModel 
            if (HCPCSCodesModel != null)
            {
                using (var hcpcsCodesBal = new HCPCSCodesService(Helpers.DefaultHcPcsTableNumber))
                {
                    if (HCPCSCodesModel.HCPCSCodesId > 0)
                    {
                        HCPCSCodesModel.ModifiedBy = Helpers.GetLoggedInUserId();
                        HCPCSCodesModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                        HCPCSCodesModel.IsActive = HCPCSCodesModel.IsActive;
                        HCPCSCodesModel.IsDeleted = false;
                    }
                    else
                    {
                        HCPCSCodesModel.CreatedBy = Helpers.GetLoggedInUserId();
                        HCPCSCodesModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                        HCPCSCodesModel.IsActive = HCPCSCodesModel.IsActive;
                        HCPCSCodesModel.IsDeleted = false;
                    }
                    //Call the AddHCPCSCodes Method to Add / Update current HCPCSCodes
                    newId = hcpcsCodesBal.AddHCPCSCodes(HCPCSCodesModel);
                }
            }
            return Json(newId);
        }

        /// <summary>
        /// Bind all the HCPCSCodes list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the HCPCSCodes list object
        /// </returns>
        //[HttpPost]
        public ActionResult BindHCPCSCodesList()
        {
            //Initialize the HCPCSCodes Communicator object
            using (var hcpcsCodesBal = new HCPCSCodesService(Helpers.DefaultHcPcsTableNumber))
            {
                //Get the facilities list
                var hcpcsCodesList = hcpcsCodesBal.GetHCPCSCodes();
                var hcpcsCodesView = new HCPCSCodesView
                {
                    HCPCSCodesList = hcpcsCodesList,
                    CurrentHCPCSCodes = new HCPCSCodes(),
                    UserId = Helpers.GetLoggedInUserId()
                };
                //Pass the ActionResult with List of HCPCSCodesViewModel object to Partial View HCPCSCodesList
                return PartialView(PartialViews.HCPCSCodesList, hcpcsCodesView);
            }
        }

        public ActionResult BindHCPCSCodesListNew(string blockNumber)
        {
            var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);
            //Initialize the HCPCSCodes Communicator object
            using (var hcpcsCodesBal = new HCPCSCodesService(Helpers.DefaultHcPcsTableNumber))
            {
                //Get the facilities list
                var hcpcsCodesList =
                    hcpcsCodesBal.GetHCPCSCodes().OrderByDescending(f => f.HCPCSCodesId).Take(takeValue).ToList();
                var hcpcsCodesView = new HCPCSCodesView
                {
                    HCPCSCodesList = hcpcsCodesList,
                    CurrentHCPCSCodes = new HCPCSCodes(),
                    UserId = Helpers.GetLoggedInUserId()
                };
                //Pass the ActionResult with List of HCPCSCodesViewModel object to Partial View HCPCSCodesList
                return PartialView(PartialViews.HCPCSCodesList, hcpcsCodesView);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockNumber"></param>
        /// <returns></returns>
        public JsonResult RebindBindHCPCSCodesList(int blockNumber,string tableNumber)
        {
            var recordCount = Helpers.DefaultRecordCount;
            using (var hcpcsCodesBal = new HCPCSCodesService(tableNumber))
            {
                var list = hcpcsCodesBal.GetHCPCSCodesListOnDemand(blockNumber, recordCount);
                var jsonResult = new
                {
                    list,
                    NoMoreData = list.Count < recordCount,
                    UserId = Helpers.GetLoggedInUserId()
                };
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Gets the HCPCS codes.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetHCPCSCodes(string id)
        {
            using (var bal = new HCPCSCodesService(Helpers.DefaultHcPcsTableNumber))
            {
                var current = bal.GetHCPCSCodesById(Convert.ToInt32(id));
                return PartialView(PartialViews.HCPCSCodesAddEdit, current);
            }
        }

        /// <summary>
        /// Reset the HCPCSCodes View Model and pass it to HCPCSCodesAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetHCPCSCodesForm()
        {
            //Intialize the new object of HCPCSCodes ViewModel
            var hcpcsCodesViewModel = new HCPCSCodes();

            //Pass the View Model as HCPCSCodesViewModel to PartialView HCPCSCodesAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.HCPCSCodesAddEdit, hcpcsCodesViewModel);
        }

        /// <summary>
        /// Delete the current HCPCSCodes based on the HCPCSCodes ID passed in the SharedViewModel
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult DeleteHCPCSCodes(CommonModel model)
        {
            using (var hcpcsCodesBal = new HCPCSCodesService(Helpers.DefaultHcPcsTableNumber))
            {
                //Get HCPCSCodes model object by current HCPCSCodes ID
                var currentHcpcsCodes = hcpcsCodesBal.GetHCPCSCodesById(Convert.ToInt32(model.Id));

                //Check If HCPCSCodes model is not null
                if (currentHcpcsCodes != null)
                {
                    currentHcpcsCodes.IsDeleted = true;
                    currentHcpcsCodes.DeletedBy = Helpers.GetLoggedInUserId();
                    currentHcpcsCodes.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current HCPCSCodes
                    var result = hcpcsCodesBal.AddHCPCSCodes(currentHcpcsCodes);

                    //return deleted ID of current HCPCSCodes as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Gets the service main categories.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetServiceMainCategories()
        {
            var startRange = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.HCPCSCodestartRange);
            var finishRange = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.HCPCSCodesFinishRange);

            using (var bal = new GlobalCodeCategoryService())
            {
                var list = bal.GetGlobalCodeCategoriesRange(startRange, finishRange);
                return Json(list);
            }
        }

        /// <summary>
        /// Exports the HCPCS codes to excel.
        /// </summary>
        /// <returns></returns>
        //public ActionResult ExportHCPCSCodesToExcel()
        //{

        //    var hcpcsCodestable = new System.Data.DataTable("HCPCSCodesData");

        //    hcpcsCodestable.Columns.Add("HCPCSCodesId", typeof(string));
        //    hcpcsCodestable.Columns.Add("CodeTableNumber", typeof(string));
        //    hcpcsCodestable.Columns.Add("CodeTableDescription", typeof(string));
        //    hcpcsCodestable.Columns.Add("CodeNumbering ", typeof(string));
        //    hcpcsCodestable.Columns.Add("CodeDescription", typeof(string));
        //    hcpcsCodestable.Columns.Add("CodePrice", typeof(string));
        //    hcpcsCodestable.Columns.Add("CodeEffectiveDate", typeof(DateTime));
        //    hcpcsCodestable.Columns.Add("CodeExpiryDate", typeof(DateTime));
        //    hcpcsCodestable.Columns.Add("CodeBasicProductApplicationRule", typeof(string));
        //    hcpcsCodestable.Columns.Add("CodeOtherProductsApplicationRule", typeof(string));
        //    hcpcsCodestable.Columns.Add("CodeServiceMainCategory", typeof(string));
        //    hcpcsCodestable.Columns.Add("CodeServiceCodeSubCategory", typeof(string));
        //    hcpcsCodestable.Columns.Add("CodeCPTMUEValues", typeof(string));

        //    var hcpcsCodesBal = new HCPCSCodesBal(Helpers.DefaultHcPcsTableNumber);
        //    //Get the facilities list
        //    var objHcpcsCodesData = hcpcsCodesBal.GetHCPCSCodes();

        //    foreach (var item in objHcpcsCodesData)
        //    {
        //        var model = new HCPCSCodes
        //        {
        //            HCPCSCodesId = item.HCPCSCodesId,
        //            CodeTableNumber = item.CodeTableNumber,
        //            CodeTableDescription = item.CodeTableDescription,
        //            CodeNumbering = item.CodeNumbering,
        //            CodeDescription = item.CodeDescription,
        //            CodePrice = item.CodePrice,
        //            CodeEffectiveDate = item.CodeEffectiveDate,
        //            CodeExpiryDate = item.CodeExpiryDate,
        //            CodeBasicProductApplicationRule = item.CodeBasicProductApplicationRule,
        //            CodeOtherProductsApplicationRule = item.CodeOtherProductsApplicationRule,
        //            CodeServiceMainCategory = item.CodeServiceMainCategory,
        //            CodeServiceCodeSubCategory = item.CodeServiceCodeSubCategory,
        //            CodeCPTMUEValues = item.CodeCPTMUEValues
        //        };
        //        hcpcsCodestable.Rows.Add(model.HCPCSCodesId, model.CodeTableNumber, model.CodeTableDescription,
        //            model.CodeNumbering, model.CodeDescription, model.CodePrice, model.CodeEffectiveDate,
        //            model.CodeExpiryDate, model.CodeBasicProductApplicationRule, model.CodeOtherProductsApplicationRule,
        //            model.CodeServiceMainCategory, model.CodeServiceCodeSubCategory, model.CodeCPTMUEValues);
        //    }

        //    var grid = new GridView();
        //    grid.DataSource = hcpcsCodestable;
        //    grid.DataBind();

        //    Response.ClearContent();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment; filename=HCPCSCodes.xls");
        //    Response.ContentType = "application/ms-excel";
        //    Response.Charset = "";
        //    var sw = new StringWriter();
        //    var htw = new HtmlTextWriter(sw);
        //    grid.RenderControl(htw);
        //    Response.Output.Write(sw.ToString());
        //    Response.Flush();
        //    Response.End();
        //    return RedirectToAction("HCPCSCodes");
        //}

        //public ActionResult ExportHCPCSCodesToExcel()
        //{
        //    var workbook = new HSSFWorkbook();
        //    var sheet = workbook.CreateSheet("HCPCSCodesData");
        //    var format = workbook.CreateDataFormat();
        //    sheet.CreateFreezePane(0, 1, 0, 1);
        //    sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:O1"));
        //    // Add header labels
        //    var rowIndex = 0;
        //    var row = sheet.CreateRow(rowIndex);
        //    row.CreateCell(0).SetCellValue("HCPCS Codes Id");
        //    row.CreateCell(1).SetCellValue("Code Table Number");
        //    row.CreateCell(2).SetCellValue("Code Table Discription");
        //    row.CreateCell(3).SetCellValue("Code Numbring");
        //    row.CreateCell(4).SetCellValue("Code Discription");
        //    row.CreateCell(5).SetCellValue("Code Price");
        //    row.CreateCell(6).SetCellValue("Code Effective Date");
        //    row.CreateCell(7).SetCellValue("Code Expiry Date");
        //    row.CreateCell(8).SetCellValue("Code Basic Product Application Rule");
        //    row.CreateCell(9).SetCellValue("Code Other Products Application Rule");
        //    row.CreateCell(10).SetCellValue("Code Service Main Category");
        //    row.CreateCell(11).SetCellValue("Code Service Code Sub Category");
        //    row.CreateCell(12).SetCellValue("Code CPTMUEV Values");


        //    rowIndex++;

        //    using (var hcpcsCodesBal = new HCPCSCodesBal(Helpers.DefaultHcPcsTableNumber))
        //    {
        //        //Get the facilities list

        //        var onjHcpcsCodesData = hcpcsCodesBal.GetHCPCSCodes();
        //        //Get the facilities list
        //        var cellStyle = workbook.CreateCellStyle();
        //        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

        //        foreach (var item in onjHcpcsCodesData)
        //        {
        //            row = sheet.CreateRow(rowIndex);
        //            row.CreateCell(0).SetCellType(CellType.Numeric);
        //            row.CreateCell(0).CellStyle = cellStyle;
        //            row.CreateCell(0).SetCellValue(Convert.ToDouble(item.HCPCSCodesId));
        //            row.CreateCell(1).SetCellValue(item.CodeTableNumber);
        //            row.CreateCell(2).SetCellValue(item.CodeTableDescription);
        //            row.CreateCell(3).SetCellValue(item.CodeNumbering);
        //            //row.CreateCell(4).SetCellType(CellType.Numeric);
        //            //row.CreateCell(4).CellStyle = cellStyle;  
        //            row.CreateCell(4).SetCellValue(item.CodeDescription);
        //            row.CreateCell(5).CellStyle = cellStyle;
        //            row.CreateCell(5).SetCellValue(item.CodePrice);
        //            row.CreateCell(6).SetCellValue(item.CodeEffectiveDate.ToString());
        //            row.CreateCell(7).SetCellValue(item.CodeExpiryDate.ToString());
        //            row.CreateCell(8).SetCellValue(item.CodeBasicProductApplicationRule);
        //            row.CreateCell(9).SetCellValue((item.CodeOtherProductsApplicationRule));
        //            row.CreateCell(10).SetCellValue((item.CodeServiceMainCategory).ToString());
        //            row.CreateCell(11).SetCellValue((item.CodeServiceCodeSubCategory).ToString());
        //            row.CreateCell(12).SetCellValue((item.CodeCPTMUEValues));

        //            rowIndex++;
        //        }
        //    }
        //    using (var exportData = new MemoryStream())
        //    {
        //        var cookie = new HttpCookie("Downloaded", "True");
        //        Response.Cookies.Add(cookie);
        //        workbook.Write(exportData);
        //        var saveAsFileName = string.Format("HCPCSCodesDataExcel-{0:d}.xls", DateTime.Now).Replace("/", "-");
        //        return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
        //    }
        //}

        public ActionResult ExportHCPCSCodesToExcel(string searchText,string tableNumber)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("HCPCSCodesData");
            var format = workbook.CreateDataFormat();
            sheet.CreateFreezePane(0, 1, 0, 1);
            sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:O1"));
            // Add header labels
            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("HCPCS Codes Id");
            row.CreateCell(1).SetCellValue("Code Table Number");
            row.CreateCell(2).SetCellValue("Code Table Discription");
            row.CreateCell(3).SetCellValue("Code Numbring");
            row.CreateCell(4).SetCellValue("Code Discription");
            row.CreateCell(5).SetCellValue("Code Price");
            row.CreateCell(6).SetCellValue("Code Effective Date");
            row.CreateCell(7).SetCellValue("Code Expiry Date");
            row.CreateCell(8).SetCellValue("Code Basic Product Application Rule");
            row.CreateCell(9).SetCellValue("Code Other Products Application Rule");
            row.CreateCell(10).SetCellValue("Code Service Main Category");
            row.CreateCell(11).SetCellValue("Code Service Code Sub Category");
            row.CreateCell(12).SetCellValue("Code CPTMUEV Values");


            rowIndex++;

            using (var hcpcsCodesBal = new HCPCSCodesService(Helpers.DefaultHcPcsTableNumber))
            {
                //Get the facilities list
               
                var onjHcpcsCodesData = searchText != null ? hcpcsCodesBal.ExportHCPCSCodes(searchText, tableNumber) : hcpcsCodesBal.GetHCPCSCodes();
                //Get the facilities list
                var cellStyle = workbook.CreateCellStyle();
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

                foreach (var item in onjHcpcsCodesData)
                {
                    row = sheet.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellType(CellType.Numeric);
                    row.CreateCell(0).CellStyle = cellStyle;
                    row.CreateCell(0).SetCellValue(Convert.ToDouble(item.HCPCSCodesId));
                    row.CreateCell(1).SetCellValue(item.CodeTableNumber);
                    row.CreateCell(2).SetCellValue(item.CodeTableDescription);
                    row.CreateCell(3).SetCellValue(item.CodeNumbering);
                    //row.CreateCell(4).SetCellType(CellType.Numeric);
                    //row.CreateCell(4).CellStyle = cellStyle;  
                    row.CreateCell(4).SetCellValue(item.CodeDescription);
                    row.CreateCell(5).CellStyle = cellStyle;
                    row.CreateCell(5).SetCellValue(item.CodePrice);
                    row.CreateCell(6).SetCellValue(item.CodeEffectiveDate.ToString());
                    row.CreateCell(7).SetCellValue(item.CodeExpiryDate.ToString());
                    row.CreateCell(8).SetCellValue(item.CodeBasicProductApplicationRule);
                    row.CreateCell(9).SetCellValue((item.CodeOtherProductsApplicationRule));
                    row.CreateCell(10).SetCellValue((item.CodeServiceMainCategory).ToString());
                    row.CreateCell(11).SetCellValue((item.CodeServiceCodeSubCategory).ToString());
                    row.CreateCell(12).SetCellValue((item.CodeCPTMUEValues));

                    rowIndex++;
                }
            }
            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                Response.Cookies.Add(cookie);
                workbook.Write(exportData);
                var saveAsFileName = string.Format("HCPCSCodesDataExcel-{0:d}.xls", currentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }

        public ActionResult BindActiveInActiveHcpcsCodesList(string blockNumber, bool showInActive)
        {
            var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);
            //Initialize the HCPCSCodes Communicator object
            using (var hcpcsCodesBal = new HCPCSCodesService(Helpers.DefaultHcPcsTableNumber))
            {
                //Get the facilities list
                var hcpcsCodesList =
                    hcpcsCodesBal.GetActiveInActiveHCPCSCodes(showInActive).OrderByDescending(f => f.HCPCSCodesId).Take(takeValue).ToList();
                var hcpcsCodesView = new HCPCSCodesView
                {
                    HCPCSCodesList = hcpcsCodesList,
                    CurrentHCPCSCodes = new HCPCSCodes(),
                    UserId = Helpers.GetLoggedInUserId()
                };
                //Pass the ActionResult with List of HCPCSCodesViewModel object to Partial View HCPCSCodesList
                return PartialView(PartialViews.HCPCSCodesList, hcpcsCodesView);
            }
        }

        public ActionResult GetHCPCSCodesOnEdit(string id)
        {
            using (var bal = new HCPCSCodesService(Helpers.DefaultHcPcsTableNumber))
            {
                var current = bal.GetHCPCSCodesById(Convert.ToInt32(id));
                var jsonData = new
                {
                    current.IsActive,
                    current.HCPCSCodesId,
                    current.CodeBasicProductApplicationRule,
                    current.CodeTableNumber,
                    current.CodeTableDescription,
                    current.CodeServiceMainCategory,
                    current.CodeServiceCodeSubCategory,
                    current.CodePrice,
                    current.CodeOtherProductsApplicationRule,
                    current.CodeNumbering,
                  CodeExpiryDate=  current.CodeExpiryDate.GetShortDateString3(),
                  CodeEffectiveDate=  current.CodeEffectiveDate.GetShortDateString3(),
                    current.CodeDescription,
                    current.CodeCPTMUEValues
                  };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

    }
}