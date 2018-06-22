using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BillingSystem.Models;
using CaptchaMvc.HtmlHelpers;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Globalization;
using System.Threading;
using System.Web;
using Kendo.Mvc.Extensions;
using Microsoft.Ajax.Utilities;
using Kendo.Mvc.UI;
using NPOI.SS.Formula.Functions;

namespace BillingSystem.Controllers
{
    public class HomeTestController : Controller
    {


        protected bool IsInRange(String startTime, String endTime)
        {
            var stringStartTime = Convert.ToString(Convert.ToDateTime(startTime).TimeOfDay);
            var stringEndTime = Convert.ToString(Convert.ToDateTime(endTime).TimeOfDay);
            var timeRange = TimeRange.Parse(stringStartTime + "-" + stringEndTime);

            var isNowInTheRange = timeRange.IsIn(Helpers.GetInvariantCultureDateTime().TimeOfDay);
            return isNowInTheRange;
        }

        private List<RoleSelectionCustomModel> GetUserRoles(int userid)
        {
            var userroleList = new List<RoleSelectionCustomModel>();
            var userroleBal = new UserRoleBal();
            var roleBal = new RoleBal();
            var facilityRole = new FacilityRoleBal();
            var facility = new FacilityBal();
            var roles = userroleBal.GetUserRolesByUserId(userid);
            foreach (var role in roles)
            {
                var roleFacilityIds = facilityRole.GetFacilityRolesByRoleId(role.RoleID);
                userroleList.AddRange(roleFacilityIds.Select(rolefacility => new RoleSelectionCustomModel
                {
                    RoleId = role.RoleID,
                    RoleName = roleBal.GetRoleNameById(role.RoleID),
                    FacilityName = facility.GetFacilityNameById(rolefacility.FacilityId),
                    FacilityId = rolefacility.FacilityId,
                    CorporateId = rolefacility.CorporateId
                }));
            }
            return userroleList;
        }



