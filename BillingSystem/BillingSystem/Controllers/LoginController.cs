using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Filters;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using CaptchaMvc.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BillingSystem.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class LoginController : Controller
    {
        private readonly IUsersService _uService;
        private readonly IPatientLoginDetailService _pldService;
        private readonly ILoginTrackingService _ltService;
        private readonly ITabsService _tService;
        private readonly IModuleAccessService _maService;
        private readonly IPatientInfoService _piService;
        private readonly IFacilityStructureService _fsService;
        private readonly IEncounterService _eService;
        private readonly IFacilityService _fService;
        private readonly ISchedulingService _sService;
        private readonly IAppointmentTypesService _atService;

        public LoginController(IUsersService uService, IPatientLoginDetailService pldService
            , ILoginTrackingService ltService, ITabsService tService, IModuleAccessService maService, ISchedulingService sService
            , IAppointmentTypesService atService)
        {
            _uService = uService;
            _pldService = pldService;
            _ltService = ltService;
            _tService = tService;
            _maService = maService;
            _sService = sService;
            _atService = atService;
        }

        /// <summary>
        /// Users the login.
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            //Not in Use
            //var pwd = EncryptDecrypt.GetDecryptedData("O1yC58YWrr/HGFQyfok2Gw==", "");
            Session.RemoveAll();
            return View();
        }


        /// <summary>
        /// Patients the login.
        /// </summary>
        /// <returns></returns>
        public ActionResult PatientLogin()
        {
            Session.RemoveAll();
            return View();
        }

        [HttpPost]
        public ActionResult PatientLogin(PatientLoginDetail model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.Email))
            {
                var flag = true;
                var currentPatient = _pldService.GetPatientLoginDetailsByEmail(model.Email);
                if (currentPatient != null)
                {
                    var patientId = currentPatient.PatientId ?? 0;
                    var enPwd = EncryptDecrypt.Encrypt(model.Password).ToLower().Trim();
                    if (string.IsNullOrEmpty(currentPatient.Password))
                    {
                        ViewBag.check = (int)LoginResponseTypes.AccountNotActivated;
                        return View();
                    }

                    if (currentPatient.Password.ToLower().Trim().Equals(enPwd))
                    {
                        if (currentPatient.FailedLoginAttempts.HasValue &&
                            currentPatient.FailedLoginAttempts.Value == 3)
                        {
                            var failedlogin = Convert.ToDateTime(currentPatient.LastInvalidLogin);
                            var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
                            if (timespan.TotalMinutes < 30)
                            {
                                flag = false;
                                ViewBag.check = (int)LoginResponseTypes.FailedAttemptsOver;
                            }
                        }

                        if (flag)
                        {
                            var loginTrackingVm = new LoginTracking
                            {
                                ID = patientId,
                                LoginTime = Helpers.GetInvariantCultureDateTime(),
                                LoginUserType = (int)LoginTrackingTypes.UserLogin,
                                FacilityId = currentPatient.PatientId,
                                CorporateId = currentPatient.CorporateId,
                                IsDeleted = false,
                                IPAddress = Helpers.GetUser_IP(),
                                CreatedBy = patientId,
                                CreatedDate = Helpers.GetInvariantCultureDateTime()
                            };

                            _ltService.AddUpdateLoginTrackingData(loginTrackingVm);
                            _pldService.UpdatePatientLoginFailedLog(patientId, 0, Helpers.GetInvariantCultureDateTime());


                            var objSession = Session[SessionNames.SessionClass.ToString()] != null
                                ? Session[SessionNames.SessionClass.ToString()] as SessionClass
                                : new SessionClass();

                            objSession.FirstTimeLogin = _ltService.IsFirstTimeLoggedIn(patientId,
                                (int)LoginTrackingTypes.PatientLogin);

                            objSession.FacilityNumber = currentPatient.FacilityNumber;
                            objSession.UserName = currentPatient.PatientName;
                            objSession.UserId = patientId;
                            objSession.SelectedCulture = CultureInfo.CurrentCulture.Name;
                            objSession.LoginUserType = (int)LoginTrackingTypes.PatientLogin;
                            objSession.UserIsAdmin = false;
                            objSession.RoleId = 0;
                            objSession.RoleName = "Patient Access";

                            objSession.MenuSessionList = _tService.GetPatientTabsListData(patientId);


                            Session[SessionNames.SessoionModuleAccess.ToString()] =
                                                            _maService.GetModulesAccessList(currentPatient.CorporateId, currentPatient.FacilityId);

                            Session[SessionNames.SessionClass.ToString()] = objSession;
                            return RedirectToAction("Index", "PatientPortal", new { pId = patientId });
                        }
                    }
                    else
                    {
                        if (currentPatient.Password == null || !currentPatient.Password.Equals(EncryptDecrypt.Encrypt(currentPatient.Password)))
                            ViewBag.check = (int)LoginResponseTypes.Failed;
                        else if (currentPatient.IsDeleted != false)
                            ViewBag.check = (int)LoginResponseTypes.IsDeleted;

                        else if (!string.IsNullOrEmpty(currentPatient.PatientName) && ViewBag.check == (int)LoginResponseTypes.Failed)
                        {
                            if (currentPatient.FailedLoginAttempts < 3 || currentPatient.FailedLoginAttempts == null)
                            {
                                var failedlogin = currentPatient.LastInvalidLogin ?? Helpers.GetInvariantCultureDateTime();
                                var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
                                var failedattempts = timespan.TotalMinutes < 30
                                    ? Convert.ToInt32(currentPatient.FailedLoginAttempts) + 1
                                    : 1;
                                _pldService.UpdatePatientLoginFailedLog(patientId, failedattempts,
                                    Helpers.GetInvariantCultureDateTime());
                            }
                            else if (currentPatient.FailedLoginAttempts == 3)
                            {
                                var failedlogin = currentPatient.LastInvalidLogin ?? Helpers.GetInvariantCultureDateTime();
                                var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
                                if (timespan.TotalMinutes < 30)
                                    flag = false;
                            }
                        }

                        if (flag == false)
                            ViewBag.check = (int)LoginResponseTypes.Failed;
                    }
                }
                else
                {
                    ViewBag.check = (int)LoginResponseTypes.Failed;
                }
            }
            else
            {
                ViewBag.check = (int)LoginResponseTypes.UnknownError;
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(Users model)
        {
            UsersViewModel user = null;
            var statusId = 0;

            //Check the Captcha Code below
            if (!this.IsCaptchaValid(string.Empty))
                statusId = -5;

            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                statusId = -3;

            if (statusId == 0)
                user = _uService.AuthenticateUser(model.UserName, model.Password, DateTime.Now, Helpers.GetUser_IP()
                    , Convert.ToString((int)LoginTrackingTypes.UserLogin), out statusId);

            switch (statusId)
            {
                case -1:
                    ViewBag.check = Convert.ToString(LoginResponseTypes.OddLoginTiming);
                    break;
                case -2:
                case -3:
                    ViewBag.check = Convert.ToString(LoginResponseTypes.Failed);
                    break;
                case -4:
                    ViewBag.check = Convert.ToString(LoginResponseTypes.FailedAttemptsOver);
                    break;
                case -5:
                    ViewBag.check = Convert.ToString(LoginResponseTypes.CaptchaFailed);
                    break;
                case 0:
                    ViewBag.check = Convert.ToString(LoginResponseTypes.Success);

                    var objSession = new SessionClass
                    {
                        CountryId = user.CountryID,
                        UserEmail = user.Email,
                        FacilityNumber = user.FacilityNumber,
                        UserName = user.UserName,
                        UserId = user.UserID,
                        SelectedCulture = CultureInfo.CurrentCulture.Name,
                        LoginUserType = (int)LoginTrackingTypes.UserLogin,
                        FacilityId = user.FacilityId.Value,
                        CorporateId = user.CorporateId.Value,
                        FacilityName = user.FacilityName,
                        UserIsAdmin = user.AdminUser.Value,
                        FirstTimeLogin = user.IsFirstTimeLoggedIn,
                        TimeZone = user.TimeZone,
                        DefaultCountryId = user.DefaultCountryId,
                        IsActiveEncountersAccessible = user.IsActiveEncountersAccessible,
                        IsBillHeaderViewAccessible = user.IsBillHeaderViewAccessible,
                        IsAuthorizationAccessible = user.IsAuthorizationAccessible,
                        IsEhrAccessible = user.IsEhrAccessible,
                        IsPatientSearchAccessible = user.IsPatientSearchAccessible,
                        SchedularAccessible = user.SchedularAccessible,
                        CptTableNumber = user.CptTableNumber,
                        BillEditRuleTableNumber = user.BillEditRuleTableNumber,
                        DiagnosisCodeTableNumber = user.DiagnosisCodeTableNumber,
                        DrgTableNumber = user.DrgTableNumber,
                        DrugTableNumber = user.DrugTableNumber,
                        HcPcsTableNumber = user.HcPcsTableNumber
                    };

                    if (user.RolesCount == 1)
                    {
                        objSession.RoleKey = user.RoleKey;
                        objSession.RoleName = user.RoleName;
                        objSession.RoleId = user.RoleId;
                        objSession.MenuSessionList = user.Tabs;
                    }

                    Session[SessionNames.SessionClass.ToString()] = objSession;

                    //Send Email Async
                    var result = await MailHelper.SendEmailAsync(objRequest: new EmailInfo
                    {
                        DisplayName = "Services Dot - Admin",
                        Email = "aj@gttechsolutions.com",
                        Subject = $"Logged-In successfully at {Request.Url.AbsoluteUri}",
                        MessageBody = $"Hello, <br/> You have successfully login at {DateTime.UtcNow}. <br/>",
                    });

                    return RedirectToAction("Welcome", "Home");
            }

            return View(new Users());
        }

        [CustomAuth]
        public ActionResult GetPatientList()
        {
            var list = new List<DropdownListData>();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var result = _piService.GetPatientList(facilityId);
            if (result.Count > 0)
            {
                list.AddRange(result.Select(item => new DropdownListData
                {
                    Text = string.Format("{0} {1}", item.PersonFirstName, item.PersonLastName),
                    Value = Convert.ToString(item.PatientID),
                    ExternalValue1 = item.PersonEmiratesIDNumber,
                    ExternalValue2 = item.PersonMedicalRecordNumber
                }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public async Task<JsonResult> IsPatientEmailValid(string emailid)
        {
            var status = ResourceKeyValues.GetKeyValue("invalidemailid");
            if (_piService.CheckIfEmailExists(emailid))
            {
                var statusobj = await SendForgotPasswordEmail(emailid);
                status = statusobj
                    ? ResourceKeyValues.GetKeyValue("resetpasswordemailsuccess")
                    : ResourceKeyValues.GetKeyValue("resetpasswordemailfailure");
            }
            var jsonStatus = new { status };
            return Json(jsonStatus, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        private async Task<bool> SendForgotPasswordEmail(string emailid)
        {
            var msgBody = ResourceKeyValues.GetFileText("patientforgotpasswordemail");
            PatientInfo patientm = null;
            PatientInfoCustomModel patientVm = null;
            var verficationTokenId = CommonConfig.GeneratePasswordResetToken(14, false);
            var patientlogindetailcustomModel = _pldService.GetPatientLoginDetailsByEmail(emailid);

            patientm = _piService.GetPatientDetailByEmailid(emailid);
            patientVm = _piService.GetPatientDetailsByPatientId(Convert.ToInt32(patientm.PatientID));


            patientlogindetailcustomModel.TokenId = verficationTokenId;
            var updatedId = _pldService.SavePatientLoginDetails(patientlogindetailcustomModel);
            if (!string.IsNullOrEmpty(msgBody) && patientVm != null)
            {
                msgBody = msgBody.Replace("{Patient}", patientVm.PatientName)
                    .Replace("{Facility-Name}", patientVm.FacilityName);
            }
            var emailInfo = new EmailInfo
            {
                VerificationTokenId = verficationTokenId,
                PatientId = patientm.PatientID,
                Email = emailid,
                Subject = ResourceKeyValues.GetKeyValue("verificationemailsubject"),
                VerificationLink = "/Login/ResetPassword",
                MessageBody = msgBody
            };
            var status = await MailHelper.SendEmailAsync(emailInfo);
            return status;
        }
        private async Task<bool> SendNewPasswordForPatientLoginPortal(int patientId, string email, string newPassword)
        {
            var msgBody = ResourceKeyValues.GetFileText("newpasswordemail");
            PatientInfoCustomModel patientVm = null;
            patientVm = _piService.GetPatientDetailsByPatientId(Convert.ToInt32(patientId));

            if (!string.IsNullOrEmpty(msgBody) && patientVm != null)
            {
                msgBody = msgBody.Replace("{Patient}", patientVm.PatientName)
                    .Replace("{Facility-Name}", patientVm.FacilityName).Replace("{Passwordval}", newPassword);
            }
            var emailInfo = new EmailInfo
            {
                VerificationTokenId = "",
                PatientId = patientId,
                Email = email,
                Subject = ResourceKeyValues.GetKeyValue("newpasswordemailsubject"),
                VerificationLink = "",
                MessageBody = msgBody
            };
            var status = await MailHelper.SendEmailAsync(emailInfo);
            return status;
        }
        private bool IsPatientDataValid(int patientId, DateTime? birthdate, string emirateid)
        {
            var patientInfoObj = _piService.GetPatientInfoById(patientId);
            var emirateidLastDigits = GetLastFourDigits(patientInfoObj.PersonEmiratesIDNumber);
            return (patientInfoObj.PersonBirthDate == birthdate &&
                    emirateidLastDigits == emirateid);
        }


        private string GetLastFourDigits(string idnumber)
        {
            var lastFourdigits = idnumber.Length == 18 ? idnumber.Replace("-", string.Empty).Substring(11, 4) : "";
            return lastFourdigits;
        }
        [AllowAnonymous]
        public async Task<JsonResult> ResetUserPassword(PatientLoginDetailCustomModel vm)
        {
            var message = string.Empty;
            var error = string.Empty;
            var userId = Helpers.GetLoggedInUserId();
            var patinetlogindetailObj = _pldService.GetPatientLoginDetailByPatientId(Convert.ToInt32(vm.PatientId));
            var newPassword = CommonConfig.GeneratePasswordResetToken(8, true);
            if (vm.PatientPortalAccess)
            {
                if (!IsPatientDataValid(Convert.ToInt32(vm.PatientId), vm.BirthDate, vm.EmriateId.Trim()))
                {
                    error = "1";
                    message = "Invalid data!";
                    var jsonStatus1 = new { message, error };
                    return Json(jsonStatus1, JsonRequestBehavior.AllowGet);
                }
                patinetlogindetailObj.Password = EncryptDecrypt.Encrypt(newPassword).ToLower().Trim();
                patinetlogindetailObj.TokenId = string.Empty;
                //Generate the 8-Digit Code
                var emailSentStatus = await SendNewPasswordForPatientLoginPortal(Convert.ToInt32(vm.PatientId),
                    vm.Email, newPassword);

                //Is Email Sent Now
                vm.ExternalValue1 = emailSentStatus ? "1" : "0";
                error = emailSentStatus ? "" : "0";
                message = emailSentStatus
                    ? ResourceKeyValues.GetKeyValue("newpasswordemailsuccess")
                    : ResourceKeyValues.GetKeyValue("newpasswordemailfailure");
            }

            var updatedId = _pldService.SavePatientLoginDetails(patinetlogindetailObj);
            if (updatedId <= 0)
                message = ResourceKeyValues.GetKeyValue("msgrecordsnotsaved");

            var jsonStatus = new { message, updatedId, vm.ExternalValue1, error };
            return Json(jsonStatus, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [CustomAuth]
        public ActionResult GetFacilityDeapartments()
        {
            var loggedinFacility = Helpers.GetDefaultFacilityId();
            var list = new List<SelectListItem>();

            var facilityDepartments = _fsService.GetFacilityDepartments(Helpers.GetSysAdminCorporateID(), loggedinFacility.ToString());
            if (facilityDepartments.Any())
            {
                list.AddRange(facilityDepartments.Select(item => new SelectListItem
                {
                    Text = string.Format(" {0} ", item.FacilityStructureName),
                    Value = Convert.ToString(item.FacilityStructureId)
                }));
            }


            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [CustomAuth]
        public ActionResult GetOldEncounterList(int pid)
        {
            var patientEncounterlist = _eService.GetEncounterListByPatientId(pid);
            return Json(patientEncounterlist);
        }
        [CustomAuth]
        public ActionResult GetEncountersListByPatientId(int patientId)
        {
            var list = new List<DropdownListData>();
            var result = _eService.GetEncounterListByPatientId(patientId).ToList();
            if (result.Any())
            {
                list.AddRange(result.Select(item => new DropdownListData
                {
                    Text = item.EncounterNumber,
                    Value = Convert.ToString(item.EncounterID),
                    ExternalValue1 = item.EncounterTypeName,
                    ExternalValue2 = item.EncounterPatientTypeName
                }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// User Service Calls
        /// </summary>
        /// <returns></returns>

        public ActionResult UserResetPassword(string e, string vtoken)
        {
            var usersObj = _uService.GetUserByEmailAndToken(e, vtoken);
            usersObj.CodeValue = vtoken;
            usersObj.OldPassword = usersObj.Password;
            return View(usersObj);
        }
        [AllowAnonymous]
        public ActionResult ForgotPassword(string e, string vtoken)
        {
            var usersObj = _uService.GetUserByEmail(e);
            e = !string.IsNullOrEmpty(e) ? e.ToLower().Trim() : string.Empty;
            if (usersObj.ResetToken == vtoken && usersObj.Email.ToLower().Equals(e))
            {
                return View(usersObj);
            }
            return Content("This page is invalid. Please try again later!");

        }
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public async Task<ActionResult> SendForgotPasswordLink(string email)
        {
            string result;
            var msgBody = ResourceKeyValues.GetFileText("forgotpwdtemplate");

            var userDetail = _uService.GetUserByEmailWithoutDecryption(email);
            if (userDetail != null && !string.IsNullOrEmpty(userDetail.Email) && userDetail.Email.ToLower().Trim().Equals(email.ToLower().Trim()))
            {
                result = "1";
                userDetail.ResetToken = CommonConfig.GenerateLoginCode(8, false);
                if (!string.IsNullOrEmpty(msgBody))
                    msgBody = msgBody.Replace("{Patient}",
                        string.Format("{0} {1}", userDetail.FirstName, userDetail.LastName))
                        .Replace("{CodeValue}", userDetail.ResetToken);

                var emailInfo = new EmailInfo
                {
                    VerificationTokenId = userDetail.ResetToken,
                    PatientId = userDetail.UserID,
                    Email = email,
                    Subject = ResourceKeyValues.GetKeyValue("resetpasswordsubject"),
                    VerificationLink = "/Login/ForgotPassword",
                    MessageBody = msgBody
                };

                if (result == "1")
                {
                    var status = await MailHelper.SendEmailAsync(emailInfo);
                    result = status ? "1" : "-2";
                }

                if (result == "1")
                {
                    var updatedId = _uService.UpdateUser(userDetail);
                    result = updatedId == userDetail.UserID ? "1" : "-3";
                }
            }
            else
                result = "-1";

            var jsonStatus = new { result };
            return Json(jsonStatus, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> SendResetPasswordLink(string email)
        {
            var msgBody = ResourceKeyValues.GetFileText("ResetPasswordTemplate");
            var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
            var oUsers = new Users
            {
                ResetToken = CommonConfig.GenerateLoginCode(8, false)
            };
            if (session != null)
            {
                oUsers.UserID = session.UserId;
                oUsers.FacilityId = session.FacilityId;
                oUsers.UserName = session.UserName;
            }
            if (!string.IsNullOrEmpty(msgBody))
            {
                if (session != null)
                {
                    var facilityName = _fService.GetFacilityNameByNumber(session.FacilityNumber);
                    msgBody = msgBody.Replace("{Patient}", oUsers.UserName)
                        .Replace("{Facility-Name}", Convert.ToString(facilityName)).Replace("{CodeValue}", oUsers.ResetToken);
                }
            }
            var emailInfo = new EmailInfo
            {
                VerificationTokenId = oUsers.ResetToken,
                PatientId = oUsers.UserID,
                Email = email,
                Subject = ResourceKeyValues.GetKeyValue("resetpasswordsubject"),
                VerificationLink = "/Security/UserResetPassword",
                MessageBody = msgBody
            };
            var status = await MailHelper.SendEmailAsync(emailInfo);
            var message = status
                        ? ResourceKeyValues.GetKeyValue("resetpasswordemailsuccess")
                        : ResourceKeyValues.GetKeyValue("resetpasswordemailfailure");

            var userObj = _uService.GetUserById(oUsers.UserID);
            userObj.ResetToken = oUsers.ResetToken;
            userObj.Password = EncryptDecrypt.GetEncryptedData(userObj.Password, "");
            var updatedId = _uService.UpdateUser(userObj);
            var jsonStatus = new { message };
            return Json(jsonStatus, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public string ResetNewPassword(UsersViewModel oUsersViewModel)
        {
            var userObj = _uService.GetUserById(oUsersViewModel.UserID);
            userObj.ResetToken = string.Empty;
            userObj.Password = EncryptDecrypt.GetEncryptedData(oUsersViewModel.NewPassword, "");
            _uService.UpdateUser(userObj);
            return "Password reset successfully";
        }
        [AllowAnonymous]
        public JsonResult SavePatientLoginDetails(PatientLoginDetailCustomModel vm)
        {
            int updatedId;
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            vm.TokenId = CommonConfig.GeneratePasswordResetToken(14, false);
            vm.ModifiedBy = vm.PatientId;
            vm.ModifiedDate = currentDateTime;
            vm.PatientPortalAccess = true;
            vm.Password = EncryptDecrypt.Encrypt(vm.Password).ToLower().Trim();
            if (vm.DeleteVerificationToken)
                vm.TokenId = string.Empty;

            if (!vm.NewCodeValue.Equals(vm.CodeValue))
            {
                var re = new
                {
                    Status = -1
                };
                return Json(re, JsonRequestBehavior.AllowGet);
            }

            updatedId = _pldService.SavePatientLoginDetails(vm);

            var jsonStatus1 = new
            {
                Message = ResourceKeyValues.GetKeyValue("patientportalisaccessiblemessage"),
                UpdatedId = updatedId,
                Status = 2
            };
            return Json(jsonStatus1, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Verifies the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="vtoken">The vtoken.</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Verify(string e, string vtoken)
        {
            var message = ResourceKeyValues.GetKeyValue("invalidverificationtokenid");
            if (!string.IsNullOrEmpty(e) && string.IsNullOrEmpty(vtoken))
                return Content(message);

            vtoken = vtoken.ToLower().Trim();
            var result = _pldService.GetPatientLoginDetailsByEmail(e);
            if (result != null && !string.IsNullOrEmpty(result.TokenId) &&
                result.TokenId.ToLower().Trim().Equals(vtoken))
                return View("Verify", result);
            return Content(message);
        }
        [AllowAnonymous]
        public ActionResult ResetPassword(string e, string vtoken)
        {
            var message = ResourceKeyValues.GetKeyValue("invalidverificationtokenid");
            if (!string.IsNullOrEmpty(e) && string.IsNullOrEmpty(vtoken))
                return Content(message);

            vtoken = vtoken.ToLower().Trim();
            var result = _pldService.GetPatientLoginDetailsByEmail(e);
            if (result != null && !string.IsNullOrEmpty(result.TokenId) &&
                result.TokenId.ToLower().Trim().Equals(vtoken))
                return View("ResetPassword", result);
            return Content(message);
        }

        public async Task<ActionResult> ConfirmationView(string st, string vtoken, int patientId, string physicianId, bool bit)
        {
            //Check If Verification Token is there
            var validRequest = !string.IsNullOrEmpty(vtoken);
            var list = new List<SchedulingCustomModel>();
            var email = string.Empty;
            var patientEmail = string.Empty;
            if (validRequest)
            {
                list = _sService.GetSchedulingListByPatient(patientId, physicianId, vtoken, out patientEmail);

                //Check if list contains items.
                validRequest = list.Count > 0;

                if (validRequest)
                {
                    list.ForEach(a =>
                    {
                        a.Status = st;
                        a.ExtValue4 = CommonConfig.GenerateLoginCode(8, false);
                        a.ModifiedBy = patientId;
                        a.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    });
                    validRequest = _sService.UpdateSchedulingEvents(list);

                    if (st == "2" && bit)//After Patient Approval Mail Will Be sent to Physician
                    {
                        var appointmentType = string.Empty;
                        foreach (var item in list)
                        {
                            var app = _atService.GetAppointmentTypesById(Convert.ToInt32(item.TypeOfProcedure));
                            appointmentType = app != null ? app.Name : string.Empty;
                            item.AppointmentType = appointmentType;
                        }

                        var objPhysician = _uService.GetPhysicianById(Convert.ToInt32(physicianId));
                        var validRequest1 = objPhysician != null && objPhysician.UserId > 0;
                        if (validRequest1)
                        {
                            email = _uService.GetUserEmailByUserId(Convert.ToInt32(objPhysician.UserId));
                            await Helpers.SendAppointmentNotification(list, email,
                                  Convert.ToString((int)SchedularNotificationTypes.appointmentapprovaltophysician),
                                  patientId, Convert.ToInt32(physicianId), 2);

                        }
                    }
                    if (st == "4" && bit)//After Physician Cancel mail will be sent to Patient
                    {
                        await Helpers.SendAppointmentNotification(list, patientEmail,
                                      Convert.ToString((int)SchedularNotificationTypes.physiciancancelappointment),
                                      patientId, Convert.ToInt32(physicianId), 5);
                    }
                    if (st == "2" && bit != true)//After Physician Approvel Mail sent to Patient
                    {
                        var appointmentType = string.Empty;
                        foreach (var item in list)
                        {
                            var app = _atService.GetAppointmentTypesById(Convert.ToInt32(item.TypeOfProcedure));
                            appointmentType = app != null ? app.Name : string.Empty;
                            item.AppointmentType = appointmentType;
                        }
                        await Helpers.SendAppointmentNotification(list, patientEmail,
                                       Convert.ToString((int)SchedularNotificationTypes.physicianapporovelemail),
                                       patientId, Convert.ToInt32(physicianId), 4);
                    }
                }
            }

            if (validRequest)
                return View(list);

            return Content("This page has been expired!");
        }
    }
}