using System.IO;
using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using NPOI.HSSF.UserModel;
using System.Threading.Tasks;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class LabTestResultController : BaseController
    {
        private readonly ILabTestResultService _service;
        private readonly IFacilityService _fService;
        private readonly IUsersService _uService;

        public LabTestResultController(ILabTestResultService service, IFacilityService fService, IUsersService uService)
        {
            _service = service;
            _fService = fService;
            _uService = uService;
        }

        /// <summary>
        /// Get the details of the LabTestResult View in the Model LabTestResult such as LabTestResultList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model LabTestResult to be passed to View LabTestResult
        /// </returns>
        public ActionResult LabTestResultMain()
        {
            var facilityid = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            //Get the Entity list
            var labTestResultList = _service.GetLabTestResult();
            //--------------------------uncomment to display data via corporate id and facility id
            //var labTestResultList = _service.GetLabTestResultByCorporateFacility(corporateId,facilityid);

            //Intialize the View Model i.e. LabTestResultView which is binded to Main View Index.cshtml under LabTestResult
            var labTestResultView = new LabTestResultView
            {
                LabTestResultList = labTestResultList,
                CurrentLabTestResult = new Model.LabTestResult()
            };

            //Pass the View Model in ActionResult to View LabTestResult
            return View(labTestResultView);
        }

        /// <summary>
        /// Bind all the LabTestResult list 
        /// </summary>
        /// <returns>action result with the partial view containing the LabTestResult list object</returns>
        [HttpPost]
        public ActionResult BindLabTestResultList()
        {
            var labTestResultList = _service.GetLabTestResult();
            return PartialView(PartialViews.LabTestResultList, labTestResultList);
        }

        /// <summary>
        /// Add New or Update the LabTestResult based on if we pass the LabTestResult ID in the LabTestResultViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of LabTestResult row
        /// </returns>
        public ActionResult SaveLabTestResult(LabTestResult model)
        {
            //Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();
            var currentdatetime = _fService.GetInvariantCultureDateTime(Convert.ToInt32(Helpers.GetDefaultFacilityId()));
            var facilityid = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            //Check if Model is not null 
            if (model != null)
            {
                model.FacilityId = facilityid;
                model.CorporateId = corporateId;
                if (model.LabTestResultID > 0)
                {
                    model.ModifiedBy = userId;
                    model.Modifieddate = currentdatetime;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentdatetime;
                }
                //Call the AddLabTestResult Method to Add / Update current LabTestResult
                newId = _service.SaveLabTestResult(model);
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current LabTestResult in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetLabTestResult(int id)
        {
            //Call the AddLabTestResult Method to Add / Update current LabTestResult
            var currentLabTestResult = _service.GetLabTestResultByID(id);

            //Pass the ActionResult with the current LabTestResultViewModel object as model to PartialView LabTestResultAddEdit
            return PartialView(PartialViews.LabTestResultAddEdit, currentLabTestResult);
        }

        /// <summary>
        /// Delete the current LabTestResult based on the LabTestResult ID passed in the LabTestResultModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteLabTestResult(int id)
        {
            var currentLabTestResult = _service.GetLabTestResultByID(id);
            var result = _service.DeleteLabTestResult(currentLabTestResult);
            return Json(result);
        }

        /// <summary>
        /// Reset the LabTestResult View Model and pass it to LabTestResultAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetLabTestResultForm()
        {
            //Intialize the new object of LabTestResult ViewModel
            var labTestResultViewModel = new Model.LabTestResult();

            //Pass the View Model as LabTestResultViewModel to PartialView LabTestResultAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.LabTestResultAddEdit, labTestResultViewModel);
        }

        /// <summary>
        /// Downloads the import excel file.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> DownloadImportExcelFile()
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(Convert.ToInt32(Helpers.GetDefaultFacilityId()));

            var virtualPath = CommonConfig.LabtestResultSetExcelTemplate;
            var serverPath = Server.MapPath(virtualPath);
            var cookie = new HttpCookie("Downloaded", "True");
            Response.Cookies.Add(cookie);
            var userKToken = CommonConfig.GeneratePasswordResetToken(20, true);
            var emailSentStatus = await SendUserTokenAndSave(userKToken);
            //Response.WriteFile(serverPath);
            var saveAsFileName = string.Format("LabTestImportSheet-{0:d}.xlsm", currentDateTime)
                .Replace("/", "-");
            return File(serverPath, "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
        }

        /// <summary>
        /// Sends the user token and save.
        /// </summary>
        /// <param name="usertoken">The usertoken.</param>
        /// <returns></returns>
        private async Task<bool> SendUserTokenAndSave(string usertoken)
        {
            var msgBody = ResourceKeyValues.GetFileText("usertokentoaccess");
            Users userCM = null;
            var tokenExpired = true;
            var currentDate = _fService.GetInvariantCultureDateTime(Convert.ToInt32(Helpers.GetDefaultFacilityId()));
            userCM = _uService.GetUserById(Convert.ToInt32(Helpers.GetLoggedInUserId()));
            var facilityname = _fService.GetFacilityNameById(Convert.ToInt32(Helpers.GetDefaultFacilityId()));
            if (!string.IsNullOrEmpty(msgBody) && userCM != null)
            {
                userCM.UserToken = usertoken;
                if (userCM.TokenExpiryDate != null)
                {
                    tokenExpired = userCM.TokenExpiryDate < currentDate;
                    userCM.TokenExpiryDate = userCM.TokenExpiryDate > currentDate
                        ? userCM.TokenExpiryDate
                        : ((DateTime)userCM.TokenExpiryDate).AddMonths(1);
                }
                else
                {
                    userCM.TokenExpiryDate = currentDate.AddMonths(1);
                }
                msgBody = msgBody.Replace("{User}", userCM.UserName)
                   .Replace("{Facility-Name}", facilityname).Replace("{CodeValue}", usertoken).Replace("{TokenGeneratedon}", currentDate.ToShortDateString()).
                   Replace("{TokenExpireOn}", ((DateTime)userCM.TokenExpiryDate).ToShortDateString())
                   ;
                var userUpdated = tokenExpired ? _uService.AddUpdateUser(userCM, 0) : 0;
            }

            var emailInfo = new EmailInfo
            {
                VerificationTokenId = "",
                PatientId = 0,
                Email = userCM.Email,
                Subject = ResourceKeyValues.GetKeyValue("usertokenemailsubject"),
                VerificationLink = "",
                MessageBody = msgBody
            };
            var status = tokenExpired && (await MailHelper.SendEmailAsync(emailInfo));
            if (status)
            {
                userCM.UserToken = usertoken;
            }
            return status;
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportToExcel()
        {
            var currentDateTime = _fService.GetInvariantCultureDateTime(Convert.ToInt32(Helpers.GetDefaultFacilityId()));

            //Thread.Sleep(10000);
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("LabTestResultSet");

            // Add header labels
            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("CPT Code");
            row.CreateCell(1).SetCellValue("Test Name");
            row.CreateCell(2).SetCellValue("Specimen");
            row.CreateCell(3).SetCellValue("Gender");
            row.CreateCell(4).SetCellValue("Age From");
            row.CreateCell(5).SetCellValue("Age To");
            row.CreateCell(6).SetCellValue("Measurement Value");
            row.CreateCell(7).SetCellValue("Low Range Result");
            row.CreateCell(8).SetCellValue("High Range Result");
            row.CreateCell(9).SetCellValue("Good From");
            row.CreateCell(10).SetCellValue("Good To");
            row.CreateCell(11).SetCellValue("Caution From");
            row.CreateCell(12).SetCellValue("Caution To");
            row.CreateCell(13).SetCellValue("Bad From");
            row.CreateCell(14).SetCellValue("Bad To");
            rowIndex++;
            //Initialize the LabTestResult BAL object
            //Get the facilities list
            var labTestResultList = _service.GetLabTestResult();
            //Pass the ActionResult with List of LabTestResultViewModel object to Partial View LabTestResultList
            foreach (var item in labTestResultList)
            {
                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(item.LabTestResultCPTCode.ToString());
                row.CreateCell(1).SetCellValue(item.LabTestResultTestName);
                row.CreateCell(2).SetCellValue(item.SpecifimenString);
                row.CreateCell(3).SetCellValue(item.GenderString);
                row.CreateCell(4).SetCellValue(item.AgeFromString);
                row.CreateCell(5).SetCellValue(item.AgeToString);
                row.CreateCell(6).SetCellValue(item.MeasurementValueString);
                row.CreateCell(7).SetCellValue(item.LabTestResultLowRangeResult.ToString());
                row.CreateCell(8).SetCellValue(item.LabTestResultHighRangeResult.ToString());
                row.CreateCell(9).SetCellValue(item.LabTestResultGoodFrom.ToString());
                row.CreateCell(10).SetCellValue(item.LabTestResultGoodTo.ToString());
                row.CreateCell(11).SetCellValue(item.LabTestResultCautionFrom.ToString());
                row.CreateCell(12).SetCellValue(item.LabTestResultCautionTo.ToString());
                row.CreateCell(13).SetCellValue(item.LabTestResultBadFrom.ToString());
                row.CreateCell(14).SetCellValue(item.LabTestResultBadTo.ToString());
                rowIndex++;
            }

            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                Response.Cookies.Add(cookie);
                workbook.Write(exportData);
                string saveAsFileName = string.Format("LabTestResultSet-{0:d}.xls", currentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }
    }
}
