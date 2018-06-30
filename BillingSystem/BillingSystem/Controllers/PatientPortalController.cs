using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using Image = System.Drawing.Image;

namespace BillingSystem.Controllers
{
    public class PatientPortalController : BaseController
    {
        private readonly IPatientInfoService _piService;
        private readonly IEncounterService _eService;
        private readonly IBillHeaderService _bhService;

        public PatientPortalController(IPatientInfoService piService, IEncounterService eService, IBillHeaderService bhService)
        {
            _piService = piService;
            _eService = eService;
            _bhService = bhService;
        }

        /// <summary>
        /// Indexes the specified p identifier.
        /// </summary>
        /// <param name="pId">The p identifier.</param>
        /// <returns></returns>
        public ActionResult Index(int? pId)
        {
            if (pId == null || Convert.ToInt32(pId) == 0)
            {
                var loginType = Helpers.GetLoginUserType();
                if (loginType == (int)LoginTrackingTypes.PatientLogin)
                    pId = Helpers.GetLoggedInUserId();
                else
                    return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch,
                         new { messageId = Convert.ToInt32(MessageType.ViewPatientPortal) });
            }

            var patientId = Convert.ToInt32(pId);
            using (var orderBal = new OpenOrderService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var enList = orderBal.GetEncountersListByPatientId(patientId);
                using (var medicalRecordbal = new MedicalRecordService()) //Updated by Shashank on Oct 28, 2014
                {
                    using (var medicalnotesbal = new MedicalNotesService()) //Updated by Shashank on Oct 28, 2014
                    {
                        using (var medicalVitals = new MedicalVitalService())
                        {
                            using (var diagnosisBal = new DiagnosisService(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
                            {
                                var currentEncounterId = (enList != null && enList.Any() &&
                                                          enList.First().EncounterEndTime == null)
                                    ? enList.First().EncounterID
                                    : 0;

                                var medicalrecords = medicalRecordbal.GetMedicalRecord();
                                var medicalvitals = medicalVitals.GetCustomMedicalVitalsByPidEncounterId(patientId,
                                    Convert.ToInt32(MedicalRecordType.Vitals), currentEncounterId);

                                //added by Shashank on Dec 01 2014
                                var patientSummaryNotes =
                                    medicalnotesbal.GetMedicalNotesByPatientIdEncounterId(patientId, currentEncounterId);
                                var allergiesList = medicalRecordbal.GetAlergyRecords(patientId,
                                    Convert.ToInt32(MedicalRecordType.Allergies));

                                var orderStatus = OrderStatus.Open.ToString();
                                var openOrdersList = orderBal.GetPhysicianOrders(currentEncounterId, orderStatus);
                                var primarydiagnosisId = 0;
                                var diagnosisList = diagnosisBal.GetDiagnosisList(patientId, currentEncounterId);
                                if (diagnosisList.Any())
                                {
                                    var diagnosisCustomModel =
                                        diagnosisList.SingleOrDefault(
                                            x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary));
                                    if (diagnosisCustomModel != null)
                                    {
                                        primarydiagnosisId = diagnosisCustomModel.DiagnosisID;
                                    }
                                }
                                var summaryView = new PatientPortalView
                                {
                                    PatientInfo = orderBal.GetPatientDetailsByPatientId(patientId),
                                    OpenOrdersList = openOrdersList,
                                    EncountersList = enList,
                                    MedicalVitalList = medicalvitals,
                                    PatientSummaryNotes = patientSummaryNotes,
                                    ClosedOrdersList =
                                        orderBal.GetPhysicianOrders(currentEncounterId, OrderStatus.Closed.ToString()),
                                    AlergyList = allergiesList,
                                    DiagnosisList = diagnosisList,
                                    PatientId = patientId
                                };
                                return View(summaryView);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the patient information.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientInfo(int? patientId)
        {
            var id = (patientId != null && Convert.ToInt32(patientId) > 0) ? Convert.ToInt32(patientId) : 0;
            var patientInfoModel = new PatientInfoView { PatientId = id };


            var currentPatient = id > 0 ? _piService.GetPatientInfoCustomModelById(id) : new PatientInfoCustomModel { PatientInfo = new PatientInfo() };

            if (id == 0 && currentPatient.PatientInfo != null)
            {
                currentPatient.PatientInfo.PersonMedicalRecordNumber = string.Empty;
                currentPatient.PatientInfo.FacilityId = Helpers.GetDefaultFacilityId();
            }
            patientInfoModel.CurrentPatient = currentPatient;
            if (currentPatient != null && currentPatient.PatientInfo != null && !string.IsNullOrEmpty(currentPatient.PatientInfo.PersonVIP))
            {
                bool vip;
                if (bool.TryParse(currentPatient.PatientInfo.PersonVIP, out vip))
                    currentPatient.PatientInfo.PersonVIP = string.Empty;
                currentPatient.PatientIsVIP = true;
            }

            //Patient Insurances
            using (var pInsurance = new PatientInsuranceService())
            {
                //Changes by Amit Jain on 14102014
                var customModel = pInsurance.GetPatientInsurance(id);
                patientInfoModel.Insurance = customModel ?? new PatientInsuranceCustomModel();
            }

            using (var phBal = new PatientPhoneService())
            {
                patientInfoModel.CurrentPhone = new PatientPhone
                {
                    PatientPhoneId = phBal.GetPhoneId(id)
                };
                patientInfoModel.PatientPhoneView = new PhonesView()
                {
                    CurrentPhone = new PatientPhone(),
                    Phonelst = phBal.GetPatientPhoneList(id)
                };
                //patientInfoModel.Phonelst = phBal.GetPatientPhone(id);
            }

            using (var phBal = new PatientAddressRelationService())
            {
                patientInfoModel.PatientAddressRealtionList = phBal.GetPatientAddressRelation(id);
                patientInfoModel.CurrentPatientAddressRelation = new PatientAddressRelation();
            }

            using (var phBal = new DocumentsTemplatesService())
            {
                patientInfoModel.PatientDocumentsView = new DocumentsView()
                {
                    Attachments =
                         phBal.GetPatientDocuments(id)
                             .Where(p =>
                                 p.DocumentName.ToLower() != "profilepicture" && p.AssociatedType == 1 &&
                                 (p.DocumentTypeID == 1 || p.DocumentTypeID == 2 || p.DocumentTypeID == 3)),
                    CurrentAttachment = new DocumentsTemplates()
                };
            }

            using (var bal = new PatientLoginDetailService())
            {
                patientInfoModel.PatientLoginDetail = new PatientLoginDetailCustomModel { IsDeleted = false };
                var vm2 = bal.GetPatientLoginDetailByPatientId(id);
                if (vm2 != null)
                    patientInfoModel.PatientLoginDetail = vm2;
            }

            patientInfoModel.EncounterOpen = _eService.GetEncounterOpenStatus(id);

            return PartialView(PartialViews.PatientInfoView, patientInfoModel);
        }

        /// <summary>
        /// Gets the patient billing information.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientBillingInfo(int? patientId)
        {
            var billDetailsView = new BillDetailsView
            {
                PatientInfo = new PatientInfoCustomModel(),
                BillHeaderList = new List<BillHeaderCustomModel>(),
                BillActivityList = new List<BillDetailCustomModel>(),
                EncounterId = 0,
                QueryStringId = 0,
                QueryStringTypeId = 0
            };
            if (patientId != null && Convert.ToInt32(patientId) != 0)
            { 
                    var encountersList = _bhService.GetAllBillHeaderListByPatientId(Convert.ToInt32(patientId));

                    //Bill Details ViewModel to be binded to UI
                    billDetailsView = new BillDetailsView
                    {
                        PatientInfo = new PatientInfoCustomModel(),
                        BillHeaderList = encountersList,
                        BillActivityList = new List<BillDetailCustomModel>(),
                        EncounterId = 0,
                        QueryStringId = 0,
                        QueryStringTypeId = 0
                    };
               
            }

            //Pass the View Model in ActionResult to partial View BillHeader
            return PartialView(PartialViews.PatientBillingInfo, billDetailsView);
        }

        /// <summary>
        /// Gets the patient results.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientResults(int? patientId)
        {
            using (var bal = new OrderActivityService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByPatientId(patientId)
                        .Where(x => x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                                    &&
                                    (x.OrderActivityStatus != 0 &&
                                     x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open))).OrderBy(x => x.ExecutedDate).ToList();
                return PartialView(PartialViews.LabClosedActivtiesList, list);
            }
        }

        /// <summary>
        /// Updates the patient information.
        /// </summary>
        /// <param name="cm">The cm.</param>
        /// <returns></returns>
        public ActionResult UpdatePatientInfo(PatientInfoView cm)
        {
            var patientId = 0;
            if (cm != null && cm.CurrentPatient != null && cm.CurrentPatient.PatientInfo != null)
            {
                var patientVm = cm.CurrentPatient.PatientInfo;
                patientId = cm.PatientId;

                if (patientId <= 0)
                    return Json(new { patientId, status = "error" }, JsonRequestBehavior.AllowGet);

                //Get the Details from logged in user global object
                var userId = Helpers.GetLoggedInUserId();
                var currentDateTime = Helpers.GetInvariantCultureDateTime();
                var facilityId = Helpers.GetDefaultFacilityId();

                //Check if FacilityViewModel
                if (patientVm != null)
                {
                    patientVm.PatientID = patientId;

                    //Check for duplicate Social Security Number, DOB and LastName
                    var isExists = _piService.CheckIfEmiratesIdExists(patientVm.PersonEmiratesIDNumber, patientId,
                        patientVm.PersonLastName, Convert.ToDateTime(patientVm.PersonBirthDate), facilityId);
                    if (isExists)
                        return Json(new { patientId = 0, status = "duplicate" }, JsonRequestBehavior.AllowGet);


                    //Check for duplicate Health Care Number (Member ID)
                    isExists = _piService.CheckForDuplicateHealthCareNumber(cm.Insurance.PersonHealthCareNumber, patientId, cm.Insurance.InsuranceCompanyId, cm.Insurance.InsurancePlanId, cm.Insurance.InsurancePolicyId);
                    if (isExists)
                        return Json(new { patientId, status = "duplicatememberid" }, JsonRequestBehavior.AllowGet);

                    //Check for duplicate Patient's Email
                    isExists = _piService.CheckForDuplicateEmail(cm.PatientLoginDetail.Email, patientId);
                    if (isExists)
                        return Json(new { patientId, status = "duplicateemail" }, JsonRequestBehavior.AllowGet);


                    using (var trans = new TransactionScope())
                    {
                        try
                        {
                            var previouspatientInfoData = _piService.GetPatientInfoById(patientId);
                            var isObjectsSame = CompareerValue(previouspatientInfoData, patientVm);
                            if (!isObjectsSame)
                            {
                                return Json(null, JsonRequestBehavior.AllowGet);
                            }
                            //patientId = bal.AddUpdatePatientInfo(patientVm);
                            if (patientId > 0)
                            {
                                var statusMessage = string.Empty;

                                //Save / Updates Profile Image
                                int imageId;
                                if (Session[SessionEnum.TempProfileFile.ToString()] != null)
                                {
                                    imageId = SaveProfileImage(patientId);
                                    if (imageId < 0)
                                        statusMessage = "imageerror";
                                }
                                else imageId = 1;

                                //Save / Updates Patient's Insurance Details
                                if (cm.Insurance != null)
                                    cm.Insurance.PatientID = patientId;
                                var insId = SavePatientInsuranceData(cm.Insurance);
                                if (insId <= 0)
                                    statusMessage = "insuranceerror";

                                //Save / Updates Patient's Phone Details
                                if (cm.CurrentPhone != null)
                                {
                                    cm.CurrentPhone.PatientID = patientId;
                                    cm.CurrentPhone.PhoneNo = cm.CurrentPatient.PatientInfo.PersonContactNumber;
                                    cm.CurrentPhone.IsPrimary = true;
                                    cm.CurrentPhone.IsdontContact = false;
                                    cm.CurrentPhone.IsDeleted = false;
                                    cm.CurrentPhone.ModifiedDate = currentDateTime;
                                    cm.CurrentPhone.ModifiedBy = userId;
                                    cm.CurrentPhone.PhoneType = (int)PhoneType.MobilePhone;
                                }
                                else
                                {
                                    cm.CurrentPhone = new PatientPhone
                                    {
                                        PatientID = patientId,
                                        PhoneNo = cm.CurrentPatient.PatientInfo.PersonContactNumber,
                                        PhoneType = (int)PhoneType.MobilePhone,
                                        IsPrimary = true,
                                        IsdontContact = false,
                                        IsDeleted = false,
                                        CreatedDate = currentDateTime,
                                        CreatedBy = userId
                                    };
                                }
                                var phoneId = SavePatientPhoneData(cm.CurrentPhone);
                                if (phoneId <= 0)
                                    statusMessage = "phoneerror";


                                //Save / Updates Patient's Login Details
                                if (cm.PatientLoginDetail != null)
                                    cm.PatientLoginDetail.PatientId = patientId;
                                var loginId = SavePatientSecuritySettings(cm.PatientLoginDetail);
                                if (loginId <= 0)
                                    statusMessage = "logindetailerror";

                                if (imageId > 0 && insId > 0 && phoneId > 0 && loginId > 0)
                                    trans.Complete();
                                else
                                    return Json(new { patientId, status = statusMessage }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch
                        {
                            return Json(new { patientId, status = "error" }, JsonRequestBehavior.AllowGet);
                            //throw ex;
                        }
                    }
                    if (patientId > 0)
                        return Json(new { patientId, status = "success" },
                                            JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { patientId, status = "error" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the profile image.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        private int SaveProfileImage(int patientId)
        {
            var newId = 0;

            //Get the Details from logged in user global object
            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();

            //Associated Type ID
            const int associatedType = (int)AttachmentType.ProfilePicture;
            var profileImage = Convert.ToString(AttachmentType.ProfilePicture);

            using (var docBal = new DocumentsTemplatesService())
            {
                if (patientId <= 0) return newId;

                //Save Patient Info Image
                var imgModel = PatientInfoProfileImage(patientId);
                if (imgModel != null)
                {
                    var newDoc =
                        docBal.GetDocumentByTypeAndPatientId(Convert.ToInt32(AttachmentType.ProfilePicture),
                            patientId) ?? new DocumentsTemplates
                            {
                                CreatedBy = userId,
                                CreatedDate = currentDateTime
                            };

                    newDoc.AssociatedID = patientId;
                    newDoc.AssociatedType = associatedType;
                    newDoc.ModifiedBy = userId;
                    newDoc.ModifiedDate = currentDateTime;
                    newDoc.IsRequired = true;
                    newDoc.IsDeleted = false;
                    newDoc.IsTemplate = false;
                    newDoc.DocumentTypeID = associatedType;
                    newDoc.DocumentName = profileImage;
                    newDoc.FileName = imgModel.FileName;
                    newDoc.FilePath = imgModel.ImageUrl;
                    newDoc.CorporateID = corporateId;
                    newDoc.FacilityID = facilityId;
                    newDoc.PatientID = patientId;

                    //Add / Update document details of current Patient
                    newId = docBal.AddUpdateDocumentTempate(newDoc);
                }
                else if (Session[SessionEnum.PatientDocName.ToString()] != null)
                {
                    var documentType = docBal.GetPatientDocuments(patientId);
                    var firstOrDefault =
                        documentType.Where(d => d.DocumentName.ToLower().Contains("profile image"))
                            .OrderByDescending(d1 => d1.DocumentsTemplatesID)
                            .FirstOrDefault();

                    if (firstOrDefault == null)
                    {
                        var documentsTemplates = new DocumentsTemplates
                        {
                            AssociatedID = patientId,
                            AssociatedType = associatedType,
                            FileName = Session[SessionEnum.PatientDocName.ToString()].ToString(),
                            FilePath = Session[SessionEnum.PatientDoc.ToString()].ToString(),
                            CreatedBy = userId,
                            CreatedDate = currentDateTime,
                            IsDeleted = false,
                            IsRequired = false,
                            IsTemplate = true,
                            DocumentTypeID = 1,
                            DocumentName = profileImage,
                            CorporateID = corporateId,
                            FacilityID = facilityId,
                            PatientID = patientId
                        };
                        newId = docBal.AddUpdateDocumentTempate(documentsTemplates);
                    }
                    else
                    {
                        var documenttoupdate = firstOrDefault;
                        documenttoupdate.FileName = Session[SessionEnum.PatientDocName.ToString()].ToString();
                        documenttoupdate.FilePath = Session[SessionEnum.PatientDoc.ToString()].ToString();
                        documenttoupdate.ModifiedBy = userId;
                        documenttoupdate.CreatedDate = currentDateTime;
                        documenttoupdate.DocumentsTemplatesID = firstOrDefault.DocumentsTemplatesID;
                        documenttoupdate.CorporateID = corporateId;
                        documenttoupdate.FacilityID = facilityId;
                        documenttoupdate.PatientID = patientId;
                        newId = docBal.AddUpdateDocumentTempate(documenttoupdate);
                    }
                }
            }
            return newId;
        }

        /// <summary>
        /// Patients the information profile image.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        private ImageInfoModel PatientInfoProfileImage(int patientId)
        {
            ImageInfoModel imgModel = null;

            if (Session[SessionEnum.TempProfileFile.ToString()] != null)
            {
                var uploadFile = (HttpPostedFileBase)Session[SessionEnum.TempProfileFile.ToString()];
                imgModel = SaveImageLocally(uploadFile, patientId);

                if (imgModel != null)
                    HttpContextSessionWrapperExtension.ContentStream = null;
            }

            // Clear temp file sesssion after filling required details
            Session[SessionEnum.TempProfileFile.ToString()] = null;
            return imgModel;
        }

        /// <summary>
        /// Saves the image locally.
        /// </summary>
        /// <param name="uploadFile">The upload file.</param>
        /// <param name="pId">The p identifier.</param>
        /// <returns></returns>
        private ImageInfoModel SaveImageLocally(HttpPostedFileBase uploadFile, int pId)
        {
            byte[] fileData = null;
            if (HttpContextSessionWrapperExtension.ContentStream != null)
                fileData = HttpContextSessionWrapperExtension.ContentStream;

            if (fileData != null)
            {
                var fi = new FileInfo(uploadFile.FileName);
                const string virtualPath = "Content\\Images\\ProfileImages\\";
                var serverPath = Server.MapPath("~");
                var imagesPath = string.Format("{0}{1}{2}", serverPath, virtualPath, pId);
                var isExists = Directory.Exists(imagesPath);

                if (!isExists)
                    Directory.CreateDirectory(imagesPath);
                else
                {
                    var fileCount = Directory.GetFiles(imagesPath).Any()
                        ? Directory.GetFiles(imagesPath).Count() + 1
                        : 1;

                    if (fileCount > 0)
                    {
                        var dirInfo = new DirectoryInfo(imagesPath);
                        foreach (var item in dirInfo.GetFiles())
                            item.Delete();
                    }
                }

                var saveImagePath = string.Format("{0}\\" + fi.Name, imagesPath);
                Stream stream = new MemoryStream(fileData);
                var img1 = Image.FromStream(stream);
                Session["TempRealImage"] = img1;
                img1.Save(saveImagePath);
                var image = new ImageInfoModel
                {
                    FileName = fi.Name,
                    ImageUrl = string.Format(CommonConfig.ProfilePicVirtualPath, pId, fi.Name)
                };
                return image;
            }
            return null;
        }

        /// <summary>
        /// Saves the patient insurance data.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        private int SavePatientInsuranceData(PatientInsuranceCustomModel vm)
        {
            if (vm != null)
            {
                using (var bal = new PatientInsuranceService())
                {
                    var userId = Helpers.GetLoggedInUserId();
                    var currentDateTime = Helpers.GetInvariantCultureDateTime();
                    //model.IsPrimary = true;
                    //if (model.PatientInsuraceID > 0)
                    //{
                    //    model.ModifiedBy = userId;
                    //    model.ModifiedDate = currentDateTime;
                    //}
                    //else
                    //{
                    //    model.CreatedBy = userId;
                    //    model.CreatedDate = currentDateTime;
                    //}

                    var model = new PatientInsurance
                    {
                        CreatedBy = userId,
                        CreatedDate = currentDateTime,
                        ModifiedBy = userId,
                        ModifiedDate = currentDateTime,
                        InsuranceCompanyId = vm.InsuranceCompanyId,
                        PatientInsuraceID = vm.PatientInsuraceID,
                        PatientID = vm.PatientID,
                        IsPrimary = true,
                        Startdate = vm.Startdate,
                        Expirydate = vm.Expirydate,
                        DeletedBy = vm.DeletedBy,
                        DeletedDate = vm.DeletedDate,
                        IsDeleted = false,
                        IsActive = true,
                        InsurancePlanId = vm.InsurancePlanId,
                        InsurancePolicyId = vm.InsurancePlanId,
                        PersonHealthCareNumber = vm.PersonHealthCareNumber
                    };

                    var result = bal.SavePatientInsurance(model);
                    return result.Count > 0 ? result[0] : 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// Saves the patient phone data.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        private int SavePatientPhoneData(PatientPhone model)
        {
            using (var patientPhoneBal = new PatientPhoneService())
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
                using (var bal = new PatientLoginDetailService())
                {
                    var userId = Helpers.GetLoggedInUserId();
                    var currentDateTime = Helpers.GetInvariantCultureDateTime();
                    if (vm.Id == 0)
                    {
                        vm.CreatedBy = userId;
                        vm.CreatedDate = currentDateTime;
                    }
                    else
                    {
                        vm.ModifiedBy = userId;
                        vm.ModifiedDate = currentDateTime;
                    }

                    vm.TokenId = vm.DeleteVerificationToken
                        ? string.Empty
                        : CommonConfig.GeneratePasswordResetToken(14, false);

                    var isEmailSentBefore = !string.IsNullOrEmpty(vm.ExternalValue1) &&
                                            Convert.ToInt32(vm.ExternalValue1) == 1;

                    if (vm.PatientPortalAccess && !isEmailSentBefore)
                    {
                        //Generate the 8-Digit Code
                        vm.TokenId = CommonConfig.GeneratePasswordResetToken(14, false);
                        vm.CodeValue = CommonConfig.GenerateLoginCode(8, false);

                        var emailSentStatus = SendVerificationLinkForPatientLoginPortal(Convert.ToInt32(vm.PatientId),
                            vm.Email,
                            vm.TokenId, vm.CodeValue);

                        //Is Email Sent Now
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
            using (var bal = new PatientLoginDetailService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
                patientVm = bal.GetPatientDetailsByPatientId(Convert.ToInt32(patientId));

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
            //var status = MailHelper.SendMail(emailInfo);
            //return status;
            return true;
        }

        private bool CompareerValue<T>(T object1, T object2)
        {
            //Get the type of the object
            Type type = typeof(T);

            //return false if any of the object is false
            if (object1 == null || object2 == null)
                return false;

            //Loop through each properties inside class and get values for the property from both the objects and compare
            foreach (System.Reflection.PropertyInfo property in type.GetProperties())
            {
                if (property.Name != "ExtensionData")
                {
                    var object1Value = string.Empty;
                    var object2Value = string.Empty;
                    if (type.GetProperty(property.Name).GetValue(object1, null) != null)
                        object1Value = type.GetProperty(property.Name).GetValue(object1, null).ToString();
                    if (type.GetProperty(property.Name).GetValue(object2, null) != null)
                        object2Value = type.GetProperty(property.Name).GetValue(object2, null).ToString();
                    if (object1Value.Trim() != object2Value.Trim())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the patient schedular.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientSchedular()
        {
            return PartialView(PartialViews.PatientSchedularView, null);
        }
    }
}