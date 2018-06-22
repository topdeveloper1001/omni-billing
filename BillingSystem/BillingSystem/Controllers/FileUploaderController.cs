// To Do: FileUploaderController.cs
// FileName :FileUploaderController.cs
// CreatedDate: 2016-03-14 8:15 PM
// ModifiedDate: 2016-05-11 11:54 AM
// CreatedBy: Shashank Awasthy

namespace BillingSystem.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Transactions;
    using System.Web;
    using System.Web.Mvc;

    using Bal.BusinessAccess;
    using Common;
    using Common.Common;
    using Model;
    using Model.CustomModel;
    using Models;

    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    #endregion

    public class FileUploaderController : BaseController
    {
        // GET: /FileUploader/
        /// <summary>
        ///     Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///     Gets the type of the documents by.
        /// </summary>
        /// <param name="associatedType"> Type of the associated. </param>
        /// <param name="pid">            The pid. </param>
        /// <returns></returns>
        public ActionResult GetDocumentsByType(int associatedType, int pid)
        {
            //Initialize the MedicalNotes BAL object
            using (var documentTemplatesBal = new DocumentsTemplatesBal())
            {
                var userid = Helpers.GetLoggedInUserId();
                //Get the facilities list
                var documentslist = documentTemplatesBal.GetDocumentsCustomModelByType(associatedType, pid).ToList();
                //GetDocumentsByType

                var fileUploaderView = new FileUploaderView
                {
                    Attachments = documentslist,
                    CurrentAttachment = new DocumentsTemplates()
                };
                if (associatedType == 2) //Radiology
                {
                    var openorderBal = new OpenOrderBal(
                        Helpers.DefaultCptTableNumber,
                        Helpers.DefaultServiceCodeTableNumber,
                        Helpers.DefaultDrgTableNumber,
                        Helpers.DefaultDrugTableNumber,
                        Helpers.DefaultHcPcsTableNumber,
                        Helpers.DefaultDiagnosisTableNumber);
                    var patientActiveEncounter = openorderBal.GetActiveEncounterId(pid);
                    //var patientOrders = openorderBal.GetOrdersByPatientId(pid);
                    var patientOrders = openorderBal.GetAllOrdersByEncounterId(patientActiveEncounter);
                    var patientOpenOrders =
                        patientOrders.Where(
                            _ =>
                            _.OrderStatus == Convert.ToInt32(OrderStatus.Open).ToString()
                            && _.CategoryId == Convert.ToInt32(GlobalCodeCategoryValue.Radiology)).ToList();
                    var patientClosedOrders =
                        patientOrders.Where(
                            _ =>
                            _.OrderStatus != Convert.ToInt32(OrderStatus.Open).ToString()
                            && _.CategoryId == Convert.ToInt32(GlobalCodeCategoryValue.Radiology)).ToList();
                    fileUploaderView.OpenOrdersList = patientOpenOrders;
                    fileUploaderView.ClosedOrdersList = patientClosedOrders;
                    using (var medicalnotesbal = new MedicalNotesBal()) //Updated by Shashank on Oct 28, 2014
                    {
                        var phyMedicalnotes = medicalnotesbal.GetCustomMedicalNotes(
                            pid,
                            Convert.ToInt32(NotesUserType.Physician));
                        fileUploaderView.CurrentMedicalNotes = new MedicalNotes();
                        fileUploaderView.MedicalNotesList = phyMedicalnotes;
                        fileUploaderView.ViewType = "2";
                    }
                    using (
                        var orderActivityBal = new OrderActivityBal(
                            Helpers.DefaultCptTableNumber,
                            Helpers.DefaultServiceCodeTableNumber,
                            Helpers.DefaultDrgTableNumber,
                            Helpers.DefaultDrugTableNumber,
                            Helpers.DefaultHcPcsTableNumber,
                            Helpers.DefaultDiagnosisTableNumber))
                    {
                        if (patientActiveEncounter != 0)
                        {
                            //var labactivitesobj =
                            //    orderActivityBal.GetOrderActivitiesByEncounterId(patientActiveEncounter);
                            var radactivitesobj =
                               orderActivityBal.GetOrderActivitiesByEncounterIdSP(patientActiveEncounter);
                            var radActivitesListObj =
                                radactivitesobj.Where(
                                    x => x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.Radiology)).ToList();
                            var radActivitesClosedListObj =
                                radActivitesListObj.Where(
                                    x =>
                                    x.OrderActivityStatus != 0
                                    && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)).ToList();
                            var radActivitesOpenListObj =
                                radActivitesListObj.Where(
                                    x =>
                                    x.OrderActivityStatus == 0
                                    || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)).ToList();
                            fileUploaderView.OpenActvitiesList = radActivitesOpenListObj;
                            fileUploaderView.ClosedActvitiesList = radActivitesClosedListObj;
                            fileUploaderView.CurrentOrderActivity = new OrderActivity();
                            fileUploaderView.EncounterOrder = new OpenOrder();
                            fileUploaderView.ClosedOrdersList = patientClosedOrders;
                        }
                        else
                        {
                            fileUploaderView.OpenActvitiesList = new List<OrderActivityCustomModel>();
                            fileUploaderView.ClosedActvitiesList = new List<OrderActivityCustomModel>();
                            fileUploaderView.CurrentOrderActivity = new OrderActivity();
                            fileUploaderView.EncounterOrder = new OpenOrder();
                            fileUploaderView.OpenOrdersList = new List<OpenOrderCustomModel>();
                            fileUploaderView.ClosedOrdersList = new List<OpenOrderCustomModel>();
                        }
                    }
                }
                fileUploaderView.CurrentAttachment.CreatedDate = Helpers.GetInvariantCultureDateTime();
                //Pass the ActionResult with List of MedicalNotesViewModel object to Partial View MedicalNotesList
                return PartialView(PartialViews.FileUploader, fileUploaderView);
            }
        }

        /// <summary>
        /// Patients the documents grid.
        /// </summary>
        /// <param name="associatedType">Type of the associated.</param>
        /// <param name="pid">The pid.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="sortdir">The sortdir.</param>
        /// <returns></returns>
        public ActionResult PatientDocumentsGrid(string associatedType, string pid, string sort, string sortdir)
        {
            var oDocumentTemplatesBal = new DocumentsTemplatesBal();

            var documentslist = oDocumentTemplatesBal.GetDocumentsCustomModelByType(
                Convert.ToInt32(associatedType),
                Convert.ToInt32(pid));
            switch (sort.ToLower())
            {
                case "documentname":
                    documentslist = sortdir.ToLower() == "asc"
                                        ? documentslist.OrderBy(i => i.DocumentName).ToList()
                                        : documentslist.OrderByDescending(i => i.DocumentName).ToList();
                    break;

                case "externalvalue1":
                    documentslist = sortdir.ToLower() == "asc"
                                        ? documentslist.OrderBy(i => i.ExternalValue1).ToList()
                                        : documentslist.OrderByDescending(i => i.ExternalValue1).ToList();
                    break;

                case "filename":
                    documentslist = sortdir.ToLower() == "asc"
                                        ? documentslist.OrderBy(i => i.FileName).ToList()
                                        : documentslist.OrderByDescending(i => i.FileName).ToList();
                    break;

                case "documentnotes":
                    documentslist = sortdir.ToLower() == "asc"
                                        ? documentslist.OrderBy(i => i.DocumentNotes).ToList()
                                        : documentslist.OrderByDescending(i => i.DocumentNotes).ToList();
                    break;

                case "createddate":
                    documentslist = sortdir.ToLower() == "asc"
                                        ? documentslist.OrderBy(i => i.CreatedDate).ToList()
                                        : documentslist.OrderByDescending(i => i.CreatedDate).ToList();
                    break;

                case "oldmedicalrecordsoruce":
                    documentslist = sortdir.ToLower() == "asc"
                                        ? documentslist.OrderBy(i => i.OldMedicalRecordSoruce).ToList()
                                        : documentslist.OrderByDescending(i => i.OldMedicalRecordSoruce).ToList();
                    break;

                default:
                    documentslist = sortdir.ToLower() == "asc"
                                        ? documentslist.OrderBy(i => i.ReferenceNumber).ToList()
                                        : documentslist.OrderByDescending(i => i.ReferenceNumber).ToList();
                    break;
            }
            return PartialView(PartialViews.FilesListing, documentslist);
        }

        /// <summary>
        ///     Ajaxes the submit.
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AjaxSubmit(int? id)
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                var pid = Request["pid"];
                var uploadedFile = Request.Files[0];
                var uploadFile = Request.Files[0];
                var fi = new FileInfo(uploadFile.FileName);
                HttpContextSessionWrapperExtension.ContentLength = null;
                HttpContextSessionWrapperExtension.ContentType = null;
                HttpContextSessionWrapperExtension.ContentStream = null;
                Session[SessionEnum.TempOtherDoc.ToString()] = uploadedFile;
                HttpContextSessionWrapperExtension.ContentLength = uploadedFile.ContentLength;
                HttpContextSessionWrapperExtension.ContentType = uploadedFile.ContentType;
                var b = new byte[uploadedFile.ContentLength];
                uploadedFile.InputStream.Read(b, 0, uploadedFile.ContentLength);
                Stream stream = new MemoryStream(b);
                if (!HttpContextSessionWrapperExtension.ContentType.Contains("pdf"))
                {
                    var orginalImage = new Bitmap(stream);
                    var orginalWidth = orginalImage.Width;
                    HttpContextSessionWrapperExtension.ContentStream = b;
                    const string virtualPath = "Content\\Documents\\Documents\\";
                    var serverPath = Server.MapPath("~");
                    var imagesPath = string.Format("{0}{1}{2}", serverPath, virtualPath, pid);
                    var isExists = Directory.Exists(imagesPath);
                    var fileCount = 1;
                    if (!isExists) Directory.CreateDirectory(imagesPath);
                    else
                        fileCount = Directory.GetFiles(imagesPath).Any()
                                        ? Directory.GetFiles(imagesPath).Count() + 1
                                        : 1;

                    var filename = fi.Name;
                    var saveImagePath = string.Format("{0}\\" + filename, imagesPath);
                    if (System.IO.File.Exists(saveImagePath))
                    {
                        filename = string.Format(
                            "{0}{1}{2}{3}",
                            fi.Name.Split('.')[0],
                            fileCount,
                            ".",
                            fi.Name.Split('.')[1]);
                        saveImagePath = string.Format("{0}\\" + filename, imagesPath);
                    }

                    var img = SaveImage(stream, ".jpg");
                    img.Save(saveImagePath);
                    var imagesUrl = string.Format("/Content/Documents/Documents/{0}/" + fi.Name, pid);
                    Session["ImagesUrl"] = imagesUrl;
                    Session["FileName"] = filename;
                    return Content(uploadedFile.ContentType + ";" + uploadedFile.ContentLength);
                }
                else
                {
                    var tempFile = new byte[uploadFile.ContentLength];
                    uploadFile.InputStream.Read(tempFile, 0, uploadFile.ContentLength);

                    const string virtualPath = "Content\\Documents\\Documents\\";
                    var serverPath = Server.MapPath("~");
                    var documentsPath = string.Format("{0}{1}{2}", serverPath, virtualPath, pid);
                    var isExists = Directory.Exists(documentsPath);
                    var fileCount = 1;
                    if (!isExists) Directory.CreateDirectory(documentsPath);
                    else
                        fileCount = Directory.GetFiles(documentsPath).Any()
                                        ? Directory.GetFiles(documentsPath).Count() + 1
                                        : 1;
                    var filename = fi.Name;
                    var documentUrl = string.Format("/Content/Documents/Documents/{0}/" + filename, pid);
                    if (System.IO.File.Exists(string.Format("{0}{1}", serverPath, documentUrl)))
                    {
                        filename = string.Format(
                            "{0}{1}{2}{3}",
                            fi.Name.Split('.')[0],
                            fileCount,
                            ".",
                            fi.Name.Split('.')[1]);
                    }
                    uploadedFile.SaveAs(Path.Combine(documentsPath, filename));
                    Session["ImagesUrl"] = documentUrl;
                    Session["FileName"] = filename;
                    Session["Filetype"] = ".pdf";
                    return Content(uploadedFile.ContentType + ";" + uploadedFile.ContentLength);
                }
            }
            return Content("");
        }

        /// <summary>
        ///     Saves the image.
        /// </summary>
        /// <param name="prop"> The property. </param>
        /// <param name="ext">  The ext. </param>
        /// <returns></returns>
        private Image SaveImage(Stream prop, string ext)
        {
            var img1 = Image.FromStream(prop);
            return img1;
        }

        /// <summary>
        ///     Load image
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns></returns>
        public ActionResult ImageLoad(int? id)
        {
            var b = HttpContextSessionWrapperExtension.ContentStream;
            var type = HttpContextSessionWrapperExtension.ContentType;
            if (!type.Contains("pdf"))
            {
                HttpContextSessionWrapperExtension.CroppedContentType = type;

                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = type;
                Response.BinaryWrite(b);
                Response.Flush();
                Response.End();
            }
            return Content("");
        }

        /// <summary>
        ///     Files the upload.
        /// </summary>
        /// <param name="files"> The files. </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase files)
        {
            if (files == null)
            {
                ModelState.AddModelError("File", "Please Upload Your file");
            }
            else if (files.ContentLength > 0)
            {
                var MaxContentLength = 1024 * 1024 * 3; //3 MB
                var AllowedFileExtensions = new[] { ".jpg", ".gif", ".png", ".pdf" };

                if (!AllowedFileExtensions.Contains(files.FileName.Substring(files.FileName.LastIndexOf('.'))))
                {
                    ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                }
                else if (files.ContentLength > MaxContentLength)
                {
                    ModelState.AddModelError(
                        "File",
                        "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
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
        ///     Saves the documents.
        /// </summary>
        /// <param name="documentsTemplates"> The documents templates. </param>
        /// <returns></returns>
        public ActionResult SaveDocuments(DocumentsTemplates documentsTemplates)
        {
            var documentsTemplatesBal = new DocumentsTemplatesBal();
            var newId = -1;
            var userid = Helpers.GetLoggedInUserId();
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();

            if (documentsTemplates.DocumentsTemplatesID > 0)
            {
                var documentTemplatetoupdate =
                    documentsTemplatesBal.GetDocumentById(documentsTemplates.DocumentsTemplatesID);
                documentTemplatetoupdate.DocumentTypeID = documentsTemplates.DocumentTypeID;
                documentTemplatetoupdate.DocumentName = documentsTemplates.DocumentName;
                documentTemplatetoupdate.AssociatedType = documentsTemplates.AssociatedType;
                if (Session["FileName"] != null)
                {
                    documentTemplatetoupdate.FileName = Session["FileName"].ToString();
                    documentTemplatetoupdate.FilePath = Session["ImagesUrl"].ToString();
                    documentTemplatetoupdate.IsTemplate = documentsTemplates.FileName.Contains("pdf");
                }
                documentTemplatetoupdate.ModifiedBy = userid;
                documentTemplatetoupdate.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                documentTemplatetoupdate.CreatedBy = userid;
                documentTemplatetoupdate.CreatedDate = documentsTemplates.CreatedDate;
                documentTemplatetoupdate.FacilityID = facilityId;
                documentTemplatetoupdate.CorporateID = corporateId;
                documentTemplatetoupdate.ExternalValue1 = documentsTemplates.ExternalValue1;
                documentTemplatetoupdate.ExternalValue2 = documentsTemplates.ExternalValue2;
                newId = documentsTemplatesBal.AddUpdateDocumentTempate(documentTemplatetoupdate);
            }
            else
            {
                if (Session["FileName"] == null) return Json(newId);
                documentsTemplates.AssociatedType = documentsTemplates.AssociatedType;
                documentsTemplates.FileName = Session["FileName"].ToString();
                documentsTemplates.FilePath = Session["ImagesUrl"].ToString();
                documentsTemplates.IsTemplate = documentsTemplates.FileName.Contains("pdf");
                documentsTemplates.CreatedBy = userid;
                documentsTemplates.CreatedDate = documentsTemplates.CreatedDate;
                documentsTemplates.FacilityID = facilityId;
                documentsTemplates.CorporateID = corporateId;
                newId = documentsTemplatesBal.AddUpdateDocumentTempate(documentsTemplates);
            }
            Session["FileName"] = null;
            return Json(newId);
        }

        /// <summary>
        ///     Gets the patient document.
        /// </summary>
        /// <param name="documentid"> The documentid. </param>
        /// <returns></returns>
        public ActionResult GetPatientDocument(string documentid)
        {
            var documentidInt = Convert.ToInt32(documentid);
            var documentsTemplatesBal = new DocumentsTemplatesBal();
            var objDocumentTemplateData = documentsTemplatesBal.GetDocumentById(documentidInt);
            if (objDocumentTemplateData != null)
            {
                return PartialView(PartialViews.PatientDocumentsAddEdit, objDocumentTemplateData);
            }
            return null;
        }

        /// <summary>
        ///     Gets the documents.
        /// </summary>
        /// <param name="patientId">      The patient identifier. </param>
        /// <param name="associatedtype"> The associatedtype. </param>
        /// <returns></returns>
        public ActionResult GetDocuments(string patientId, int associatedtype)
        {
            var patientIdint = Convert.ToInt32(patientId);
            var documentsTemplatesBal = new DocumentsTemplatesBal();
            //var objDocumentTemplateData = documentsTemplatesBal.GetPatientDocuments(patientIdint, associatedtype);
            var objDocumentTemplateData = documentsTemplatesBal.GetPatientCustomDocuments(patientIdint, associatedtype);
            //GetPatientCustomDocuments
            return PartialView(PartialViews.FilesListing, objDocumentTemplateData);
        }

        /// <summary>
        ///     Gets the document by identifier.
        /// </summary>
        /// <param name="documentid"> The documentid. </param>
        /// <returns></returns>
        public ActionResult GetDocumentById(int documentid)
        {
            var documentidInt = Convert.ToInt32(documentid);
            var documentsTemplatesBal = new DocumentsTemplatesBal();
            var objDocumentTemplateData = documentsTemplatesBal.GetDocumentById(documentidInt);
            if (objDocumentTemplateData != null)
            {
                return PartialView(PartialViews.FilesView, objDocumentTemplateData);
            }
            return null;
        }

        /// <summary>
        ///     Delete the current facility based on the Facility ID passed in the SharedViewModel
        /// </summary>
        /// <param name="Id"> The identifier. </param>
        /// <returns></returns>
        public ActionResult DeleteFile(int Id)
        {
            using (var bal = new DocumentsTemplatesBal())
            {
                //Get facility model object by current facility ID
                var model = bal.GetDocumentById(Convert.ToInt32(Id));

                //Check If facility model is not null
                if (model != null)
                {
                    model.IsDeleted = true;
                    model.DeletedBy = Helpers.GetLoggedInUserId();
                    model.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current facility
                    var result = bal.AddUpdateDocumentTempate(model);

                    //return deleted ID of current facility as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        ///     Reset the Facility View Model and pass it to FacilityAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetDocumentForm()
        {
            //Intialize the new object of Facility ViewModel
            var documentTemplate = new DocumentsTemplates();
            documentTemplate.CreatedDate = Helpers.GetInvariantCultureDateTime();
            //Pass the View Model as FacilityViewModel to PartialView FacilityAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.FilesAddEdit, documentTemplate);
        }

        /// <summary>
        /// Deletes the file permanently.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public JsonResult DeleteFilePermanently(int id, int patientId)
        {
            var result = false;
            using (var transcope = new TransactionScope())
            {
                using (var bal = new DocumentsTemplatesBal())
                {
                    if (id > 0 && patientId > 0)
                    {
                        var model = bal.GetDocumentById(id);
                        if (model != null)
                        {
                            //Update Operation of current facility
                            var deleteId = bal.DeleteDocument(id);
                            if (deleteId > 0)
                            {
                                var fileName = model.FilePath;
                                var filePath = Server.MapPath(fileName);
                                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                    transcope.Complete();
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(result);
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            var msg = string.Empty;
            if (file != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var loggedInUserId = Helpers.GetLoggedInUserId();
                var fileN = Path.GetFileName(file.FileName);
                var filePath = string.Format(CommonConfig.XMLBillFilePath, corporateId, facilityId, loggedInUserId);
                var completePath = Server.MapPath(filePath);

                if (!Directory.Exists(completePath)) Directory.CreateDirectory(completePath);

                filePath = filePath + fileN;
                file.SaveAs(completePath + fileN);

                var xml = Helpers.GetXML(completePath + fileN);

                if (!string.IsNullOrEmpty(xml))
                {
                    using (var bal = new XMLBillingBal())
                    {
                        var result = bal.XMLBillFileParser(xml, filePath, true, corporateId, facilityId, string.Empty);
                        msg = result;
                    }
                }
            }
            return Content(msg);
        }

        #region Enter/Upload Remittance Advice

        /// <summary>
        ///     Remittances the XML billing.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <returns></returns>
        public ActionResult RemittanceXMLBilling(string message)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            if (!string.IsNullOrEmpty(message)) ViewBag.Message = message;
            var xFileHeaderBal = new XFileHeaderBal();
            var xFileHeaderList = xFileHeaderBal.GetXFileHeaderByCId(corporateId, facilityId);
            var dataView = new RemittanceAdviceView
            {
                XAdviceXMLData = new List<XAdviceXMLParsedDataCustomModel>(),
                //objBal.GetXAdviceXMLParsedDataCustom(corporateId, facilityId)
                FilesUploaded = xFileHeaderList
            };
            return View(dataView);
        }

        /// <summary>
        ///     Uploads the xm ls.
        /// </summary>
        /// <param name="file"> The file. </param>
        /// <returns></returns>
        public ActionResult UploadXMLs(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var loggedInUserId = Helpers.GetLoggedInUserId();
                var fileN = Path.GetFileName(file.FileName);
                var filePath = string.Format(
                    CommonConfig.RemittanceAdviceXmlFilePath,
                    corporateId,
                    facilityId,
                    loggedInUserId);
                var completePath = Server.MapPath(filePath);
                if (!Directory.Exists(completePath)) Directory.CreateDirectory(completePath);

                filePath = filePath + fileN;
                file.SaveAs(completePath + fileN);

                var xml = Helpers.GetXML(completePath + fileN);
                if (!string.IsNullOrEmpty(xml))
                {
                    using (var bal = new XMLBillingBal())
                    {
                        var result = bal.RemittanceXMLParser(xml, filePath, true, corporateId, facilityId);
                        var msg = string.IsNullOrEmpty(result) || result.Equals("1") ? "1" : result;
                        return RedirectToAction("RemittanceXMLBilling", new { message = msg });
                        //if (result)
                        //    return RedirectToAction("RemittanceXMLBilling", new { message = msg });
                        //else
                        //{
                        //    return RedirectToAction("RemittanceXMLBilling", new { message = "2" });
                        //}
                    }
                }
            }
            return RedirectToAction("RemittanceXMLBilling");
        }

        /// <summary>
        ///     Views the remittance file.
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns></returns>
        public string ViewRemittanceFile(int id)
        {
            using (var xFileHeaderBal = new XFileHeaderBal())
            {
                var xmlString = xFileHeaderBal.GetRemittanceFormattedXmlStringByFileId(id);
                return (xmlString);
            }
        }

        /// <summary>
        ///     Views the remittance data.
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns></returns>
        public ActionResult ViewRemittanceData(int id)
        {
            var listToreturn = new List<XAdviceXMLParsedDataCustomModel>();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            using (var objBal = new XAdviceXMLParsedDataBal())
            {
                listToreturn = objBal.GetXAdviceXmlParsedDataById(corporateId, facilityId, id);
            }
            var systemClaims = RenderPartialViewToString(
                PartialViews.RemittanceXMLParsedDataView,
                listToreturn.Where(x => x.IsFacilityEncounter != 0).ToList());
            //var nonSystemClaims = RenderPartialViewToString(PartialViews.RemittanceXMLParsedDataNonSystemView, listToreturn.Where(x => x.IsFacilityEncounter == 0).Take(1000).ToList());
            var nonSystemClaims = RenderPartialViewToString(
                PartialViews.RemittanceXMLParsedDataNonSystemView,
                listToreturn.Where(x => x.IsFacilityEncounter == 0).ToList());
            //return PartialView(PartialViews.RemittanceXMLParsedDataView, listToreturn);
            var jsonresulttoRetrun = Json(new { systemClaims, nonSystemClaims }, JsonRequestBehavior.AllowGet);
            jsonresulttoRetrun.MaxJsonLength = int.MaxValue;
            return jsonresulttoRetrun;
        }

        /// <summary>
        ///     Applies the charges in remittance advice.
        /// </summary>
        /// <param name="fileId"> The file identifier. </param>
        /// <returns></returns>
        public ActionResult ApplyChargesInRemittanceAdvice(int fileId)
        {
            var parsedList = new List<XFileHeaderCustomModel>();
            using (var bal = new XclaimBal())
            {
                var corporateid = Helpers.GetSysAdminCorporateID();
                var facilityid = Helpers.GetDefaultFacilityId();
                var xclaimList = bal.ApplyAdvicePaymentInRemittanceAdvice(corporateid, facilityid, fileId);
                if (xclaimList)
                {
                    using (var xFileHeaderBal = new XFileHeaderBal())
                    {
                        parsedList = xFileHeaderBal.GetXFileHeaderByCId(corporateid, facilityid);
                    }
                }
                var stringpartialView = RenderPartialViewToString(PartialViews.XMLRemittanceFile, parsedList);
                return Json(new { xclaimList, parsedList = stringpartialView });
            }
        }

        /// <summary>
        ///     Renders the partial view to string.
        /// </summary>
        /// <param name="viewName"> Name of the view. </param>
        /// <param name="model">    The model. </param>
        /// <returns></returns>
        private string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName)) viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion Enter/Upload Remittance Advice

        #region XML Billing Methods

        /// <summary>
        ///     Imports the bill file.
        /// </summary>
        /// <param name="msg"> The MSG. </param>
        /// <returns></returns>
        public ActionResult ImportBillFile(string msg)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();

            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.Message = msg;
            }

            var objBal = new TPXMLParsedDataBal();
            var dataView = new XMLBillFileView
            {
                XAdviceXMLData = new List<TPXMLParsedDataCustomModel>(),
                // objBal.TPXMLParsedDataListCIDFID(corporateId, facilityId),
                XMLBillFile = objBal.TPXMLFilesListCIDFID(corporateId, facilityId)
            };
            return View(dataView);
        }

        /// <summary>
        ///     Imports the bill XML file.
        /// </summary>
        /// <param name="file"> The file. </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportBillXmlFile()
        {

            var executeDetails = Convert.ToBoolean(Request.Form["executeDetails"]);
            //if (file != null)
            //{
            //    var corporateId = Helpers.GetSysAdminCorporateID();
            //    var facilityId = Helpers.GetDefaultFacilityId();
            //    var loggedInUserId = Helpers.GetLoggedInUserId();
            //    var fileN = Path.GetFileName(file.FileName);
            //    var filePath = string.Format(CommonConfig.XMLBillFilePath, corporateId, facilityId, loggedInUserId);
            //    var completePath = Server.MapPath(filePath);
            //    if (!Directory.Exists(completePath))
            //        Directory.CreateDirectory(completePath);
            //    var fastZip = new FastZip();
            //    string fileFilter = null;
            //    // Will always overwrite if target filenames already exist
            //    fastZip.ExtractZip(Path.GetFullPath(file.FileName), completePath, fileFilter);
            //    var fs = System.IO.File.OpenRead(Path.GetFullPath(file.FileName));

            // var zf = new ZipFile(fs); //var fname = ""; foreach (var zipEntry in zf) { fileN =
            // ((ZipEntry)(zipEntry)).Name; }

            // filePath = filePath + fileN; //file.SaveAs(completePath + fileN);

            // var xml = Helpers.GetXML(completePath + fileN); if (!string.IsNullOrEmpty(xml)) {
            // using (var bal = new XMLBillingBal()) { var result = bal.XMLBillFileParser(xml,
            // filePath, true, corporateId, facilityId, string.Empty); if
            // (string.IsNullOrEmpty(result) || result == "1") { return
            // RedirectToAction("ImportBillFile", new { msg = "1" }); }

            // return this.RedirectToAction("ImportBillFile", new { msg = result }); } }

            //    return this.RedirectToAction("ImportBillFile", new { msg = "3" });
            //}

            var msg = "";
            var file = Request.Files[0];
            if (file != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var loggedInUserId = Helpers.GetLoggedInUserId();
                var fileN = Path.GetFileName(file.FileName);
                var filePath = string.Format(CommonConfig.XMLBillFilePath, corporateId, facilityId, loggedInUserId);
                var completePath = Server.MapPath(filePath);

                if (!Directory.Exists(completePath)) Directory.CreateDirectory(completePath);

                filePath = filePath + fileN;
                file.SaveAs(completePath + fileN);

                var xml = Helpers.GetXML(completePath + fileN);

                if (!string.IsNullOrEmpty(xml))
                {
                    using (var bal = new XMLBillingBal())
                    {
                        var result = bal.XMLBillFileParser(xml, filePath, true, corporateId, facilityId, string.Empty, executeDetails, loggedInUserId);
                        msg = string.IsNullOrEmpty(result) || result.Equals("1") ? "1" : result;
                    }
                }
                else msg = "3";
            }

            return RedirectToAction("ImportBillFile", new { msg });
        }

        /// <summary>
        ///     Views the file.
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns></returns>
        public string ViewFile(int id)
        {
            using (var xFileHeaderBal = new TpFileHeaderBal())
            {
                var xmlString = xFileHeaderBal.GetFormattedXmlStringByXFileId(id);
                return (xmlString);
            }
        }

        /// <summary>
        ///     Views the parsed data.
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns></returns>
        public ActionResult ViewParsedData(int id)
        {
            //var listToreturn = new List<TPXMLParsedDataCustomModel>();
            //using (var objBal = new TPXMLParsedDataBal())
            //{
            //    listToreturn = objBal.TPXMLParsedDataList(id);
            //}
            //return PartialView(PartialViews.XMLBillFileView, listToreturn);

            var result = GetXmlParsedData(id);
            return PartialView(PartialViews.XMLBillFileView, result);
        }

        /// <summary>
        /// Exports the x ml view to excel.
        /// </summary>
        /// <param name="FileHeaderId">The file header identifier.</param>
        /// <returns></returns>
        public ActionResult ExportXMlViewToExcel(int? FileHeaderId)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var workbook = new HSSFWorkbook();

            #region Excel Creation and Formatting

            var sheet = workbook.CreateSheet("XMLParsedData");
            sheet.CreateFreezePane(0, 1, 0, 1);
            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            var font = workbook.CreateFont();
            var format = workbook.CreateDataFormat();
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.Color = IndexedColors.Black.Index;
            style.SetFont(font);
            sheet.SetColumnWidth(0, 5000);
            sheet.SetColumnWidth(1, 5000);
            sheet.SetColumnWidth(2, 5000);
            sheet.SetColumnWidth(3, 5000);
            sheet.SetColumnWidth(4, 5000);
            sheet.SetColumnWidth(5, 5000);
            sheet.SetColumnWidth(6, 5000);
            sheet.SetColumnWidth(7, 5000);
            sheet.SetColumnWidth(8, 5000);
            sheet.SetColumnWidth(9, 5000);
            sheet.SetColumnWidth(10, 5000);
            sheet.SetColumnWidth(11, 5000);
            sheet.SetColumnWidth(12, 5000);
            sheet.SetColumnWidth(13, 5000);
            sheet.SetColumnWidth(14, 5000);
            sheet.SetColumnWidth(15, 5000);
            sheet.SetColumnWidth(16, 5000);
            sheet.SetColumnWidth(17, 5000);
            sheet.SetColumnWidth(18, 5000);
            sheet.SetColumnWidth(19, 5000);
            sheet.SetColumnWidth(20, 5000);
            sheet.SetColumnWidth(21, 5000);
            sheet.SetColumnWidth(22, 5000);
            sheet.SetColumnWidth(23, 5000);
            sheet.SetColumnWidth(24, 5000);
            sheet.SetColumnWidth(25, 5000);
            sheet.SetColumnWidth(26, 5000);
            sheet.SetColumnWidth(27, 5000);
            sheet.SetColumnWidth(28, 5000);
            sheet.SetColumnWidth(29, 5000);

            var columnstyle = workbook.CreateCellStyle();
            sheet.SetDefaultColumnStyle(0, columnstyle);
            sheet.SetDefaultColumnStyle(1, columnstyle);
            sheet.SetDefaultColumnStyle(2, columnstyle);
            sheet.SetDefaultColumnStyle(3, columnstyle);
            sheet.SetDefaultColumnStyle(4, columnstyle);
            sheet.SetDefaultColumnStyle(5, columnstyle);
            sheet.SetDefaultColumnStyle(6, columnstyle);
            sheet.SetDefaultColumnStyle(7, columnstyle);
            sheet.SetDefaultColumnStyle(8, columnstyle);
            sheet.SetDefaultColumnStyle(9, columnstyle);
            sheet.SetDefaultColumnStyle(10, columnstyle);
            sheet.SetDefaultColumnStyle(11, columnstyle);
            sheet.SetDefaultColumnStyle(12, columnstyle);
            sheet.SetDefaultColumnStyle(13, columnstyle);
            sheet.SetDefaultColumnStyle(14, columnstyle);
            sheet.SetDefaultColumnStyle(15, columnstyle);
            sheet.SetDefaultColumnStyle(16, columnstyle);
            sheet.SetDefaultColumnStyle(17, columnstyle);
            sheet.SetDefaultColumnStyle(18, columnstyle);
            sheet.SetDefaultColumnStyle(19, columnstyle);
            sheet.SetDefaultColumnStyle(20, columnstyle);
            sheet.SetDefaultColumnStyle(21, columnstyle);
            sheet.SetDefaultColumnStyle(22, columnstyle);
            sheet.SetDefaultColumnStyle(23, columnstyle);
            sheet.SetDefaultColumnStyle(24, columnstyle);
            sheet.SetDefaultColumnStyle(25, columnstyle);
            sheet.SetDefaultColumnStyle(26, columnstyle);
            sheet.SetDefaultColumnStyle(27, columnstyle);
            sheet.SetDefaultColumnStyle(28, columnstyle);
            sheet.SetDefaultColumnStyle(29, columnstyle);

            #endregion Excel Creation and Formatting

            #region Header row 1

            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            var style1 = workbook.CreateCellStyle();
            style1.FillForegroundColor = HSSFColor.White.Index;
            style1.FillBackgroundColor = HSSFColor.Blue.Index;
            style1.FillPattern = FillPattern.SolidForeground;
            style1.BorderTop = BorderStyle.Medium;
            style1.BorderBottom = BorderStyle.Medium;
            style1.BorderLeft = BorderStyle.Medium;
            style1.BorderRight = BorderStyle.Medium;
            style1.SetFont(font);
            row.CreateCell(0).SetCellValue("Batch Number");
            row.GetCell(0).CellStyle = style1;
            row.CreateCell(1).SetCellValue("File ID");
            row.GetCell(1).CellStyle = style1;
            row.CreateCell(2).SetCellValue("Claim ID");
            row.GetCell(2).CellStyle = style1;
            row.CreateCell(3).SetCellValue("Bill Number");
            row.GetCell(3).CellStyle = style1;
            row.CreateCell(4).SetCellValue("Member ID");
            row.GetCell(4).CellStyle = style1;
            row.CreateCell(5).SetCellValue("Payer ID");
            row.GetCell(5).CellStyle = style1;
            row.CreateCell(6).SetCellValue("Provider ID");
            row.GetCell(6).CellStyle = style1;
            row.CreateCell(7).SetCellValue(ResourceKeyValues.GetKeyValue("socialsecuritynumber"));
            row.GetCell(7).CellStyle = style1;
            row.CreateCell(8).SetCellValue("Gross");
            row.GetCell(8).CellStyle = style1;
            row.CreateCell(9).SetCellValue("Patient Share");
            row.GetCell(9).CellStyle = style1;
            row.CreateCell(10).SetCellValue("Net");
            row.GetCell(10).CellStyle = style1;
            row.CreateCell(11).SetCellValue("Facility");
            row.GetCell(11).CellStyle = style1;
            row.CreateCell(12).SetCellValue("Encounter Type");
            row.GetCell(12).CellStyle = style1;
            row.CreateCell(13).SetCellValue("Patient");
            row.GetCell(13).CellStyle = style1;
            row.CreateCell(14).SetCellValue("Eligibility ID Payer");
            row.GetCell(14).CellStyle = style1;
            row.CreateCell(15).SetCellValue("Start");
            row.GetCell(15).CellStyle = style1;
            row.CreateCell(16).SetCellValue("End");
            row.GetCell(16).CellStyle = style1;
            row.CreateCell(17).SetCellValue("Start Type");
            row.GetCell(17).CellStyle = style1;
            row.CreateCell(18).SetCellValue("End Type");
            row.GetCell(18).CellStyle = style1;
            row.CreateCell(19).SetCellValue("Diagnosis Type");
            row.GetCell(19).CellStyle = style1;
            row.CreateCell(20).SetCellValue("Diagnosis Code");
            row.GetCell(20).CellStyle = style1;
            row.CreateCell(21).SetCellValue("Start");
            row.GetCell(21).CellStyle = style1;
            row.CreateCell(22).SetCellValue("Activity Type");
            row.GetCell(22).CellStyle = style1;
            row.CreateCell(23).SetCellValue("Activity Code");
            row.GetCell(23).CellStyle = style1;
            row.CreateCell(24).SetCellValue("Quantity");
            row.GetCell(24).CellStyle = style1;
            row.CreateCell(25).SetCellValue("Activity Net");
            row.GetCell(25).CellStyle = style1;
            row.CreateCell(26).SetCellValue("Ordering Clinician");
            row.GetCell(26).CellStyle = style1;
            row.CreateCell(27).SetCellValue("Executing Clinician");
            row.GetCell(27).CellStyle = style1;
            row.CreateCell(28).SetCellValue("Prior Authorization COde");
            row.GetCell(28).CellStyle = style1;
            row.CreateCell(29).SetCellValue("Package Name");
            row.GetCell(29).CellStyle = style1;
            row.CreateCell(30).SetCellValue("Status");
            row.GetCell(30).CellStyle = style1;
            rowIndex++;

            #endregion Header row 1

            #region Data Rows

            //var listToreturn = new List<TPXMLParsedDataCustomModel>();
            using (var objBal = new TPXMLParsedDataBal())
            {
                //listToreturn = objBal.TPXMLParsedDataList(Convert.ToInt32(FileHeaderId));
                var result = objBal.GetXmlParsedData(Convert.ToInt32(FileHeaderId));
                foreach (var item in result)
                {
                    row = sheet.CreateRow(rowIndex);
                    row.CreateCell(0);
                    row.CreateCell(1);
                    row.CreateCell(2);
                    row.CreateCell(3);
                    row.CreateCell(4);
                    row.CreateCell(5);
                    row.CreateCell(6);
                    row.CreateCell(7);
                    row.CreateCell(8);
                    row.CreateCell(9);
                    row.CreateCell(10);
                    row.CreateCell(11);
                    row.CreateCell(12);
                    row.CreateCell(13);
                    row.CreateCell(14);
                    row.CreateCell(15);
                    row.CreateCell(16);
                    row.CreateCell(17);
                    row.CreateCell(18);
                    row.CreateCell(19);
                    row.CreateCell(20);
                    row.CreateCell(21);
                    row.CreateCell(22);
                    row.CreateCell(23);
                    row.CreateCell(24);
                    row.CreateCell(25);
                    row.CreateCell(26);
                    row.CreateCell(27);
                    row.CreateCell(28);
                    row.CreateCell(29);
                    row.CreateCell(30);
                    row.GetCell(0).SetCellValue(Convert.ToString(item.SystemBatchNumber));
                    row.GetCell(1).SetCellValue(Convert.ToString(item.TPFileID));
                    row.GetCell(2).SetCellValue(Convert.ToString(item.CClaimID));
                    row.GetCell(3).SetCellValue(Convert.ToString(item.BillNumber));
                    row.GetCell(4).SetCellValue(Convert.ToString(item.CMemberID));
                    row.GetCell(5).SetCellValue(Convert.ToString(item.CPayerID));
                    row.GetCell(6).SetCellValue(Convert.ToString(item.CProviderID));
                    row.GetCell(7).SetCellValue(Convert.ToString(item.CEmiratesIDNumber));
                    row.GetCell(8).SetCellValue(Convert.ToString(item.CGross));
                    row.GetCell(9).SetCellValue(Convert.ToString(item.CPatientShare));
                    row.GetCell(10).SetCellValue(Convert.ToString(item.CNet));
                    row.GetCell(11).SetCellValue(Convert.ToString(item.EFacilityID));
                    row.GetCell(12).SetCellValue(Convert.ToString(item.EncounterType));
                    row.GetCell(13).SetCellValue(Convert.ToString(item.PatientName));
                    row.GetCell(14).SetCellValue(Convert.ToString(item.EligibilityIDPayer));
                    row.GetCell(15).SetCellValue(Convert.ToString(item.EStart));
                    row.GetCell(16).SetCellValue(Convert.ToString(item.EEnd));
                    row.GetCell(17).SetCellValue(Convert.ToString(item.EncounterStartType));
                    row.GetCell(18).SetCellValue(Convert.ToString(item.EncounterEndType));
                    row.GetCell(19).SetCellValue(Convert.ToString(item.DType));
                    row.GetCell(20).SetCellValue(Convert.ToString(item.DCode));
                    row.GetCell(21).SetCellValue(Convert.ToString(item.AStart));
                    row.GetCell(22).SetCellValue(Convert.ToString(item.AType));
                    row.GetCell(23).SetCellValue(Convert.ToString(item.ACode));
                    row.GetCell(24).SetCellValue(Convert.ToString(item.AQuantity));
                    row.GetCell(25).SetCellValue(Convert.ToString(item.ANet));
                    row.GetCell(26).SetCellValue(Convert.ToString(item.AOrderingClinician));
                    row.GetCell(27).SetCellValue(Convert.ToString(item.AExecutingClinician));
                    row.GetCell(28).SetCellValue(Convert.ToString(item.APriorAuthorizationID));
                    row.GetCell(29).SetCellValue(Convert.ToString(item.CNPackageName));
                    row.GetCell(30).SetCellValue(Convert.ToString(item.PStatus));
                    rowIndex++;
                }
            }

            #endregion Data Rows

            using (var exportData = new MemoryStream())
            {
                var cookie = new HttpCookie("Downloaded", "True");
                Response.Cookies.Add(cookie);
                workbook.Write(exportData);
                var saveAsFileName = string.Format("XMLParsedData-{0:d}.xls", currentDateTime).Replace("/", "-");
                return File(exportData.ToArray(), "application/vnd.ms-excel", string.Format("{0}", saveAsFileName));
            }
        }



        private List<TPXMLParsedDataCustomModel> GetXmlParsedData(long id)
        {
            using (var bal = new TPXMLParsedDataBal())
            {
                var result = bal.GetXmlParsedData(id);
                return result;
            }
        }


        public PartialViewResult DeleteByFileIdAndGetXmlData(int fileId)
        {
            using (var bal = new TPXMLParsedDataBal())
            {
                var result = bal.DeleteAndThenGetXmlFileData(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), fileId, withDetails: true);
                return PartialView(PartialViews.XMLBillFile, result);
            }
        }


        public JsonResult ExecuteXmlDetails(int fileId)
        {
            using (var bal = new TPXMLParsedDataBal())
            {
                var result = bal.ExecuteXmlFileDetails(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), fileId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion XML Billing Methods

        #region Online Help

        /// <summary>
        /// Called when [help].
        /// </summary>
        /// <returns></returns>
        public ActionResult OnlineHelp()
        {
            using (var bal = new DocumentsTemplatesBal())
            {
                var associatedTypeId = Convert.ToInt32(AttachmentType.OnlineHelp);
                var docTypeId = (int)DocumentTemplateTypes.OnlineHelp;
                var view = new FileUploaderView
                {
                    Attachments = bal.GetListByAssociateType(associatedTypeId),
                    Model = new DocumentsTemplatesCustomModel
                    {
                        IsDeleted = false,
                        IsRequired = false,
                        IsTemplate = false,
                        AssociatedType = (int)AttachmentType.OnlineHelp,
                        AssociatedID = (int)GlobalCodeCategoryValue.OnlineHelp,
                        DocumentTypeID = docTypeId
                    }
                };
                return View(view);
            }
        }

        /// <summary>
        /// Uploads the online help files.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult UploadOnlineHelpFiles(DocumentsTemplatesCustomModel model)
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                var associatedId = Convert.ToInt32(GlobalCodeCategoryValue.OnlineHelp);
                var associatedTypeId = Convert.ToInt32(DocAssociatedType.OnlineHelp);
                var docTypeId = (int)DocumentTemplateTypes.OnlineHelp;

                using (var bal = new DocumentsTemplatesBal())
                {
                    if (ModelState.IsValid)
                    {
                        //TO:DO
                        var fileName = Path.GetFileName(model.File.FileName);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            var dirPath = Server.MapPath("~/" + CommonConfig.OnlineHelpFilePath);
                            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

                            var path = Path.Combine(dirPath, fileName);
                            var dbFilePath = CommonConfig.OnlineHelpFilePath + fileName;
                            model.File.SaveAs(path);
                            var currentModel = new DocumentsTemplates
                            {
                                FileName = fileName,
                                FilePath = dbFilePath,
                                DocumentTypeID = docTypeId,
                                DocumentName = model.DocumentName,
                                DocumentNotes = model.DocumentNotes,
                                DocumentsTemplatesID = 0,
                                AssociatedID = associatedId,
                                AssociatedType = associatedTypeId,
                                ExternalValue1 = model.ExternalValue1,
                                ExternalValue2 = model.ExternalValue2,
                                CorporateID = Helpers.GetDefaultCorporateId(),
                                FacilityID = Helpers.GetDefaultFacilityId(),
                                IsDeleted = false,
                                IsRequired = false,
                                IsTemplate = false,
                                CreatedBy = Helpers.GetLoggedInUserId(),
                                CreatedDate = Helpers.GetInvariantCultureDateTime()
                            };

                            bal.AddUpdateDocumentTempate(currentModel);
                            ViewBag.Message = "File has been uploaded successfully";
                            ModelState.Clear();
                        }
                    }
                }
            }
            return RedirectToAction("OnlineHelp");
        }

        /// <summary>
        /// Deletes the document template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="associatedTypeId">The associated type identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteDocumentTemplate(int id, int associatedTypeId)
        {
            using (var documentTemplateBal = new DocumentsTemplatesBal())
            {
                //Get facility model object by current facility ID
                var currentFacility = documentTemplateBal.GetDocumentById(id);

                //Check If facility model is not null
                if (currentFacility != null)
                {
                    currentFacility.IsDeleted = true;
                    currentFacility.DeletedBy = Helpers.GetLoggedInUserId();
                    currentFacility.DeletedDate = Helpers.GetInvariantCultureDateTime();
                    //Update Operation of current facility
                    var result = documentTemplateBal.AddUpdateDocumentTempate(currentFacility);

                    var list = documentTemplateBal.GetListByAssociateType(associatedTypeId);
                    return PartialView(PartialViews.OnlineHelpDocsListView, list);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        #endregion Online Help
    }
}