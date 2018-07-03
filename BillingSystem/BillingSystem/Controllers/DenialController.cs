using BillingSystem.Models;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class DenialController : BaseController
    {
        private readonly IDenialService _service;
        private readonly IGlobalCodeService _gService;

        public DenialController(IDenialService service, IGlobalCodeService gService)
        {
            _service = service;
            _gService = gService;
        }


        /// <summary>
        /// Denials this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Denial()
        {
            //Get the facilities list
            //var denialList = denialBal.GetDenial();
            var denialList = _service.GetListOnDemand(1, Helpers.DefaultRecordCount);


            //Intialize the View Model i.e. DenialView which is binded to Main View Index.cshtml under Denial
            var denialView = new DenialView
            {
                DenialList = denialList,
                CurrentDenial = new Denial()
            };

            //Pass the View Model in ActionResult to View Denial
            return View(denialView);
        }

        /// <summary>
        /// Saves the denial.
        /// </summary>
        /// <param name="model">The denial model.</param>
        /// <returns></returns>
        public ActionResult SaveDenial(Denial model)
        {
            //Initialize the newId variable 
            var newId = -1;

            //Check if DenialViewModel 
            if (model != null)
            {
                if (model.DenialSetNumber > 0)
                {
                    model.ModifiedBy = Helpers.GetLoggedInUserId();
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                //Call the AddDenial Method to Add / Update current Denial
                newId = _service.AddUpdateDenial(model);
            }
            return Json(newId);
        }

        /// <summary>
        /// Bind all the Denial list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the Denial list object
        /// </returns>
        //[HttpPost]
        public ActionResult BindDenialList(string blockNumber)
        {
            var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);

            //Get the facilities list
            var denialList = _service.BindDenialCodes(takeValue).ToList();

            //Pass the ActionResult with List of DenialViewModel object to Partial View DenialList
            return PartialView(PartialViews.DenialList, denialList);
        }

        /// <summary>
        /// Gets the denial.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetDenial(string id)
        {
            var current = _service.GetDenialById(Convert.ToInt32(id));
            return PartialView(PartialViews.DenialAddEdit, current);
        }

        /// <summary>
        /// Reset the Denial View Model and pass it to DenialAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetDenialForm()
        {
            //Intialize the new object of Denial ViewModel
            var denialViewModel = new Denial();

            //Pass the View Model as DenialViewModel to PartialView DenialAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.DenialAddEdit, denialViewModel);
        }

        /// <summary>
        /// Delete the current Denial based on the Denial ID passed in the SharedViewModel
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult DeleteDenial(CommonModel model)
        {
            //Get Denial model object by current Denial ID
            var currentDenial = _service.GetDenialById(Convert.ToInt32(model.Id));

            //Check If Denial model is not null
            if (currentDenial != null)
            {
                currentDenial.IsDeleted = true;
                currentDenial.DeletedBy = Helpers.GetLoggedInUserId();
                //currentDenial.DeletedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current Denial
                var result = _service.AddUpdateDenial(currentDenial);

                //return deleted ID of current Denial as Json Result to the Ajax Call.
                return Json(result);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Get Facilities list
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAuthorizationDenialsCode()
        {
            var facilities = _service.GetAuthorizationDenialsCode();
            if (facilities.Count > 0)
            {
                var list = new List<SelectListItem>();
                list.AddRange(facilities.Select(item => new SelectListItem
                {
                    Text = item.DenialCode + @" - " + item.DenialDescription,
                    Value = Convert.ToString(item.DenialSetNumber)
                }));
                return Json(list);
            }

            return Json(null);
        }


        /// <summary>
        /// Exports the denial to excel.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        //public ActionResult ExportDenialToExcel()
        //{

        //    var Denialtable = new System.Data.DataTable("DenialData");

        //    Denialtable.Columns.Add("DenialSetNumber", typeof(string));
        //    Denialtable.Columns.Add("InsuranceCompanyNumber", typeof(string));
        //    Denialtable.Columns.Add("DenialSetDescription", typeof(string));
        //    Denialtable.Columns.Add("DenialSetStartDate ", typeof(string));
        //    Denialtable.Columns.Add("DenialSetEndDate", typeof(string));
        //    Denialtable.Columns.Add("DenialCode", typeof(string));
        //    Denialtable.Columns.Add("DenialDescription", typeof(string));
        //    Denialtable.Columns.Add("Denial Explaination", typeof(string));
        //    Denialtable.Columns.Add("DenialStatus", typeof(string));
        //    Denialtable.Columns.Add("DenialType", typeof(string));
        //    Denialtable.Columns.Add("DenialStartDate", typeof(string));
        //    Denialtable.Columns.Add("DenialEndDate", typeof(string));


        //    var DenialBal = new DenialBal();
        //    //Get the facilities list

        //    var objDenialData = DenialBal.GetDenial();

        //    foreach (var item in objDenialData)
        //    {
        //        Denial model = new Denial();
        //        model.DenialSetNumber = item.DenialSetNumber;
        //        model.InsuranceCompanyNumber = item.InsuranceCompanyNumber;
        //        model.DenialSetDescription = item.DenialSetDescription;
        //        model.DenialSetStartDate = item.DenialSetStartDate;
        //        model.DenialSetEndDate = item.DenialSetEndDate;
        //        model.DenialCode = item.DenialCode;
        //        model.DenialDescription = item.DenialDescription;
        //        model.DenialExplain = item.DenialExplain;
        //        model.DenialStatus = item.DenialStatus;

        //        model.DenialType = item.DenialType;
        //        model.DenialStartDate = item.DenialStartDate;
        //        model.DenialEndDate = item.DenialEndDate;

        //        Denialtable.Rows.Add(
        //            model.DenialSetNumber,
        //            model.InsuranceCompanyNumber == null ? model.InsuranceCompanyNumber : model.InsuranceCompanyNumber,
        //            model.DenialSetDescription == null ? "" : model.DenialSetDescription,
        //            model.DenialSetStartDate ,
        //            model.DenialSetEndDate,
        //            model.DenialCode == null ? "" : model.DenialCode,
        //            model.DenialDescription == null ? "" : model.DenialDescription,
        //            model.DenialExplain == null ? "" : model.DenialDescription,
        //            model.DenialStatus == null ? "" : model.DenialStatus,
        //            model.DenialType == null ? "" : model.DenialType,
        //            model.DenialStartDate,
        //            model.DenialEndDate);
        //    }

        //    var grid = new GridView();
        //    grid.DataSource = Denialtable;
        //    grid.DataBind();

        //    Response.ClearContent();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment; filename=Denial.xls");
        //    Response.ContentType = "application/ms-excel";
        //    Response.Charset = "";
        //    StringWriter sw = new StringWriter();
        //    HtmlTextWriter htw = new HtmlTextWriter(sw);
        //    grid.RenderControl(htw);

        //    Response.Output.Write(sw.ToString());
        //    Response.Flush();
        //    Response.End();

        //    //return RedirectToAction("Denial");
        //    return null;
        //}


        public ActionResult ExportDenialToExcel(string searchText)
        {

            var denialtable = new System.Data.DataTable("DenialData");

            denialtable.Columns.Add("DenialSetNumber", typeof(string));
            denialtable.Columns.Add("InsuranceCompanyNumber", typeof(string));
            denialtable.Columns.Add("DenialSetDescription", typeof(string));
            denialtable.Columns.Add("DenialSetStartDate ", typeof(string));
            denialtable.Columns.Add("DenialSetEndDate", typeof(string));
            denialtable.Columns.Add("DenialCode", typeof(string));
            denialtable.Columns.Add("DenialDescription", typeof(string));
            denialtable.Columns.Add("Denial Explaination", typeof(string));
            denialtable.Columns.Add("DenialStatus", typeof(string));
            denialtable.Columns.Add("DenialType", typeof(string));
            denialtable.Columns.Add("DenialStartDate", typeof(string));
            denialtable.Columns.Add("DenialEndDate", typeof(string));


            var objDenialData = searchText != null ? _service.GetFilteredDenialCodes(searchText) : _service.GetDenial();

            foreach (var item in objDenialData)
            {
                var model = new Denial
                {
                    DenialSetNumber = item.DenialSetNumber,
                    InsuranceCompanyNumber = item.InsuranceCompanyNumber,
                    DenialSetDescription = item.DenialSetDescription,
                    DenialSetStartDate = item.DenialSetStartDate,
                    DenialSetEndDate = item.DenialSetEndDate,
                    DenialCode = item.DenialCode,
                    DenialDescription = item.DenialDescription,
                    DenialExplain = item.DenialExplain,
                    DenialStatus = item.DenialStatus,
                    DenialType = item.DenialType,
                    DenialStartDate = item.DenialStartDate,
                    DenialEndDate = item.DenialEndDate
                };

                denialtable.Rows.Add(
                    model.DenialSetNumber,
                    model.InsuranceCompanyNumber,
                    model.DenialSetDescription ?? "",
                    model.DenialSetStartDate,
                    model.DenialSetEndDate,
                    model.DenialCode ?? "",
                    model.DenialDescription ?? "",
                    model.DenialExplain == null ? "" : model.DenialDescription,
                    model.DenialStatus ?? "",
                    model.DenialType ?? "",
                    model.DenialStartDate,
                    model.DenialEndDate);
            }



            var grid = new GridView { DataSource = denialtable.Rows.Count > 0 ? denialtable : null };
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Denial.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            if (objDenialData.Count > 0)
            {
                var sw = new StringWriter();

                var htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }


            //return RedirectToAction("Denial");
            return null;
        }


        /// <summary>
        /// Gets the denail codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public JsonResult GetDenailCodes(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var list = _service.GetFilteredDenialCodes(text);
                var filteredList = list.Select(item => new
                {
                    ID = item.DenialCode,
                    Menu_Title = string.Format("{0} - {1}", item.DenialCode, item.DenialDescription),
                    Name = item.DenialDescription,
                    Code = item.DenialCode
                }).ToList();

                return Json(filteredList, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }

        /// <summary>
        /// Binds the global codes dropdown data.
        /// Denial Status: 5202
        /// Denial Types: 5203
        /// </summary>
        /// <returns></returns>

        public ActionResult BindGlobalCodesDropdownData()
        {
            var categories = new List<string> { "5202", "5203" };
            List<DropdownListData> list;
            list = _gService.GetListByCategoriesRange(categories);

            var jsonResult = new
            {
                listDenialStatus = list.Where(g => g.ExternalValue1.Equals("5202")).ToList(),
                listDenialType = list.Where(g => g.ExternalValue1.Equals("5203")).ToList(),
                //listServiceCodes = listData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetDenialCodesData(int id)
        {
            var current = _service.GetDenialById(Convert.ToInt32(id));
            var jsonData = new
            {
                current.DenialCode,
                current.DenialDescription,
                DenialEndDate = current.DenialEndDate.GetShortDateString3(),
                current.DenialExplain,
                current.DenialSetDescription,
                DenialSetEndDate = current.DenialSetEndDate.GetShortDateString3(),
                current.DenialSetNumber,
                DenialSetStartDate = current.DenialSetStartDate.GetShortDateString3(),
                DenialStartDate = current.DenialStartDate.GetShortDateString3(),
                current.DenialStatus,
                current.DenialType,


            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }



        public JsonResult RebindDenialCodeList(int blockNumber)
        {
            var recordCount = Helpers.DefaultRecordCount;

            var list = _service.GetListOnDemand(blockNumber, recordCount);
            var jsonResult = new
            {
                list,
                NoMoreData = list.Count < recordCount,
                UserId = Helpers.GetLoggedInUserId()
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

    }
}