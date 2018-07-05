using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using CaptchaMvc.HtmlHelpers;
using System;
using System.Globalization;
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

        public LoginController(IUsersService uService, IPatientLoginDetailService pldService
            , ILoginTrackingService ltService, ITabsService tService, IModuleAccessService maService)
        {
            _uService = uService;
            _pldService = pldService;
            _ltService = ltService;
            _tService = tService;
            _maService = maService;
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
    }
}