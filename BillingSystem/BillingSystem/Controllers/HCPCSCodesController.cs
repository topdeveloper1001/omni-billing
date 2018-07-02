using System.Web;
using System.Linq;
using BillingSystem.Models;
using BillingSystem.Model;
using System;
using System.IO;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class HCPCSCodesController : BaseController
    {
        private readonly IHCPCSCodesService _service;
        private readonly IGlobalCodeCategoryService _gcService;

        public HCPCSCodesController(IHCPCSCodesService service, IGlobalCodeCategoryService gcService)
        {
            _service = service;
            _gcService = gcService;
        }

        public ActionResult HCPCSCodes()
        {
            //Get the facilities list
            var hcpcsCodesList = _service.GetHCPCSCodesListOnDemand(1, Helpers.DefaultRecordCount, Helpers.DefaultHcPcsTableNumber);

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

            var hcpcsCodesList = _service.GetHCPCSCodesListOnDemand(1, Helpers.DefaultRecordCount, Helpers.DefaultHcPcsTableNumber);
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
                newId = _service.AddHCPCSCodes(HCPCSCodesModel, Helpers.DefaultHcPcsTableNumber);

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

            //Get the facilities list
            var hcpcsCodesList = _service.GetHCPCSCodes(Helpers.DefaultHcPcsTableNumber);
            var hcpcsCodesView = new HCPCSCodesView
            {
                HCPCSCodesList = hcpcsCodesList,
                CurrentHCPCSCodes = new HCPCSCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };
            //Pass the ActionResult with List of HCPCSCodesViewModel object to Partial View HCPCSCodesList
            return PartialView(PartialViews.HCPCSCodesList, hcpcsCodesView);

        }

        public ActionResult BindHCPCSCodesListNew(string blockNumber)
        {
            var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);

            //Get the facilities list
            var hcpcsCodesList =
                _service.GetHCPCSCodes(Helpers.DefaultHcPcsTableNumber).OrderByDescending(f => f.HCPCSCodesId).Take(takeValue).ToList();
            var hcpcsCodesView = new HCPCSCodesView
            {
                HCPCSCodesList = hcpcsCodesList,
                CurrentHCPCSCodes = new HCPCSCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };
            //Pass the ActionResult with List of HCPCSCodesViewModel object to Partial View HCPCSCodesList
            return PartialView(PartialViews.HCPCSCodesList, hcpcsCodesView);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockNumber"></param>
        /// <returns></returns>
        public JsonResult RebindBindHCPCSCodesList(int blockNumber, string tableNumber)
        {
            var recordCount = Helpers.DefaultRecordCount;

            var list = _service.GetHCPCSCodesListOnDemand(blockNumber, recordCount, Helpers.DefaultHcPcsTableNumber);
            var jsonResult = new
            {
                list,
                NoMoreData = list.Count < recordCount,
                UserId = Helpers.GetLoggedInUserId()
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Gets the HCPCS codes.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetHCPCSCodes(string id)
        {
            var current = _service.GetHCPCSCodesById(Convert.ToInt32(id));
            return PartialView(PartialViews.HCPCSCodesAddEdit, current);
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
            //Get HCPCSCodes model object by current HCPCSCodes ID
            var currentHcpcsCodes = _service.GetHCPCSCodesById(Convert.ToInt32(model.Id));

            //Check If HCPCSCodes model is not null
            if (currentHcpcsCodes != null)
            {
                currentHcpcsCodes.IsDeleted = true;
                currentHcpcsCodes.DeletedBy = Helpers.GetLoggedInUserId();
                currentHcpcsCodes.DeletedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current HCPCSCodes
                var result = _service.AddHCPCSCodes(currentHcpcsCodes, Helpers.DefaultHcPcsTableNumber);

                //return deleted ID of current HCPCSCodes as Json Result to the Ajax Call.
                return Json(result);
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

            var list = _gcService.GetGlobalCodeCategoriesRange(startRange, finishRange);
            return Json(list);
        }



        public ActionResult ExportHCPCSCodesToExcel(string searchText, string tableNumber)
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
            //Get the facilities list

            var onjHcpcsCodesData = searchText != null ? _service.ExportHCPCSCodes(searchText, tableNumber) : _service.GetHCPCSCodes(Helpers.DefaultHcPcsTableNumber);
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
            //Get the facilities list
            var hcpcsCodesList =
                _service.GetActiveInActiveHCPCSCodes(showInActive, Helpers.DefaultHcPcsTableNumber).OrderByDescending(f => f.HCPCSCodesId).Take(takeValue).ToList();
            var hcpcsCodesView = new HCPCSCodesView
            {
                HCPCSCodesList = hcpcsCodesList,
                CurrentHCPCSCodes = new HCPCSCodes(),
                UserId = Helpers.GetLoggedInUserId()
            };
            //Pass the ActionResult with List of HCPCSCodesViewModel object to Partial View HCPCSCodesList
            return PartialView(PartialViews.HCPCSCodesList, hcpcsCodesView);
        }


        public ActionResult GetHCPCSCodesOnEdit(string id)
        {
            var current = _service.GetHCPCSCodesById(Convert.ToInt32(id));
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
                CodeExpiryDate = current.CodeExpiryDate.GetShortDateString3(),
                CodeEffectiveDate = current.CodeEffectiveDate.GetShortDateString3(),
                current.CodeDescription,
                current.CodeCPTMUEValues
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


    }
}