using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using WebGrease.Css.Extensions;
using System.Threading.Tasks;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class PatientInfoController : BaseController
    {
        private readonly IPatientInfoService _service;
        private readonly IEncounterService _eService;
        private readonly ICountryService _cService;
        private readonly IPatientPhoneService _ppService;
        private readonly IGlobalCodeService _gService;
        private readonly IInsuranceCompanyService _icService;
        private readonly IDocumentsTemplatesService _docService;
        private readonly IPatientInsuranceService _pinService;
        private readonly IInsurancePlansService _ipService;
        private readonly IInsurancePolicesService _ipsService;
        private readonly IPatientAddressRelationService _parService;
        private readonly IFacilityService _fService;
        private readonly IPatientLoginDetailService _pldService;
        private readonly IRoleTabsService _rtService;

        const string partialViewPath = "../PatientInfo/";

        public PatientInfoController(IPatientInfoService service, IEncounterService eService, ICountryService cService, IPatientPhoneService ppService, IGlobalCodeService gService, IInsuranceCompanyService icService, IDocumentsTemplatesService docService, IPatientInsuranceService pinService, IInsurancePlansService ipService, IInsurancePolicesService ipsService, IPatientAddressRelationService parService, IFacilityService fService, IPatientLoginDetailService pldService, IRoleTabsService rtService)
        {
            _service = service;
            _eService = eService;
            _cService = cService;
            _ppService = ppService;
            _gService = gService;
            _icService = icService;
            _docService = docService;
            _pinService = pinService;
            _ipService = ipService;
            _ipsService = ipsService;
            _parService = parService;
            _fService = fService;
            _pldService = pldService;
            _rtService = rtService;
        }

        /// <summary>
        ///     Patients the information.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult PatientInfo(int? patientId)
        {
            ResetPatientDocSessions();

            var id = (patientId != null && Convert.ToInt32(patientId) > 0) ? Convert.ToInt32(patientId) : 0;
            var v = new PatientInfoView { PatientId = id };
            PatientInfoViewData vm = new PatientInfoViewData();

            if (id > 0)
            {
                vm = _service.GetPatientInfoOnLoad(id, 0);
            }

            var now = Helpers.GetInvariantCultureDateTime();
            v.CurrentPatient = vm.PatientInfo ?? new PatientInfoCustomModel();
            v.Insurance = vm.PatientInsurance ?? new PatientInsuranceCustomModel { StartDate2 = new DateTime(now.Year, now.Month, 1), EndDate2 = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1) };
            v.PatientLoginDetail = vm.PatientLoginInfo ?? new PatientLoginDetailCustomModel();
            v.CurrentPhone = vm.PatientPhone ?? new PatientPhone();
            v.EncounterOpen = vm.EncounterOpen;

            return View(v);
        }

        /// <summary>
        /// Registers the patient.
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterPatient()
        {
            ResetPatientDocSessions();
            var patientVm = new PatientInfoCustomModel
            {
                PatientInfo = new PatientInfo
                {
                    PersonMedicalRecordNumber = string.Empty,
                    FacilityId = Helpers.GetDefaultFacilityId(),
                    CorporateId = Helpers.GetSysAdminCorporateID(),
                }
            };

            var now = Helpers.GetInvariantCultureDateTime();
            var startDate = new DateTime(now.Year, 1, 1);

            var vmData = new RegisterPatientView
            {
                CurrentPatient = patientVm,
                Insurance =
                    new PatientInsuranceCustomModel
                    {
                        IsActive = true,
                        IsDeleted = false,
                        Startdate = startDate,
                        Expirydate = startDate.AddMonths(12).AddDays(-1)
                    },
                CurrentPhone = new PatientPhone(),
                CurrentPatientAddressRelation = new PatientAddressRelation(),
                DocumentsAttachment = new DocumentsTemplates(),
                CityList = new List<City>(),
                StatesList = new List<State>(),
                CountryList = new List<Country>()

            };
            return View(vmData);
        }

        /// <summary>
        ///     Gets the patient relations.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetPatientRelations()
        {
            var list = _gService.GetGlobalCodesByCategoryValue(Convert.ToString((int)GlobalCodeCategoryValue.PatientRelationTypes)).OrderBy(x => x.GlobalCodeID).ToList();
            return Json(list);
        }

        /// <summary>
        ///     Checks the active encounter.
        /// </summary>
        /// <param name="patientid">The patientid.</param>
        /// <param name="encountertype">The encountertype.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckActiveEncounter(string patientid, string encountertype)
        {
            var patientidint = Convert.ToInt32(patientid);
            var encountertypeint = Convert.ToInt32(encountertype);
            var isActiveEncounter = _service.CheckPatientActiveEncounter(patientidint, encountertypeint);
            return Json(isActiveEncounter);
        }

        /// <summary>
        ///     Files the upload.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase files)
        {
            if (files == null)
            {
                ModelState.AddModelError("File", @"Please Upload Your file");
            }
            else if (files.ContentLength > 0)
            {
                var maxContentLength = CommonConfig.MaxFileSizeInMB;
                var allowedFileExtensions = new[] { ".jpg", ".gif", ".png", ".pdf" };

                if (!allowedFileExtensions.Contains(files.FileName.Substring(files.FileName.LastIndexOf('.'))))
                    ModelState.AddModelError("File", @"Please file of type: " + string.Join(", ", allowedFileExtensions));

                else if (files.ContentLength > maxContentLength)
                {
                    ModelState.AddModelError("File",
                        @"Your file is too large, maximum allowed size is: " + maxContentLength + @" MB");
                }
                else
                {
                    //TO:DO
                    var fileName = Path.GetFileName(files.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
                    files.SaveAs(path);
                    ModelState.Clear();
                    ViewBag.Message = "File uploaded successfully";
                }
            }
            return Json(null);
        }

        /// <summary>
        ///     Gets the patient document.
        /// </summary>
        /// <param name="documentid">The documentid.</param>
        /// <returns></returns>
        public ActionResult GetPatientDocument(string documentid)
        {
            var documentidInt = Convert.ToInt32(documentid);
            var objDocumentTemplateData = _docService.GetDocumentById(documentidInt);
            if (objDocumentTemplateData != null)
            {
                return PartialView(PartialViews.PatientDocumentsAddEdit, objDocumentTemplateData);
            }
            return null;
        }

        /// <summary>
        ///     Gets the patient documents.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientDocuments(string patientId)
        {
            var patientIdint = Convert.ToInt32(patientId);
            var objDocumentTemplateData = _docService.GetPatientDocuments(patientIdint)
                    .Where(
                        _ =>
                            _.DocumentName.ToLower() != "profilepicture" && _.AssociatedType == 1 &&
                            (_.DocumentTypeID == 1 || _.DocumentTypeID == 2 || _.DocumentTypeID == 3));
            objDocumentTemplateData.ForEach(x => x.FileName = x.FileName.Split('.')[0].ToString());
            return PartialView(PartialViews.PatientDocumentsList, objDocumentTemplateData);
        }

        /// <summary>
        ///     Checks the state of the encounter.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="patientID">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult CheckEncounterState(string state, int patientID)
        {
            var isNew = false;
            var encounterCustom = _eService.GetEncounterDetailByPatientId(patientID);
            var actualMessageId =
                GetMessageId(encounterCustom.EncounterState != null
                    ? encounterCustom.EncounterState.ToLower()
                    : string.Empty);
            var selectedMessageId = GetMessageId(state.ToLower());
            var message = encounterCustom.EncounterState != null
                ? GetMessage(actualMessageId, selectedMessageId)
                : string.Empty;
            var encounterPatientType = encounterCustom.EncounterPatientType;
            //.. Added this to check the Encounter State.
            var encounterId = encounterCustom.EncounterPatientType != null ? encounterCustom.EncounterID : 0;
            if (string.IsNullOrEmpty(encounterCustom.EncounterState) &&
                (selectedMessageId == 1 || selectedMessageId == 2))
                isNew = true;
            var objJson =
                new
                {
                    isRecordExist = (encounterCustom != null && encounterCustom.EncounterState != null),
                    messageId = selectedMessageId,
                    message,
                    isNew,
                    encPatientType = encounterPatientType,
                    encId = encounterId
                };
            return Json(objJson);
        }



        /// <summary>
        ///     Checks the patient encounter.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult CheckPatientEncounter(string patientId)
        {
            var encounter = _eService.GetEncounterStateByPatientId(Convert.ToInt32(patientId));
            return Json(encounter);
        }

        /// <summary>
        ///     Checks if birth date exists.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <returns></returns>
        public ActionResult CheckIfBirthDateExists(string birthDate)
        {
            var status = _service.CheckIfBirthDateExists(DateTime.Parse(birthDate));
            return Json(status);

        }

        /// <summary>
        ///     Checks if passport exists.
        /// </summary>
        /// <param name="passport">The passport.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public JsonResult CheckIfPassportExists(string passport, string patientId)
        {
            var status = _service.CheckIfPassportExists(passport, Convert.ToInt32(patientId));
            return Json(status);

        }

        /// <summary>
        /// Checks if emirates identifier exists.
        /// </summary>
        /// <param name="emiratesId">The emirates identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="birthDate">The birth date.</param>
        /// <returns></returns>
        public ActionResult CheckIfEmiratesIDExists(string emiratesId, string patientId, string lastName,
            string birthDate)
        {
            var status = _service.CheckIfEmiratesIdExists(emiratesId, Convert.ToInt32(patientId), lastName,
                    Convert.ToDateTime(birthDate), Helpers.GetDefaultFacilityId());
            return Json(status);

        }

        //function to get patient info when selected self
        /// <summary>
        ///     Gets the patient by identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetPatientById(int Id)
        {
            var info = _service.GetPatientInfoById(Id);
            return Json(info);
        }

        //function to get auto generate medical number
        /// <summary>
        ///     Gets the automatic generate medical number.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAutoGenerateMedicalNumber()
        {
            var medicalNumber = _service.GetMaxMedicalRecordNumber();
            return Json(medicalNumber);
        }

        //function to check if medical number already exist
        /// <summary>
        ///     Checks the medical number exist.
        /// </summary>
        /// <param name="newMedicalRecordNumber">The new medical record number.</param>
        /// <returns></returns>
        public ActionResult CheckMedicalNumberExist(string newMedicalRecordNumber)
        {
            var isExists = _service.CheckIfMedicalRecordNumber(newMedicalRecordNumber);
            return Json(isExists);
        }

        /// <summary>
        ///     Method to get the autorization for the current selected Encounter.
        /// </summary>
        /// <param name="encId">Encounter ID</param>
        /// <returns>
        ///     Partial view
        /// </returns>
        public async Task<ActionResult> GetAuthorization(string encId)
        {
            var eId = Convert.ToInt32(encId);
            if (eId > 0)
            {
                var viewData = await _eService.GetAuthorizationViewDataAsync(eId);
                return PartialView(PartialViews.GetAuthorizationPartialView, viewData);
            }
            return null;
        }

        /// <summary>
        ///     Gets the active encounter identifier.
        /// </summary>
        /// <param name="PatientID">The patient identifier.</param>
        /// <returns></returns>
        public async Task<ActionResult> GetActiveEncounterId(int PatientID)
        {
            var currentEncounter = _eService.GetActiveEncounterByPateintId(PatientID);
            if (currentEncounter == null)
            {
                return Json(0); // : currentEncounter.EncounterID);
            }
            return await GetAuthorization(currentEncounter.EncounterID.ToString());
        }
        /// <summary>
        /// Gets the active encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public async Task<ActionResult> GetAuthorizationPopup(int encounterId)
        {
            return await GetAuthorization(Convert.ToString(encounterId));
        }

        /// <summary>
        /// Gets the XML active encounter identifier.
        /// </summary>
        /// <param name="PatientID">The patient identifier.</param>
        /// <returns></returns>
        public async Task<ActionResult> GetXMLActiveEncounterId(int PatientID)
        {
            var currentEncounter = _eService.GetXMLActiveEncounterByPateintId(PatientID);
            if (currentEncounter == null)
            {
                return Json(0); // : currentEncounter.EncounterID);
            }
            return await GetAuthorization(currentEncounter.EncounterID.ToString());
        }

        /// <summary>
        ///     Gets the patient custom detail by identifier.
        /// </summary>
        /// <param name="PatientID">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientCustomDetailById(int PatientID)
        {
            var info = _service.GetPatientInfoById(PatientID);
            var patientInsuranceobj = _pinService.GetPatientInsurance(PatientID);
            var patientPhoneObj = _ppService.GetPatientPersonalPhoneByPateintId(PatientID);

            var customModel = new CommonModel
            {
                PersonEmiratesIDNumber = info.PersonEmiratesIDNumber != null ? info.PersonEmiratesIDNumber : "NA",
                PersonPassportNumber = info.PersonPassportNumber != null ? info.PersonPassportNumber : "NA",
                ContactMobilePhone = patientPhoneObj != null
                    ? patientPhoneObj.PhoneNo
                    : "NA",

                PatientCompanyName = patientInsuranceobj != null
                    ? _icService.GetCompanyNameById(patientInsuranceobj.InsuranceCompanyId) ?? "NA"
                    : "NA",


                PatientCompanyClaimPhoneNumber = patientInsuranceobj != null
                        ? _icService.GetInsuranceCompanyById(patientInsuranceobj.InsuranceCompanyId) != null
                            ? _icService.GetInsuranceCompanyById(patientInsuranceobj.InsuranceCompanyId)
                                .InsuranceCompanyClaimsContactPhone != null ?
                                _icService.GetInsuranceCompanyById(patientInsuranceobj.InsuranceCompanyId)
                                .InsuranceCompanyClaimsContactPhone : "NA"
                            : "NA"
                        : "NA"




            };


            return Json(customModel);
        }

        #region ImageLoad

        ///// <summary>
        ///// Load image
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        public ActionResult ImageLoad(int? id)
        {
            var b = Session[SessionEnum.TempOtherDoc.ToString()] == null ? (byte[])HttpContextSessionWrapperExtension.ContentStream
                : (byte[])HttpContextSessionWrapperExtension.ContentStreamDoc;
            var length = (int)HttpContextSessionWrapperExtension.ContentLength;
            var type = (string)HttpContextSessionWrapperExtension.ContentType;
            HttpContextSessionWrapperExtension.CroppedContentType = type;
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = type;
            Response.BinaryWrite(b);
            Response.Flush();
            Response.End();
            return Content("");
        }

        #endregion

        #region Patient Info Screen

        /// <summary>
        ///     Deletes the patient information.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult DeletePatientInfo(int patientId)
        {
            var currentPatientInfo = _service.GetPatientInfoById(patientId);
            if (currentPatientInfo != null)
            {
                var result = _service.AddUpdatePatientInfo(currentPatientInfo);

                //return deleted ID of current facility as Json Result to the Ajax Call.
                return Json(result);
            }
            return null;
        }

        /// <summary>
        /// Patients the information profile image.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        private ImageInfoModel PatientInfoProfileImage(int patientId)
        {
            if (Session[SessionEnum.TempProfileFile.ToString()] != null)
            {
                var uploadFile = (HttpPostedFileBase)Session[SessionEnum.TempProfileFile.ToString()];
                byte[] fileData = null;

                if (HttpContextSessionWrapperExtension.ContentStream != null)
                    fileData = HttpContextSessionWrapperExtension.ContentStream;

                if (fileData != null)
                {
                    var dPath = CommonConfig.ImageUploads;
                    var serverPath = Server.MapPath("~");
                    var fullDPath = $"{serverPath}{dPath}";
                    //const string virtualPath = "Content\\Images\\ProfileImages\\";
                    //var imagesPath = string.Format("{0}{1}{2}", serverPath, virtualPath, patientId);
                    var isExists = Directory.Exists(fullDPath);

                    if (!isExists)
                        Directory.CreateDirectory(fullDPath);


                    //else
                    //{
                    //    var fileCount = Directory.GetFiles(imagesPath).Any()
                    //        ? Directory.GetFiles(imagesPath).Count() + 1
                    //        : 1;

                    //    if (fileCount > 0)
                    //    {
                    //        var dirInfo = new DirectoryInfo(imagesPath);
                    //        foreach (var item in dirInfo.GetFiles())
                    //            item.Delete();
                    //    }
                    //}

                    var fi = new FileInfo(uploadFile.FileName);


                    //var saveImagePath = string.Format("{0}\\" + fi.Name, imagesPath);

                    var stream = new MemoryStream(fileData);
                    var img1 = Image.FromStream(stream);
                    Session[SessionEnum.ProfileImage.ToString()] = img1;

                    var ext = fi.Extension;
                    var file = Convert.ToString((new Random()).Next(1000)) + ext;
                    var filePath = $"{fullDPath}{file}";
                    var isFileExists = System.IO.File.Exists(filePath);

                    while (isFileExists)
                    {
                        file = Convert.ToString((new Random()).Next(1000)) + ext;
                        filePath = $"{fullDPath}{file}";
                        isFileExists = System.IO.File.Exists(filePath);
                    }

                    img1.Save(filePath);

                    if (Session[SessionEnum.OldDoc.ToString()] != null)
                    {
                        var oldImage = Convert.ToString(Session[SessionEnum.OldDoc.ToString()]);
                        if (System.IO.File.Exists(oldImage))
                            System.IO.File.Delete(oldImage);
                    }


                    var image = new ImageInfoModel
                    {
                        FileName = file,
                        ImageUrl = $"{CommonConfig.ImageUploads}{file}",//string.Format(CommonConfig.ProfilePicVirtualPath, patientId, fi.Name)
                    };

                    return image;
                }
            }
            return null;
        }

        /// <summary>
        /// Saves the patient information.
        /// </summary>
        /// <param name="patientInfoModel">The patient information model.</param>
        /// <returns></returns>
        public ActionResult SavePatientInfoDetail(PatientInfo patientInfoModel)
        {
            //Initialize the newId variable
            var updatedPatientId = -1;
            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            //Associated Type ID
            var associatedType = Convert.ToInt32(AttachmentType.ProfilePicture);

            var profileImage = AttachmentType.ProfilePicture.ToString();

            //Check if FacilityViewModel
            if (patientInfoModel != null)
            {
                ImageInfoModel imgModel = null;
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                if (patientInfoModel.PatientID > 0)
                {
                    patientInfoModel.ModifiedBy = userId;
                    patientInfoModel.ModifiedDate = currentDateTime;
                }
                else
                {
                    patientInfoModel.CreatedBy = userId;
                    patientInfoModel.CreatedDate = currentDateTime;
                }
                patientInfoModel.CorporateId = corporateId;
                patientInfoModel.FacilityId = facilityId;
                patientInfoModel.IsDeleted = false;

                updatedPatientId = _service.AddUpdatePatientInfo(patientInfoModel);

                if (updatedPatientId > 0)
                {
                    //Save Patient Info Image
                    if (Session[SessionEnum.TempProfileFile.ToString()] != null)
                        imgModel = PatientInfoProfileImage(updatedPatientId);


                    if (imgModel != null)
                    {
                        var newDoc =
                            _docService.GetDocumentByTypeAndPatientId(Convert.ToInt32(AttachmentType.ProfilePicture),
                                updatedPatientId) ?? new DocumentsTemplates
                                {
                                    CreatedBy = userId,
                                    CreatedDate = currentDateTime
                                };

                        newDoc.AssociatedID = updatedPatientId;
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
                        newDoc.PatientID = updatedPatientId;

                        //Add / Update document details of current Patient
                        _docService.AddUpdateDocumentTempate(newDoc);
                    }
                    else if (Session[SessionEnum.PatientDocName.ToString()] != null)
                    {
                        var documentType = _docService.GetPatientDocuments(patientInfoModel.PatientID);
                        var firstOrDefault =
                            documentType.Where(d => d.DocumentName.ToLower().Contains("profile image"))
                                .OrderByDescending(d1 => d1.DocumentsTemplatesID)
                                .FirstOrDefault();

                        if (firstOrDefault == null)
                        {
                            var documentsTemplates = new DocumentsTemplates
                            {
                                AssociatedID = patientInfoModel.PatientID,
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
                                PatientID = updatedPatientId
                            };
                            _docService.AddUpdateDocumentTempate(documentsTemplates);
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
                            documenttoupdate.PatientID = updatedPatientId;
                            _docService.AddUpdateDocumentTempate(documenttoupdate);
                        }

                    }
                }
            }
            return Json(updatedPatientId);
        }

        #endregion

        #region Phone Tab

        /// <summary>
        ///     Saves the patient phone.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SavePatientPhone(PatientPhone model)
        {
            if (model != null)
            {
                var userId = Helpers.GetLoggedInUserId();
                var currentDateTime = Helpers.GetInvariantCultureDateTime();

                if (model.PatientPhoneId > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDateTime;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDateTime;
                }
                var result = _ppService.SavePatientPhone(model);
                var list = Enumerable.Empty<PatientPhoneCustomModel>();
                var pView = string.Empty;

                if (result > 0)
                {
                    list = _ppService.GetPatientPhoneList(model.PatientID);
                    var viewPath = $"{partialViewPath}{PartialViews.PhoneGrid}";
                    pView = RenderPartialViewToStringBase(viewPath, list);
                }
                var jsonData = new { pView, result };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            return Json(new { pView = string.Empty, result = -1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Gets the patient phone by identifier.
        /// </summary>
        /// <param name="patientphoneId">The patientphone identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientPhoneById(int patientphoneId)
        {
            var model = _ppService.GetPatientPhoneById(patientphoneId);
            if (model != null)
                return Json(model, JsonRequestBehavior.AllowGet);

            return Json(null, JsonRequestBehavior.AllowGet); ;
        }

        /// <summary>
        ///     Deletes the patient phone.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeletePatientPhone(int id)
        {
            var model = _ppService.GetPatientPhoneById(id);
            if (model != null)
            {
                model.IsDeleted = true;
                model.DeletedBy = Helpers.GetLoggedInUserId();
                model.DeletedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current Patient's Phone
                var result = _ppService.DeletePatientPhone(model);

                //return deleted ID of current Patient's Phone as Json Result to the Ajax Call.
                return PartialView(PartialViews.PhoneGrid, result);
            }
            return null;
        }

        ///// <summary>
        ///// Saves the patient phone data.
        ///// </summary>
        ///// <param name="model">The model.</param>
        ///// <returns></returns>
        //private int SavePatientPhoneData(PatientPhone model)
        //{
        //    using (var patientPhoneBal = new PatientPhoneBal())
        //    {
        //        var newId = patientPhoneBal.SavePatientPhone(model);
        //        return newId;
        //    }
        //}

        #endregion

        #region Address and Contacts Tab

        /// <summary>
        ///     Saves the patient address relation.
        /// </summary>
        /// <param name="model">The patient address model.</param>
        /// <returns></returns>
        public ActionResult SavePatientAddressRelation(PatientAddressRelation model)
        {
            if (model != null)
            {
                var userId = Helpers.GetLoggedInUserId();
                var currentDateTime = _fService.GetInvariantCultureDateTime(Helpers.GetDefaultFacilityId());

                if (model.PatientAddressRelationID > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDateTime;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDateTime;
                }
                var newId = _parService.AddPatientAddressRelation(model);
                return Json(newId);
            }
            return Json(null);
        }

        /// <summary>
        ///     Gets the patient address information.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientAddressInfo(int patientId)
        {
            var objPatientAddressRelatioData = _parService.GetPatientAddressRelation(patientId);
            if (objPatientAddressRelatioData != null)
            {
                return PartialView(PartialViews.AddressRelationGrid, objPatientAddressRelatioData);
            }
            return null;
        }

        /// <summary>
        ///     Gets the patient address by identifier.
        /// </summary>
        /// <param name="patientRelationId">The patient relation identifier.</param>
        /// <returns></returns>
        public JsonResult GetPatientAddressById(int patientRelationId)
        {
            var model = _parService.GetPatientRelationAddressById(patientRelationId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Reset the Facility View Model and pass it to FacilityAddEdit Partial View.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult ResetPatientAddressForm(string patientId)
        {
            //Intialize the new object of Facility ViewModel
            var patientAddressRelationViewModel = new PatientAddressRelation();

            //Pass the View Model as FacilityViewModel to PartialView FacilityAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.AddressAddEdit, patientAddressRelationViewModel);
        }

        /// <summary>
        ///     Deletes the patient address relation.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeletePatientAddressRelation(int Id)
        {
            var currentPatientInfo =
                _parService.GetPatientRelationAddressById(Id);
            if (currentPatientInfo != null)
            {
                currentPatientInfo.IsDeleted = true;
                currentPatientInfo.DeletedBy = Helpers.GetLoggedInUserId();
                currentPatientInfo.DeletedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current facility
                var result = _parService.AddPatientAddressRelation(currentPatientInfo);

                //return deleted ID of current facility as Json Result to the Ajax Call.
                return Json(result);
            }
            return null;
        }

        /// <summary>
        /// Gets the address partial view.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public PartialViewResult GetAddressPartialView(int patientId)
        {
            //Patient Addresses
            var patientInfoModel = new PatientInfoView
            {
                PatientId = patientId,
                CurrentPatientAddressRelation = new PatientAddressRelation(),
                PatientAddressRealtionList = patientId > 0
                    ? _parService.GetPatientAddressRelation(patientId)
                    : new List<PatientAddressRelationCustomModel>()
            };
            return PartialView(PartialViews.AddressPartialView, patientInfoModel);

        }

        #endregion

        #region Insurance Tab

        /// <summary>
        ///     Saves the patient insurance.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public JsonResult SavePatientInsurance2(PatientInsurance model)
        {
            if (model != null)
            {
                var userId = Helpers.GetLoggedInUserId();
                var currentDateTime = _fService.GetInvariantCultureDateTime(Helpers.GetDefaultFacilityId());
                if (model.PatientInsuraceID > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDateTime;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDateTime;
                }

                var result = _pinService.SavePatientInsurance(model);
                if (result.Count > 0)
                {
                    var jsonResult = new
                    {
                        InsuranceId1 = result[0],
                        InsuranceId2 = result.Count > 1 ? result[1] : 0
                    };
                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
                return Json(new { InsuranceId1 = 0, InsuranceId2 = 0 }, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }

        /// <summary>
        ///     Gets the insurance plans by company identifier.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns></returns>
        public JsonResult GetInsurancePlansByCompanyId(string companyId)
        {
            var planlist = _ipService.GetInsurancePlansByCompanyId(Convert.ToInt32(companyId), CurrentDateTime);
            var list = new List<DropdownListData>();
            list.AddRange(planlist.Select(item => new DropdownListData
            {
                Text = item.PlanName,
                Value = Convert.ToString(item.InsurancePlanId),
                ExternalValue1 =
                    item.PlanEndDate != null
                        ? (string.Format("{0}/{1}/{2}", ((DateTime)item.PlanEndDate).Month,
                            ((DateTime)item.PlanEndDate).Day, ((DateTime)item.PlanEndDate).Year))
                        : string.Empty
            }));
            return Json(list);
        }

        /// <summary>
        ///     Gets the insurance polices by plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public JsonResult GetInsurancePolicesByPlanId(string planId)
        {
            var policiesList = _ipsService.GetInsurancePolicesByPlanId(Convert.ToInt32(planId), CurrentDateTime);
            var list = new List<DropdownListData>();
            list.AddRange(policiesList.Select(item => new DropdownListData
            {
                Text = item.PolicyName,
                Value = Convert.ToString(item.InsurancePolicyId),
                //ExternalValue1 = item.PolicyEndDate != null ? ((DateTime)item.PolicyEndDate).ToShortDateString() : string.Empty
                ExternalValue1 =
                    item.PolicyEndDate != null
                        ? (string.Format("{0}/{1}/{2}", ((DateTime)item.PolicyEndDate).Month,
                            ((DateTime)item.PolicyEndDate).Day, ((DateTime)item.PolicyEndDate).Year))
                        : string.Empty
            }));
            return Json(list);
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
                var userId = Helpers.GetLoggedInUserId();
                var currentDateTime = _fService.GetInvariantCultureDateTime(Helpers.GetDefaultFacilityId());
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
                    InsurancePolicyId = vm.InsurancePolicyId,
                    PersonHealthCareNumber = string.IsNullOrEmpty(vm.PersonHealthCareNumber) ? "0" : vm.PersonHealthCareNumber
                };
                var result = _pinService.SavePatientInsurance(model);

                return result.Count > 0 ? result[0] : 0;
            }
            return 0;
        }
        #endregion

        #region Patient Documents Tab
        /// <summary>
        ///     Saves the patient document.
        /// </summary>
        /// <param name="model">The documents templates.</param>
        /// <returns></returns>
        public ActionResult SavePatientDocument(DocumentsTemplates model)
        {


            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var userId = Helpers.GetLoggedInUserId();
            var cDate = _fService.GetInvariantCultureDateTime(facilityId);




            if (Session[SessionEnum.TempOtherDoc.ToString()] != null && Session[SessionEnum.TempOtherDoc.ToString()] != null)
            {
                var otherDoc = SaveOtherDocument();
                var documentName = _gService.GetNameByGlobalCodeValueAndCategoryValue("1103", Convert.ToString(Session[SessionEnum.DocTypeId.ToString()]));

                model.AssociatedType = (int)DocAssociatedType.PatientDemographicDocument;
                model.DocumentName = documentName;
                model.FileName = otherDoc.FileName;
                model.FilePath = otherDoc.ImageUrl;
                model.DocumentTypeID = Session[SessionEnum.DocTypeId.ToString()] != null ? Convert.ToInt32(Session[SessionEnum.DocTypeId.ToString()]) : 0;
                model.CorporateID = corporateId;
                model.FacilityID = facilityId;

                var m = _docService.GetDocumentByType(model.PatientID.Value, model.DocumentTypeID);
                var oldFilePath = string.Empty;
                if (m == null)
                {
                    m = model;
                    m.CreatedBy = userId;
                    m.CreatedDate = cDate;
                }
                else
                {
                    model.DocumentsTemplatesID = m.DocumentsTemplatesID;
                    m = model;
                    m.ModifiedBy = userId;
                    m.ModifiedDate = cDate;

                    oldFilePath = $"{Server.MapPath("~")}{m.FilePath}";
                }

                int? result = -1;
                var list = _docService.SavePatientDocuments(m, out result);

                if (result >= 0 && !string.IsNullOrEmpty(oldFilePath) && System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);


                if (list.Count > 0)
                    return PartialView(PartialViews.PatientDocumentsList, list);
            }
            return PartialView(PartialViews.PatientDocumentsList, new List<DocumentsTemplates>());
        }

        /// <summary>
        ///     Ajaxes the submit document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AjaxSubmitDoc()
        {
            if (Request.Files != null && Request.Files.Count > 0 && Request["PatientId_Document"] != null && !string.IsNullOrEmpty(Request["PatientId_Document"]))
            {
                var uploadedFile = Request.Files[0];
                Session[SessionEnum.OtherDoc.ToString()] = uploadedFile;

                var patientId = Convert.ToInt32(Request["PatientId_Document"]);
                if (uploadedFile != null)
                {
                    HttpContextSessionWrapperExtension.ContentLength = null;
                    HttpContextSessionWrapperExtension.ContentType = null;

                    var b = new byte[uploadedFile.ContentLength];
                    uploadedFile.InputStream.Read(b, 0, uploadedFile.ContentLength);
                    Stream stream = new MemoryStream(b);
                    HttpContextSessionWrapperExtension.ContentStreamDoc = b;

                    var virtualPath = CommonConfig.PatientDocumentsFilePath;
                    var serverPath = Server.MapPath(virtualPath);

                    var imagesDirectoryPath = string.Format(serverPath, patientId);
                    var isExists = Directory.Exists(imagesDirectoryPath);
                    if (!isExists)
                        Directory.CreateDirectory(imagesDirectoryPath);


                    var saveImagePath = string.Format("{0}\\{1}", imagesDirectoryPath, Path.GetFileName(uploadedFile.FileName));

                    var img = SaveImage(stream);
                    img.Save(saveImagePath);
                    Session[SessionEnum.PatientDoc.ToString()] = string.Format(virtualPath, patientId) + "\\" + Path.GetFileName(uploadedFile.FileName);
                    Session[SessionEnum.PatientDocName.ToString()] = Path.GetFileName(uploadedFile.FileName);
                    Session[SessionEnum.DocTypeId.ToString()] = Convert.ToInt32(Request["OtherDocumentTypeId"]);
                    return Content(uploadedFile.ContentType + ";" + uploadedFile.ContentLength);
                }
            }
            return Content("");
        }

        /// <summary>
        /// Gets the type of the patient document.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPatientDocumentType()
        {
            var list = _gService.GetGlobalCodesByCategoryValue(Convert.ToString((int)GlobalCodeCategoryValue.PatientDocumentTypes)).OrderBy(x => x.GlobalCodeID).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the file permanently.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public async Task<PartialViewResult> DeleteFilePermanently(int id, int patientId)
        {
            IEnumerable<DocumentsTemplates> list = null;
            using (var transcope = new TransactionScope())
            {
                if (id > 0 && patientId > 0)
                {
                    var model = _docService.GetDocumentById(id);
                    if (model != null)
                    {
                        //Update Operation of current facility
                        var deleteId = _docService.DeleteDocument(id);
                        if (deleteId > 0)
                        {
                            var fileName = Path.GetFileName(model.FilePath);
                            var filePath = Server.MapPath(fileName);
                            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);

                            list = await _docService.GetPatientDocumentsList(patientId);
                            transcope.Complete();
                        }
                    }
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return PartialView(PartialViews.PatientDocumentsList, list);
        }
        #endregion

        #region Patient Login Detail

        /// <summary>
        /// Gets the patient login detail partial view.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public PartialViewResult GetPatientLoginDetailPartialView(int patientId)
        {
            var vm = new PatientLoginDetailCustomModel
            {
                IsDeleted = false,
            };
            var vm2 = _pldService.GetPatientLoginDetailByPatientId(patientId);
            if (vm2 != null)
                vm = vm2;

            return PartialView(PartialViews.PatientLoginDetail, vm);
        }

        /// <summary>
        /// Saves the patient login details.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        public async Task<JsonResult> SavePatientLoginDetails(PatientLoginDetailCustomModel vm)
        {
            var message = string.Empty;
            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = _fService.GetInvariantCultureDateTime(Helpers.GetDefaultFacilityId());
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

            vm.TokenId = vm.DeleteVerificationToken ? string.Empty : CommonConfig.GeneratePasswordResetToken(14, false);

            var isEmailSentBefore = !string.IsNullOrEmpty(vm.ExternalValue1) &&
                                    Convert.ToInt32(vm.ExternalValue1) == 1;
            if (vm.PatientPortalAccess && !isEmailSentBefore)
            {
                //Generate the 8-Digit Code
                vm.TokenId = CommonConfig.GeneratePasswordResetToken(14, false);
                vm.CodeValue = CommonConfig.GenerateLoginCode(8, false);

                var emailSentStatus = await SendVerificationLinkForPatientLoginPortal(Convert.ToInt32(vm.PatientId)
                    , vm.Email, vm.TokenId, vm.CodeValue, string.Empty, string.Empty);

                //Is Email Sent Now
                vm.ExternalValue1 = emailSentStatus ? "1" : "0";
                message = emailSentStatus
                    ? ResourceKeyValues.GetKeyValue("verificationemailsuccess")
                    : ResourceKeyValues.GetKeyValue("verificationemailfailuremessage");
            }

            var updatedId = _pldService.SavePatientLoginDetails(vm);
            if (updatedId <= 0)
                message = ResourceKeyValues.GetKeyValue("msgrecordsnotsaved");

            var jsonStatus = new { message, updatedId, vm.ExternalValue1 };
            return Json(jsonStatus, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        public JsonResult ChangePassword(int patientId, string newPassword)
        {
            var updatedId = 0;

            var vm = _pldService.GetPatientLoginDetailByPatientId(patientId);
            if (vm != null)
            {
                vm.Password = EncryptDecrypt.Encrypt(newPassword).ToLower().Trim();
                updatedId = _pldService.SavePatientLoginDetails(vm);
            }
            return Json(updatedId, JsonRequestBehavior.AllowGet);
        }



        #endregion

        /// <summary>
        /// Gets the encounters ListView.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public PartialViewResult GetEncountersListView(int patientId)
        {
            List<EncounterCustomModel> vmData = _eService.GetEncountersByPatientId(patientId);

            if (vmData != null && vmData.Any())
            {
                var roleId = Helpers.GetDefaultRoleId();
                var encountersFirstItem = vmData.First();
                encountersFirstItem.EhrViewAccessible = _rtService.CheckIfTabNameAccessibleToGivenRole("EHR",
                    ControllerAccess.Summary.ToString(), ActionNameAccess.PatientSummary.ToString(),
                    Convert.ToInt32(roleId));
            }
            return PartialView(PartialViews.PatientEncountersPartialView, vmData);
        }

        /// <summary>
        /// Gets the patient attachments partial view.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public async Task<PartialViewResult> GetPatientAttachmentsPartialView(int patientId)
        {
            var vm = new DocumentsView();
            vm.Attachments = await _docService.GetPatientDocumentsList(patientId);
            vm.CurrentAttachment = new DocumentsTemplates();
            return PartialView(PartialViews.PatientAttachmentsPartialView, vm);
        }

        /// <summary>
        /// Gets the patient phones partial view.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public PartialViewResult GetPatientPhonesPartialView(int patientId)
        {
            var vm = new PhonesView();
            if (patientId > 0)
                vm.CurrentPhone = _ppService.GetPatientPersonalPhoneByPateintId(patientId);

            if (vm.CurrentPhone == null)
                vm.CurrentPhone = new PatientPhone();
            vm.Phonelst = _ppService.GetPatientPhoneList(patientId);
            return PartialView(PartialViews.PatientPhonePartialView, vm);
        }



        #region Sorting Methods

        /// <summary>
        /// Gets the patient phones by sort.
        /// </summary>
        /// <addedby>
        /// Krishna on 18082015
        /// </addedby>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public PartialViewResult GetPatientPhonesBySort(int patientId)
        {
            var phonelst = _ppService.GetPatientPhoneList(patientId);
            return PartialView(PartialViews.PhoneGrid, phonelst);
        }

        /// <summary>
        /// Gets the patient attachments partial view1.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public async Task<PartialViewResult> GetPatientAttachmentsPartialView1(int patientId)
        {
            var attachmentsData = await _docService.GetPatientDocumentsList(patientId);
            return PartialView(PartialViews.AttachmentsGrid, attachmentsData);
        }

        #endregion

        /// <summary>
        /// Method is used to get the patient name
        /// </summary>
        /// <param name="patientName">Name of the patient.</param>
        /// <returns></returns>
        public ActionResult GetPatientInfoByPatientName(string patientName, long facilityId)
        {
            int corporateId = Convert.ToInt32(Helpers.GetSysAdminCorporateID().ToString());
            var list = _service.GetPatientInfoByPatientName(patientName, corporateId, facilityId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNextPatientId()
        {
            var nextPId = _service.GetNextPatientId();
            return Json(nextPId);

        }

        public ActionResult GetPatientInsuranceInfo(int patinetId)
        {
            var list = _pinService.GetPatientInsuranceView(patinetId);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMotherName(int patientId)
        {
            var mName = string.Empty;

            var motherId = _service.GetPersonMotherNameById(patientId);
            if (!string.IsNullOrEmpty(motherId))
            {
                mName = _service.GetPatientNameById(Convert.ToInt32(motherId));
            }
            return Json(mName);

        }

        public JsonResult GetPatientDataOnLoad(int patientId)
        {
            IEnumerable<CountryCustomModel> countryData;
            var insList = new List<SelectListItem>();
            var defaultCountry = Helpers.GetDefaultCountryCode;
            var mName = string.Empty;

            //Country Data 
            countryData = _cService.GetCountryWithCode().OrderBy(x => x.CountryName);


            var result = _icService.GetInsuranceCompanies(true, Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
            if (result.Count > 0)
            {
                insList.AddRange(result.Select(item => new SelectListItem
                {
                    Text = item.InsuranceCompanyName,
                    Value = Convert.ToString(item.InsuranceCompanyId)
                }));
            }

            //Get Mother Name of the Patient if any
            var motherId = _service.GetPersonMotherNameById(patientId);
            if (!string.IsNullOrEmpty(motherId))
                mName = _service.GetPatientNameById(Convert.ToInt32(motherId));

            var jsonData = new { countryData, defaultCountry, insList, mName };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates the patient information.
        /// </summary>
        /// <param name="cm">The patient information model.</param>
        /// <returns></returns>
        public async Task<ActionResult> UpdatePatientInfo(PatientInfoView cm)
        {
            cm.CurrentPatient.PersonAge = cm.CurrentPatient.PatientInfo != null && cm.CurrentPatient.PatientInfo.PersonAge.HasValue
                ? Convert.ToInt32(cm.CurrentPatient.PatientInfo.PersonAge) : 0;

            cm.CurrentPatient.PatientInfo.PersonContactNumber = cm.CurrentPhone.PhoneNo;
            var result = await SavePatientInfo(cm.PatientId, cm.CurrentPatient, cm.Insurance, null, cm.PatientLoginDetail.Email);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> RegisterNewPatient(RegisterPatientView cm)
        {
            //Procedure Call to Save PatientInfo / Insurance / ContactNumber 
            var result = await SavePatientInfo(0, cm.CurrentPatient, cm.Insurance, cm.CurrentPatientAddressRelation, cm.PatientLoginDetail.Email, false, true);// bal.AddUpdatePatientInfoNew(patientVm, cm.Insurance, cm.CurrentPhone);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientDocumentsOnChange()
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                var uploadedFile = Request.Files[0];
                HttpContextSessionWrapperExtension.ContentLength = null;
                HttpContextSessionWrapperExtension.ContentType = null;

                Session[SessionEnum.TempOtherDoc.ToString()] = uploadedFile;

                if (Request["OtherDocumentTypeId"] != null)
                    Session[SessionEnum.DocTypeId.ToString()] = Convert.ToInt32(Request["OtherDocumentTypeId"]);

                if (Request["hf_OtherDocCurrentSource"] != null)
                    Session[SessionEnum.OtherOldDoc.ToString()] = Convert.ToInt32(Request["hf_OtherDocCurrentSource"]);

                var b = new byte[uploadedFile.ContentLength];
                uploadedFile.InputStream.Read(b, 0, uploadedFile.ContentLength);
                Stream stream = new MemoryStream(b);
                HttpContextSessionWrapperExtension.ContentStreamDoc = b;

                return Content(uploadedFile.ContentType + ";" + uploadedFile.ContentLength);
            }
            return Content("");
        }




        /// <summary>
        /// Sends the verification link for patient login portal.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="email">The email.</param>
        /// <param name="verificationTokenId">The verification token identifier.</param>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        private async Task<bool> SendVerificationLinkForPatientLoginPortal(int patientId, string email, string verificationTokenId
            , string code, string patientName, string facilityName)
        {
            var msgBody = ResourceKeyValues.GetFileText("patientportalemailVerification");
            PatientInfoCustomModel patientVm;
            patientVm = _service.GetPatientDetailsByPatientId(Convert.ToInt32(patientId));

            if (!string.IsNullOrEmpty(msgBody) && patientVm != null)
            {
                msgBody = msgBody.Replace("{Patient}", patientName)
                    .Replace("{Facility-Name}", facilityName).Replace("{CodeValue}", code);
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
            var status = await MailHelper.SendEmailAsync(emailInfo);
            return status;
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
            const int associatedType = (int)DocAssociatedType.PatientDemographicDocument;
            const int docType = (int)DocumentTemplateTypes.ProfileImage;
            var profileImage = Convert.ToString(DocumentTemplateTypes.ProfileImage);

            if (patientId <= 0) return newId;

            //Save Patient Info Image
            var imgModel = PatientInfoProfileImage(patientId);
            if (imgModel != null)
            {
                var newDoc =
                    _docService.GetDocumentByTypeAndPatientId(Convert.ToInt32(AttachmentType.ProfilePicture),
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
                newDoc.DocumentTypeID = docType;
                newDoc.DocumentName = profileImage;
                newDoc.FileName = imgModel.FileName;
                newDoc.FilePath = imgModel.ImageUrl;
                newDoc.CorporateID = corporateId;
                newDoc.FacilityID = facilityId;
                newDoc.PatientID = patientId;

                //Add / Update document details of current Patient
                newId = _docService.AddUpdateDocumentTempate(newDoc);
            }
            else if (Session[SessionEnum.PatientDocName.ToString()] != null)
            {
                var documentType = _docService.GetPatientDocuments(patientId);
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
                    newId = _docService.AddUpdateDocumentTempate(documentsTemplates);
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
                    newId = _docService.AddUpdateDocumentTempate(documenttoupdate);
                }
            }
            return newId;
        }

        /// <summary>
        /// Saves the patient security settings.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        private async Task<int> SavePatientSecuritySettings(PatientLoginDetailCustomModel vm)
        {
            if (vm != null)
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

                    var emailSentStatus = await SendVerificationLinkForPatientLoginPortal(Convert.ToInt32(vm.PatientId),
                        vm.Email, vm.TokenId, vm.CodeValue, string.Empty, string.Empty);

                    //Is Email Sent Now
                    vm.ExternalValue1 = emailSentStatus ? "1" : "0";
                }

                var updatedId = _pldService.SavePatientLoginDetails(vm);
                return updatedId;
            }
            return 0;
        }

        private ImageInfoModel SaveOtherDocument()
        {
            if (Session[SessionEnum.TempOtherDoc.ToString()] != null)
            {
                var uploadFile = (HttpPostedFileBase)Session[SessionEnum.TempOtherDoc.ToString()];
                byte[] fileData = null;

                if (HttpContextSessionWrapperExtension.ContentStreamDoc != null)
                    fileData = HttpContextSessionWrapperExtension.ContentStreamDoc;

                if (fileData != null)
                {
                    var dPath = CommonConfig.ImageUploads;
                    var serverPath = Server.MapPath("~");
                    var fullDPath = $"{serverPath}{dPath}";
                    var isExists = Directory.Exists(fullDPath);

                    if (!isExists)
                        Directory.CreateDirectory(fullDPath);
                    var fi = new FileInfo(uploadFile.FileName);

                    var stream = new MemoryStream(fileData);
                    var img1 = Image.FromStream(stream);
                    Session[SessionEnum.OtherDoc.ToString()] = img1;

                    var ext = fi.Extension;
                    var file = Convert.ToString((new Random()).Next(1000)) + ext;
                    var filePath = $"{fullDPath}{file}";
                    var isFileExists = System.IO.File.Exists(filePath);

                    while (isFileExists)
                    {
                        file = Convert.ToString((new Random()).Next(1000)) + ext;
                        filePath = $"{fullDPath}{file}";
                        isFileExists = System.IO.File.Exists(filePath);
                    }

                    img1.Save(filePath);

                    if (Session[SessionEnum.OtherOldDoc.ToString()] != null)
                    {
                        var otherOldDoc = serverPath + Convert.ToString(Session[SessionEnum.OtherOldDoc.ToString()]);
                        if (System.IO.File.Exists(otherOldDoc))
                            System.IO.File.Delete(otherOldDoc);
                    }

                    var image = new ImageInfoModel
                    {
                        FileName = file,
                        ImageUrl = $"{CommonConfig.ImageUploads}{file}",
                    };
                    HttpContextSessionWrapperExtension.ContentStreamDoc = null;
                    return image;
                }
            }
            return null;
        }

        private ImageInfoModel SavePatientDocLocally(HttpPostedFileBase uploadFile, int pId)
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
                Session[SessionEnum.ProfileImage.ToString()] = img1;
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

        private void ResetPatientDocSessions()
        {
            Session[SessionEnum.TempProfileFile.ToString()] = null;
            Session[SessionEnum.ProfileImage.ToString()] = null;
            Session[SessionEnum.TempOtherDoc.ToString()] = null;
            Session[SessionEnum.OtherDoc.ToString()] = null;
            Session[SessionEnum.OtherOldDoc.ToString()] = null;
            Session[SessionEnum.DocTypeId.ToString()] = null;
            HttpContextSessionWrapperExtension.ContentStream = null;
        }

        /// <summary>
        ///     Saves the image.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns></returns>
        private Image SaveImage(Stream prop)
        {
            var img1 = Image.FromStream(prop);
            return img1;
        }


        private async Task<ResponseData> SavePatientInfo(int patientId, PatientInfoCustomModel currentPatient, PatientInsuranceCustomModel insurance,
              PatientAddressRelation address, string emailAddress, bool isEmailSentBefore = true, bool patientPortalAccess = true)
        {
            var result = new ResponseData { Status = 0, Message = "Error while Saving Patient Details" };

            if (currentPatient.PatientInfo != null)
            {
                var patientVm = currentPatient.PatientInfo;
                var docs = new List<DocumentsTemplates>();

                patientVm.PatientID = patientId;
                patientVm.PersonAge = currentPatient.PersonAge;
                patientVm.PersonEmailAddress = emailAddress;
                //patientVm.PersonMedicalRecordNumber = insurance.PersonHealthCareNumber;

                //Check If Patient has Profile Image to save in the database.
                if (Session[SessionEnum.TempProfileFile.ToString()] != null)
                {
                    var image = PatientInfoProfileImage(patientId);
                    if (image != null)
                    {
                        docs.Add(new DocumentsTemplates
                        {
                            AssociatedID = patientId,
                            AssociatedType = (int)DocAssociatedType.PatientDemographicDocument,
                            DocumentName = image.FileName,
                            FileName = image.FileName,
                            FilePath = image.ImageUrl,
                            DocumentTypeID = (int)DocumentTemplateTypes.ProfileImage
                        });
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "Error While Saving the Profile Image";
                        return result;
                    }
                }

                //Check If Patient has some documents to save in the database.
                if (Session[SessionEnum.TempOtherDoc.ToString()] != null && Session[SessionEnum.DocTypeId.ToString()] != null)
                {
                    var otherDoc = SaveOtherDocument();
                    docs.Add(new DocumentsTemplates
                    {
                        AssociatedID = patientId,
                        AssociatedType = (int)DocAssociatedType.PatientDemographicDocument,
                        DocumentName = otherDoc.FileName,
                        FileName = otherDoc.FileName,
                        FilePath = otherDoc.ImageUrl,
                        DocumentTypeID = Session[SessionEnum.DocTypeId.ToString()] != null ? Convert.ToInt32(Session[SessionEnum.DocTypeId.ToString()]) : 0,
                        PatientID = patientId
                    });
                }

                patientVm.CorporateId = Helpers.GetSysAdminCorporateID();
                patientVm.FacilityId = Helpers.GetDefaultFacilityId();

                var token = CommonConfig.GeneratePasswordResetToken(14, false);
                var code = CommonConfig.GenerateLoginCode(8, false);

                //-----------Save Patient Info---------------------------------
                result = await _service.SavePatientInfo(patientVm, insurance, address, Helpers.GetLoggedInUserId(),
                    Helpers.GetInvariantCultureDateTime(), token, code, docs);

                //Check If Patient Info Saved Successfully. If Yes, then Send Notification to the Patient.
                if (result.Status > 0)
                {
                    patientId = result.Status;

                    //Send Verification Email After Registering the New Patient.
                    if (patientPortalAccess && !isEmailSentBefore)
                    {
                        //Generate the 8-Digit Code
                        var emailSentStatus = await SendVerificationLinkForPatientLoginPortal(patientId,
                            patientVm.PersonEmailAddress, token, code, result.Value1, result.Value2);

                        if (!emailSentStatus)
                        {
                            result.Status = 0;
                            result.Message = "Patients Details Saved Successfully but some exception occurred while sending Notifications!";
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Gets the message identifier.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private int GetMessageId(string state)
        {
            var messageId = 0;
            if (!string.IsNullOrEmpty(state))
            {
                var enumEncounterState = (EncounterStates)Enum.Parse(typeof(EncounterStates), state);
                switch (enumEncounterState)
                {
                    case EncounterStates.admitpatient:
                        messageId = 1;
                        break;
                    case EncounterStates.outpatient:
                        messageId = 2;
                        break;
                    case EncounterStates.discharge:
                        messageId = 3;
                        break;
                    case EncounterStates.endencounter:
                        messageId = 4;
                        break;
                }
            }
            return messageId;
        }

        /// <summary>
        ///     Gets the message.
        /// </summary>
        /// <param name="actual">The actual.</param>
        /// <param name="selected">The selected.</param>
        /// <returns></returns>
        private string GetMessage(int actual, int selected)
        {
            var message = string.Empty;
            if (actual == selected)
            {
                message = "Patient already in this state";
            }
            else
            {
                switch (actual)
                {
                    //Actual Admit State
                    case 1:
                        if (selected == 2 || selected == 4)
                            message = "Patient already in Admit state";
                        break;

                    //Actual Out Patient State
                    case 2:
                        if (selected == 1 || selected == 3)
                            message = "Patient already in OutPatient state";
                        break;
                    //Actual Discharge State
                    case 3:
                        if (selected == 2)
                            message = "Patient already in OutPatient state";
                        else if (selected == 4)
                            message = "Patient already in End Encounter state";
                        break;
                    //Actual End Encounter State
                    case 4:
                        if (selected == 1)
                            message = "Patient already in Admit state";
                        else if (selected == 3)
                            message = "Patient already in Discharge state";
                        break;
                }
            }
            return message;
        }









        //  -----------------Methods Not In Use------------------------------------------------------------------------------------

        /// <summary>
        ///     Saves the image locally.
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

                Session[SessionEnum.ProfileImage.ToString()] = img1;
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


        //public async Task<ActionResult> RegisterNewPatientDemo(RegisterPatientView cm)
        //{
        //    const string defaultEmirate = "111-11-1111";
        //    var patientId = 0;
        //    if (cm != null && cm.CurrentPatient != null && cm.CurrentPatient.PatientInfo != null)
        //    {
        //        var patientVm = cm.CurrentPatient.PatientInfo;


        //        //Get the Details from logged in user global object
        //        var userId = Helpers.GetLoggedInUserId();
        //        var currentDateTime = Helpers.GetInvariantCultureDateTime();
        //        var corporateId = Helpers.GetSysAdminCorporateID();
        //        var facilityId = Helpers.GetDefaultFacilityId();

        //        //Check if FacilityViewModel
        //        if (patientVm != null)
        //        {
        //            using (var bal = new PatientInfoBal())
        //            {
        //                //cm.CurrentPatient.PersonAge = bal.GetAgeByDate(Convert.ToDateTime(patientVm.PersonBirthDate));
        //                patientVm.CreatedBy = userId;
        //                patientVm.CreatedDate = currentDateTime;
        //                patientVm.CorporateId = corporateId;
        //                patientVm.FacilityId = facilityId;
        //                patientVm.IsDeleted = false;
        //                patientVm.PersonAge = cm.CurrentPatient.PersonAge;


        //                //Check for duplicate Social Security Number, DOB and LastName
        //                var isExists = bal.CheckIfEmiratesIdExists(patientVm.PersonEmiratesIDNumber, patientId,
        //                    patientVm.PersonLastName, Convert.ToDateTime(patientVm.PersonBirthDate), facilityId);
        //                if (isExists)
        //                    return Json(new { patientId, status = "duplicate" }, JsonRequestBehavior.AllowGet);

        //                if (patientVm.PersonEmiratesIDNumber == null)
        //                {
        //                    patientVm.PersonEmiratesIDNumber = defaultEmirate;
        //                }
        //                //Check for duplicate Health Care Number (Member ID)
        //                isExists = bal.CheckForDuplicateHealthCareNumber(cm.Insurance.PersonHealthCareNumber, patientId, cm.Insurance.InsuranceCompanyId, cm.Insurance.InsurancePlanId, cm.Insurance.InsurancePolicyId);
        //                if (isExists)
        //                    return Json(new { patientId, status = "duplicatememberid" }, JsonRequestBehavior.AllowGet);


        //                //Check for duplicate Patient's Email
        //                if (!string.IsNullOrEmpty(cm.PatientLoginDetail.Email))
        //                {
        //                    isExists = bal.CheckForDuplicateEmail(cm.PatientLoginDetail.Email, patientId);
        //                    if (isExists)
        //                        return Json(new { patientId, status = "duplicateemail" }, JsonRequestBehavior.AllowGet);
        //                }

        //                using (var trans = new TransactionScope())
        //                {
        //                    try
        //                    {
        //                        patientId = bal.AddUpdatePatientInfo(patientVm);
        //                        if (patientId > 0)
        //                        {
        //                            var statusMessage = string.Empty;

        //                            //Save / Updates Profile Image
        //                            int imageId;
        //                            if (Session[SessionEnum.TempProfileFile.ToString()] != null)
        //                            {
        //                                imageId = SaveProfileImage(patientId);
        //                                if (imageId < 0)
        //                                    statusMessage = "imageerror";
        //                            }
        //                            else
        //                                imageId = 1;

        //                            if (cm.Insurance != null)
        //                                cm.Insurance.PatientID = patientId;
        //                            //cm.Insurance.PersonHealthCareNumber=patientVm
        //                            //cm.Insurance.PersonHealthCareNumber=
        //                            var insId = SavePatientInsuranceData(cm.Insurance);
        //                            if (insId <= 0)
        //                                statusMessage = "insuranceerror";

        //                            //    //Save / Updates Patient's Phone Details
        //                            var phone = new PatientPhone
        //                            {
        //                                PatientID = patientId,
        //                                PhoneNo = cm.CurrentPatient.PatientInfo.PersonContactNumber,
        //                                PhoneType = (int)PhoneType.MobilePhone,
        //                                IsPrimary = true,
        //                                IsdontContact = false,
        //                                IsDeleted = false,
        //                                CreatedDate = currentDateTime,
        //                                CreatedBy = userId
        //                            };
        //                            var phoneId = SavePatientPhoneData(phone);
        //                            if (phoneId <= 0)
        //                                statusMessage = "phoneerror";


        //                            if (cm.CurrentPatientAddressRelation != null &&
        //                                !string.IsNullOrEmpty(cm.CurrentPatientAddressRelation.FirstName) &&
        //                                !string.IsNullOrEmpty(cm.CurrentPatientAddressRelation.LastName))
        //                            {
        //                                cm.CurrentPatientAddressRelation.PatientID = patientId;
        //                                SavePatientAddressRelation(cm.CurrentPatientAddressRelation);
        //                            }

        //                            if (cm.DocumentsAttachment != null)
        //                            {
        //                                cm.DocumentsAttachment.DocumentTypeID = (int)DocumentTemplateTypes.OtherPatientDocuments;
        //                                cm.DocumentsAttachment.AssociatedID = patientId;
        //                                cm.DocumentsAttachment.IsDeleted = false;
        //                                cm.DocumentsAttachment.IsRequired = false;
        //                                cm.DocumentsAttachment.IsTemplate = false;
        //                                cm.DocumentsAttachment.AssociatedType = (int)DocAssociatedType.PatientID;
        //                                SavePatientDocument(cm.DocumentsAttachment);
        //                            }



        //                            //Save / Updates Patient's Login Details
        //                            if (cm.PatientLoginDetail != null)
        //                                cm.PatientLoginDetail.PatientId = patientId;
        //                            if (!string.IsNullOrEmpty(cm.PatientLoginDetail.Email))
        //                            {
        //                                var loginId = await SavePatientSecuritySettings(cm.PatientLoginDetail);
        //                                if (loginId <= 0)
        //                                    statusMessage = "logindetailerror";

        //                            }
        //                            if (imageId > 0 && insId > 0 && phoneId > 0)
        //                                trans.Complete();
        //                            else
        //                                return Json(new { patientId, status = statusMessage }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        //trans.Complete();
        //                    }
        //                    catch
        //                    {
        //                        return Json(new { patientId, status = "error" }, JsonRequestBehavior.AllowGet);
        //                        //throw ex;
        //                    }
        //                }
        //                if (patientId > 0)
        //                    return Json(new { patientId, status = "success" },
        //                                        JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }

        //    return Json(new { patientId, status = "error" }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Updates the patient information.
        /// </summary>
        /// <param name="cm">The patient information model.</param>
        /// <returns></returns>
        //public async Task<ActionResult> UpdatePatientInfoOld(PatientInfoView cm)
        //{
        //    var loginId = 0;
        //    const string defaultEmirate = "111-11-1111";
        //    var patientId = 0;
        //    if (cm != null && cm.CurrentPatient != null && cm.CurrentPatient.PatientInfo != null)
        //    {
        //        var patientVm = cm.CurrentPatient.PatientInfo;
        //        patientId = cm.PatientId;

        //        if (patientId <= 0)
        //            return Json(new { patientId, status = "error" }, JsonRequestBehavior.AllowGet);

        //        //Get the Details from logged in user global object
        //        var userId = Helpers.GetLoggedInUserId();
        //        var currentDateTime = Helpers.GetInvariantCultureDateTime();
        //        var facilityId = Helpers.GetDefaultFacilityId();

        //        if (!string.IsNullOrEmpty(cm.CurrentPhone.PhoneNo))
        //            cm.CurrentPatient.PatientInfo.PersonContactNumber = cm.CurrentPhone.PhoneNo;

        //        //Check if FacilityViewModel
        //        if (patientVm != null)
        //        {
        //            using (var bal = new PatientInfoBal())
        //            {
        //                patientVm.PatientID = patientId;
        //                //cm.CurrentPatient.PersonAge = bal.GetAgeByDate(Convert.ToDateTime(patientVm.PersonBirthDate));

        //                //Check for duplicate Social Security Number, DOB and LastName
        //                var isExists = bal.CheckIfEmiratesIdExists(patientVm.PersonEmiratesIDNumber, patientId,
        //                                            patientVm.PersonLastName, Convert.ToDateTime(patientVm.PersonBirthDate), facilityId);
        //                if (isExists)
        //                    return Json(new { patientId = 0, status = "duplicate" }, JsonRequestBehavior.AllowGet);

        //                if (patientVm.PersonEmiratesIDNumber == null)
        //                    patientVm.PersonEmiratesIDNumber = defaultEmirate;


        //                //Check for duplicate Health Care Number (Member ID)
        //                isExists = bal.CheckForDuplicateHealthCareNumber(cm.Insurance.PersonHealthCareNumber, patientId, cm.Insurance.InsuranceCompanyId, cm.Insurance.InsurancePlanId, cm.Insurance.InsurancePolicyId);
        //                if (isExists)
        //                    return Json(new { patientId, status = "duplicatememberid" }, JsonRequestBehavior.AllowGet);
        //                if (!string.IsNullOrEmpty(cm.PatientLoginDetail.Email))
        //                {
        //                    //Check for duplicate Patient's Email
        //                    isExists = bal.CheckForDuplicateEmail(cm.PatientLoginDetail.Email, patientId);
        //                    if (isExists)
        //                        return Json(new { patientId, status = "duplicateemail" }, JsonRequestBehavior.AllowGet);
        //                }

        //                using (var trans = new TransactionScope())
        //                {
        //                    try
        //                    {
        //                        patientId = bal.AddUpdatePatientInfo(patientVm);
        //                        if (patientId > 0)
        //                        {
        //                            var statusMessage = string.Empty;

        //                            //Save / Updates Profile Image
        //                            int imageId;
        //                            if (Session[SessionEnum.TempProfileFile.ToString()] != null)
        //                            {
        //                                imageId = SaveProfileImage(patientId);
        //                                if (imageId < 0)
        //                                    statusMessage = "imageerror";
        //                            }
        //                            else imageId = 1;

        //                            //Save / Updates Patient's Insurance Details
        //                            if (cm.Insurance != null)
        //                                cm.Insurance.PatientID = patientId;
        //                            var insId = SavePatientInsuranceData(cm.Insurance);
        //                            if (insId <= 0)
        //                                statusMessage = "insuranceerror";
        //                            else//...........Code added to recalculate the Bills when the Insurance Changes for the patient.
        //                            {
        //                                using (var billheaderbal = new BillHeaderBal())
        //                                {
        //                                    using (var encounterbal = new EncounterBal())
        //                                    {
        //                                        var encounterObj = encounterbal.GetEncounterByPatientIdAndActive(patientId);
        //                                        var encounterId = encounterObj != null ? encounterObj.EncounterID : 0;
        //                                        billheaderbal.RecalculateBill(Helpers.GetSysAdminCorporateID(),
        //                                            facilityId, encounterId, "", 0, userId);
        //                                    }
        //                                }
        //                            }

        //                            //Save / Updates Patient's Phone Details
        //                            if (cm.CurrentPhone != null)
        //                            {
        //                                cm.CurrentPhone.PatientID = patientId;
        //                                cm.CurrentPhone.IsPrimary = true;
        //                                cm.CurrentPhone.IsdontContact = false;
        //                                cm.CurrentPhone.IsDeleted = false;
        //                                cm.CurrentPhone.ModifiedDate = currentDateTime;
        //                                cm.CurrentPhone.ModifiedBy = userId;
        //                                cm.CurrentPhone.CreatedDate = currentDateTime;
        //                                cm.CurrentPhone.CreatedBy = userId;
        //                                cm.CurrentPhone.PhoneType = (int)PhoneType.MobilePhone;
        //                            }
        //                            else
        //                            {
        //                                cm.CurrentPhone = new PatientPhone
        //                                {
        //                                    PatientID = patientId,
        //                                    PhoneNo = cm.CurrentPhone.PhoneNo,
        //                                    PhoneType = (int)PhoneType.MobilePhone,
        //                                    IsPrimary = true,
        //                                    IsdontContact = false,
        //                                    IsDeleted = false,
        //                                    CreatedDate = currentDateTime,
        //                                    CreatedBy = userId
        //                                };
        //                            }
        //                            var phoneId = SavePatientPhoneData(cm.CurrentPhone);
        //                            if (phoneId <= 0)
        //                                statusMessage = "phoneerror";


        //                            //Save / Updates Patient's Login Details
        //                            if (cm.PatientLoginDetail != null)
        //                            {
        //                                cm.PatientLoginDetail.PatientId = patientId;
        //                            }
        //                            if (!string.IsNullOrEmpty(cm.PatientLoginDetail.Email))
        //                            {
        //                                loginId = await SavePatientSecuritySettings(cm.PatientLoginDetail);
        //                                if (loginId <= 0)
        //                                    statusMessage = "logindetailerror";
        //                            }
        //                            if (imageId > 0 && insId > 0 && phoneId > 0)
        //                                trans.Complete();
        //                            else
        //                                return Json(new { patientId, status = statusMessage }, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        return Json(new { patientId, status = "error" }, JsonRequestBehavior.AllowGet);
        //                        //throw ex;
        //                    }
        //                }
        //                if (patientId > 0)
        //                    return Json(new { patientId, status = "success", loginId },
        //                                        JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    return Json(new { patientId, status = "error" }, JsonRequestBehavior.AllowGet);
        //}
    }
}
