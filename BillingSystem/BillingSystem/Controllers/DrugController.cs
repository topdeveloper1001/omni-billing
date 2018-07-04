using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using BillingSystem.Models;
using BillingSystem.Common;
using System.Web.Mvc;
using BillingSystem.Model;
using Excel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class DrugController : BaseController
    {
        private readonly IDrugService _service;

        public DrugController(IDrugService service)
        {
            _service = service;
        }


        /// <summary>
        /// Get the details of the Drug View in the Model Drug such as DrugList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model Drug to be passed to View Drug
        /// </returns>
        public ActionResult DrugMain()
        {
            var DrugList = _service.GetDrugListOnDemand(1, Helpers.DefaultRecordCount, "Active", Helpers.DefaultDrugTableNumber);

            //Intialize the View Model i.e. DrugView which is binded to Main View Index.cshtml under Drug
            var DrugView = new DrugView
            {
                DrugList = DrugList,
                CurrentDrug = new Model.Drug(),
                UserId = Helpers.GetLoggedInUserId()
            };

            //Pass the View Model in ActionResult to View Drug
            return View(DrugView);
        }

        /// <summary>
        /// Bind all the Drug list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the Drug list object
        /// </returns>
        [HttpPost]
        public ActionResult BindDrugList(string ViewVal)
        {
            //Get the facilities list
            var list = _service.GetDrugListByDrugView(ViewVal, Helpers.DefaultDrugTableNumber);
            var viewData = new DrugView
            {
                CurrentDrug = new Drug(),
                DrugList = list,
                UserId = Helpers.GetLoggedInUserId()
            };

            //Pass the ActionResult with List of DrugViewModel object to Partial View DrugList
            return PartialView(PartialViews.DrugList, viewData);
        }
        [HttpPost]
        public ActionResult BindDrugListNew(string blockNumber, string viewVal)
        {
            var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);

            var list = _service.GetDrugListByDrugView(viewVal, Helpers.DefaultDrugTableNumber).OrderByDescending(f => f.Id).Take(takeValue).ToList(); ;
            var viewData = new DrugView
            {
                CurrentDrug = new Drug(),
                DrugList = list,
                UserId = Helpers.GetLoggedInUserId()
            };

            //Pass the ActionResult with List of DrugViewModel object to Partial View DrugList
            return PartialView(PartialViews.DrugList, viewData);
        }
        /// <summary>
        /// Add New or Update the Drug based on if we pass the Drug ID in the DrugViewModel object.
        /// </summary>
        /// <param name="DrugModel">pass the details of Drug in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of Drug row
        /// </returns>
        public ActionResult SaveDrug(Drug DrugModel)
        {
            //Initialize the newId variable 
            var newId = -1;

            //Check if DrugViewModel 
            if (DrugModel != null)
            {
                newId = _service.AddUptdateDrug(DrugModel, Helpers.DefaultDrugTableNumber);
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current Drug in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetDrug(int id)
        {
            var currentDrug = _service.GetDrugByID(id);
            return PartialView(PartialViews.DrugAddEdit, currentDrug);
        }

        public JsonResult RebindBindDrugList(int blockNumber, string viewVal, string tableNumber)
        {
            var recordCount = Helpers.DefaultRecordCount;
            var list = _service.GetDrugListOnDemand(blockNumber, recordCount, viewVal, Helpers.DefaultDrugTableNumber);
            var jsonResult = new
            {
                list,
                NoMoreData = list.Count < recordCount,
                UserId = Helpers.GetLoggedInUserId()
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Reset the Drug View Model and pass it to DrugAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetDrugForm()
        {
            //Intialize the new object of Drug ViewModel
            var DrugViewModel = new Model.Drug();

            //Pass the View Model as DrugViewModel to PartialView DrugAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.DrugAddEdit, DrugViewModel);
        }

        /// <summary>
        /// Binds the drug list custom.
        /// </summary>
        /// <param name="ViewVal">The view value.</param>
        /// <returns></returns>
        public ActionResult BindDrugListCustom(string ViewVal)
        {
            var list = _service.GetDrugListByDrugView(ViewVal, Helpers.DefaultDrugTableNumber);
            var viewData = new DrugView
            {
                CurrentDrug = new Drug(),
                DrugList = list,
                UserId = Helpers.GetLoggedInUserId()
            };
            //Pass the ActionResult with List of DrugViewModel object to Partial View DrugList
            return PartialView(PartialViews.DrugList, viewData);
        }

        public JsonResult GetSearchedDrugCodes(string text)
        {
            var result = _service.GetFilteredDrugCodes(text, Helpers.DefaultDrugTableNumber);
            var filteredList = result.Select(item => new
            {
                ID = item.DrugCode,
                Menu_Title = string.Format("{0} - {1}", item.DrugGenericName, item.DrugCode),
                Name = item.DrugGenericName
            }).ToList();
            return Json(filteredList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExportDrugCodesToExcel(string searchText, string tableNumber)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("DrugCodeExcel");
            var format = workbook.CreateDataFormat();
            sheet.CreateFreezePane(0, 1, 0, 1);
            sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:O1"));
            // Add header labels
            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("Id");
            row.CreateCell(1).SetCellValue("DrugTableNumber");
            row.CreateCell(2).SetCellValue("DrugDescription");
            row.CreateCell(3).SetCellValue("DrugCode");
            row.CreateCell(4).SetCellValue("DrugInsurancePlan");
            row.CreateCell(5).SetCellValue("DrugPackageName");
            row.CreateCell(6).SetCellValue("DrugGenericName");
            row.CreateCell(7).SetCellValue("DrugStrength");
            row.CreateCell(8).SetCellValue("DrugDosage");
            row.CreateCell(9).SetCellValue("DrugPackageSize");
            row.CreateCell(10).SetCellValue("DrugPricePublic");
            row.CreateCell(11).SetCellValue("DrugPricePharmacy");
            row.CreateCell(12).SetCellValue("DrugUnitPricePublic");
            row.CreateCell(13).SetCellValue("DrugUnitPricePharmacy");//CodeGroup
            row.CreateCell(14).SetCellValue("DrugStatus");//CodeGroup

            row.CreateCell(15).SetCellValue("DrugDeleteDate");
            row.CreateCell(16).SetCellValue("DrugLastChange");
            row.CreateCell(17).SetCellValue("DrugAgentName");
            row.CreateCell(18).SetCellValue("DrugManufacturer");
            row.CreateCell(19).SetCellValue("DrugStrengthHardcode");
            row.CreateCell(20).SetCellValue("DrugStrengthHardcodeUOM");
            row.CreateCell(21).SetCellValue("DrugPackagesizeHardcode");
            row.CreateCell(22).SetCellValue("DrugPackagesizeHardcodeUOM");
            row.CreateCell(23).SetCellValue("BrandCode");
            row.CreateCell(24).SetCellValue("InStock");
            row.CreateCell(25).SetCellValue("ATCCode");


            rowIndex++;

            //Get the facilities list
            var objDrugData = searchText != null ? _service.ExportFilteredDrugCodes(searchText, tableNumber) : _service.GetDrugList(Helpers.DefaultDrugTableNumber);
            //Get the facilities list
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

            foreach (var item in objDrugData)
            {
                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellType(CellType.Numeric);
                row.CreateCell(0).CellStyle = cellStyle;
                row.CreateCell(0).SetCellValue(Convert.ToDouble(item.Id));
                row.CreateCell(1).SetCellValue(item.DrugTableNumber);
                row.CreateCell(2).SetCellValue(item.DrugDescription);
                row.CreateCell(3).SetCellValue(item.DrugCode);
                //row.CreateCell(4).SetCellType(CellType.Numeric);
                //row.CreateCell(4).CellStyle = cellStyle;
                row.CreateCell(4).SetCellValue((item.DrugInsurancePlan));
                row.CreateCell(5).SetCellValue(item.DrugPackageName);
                row.CreateCell(6).SetCellValue(item.DrugGenericName);
                row.CreateCell(7).SetCellValue(item.DrugStrength);
                row.CreateCell(8).SetCellValue(item.DrugDosage);
                row.CreateCell(9).SetCellValue((item.DrugPackageSize));
                row.CreateCell(10).SetCellValue(item.DrugPricePublic);
                row.CreateCell(11).SetCellValue((item.DrugPricePharmacy));
                //row.CreateCell(12).SetCellValue((item.DrugUnitPricePublic));
                row.CreateCell(12).SetCellValue((item.DrugUnitPricePublic));
                row.CreateCell(13).SetCellValue((item.DrugUnitPricePharmacy));


                row.CreateCell(14).SetCellValue(item.DrugStatus);
                row.CreateCell(15).SetCellValue(item.DrugDeleteDate.ToString());
                row.CreateCell(16).SetCellValue(item.DrugLastChange.ToString());
                row.CreateCell(17).SetCellValue(item.DrugAgentName);
                row.CreateCell(18).SetCellValue((item.DrugManufacturer));
                row.CreateCell(19).SetCellValue(item.DrugStrengthHardcode);
                row.CreateCell(20).SetCellValue((item.DrugStrengthHardcodeUOM));
                row.CreateCell(21).SetCellValue((item.DrugPackagesizeHardcode));
                row.CreateCell(22).SetCellValue((item.DrugPackagesizeHardcodeUOM));
                row.CreateCell(23).SetCellValue((item.BrandCode));

                row.CreateCell(24).SetCellValue((item.InStock).ToString());
                row.CreateCell(25).SetCellValue((item.ATCCode));



                rowIndex++;
            }
            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                Response.Cookies.Add(cookie);
                workbook.Write(exportData);
                var saveAsFileName = string.Format("DrugCodesExcel-{0:d}.xls", currentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }

        [HttpPost]
        public string ImportDrugCodesToDB(HttpPostedFileBase file)
        {
            var returnStr = "";
            var dsResult = new DataSet();
            //HttpPostedFileBase objpostFile = file;
            if (file != null && file.ContentLength > 0)
            {
                if (file.FileName.Contains(".xlsx") || file.FileName.Contains(".xls"))
                {
                    Stream stream = file.InputStream;

                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    excelReader.IsFirstRowAsColumnNames = true;

                    dsResult = excelReader.AsDataSet();

                    excelReader.Close();

                    stream.Close();

                    returnStr = _service.ImportDrugCodesToDB(dsResult.Tables[0], Helpers.GetLoggedInUserId(),
                        Helpers.DefaultDrugTableNumber, "6");
                }
                else
                {
                    returnStr = "Imports only excel file`2";
                }
            }
            else
            {
                returnStr = "Please select a file`2";
            }
            return returnStr;
        }
    }
}
