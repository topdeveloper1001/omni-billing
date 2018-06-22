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

namespace BillingSystem.Controllers
{
    public class InsurancePlansController : BaseController
    {
        /// <summary>
        /// Insurances the plans.
        /// </summary>
        /// <returns></returns>
        public ActionResult InsurancePlans()
        {
            using (var bal = new InsurancePlansBal())
            {
                //Get the facilities list
                var list = bal.GetInsurancePlanList(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId(), Helpers.GetLoggedInUserId());

                //Intialize the View Model i.e. InsurancePlansView which is binded to Main View Index.cshtml under InsurancePlans
                var insurancePlansView = new InsurancePlansView
                {
                    InsurancePlansList = list,
                    CurrentInsurancePlans = new InsurancePlans { IsActive = true }
                };


                //Pass the View Model in ActionResult to View InsurancePlans
                return View(insurancePlansView);
            }
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
                using (var bal = new InsurancePlansBal())
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
                    newId = bal.AddUpdateInsurancePlans(m);
                }
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
            //Initialize the InsurancePlans Communicator object
            using (var bal = new InsurancePlansBal())
            {
                //Get the facilities list
                var list = bal.GetInsurancePlanList(showIsActive, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId(), Helpers.GetLoggedInUserId());

                //Pass the ActionResult with List of InsurancePlansViewModel object to Partial View InsurancePlansList
                return PartialView(PartialViews.PlansList, list);
            }
        }

        /// <summary>
        /// Gets the insurance plan by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetInsurancePlanById(string id)
        {
            using (var bal = new InsurancePlansBal())
            {
                var current = bal.GetInsurancePlanById(Convert.ToInt32(id));
                return PartialView(PartialViews.insurancePlansAddEdit, current);
            }
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
            using (var insurancePlansBal = new InsurancePlansBal())
            {
                using (var policeBal = new InsurancePolicesBal())
                {
                    var isExist = policeBal.CheckInsurancePolicyExist(Convert.ToInt32(model.Id));
                    if (isExist)
                    {
                        return Json("-1");
                    }
                }
                //Get InsurancePlans model object by current InsurancePlans ID
                var currentInsurancePlans = insurancePlansBal.GetInsurancePlanById(Convert.ToInt32(model.Id));

                //Check If InsurancePlans model is not null
                if (currentInsurancePlans != null)
                {
                    currentInsurancePlans.IsDeleted = true;
                    currentInsurancePlans.DeletedBy = Helpers.GetLoggedInUserId();
                    currentInsurancePlans.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current InsurancePlans
                    var result = insurancePlansBal.AddUpdateInsurancePlans(currentInsurancePlans);

                    //return deleted ID of current InsurancePlans as Json Result to the Ajax Call.
                    return Json(result);
                }
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
            using (var bal = new InsuranceCompanyBal())
            {
                var list = bal.GetInsuranceCompanies(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
                return Json(list);
            }
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
            using (var bal = new InsurancePlansBal())
            {
                var result = bal.CheckDuplicateInsurancePlan(planName, planNumber, id, insuranceCompanyId);
                return Json(result);
            }
        }

        /// <summary>
        /// Gets the plan name by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetPlanNameById(int id)
        {
            using (var bal = new InsurancePlansBal())
            {
                var result = bal.GetInsurancePlanById(id);
                return Json(result != null ? result.PlanName : string.Empty);
            }
        }


        #region Export Feature
        /// <summary>
        /// Users the login activity PDF.
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportToPDF()
        {
            using (var bal = new InsurancePlansBal())
            {
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                var list = bal.GetInsurancePlanList(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId(), Helpers.GetLoggedInUserId());

                var pdf = new PdfResult(list, "ExportToPDF");

                pdf.ViewBag.Title = "Insurance Plans List";

                //Get the facilities list
                return pdf;
            }
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <returns></returns>
        //public ActionResult ExportToExcel1()
        //{
        //    using (var bal = new InsurancePlansBal())
        //    {
        //        //Response.AddHeader("Content-Type", "application/vnd.ms-excel");


        //        Response.ContentType = "application/vnd.ms-excel";
        //        Response.AppendHeader("content-disposition",
        //                           "attachment; InsurancePlan.xls");

        //        //Get the facilities list
        //        var list = bal.GetInsurancePlanList(true);

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
            using (var iBal = new InsurancePlansBal())
            {
                //Get the facilities list
                var objCompanyPlan = iBal.GetInsurancePlanList(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId(), Helpers.GetLoggedInUserId());
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