        // GET: HomeTest
        public ActionResult Index()
        {
            var Rqstid = Request.QueryString["id"];

            //Changes by Amit Jain on 07102014
            //Changes start here
            //var objSystemConfigurationWebCommunicator = new SystemConfigurationWebCommunicator();
            //var _configdata = objSystemConfigurationWebCommunicator.getOfflineTime();
            var login = new Users();

            var systemConfigurationBal = new SystemConfigurationBal();
            var configdata = systemConfigurationBal.getOfflineTime();
            //Changes end here

            var startTime = (TimeSpan)configdata.LoginStartTime;
            var endTime = (TimeSpan)configdata.LoginEndTime;
            var starttimetext = Convert.ToDateTime(string.Format("{0:hh\\:mm\\:ss}", startTime)).ToLongTimeString();
            var endtimetext = Convert.ToDateTime(string.Format("{0:hh\\:mm\\:ss}", endTime)).ToLongTimeString();

            var flag = true;

            //Check the Captcha Code below
            //if (!this.IsCaptchaValid(string.Empty)) // means codes does not matched
            //{
            //    ViewBag.check = LoginResponseTypes.CaptchaFailed.ToString();
            //    return View(login);
            //}

            //Authenticate the user details here
            var usersBal = new UsersBal();
            var currentUser = usersBal.GetUser("sysadmin", "Loveme1961");//added jagjeet 07102014

            if (!IsInRange(starttimetext, endtimetext))//only check for users who do not have assigned system offline overwrite' user role)
            {
                ViewBag.check = "Offline (" + string.Format("{0:HH:mm:ss tt}", endtimetext)
                   + " to " + string.Format("{0:HH:mm:ss tt}", starttimetext) + ")";
            }
            else
            {
                //Changes by Amit Jain and added a check if object is not null
                var encryptPassword = !string.IsNullOrEmpty("Loveme1961") ? EncryptDecrypt.GetEncryptedData("Loveme1961", string.Empty) : string.Empty;
                if (currentUser != null && currentUser.UserName != null && currentUser.Password.Equals(encryptPassword) && currentUser.IsActive && (currentUser.IsDeleted != null && !Convert.ToBoolean(currentUser.IsDeleted)))
                {
                    if (currentUser.FailedLoginAttempts == 3)
                    {
                        var failedlogin = Convert.ToDateTime(currentUser.LastInvalidLogin);
                        var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
                        if (timespan.TotalMinutes < 30)
                        {
                            flag = false;
                            ViewBag.check = LoginResponseTypes.FailedAttemptsOver.ToString();
                        }
                    }

                    var bal = new LoginTrackingBal();
                    if (flag)
                    {
                        ViewBag.check = LoginResponseTypes.Success.ToString();
                        //Commented by Amit Jain
                        //Menu Manipulations 
                        //var tabsList = usersBal.GetTabsByUserName(model.UserName);
                        //System.Web.HttpContext.Current.Session["MenuSession"] = tabsList;
                        var objSession = Session[SessionNames.SessionClass.ToString()] != null
                            ? Session[SessionNames.SessionClass.ToString()] as SessionClass
                            : new SessionClass();

                        if (objSession != null)
                        {
                            objSession.UserEmail = currentUser.Email;//Added by Nitin on 20170806
                            objSession.MenuSessionList = null;
                            objSession.FirstTimeLogin = bal.IsFirstTimeLoggedIn(currentUser.UserID,
                                (int)LoginTrackingTypes.UserLogin);

                            var loginTrackingVm = new LoginTracking
                            {
                                ID = currentUser.UserID,
                                LoginTime = Helpers.GetInvariantCultureDateTime(),
                                LoginUserType = (int)LoginTrackingTypes.UserLogin,
                                FacilityId = currentUser.FacilityId,
                                CorporateId = currentUser.CorporateId,
                                IsDeleted = false,
                                IPAddress = Helpers.GetUser_IP(),
                                CreatedBy = currentUser.UserID,
                                CreatedDate = Helpers.GetInvariantCultureDateTime()
                            };

                            bal.AddUpdateLoginTrackingData(loginTrackingVm);

                            // UpdateFailedLog(currentUser.UserID, 0);
                            /*
                             * Owner: Amit Jain
                             * On: 05102014
                             * Purpose: Change the home page after login success
                             * Earlier: It was PatientLogin, now its PatientSearch
                             */
                            //Changes start here
                            //return RedirectToAction("PatientLogin");
                            objSession.FacilityNumber = "1002";
                            objSession.UserName = currentUser.UserName;
                            objSession.UserId = currentUser.UserID;
                            objSession.SelectedCulture = CultureInfo.CurrentCulture.Name;
                            objSession.LoginUserType = (int)LoginTrackingTypes.UserLogin;

                            var assignedRoles = GetUserRoles(currentUser.UserID);
                            if (assignedRoles.Count == 1)
                            {
                                var currentRole = assignedRoles[0];
                                objSession.FacilityId = currentRole.FacilityId;
                                objSession.RoleId = currentRole.RoleId;
                                objSession.CorporateId = currentRole.CorporateId;
                                // Changed by Shashank ON : 5th May 2015 : To add the Module access level Security when user log in via Facility and Corporate 
                                using (var userbal = new UsersBal())
                                    objSession.MenuSessionList = userbal.GetTabsByUserIdRoleId(objSession.UserId, objSession.RoleId, currentRole.FacilityId, currentRole.CorporateId, isDeleted: false, isActive: true);

                                var moduleAccessBal = new ModuleAccessBal();
                                Session[SessionNames.SessoionModuleAccess.ToString()] =
                                    moduleAccessBal.GetModulesAccessList(currentRole.CorporateId, currentRole.FacilityId);

                                using (var userBal = new UsersBal())
                                {
                                    var userDetails = userBal.GetUserDetails(currentRole.RoleId, currentRole.FacilityId, objSession.UserId);
                                    objSession.RoleName = userDetails.RoleName;
                                    objSession.FacilityName = userDetails.DefaultFacility;
                                    objSession.UserName = userDetails.UserName;
                                    objSession.FacilityNumber = userDetails.FacilityNumber;
                                    objSession.UserIsAdmin = userDetails.UserIsAdmin;
                                    objSession.LoginUserType = (int)LoginTrackingTypes.UserLogin;
                                }

                                using (var facilitybal = new FacilityBal())
                                {
                                    var facilityObj = facilitybal.GetFacilityByFacilityId(currentRole.FacilityId);
                                    var timezoneValue = facilityObj.FacilityTimeZone;
                                    if (!string.IsNullOrEmpty(timezoneValue))
                                    {
                                        var timezoneobj = TimeZoneInfo.FindSystemTimeZoneById(timezoneValue);
                                        objSession.TimeZone = timezoneobj.BaseUtcOffset.TotalHours.ToString();
                                    }
                                    else
                                        objSession.TimeZone = "0.0";
                                }

                                using (var rtBal = new RoleTabsBal())
                                {
                                    objSession.IsPatientSearchAccessible = rtBal.CheckIfTabNameAccessibleToGivenRole("Patient Lookup",
                                        ControllerAccess.PatientSearch.ToString(), ActionNameAccess.PatientSearch.ToString(),
                                        Convert.ToInt32(currentRole.RoleId));
                                    objSession.IsAuthorizationAccessible =
                                        rtBal.CheckIfTabNameAccessibleToGivenRole("Obtain Insurance Authorization",
                                            ControllerAccess.Authorization.ToString(),
                                            ActionNameAccess.AuthorizationMain.ToString(), Convert.ToInt32(currentRole.RoleId));
                                    objSession.IsActiveEncountersAccessible =
                                        rtBal.CheckIfTabNameAccessibleToGivenRole("Active Encounters",
                                            ControllerAccess.ActiveEncounter.ToString(),
                                            ActionNameAccess.ActiveEncounter.ToString(),
                                            Convert.ToInt32(currentRole.RoleId));
                                    objSession.IsBillHeaderViewAccessible =
                                        rtBal.CheckIfTabNameAccessibleToGivenRole("Generate Preliminary Bill",
                                            ControllerAccess.BillHeader.ToString(),
                                            ActionNameAccess.Index.ToString(), Convert.ToInt32(currentRole.RoleId));
                                    objSession.IsEhrAccessible =
                                        rtBal.CheckIfTabNameAccessibleToGivenRole("EHR",
                                            ControllerAccess.Summary.ToString(),
                                            ActionNameAccess.PatientSummary.ToString(), Convert.ToInt32(currentRole.RoleId));

                                    objSession.SchedularAccessible =
                                        rtBal.CheckIfTabNameAccessibleToGivenRole("Scheduling", string.Empty, string.Empty, Convert.ToInt32(currentRole.RoleId));
                                }

                                /*
                                 * By: Amit Jain
                                 * On: 24082015
                                 * Purpose: Setting up the table numbers for the Billing Codes
                                 */
                                //----Billing Codes' Table Number additions start here---------------
                                if (objSession.CorporateId > 0 && !string.IsNullOrEmpty(objSession.FacilityNumber))
                                {
                                    using (var bBal = new BillingSystemParametersBal())
                                    {
                                        var currentParameter = bBal.GetDetailsByCorporateAndFacility(
                                            objSession.CorporateId, objSession.FacilityNumber);

                                        var cDetails = new Corporate();
                                        using (var cBal = new CorporateBal())
                                            cDetails = cBal.GetCorporateById(objSession.CorporateId);


                                        if (objSession.UserId != 1)
                                        {
                                            objSession.CptTableNumber =
                                                currentParameter != null && !string.IsNullOrEmpty(currentParameter.CPTTableNumber)
                                                    ? currentParameter.CPTTableNumber
                                                    : cDetails.DefaultCPTTableNumber;

                                            objSession.ServiceCodeTableNumber =
                                                currentParameter != null && !string.IsNullOrEmpty(currentParameter.ServiceCodeTableNumber)
                                                    ? currentParameter.ServiceCodeTableNumber
                                                    : cDetails.DefaultServiceCodeTableNumber;

                                            objSession.DrugTableNumber =
                                                currentParameter != null && !string.IsNullOrEmpty(currentParameter.DrugTableNumber)
                                                    ? currentParameter.DrugTableNumber
                                                    : cDetails.DefaultDRUGTableNumber;

                                            objSession.DrgTableNumber =
                                                currentParameter != null && !string.IsNullOrEmpty(currentParameter.DRGTableNumber)
                                                    ? currentParameter.DRGTableNumber
                                                    : cDetails.DefaultDRGTableNumber;

                                            objSession.HcPcsTableNumber =
                                                currentParameter != null && !string.IsNullOrEmpty(currentParameter.HCPCSTableNumber)
                                                    ? currentParameter.HCPCSTableNumber
                                                    : cDetails.DefaultHCPCSTableNumber;

                                            objSession.DiagnosisCodeTableNumber =
                                                currentParameter != null && !string.IsNullOrEmpty(currentParameter.DiagnosisTableNumber)
                                                    ? currentParameter.DiagnosisTableNumber
                                                    : cDetails.DefaultDiagnosisTableNumber;


                                            objSession.BillEditRuleTableNumber =
                                                currentParameter != null && !string.IsNullOrEmpty(currentParameter.BillEditRuleTableNumber)
                                                    ? currentParameter.BillEditRuleTableNumber
                                                    : cDetails.BillEditRuleTableNumber;
                                        }
                                        else
                                        {
                                            objSession.CptTableNumber = "0";
                                            objSession.ServiceCodeTableNumber = "0";
                                            objSession.DrugTableNumber = "0";
                                            objSession.DrgTableNumber = "0";
                                            objSession.HcPcsTableNumber = "0";
                                            objSession.DiagnosisCodeTableNumber = "0";
                                            objSession.BillEditRuleTableNumber = "0";
                                        }
                                    }
                                }
                                //----Billing Codes' Table Number additions end here---------------

                            }
                            Session[SessionNames.SessionClass.ToString()] = objSession;
                        }
                        //return RedirectToAction("PatientSearch", "PatientSearch");  
                        if (Rqstid == "1")
                            return RedirectToAction("RegisterPatient", "PatientInfo");
                        else if (Rqstid == "2")
                            return RedirectToAction("PatientSummary", "Summary", new { pId = 1095 });
                        //Changes end here
                    }
                }
                else
                {
                    if (currentUser != null)
                    {
                        if (currentUser.Password == null || !currentUser.Password.Equals(encryptPassword))
                            ViewBag.check = LoginResponseTypes.Failed.ToString();
                        else if (!currentUser.IsActive)
                            ViewBag.check = LoginResponseTypes.InActive.ToString();
                        else if (currentUser.IsDeleted != null && Convert.ToBoolean(currentUser.IsDeleted))
                            ViewBag.check = LoginResponseTypes.IsDeleted.ToString();
                        else
                            ViewBag.check = LoginResponseTypes.Failed.ToString();
                    }
                    else
                        ViewBag.check = LoginResponseTypes.Failed.ToString();

                    if (currentUser != null && !string.IsNullOrEmpty(currentUser.UserName) && ViewBag.check == LoginResponseTypes.Failed.ToString())
                    {
                        if (currentUser.FailedLoginAttempts < 3 || currentUser.FailedLoginAttempts == null)
                        {
                            var failedlogin = Convert.ToDateTime(currentUser.LastInvalidLogin);
                            var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
                            var failedattempts = timespan.TotalMinutes < 30 ? Convert.ToInt32(currentUser.FailedLoginAttempts) + 1 : 1;
                            //  UpdateFailedLog(currentUser.UserID, failedattempts);
                        }
                        else if (currentUser.FailedLoginAttempts == 3)
                        {
                            var failedlogin = Convert.ToDateTime(currentUser.LastInvalidLogin);
                            var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
                            if (timespan.TotalMinutes < 30)
                                flag = false;
                        }
                    }
                    if (flag == false)
                        ViewBag.check = LoginResponseTypes.Blocked.ToString();//"User is Blocked for 3 failed attempts.";Blocked
                }
            }
            return View(login);
        }
    }
}