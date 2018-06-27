// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerController.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Transactions;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// PatientPreScheduling controller.
    /// </summary>
    [AllowAnonymous]
    public class PatientPreSchedulingController : Controller
    {
        private readonly IPatientInfoService _piService;
        private readonly ICountryService _cService;

        public PatientPreSchedulingController(IPatientInfoService piService, ICountryService cService)
        {
            _piService = piService;
            _cService = cService;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the PatientPreScheduling list
        /// </summary>
        /// <returns>action result with the partial view containing the PatientPreScheduling list object</returns>
        [HttpPost]
        public ActionResult BindPatientPreSchedulingList()
        {
            // Initialize the PatientPreScheduling BAL object
            using (var patientPreSchedulingBal = new PatientPreSchedulingBal())
            {
                // Get the facilities list
                var patientPreSchedulingList = patientPreSchedulingBal.GetPatientPreScheduling();

                // Pass the ActionResult with List of PatientPreSchedulingViewModel object to Partial View PatientPreSchedulingList
                return this.PartialView(PartialViews.PatientPreSchedulingList, patientPreSchedulingList);
            }
        }

        /// <summary>
        /// Delete the current PatientPreScheduling based on the PatientPreScheduling ID passed in the PatientPreSchedulingModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeletePatientPreScheduling(int id)
        {
            using (var bal = new PatientPreSchedulingBal())
            {
                // Get PatientPreScheduling model object by current PatientPreScheduling ID
                var currentPatientPreScheduling = bal.GetPatientPreSchedulingById(id);
                var userId = Helpers.GetLoggedInUserId();

                // Check If PatientPreScheduling model is not null
                if (currentPatientPreScheduling != null)
                {
                    currentPatientPreScheduling.IsActive = false;

                    // currentPatientPreScheduling.ModifiedBy = userId;
                    // currentPatientPreScheduling.ModifiedDate = DateTime.Now;

                    // Update Operation of current PatientPreScheduling
                    int result = bal.SavePatientPreScheduling(currentPatientPreScheduling);

                    // return deleted ID of current PatientPreScheduling as Json Result to the Ajax Call.
                    return this.Json(result);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return this.Json(null);
        }

        /// <summary>
        /// Get the details of the current PatientPreScheduling in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetPatientPreScheduling(int id)
        {
            using (var bal = new PatientPreSchedulingBal())
            {
                // Call the AddPatientPreScheduling Method to Add / Update current PatientPreScheduling
                var currentPatientPreScheduling = bal.GetPatientPreSchedulingById(id);

                // Pass the ActionResult with the current PatientPreSchedulingViewModel object as model to PartialView PatientPreSchedulingAddEdit
                return this.PartialView(PartialViews.PatientPreSchedulingAddEdit, currentPatientPreScheduling);
            }
        }

        /// <summary>
        /// Get the details of the PatientPreScheduling View in the Model PatientPreScheduling such as PatientPreSchedulingList, list of
        /// countries etc.
        /// </summary>
        /// <param name="CId">The c identifier.</param>
        /// <param name="FId">The f identifier.</param>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model PatientPreScheduling to be passed to View
        /// PatientPreScheduling
        /// </returns>
        public ActionResult Index(int? CId, int? FId, int? msg)
        {
            // Pass the View Model in ActionResult to View PatientPreScheduling
            var patientSchedularlinkBal = new PreSchedulingLinkBal();
            var patientSchedulingObj = patientSchedularlinkBal.GetPreSchedulingLink(
                Convert.ToInt32(CId),
                Convert.ToInt32(FId));
            if (patientSchedulingObj.Any())
            {
                ViewBag.check = msg;
                var objtoreturn = new Users()
                {
                    CorporateId = Convert.ToInt32(CId),
                    FacilityId = Convert.ToInt32(FId)
                };
                return View(objtoreturn);
            }
            else
            {
                return RedirectToAction("NotFound", "Error");
            }
        }

        /// <summary>
        /// Index_2s the specified c identifier.
        /// </summary>
        /// <param name="CId">The c identifier.</param>
        /// <param name="FId">The f identifier.</param>
        /// <returns></returns>
        public ActionResult Index_2(int? CId, int? FId)
        {
            // Initialize the PatientPreScheduling BAL object
            List<PatientPreSchedulingCustomModel> patientPreSchedulingList;
            using (var patientPreSchedulingBal = new PatientPreSchedulingBal())
            {
                patientPreSchedulingList = patientPreSchedulingBal.GetPatientPreScheduling();
            }

            // Intialize the View Model i.e. PatientPreSchedulingView which is binded to Main View Index.cshtml under PatientPreScheduling
            //var patientPreSchedulingView = new PatientPreSchedulingView
            //{
            //    PatientPreSchedulingList = patientPreSchedulingList,
            //    CurrentPatientPreScheduling = new PatientPreScheduling()
            //};

            // Pass the View Model in ActionResult to View PatientPreScheduling
            return View();
        }

        /// <summary>
        /// Reset the PatientPreScheduling View Model and pass it to PatientPreSchedulingAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetPatientPreSchedulingForm()
        {
            // Intialize the new object of PatientPreScheduling ViewModel
            var patientPreSchedulingViewModel = new PatientPreScheduling();

            // Pass the View Model as PatientPreSchedulingViewModel to PartialView PatientPreSchedulingAddEdit just to update the AddEdit partial view.
            return this.PartialView(PartialViews.PatientPreSchedulingAddEdit, patientPreSchedulingViewModel);
        }

        /// <summary>
        /// Add New or Update the PatientPreScheduling based on if we pass the PatientPreScheduling ID in the PatientPreSchedulingViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of PatientPreScheduling row
        /// </returns>
        public ActionResult SavePatientPreScheduling(PatientPreScheduling model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new PatientPreSchedulingBal())
                {
                    if (model.PatientPreSchedulingId > 0)
                    {
                        // model.ModifiedBy = userId;
                        // model.ModifiedDate = DateTime.Now;
                    }

                    // Call the AddPatientPreScheduling Method to Add / Update current PatientPreScheduling
                    newId = bal.SavePatientPreScheduling(model);
                }
            }

            return this.Json(newId);
        }

        /// <summary>
        /// Patients the login.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PatientLogin(PatientLoginDetailCustomModel model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.Email))
            {
                var flag = true;
                using (var pbal = new PatientLoginDetailBal())
                {
                    var currentPatient = pbal.GetPatientLoginDetailsByEmail(model.Email);
                    if (currentPatient != null)
                    {
                        var patientId = currentPatient.PatientId.HasValue ? currentPatient.PatientId.Value : 0;
                        var enPwd = EncryptDecrypt.Encrypt(model.Password).ToLower().Trim();
                        if (string.IsNullOrEmpty(currentPatient.Password))
                        {
                            ViewBag.check = (int)LoginResponseTypes.AccountNotActivated;
                            return RedirectToAction("Index", new { CId = model.CorporateId, FId = model.FacilityId, msg = (int)LoginResponseTypes.UserNotFoundInCorporate });
                        }

                        if (currentPatient.Password.ToLower().Trim().Equals(enPwd))
                        {
                            if (currentPatient.FailedLoginAttempts.HasValue
                                && currentPatient.FailedLoginAttempts.Value == 3)
                            {
                                var failedlogin = Convert.ToDateTime(currentPatient.LastInvalidLogin);
                                var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
                                if (timespan.TotalMinutes < 30)
                                {
                                    flag = false;
                                    ViewBag.check = (int)LoginResponseTypes.FailedAttemptsOver;
                                }
                            }

                            if (currentPatient.CorporateId != model.CorporateId)
                            {
                                ViewBag.check = (int)LoginResponseTypes.UserNotFoundInCorporate;
                                return RedirectToAction("Index", new { CId = model.CorporateId, FId = model.FacilityId, msg = (int)LoginResponseTypes.UserNotFoundInCorporate });
                            }

                            if (flag)
                            {
                                using (var bal = new LoginTrackingBal())
                                {
                                    var loginTrackingVm = new LoginTracking
                                    {
                                        ID = patientId,
                                        LoginTime =
                                                                      Helpers.GetInvariantCultureDateTime(),
                                        LoginUserType =
                                                                      (int)LoginTrackingTypes.UserLogin,
                                        FacilityId = currentPatient.FacilityId,
                                        CorporateId = currentPatient.CorporateId,
                                        IsDeleted = false,
                                        IPAddress = Helpers.GetUser_IP(),
                                        CreatedBy = patientId,
                                        CreatedDate =
                                                                      Helpers.GetInvariantCultureDateTime()
                                    };

                                    bal.AddUpdateLoginTrackingData(loginTrackingVm);
                                    pbal.UpdatePatientLoginFailedLog(
                                        patientId,
                                        0,
                                        Helpers.GetInvariantCultureDateTime());


                                    var objSession = Session[SessionNames.SessionClass.ToString()] != null
                                                         ? Session[SessionNames.SessionClass.ToString()] as SessionClass
                                                         : new SessionClass();

                                    objSession.FirstTimeLogin = bal.IsFirstTimeLoggedIn(
                                        patientId,
                                        (int)LoginTrackingTypes.PatientLogin);

                                    objSession.FacilityNumber = currentPatient.FacilityNumber;
                                    objSession.UserName = currentPatient.PatientName;
                                    objSession.UserId = patientId;
                                    objSession.SelectedCulture = CultureInfo.CurrentCulture.Name;
                                    objSession.LoginUserType = (int)LoginTrackingTypes.PatientLogin;
                                    objSession.UserIsAdmin = false;
                                    objSession.RoleId = 0;
                                    objSession.RoleName = "Patient Scheduler";

                                    using (var tBal = new TabsBal()) objSession.MenuSessionList = tBal.GetPatientTabsList();

                                    using (var mBal = new ModuleAccessBal())
                                    {
                                        Session[SessionNames.SessoionModuleAccess.ToString()] =
                                            mBal.GetModulesAccessList(
                                                currentPatient.CorporateId,
                                                currentPatient.FacilityId);
                                    }

                                    Session[SessionNames.SessionClass.ToString()] = objSession;
                                    return RedirectToAction("Index", "PatientSchedulerPortal", new { pId = patientId, fId = currentPatient.FacilityId, cId = currentPatient.CorporateId });
                                }
                            }
                        }
                        else
                        {
                            if (currentPatient.Password == null
                                || !currentPatient.Password.Equals(EncryptDecrypt.Encrypt(currentPatient.Password))) ViewBag.check = (int)LoginResponseTypes.Failed;
                            else if (currentPatient.IsDeleted != false) ViewBag.check = (int)LoginResponseTypes.IsDeleted;

                            else if (!string.IsNullOrEmpty(currentPatient.PatientName)
                                     && ViewBag.check == (int)LoginResponseTypes.Failed)
                            {
                                if (currentPatient.FailedLoginAttempts < 3
                                    || currentPatient.FailedLoginAttempts == null)
                                {
                                    var failedlogin = currentPatient.LastInvalidLogin.HasValue
                                                          ? currentPatient.LastInvalidLogin.Value
                                                          : Helpers.GetInvariantCultureDateTime();
                                    var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
                                    var failedattempts = timespan.TotalMinutes < 30
                                                             ? Convert.ToInt32(
                                                                 currentPatient.FailedLoginAttempts) + 1
                                                             : 1;
                                    using (var bal = new PatientLoginDetailBal())
                                    {
                                        bal.UpdatePatientLoginFailedLog(
                                            patientId,
                                            failedattempts,
                                            Helpers.GetInvariantCultureDateTime());
                                    }
                                }
                                else if (currentPatient.FailedLoginAttempts == 3)
                                {
                                    var failedlogin = currentPatient.LastInvalidLogin.HasValue
                                                          ? currentPatient.LastInvalidLogin.Value
                                                          : Helpers.GetInvariantCultureDateTime();
                                    var timespan = Helpers.GetInvariantCultureDateTime().Subtract(failedlogin);
                                    if (timespan.TotalMinutes < 30) flag = false;
                                }
                            }

                            if (flag == false) ViewBag.check = (int)LoginResponseTypes.Failed;
                        }
                    }
                    else
                    {
                        ViewBag.check = (int)LoginResponseTypes.Failed;
                    }
                }
            }
            else
            {
                ViewBag.check = (int)LoginResponseTypes.UnknownError;
            }
            return View("Index");
        }

        /// <summary>
        /// Gets the countries with code.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCountriesWithCode()
        { 
                var list = _cService.GetCountryWithCode();
                return Json(list);
           
        }

        /// <summary>
        /// Registers the custom patient.
        /// </summary>
        /// <param name="pinfo">The pinfo.</param>
        /// <returns></returns>
        public ActionResult RegisterCustomPatient(RegisterPatientCustomModel pinfo)
        {
            // return null;
            const string DefaultEmirate = "111-11-1111"; //"111-1111-1111111-1";
            var patientId = 0;
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            var pinfoObj = new PatientInfo()
            {
                CorporateId = pinfo.CorporateId,
                FacilityId = pinfo.FacilityId,
                CreatedBy = 9999,
                CreatedDate = currentDateTime,
                PersonEmiratesIDNumber = DefaultEmirate,
                IsDeleted = false,
                PersonBirthDate = Convert.ToDateTime(pinfo.PersonDateOfBirth),
                PersonLastName = pinfo.PersonFirstName,
                PersonFirstName = pinfo.PersonFirstName,
                PersonEmailAddress = pinfo.PersonEmailId,
            };

            // Check for duplicate Social Security Number, DOB and LastName
            var isExists = _piService.CheckIfEmiratesIdExists(
                pinfoObj.PersonEmiratesIDNumber,
                patientId,
                pinfoObj.PersonLastName,
                Convert.ToDateTime(pinfoObj.PersonBirthDate),
                Convert.ToInt32(pinfoObj.FacilityId));
            if (isExists)
            {
                return Json(new { patientId, status = "duplicate" }, JsonRequestBehavior.AllowGet);
            }

            // Check for duplicate Patient's Email
            if (!string.IsNullOrEmpty(pinfo.PersonEmailId))
            {
                isExists = _piService.CheckForDuplicateEmail(pinfo.PersonEmailId, patientId);
                if (isExists)
                {
                    return Json(new { patientId, status = "duplicateemail" }, JsonRequestBehavior.AllowGet);
                }
            }

            using (var trans = new TransactionScope())
            {
                try
                {
                    patientId = _piService.AddUpdatePatientInfo(pinfoObj);
                    if (patientId > 0)
                    {
                        var statusMessage = string.Empty;

                        // Save / Updates Patient's Phone Details
                        var phone = new PatientPhone
                        {
                            PatientID = patientId,
                            PhoneNo = pinfo.PersonPhoneNumber,
                            PhoneType = (int)PhoneType.MobilePhone,
                            IsPrimary = true,
                            IsdontContact = false,
                            IsDeleted = false,
                            CreatedDate = currentDateTime,
                            CreatedBy = 9999
                        };

                        var phoneId = SavePatientPhoneData(phone);
                        if (phoneId <= 0)
                        {
                            statusMessage = "phoneerror";
                        }

                        // Save / Updates Patient's Login Details
                        var personLoginDetails = new PatientLoginDetailCustomModel()
                        {
                            Email =
                                pinfo.PersonEmailId,
                            PatientId = patientId,
                            FacilityId =
                                pinfo.FacilityId,
                            CorporateId =
                                pinfo.CorporateId,
                            CreatedBy = 9999,
                            CreatedDate = currentDateTime,
                            PatientPortalAccess = true
                        };
                        var loginId = SavePatientSecuritySettings(personLoginDetails);
                        if (loginId <= 0)
                        {
                            statusMessage = "logindetailerror";
                        }

                        if (loginId > 0 && phoneId > 0)
                        {
                            trans.Complete();
                        }
                        else
                        {
                            return Json(new { patientId, status = statusMessage }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                catch (Exception)
                {
                    return Json(new { patientId, status = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { patientId, status = "success" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Saves the patient phone data.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        private int SavePatientPhoneData(PatientPhone model)
        {
            using (var patientPhoneBal = new PatientPhoneBal())
            {
                var newId = patientPhoneBal.SavePatientPhone(model);
                return newId;
            }
        }

        /// <summary>
        /// Saves the patient security settings.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        private int SavePatientSecuritySettings(PatientLoginDetailCustomModel vm)
        {
            if (vm != null)
            {
                using (var bal = new PatientLoginDetailBal())
                {
                    vm.TokenId = vm.DeleteVerificationToken
                        ? string.Empty
                        : CommonConfig.GeneratePasswordResetToken(14, false);

                    var isEmailSentBefore = !string.IsNullOrEmpty(vm.ExternalValue1) &&
                                            Convert.ToInt32(vm.ExternalValue1) == 1;

                    if (vm.PatientPortalAccess && !isEmailSentBefore)
                    {
                        // Generate the 8-Digit Code
                        vm.TokenId = CommonConfig.GeneratePasswordResetToken(14, false);
                        vm.CodeValue = CommonConfig.GenerateLoginCode(8, false);

                        var emailSentStatus = SendVerificationLinkForPatientLoginPortal(
                            Convert.ToInt32(vm.PatientId),
                            vm.Email,
                            vm.TokenId,
                            vm.CodeValue);

                        // Is Email Sent Now
                        vm.ExternalValue1 = emailSentStatus ? "1" : "0";
                    }

                    var updatedId = bal.SavePatientLoginDetails(vm);
                    return updatedId;
                }
            }
            return 0;
        }

        /// <summary>
        /// Sends the verification link for patient login portal.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="email">The email.</param>
        /// <param name="verificationTokenId">The verification token identifier.</param>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        private bool SendVerificationLinkForPatientLoginPortal(int patientId, string email, string verificationTokenId, string code)
        {
            var msgBody = ResourceKeyValues.GetFileText("patientportalemailVerification");
            PatientInfoCustomModel patientVm;
            using (var bal = new PatientLoginDetailBal())
            {
                patientVm = bal.GetPatientDetailsByPatientId(Convert.ToInt32(patientId));
            }

            if (!string.IsNullOrEmpty(msgBody) && patientVm != null)
            {
                msgBody = msgBody.Replace("{Patient}", patientVm.PatientName)
                    .Replace("{Facility-Name}", patientVm.FacilityName).Replace("{CodeValue}", code);
            }
            var emailInfo = new EmailInfo
            {
                VerificationTokenId = verificationTokenId,
                PatientId = patientId,
                Email = email,
                Subject = ResourceKeyValues.GetKeyValue("verificationemailsubject"),
                VerificationLink = "/Home/Verify",
                MessageBody = msgBody
            };
            return true;
            //var status = MailHelper.SendMail(emailInfo);
            //return status;
        }
        #endregion
    }
}
