using System.IO;
using System.Web;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Models;
using BillingSystem.Common;
using System.Web.Mvc;
using System;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using RazorPDF;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class InsurancePlansController : BaseController
    {
        private readonly IInsurancePlansService _service;
        private readonly IInsurancePolicesService _ipService;
        private readonly IInsuranceCompanyService _icService;

        public InsurancePlansController(IInsurancePlansService service, IInsurancePolicesService ipService, IInsuranceCompanyService icService)
        {
            _service = service;
            _ipService = ipService;
            _icService = icService;
        }


        /// <summary>
        /// Insurances the plans.
        /// </summary>
        /// <returns></returns>
        public ActionResult InsurancePlans()
        {
            var list = _service.GetInsurancePlanList(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId(), Helpers.GetLoggedInUserId());
            var insurancePlansView = new InsurancePlansView
            {
                InsurancePlansList = list,
                CurrentInsurancePlans = new InsurancePlans { IsActive = true }
            };
            return View(insurancePlansView);
        }

        /// <summary>
        /// Saves the insurance plans.
        /// </summary>
        /// <param name="m">The insurance plans model.</param>
        /// <returns></returns>
        public ActionResult SaveInsurancePlans(InsurancePlans m)
        {
            //Initialize the newId variable 
            var newId = -1;

            //Check if InsurancePlansViewModel 
            if (m != null)
            {
                if (m.InsurancePlanId > 0)
                {
                    m.ModifiedBy = Helpers.GetLoggedInUserId();
                    m.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    m.CreatedBy = Helpers.GetLoggedInUserId();
                    m.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }

                m.IsDeleted = false;
                m.IsActive = true;

                //Call the AddInsurancePlans Method to Add / Update current InsurancePlans
                newId = _service.AddUpdateInsurancePlans(m);
            }
            return Json(newId);
        }

        /// <summary>
        /// Bind all the InsurancePlans list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the InsurancePlans list object
        /// </returns>
        //[HttpPost]
        public ActionResult BindInsurancePlansList(bool showIsActive)
        {
            var list = _service.GetInsurancePlanList(showIsActive, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId(), Helpers.GetLoggedInUserId());
            return PartialView(PartialViews.PlansList, list);
        }

        /// <summary>
        /// Gets the insurance plan by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetInsurancePlanById(string id)
        {
            var current = _service.GetInsurancePlanById(Convert.ToInt32(id));
            return PartialView(PartialViews.insurancePlansAddEdit, current);
        }

        /// <summary>
        /// Reset the InsurancePlans View Model and pass it to InsurancePlansAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetInsurancePlansForm()
        {
            //Intialize the new object of InsurancePlans ViewModel
            var insurancePlansViewModel = new InsurancePlans { IsActive = true };

            //Pass the View Model as InsurancePlansViewModel to PartialView InsurancePlansAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.insurancePlansAddEdit, insurancePlansViewModel);
        }

        /// <summary>
        /// Delete the current InsurancePlans based on the InsurancePlans ID passed in the SharedViewModel
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult DeleteInsurancePlans(CommonModel model)
        {
            var isExist = _ipService.CheckInsurancePolicyExist(Convert.ToInt32(model.Id));
            if (isExist)
            {
                return Json("-1");
            }
            //Get InsurancePlans model object by current InsurancePlans ID
            var currentInsurancePlans = _service.GetInsurancePlanById(Convert.ToInt32(model.Id));

            //Check If InsurancePlans model is not null
            if (currentInsurancePlans != null)
            {
                currentInsurancePlans.IsDeleted = true;
                currentInsurancePlans.DeletedBy = Helpers.GetLoggedInUserId();
                currentInsurancePlans.DeletedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current InsurancePlans
                var result = _service.AddUpdateInsurancePlans(currentInsurancePlans);

                //return deleted ID of current InsurancePlans as Json Result to the Ajax Call.
                return Json(result);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Gets the insurance companies.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetInsuranceCompanies()
        {
            var list = _icService.GetInsuranceCompanies(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
            return Json(list);
        }

        /// <summary>
        /// Validates the plan name plan number.
        /// </summary>
        /// <param name="planName">Name of the plan.</param>
        /// <param name="planNumber">The plan number.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult ValidatePlanNamePlanNumber(string planName, string planNumber, int id, int insuranceCompanyId)
        {
            var result = _service.CheckDuplicateInsurancePlan(planName, planNumber, id, insuranceCompanyId);
            return Json(result);
        }

        /// <summary>
        /// Gets the plan name by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetPlanNameById(int id)
        {
            var result = _service.GetInsurancePlanById(id);
            return Json(result != null ? result.PlanName : string.Empty);
        }


        #region Export Feature
        /// <summary>
        /// Users the login activity PDF.
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportToPDF()
        {
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            var list = _service.GetInsurancePlanList(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId(), Helpers.GetLoggedInUserId());

            var pdf = new PdfResult(list, "ExportToPDF");

            pdf.ViewBag.Title = "Insurance Plans List";

            //Get the facilities list
            return pdf;
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <returns></returns>
        //public ActionResult ExportToExcel1()
        //{
        //    using (var _service = new InsurancePlansBal())
        //    {
        //        //Response.AddHeader("Content-Type", "application/vnd.ms-excel");


        //        Response.ContentType = "application/vnd.ms-excel";
        //        Response.AppendHeader("content-disposition",
        //                           "attachment; InsurancePlan.xls");

        //        //Get the facilities list
        //        var list = _service.GetInsurancePlanList(true);

        //        //Pass the ActionResult with List of InsuranceCompanyViewModel object to Partial View InsuranceCompanyList
        //        return PartialView(PartialViews.PlansListExportView, list);
        //    }
        //}


        public ActionResult ExportToExcel()
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("InsurancePlanExcel");
            var format = workbook.CreateDataFormat();
            sheet.CreateFreezePane(0, 1, 0, 1);
            sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:O1"));
            // Add header labels
            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("Company Name");
            row.CreateCell(1).SetCellValue("Plan Name");
            row.CreateCell(2).SetCellValue("Package ID Payer");
            row.CreateCell(3).SetCellValue("Begin Date");
            row.CreateCell(4).SetCellValue("End Date");
            row.CreateCell(5).SetCellValue("Description");
            rowIndex++;
            //Get the facilities list
            var objCompanyPlan = _service.GetInsurancePlanList(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId(), Helpers.GetLoggedInUserId());
            //Get the facilities list
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

            foreach (var item in objCompanyPlan)
            {
                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellType(CellType.Numeric);
                row.CreateCell(0).CellStyle = cellStyle;
                row.CreateCell(0).SetCellValue(item.InsuranceCompanyName);
                row.CreateCell(1).SetCellValue(item.InsurancePlan.PlanName);
                row.CreateCell(2).SetCellValue(item.InsurancePlan.PlanNumber);
                row.CreateCell(3).SetCellValue(Convert.ToString(item.InsurancePlan.PlanBeginDate));
                row.CreateCell(4).SetCellValue(Convert.ToString(item.InsurancePlan.PlanEndDate));
                row.CreateCell(5).SetCellValue(item.InsurancePlan.PlanDescription);
                rowIndex++;
            }
            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                Response.Cookies.Add(cookie);
                workbook.Write(exportData);
                var saveAsFileName = string.Format("InsurancePlanExcel-{0:d}.xls", currentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }

        #endregion
    }
}
