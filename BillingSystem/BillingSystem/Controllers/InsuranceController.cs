using System;
using System.IO;
using System.Linq;
using System.Web;
using BillingSystem.Models;
using BillingSystem.Common;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using RazorPDF;
using System.Collections.Generic;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class InsuranceController : BaseController
    {
        private readonly ICountryService _cService;
        /// <summary>
        /// Get the details of the InsuranceCompany View in the Model InsuranceCompany such as InsuranceCompanyList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model InsuranceCompany to be passed to View InsuranceCompany
        /// </returns>
        public ActionResult InsuranceCompany()
        {
            //Initialize the InsuranceCompany Communicator object
            using (var bal = new InsuranceCompanyBal())
            {
                //Get the facilities list
                var list = bal.GetInsuranceCompanies(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());

                //Intialize the View Model i.e. InsuranceCompanyView which is binded to Main View Index.cshtml under InsuranceCompany
                var view = new InsuranceCompanyView
                {
                    InsuranceCompaniesList = list,
                    CurrentInsurance = new InsuranceCompany { InsuranceCompanyId = 0, IsActive = true, IsDeleted = false }
                };


                //Pass the View Model in ActionResult to View InsuranceCompany
                return View(view);
            }
        }

        /// <summary>
        /// Bind all the InsuranceCompany list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the InsuranceCompany list object
        /// </returns>
        public ActionResult BindInsuranceCompanyList(bool showIsActive)
        {
            //Initialize the InsuranceCompany Communicator object
            using (var bal = new InsuranceCompanyBal())
            {
                //Get the insurance list
                var list = bal.GetInsuranceCompanies(showIsActive, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());

                //Pass the ActionResult with List of InsuranceCompanyViewModel object to Partial View InsuranceCompanyList
                return PartialView(PartialViews.InsuranceList, list);
            }
        }

        /// <summary>
        /// Add New or Update the InsuranceCompany based on if we pass the InsuranceCompany ID in the InsuranceCompanyViewModel object.
        /// </summary>
        /// <param name="m">pass the details of InsuranceCompany in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of InsuranceCompany row
        /// </returns>
        public ActionResult SaveInsuranceCompany(InsuranceCompany m)
        {
            //Initialize the newId variable 
            var newId = -1;

            //Check if InsuranceCompanyViewModel 
            if (m != null)
            {
                using (var insuranceCompanyBal = new InsuranceCompanyBal())
                {
                    var userId = Helpers.GetLoggedInUserId();
                    var currentDateTime = Helpers.GetInvariantCultureDateTime();
                    m.FacilityId = Helpers.GetDefaultFacilityId();
                    m.CorporateId = Helpers.GetDefaultCorporateId();

                    if (m.InsuranceCompanyId > 0)
                    {
                        m.ModifiedBy = userId;
                        m.ModifiedDate = currentDateTime;
                    }
                    else
                    {
                        m.CreatedBy = userId;
                        m.CreatedDate = currentDateTime;
                    }

                    //Call the AddInsuranceCompany Method to Add / Update current InsuranceCompany
                    newId = insuranceCompanyBal.SaveInsuranceCompany(m);
                }
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current InsuranceCompany in the view model by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetInsuranceCompany(int id)
        {
            using (var insuranceCompanyBal = new InsuranceCompanyBal())
            {
                //Call the AddInsuranceCompany Method to Add / Update current InsuranceCompany
                var currentInsuranceCompany = insuranceCompanyBal.GetInsuranceCompanyById(id);

                //Pass the ActionResult with the current InsuranceCompanyViewModel object as model to PartialView InsuranceCompanyAddEdit
                return PartialView(PartialViews.insuranceCompanyAddEdit, currentInsuranceCompany);
            }
        }

        /// <summary>
        /// Delete the current InsuranceCompany based on the InsuranceCompany ID passed in the SharedViewModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteInsuranceCompany(int id)
        {
            using (var insuranceCompanyBal = new InsuranceCompanyBal())
            {
                using (var planBal = new InsurancePlansBal())
                {
                    var IsExist = planBal.GetInsurancePlanByCompanyId(id);
                    if (IsExist)
                    {
                        return Json("-1");
                    }
                }
                //Get InsuranceCompany model object by current InsuranceCompany ID
                var currentInsuranceCompany = insuranceCompanyBal.GetInsuranceCompanyById(id);
                var patientInsuranceBal = new PatientInsuranceBal();

                if (patientInsuranceBal.IsInsuranceComapnyInUse(id))
                    return Json(0);

                //Check If InsuranceCompany model is not null
                if (currentInsuranceCompany != null)
                {
                    currentInsuranceCompany.IsDeleted = true;
                    currentInsuranceCompany.DeletedBy = Helpers.GetLoggedInUserId();
                    currentInsuranceCompany.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current InsuranceCompany
                    var result = insuranceCompanyBal.SaveInsuranceCompany(currentInsuranceCompany);

                    //return deleted ID of current InsuranceCompany as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the InsuranceCompany View Model and pass it to InsuranceCompanyAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetInsuranceCompanyForm()
        {
            //Intialize the new object of InsuranceCompany ViewModel
            var insuranceCompanyViewModel = new InsuranceCompany { IsActive = true, IsDeleted = false };

            //Pass the View Model as InsuranceCompanyViewModel to PartialView InsuranceCompanyAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.insuranceCompanyAddEdit, insuranceCompanyViewModel);
        }

        /// <summary>
        /// Validates the insurance company name insurance company license number.
        /// </summary>
        /// <param name="insuranceCompanyName">Name of the insurance company.</param>
        /// <param name="insuranceCompanyLicenseNumber">The insurance company license number.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult ValidateInsuranceCompanyNameInsuranceCompanyLicenseNumber(string insuranceCompanyName, string insuranceCompanyLicenseNumber, int id)
        {
            using (var insuranceCompanyBal = new InsuranceCompanyBal())
            {
                var result = insuranceCompanyBal.ValidateInsuranceCompanyNameInsuranceCompanyLicenseNumber(insuranceCompanyName, insuranceCompanyLicenseNumber, id);
                return Json(result);
            }
        }

        public JsonResult GetInsuranceCompaniesDropdownData()
        {
            var list = new List<SelectListItem>();
            using (var bal = new InsuranceCompanyBal())
            {
                var result = bal.GetInsuranceCompanies(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
                if (result.Count > 0)
                {
                    list.AddRange(result.Select(item => new SelectListItem
                    {
                        Text = item.InsuranceCompanyName,
                        Value = Convert.ToString(item.InsuranceCompanyId)
                    }));
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInsurancePayerId(int id)
        {
            using (var bal = new InsuranceCompanyBal())
            {
                var result = bal.GetPayerId(id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        #region Export Feature
        /// <summary>
        /// Users the login activity PDF.
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportToPDF(bool showInActive)
        {
            using (var bal = new InsuranceCompanyBal())
            {
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                var list = bal.GetInsuranceCompanies(showInActive, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());

                var pdf = new PdfResult(list, "ExportToPDF");
                pdf.ViewBag.Title = "Insurance Companies List";

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
        //    using (var insuranceCompanyBal = new InsuranceCompanyBal())
        //    {
        //        Response.AddHeader("Content-Type", "application/vnd.ms-excel");

        //        //Get the facilities list
        //        var list = insuranceCompanyBal.GetInsuranceCompanies();

        //        //Pass the ActionResult with List of InsuranceCompanyViewModel object to Partial View InsuranceCompanyList
        //        return PartialView(PartialViews.InsuranceListExportView, list);
        //    }
        //}

        public ActionResult BindCountriesWithCode()
        {
            var list = _cService.GetCountryWithCode();
            return Json(list);
        }




        public ActionResult ExportToExcel(bool showInActive)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("InsuranceCompanyExcel");
            var format = workbook.CreateDataFormat();
            sheet.CreateFreezePane(0, 1, 0, 1);
            sheet.SetAutoFilter(CellRangeAddress.ValueOf("A1:O1"));
            // Add header labels
            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("Payor ID");
            row.CreateCell(1).SetCellValue("Company Name");
            row.CreateCell(2).SetCellValue("Address");
            row.CreateCell(3).SetCellValue("City");
            row.CreateCell(4).SetCellValue("Zip Code");
            row.CreateCell(5).SetCellValue("Company Main Phone");
            row.CreateCell(6).SetCellValue("Email Address");

            rowIndex++;
            using (var iBal = new InsuranceCompanyBal())
            {
                //Get the facilities list
                var objCompany = iBal.GetInsuranceCompanies(showInActive, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
                //Get the facilities list
                var cellStyle = workbook.CreateCellStyle();
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

                foreach (var item in objCompany)
                {
                    row = sheet.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellType(CellType.Numeric);
                    row.CreateCell(0).CellStyle = cellStyle;
                    row.CreateCell(0).SetCellValue(item.InsuranceCompanyLicenseNumber);
                    row.CreateCell(1).SetCellValue(item.InsuranceCompanyName);
                    row.CreateCell(2).SetCellValue(item.InsuranceCompanyStreetAddress);
                    row.CreateCell(3).SetCellValue(item.InsuranceCompanyCity);
                    row.CreateCell(4).SetCellValue(Convert.ToString(item.InsuranceCompanyZipCode));
                    row.CreateCell(5).SetCellValue(item.InsuranceCompanyMainPhone);
                    row.CreateCell(6).SetCellValue(item.InsuranceCompanyEmailAddress);
                    rowIndex++;
                }
            }
            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                Response.Cookies.Add(cookie);
                workbook.Write(exportData);
                var saveAsFileName = string.Format("InsuranceCompanyExcel-{0:d}.xls", currentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }


        #endregion
    }
}