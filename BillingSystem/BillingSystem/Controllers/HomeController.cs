using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Models;
using CaptchaMvc.HtmlHelpers;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Globalization;
using System.Threading;
using System.Web;
using Kendo.Mvc.Extensions;
using Microsoft.Ajax.Utilities;
using Kendo.Mvc.UI;
using System.Net.Mime;
using System.Data;
using Excel;
using System.Threading.Tasks;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Filters;

namespace BillingSystem.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [CustomAuth]
    public class HomeController : BaseController
    {
        #region Services

        private readonly IAuditLogService _adService;
        private readonly IBillHeaderService _bhService;
        private readonly IATCCodesService _atcService;
        private readonly IBedRateCardService _service;
        private readonly IPatientInfoService _piService;
        private readonly IFacilityStructureService _fsService;
        private readonly IEncounterService _eService;
        private readonly IBillingSystemParametersService _bspService;
        private readonly ICPTCodesService _cptService;
        private readonly IUsersService _uService;
        private readonly ICountryService _cService;
        private readonly ICorporateService _coService;
        private readonly ICityService _ctService;
        private readonly IIndicatorDataCheckListService _iService;
        private readonly IPatientLoginDetailService _pldService;
        private readonly ILoginTrackingService _ltService;
        private readonly IModuleAccessService _maService;
        private readonly ISystemConfigurationService _scsService;
        private readonly IFacilityService _fService;
        //private readonly IStateService _stService;
        //private readonly IGlobalCodeCategoryService _gcService;
        //private readonly IRoleService _roService;
        //private readonly IGlobalCodeService _gService;
        private readonly IServiceCodeService _scService;
        private readonly IDRGCodesService _drgService;
        private readonly IHCPCSCodesService _hcpcService;
        private readonly IDenialService _denService;
        private readonly IDiagnosisCodeService _diacService;
        private readonly IDrugService _drugService;
        private readonly IOpenOrderService _ooService;
        private readonly IRuleMasterService _rmService;
        //private readonly IBillActivityService _baService;
        private readonly IUserRoleService _urService;
        //private readonly IFacilityRoleService _frService;
        //private readonly IPhysicianService _phService;
        //private readonly ISchedulingService _schService;
        //private readonly IAppointmentTypesService _atService;
        //private readonly IDeptTimmingService _deptService;
        //private readonly IDocumentsTemplatesService _docService;
        #endregion



        //public HomeController(IAppointmentTypesService atService, IAuditLogService adService
        //    , IBillHeaderService bhService, IATCCodesService atcService, IBedRateCardService service
        //    , IPatientInfoService piService, IFacilityStructureService fsService, IEncounterService eService
        //    , IBillingSystemParametersService bspService, ICPTCodesService cptService
        //    , IUsersService uService, ICountryService cService, ICorporateService coService
        //    , ICityService ctService, IIndicatorDataCheckListService iService
        //    , IPatientLoginDetailService pldService, ILoginTrackingService ltService, ITabsService tService
        //    , IModuleAccessService maService, ISystemConfigurationService scsService
        //    , IFacilityService fService, IRoleTabsService rtService, IStateService stService
        //    , IGlobalCodeCategoryService gcService, IRoleService roService, IGlobalCodeService gService
        //    , IServiceCodeService scService, IDRGCodesService drgService, IHCPCSCodesService hcpcService
        //    , IDenialService denService, IDiagnosisCodeService diacService, IDrugService drugService
        //    , IOpenOrderService ooService, IRuleMasterService rmService, IBillActivityService baService
        //    , IUserRoleService urService, IFacilityRoleService frService, IPhysicianService phService
        //    , ISchedulingService schService, IDeptTimmingService deptService
        //    , IDocumentsTemplatesService docService)
        //{
        //    _atService = atService;
        //    _adService = adService;
        //    _bhService = bhService;
        //    _atcService = atcService;
        //    _service = service;
        //    _piService = piService;
        //    _fsService = fsService;
        //    _eService = eService;
        //    _bspService = bspService;
        //    _cptService = cptService;
        //    _uService = uService;
        //    _cService = cService;
        //    _coService = coService;
        //    _ctService = ctService;
        //    _iService = iService;
        //    _pldService = pldService;
        //    _ltService = ltService;
        //    _tService = tService;
        //    _maService = maService;
        //    _scsService = scsService;
        //    _fService = fService;
        //    _rtService = rtService;
        //    _stService = stService;
        //    _gcService = gcService;
        //    _roService = roService;
        //    _gService = gService;
        //    _scService = scService;
        //    _drgService = drgService;
        //    _hcpcService = hcpcService;
        //    _denService = denService;
        //    _diacService = diacService;
        //    _drugService = drugService;
        //    _ooService = ooService;
        //    _rmService = rmService;
        //    _baService = baService;
        //    _urService = urService;
        //    _frService = frService;
        //    _phService = phService;
        //    _schService = schService;
        //    _deptService = deptService;
        //    _docService = docService;
        //}

        public HomeController(IAuditLogService adService
        , IBillHeaderService bhService, IATCCodesService atcService, IBedRateCardService service
        , IPatientInfoService piService, IFacilityStructureService fsService, IEncounterService eService
        , IBillingSystemParametersService bspService, ICPTCodesService cptService
        , IUsersService uService, ICountryService cService, ICorporateService coService
        , ICityService ctService, IIndicatorDataCheckListService iService
        , IPatientLoginDetailService pldService, ILoginTrackingService ltService
        , IModuleAccessService maService, ISystemConfigurationService scsService
        , IFacilityService fService, IRoleTabsService rtService
        , IServiceCodeService scService, IDRGCodesService drgService, IHCPCSCodesService hcpcService
        , IDenialService denService, IDiagnosisCodeService diacService, IDrugService drugService
        , IOpenOrderService ooService, IRuleMasterService rmService
        , IUserRoleService urService, IFacilityRoleService frService
        )
        {
            //_atService = atService;
            _adService = adService;
            _bhService = bhService;
            _atcService = atcService;
            _service = service;
            _piService = piService;
            _fsService = fsService;
            _eService = eService;
            _bspService = bspService;
            _cptService = cptService;
            _uService = uService;
            _cService = cService;
            _coService = coService;
            _ctService = ctService;
            _iService = iService;
            _pldService = pldService;
            _ltService = ltService;
            _maService = maService;
            _scsService = scsService;
            _fService = fService;
            //_stService = stService;
            //_gcService = gcService;
            //_roService = roService;
            //_gService = gService;
            _scService = scService;
            _drgService = drgService;
            _hcpcService = hcpcService;
            _denService = denService;
            _diacService = diacService;
            _drugService = drugService;
            _ooService = ooService;
            _rmService = rmService;
            //_baService = baService;
            _urService = urService;
            //_frService = frService;
            //_phService = phService;
            //_schService = schService;
            //_deptService = deptService;
        }

        ///// <summary>
        ///// Users the login.
        ///// </summary>
        ///// <returns></returns>
        //[AllowAnonymous]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public ActionResult UserLogin()
        //{
        //    //Not in Use
        //    //var pwd = EncryptDecrypt.GetDecryptedData("O1yC58YWrr/HGFQyfok2Gw==", "");
        //    Session.RemoveAll();
        //    return View();
        //}

        ///// <summary>
        ///// Patients the login.
        ///// </summary>
        ///// <returns></returns>
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //[AllowAnonymous]
        //public ActionResult PatientLogin()
        //{
        //    Session.RemoveAll();
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult PatientLogin(PatientLoginDetail model)
        //{
        //    if (model != null && !string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.Email))
        //    {
        //        var flag = true;
        //        var currentPatient = _pldService.GetPatientLoginDetailsByEmail(model.Email);
        //        if (currentPatient != null)
        //        {
        //            var patientId = currentPatient.PatientId ?? 0;
        //            var enPwd = EncryptDecrypt.Encrypt(model.Password).ToLower().Trim();
        //            if (string.IsNullOrEmpty(currentPatient.Password))
        //            {
        //                ViewBag.check = (int)LoginResponseTypes.AccountNotActivated;
        //                return View();
        //            }

        //            if (currentPatient.Password.ToLower().Trim().Equals(enPwd))
        //            {
        //                if (currentPatient.FailedLoginAttempts.HasValue &&
        //                    currentPatient.FailedLoginAttempts.Value == 3)
        //                {
        //                    var failedlogin = Convert.ToDateTime(currentPatient.LastInvalidLogin);
        //                    var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
        //                    if (timespan.TotalMinutes < 30)
        //                    {
        //                        flag = false;
        //                        ViewBag.check = (int)LoginResponseTypes.FailedAttemptsOver;
        //                    }
        //                }

        //                if (flag)
        //                {
        //                    var loginTrackingVm = new LoginTracking
        //                    {
        //                        ID = patientId,
        //                        LoginTime = Helpers.GetInvariantCultureDateTime(),
        //                        LoginUserType = (int)LoginTrackingTypes.UserLogin,
        //                        FacilityId = currentPatient.PatientId,
        //                        CorporateId = currentPatient.CorporateId,
        //                        IsDeleted = false,
        //                        IPAddress = Helpers.GetUser_IP(),
        //                        CreatedBy = patientId,
        //                        CreatedDate = Helpers.GetInvariantCultureDateTime()
        //                    };

        //                    _ltService.AddUpdateLoginTrackingData(loginTrackingVm);
        //                    _pldService.UpdatePatientLoginFailedLog(patientId, 0, Helpers.GetInvariantCultureDateTime());


        //                    var objSession = Session[SessionNames.SessionClass.ToString()] != null
        //                        ? Session[SessionNames.SessionClass.ToString()] as SessionClass
        //                        : new SessionClass();

        //                    objSession.FirstTimeLogin = _ltService.IsFirstTimeLoggedIn(patientId,
        //                        (int)LoginTrackingTypes.PatientLogin);

        //                    objSession.FacilityNumber = currentPatient.FacilityNumber;
        //                    objSession.UserName = currentPatient.PatientName;
        //                    objSession.UserId = patientId;
        //                    objSession.SelectedCulture = CultureInfo.CurrentCulture.Name;
        //                    objSession.LoginUserType = (int)LoginTrackingTypes.PatientLogin;
        //                    objSession.UserIsAdmin = false;
        //                    objSession.RoleId = 0;
        //                    objSession.RoleName = "Patient Access";

        //                    objSession.MenuSessionList = _tService.GetPatientTabsListData(patientId);


        //                    Session[SessionNames.SessoionModuleAccess.ToString()] =
        //                                                    _maService.GetModulesAccessList(currentPatient.CorporateId, currentPatient.FacilityId);

        //                    Session[SessionNames.SessionClass.ToString()] = objSession;
        //                    return RedirectToAction("Index", "PatientPortal", new { pId = patientId });
        //                }
        //            }
        //            else
        //            {
        //                if (currentPatient.Password == null || !currentPatient.Password.Equals(EncryptDecrypt.Encrypt(currentPatient.Password)))
        //                    ViewBag.check = (int)LoginResponseTypes.Failed;
        //                else if (currentPatient.IsDeleted != false)
        //                    ViewBag.check = (int)LoginResponseTypes.IsDeleted;

        //                else if (!string.IsNullOrEmpty(currentPatient.PatientName) && ViewBag.check == (int)LoginResponseTypes.Failed)
        //                {
        //                    if (currentPatient.FailedLoginAttempts < 3 || currentPatient.FailedLoginAttempts == null)
        //                    {
        //                        var failedlogin = currentPatient.LastInvalidLogin ?? Helpers.GetInvariantCultureDateTime();
        //                        var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
        //                        var failedattempts = timespan.TotalMinutes < 30
        //                            ? Convert.ToInt32(currentPatient.FailedLoginAttempts) + 1
        //                            : 1;
        //                        _pldService.UpdatePatientLoginFailedLog(patientId, failedattempts,
        //                            Helpers.GetInvariantCultureDateTime());
        //                    }
        //                    else if (currentPatient.FailedLoginAttempts == 3)
        //                    {
        //                        var failedlogin = currentPatient.LastInvalidLogin ?? Helpers.GetInvariantCultureDateTime();
        //                        var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
        //                        if (timespan.TotalMinutes < 30)
        //                            flag = false;
        //                    }
        //                }

        //                if (flag == false)
        //                    ViewBag.check = (int)LoginResponseTypes.Failed;
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.check = (int)LoginResponseTypes.Failed;
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.check = (int)LoginResponseTypes.UnknownError;
        //    }
        //    return View();
        //}

        ///// <summary>
        ///// Users the login.
        ///// </summary>
        ///// <param name="model">The model.</param>
        ///// <returns></returns>
        ////[HttpPost]
        ////public async Task<ActionResult> UserLogin(Users model)
        ////{
        ////    //Changes by Amit Jain on 07102014
        ////    //Changes start here
        ////    //var objSystemConfigurationWebCommunicator = new SystemConfigurationWebCommunicator();
        ////    //var _configdata = objSystemConfigurationWebCommunicator.getOfflineTime();
        ////    var login = new Users();

        ////    var configdata = _scsService.getOfflineTime();
        ////    //Changes end here

        ////    var startTime = (TimeSpan)configdata.LoginStartTime;
        ////    var endTime = (TimeSpan)configdata.LoginEndTime;
        ////    var starttimetext = Convert.ToDateTime(string.Format("{0:hh\\:mm\\:ss}", startTime)).ToLongTimeString();
        ////    var endtimetext = Convert.ToDateTime(string.Format("{0:hh\\:mm\\:ss}", endTime)).ToLongTimeString();

        ////    var flag = true;

        ////    //Check the Captcha Code below
        ////    if (!this.IsCaptchaValid(string.Empty)) // means codes does not matched
        ////    {
        ////        ViewBag.check = LoginResponseTypes.CaptchaFailed.ToString();
        ////        return View(login);
        ////    }

        ////    var currentUser = _uService.GetUser(model.UserName, model.Password);//added jagjeet 07102014

        ////    if (!IsInRange(starttimetext, endtimetext))//only check for users who do not have assigned system offline overwrite' user role)
        ////    {
        ////        ViewBag.check = "Offline (" + string.Format("{0:HH:mm:ss tt}", endtimetext)
        ////           + " to " + string.Format("{0:HH:mm:ss tt}", starttimetext) + ")";
        ////    }
        ////    else
        ////    {
        ////        //Changes by Amit Jain and added a check if object is not null
        ////        var encryptPassword = !string.IsNullOrEmpty(model.Password) ? EncryptDecrypt.GetEncryptedData(model.Password, string.Empty) : string.Empty;
        ////        if (currentUser != null && currentUser.UserName != null && currentUser.Password.Equals(encryptPassword) && currentUser.IsActive && (currentUser.IsDeleted != null && !Convert.ToBoolean(currentUser.IsDeleted)))
        ////        {
        ////            if (currentUser.FailedLoginAttempts == 3)
        ////            {
        ////                var failedlogin = Convert.ToDateTime(currentUser.LastInvalidLogin);
        ////                var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
        ////                if (timespan.TotalMinutes < 30)
        ////                {
        ////                    flag = false;
        ////                    ViewBag.check = LoginResponseTypes.FailedAttemptsOver.ToString();
        ////                }
        ////            }

        ////            if (flag)
        ////            {
        ////                ViewBag.check = LoginResponseTypes.Success.ToString();
        ////                //Commented by Amit Jain
        ////                //Menu Manipulations 
        ////                //var tabsList = usersBal.GetTabsByUserName(model.UserName);
        ////                //System.Web.HttpContext.Current.Session["MenuSession"] = tabsList;
        ////                var objSession = Session[SessionNames.SessionClass.ToString()] != null
        ////                    ? Session[SessionNames.SessionClass.ToString()] as SessionClass
        ////                    : new SessionClass();

        ////                if (objSession != null)
        ////                {
        ////                    objSession.CountryId = currentUser.CountryID;
        ////                    objSession.UserEmail = currentUser.Email;
        ////                    objSession.MenuSessionList = null;
        ////                    objSession.FirstTimeLogin = _ltService.IsFirstTimeLoggedIn(currentUser.UserID,
        ////                        (int)LoginTrackingTypes.UserLogin);

        ////                    var loginTrackingVm = new LoginTracking
        ////                    {
        ////                        ID = currentUser.UserID,
        ////                        LoginTime = Helpers.GetInvariantCultureDateTime(),
        ////                        LoginUserType = (int)LoginTrackingTypes.UserLogin,
        ////                        FacilityId = currentUser.FacilityId,
        ////                        CorporateId = currentUser.CorporateId,
        ////                        IsDeleted = false,
        ////                        IPAddress = Helpers.GetUser_IP(),
        ////                        CreatedBy = currentUser.UserID,
        ////                        CreatedDate = _fService.GetInvariantCultureDateTime(Helpers.GetDefaultFacilityId())
        ////                    };

        ////                    _ltService.AddUpdateLoginTrackingData(loginTrackingVm);

        ////                    UpdateFailedLog(currentUser.UserID, 0);
        ////                    /*
        ////                     * Owner: Amit Jain
        ////                     * On: 05102014
        ////                     * Purpose: Change the home page after login success
        ////                     * Earlier: It was PatientLogin, now its PatientSearch
        ////                     */
        ////                    //Changes start here
        ////                    //return RedirectToAction("PatientLogin");
        ////                    objSession.FacilityNumber = "1002";
        ////                    objSession.UserName = currentUser.UserName;
        ////                    objSession.UserId = currentUser.UserID;
        ////                    objSession.SelectedCulture = CultureInfo.CurrentCulture.Name;
        ////                    objSession.LoginUserType = (int)LoginTrackingTypes.UserLogin;

        ////                    var assignedRoles = GetUserRoles(currentUser.UserID);
        ////                    if (assignedRoles.Count == 1)
        ////                    {
        ////                        var currentRole = assignedRoles[0];
        ////                        objSession.FacilityId = currentRole.FacilityId;
        ////                        objSession.RoleId = currentRole.RoleId;
        ////                        objSession.CorporateId = currentRole.CorporateId;
        ////                        // Changed by Shashank ON : 5th May 2015 : To add the Module access level Security when user log in via Facility and Corporate 
        ////                        objSession.MenuSessionList = _uService.GetTabsByUserIdRoleId(objSession.UserId, objSession.RoleId, currentRole.FacilityId, currentRole.CorporateId, isDeleted: false, isActive: true);

        ////                        //var moduleAccessBal = new ModuleAccessBal();
        ////                        //Session[SessionNames.SessoionModuleAccess.ToString()] =
        ////                        //    moduleAccessBal.GetModulesAccessList(currentRole.CorporateId, currentRole.FacilityId);

        ////                        var cm = _uService.GetUserDetails(currentRole.RoleId, currentRole.FacilityId, objSession.UserId);
        ////                        objSession.RoleName = cm.RoleName;
        ////                        objSession.FacilityName = cm.DefaultFacility;
        ////                        objSession.UserName = cm.UserName;
        ////                        objSession.FacilityNumber = cm.FacilityNumber;
        ////                        objSession.UserIsAdmin = cm.UserIsAdmin;
        ////                        objSession.LoginUserType = (int)LoginTrackingTypes.UserLogin;
        ////                        objSession.RoleKey = cm.RoleKey;


        ////                        var facilityObj = _fService.GetFacilityById(currentRole.FacilityId);
        ////                        var timezoneValue = facilityObj.FacilityTimeZone;
        ////                        if (!string.IsNullOrEmpty(timezoneValue))
        ////                        {
        ////                            var timezoneobj = TimeZoneInfo.FindSystemTimeZoneById(timezoneValue);
        ////                            objSession.TimeZone = timezoneobj.BaseUtcOffset.TotalHours.ToString();
        ////                        }
        ////                        else
        ////                            objSession.TimeZone = "0.0";

        ////                        objSession.IsPatientSearchAccessible = _rtService.CheckIfTabNameAccessibleToGivenRole("Patient Lookup",
        ////                            ControllerAccess.PatientSearch.ToString(), ActionNameAccess.PatientSearch.ToString(),
        ////                            Convert.ToInt32(currentRole.RoleId));
        ////                        objSession.IsAuthorizationAccessible =
        ////                            _rtService.CheckIfTabNameAccessibleToGivenRole("Obtain Insurance Authorization",
        ////                                ControllerAccess.Authorization.ToString(),
        ////                                ActionNameAccess.AuthorizationMain.ToString(), Convert.ToInt32(currentRole.RoleId));
        ////                        objSession.IsActiveEncountersAccessible =
        ////                            _rtService.CheckIfTabNameAccessibleToGivenRole("Active Encounters",
        ////                                ControllerAccess.ActiveEncounter.ToString(),
        ////                                ActionNameAccess.ActiveEncounter.ToString(),
        ////                                Convert.ToInt32(currentRole.RoleId));
        ////                        objSession.IsBillHeaderViewAccessible =
        ////                            _rtService.CheckIfTabNameAccessibleToGivenRole("Generate Preliminary Bill",
        ////                                ControllerAccess.BillHeader.ToString(),
        ////                                ActionNameAccess.Index.ToString(), Convert.ToInt32(currentRole.RoleId));
        ////                        objSession.IsEhrAccessible =
        ////                            _rtService.CheckIfTabNameAccessibleToGivenRole("EHR",
        ////                                ControllerAccess.Summary.ToString(),
        ////                                ActionNameAccess.PatientSummary.ToString(), Convert.ToInt32(currentRole.RoleId));

        ////                        objSession.SchedularAccessible =
        ////                            _rtService.CheckIfTabNameAccessibleToGivenRole("Scheduling", string.Empty, string.Empty, Convert.ToInt32(currentRole.RoleId));

        ////                        /*
        ////                         * By: Amit Jain
        ////                         * On: 24082015
        ////                         * Purpose: Setting up the table numbers for the Billing Codes
        ////                         */
        ////                        //----Billing Codes' Table Number additions start here---------------
        ////                        if (objSession.CorporateId > 0 && !string.IsNullOrEmpty(objSession.FacilityNumber))
        ////                        {
        ////                            var currentParameter = _bspService.GetDetailsByCorporateAndFacility(
        ////                                objSession.CorporateId, objSession.FacilityNumber);

        ////                            var cDetails = new Corporate();
        ////                            cDetails = _coService.GetCorporateById(objSession.CorporateId);


        ////                            if (objSession.UserId != 1)
        ////                            {
        ////                                objSession.CptTableNumber =
        ////                                    currentParameter != null && !string.IsNullOrEmpty(currentParameter.CPTTableNumber)
        ////                                        ? currentParameter.CPTTableNumber
        ////                                        : cDetails.DefaultCPTTableNumber;

        ////                                objSession.ServiceCodeTableNumber =
        ////                                    currentParameter != null && !string.IsNullOrEmpty(currentParameter.ServiceCodeTableNumber)
        ////                                        ? currentParameter.ServiceCodeTableNumber
        ////                                        : cDetails.DefaultServiceCodeTableNumber;

        ////                                objSession.DrugTableNumber =
        ////                                    currentParameter != null && !string.IsNullOrEmpty(currentParameter.DrugTableNumber)
        ////                                        ? currentParameter.DrugTableNumber
        ////                                        : cDetails.DefaultDRUGTableNumber;

        ////                                objSession.DrgTableNumber =
        ////                                    currentParameter != null && !string.IsNullOrEmpty(currentParameter.DRGTableNumber)
        ////                                        ? currentParameter.DRGTableNumber
        ////                                        : cDetails.DefaultDRGTableNumber;

        ////                                objSession.HcPcsTableNumber =
        ////                                    currentParameter != null && !string.IsNullOrEmpty(currentParameter.HCPCSTableNumber)
        ////                                        ? currentParameter.HCPCSTableNumber
        ////                                        : cDetails.DefaultHCPCSTableNumber;

        ////                                objSession.DiagnosisCodeTableNumber =
        ////                                    currentParameter != null && !string.IsNullOrEmpty(currentParameter.DiagnosisTableNumber)
        ////                                        ? currentParameter.DiagnosisTableNumber
        ////                                        : cDetails.DefaultDiagnosisTableNumber;


        ////                                objSession.BillEditRuleTableNumber =
        ////                                    currentParameter != null && !string.IsNullOrEmpty(currentParameter.BillEditRuleTableNumber)
        ////                                        ? currentParameter.BillEditRuleTableNumber
        ////                                        : cDetails.BillEditRuleTableNumber;

        ////                                objSession.DefaultCountryId = currentParameter.DefaultCountry > 0
        ////                                    ? currentParameter.DefaultCountry : 45;
        ////                            }
        ////                            else
        ////                            {
        ////                                objSession.CptTableNumber = "0";
        ////                                objSession.ServiceCodeTableNumber = "0";
        ////                                objSession.DrugTableNumber = "0";
        ////                                objSession.DrgTableNumber = "0";
        ////                                objSession.HcPcsTableNumber = "0";
        ////                                objSession.DiagnosisCodeTableNumber = "0";
        ////                                objSession.BillEditRuleTableNumber = "0";
        ////                            }

        ////                        }
        ////                        //----Billing Codes' Table Number additions end here---------------

        ////                    }
        ////                    Session[SessionNames.SessionClass.ToString()] = objSession;
        ////                }

        ////                //Send Email Async
        ////                var resul = await MailHelper.SendEmailAsync(objRequest: new EmailInfo
        ////                {
        ////                    DisplayName = "Services Dot - Admin",
        ////                    Email = "aj@gttechsolutions.com",
        ////                    Subject = $"Logged-In successfully at {Request.Url.AbsoluteUri}",
        ////                    MessageBody = $"Hello, <br/> You have successfully login at {DateTime.UtcNow}. <br/>",
        ////                });


        ////                //return RedirectToAction("PatientSearch", "PatientSearch");  
        ////                return RedirectToAction("Welcome", "Home");
        ////                //Changes end here
        ////            }
        ////        }
        ////        else
        ////        {
        ////            if (currentUser != null)
        ////            {
        ////                if (currentUser.Password == null || !currentUser.Password.Equals(encryptPassword))
        ////                    ViewBag.check = LoginResponseTypes.Failed.ToString();
        ////                else if (!currentUser.IsActive)
        ////                    ViewBag.check = LoginResponseTypes.InActive.ToString();
        ////                else if (currentUser.IsDeleted != null && Convert.ToBoolean(currentUser.IsDeleted))
        ////                    ViewBag.check = LoginResponseTypes.IsDeleted.ToString();
        ////                else
        ////                    ViewBag.check = LoginResponseTypes.Failed.ToString();
        ////            }
        ////            else
        ////                ViewBag.check = LoginResponseTypes.Failed.ToString();

        ////            if (currentUser != null && !string.IsNullOrEmpty(currentUser.UserName) && ViewBag.check == LoginResponseTypes.Failed.ToString())
        ////            {
        ////                if (currentUser.FailedLoginAttempts < 3 || currentUser.FailedLoginAttempts == null)
        ////                {
        ////                    var failedlogin = Convert.ToDateTime(currentUser.LastInvalidLogin);
        ////                    var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
        ////                    var failedattempts = timespan.TotalMinutes < 30 ? Convert.ToInt32(currentUser.FailedLoginAttempts) + 1 : 1;
        ////                    UpdateFailedLog(currentUser.UserID, failedattempts);
        ////                }
        ////                else if (currentUser.FailedLoginAttempts == 3)
        ////                {
        ////                    var failedlogin = Convert.ToDateTime(currentUser.LastInvalidLogin);
        ////                    var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
        ////                    if (timespan.TotalMinutes < 30)
        ////                        flag = false;
        ////                    var passwordDisablelog = new AuditLog()
        ////                    {
        ////                        CorporateId = currentUser.CorporateId,
        ////                        UserId = currentUser.UserID,
        ////                        CreatedDate = Helpers.GetInvariantCultureDateTime(),
        ////                        TableName = "Users",
        ////                        FieldName = "Password_Disabled",
        ////                        PrimaryKey = 0,
        ////                        FacilityId = currentUser.FacilityId,
        ////                        EventType = "Added"
        ////                    };
        ////                    var auditlogbal = _adService.AddUptdateAuditLog(passwordDisablelog);
        ////                }
        ////            }
        ////            else if (currentUser == null && ViewBag.check == LoginResponseTypes.Failed.ToString())
        ////            {
        ////                var userbyUsername = _uService.GetUserbyUserName(model.UserName);
        ////                if (userbyUsername != null)
        ////                {
        ////                    if (userbyUsername.FailedLoginAttempts < 3 || userbyUsername.FailedLoginAttempts == null)
        ////                    {
        ////                        var failedlogin = Convert.ToDateTime(userbyUsername.LastInvalidLogin);
        ////                        var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
        ////                        var failedattempts = timespan.TotalMinutes < 30
        ////                                                 ? Convert.ToInt32(userbyUsername.FailedLoginAttempts) + 1
        ////                                                 : 1;
        ////                        UpdateFailedLog(userbyUsername.UserID, failedattempts);
        ////                    }
        ////                    else if (userbyUsername.FailedLoginAttempts == 3)
        ////                    {
        ////                        var failedlogin = Convert.ToDateTime(userbyUsername.LastInvalidLogin);
        ////                        var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
        ////                        if (timespan.TotalMinutes < 30)
        ////                        {
        ////                            flag = false;
        ////                        }
        ////                        var passwordDisablelog = new AuditLog()
        ////                        {
        ////                            CorporateId = userbyUsername.CorporateId,
        ////                            UserId = userbyUsername.UserID,
        ////                            CreatedDate = Helpers.GetInvariantCultureDateTime(),
        ////                            TableName = "Users",
        ////                            FieldName = "Password_Disabled",
        ////                            PrimaryKey = 0,
        ////                            FacilityId = userbyUsername.FacilityId,
        ////                            EventType = "Added"
        ////                        };
        ////                        var auditlogbal = _adService.AddUptdateAuditLog(passwordDisablelog);
        ////                    }
        ////                }
        ////            }
        ////            if (flag == false)
        ////                ViewBag.check = LoginResponseTypes.Blocked.ToString();//"User is Blocked for 3 failed attempts.";Blocked
        ////        }
        ////    }
        ////    return View(login);
        ////}
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<ActionResult> UserLogin(Users model)
        //{
        //    UsersViewModel user = null;
        //    var statusId = 0;

        //    //Check the Captcha Code below
        //    if (!this.IsCaptchaValid(string.Empty))
        //        statusId = -5;

        //    if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
        //        statusId = -3;

        //    if (statusId == 0)
        //        user = _uService.AuthenticateUser(model.UserName, model.Password, DateTime.Now, Helpers.GetUser_IP()
        //            , Convert.ToString((int)LoginTrackingTypes.UserLogin), out statusId);

        //    switch (statusId)
        //    {
        //        case -1:
        //            ViewBag.check = Convert.ToString(LoginResponseTypes.OddLoginTiming);
        //            break;
        //        case -2:
        //        case -3:
        //            ViewBag.check = Convert.ToString(LoginResponseTypes.Failed);
        //            break;
        //        case -4:
        //            ViewBag.check = Convert.ToString(LoginResponseTypes.FailedAttemptsOver);
        //            break;
        //        case -5:
        //            ViewBag.check = Convert.ToString(LoginResponseTypes.CaptchaFailed);
        //            break;
        //        case 0:
        //            ViewBag.check = Convert.ToString(LoginResponseTypes.Success);

        //            var objSession = new SessionClass
        //            {
        //                CountryId = user.CountryID,
        //                UserEmail = user.Email,
        //                FacilityNumber = user.FacilityNumber,
        //                UserName = user.UserName,
        //                UserId = user.UserID,
        //                SelectedCulture = CultureInfo.CurrentCulture.Name,
        //                LoginUserType = (int)LoginTrackingTypes.UserLogin,
        //                FacilityId = user.FacilityId.Value,
        //                CorporateId = user.CorporateId.Value,
        //                FacilityName = user.FacilityName,
        //                UserIsAdmin = user.AdminUser.Value,
        //                FirstTimeLogin = user.IsFirstTimeLoggedIn,
        //                TimeZone = user.TimeZone,
        //                DefaultCountryId = user.DefaultCountryId,
        //                IsActiveEncountersAccessible = user.IsActiveEncountersAccessible,
        //                IsBillHeaderViewAccessible = user.IsBillHeaderViewAccessible,
        //                IsAuthorizationAccessible = user.IsAuthorizationAccessible,
        //                IsEhrAccessible = user.IsEhrAccessible,
        //                IsPatientSearchAccessible = user.IsPatientSearchAccessible,
        //                SchedularAccessible = user.SchedularAccessible,
        //                CptTableNumber = user.CptTableNumber,
        //                BillEditRuleTableNumber = user.BillEditRuleTableNumber,
        //                DiagnosisCodeTableNumber = user.DiagnosisCodeTableNumber,
        //                DrgTableNumber = user.DrgTableNumber,
        //                DrugTableNumber = user.DrugTableNumber,
        //                HcPcsTableNumber = user.HcPcsTableNumber
        //            };

        //            if (user.RolesCount == 1)
        //            {
        //                objSession.RoleKey = user.RoleKey;
        //                objSession.RoleName = user.RoleName;
        //                objSession.RoleId = user.RoleId;
        //                objSession.MenuSessionList = user.Tabs;
        //            }

        //            Session[SessionNames.SessionClass.ToString()] = objSession;

        //            //Send Email Async
        //            var result = await MailHelper.SendEmailAsync(objRequest: new EmailInfo
        //            {
        //                DisplayName = "Services Dot - Admin",
        //                Email = "aj@gttechsolutions.com",
        //                Subject = $"Logged-In successfully at {Request.Url.AbsoluteUri}",
        //                MessageBody = $"Hello, <br/> You have successfully login at {DateTime.UtcNow}. <br/>",
        //            });

        //            return RedirectToAction("Welcome", "Home");
        //    }

        //    return View(new Users());
        //}

        /// <summary>
        /// Changes the new password.
        /// Also update the Audit log for change password
        /// </summary>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        public ActionResult ChangeNewPassword(String newPassword)
        {
            var userid = Helpers.GetLoggedInUserId();

            var isExists = _uService.CheckExistsPassword(newPassword, userid);
            if (isExists)
                return Json("-1");

            var currentUser = _uService.GetUserById(userid);
            currentUser.Password = newPassword;
            var isupdated = _uService.AddUpdateUser(currentUser, 0);
            var auditlogObj = new AuditLog
            {
                AuditLogID = 0,
                UserId = userid,
                CreatedDate = Helpers.GetInvariantCultureDateTime(),
                TableName = "Users",
                FieldName = "Password",
                PrimaryKey = 0,
                OldValue = string.Empty,
                NewValue = string.Empty,
                CorporateId = Helpers.GetSysAdminCorporateID(),
                FacilityId = Helpers.GetDefaultFacilityId()
            };
            _adService.AddUptdateAuditLog(auditlogObj);
            return Json(isupdated > 0);
        }

        /// <summary>
        /// Logs the off.
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var objSession = Session[SessionNames.SessionClass.ToString()] as SessionClass;
            var userType = 1;
            if (objSession != null)
            {
                _ltService.UpdateLoginOutTime(objSession.UserId, Helpers.GetInvariantCultureDateTime());
                userType = objSession.LoginUserType;
            }
            Session.RemoveAll();

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            var cookie2 = new HttpCookie("ASP.NET_SessionId", "") { Expires = currentDateTime.AddYears(-1) };
            Response.Cookies.Add(cookie2);

            // Invalidate the Cache on the Client Side
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetNoStore();


            return RedirectToAction(userType == (int)LoginTrackingTypes.PatientLogin ? "PatientLogin" : "UserLogin");
        }

        #region Country, States and Cities Dropdown Data Binding


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetCitiesByStateId(string stateId)
        {
            var list = _ctService.GetCityListByStateId(Convert.ToInt32(stateId));
            return Json(list);

        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetCountryInfoByCountryID(string countryId)
        {
            var objCountry = _cService.GetCountryInfoByCountryID(Convert.ToInt32(countryId));
            return Json(objCountry);

        }


        #endregion


        /// <summary>
        /// Welcomes this instance.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult Welcome()
        {
            var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
            ViewBag.Role = session == null ? string.Empty : session.RoleId.ToString(); //for view bag role session value
            ViewBag.FirstTimeLogin = session == null || session.FirstTimeLogin;
            return View();
        }

        /// <summary>
        /// Gets the countries with code.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetCountriesWithCode()
        {
            var list = _cService.GetCountryWithCode().OrderBy(x => x.CountryName);
            return Json(list);

        }

        /// <summary>
        /// Gets the facility name by identifier.
        /// </summary>
        /// <param name="facilityNumber">The facility number.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [CustomAuth]
        public ActionResult GetFacilityNameById(string facilityNumber)
        {
            if (string.IsNullOrEmpty(facilityNumber))
            {
                if (Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    facilityNumber = session.FacilityNumber;
                }
            }
            var name = _fService.GetFacilityNameByNumber(facilityNumber);
            return Json(name);
        }

        /// <summary>
        /// Binds the generic enitity DDL.
        /// </summary>
        /// <param name="column1">The column1.</param>
        /// <param name="column2">The column2.</param>
        /// <param name="enitityName">Name of the enitity.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BindGenericEnitityDDL(string column1, string column2, string enitityName)
        {
            return Json(null);
        }

        /// <summary>
        /// Binds the corporate DDL.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [CustomAuth]
        public ActionResult BindCorporateDDL()
        {
            var corporatId = Helpers.GetDefaultCorporateId();
            var list = _coService.GetCorporateDDL(corporatId);
            return Json(list);

        }


        /// <summary>
        /// Gets the corporates dropdown data.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetCorporatesDropdownData()
        {
            var cId = Helpers.GetDefaultCorporateId();
            var corpList = _coService.GetCorporateDDL(cId);
            if (corpList != null)
            {
                var list = new List<SelectListItem>();
                list.AddRange(corpList.Select(item => new SelectListItem
                {
                    Text = item.CorporateName,
                    Value = item.CorporateID.ToString()
                }));
                return Json(list.OrderBy(x => x.Text));
            }

            return Json(null);
        }



        /// <summary>
        /// Gets the user header.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetUserHeader()
        {

            return null;
        }

        /// <summary>
        /// Gets the filtered codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="searchType">Type of the search.</param>
        /// <param name="drugStatus">The drug status.</param>
        /// <param name="tableNumber"></param>
        /// <param name="blockNumber"></param>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetFilteredCodes(string text, string searchType, string drugStatus, string tableNumber, string blockNumber = null)
        {
            var st = Convert.ToInt32(searchType);
            var codeType = (SearchType)Enum.Parse(typeof(SearchType), searchType);
            var viewpath = string.Empty;
            switch (codeType)
            {
                case SearchType.ServiceCode:
                    var userid = Helpers.GetLoggedInUserId();
                    viewpath = string.Format("../ServiceCode/{0}", PartialViews.ServiceCodeList);
                    var result1 = !string.IsNullOrEmpty(text) ? _scService.GetFilteredServiceCodes(text, string.IsNullOrEmpty(tableNumber) ? Helpers.DefaultServiceCodeTableNumber : tableNumber) : _scService.GetServiceCodesCustomList(Helpers.DefaultServiceCodeTableNumber);

                    var serviceCodeView = new ServiceCodeViewModel
                    {
                        ServiceCodeList = result1,
                        CurrentServiceCode = new ServiceCode(),
                        UserId = userid
                    };
                    return PartialView(viewpath, serviceCodeView);

                case SearchType.CPT:

                    viewpath = string.Format("../CPTCodes/{0}", PartialViews.CPTCodesList);
                    var result2 = !string.IsNullOrEmpty(text) ? _cptService.GetFilteredCptCodes(text, string.IsNullOrEmpty(tableNumber) ? Helpers.DefaultCptTableNumber : tableNumber) : _cptService.GetCPTCodes(Helpers.DefaultCptTableNumber);
                    var viewData = new CPTCodesView
                    {
                        CPTCodesList = result2,
                        CurrentCPTCode = new CPTCodes(),
                        UserId = Helpers.GetLoggedInUserId()
                    };
                    return PartialView(viewpath, viewData);

                case SearchType.DRG:

                    viewpath = string.Format("../DRGCodes/{0}", PartialViews.DRGCodesList);
                    var result3 = !string.IsNullOrEmpty(text) ? _drgService.GetDRGCodesFiltered(text, string.IsNullOrEmpty(tableNumber) ? Helpers.DefaultDrgTableNumber : tableNumber) : _drgService.GetDrgCodes(Helpers.DefaultDrgTableNumber);

                    var drgCodesView = new DRGCodesView
                    {
                        DRGCodesList = result3,
                        CurrentDRGCodes = new DRGCodes(),
                        UserId = Helpers.GetLoggedInUserId()
                    };

                    return PartialView(viewpath, drgCodesView);

                case SearchType.HCPCS:

                    viewpath = string.Format("../HCPCSCodes/{0}", PartialViews.HCPCSCodesList);
                    //var result = !string.IsNullOrEmpty(text) ? bal.GetFilteredHCPCSCodes(text) : bal.GetHCPCSCodes();
                    var result4 = !string.IsNullOrEmpty(text) ? _hcpcService.GetHCPCSCodesFilterData(text, string.IsNullOrEmpty(tableNumber) ? Helpers.DefaultHcPcsTableNumber : tableNumber) : _hcpcService.GetHCPCSCodes(Helpers.DefaultHcPcsTableNumber);
                    var hcpcsCodesView = new HCPCSCodesView
                    {
                        HCPCSCodesList = result4,
                        CurrentHCPCSCodes = new HCPCSCodes(),
                        UserId = Helpers.GetLoggedInUserId()
                    };
                    return PartialView(viewpath, hcpcsCodesView);

                case SearchType.Denial:

                    viewpath = string.Format("../Denial/{0}", PartialViews.DenialList);
                    var result5 = !string.IsNullOrEmpty(text) ? _denService.GetFilteredDenialCodes(text) : _denService.GetDenial();

                    var denialView = new DenialView
                    {
                        DenialList = result5,
                        CurrentDenial = new Denial()
                    };

                    return PartialView(viewpath, result5);

                case SearchType.Diagnosis:
                    viewpath = string.Format("../DiagnosisCode/{0}", PartialViews.DiagnosisCodeList);
                    //var result = !string.IsNullOrEmpty(text) ? bal.GetFilteredDiagnosisCodes(text) : bal.GetDiagnosisCode();
                    var result6 = !string.IsNullOrEmpty(text) ? _diacService.GetFilteredDiagnosisCodesData(text, string.IsNullOrEmpty(tableNumber) ? Helpers.DefaultDiagnosisTableNumber : tableNumber) : _diacService.GetDiagnosisCode(Helpers.DefaultDiagnosisTableNumber);

                    if (blockNumber != null)
                    {
                        var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);
                        result6 = result6.ToList().OrderByDescending(i => i.DiagnosisTableNumberId).Take(takeValue).ToList();
                    }
                    var diagnosisCodeView = new DiagnosisCodeView
                    {
                        DiagnosisCodeList = result6,
                        CurrentDiagnosisCode = new DiagnosisCode(),
                        UserId = Helpers.GetLoggedInUserId()
                    };
                    return PartialView(viewpath, diagnosisCodeView);
                case SearchType.DRUG:

                    viewpath = string.Format("../Drug/{0}", PartialViews.DrugList);
                    var result7 = !string.IsNullOrEmpty(text) ?
                        _drugService.GetFilteredDrugCodesData(text, drugStatus, string.IsNullOrEmpty(tableNumber) ? Helpers.DefaultDrugTableNumber : tableNumber) :
                        _drugService.GetDrugList(Helpers.DefaultDrugTableNumber);

                    if (blockNumber != null)
                    {
                        var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);
                        result7 = result7.ToList().OrderByDescending(i => i.Id).Take(takeValue).ToList();
                    }
                    var viewData1 = new DrugView
                    {
                        CurrentDrug = new Drug(),
                        DrugList = result7,
                        UserId = Helpers.GetLoggedInUserId()
                    };

                    return PartialView(viewpath, viewData1);


                case SearchType.ATC:
                    viewpath = string.Format("../ATCCodes/{0}", PartialViews.ATCCodesList);
                    var result8 = _atcService.GetATCCodes(text);
                    return PartialView(viewpath, result8);
                default:
                    break;
            }
            return PartialView();
        }



        /// <summary>
        /// Gets the order codes.
        /// </summary>
        /// <param name="codetypeid">The codetypeid.</param>
        /// <returns></returns>
        public ActionResult GetOrderCodes(string codetypeid)
        {
            var orderType = (OrderType)Enum.Parse(typeof(OrderType), codetypeid);
            switch (orderType)
            {
                case OrderType.CPT:
                    var cptcodeslist = _cptService.GetCPTCodes(Helpers.DefaultCptTableNumber);
                    return Json(cptcodeslist);
                case OrderType.HCPCS:
                    var hcpcsCodeslist = _hcpcService.GetHCPCSCodes(Helpers.DefaultHcPcsTableNumber);
                    return Json(hcpcsCodeslist);
                case OrderType.DRG:
                    var drgCodeslist = _drgService.GetDrgCodes(Helpers.DefaultDrgTableNumber);
                    return Json(drgCodeslist);
                case OrderType.DRUG:
                    var list = _drugService.GetDrugList(Helpers.DefaultDrugTableNumber);
                    return Json(list);
            }
            return Json(null);
        }

        /// <summary>
        /// Gets the old encounter list.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <returns></returns>
        public ActionResult GetOldEncounterList(int pid)
        {
            var patientEncounterlist = _eService.GetEncounterListByPatientId(pid);
            return Json(patientEncounterlist);
        }

        /// <summary>
        /// Gets the serach list.
        /// </summary>
        /// <param name="searchType">Type of the search.</param>
        /// <returns></returns>
        public ActionResult GetSerachList(string searchType)
        {
            if (!string.IsNullOrEmpty(searchType))
            {
                var viewpath = string.Empty;
                var st = Convert.ToInt32(searchType);
                var codeType = (OrderType)Enum.Parse(typeof(OrderType), searchType);
                switch (codeType)
                {
                    case OrderType.CPT:
                        viewpath = string.Format("../CPTCodes/{0}", PartialViews.CPTCodesList);
                        var result = _cptService.GetCPTCodes(Helpers.DefaultCptTableNumber).Take(100).ToList();
                        var CPTCodesList = new CPTCodesView
                        {
                            CPTCodesList = result,
                            CurrentCPTCode = new CPTCodes(),

                        };
                        //return PartialView(viewpath, result);
                        return PartialView(viewpath, CPTCodesList);
                    case OrderType.DRG:

                        viewpath = string.Format("../DRGCodes/{0}", PartialViews.DRGCodesList);
                        var result5 = _drgService.GetDrgCodes(Helpers.DefaultDrgTableNumber).Take(100).ToList();
                        var DRGCodeList = new DRGCodesView
                        {
                            DRGCodesList = result5,
                            CurrentDRGCodes = new DRGCodes()
                        };

                        return PartialView(viewpath, DRGCodeList);
                    case OrderType.HCPCS:
                        viewpath = string.Format("../HCPCSCodes/{0}", PartialViews.HCPCSCodesList);
                        var result4 = _hcpcService.GetHCPCSCodes(Helpers.DefaultHcPcsTableNumber).Take(100).ToList();
                        var HCPCSCodesList = new HCPCSCodesView
                        {
                            HCPCSCodesList = result4,
                            CurrentHCPCSCodes = new HCPCSCodes()
                        };
                        return PartialView(viewpath, HCPCSCodesList);
                    case OrderType.Orders:
                        viewpath = string.Format("../Summary/{0}", PartialViews.OpenOrderListPatientSummary);
                        var result3 = _ooService.GetOrdersByPhysicianId(1, Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber).Take(100).ToList();

                        return PartialView(viewpath, result3);
                    case OrderType.Diagnosis:
                        viewpath = string.Format("../DiagnosisCode/{0}", PartialViews.DiagnosisCodeList);
                        var result2 = _diacService.GetDiagnosisCode(Helpers.DefaultDiagnosisTableNumber).Take(100).ToList();
                        var DiagnosisCodeList = new DiagnosisCodeView
                        {
                            DiagnosisCodeList = result2,
                            CurrentDiagnosisCode = new DiagnosisCode()

                        };
                        return PartialView(viewpath, DiagnosisCodeList);
                    case OrderType.DRUG:

                        viewpath = string.Format("../Drug/{0}", PartialViews.DrugList);
                        var result1 = _drugService.GetDrugList(Helpers.DefaultDrugTableNumber).Take(100).ToList();
                        var Druglist = new DrugView()
                        {
                            DrugList = result1,
                            CurrentDrug = new Drug()
                        };
                        return PartialView(viewpath, Druglist);
                    default:
                        break;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the ordering codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="subCategoryId">The sub category identifier.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public ActionResult GetOrderingCodes(string text, int subCategoryId, int categoryId)
        {
            var finalList = new List<GeneralCodesCustomModel>();
            var largedata = Json(finalList, JsonRequestBehavior.AllowGet);
            largedata.MaxJsonLength = int.MaxValue;
            return largedata;
        }

        public List<string> GetColumnsByTableNameb(string tableName)
        {
            var list = new List<string>();
            var entity = (TableNames)Enum.Parse(typeof(TableNames), tableName);
            switch (entity)
            {
                case TableNames.Authorization:
                    list = typeof(Authorization).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.BillHeader:
                    list = typeof(BillHeader).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Corporate:
                    list = typeof(Corporate).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Denial:
                    list = typeof(Denial).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Diagnosis:
                    list = typeof(Diagnosis).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.DiagnosisCode:
                    list = typeof(DiagnosisCode).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Drug:
                    list = typeof(Drug).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Encounter:
                    list = typeof(Encounter).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Facility:
                    list = typeof(Facility).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.HCPCSCodes:
                    list = typeof(HCPCSCodes).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.InsuranceCompany:
                    list = typeof(InsuranceCompany).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.InsurancePlans:
                    list = typeof(InsurancePlans).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.InsurancePolices:
                    list = typeof(InsurancePolices).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.MCContract:
                    list = typeof(ManagedCare).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.OpenOrder:
                    list = typeof(OpenOrder).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.OpenOrderActivitySchedule:
                    list = typeof(OpenOrderActivitySchedule).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.OpenOrderActivityTime:
                    list = typeof(OpenOrderActivityTime).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.OrderActivity:
                    list = typeof(OrderActivity).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.PatientInfo:
                    list = typeof(PatientInfo).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.RuleMaster:
                    list = typeof(RuleMaster).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.PatientInsurance:
                    list = typeof(PatientInsurance).GetProperties().Select(i => i.Name).ToList();
                    break;
                default:
                    break;
            }
            return list;
        }

        /// <summary>
        /// Gets the drug details by drug code.
        /// </summary>
        /// <param name="drugCode">The drug code.</param>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetDrugDetailsByDrugCode(string drugCode)
        {
            var drugObj = _drugService.GetDrugListbyDrugCode(drugCode, Helpers.DefaultDrugTableNumber);
            return Json(drugObj, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetUsersByDefaultCorporateId()
        {
            var list = new List<DropdownListData>();
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : Helpers.GetDefaultFacilityId();
            var usersList = _uService.GetUsersByCorporateIdFacilityId(corporateId, facilityId);
            list.AddRange(usersList.Select(item => new DropdownListData
            {
                Text = item.Name,
                Value = item.CurrentUser.UserID.ToString(),
            }));

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetUsersByCorporateId()
        {
            var list = new List<DropdownListData>();
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : Helpers.GetDefaultFacilityId();
            var usersList = _uService.GetUsersByCorporateandFacilityId(corporateId, facilityId).OrderBy(x => x.FirstName).ToList();
            list.AddRange(usersList.Select(item => new DropdownListData
            {
                Text =
                                                               item.FirstName + " " + item.LastName,
                Value = item.UserID.ToString(),
            }));

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the users detail by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <added by="Shashank">ON 12/16/2014</added>
        /// <returns></returns>
        public ActionResult GetUsersDetailByUserID(Int32 userId)
        {
            var userObj = _uService.GetUserById(userId);
            return Json(userObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the facilitiesby corporate.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <returns></returns>
        public ActionResult GetFacilitiesbyCorporate(int corporateid)
        {
            var finalList = new List<DropdownListData>();
            var list = _fService.GetFacilitiesByCorporateId(corporateid).ToList().OrderBy(x => x.FacilityName).ToList();
            if (list.Count > 0)
            {
                var facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : Helpers.GetDefaultFacilityId();
                if (facilityId > 0 && corporateid > 0)
                    list = list.Where(f => f.FacilityId == facilityId).ToList();

                finalList.AddRange(list.Select(item => new DropdownListData
                {
                    Text = item.FacilityName,
                    Value = Convert.ToString(item.FacilityId)
                }));
            }
            return Json(finalList);
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetUsers()
        {
            var users = new List<DropdownListData>();

            var result = _uService.GetUsersByCorporateIdFacilityId(Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId());
            if (result.Count > 0)
            {
                users.AddRange(result.Select(item => new DropdownListData
                {
                    Text = item.Name,
                    Value = Convert.ToString(item.CurrentUser.UserID),
                    //ExternalValue1 = Convert.ToString(item.CurrentUser.UserType)
                }));
            }

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the patient list.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the encounters list by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
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
        /// Gets the bill header list by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetBillHeaderListByEncounterId(int encounterId)
        {
            var list = new List<DropdownListData>();
            var result = _bhService.GetBillHeaderModelListByEncounterId(encounterId);
            if (result.Any())
            {
                list.AddRange(result.Select(item => new DropdownListData
                {
                    Text = item.BillNumber,
                    Value = Convert.ToString(item.BillHeaderID)
                }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the users by facility identifier.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetUsersByFacilityId(int facilityId)
        {
            var finalList = new List<DropdownListData>();
            var list = _uService.GetAllUsersByFacilityId(facilityId);
            if (list.Count > 0)
            {
                finalList.AddRange(list.Select(item => new DropdownListData
                {

                    Text = item.Name,
                    Value = Convert.ToString(item.CurrentUser.UserID)
                }));
            }

            return Json(finalList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the order codes by order type identifier.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="orderTypeId">The order type identifier.</param>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetOrderCodesByOrderTypeId(string text, int orderTypeId)
        {
            var list = new List<GeneralCodesCustomModel>();
            if (!string.IsNullOrEmpty(text) && orderTypeId > 0 && orderTypeId < 6)
            {
                list = GetOrderCodesByOrderTypeId1(text, orderTypeId);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public List<GeneralCodesCustomModel> GetOrderCodesByOrderTypeId1(string text, int orderTypeId)
        {
            var finalList = new List<GeneralCodesCustomModel>();
            switch (orderTypeId)
            {
                case 1:
                    break;
                case 2:
                    var result1 = _hcpcService.GetHCPCSCodes(Helpers.DefaultHcPcsTableNumber);
                    finalList.AddRange(
                        result1.Select(
                            item =>
                            new GeneralCodesCustomModel
                            {
                                Code = item.CodeNumbering,
                                Description = item.CodeDescription,
                                CodeDescription =
                                        string.Format(
                                            "{0} - {1}",
                                            item.CodeDescription,
                                            item.CodeNumbering),
                                CodeType = Convert.ToInt32(OrderType.HCPCS).ToString(),
                                CodeTypeName = "HCPCS",
                                ExternalCode = string.Empty,
                                ID = item.HCPCSCodesId.ToString()
                            }));
                    break;
                case 4:
                    var result2 = _drgService.GetFilteredDRGCodes(text, Helpers.DefaultDrgTableNumber);
                    finalList.AddRange(
                        result2.Select(
                            item =>
                            new GeneralCodesCustomModel
                            {
                                Code = item.CodeNumbering,
                                Description = item.CodeDescription,
                                CodeDescription =
                                        string.Format(
                                            "{0} - {1}",
                                            item.CodeDescription,
                                            item.CodeNumbering),
                                CodeType =
                                        Convert.ToString(Convert.ToInt32(OrderType.HCPCS)),
                                CodeTypeName = "DRG",
                                ExternalCode = string.Empty,
                                ID = Convert.ToString(item.DRGCodesId)
                            }));
                    break;
                case 5:
                    var result3 = _drugService.GetFilteredDrugCodes(text, Helpers.DefaultDrugTableNumber).Where(x => x.InStock == true).ToList();
                    finalList.AddRange(
                        result3.Select(
                            item =>
                            new GeneralCodesCustomModel
                            {
                                Code = item.DrugCode,
                                Description = item.DrugPackageName,
                                CodeDescription =
                                        string.Format(
                                            "{0} - {1} - {2} - {3}",
                                            item.DrugPackageName,
                                            item.DrugCode,
                                            item.DrugStrength,
                                            item.DrugDosage),
                                CodeType =
                                        Convert.ToString(Convert.ToInt32(OrderType.DRUG)),
                                CodeTypeName = "DRUG",
                                ExternalCode = Convert.ToString(item.BrandCode),
                                ID = Convert.ToString(item.Id)
                            }));
                    break;
            }
            return finalList;
        }

        /// <summary>
        /// Gets the time zones.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTimeZones()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            //foreach (TimeZoneInfo timeZoneInfo in timeZones)
            //    Console.WriteLine("{0}", timeZoneInfo.DisplayName);
            var list = new List<SelectListItem>();
            if (timeZones.Count > 0)
            {
                list.AddRange(timeZones.Select(item => new SelectListItem
                {
                    Text = item.DisplayName,
                    Value = item.Id.ToString()
                }));
            }
            return Json(list);
        }

        /// <summary>
        /// Gets the type of the column data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetColumnDataType(string tableName, string columnName)
        {
            var datatype = GetColumnDataTypeByTableNameColumnName(tableName, columnName);
            return Json(datatype, JsonRequestBehavior.AllowGet);
        }
        public string GetColumnDataTypeByTableNameColumnName(string tableName, string columnName)
        {
            var list = string.Empty;
            var entity = (TableNames)Enum.Parse(typeof(TableNames), tableName);
            switch (entity)
            {
                case TableNames.Authorization:
                    var authorizationval =
                        typeof(Authorization).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (authorizationval != null)
                        list = authorizationval.PropertyType.GenericTypeArguments.Length > 0
                                   ? authorizationval.PropertyType.GenericTypeArguments[0].Name
                                   : authorizationval.PropertyType.Name;
                    break;
                case TableNames.BillHeader:
                    var billHeaderVal = typeof(BillHeader).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (billHeaderVal != null)
                        list = billHeaderVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? billHeaderVal.PropertyType.GenericTypeArguments[0].Name
                                   : billHeaderVal.PropertyType.Name;
                    break;
                case TableNames.Corporate:
                    var corporateVal = typeof(Corporate).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (corporateVal != null)
                        list = corporateVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? corporateVal.PropertyType.GenericTypeArguments[0].Name
                                   : corporateVal.PropertyType.Name;
                    break;
                case TableNames.Denial:
                    var DenialVal = typeof(Denial).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (DenialVal != null)
                        list = DenialVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? DenialVal.PropertyType.GenericTypeArguments[0].Name
                                   : DenialVal.PropertyType.Name;
                    break;
                case TableNames.Diagnosis:
                    var DiagnosisVal = typeof(Diagnosis).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (DiagnosisVal != null)
                        list = DiagnosisVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? DiagnosisVal.PropertyType.GenericTypeArguments[0].Name
                                   : DiagnosisVal.PropertyType.Name;
                    break;
                case TableNames.DiagnosisCode:
                    var DiagnosisCodeVal =
                        typeof(DiagnosisCode).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (DiagnosisCodeVal != null)
                        list = DiagnosisCodeVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? DiagnosisCodeVal.PropertyType.GenericTypeArguments[0].Name
                                   : DiagnosisCodeVal.PropertyType.Name;
                    break;
                case TableNames.Drug:
                    var DrugVal = typeof(Drug).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (DrugVal != null)
                        list = DrugVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? DrugVal.PropertyType.GenericTypeArguments[0].Name
                                   : DrugVal.PropertyType.Name;
                    break;
                case TableNames.Encounter:
                    var EncounterVal = typeof(Encounter).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (EncounterVal != null)
                        list = EncounterVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? EncounterVal.PropertyType.GenericTypeArguments[0].Name
                                   : EncounterVal.PropertyType.Name;
                    break;
                case TableNames.Facility:
                    var FacilityVal = typeof(Facility).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (FacilityVal != null)
                        list = FacilityVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? FacilityVal.PropertyType.GenericTypeArguments[0].Name
                                   : FacilityVal.PropertyType.Name;
                    break;
                case TableNames.HCPCSCodes:
                    var HCPCSCodesVal = typeof(HCPCSCodes).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (HCPCSCodesVal != null)
                        list = HCPCSCodesVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? HCPCSCodesVal.PropertyType.GenericTypeArguments[0].Name
                                   : HCPCSCodesVal.PropertyType.Name;
                    break;
                case TableNames.InsuranceCompany:
                    var InsuranceCompanyVal =
                        typeof(InsuranceCompany).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (InsuranceCompanyVal != null)
                        list = InsuranceCompanyVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? InsuranceCompanyVal.PropertyType.GenericTypeArguments[0].Name
                                   : InsuranceCompanyVal.PropertyType.Name;
                    break;
                case TableNames.InsurancePlans:
                    var InsurancePlansVal =
                        typeof(InsurancePlans).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (InsurancePlansVal != null)
                        list = InsurancePlansVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? InsurancePlansVal.PropertyType.GenericTypeArguments[0].Name
                                   : InsurancePlansVal.PropertyType.Name;
                    break;
                case TableNames.InsurancePolices:
                    var InsurancePolicesVal =
                        typeof(InsurancePolices).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (InsurancePolicesVal != null)
                        list = InsurancePolicesVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? InsurancePolicesVal.PropertyType.GenericTypeArguments[0].Name
                                   : InsurancePolicesVal.PropertyType.Name;
                    break;
                case TableNames.MCContract:
                    var ManagedCareVal = typeof(MCContract).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (ManagedCareVal != null)
                        list = ManagedCareVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? ManagedCareVal.PropertyType.GenericTypeArguments[0].Name
                                   : ManagedCareVal.PropertyType.Name;
                    break;
                case TableNames.OpenOrder:
                    var OpenOrderVal = typeof(OpenOrder).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (OpenOrderVal != null)
                        list = OpenOrderVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? OpenOrderVal.PropertyType.GenericTypeArguments[0].Name
                                   : OpenOrderVal.PropertyType.Name;
                    break;
                case TableNames.OpenOrderActivitySchedule:
                    var OpenOrderActivityScheduleVal =
                        typeof(OpenOrderActivitySchedule).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (OpenOrderActivityScheduleVal != null)
                        list = OpenOrderActivityScheduleVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? OpenOrderActivityScheduleVal.PropertyType.GenericTypeArguments[0].Name
                                   : OpenOrderActivityScheduleVal.PropertyType.Name;
                    break;
                case TableNames.OpenOrderActivityTime:
                    var OpenOrderActivityTimeVal =
                        typeof(OpenOrderActivityTime).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (OpenOrderActivityTimeVal != null)
                        list = OpenOrderActivityTimeVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? OpenOrderActivityTimeVal.PropertyType.GenericTypeArguments[0].Name
                                   : OpenOrderActivityTimeVal.PropertyType.Name;
                    break;
                case TableNames.OrderActivity:
                    var OrderActivityVal =
                        typeof(OrderActivity).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (OrderActivityVal != null)
                        list = OrderActivityVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? OrderActivityVal.PropertyType.GenericTypeArguments[0].Name
                                   : OrderActivityVal.PropertyType.Name;
                    break;
                case TableNames.PatientInfo:
                    var PatientInfoVal = typeof(PatientInfo).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (PatientInfoVal != null)
                        list = PatientInfoVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? PatientInfoVal.PropertyType.GenericTypeArguments[0].Name
                                   : PatientInfoVal.PropertyType.Name;
                    break;
                case TableNames.RuleMaster:
                    var RuleMasterVal = typeof(RuleMaster).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (RuleMasterVal != null)
                        list = RuleMasterVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? RuleMasterVal.PropertyType.GenericTypeArguments[0].Name
                                   : RuleMasterVal.PropertyType.Name;
                    break;
                case TableNames.PatientInsurance:
                    var PatientInsuranceVal =
                        typeof(PatientInsurance).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (PatientInsuranceVal != null)
                        list = PatientInsuranceVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? PatientInsuranceVal.PropertyType.GenericTypeArguments[0].Name
                                   : PatientInsuranceVal.PropertyType.Name;
                    break;
                case TableNames.CPTCodes:
                    var CPTCodesVal = typeof(CPTCodes).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (CPTCodesVal != null) //list = CPTCodesVal.PropertyType.GenericTypeArguments[0].Name;
                        list = CPTCodesVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? CPTCodesVal.PropertyType.GenericTypeArguments[0].Name
                                   : CPTCodesVal.PropertyType.Name;
                    break;
                case TableNames.DRGCodes:
                    var DRGCodesVal = typeof(DRGCodes).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (DRGCodesVal != null)
                        list = DRGCodesVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? DRGCodesVal.PropertyType.GenericTypeArguments[0].Name
                                   : DRGCodesVal.PropertyType.Name;
                    break;
                case TableNames.ServiceCode:
                    var ServiceCodeVal = typeof(ServiceCode).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (ServiceCodeVal != null)
                        list = ServiceCodeVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? ServiceCodeVal.PropertyType.GenericTypeArguments[0].Name
                                   : ServiceCodeVal.PropertyType.Name;
                    break;
                default:
                    break;
            }
            return list;
        }

        /// <summary>
        /// Gets the order code desc.
        /// </summary>
        /// <param name="ordercode">The ordercode.</param>
        /// <param name="ordrtype">The ordrtype.</param>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetOrderCodeDesc(string ordercode, string ordrtype)
        {
            if (!string.IsNullOrEmpty(ordercode) && !string.IsNullOrEmpty(ordrtype))
            {
                var st = Convert.ToInt32(ordrtype);
                var codeType = (OrderType)Enum.Parse(typeof(OrderType), ordrtype);
                switch (codeType)
                {
                    case OrderType.DRG:
                        var result3 = _drgService.GetDrgDescriptionByCode(ordercode, Helpers.DefaultDrgTableNumber);
                        return Json(result3);
                    case OrderType.DRUG:
                        var result2 = _drugService.GetDRUGCodeDescription(ordercode, Helpers.DefaultDrugTableNumber);
                        return Json(result2);
                    case OrderType.CPT:
                        var result = _cptService.GetOrderCodeDescbyCode(ordercode, Helpers.DefaultCptTableNumber);
                        return Json(result);

                    case OrderType.BedCharges:
                        var result1 = _scService.GetServiceCodeDescription(ordercode, Helpers.DefaultServiceCodeTableNumber);
                        return Json(result1);
                    default:
                        return Json(string.Empty);
                }
            }
            return Json(string.Empty);
        }

        //Language Selection
        /// <summary>
        /// Sets the language fron front page.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetLanguageFronFrontPage(string language)
        {
            if (Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                session.SelectedCulture = !string.IsNullOrEmpty(language) ? language : "en-US";
                if (session.SelectedCulture.ToLower().Equals("en-us"))
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");
                }
                else
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-SA");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("ar-SA");
                }
            }
            return Json(true);
        }

        /// <summary>
        /// Sets the tab session.
        /// </summary>
        /// <param name="oTabsCustomModel">The o tabs custom model.</param>
        /// <returns></returns>
        public JsonResult SetTabSession(TabsCustomModel oTabsCustomModel)
        {
            var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
            if (session != null) session.NavTabId = oTabsCustomModel.TabId;
            if (session != null) session.NavParentTabId = oTabsCustomModel.ParentTabId;
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the selected language.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSelectedLanguage()
        {
            if (Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                var selectedCulture = session.SelectedCulture;
                if (!string.IsNullOrEmpty(selectedCulture) && selectedCulture.ToLower().Contains("ar"))
                    return Json("2", JsonRequestBehavior.AllowGet);

                return Json("1", JsonRequestBehavior.AllowGet);
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the numbers.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public JsonResult GetNumbers(int limit)
        {
            var list = new List<DropdownListData>();
            for (var i = 1; i <= limit; i++)
            {
                list.Add(new DropdownListData
                {
                    Text = Convert.ToString(i),
                    Value = Convert.ToString(i),
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the facilities dropdown data with facility number.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetFacilitiesDropdownDataWithFacilityNumber(int? corporateId)
        {
            var facilities = _fService.GetFacilities(Convert.ToInt32(corporateId));
            if (facilities.Count > 0)
            {
                var list = new List<SelectListItem>();
                list.AddRange(facilities.Select(item => new SelectListItem
                {
                    Text = item.FacilityName,
                    Value = item.FacilityNumber,
                }));
                return Json(list);
            }
            return Json(null);
        }

        /// <summary>
        /// Gets the service codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetServiceCodes(string text)
        {
            var finalList = new List<GeneralCodesCustomModel>();
            finalList = GetServiceCodes1(text);
            return Json(finalList, JsonRequestBehavior.AllowGet);
        }
        public List<GeneralCodesCustomModel> GetServiceCodes1(string text)
        {
            var finalList = new List<GeneralCodesCustomModel>();
            var serviceCodesList = _scService.GetServiceCodes(Helpers.DefaultServiceCodeTableNumber);
            finalList.AddRange(
                serviceCodesList.Select(
                    item =>
                    new GeneralCodesCustomModel
                    {
                        Code = item.ServiceCodeValue,
                        Description = item.ServiceCodeDescription,
                        CodeDescription =
                                string.Format(
                                    "{0} - {1}",
                                    item.ServiceCodeValue,
                                    item.ServiceCodeDescription),
                        CodeType = Convert.ToString(OrderType.BedCharges),
                        CodeTypeName = "Service Code",
                        //ExternalCode = item.CTPCodeRangeValue.ToString(),
                        ID = item.ServiceCodeId.ToString()
                    }));
            if (finalList.Count > 0)
            {
                text = text.ToLower().Trim();
                finalList = finalList.Where(f => f.CodeDescription.ToLower().Contains(text)).ToList();
            }
            return finalList;
        }

        /// <summary>
        /// Gets the service codes list.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetServiceCodesList()
        {
            var finalList = GetServiceCodes1(Helpers.DefaultServiceCodeTableNumber);
            return Json(finalList, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Gets the service codes by code main value.
        /// </summary>
        /// <param name="codeMainValue">The code main value.</param>
        /// <param name="rowCount">The row count.</param>
        /// <returns></returns>
        [CustomAuth]
        public JsonResult GetServiceCodesByCodeMainValue(string codeMainValue, int rowCount)
        {
            var list = new List<SelectListItem>();

            var result = _scService.GetServiceCodesByCodeMainValue(codeMainValue, rowCount, Helpers.DefaultServiceCodeTableNumber);
            if (result.Count > 0)
            {
                list.AddRange(result.Select(item => new SelectListItem
                {
                    Text = item.ServiceCodeDescription,
                    Value = item.ServiceCodeValue
                }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the CPT codes list by mue value.
        /// </summary>
        /// <param name="mueValue">The mue value.</param>
        /// <returns></returns>
        [CustomAuth]
        public JsonResult GetCptCodesListByMueValue(string mueValue)
        {
            var list = new List<SelectListItem>();

            var result = _cptService.GetCptCodesListByMueValue(mueValue, Helpers.DefaultCptTableNumber);
            if (result.Count > 0)
            {
                list.AddRange(result.Select(item => new SelectListItem
                {
                    Text = item.CodeDescription.Trim(),
                    Value = item.CodeNumbering.Trim()
                }));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region Patient Login Detail
        /// <summary>
        /// Saves the patient login details.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
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


        /// <summary>
        /// Determines whether [is patient email valid] [the specified emailid].
        /// </summary>
        /// <param name="emailid">The emailid.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sends the forgot password email.
        /// </summary>
        /// <param name="emailid">The emailid.</param>
        /// <returns></returns>
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
                VerificationLink = "/Home/ResetPassword",
                MessageBody = msgBody
            };
            var status = await MailHelper.SendEmailAsync(emailInfo);
            return status;
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


        /// <summary>
        /// Resets the user password.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sends the new password for patient login portal.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="email">The email.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Determines whether [is patient data valid] [the specified patient identifier].
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="birthdate">The birthdate.</param>
        /// <param name="emirateid">The emirateid.</param>
        /// <returns></returns>
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
        #endregion

        /// <summary>
        /// Get Facilities list
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetFacilitiesWithoutCorporateDropdownData()
        {
            var cId = Helpers.GetDefaultCorporateId();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var facilities = userisAdmin ? _fService.GetFacilitiesWithoutCorporateFacility(cId) : _fService.GetFacilitiesWithoutCorporateFacility(cId, Helpers.GetDefaultFacilityId());
            if (facilities.Any())
            {
                var list = new List<SelectListItem>();
                list.AddRange(facilities.Select(item => new SelectListItem
                {
                    Text = item.FacilityName,
                    Value = Convert.ToString(item.FacilityId),
                }));
                return Json(list);
            }

            return Json(null);
        }

        /// <summary>
        /// Gets the facility users.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [CustomAuth]
        public ActionResult GetFacilityUsers(int facilityId)
        {
            var list = new List<SelectListItem>();
            var users = _uService.GetFacilityUsers(facilityId);
            if (users.Any())
            {
                list.AddRange(users.Select(item => new SelectListItem
                {
                    Text = string.Format("{0} {1}", item.FirstName, item.LastName),
                    Value = Convert.ToString(item.UserID)
                }));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the non admin users by corporate.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetNonAdminUsersByCorporate()
        {
            var cId = Helpers.GetSysAdminCorporateID();
            var list = new List<SelectListItem>();
            var users = _uService.GetNonAdminUsersByCorporate(cId);
            if (users.Any())
            {
                list.AddRange(users.Select(item => new SelectListItem
                {
                    Text = string.Format("{0} {1}", item.FirstName, item.LastName),
                    Value = Convert.ToString(item.UserID)
                }));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the snapshot.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveSnapshot()
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var saved = false;
            var fileName = "snapshot" + currentDateTime.Ticks + ".png";
            var serverMapPath = Server.MapPath("~/Documents/Upload");
            if (Request.Form["base64data"] != null)
            {
                var image = Request.Form["base64data"].ToString();
                var data = Convert.FromBase64String(image);
                var path = Path.Combine(serverMapPath, fileName);
                System.IO.File.WriteAllBytes(path, data);
                saved = true;
                Session["AttachedSnapShot"] = path;
            }
            return Json(saved);
        }

        /// <summary>
        /// Gets the service code and desc list.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetServiceCodeAndDescList()
        {
            var list = new List<SelectListItem>();

            var finalList = _scService.GetServiceCodes(Helpers.DefaultServiceCodeTableNumber);
            if (finalList.Count > 0)
            {
                list.AddRange(finalList.Select(item => new SelectListItem
                {
                    Text = string.Format("{0} - {1}", item.ServiceCodeValue, item.ServiceCodeDescription),
                    Value = item.ServiceCodeValue.Trim()
                }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// Sends the reset password link.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
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
                VerificationLink = "/Home/UserResetPassword",
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

        /// <summary>
        /// Users the reset password.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="vtoken">The vtoken.</param>
        /// <returns></returns>
        public ActionResult UserResetPassword(string e, string vtoken)
        {
            var usersObj = _uService.GetUserByEmailAndToken(e, vtoken);
            usersObj.CodeValue = vtoken;
            usersObj.OldPassword = usersObj.Password;
            return View(usersObj);
        }

        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="vtoken">The vtoken.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Resets the new password.
        /// </summary>
        /// <param name="oUsersViewModel">The o users view model.</param>
        /// <returns></returns>
        [AllowAnonymous]
        public string ResetNewPassword(UsersViewModel oUsersViewModel)
        {
            var userObj = _uService.GetUserById(oUsersViewModel.UserID);
            userObj.ResetToken = string.Empty;
            userObj.Password = EncryptDecrypt.GetEncryptedData(oUsersViewModel.NewPassword, "");
            _uService.UpdateUser(userObj);
            return "Password reset successfully";
        }

        /// <summary>
        /// Sends the forgot password link.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
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
                    VerificationLink = "/Home/ForgotPassword",
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

        /// <summary>
        /// Sessions the timeout.
        /// </summary>
        /// <returns></returns>
        public ActionResult SessionTimeout()
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var objSession = Session[SessionNames.SessionClass.ToString()] as SessionClass;
            var userType = 1;
            var jsonObjreturn = false;
            if (objSession != null)
            {
                _ltService.UpdateLoginOutTime(objSession.UserId, Helpers.GetInvariantCultureDateTime());
                userType = objSession.LoginUserType;
                jsonObjreturn = true;
            }
            Session.RemoveAll();

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            var cookie2 = new HttpCookie("ASP.NET_SessionId", "") { Expires = currentDateTime.AddYears(-1) };
            Response.Cookies.Add(cookie2);

            // Invalidate the Cache on the Client Side
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetNoStore();


            return Json(jsonObjreturn);
        }

        /// <summary>
        /// Checks the tabs access.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public JsonResult CheckTabsAccess()
        {
            var ehrAccess = true;
            var acEncounterAccess = true;
            var authAccess = true;
            var pSearchAccess = true;
            if (Session[SessionNames.SessionClass.ToString()] != null && Session[SessionNames.SessionClass.ToString()] is SessionClass objSession)
            {
                ehrAccess = objSession.IsEhrAccessible;
                acEncounterAccess = objSession.IsActiveEncountersAccessible;
                authAccess = objSession.IsAuthorizationAccessible;
                pSearchAccess = objSession.IsPatientSearchAccessible;
            }
            var jsonResult = new
            {
                ehrAccess,
                acEncounterAccess,
                authAccess,
                pSearchAccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the rule editor users.
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetRuleEditorUsers()
        {
            var list = new List<DropdownListData>();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();

            var usersList = _uService.GetBillEditorUsers(corporateId, facilityId);
            list.AddRange(usersList.Select(item => new DropdownListData
            {
                Text = item.Name + " (" + item.RoleName + ")",
                Value = item.UserId.ToString(),
            }));

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the codes by facility.
        /// </summary>
        /// <param name="tableNumber"></param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetCodesByFacility(string tableNumber, string type)
        {
            // var tableNumber = GetTableNumber(corporateid, facilitynumber, type);
            var loggedinUserId = Helpers.GetLoggedInUserId();
            var viewpath = string.Empty;
            switch (type)
            {
                case "3": // ---- CPT Code Value
                    var finalList = _cptService.GetCptCodesListOnDemand(1, Helpers.DefaultRecordCount, Helpers.DefaultCptTableNumber); // --------- Get Service Codes for the table number
                    var viewData = new CPTCodesView
                    {
                        CPTCodesList = finalList,
                        CurrentCPTCode = new CPTCodes(),
                        UserId = loggedinUserId
                    };
                    viewpath = string.Format("../CPTCodes/{0}", PartialViews.CPTCodesList);

                    // Pass the ActionResult with List of CPTCodesViewModel object to Partial View CPTCodesList
                    return PartialView(viewpath, viewData);

                case "4": // ---- HCPCS Code Value
                    var finalList4 = _hcpcService.GetHCPCSCodesListOnDemand(1, Helpers.DefaultRecordCount, Helpers.DefaultHcPcsTableNumber); // --------- Get Service Codes for the table number
                    var viewData2 = new HCPCSCodesView
                    {
                        HCPCSCodesList = finalList4,
                        CurrentHCPCSCodes = new HCPCSCodes(),
                        UserId = loggedinUserId
                    };
                    viewpath = string.Format("../HCPCSCodes/{0}", PartialViews.HCPCSCodesList);

                    // Pass the ActionResult with List of CPTCodesViewModel object to Partial View CPTCodesList
                    return PartialView(viewpath, viewData2);

                case "5": // ---- DRUG Code Value
                    var finalList3 = _drugService.GetDrugListOnDemand(1, Helpers.DefaultRecordCount, "Active", Helpers.DefaultDrugTableNumber); // --------- Get Service Codes for the table number
                    var viewData1 = new DrugView
                    {
                        DrugList = finalList3,
                        CurrentDrug = new Drug(),
                        UserId = loggedinUserId
                    };
                    viewpath = string.Format("../Drug/{0}", PartialViews.DrugList);

                    // Pass the ActionResult with List of CPTCodesViewModel object to Partial View CPTCodesList
                    return PartialView(viewpath, viewData1);

                case "8": // ---- Service Code Value
                          // var finalList = bal.GetServiceCodes(); // --------- Get Service Codes for the table number
                    var serviceCodeList = _scService.GetServiceCodesCustomList(Helpers.DefaultServiceCodeTableNumber).OrderByDescending(f => f.ServiceCodeId).ToList();
                    var serviceCodeView = new ServiceCodeViewModel
                    {
                        //ServiceCodeListData = finalList,
                        ServiceCodeList = serviceCodeList,
                        CurrentServiceCode = new ServiceCode(),
                        UserId = Helpers.GetLoggedInUserId()
                    };
                    viewpath = string.Format("../ServiceCode/{0}", PartialViews.ServiceCodeList);

                    // Pass the ActionResult with List of ServiceCodeViewModel object to Partial View ServiceCodeList
                    return PartialView(viewpath, serviceCodeView);

                case "9": // ---- DRG Code Value
                    var finalList2 = _drgService.GetDrgCodesListOnDemand(1, Helpers.DefaultRecordCount, Helpers.DefaultDrgTableNumber); // --------- Get Service Codes for the table number
                    var drgCodesView = new DRGCodesView
                    {
                        DRGCodesList = finalList2,
                        CurrentDRGCodes = new DRGCodes(),
                        UserId = Helpers.GetLoggedInUserId()
                    };

                    viewpath = string.Format("../DRGCodes/{0}", PartialViews.DRGCodesList);

                    // Pass the ActionResult with List of ServiceCodeViewModel object to Partial View ServiceCodeList
                    return PartialView(viewpath, drgCodesView);

                case "16": // ---- Diagnosis Code Value
                    var finalList1 = _diacService.GetListOnDemand(1, Helpers.DefaultRecordCount, Helpers.DefaultDiagnosisTableNumber); // --------- Get Service Codes for the table number
                    var drgCodesView1 = new DiagnosisCodeView
                    {
                        DiagnosisCodeList = finalList1,
                        CurrentDiagnosisCode = new DiagnosisCode(),
                        UserId = Helpers.GetLoggedInUserId()
                    };
                    viewpath = string.Format("../DiagnosisCode/{0}", PartialViews.DiagnosisCodeList);

                    // Pass the ActionResult with List of ServiceCodeViewModel object to Partial View ServiceCodeList
                    return PartialView(viewpath, drgCodesView1);

                case "19": // ---- Bill Edit Rule Value
                    var list = _rmService.GetRuleMasterList(Helpers.DefaultBillEditRuleTableNumber);
                    viewpath = string.Format("../RuleMaster/{0}", PartialViews.RuleMasterList);

                    // Pass the ActionResult with List of ServiceCodeViewModel object to Partial View ServiceCodeList
                    return PartialView(viewpath, list);
            }

            return null;
        }

        /// <summary>
        /// Method is used to mask code as in-active
        /// </summary>
        /// <param name="codeValues"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public ActionResult MarkInActive(IEnumerable<string> codeValues, string orderType)
        {
            var codeType = (OrderType)Enum.Parse(typeof(OrderType), orderType);
            switch (codeType)
            {
                case OrderType.DRG:

                    foreach (var item in codeValues)
                    {
                        var model = _drgService.GetDrgCodesById(Convert.ToInt32(item));
                        model.IsActive = false;
                        _drgService.SaveDrgCode(model, Helpers.DefaultDrgTableNumber);
                    }
                    return Json("true");

                case OrderType.DRUG:
                    foreach (var item in codeValues)
                    {
                        var model = _drugService.GetDrugByID(Convert.ToInt32(item));
                        model.DrugStatus = "Deleted";
                        _drugService.AddUptdateDrug(model, Helpers.DefaultDrugTableNumber);
                    }
                    return Json("true");
                case OrderType.CPT:
                    foreach (var item in codeValues)
                    {
                        var model = _cptService.GetCPTCodesById(Convert.ToInt32(item));
                        model.IsActive = false;
                        _cptService.AddUpdateCPTCodes(model, Helpers.DefaultCptTableNumber);
                    }
                    return Json("true");

                case OrderType.BedCharges:

                    foreach (var item in codeValues)
                    {
                        var model = _scService.GetServiceCodeById(Convert.ToInt32(item));
                        model.IsActive = false;
                        _scService.AddUpdateServiceCode(model, Helpers.DefaultServiceCodeTableNumber);
                    }
                    return Json("true");

                case OrderType.HCPCS:
                    foreach (var item in codeValues)
                    {
                        var model = _hcpcService.GetHCPCSCodesById(Convert.ToInt32(item));
                        model.IsActive = false;
                        _hcpcService.AddHCPCSCodes(model, Helpers.DefaultHcPcsTableNumber);
                    }
                    return Json("true");
                case OrderType.Diagnosis:
                    foreach (var item in codeValues)
                    {
                        var model = _diacService.GetDiagnosisCodeByID(Convert.ToInt32(item));
                        model.IsDeleted = true;
                        _diacService.AddUptdateDiagnosisCode(model, Helpers.DefaultDiagnosisTableNumber);
                    }
                    return Json("true");
                case OrderType.BillEditRules:
                    var list = codeValues.Select(i => int.Parse(i)).ToList();
                    var result = _rmService.DeleteMultipleRules(list);
                    return Json(result ? "true" : "false", JsonRequestBehavior.AllowGet);
                default:
                    return Json(string.Empty);
            }
        }


        /// <summary>
        /// Gets the facility deapartments.
        /// </summary>
        /// <returns>Json List</returns>
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

        /// <summary>
        /// Determines whether [is in range] [the specified start time].
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns></returns>
        protected bool IsInRange(String startTime, String endTime)
        {
            var stringStartTime = Convert.ToString(Convert.ToDateTime(startTime).TimeOfDay);
            var stringEndTime = Convert.ToString(Convert.ToDateTime(endTime).TimeOfDay);
            var timeRange = TimeRange.Parse(stringStartTime + "-" + stringEndTime);

            var isNowInTheRange = timeRange.IsIn(Helpers.GetInvariantCultureDateTime().TimeOfDay);
            return isNowInTheRange;
        }

        /// <summary>
        /// Updates the failed log.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="failedLoginAttempt">The failed login attempt.</param>
        private void UpdateFailedLog(int userId, int failedLoginAttempt)
        {
            var objUsersViewModel = _uService.GetUserById(userId);
            objUsersViewModel.FailedLoginAttempts = failedLoginAttempt;
            objUsersViewModel.LastInvalidLogin = Helpers.GetInvariantCultureDateTime();
            _uService.AddUpdateUser(objUsersViewModel, 0);
        }



        /// <summary>
        /// Get Facilities list
        /// </summary>
        /// <returns></returns>
        [CustomAuth]
        public ActionResult GetFacilitiesDropdownData()
        {
            var cId = Helpers.GetDefaultCorporateId();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var facilities = userisAdmin ? _fService.GetFacilities(cId) : _fService.GetFacilities(cId, Helpers.GetDefaultFacilityId());
            if (facilities.Any())
            {
                var list = new List<SelectListItem>();
                list.AddRange(facilities.Select(item => new SelectListItem
                {
                    Text = item.FacilityName,
                    Value = Convert.ToString(item.FacilityId),
                }));

                list = list.OrderBy(f => f.Text).ToList();
                return Json(list);
            }
            return Json(null);
        }


        /// <summary>
        /// Method to get facilities on scheduler
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFacilitiesDropdownDataOnScheduler()
        {
            var cId = Helpers.GetSysAdminCorporateID();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var facilities = userisAdmin ? _fService.GetFacilities(cId) : _fService.GetFacilities(cId, Helpers.GetDefaultFacilityId());
            if (facilities.Any())
            {
                var list = new List<SelectListItem>();
                list.AddRange(facilities.Select(item => new SelectListItem
                {
                    Text = item.FacilityName,
                    Value = Convert.ToString(item.FacilityId),
                }));

                list = list.OrderBy(f => f.Text).ToList();
                return Json(list);
            }
            return Json(null);
        }
        /// <summary>
        /// Gets the facilty list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFaciltyListTreeView()
        {
            // Get the facilities list
            var cId = Helpers.GetSysAdminCorporateID();
            var facilityList = _fService.GetFacilityList(cId);
            var viewpath = string.Format("../Scheduler/{0}", PartialViews.LocationListView);
            return PartialView(viewpath, facilityList);
        }

        /// <summary>
        /// Loads the facility department data.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public ActionResult LoadFacilityDepartmentData(string facilityid)
        {
            // Get the facilities list
            var cId = Helpers.GetSysAdminCorporateID();
            var facilityDepartmentList = _fsService.GetFacilityDepartments(cId, facilityid);
            var viewpath = string.Format("../Scheduler/{0}", PartialViews.FacilityDepartmentListView);
            return PartialView(viewpath, facilityDepartmentList);

        }

        /// <summary>
        /// Loads the facility rooms data.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public ActionResult LoadFacilityRoomsData(string facilityid)
        {
            // Get the facilities list
            var cId = Helpers.GetSysAdminCorporateID();
            var lst = _fsService.GetFacilityRooms(cId, facilityid);
            var viewpath = string.Format("../Scheduler/{0}", PartialViews.FacilityRoomsListView);
            return PartialView(viewpath, lst);
        }


        /// <summary>
        /// Loads the facility rooms data custom.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public ActionResult LoadFacilityRoomsDataCustom(string facilityid)
        {
            // Get the facilities list
            var cId = Helpers.GetSysAdminCorporateID();
            var facilityDepartmentList = _fsService.GetFacilityRoomsCustomModel(cId, facilityid);
            var viewpath = string.Format("../Scheduler/{0}", PartialViews.FacilityRoomsListView);
            return PartialView(viewpath, facilityDepartmentList);
        }

        /// <summary>
        /// Gets the department rooms.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        public ActionResult GetDepartmentRooms(List<SchedularTypeCustomModel> filters)
        {
            var selectedDepartmentList = filters[0].DeptData;
            var facilityid = filters[0].Facility;
            //var deptIds = string.Join(",", selectedDepartmentList.Select(x => x.Id));
            var facilityDepartmentList = _fsService.GetDepartmentRooms(selectedDepartmentList, facilityid);
            var viewpath = string.Format("../Scheduler/{0}", PartialViews.FacilityRoomsListView);
            return PartialView(viewpath, facilityDepartmentList);
        }



        #region Telerik Schedular Methods ---Just for testing Purposes
        [AllowAnonymous]
        public ViewResult Temp()
        {
            return View();
        }

        private List<TaskViewModel> GetTasksList()
        {
            var curentDateTime = Helpers.GetInvariantCultureDateTime();
            var list = new List<TaskViewModel>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(new TaskViewModel
                {
                    TaskID = i,
                    Description = string.Format("Test Description of Task {0}", i),
                    End = curentDateTime.AddDays(20 + i),
                    EndTimezone = "Etc/UTC",
                    IsAllDay = i % 2 == 0,
                    OwnerID = i + 1,
                    RecurrenceException = string.Empty,
                    RecurrenceID = i,
                    RecurrenceRule = "Test Rule of Task" + i,
                    Start = curentDateTime.AddDays(-10).AddDays(i),
                    StartTimezone = "Etc/UTC",
                    Title = "Scheduling Task " + i
                });
            }
            return list;
        }


        [AllowAnonymous]
        public virtual JsonResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetTasksList().ToDataSourceResult(request));
        }

        [AllowAnonymous]
        public virtual JsonResult Destroy([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                //taskService.Delete(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public virtual JsonResult Create([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                //taskService.Insert(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public virtual JsonResult Update([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                //taskService.Update(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        [AllowAnonymous]
        public virtual JsonResult Save(string value)
        {
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        #endregion


        /// <summary>
        /// Gets the department name by room identifier.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <returns></returns>
        public JsonResult GetDepartmentNameByRoomId(int roomId)
        {
            var departmentName = _fsService.GetParentNameByFacilityStructureId(roomId);
            return Json(departmentName, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the facility rooms.
        /// </summary>
        /// <param name="coporateId">The coporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public JsonResult GetFacilityRooms(int coporateId, int facilityId)
        {
            var facilityDepartmentList = _fsService.GetFacilityRooms(coporateId, facilityId.ToString());
            return Json(facilityDepartmentList, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// Validates the department rooms.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="deptid">The deptid.</param>
        /// <returns></returns>
        public ActionResult ValidateDepartmentRooms(string facilityid, int deptid)
        {
            var lst = _fsService.GetDepartmentRooms(deptid, facilityid);
            return Json(lst.Count > 0, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the physicians appt types.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="deptid">The deptid.</param>
        /// <returns></returns>
        public ActionResult GetPhysiciansApptTypes(string facilityid, int deptid)
        {
            var lst = _fsService.GetDepartmentAppointmentTypes(deptid, facilityid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFilteredCodesInFav(string text, string searchType, string blockNumber = null)
        {
            var tableNumber = string.Empty;
            var st = Convert.ToInt32(searchType);
            var viewpath = string.Empty;
            var codeType = (SearchType)Enum.Parse(typeof(SearchType), searchType);
            switch (codeType)
            {
                case SearchType.ServiceCode:
                    tableNumber = Helpers.DefaultServiceCodeTableNumber;
                    var userid = Helpers.GetLoggedInUserId();
                    viewpath = string.Format("../ServiceCode/{0}", PartialViews.ServiceCodeList);
                    var result6 = !string.IsNullOrEmpty(text) ?
                        _scService.GetFilteredServiceCodes(text, tableNumber) :
                        _scService.GetServiceCodesCustomList(Helpers.DefaultServiceCodeTableNumber);

                    var serviceCodeView = new ServiceCodeViewModel
                    {
                        ServiceCodeList = result6,
                        CurrentServiceCode = new ServiceCode(),
                        UserId = userid
                    };
                    return PartialView(viewpath, serviceCodeView);
                case SearchType.CPT:
                    tableNumber = Helpers.DefaultCptTableNumber;
                    viewpath = string.Format("../CPTCodes/{0}", PartialViews.CPTCodesList);
                    var result = !string.IsNullOrEmpty(text) ? _cptService.GetFilteredCptCodes(text, tableNumber) : _cptService.GetCPTCodes(Helpers.DefaultCptTableNumber);
                    var viewData = new CPTCodesView
                    {
                        CPTCodesList = result,
                        CurrentCPTCode = new CPTCodes(),
                        UserId = Helpers.GetLoggedInUserId()
                    };
                    return PartialView(viewpath, viewData);
                case SearchType.DRG:

                    tableNumber = Helpers.DefaultDrgTableNumber;
                    viewpath = string.Format("../DRGCodes/{0}", PartialViews.DRGCodesList);
                    var result5 = !string.IsNullOrEmpty(text) ? _drgService.GetDRGCodesFiltered(text, tableNumber) : _drgService.GetDrgCodes(Helpers.DefaultDrgTableNumber);

                    var drgCodesView = new DRGCodesView
                    {
                        DRGCodesList = result5,
                        CurrentDRGCodes = new DRGCodes(),
                        UserId = Helpers.GetLoggedInUserId()
                    };

                    return PartialView(viewpath, drgCodesView);
                case SearchType.HCPCS:
                    tableNumber = Helpers.DefaultHcPcsTableNumber;
                    viewpath = string.Format("../HCPCSCodes/{0}", PartialViews.HCPCSCodesList);
                    var result4 = !string.IsNullOrEmpty(text) ? _hcpcService.GetHCPCSCodesFilterData(text, tableNumber) : _hcpcService.GetHCPCSCodes(Helpers.DefaultHcPcsTableNumber);
                    var hcpcsCodesView = new HCPCSCodesView
                    {
                        HCPCSCodesList = result4,
                        CurrentHCPCSCodes = new HCPCSCodes(),
                        UserId = Helpers.GetLoggedInUserId()
                    };
                    return PartialView(viewpath, hcpcsCodesView);
                case SearchType.Denial:
                    viewpath = string.Format("../Denial/{0}", PartialViews.DenialList);
                    var result3 = !string.IsNullOrEmpty(text) ? _denService.GetFilteredDenialCodes(text) : _denService.GetDenial();

                    var denialView = new DenialView
                    {
                        DenialList = result3,
                        CurrentDenial = new Denial()
                    };

                    return PartialView(viewpath, result3);
                case SearchType.Diagnosis:

                    tableNumber = Helpers.DefaultDiagnosisTableNumber;
                    viewpath = string.Format("../DiagnosisCode/{0}", PartialViews.DiagnosisCodeList);
                    var result2 = !string.IsNullOrEmpty(text) ? _diacService.GetFilteredDiagnosisCodesData(text, tableNumber) : _diacService.GetDiagnosisCode(Helpers.DefaultDiagnosisTableNumber);

                    if (blockNumber != null)
                    {
                        var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);
                        result2 = result2.ToList().OrderByDescending(i => i.DiagnosisTableNumberId).Take(takeValue).ToList();
                    }
                    var diagnosisCodeView = new DiagnosisCodeView
                    {
                        DiagnosisCodeList = result2,
                        CurrentDiagnosisCode = new Model.DiagnosisCode(),
                        UserId = Helpers.GetLoggedInUserId()
                    };
                    return PartialView(viewpath, diagnosisCodeView);
                case SearchType.DRUG:
                    tableNumber = Helpers.DefaultDrugTableNumber;
                    viewpath = string.Format("../Drug/{0}", PartialViews.DrugList);
                    var result1 = !string.IsNullOrEmpty(text) ? _drugService.GetFilteredDrugCodesData(text, "0", tableNumber) : _drugService.GetDrugList(Helpers.DefaultDrugTableNumber);

                    if (blockNumber != null)
                    {
                        var takeValue = Convert.ToInt32(Helpers.DefaultRecordCount) * Convert.ToInt32(blockNumber);
                        result1 = result1.ToList().OrderByDescending(i => i.Id).Take(takeValue).ToList();
                    }
                    var viewData1 = new DrugView
                    {
                        CurrentDrug = new Drug(),
                        DrugList = result1,
                        UserId = Helpers.GetLoggedInUserId()
                    };

                    return PartialView(viewpath, viewData1);
                default:
                    break;
            }
            //}
            return PartialView();
        }



        [AllowAnonymous]
        public ViewResult UnauthorizedView()
        {
            return View();
        }

        [CustomAuth]
        public JsonResult SaveFilesTemporarily()
        {
            var result = false;
            if (Request.Files.Count > 0)
            {
                Session[SessionNames.Files.ToString()] = Request.Files;
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExportCodesToExcel(string searchText, string codeType, string tn)
        {
            var excelData = new ExcelData { FreezeTopRow = true, AreCustomColumns = false, SheetName = $"{codeType} Codes", FileName = $"{codeType}CodesFile-{DateTime.Now.ToString("yyyy-MM-dd")}.xls" };
            var columns = string.Empty;
            excelData.Data = GetBillingCodesToExport(codeType, searchText, tn, out columns);

            var result = ExcelExportHelper.ExportExcel(excelData);
            return result;
        }

        private DataTable GetBillingCodesToExport(string codeType, string sText, string tn, out string columns)
        {
            var codeTableNo = string.Empty;
            List<ExportCodesData> data = null;
            if (string.IsNullOrEmpty(tn))
            {
                switch (codeType.ToLower())
                {
                    case "cpt":
                        codeTableNo = Helpers.DefaultCptTableNumber;
                        break;
                    case "hcpcs":
                        codeTableNo = Helpers.DefaultHcPcsTableNumber;
                        break;
                    case "drug":
                        codeTableNo = Helpers.DefaultDrugTableNumber;
                        break;
                    case "drg":
                        codeTableNo = Helpers.DefaultDrgTableNumber;
                        break;
                    case "diagnosis":
                        codeTableNo = Helpers.DefaultDiagnosisTableNumber;
                        break;
                    default:
                        break;
                }
            }
            else
                codeTableNo = tn;

            data = _cptService.GetCodesDataToExport(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId()
               , codeTableNo, codeType, sText, out columns);

            var dt = Helpers.FillDataTableToExport(data, codeType);
            return dt;
        }

        public int ImportAndSaveBillingCodesToDB(string codeType, HttpPostedFileBase file)
        {
            var dsResult = new DataSet();
            var result = false;
            if (file != null && file.ContentLength > 0)
            {
                if (file.FileName.Contains(".xlsx") || file.FileName.Contains(".xls"))
                {
                    Stream stream = file.InputStream;
                    IExcelDataReader reader = null;

                    if (Path.GetExtension(file.FileName).Equals(".xlsx"))
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);

                    reader.IsFirstRowAsColumnNames = true;

                    dsResult = reader.AsDataSet();

                    reader.Close();
                    stream.Close();

                    var codeTableNo = string.Empty;
                    switch (codeType.ToLower())
                    {
                        case "cpt":
                            codeTableNo = Helpers.DefaultCptTableNumber;
                            break;
                        case "hcpcs":
                            codeTableNo = Helpers.DefaultHcPcsTableNumber;
                            break;
                        case "drug":
                            codeTableNo = Helpers.DefaultDrugTableNumber;
                            break;
                        case "drg":
                            codeTableNo = Helpers.DefaultDrgTableNumber;
                            break;
                        case "diagnosis":
                            codeTableNo = Helpers.DefaultDiagnosisTableNumber;
                            break;
                        default:
                            break;
                    }

                    var status = (int)ExcelImportResultCodes.Initialized;
                    var dt = Helpers.FillDataTableToImport(dsResult.Tables[0], codeType, out status);

                    if (status == (int)ExcelImportResultCodes.Success)
                        result = _cptService.ImportAndSaveCodesToDatabase(codeType, Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), codeTableNo, string.Empty, Helpers.GetLoggedInUserId(), dt);
                    else
                        return status;
                }
            }
            return result ? 1 : 0;
        }

        [CustomAuth]
        public ActionResult GetCountriesWithDefault()
        {
            var list = _cService.GetCountryWithCode().OrderBy(x => x.CountryName);
            var defaultCountry = Helpers.GetDefaultCountryCode;

            //var countryId = defaultCountry > 0 ? list.Where(a => a.CodeValue == Convert.ToString(defaultCountry))
            //    .Select(s => s.CountryID).FirstOrDefault() : 0;
            var jsonData = new { list, defaultCountry };
            return Json(jsonData);
        }


        public JsonResult CalculateAgeInYears(DateTime dValue)
        {
            var ageInYears = GetAgeInYears(dValue);
            return Json(ageInYears, JsonRequestBehavior.AllowGet);
        }

        /// <summary>  
        /// For calculating age  
        /// </summary>  
        /// <param name="Dob">Enter Date of Birth to Calculate the age</param>  
        /// <returns> years, months,days, hours...</returns>  
        static int GetAgeInYears(DateTime Dob)
        {
            var Now = DateTime.Now;
            var ticks = Now.Subtract(Dob).Ticks;
            int Years = ticks > 0 ? new DateTime(ticks).Year - 1 : 0;
            return Years;

            //var PastYearDate = Dob.AddYears(Years);
            //int Months = 0;
            //for (int i = 1; i <= 12; i++)
            //{
            //    if (PastYearDate.AddMonths(i) == Now)
            //    {
            //        Months = i;
            //        break;
            //    }
            //    else if (PastYearDate.AddMonths(i) >= Now)
            //    {
            //        Months = i - 1;
            //        break;
            //    }
            //}
            //int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
            //int Hours = Now.Subtract(PastYearDate).Hours;
            //int Minutes = Now.Subtract(PastYearDate).Minutes;
            //int Seconds = Now.Subtract(PastYearDate).Seconds;
            //return String.Format("Age: {0} Year(s) {1} Month(s) {2} Day(s) {3} Hour(s) {4} Second(s)",
            //Years, Months, Days, Hours, Seconds);
        }


        #region Not in Use


        ///// <summary>
        ///// Method is used to bind the user type drop down
        ///// </summary>
        ///// <param name="corporateId"></param>
        ///// <param name="facilityId"></param>
        ///// <returns></returns>
        //public JsonResult BindUsersType(string corporateId, string facilityId)
        //{
        //    var list = new List<DropdownListData>();
        //    var roleList = _frService.GetUserTypeRoleDropDown(Convert.ToInt32(corporateId), Convert.ToInt32(facilityId), true);
        //    if (roleList.Count > 0)
        //    {
        //        list.AddRange(roleList.Select(item => new DropdownListData
        //        {
        //            Text = string.Format("{0}", item.RoleName),
        //            Value = Convert.ToString(item.RoleId)
        //        }));
        //    }
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Download(int fileId)
        //{
        //    var file = _docService.GetDocumentById(fileId);
        //    if (file != null)
        //    {
        //        var fullPath = Server.MapPath("~" + file.FilePath);
        //        if (System.IO.File.Exists(fullPath))
        //        {
        //            var cd = new ContentDisposition
        //            {
        //                FileName = file.FileName,
        //                // always prompt the user for downloading, set to true if you want 
        //                // the browser to try to show the file inline
        //                Inline = false,
        //            };
        //            var contentType = MimeMapping.GetMimeMapping(fileName: file.FileName);
        //            Response.AppendHeader("Content-Disposition", cd.ToString());
        //            //var ext = Path.GetExtension(file.FileName);
        //            //var fileType = (MimeTypes)Enum.Parse(typeof(MimeTypes), ext);
        //            //var contentType = fileType.GetEnumDescription();
        //            return File(fullPath, contentType, file.FileName);
        //        }
        //    }
        //    return Content("File No Found");
        //}

        //public JsonResult GetDepartmentTiming(int deptId)
        //{
        //    var deptTimingList = _deptService.GetDeptTimmingByDepartmentId(deptId);
        //    var listToReturn = new
        //    {
        //        deptOpeningDays = string.Join(",", deptTimingList.Select(x => x.OpeningDayId)),
        //        deptTimingList,
        //    };
        //    return Json(listToReturn, JsonRequestBehavior.AllowGet);
        //}


        ///// <summary>
        ///// Gets the corporate physicians.
        ///// </summary>
        ///// <param name="corporateId">The corporate identifier.</param>
        ///// <param name="facilityId">The facility identifier.</param>
        ///// <returns></returns>
        //public ActionResult GetCorporatePhysicians(string corporateId, string facilityId)
        //{
        //    var cId = string.IsNullOrEmpty(corporateId) ? Helpers.GetSysAdminCorporateID().ToString() : corporateId;
        //    cId = string.IsNullOrEmpty(facilityId)
        //              ? cId
        //              : Helpers.GetCorporateIdByFacilityId(Convert.ToInt32(facilityId)).ToString();
        //    var isAdmin = Helpers.GetLoggedInUserIsAdmin();
        //    var userid = Helpers.GetLoggedInUserId();
        //    var corporateUsers = _phService.GetCorporatePhysiciansList(Convert.ToInt32(cId), isAdmin, userid, Convert.ToInt32(facilityId));
        //    var viewpath = string.Format("../Scheduler/{0}", PartialViews.PhysicianCheckBoxList);
        //    return PartialView(viewpath, corporateUsers);
        //}

        //public async Task<ActionResult> PatientAction(List<SchedulingCustomModel> list, int actionId, string status, int patientId, int physicianId)
        //{
        //    var success = false;
        //    //get the Physician Details by Current Physician ID.
        //    var objPhysician = _uService.GetPhysicianById(Convert.ToInt32(physicianId));
        //    var validRequest = objPhysician != null && objPhysician.UserId > 0;
        //    if (validRequest)
        //    {
        //        //get the Sender's mail address of current User.
        //        var email = _uService.GetUserEmailByUserId(Convert.ToInt32(objPhysician.UserId));

        //        success = await Helpers.SendAppointmentNotification(list, email,
        //            Convert.ToString((int)SchedularNotificationTypes.appointmentapprovaltophysician), patientId, Convert.ToInt32(physicianId), 2);

        //        //If Success, Update the Scheduling list with the status 'Confirmed' and delete the verification token.
        //        if (success)
        //        {
        //            list.ForEach(a =>
        //            {
        //                a.Status = status;
        //                a.ExtValue4 = string.Empty;
        //                a.ModifiedBy = patientId;
        //                a.ModifiedDate = Helpers.GetInvariantCultureDateTime();
        //            });

        //            success = _schService.UpdateSchedulingEvents(list);
        //        }
        //    }
        //    return Json(success ? 1 : 0, JsonRequestBehavior.AllowGet);
        //}
        #endregion
    }

    public class TaskViewModel : ISchedulerEvent
    {
        public int TaskID { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public int RecurrenceID { get; set; }
        public int OwnerID { get; set; }
    }

}