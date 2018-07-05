using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;


namespace BillingSystem.Controllers
{

    /// <summary>
    ///     The bill header controller.
    /// </summary>
    public class BillHeaderController : BaseController
    {
        private readonly IBillHeaderService _service;
        private readonly IFacilityService _fService;
        private readonly IBillActivityService _baService;
        private readonly IPatientInfoService _piService;
        private readonly IEncounterService _eService;
        private readonly IErrorMasterService _erService;
        private readonly IXMLBillingService _xService;

        public BillHeaderController(IBillHeaderService service, IFacilityService fService, IBillActivityService baService, IPatientInfoService piService, IEncounterService eService, IErrorMasterService erService, IXMLBillingService xService)
        {
            _service = service;
            _fService = fService;
            _baService = baService;
            _piService = piService;
            _eService = eService;
            _erService = erService;
            _xService = xService;
        }


        #region Public Methods and Operators

        /// <summary>
        ///     Indexes the specified identifier.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <param name="typeId">
        ///     The type identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult Index(int? id, int? typeId)
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();

            BillDetailsView billDetailsView;
            if (id == null || typeId == null || Convert.ToInt32(id) == 0)
            {
                var billHeaderList = _service.GetAllBillHeaderList(corporateId, facilityId);

                // Bill Details ViewModel to be binded to UI
                billDetailsView = new BillDetailsView
                {
                    PatientInfo = new PatientInfoCustomModel(),
                    BillHeaderList = billHeaderList,
                    BillActivityList = new List<BillDetailCustomModel>(),
                    EncounterId = 0,
                    QueryStringId = 0,
                    QueryStringTypeId = 0
                };

            }
            else
            {
                billDetailsView = GetBillHeaderDetailsView(
                    Convert.ToInt32(id),
                    Convert.ToString(typeId),
                    corporateId,
                    facilityId);
            }

            // Pass the View Model in ActionResult to View BillHeader
            return View(billDetailsView);
        }

        /// <summary>
        ///     Finals the bills ListView.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <param name="typeId">
        ///     The type identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        /// <summary>
        ///     Applies the scrub bill.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ApplyScrubBill()
        {
            return Json(null);
        }

        /// <summary>
        ///     The get bill activities by bill header id.
        /// </summary>
        /// <param name="billHeaderId">
        ///     The bill header id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetBillActivitiesByBillHeaderId(int billHeaderId)
        {
            var list = _baService.GetBillActivitiesByBillHeaderId(billHeaderId);
            return PartialView(PartialViews.BillActivityList, list);
        }

        /// <summary>
        ///     The get bill header by id.
        /// </summary>
        /// <param name="billHeaderId">
        ///     The bill header id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetBillHeaderById(int billHeaderId)
        {
            var billHeader = _service.GetBillHeaderById(billHeaderId);
            return PartialView(PartialViews.BillHeaderAddEdit, billHeader);

        }

        /// <summary>
        ///     Gets the bill header details.
        /// </summary>
        /// <param name="billHeaderId">
        ///     The bill header identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetBillHeaderDetails(int billHeaderId)
        {
            var billheaderObj = _service.GetBillHeaderToUpdateById(billHeaderId);
            return Json(billheaderObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Gets the claim identifier for bill.
        /// </summary>
        /// <param name="billHeaderId">
        ///     The bill header identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetClaimIdForBill(int billHeaderId)
        {
            var billclaimlist = _service.GetBillHeadersByBillId(billHeaderId);
            return Json(billclaimlist);
        }

        /// <summary>
        ///     Gets the diagnosis codes.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <returns>
        ///     The <see cref="JsonResult" />.
        /// </returns>
        public JsonResult GetDenialCodes(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var list = _erService.GetSearchedDenialsList(text);
                var filteredList =
                    list.Select(
                        item =>
                        new
                        {
                            ID = item.ErrorMasterID,
                            Menu_Title = string.Format("{0} - {1}", item.ErrorCode, item.ErrorDescription),
                            Name = item.ErrorCode
                        }).ToList();

                return Json(filteredList, JsonRequestBehavior.AllowGet);
            }

            return Json(null);
        }

        /// <summary>
        ///     Gets the pre XML file.
        /// </summary>
        /// <param name="billHeaderId">
        ///     The bill header identifier.
        /// </param>
        /// <param name="facilityId">
        ///     The facility identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetPreXMLFile(int billHeaderId, int facilityId)
        {
            var billheaderObj = _service.GetPreXMLFile(billHeaderId, facilityId);
            return PartialView(PartialViews.PreXMLFileList, billheaderObj);
        }

        /// <summary>
        ///     Determines whether [is authorized encounter] [the specified encounter identifier].
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult IsAuthorizedEncounter(int encounterId)
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var result =
                _service.GetBillHeaderListByEncounterId(encounterId, corporateId, facilityId)
                    .OrderByDescending(x => x.BillHeaderID)
                    .FirstOrDefault();
            if (result != null && result.AuthID != null && result.MCID != null)
            {
                return Json(true);
            }

            return Json(false);
        }

        /// <summary>
        ///     The manual payment.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter id.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ManualPayment(int? encounterId, string message)
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = "1";
            }

            if (encounterId == null || (int)encounterId == 0)
            {
                return RedirectToAction(
                    ActionResults.activeEncounterDefaultAction,
                    ControllerNames.activeEncounterController,
                    new { messageId = Convert.ToInt32(MessageType.ViewBillingHeader) });
            }


            var eId = Convert.ToInt32(encounterId);

            // Get Patient Details by current Encounter ID
            var patientId = _piService.GetPatientIdByEncounterId(eId);
            var patientInfo = _piService.GetPatientDetailsByPatientId(patientId);

            // Bill Details ViewModel to be binded to UI
            var billDetailsView = new BillDetailsView
            {
                PatientInfo = patientInfo,
                BillHeaderList =
                    _service.GetBillHeaderListByEncounterId(
                                                  eId,
                                                  corporateId,
                                                  facilityId),

                // BillActivityList = new List<BillActivityCustomModel>(),
                EncounterId = eId
            };

            // Pass the View Model in ActionResult to View BillHeader
            return View(billDetailsView);

        }

        /// <summary>
        ///     Refreshes the bill charges.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <param name="applyBedCharges">
        ///     if set to <c>true</c> [apply bed charges].
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult RefreshBillCharges(int encounterId, bool applyBedCharges)
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();

            // Commented by Shashank on Sept 15 2015
            // if (applyBedCharges)
            // bal.ApplyBedChargesAndOrderBill(encounterId);
            var list = _service.GetBillHeaderListByEncounterId(encounterId, corporateId, facilityId);
            return PartialView(PartialViews.BillHeaderList, list);
        }

        /// <summary>
        ///     The refresh bill header list.
        /// </summary>
        /// <param name="billHeaderId">
        ///     The bill header id.
        /// </param>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult RefreshBillHeaderList(int billHeaderId, int patientId, int encounterId)
        {
            var isEncounterSelected = patientId <= 0 && encounterId > 0;
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            if (encounterId == 0)
            {
                var billHeaderDetail = _service.GetBillHeaderById(billHeaderId);
                encounterId = Convert.ToInt32(billHeaderDetail.EncounterID);
            }

            // var result = bal.UpdateBillHeadersByEncounterId(encounterId, patientId, isEncounterSelected);
            // var result = bal.UpdateBillHeadersByEncounterId(encounterId, patientId, isEncounterSelected, corporateId, facilityId);
            var result = _service.UpdateBillHeaderByBillHeaderEncounterId(
                encounterId,
                billHeaderId,
                isEncounterSelected,
                patientId,
                corporateId,
                facilityId);
            return PartialView(PartialViews.BillHeaderList, result);
        }

        /// <summary>
        ///     The save bill header details.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SaveBillHeaderDetails(BillHeader model)
        {
            var result = -1;
            if (model.BillHeaderID > 0)
            {
                var current = _service.GetBillHeaderToUpdateById(model.BillHeaderID);
                current.DenialCode = model.DenialCode;
                current.PaymentReference = model.PaymentReference;
                current.DateSettlement = model.DateSettlement;
                current.PaymentAmount = model.PaymentAmount;
                current.PatientPayReference = model.PatientPayReference;
                current.PatientDateSettlement = model.PatientDateSettlement;
                current.PatientPayAmount = model.PatientPayAmount;
                current.ModifiedBy = Helpers.GetLoggedInUserId();
                current.ModifiedDate = _fService.GetInvariantCultureDateTime(Convert.ToInt32(model.FacilityID));
                result = _service.SaveManualPayment(current);

            }

            return Json(result);
        }

        /// <summary>
        ///     Scrubs the XML bill.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ScrubXMLBill(int encounterId)
        {
            var userId = Helpers.GetLoggedInUserId();

            var billheaderObj = _eService.GetEncounterEndCheck(encounterId, userId);
            return Json(billheaderObj, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        ///     Updates the bill header status.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        /// <summary>
        ///     Sets the bill header status for pre bill.
        /// </summary>
        /// <param name="billHeaderIds">
        ///     The bill header ids.
        /// </param>
        /// <param name="oldStatus">
        ///     The old status.
        /// </param>
        /// <param name="typeId">
        ///     The type identifier.
        /// </param>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SetBillHeaderStatusForPreBill(string billHeaderIds, string oldStatus, int typeId, int id)
        {
            var userId = Helpers.GetLoggedInUserId();
            var list = new List<BillHeaderCustomModel>();
            if (!string.IsNullOrEmpty(billHeaderIds))
            {
                var corporateId = Helpers.GetDefaultCorporateId();
                var facilityId = Helpers.GetDefaultFacilityId();

                var lstBhIds = billHeaderIds.Split(',').Select(int.Parse).ToList();

                var encounteridlist =
                    lstBhIds.Select(_service.GetBillHeaderById)
                        .Select(billHeaderObj => Convert.ToInt32(billHeaderObj.EncounterID))
                        .ToList();
                var preliminaryStatus = _service.SetPreliminaryBillStatusByEncounterId(encounteridlist, userId);

                if (typeId == Convert.ToInt32(QueryStringType.Encounter))
                {
                    list = _service.GetBillHeaderListByEncounterId(id, corporateId, facilityId);
                }
                else if (typeId == Convert.ToInt32(QueryStringType.Patient))
                {
                    list = _service.GetBillHeaderListByPatientId(id, corporateId, facilityId);
                }
                else
                {
                    list = _service.GetAllBillHeaderList(corporateId, facilityId);
                }

                return PartialView(PartialViews.BillHeaderList, list);

            }

            return PartialView(PartialViews.BillHeaderList, list);
        }

        /// <summary>
        ///     The upload remittance xml.
        /// </summary>
        /// <param name="xmlFile">
        ///     The xml file.
        /// </param>
        /// <param name="EncounterID">
        ///     The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult UploadRemittanceXml(HttpPostedFileBase xmlFile, int EncounterID)
        {
            if (xmlFile != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var loggedInUserId = Helpers.GetLoggedInUserId();
                var fileN = Path.GetFileName(xmlFile.FileName);
                var savedFileName = string.Format(
                    CommonConfig.RemittanceAdviceXmlFilePath,
                    corporateId,
                    facilityId,
                    loggedInUserId);
                var completePath = Server.MapPath(savedFileName);
                if (!Directory.Exists(completePath))
                {
                    Directory.CreateDirectory(completePath);
                }

                savedFileName = savedFileName + fileN;
                xmlFile.SaveAs(completePath + fileN);

                var xml = Helpers.GetXML(completePath);
                if (!string.IsNullOrEmpty(xml))
                {
                    var result = _xService.RemittanceXMLParser(xml, savedFileName, true, corporateId, facilityId);
                    var msg = string.IsNullOrEmpty(result) || result.Equals("1") ? "1" : result;
                    //if (result)
                    //{
                    return RedirectToAction("ManualPayment", new { message = msg, encounterId = EncounterID });
                    //}
                }
            }

            return RedirectToAction("ManualPayment", new { encounterId = EncounterID });
        }

        /// <summary>
        ///     es the claim.
        /// </summary>
        /// <returns></returns>
        public ActionResult EClaim()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            BillDetailsView billDetailsView = null;
            ViewBag.ShowPatientHeader = false;

            var billHeaderList = _service.GetFinalBillHeadersList(0, 0, false, corporateId, facilityId);
            var payerWiseBillHeaderList = _service.GetFinalBillPayerHeadersList(corporateId, facilityId);

            // Bill Details ViewModel to be binded to UI
            billDetailsView = new BillDetailsView
            {
                PatientInfo = new PatientInfoCustomModel(),
                BillHeaderList = billHeaderList,
                BillActivityList = new List<BillDetailCustomModel>(),
                EncounterId = 0,
                QueryStringId = 0,
                QueryStringTypeId = 0,
                PayerWiseBillHeaderList = payerWiseBillHeaderList
            };


            // Pass the View Model in ActionResult to View BillHeader
            return View(billDetailsView);
        }

        /// <summary>
        ///     Sends the e claims by payer ids.
        /// </summary>
        /// <param name="payerId">The payer identifier.</param>
        /// <param name="billHeaderIds">The bill header ids.</param>
        /// <returns></returns>
        public ActionResult SendEClaimsByPayerIds(string payerId, string billHeaderIds)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var data = _service.SendEClaimsByPayer(Helpers.GetDefaultFacilityId(), payerId, billHeaderIds);
            var xmlFileId = _xService.GetLatestXFileHeaderId();
            var filePath = string.Format(
                "{0}\\Content\\Documents\\Corporate{1}\\{2}",
                Server.MapPath("~"),
                corporateId,
                xmlFileId);

            var fileIsExists = Directory.Exists(filePath);
            if (fileIsExists)
            {
                Directory.Delete(filePath, true);
                var dir = new DirectoryInfo(filePath);
                dir.Refresh();
            }

            // Call the AddXFileHeader Method to Add / Update current XFileHeader
            var billHeaderXmlModel = data.FirstOrDefault();
            if (billHeaderXmlModel != null) XmlParser.SaveStringToXMLFile(filePath, billHeaderXmlModel.XMLOUT, Convert.ToString(xmlFileId));

            var xmlString = string.Format("/Content/Documents/Corporate{0}/{1}/{2}.xml", Helpers.GetDefaultCorporateId(),
                xmlFileId, xmlFileId);

            var getXfileHeader = _xService.GetXFileHeaderByID(xmlFileId);
            if (getXfileHeader != null)
            {
                getXfileHeader.XPath = xmlString;
                _xService.AddUptdateXFileHeader(getXfileHeader);
            }
            var facilityId = Helpers.GetDefaultFacilityId();

            var billHeaderListView = string.Empty;
            //var billHeaderList = bal.GetFinalBillByPayerHeadersList(corporateId, Helpers.GetDefaultFacilityId(), payerId);
            var billHeaderList = _service.GetFinalBillHeadersList(0, 0, false, corporateId, facilityId);
            var opPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("OutPatient")).ToList();
            var inPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("InPatient")).ToList();
            var erPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("ERPatient")).ToList();

            var payerClaimsList = _service.GetFinalBillPayerHeadersList(corporateId, Helpers.GetDefaultFacilityId());
            var payerClaimsView = RenderPartialViewToString(PartialViews.PayerClaimsList, payerClaimsList);
            var inPatientView = RenderPartialViewToString(PartialViews.InPatientFinalBillList, inPatientList);
            var outPatientView = RenderPartialViewToString(PartialViews.OutPatientFinalBillList, opPatientList);

            var erPatientView = RenderPartialViewToString(PartialViews.ErPatientFinalBillList, erPatientList);

            var jsonData = new { payerClaimsView, inPatientView, outPatientView, erPatientView };
            var datatoReturn = Json(jsonData, JsonRequestBehavior.AllowGet);
            datatoReturn.MaxJsonLength = int.MaxValue;
            return datatoReturn;

        }

        /// <summary>
        ///     Sorts the bill header grid.
        /// </summary>
        /// <returns></returns>
        public ActionResult SortBillHeaderGrid()
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var listToReturn = new List<BillHeaderCustomModel>();

            listToReturn = _service.GetAllBillHeaderList(corporateId, facilityId);

            return PartialView(PartialViews.BillHeaderList, listToReturn);
        }

        #endregion

        #region Review Final Bills For Claim Submissions

        public ActionResult GetBillPayerHeadersList()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();

            var billHeaderList = _service.GetFinalBillHeadersList(0, 0, false, corporateId, facilityId);

            var opPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("OutPatient")).ToList();
            var inPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("InPatient")).ToList();
            var erPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("ERPatient")).ToList();
            var payerClaimsList = _service.GetFinalBillPayerHeadersList(corporateId, Helpers.GetDefaultFacilityId());

            var inPatientView = RenderPartialViewToString(PartialViews.InPatientFinalBillList, inPatientList);
            var outPatientView = RenderPartialViewToString(PartialViews.OutPatientFinalBillList, opPatientList);

            var erPatientView = RenderPartialViewToString(PartialViews.ErPatientFinalBillList, erPatientList);
            //var billHeaderPatialView = RenderPartialViewToString(PartialViews.FinalBillHeadersListView,
            //    billHeaderList);
            var payerclaimPartialView = RenderPartialViewToString(
                PartialViews.PayerClaimsList,
                payerClaimsList);

            var jsonData = new { outPatientView, inPatientView, erPatientView, payerclaimPartialView };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetFinalBillByPayerHeadersList(string payerId)
        {
            var inPatientView = "";
            var outPatientView = "";
            var erPatientView = "";
            List<BillHeaderCustomModel> billHeaderList = null;

            billHeaderList = !string.IsNullOrEmpty(payerId) ?
                   _service.GetFinalBillByPayerHeadersList(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), payerId) :
                   _service.GetFinalBillHeadersList(0, 0, false, Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId());

            var opPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("OutPatient")).ToList();
            var inPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("InPatient")).ToList();
            var erPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("ERPatient")).ToList();
            inPatientView = RenderPartialViewToString(PartialViews.InPatientFinalBillList, inPatientList);
            outPatientView = RenderPartialViewToString(PartialViews.OutPatientFinalBillList, opPatientList);

            erPatientView = RenderPartialViewToString(PartialViews.ErPatientFinalBillList, erPatientList);
            var jsonData = new { inPatientView, outPatientView, erPatientView };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        public ActionResult FinalBillsListView(int? id, int? typeId)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            BillDetailsView billDetailsView = null;
            if (id == null || typeId == null || Convert.ToInt32(id) == 0)
            {
                ViewBag.ShowPatientHeader = false;

                var billHeaderList = _service.GetFinalBillHeadersList(0, 0, false, corporateId, facilityId);

                /* Who: Krishna
                 When: 12-April-2016
                 What: To get 3 different List (In/Op/Er)
                 Why: Clicent Requriment Task Id On Tms: XXXXX*/
                var opPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("OutPatient")).ToList();
                var inPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("InPatient")).ToList();
                var erPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("ERPatient")).ToList();

                var payerWiseBillHeaderList = _service.GetFinalBillPayerHeadersList(corporateId, facilityId);

                // Bill Details ViewModel to be binded to UI
                billDetailsView = new BillDetailsView
                {
                    PatientInfo = new PatientInfoCustomModel(),
                    //BillHeaderList = billHeaderList,
                    InPatientListView = inPatientList,
                    OutPatientListView = opPatientList,
                    ErPatientListView = erPatientList,
                    BillActivityList = new List<BillDetailCustomModel>(),
                    EncounterId = 0,
                    QueryStringId = 0,
                    QueryStringTypeId = 0,
                    PayerWiseBillHeaderList = payerWiseBillHeaderList
                };

            }

            // Pass the View Model in ActionResult to View BillHeader
            return View(billDetailsView);
        }

        /// <summary>
        ///     Gets the claims by payer identifier.
        /// </summary>
        /// <param name="payerid">The payerid.</param>
        /// <returns></returns>
        public ActionResult GetClaimsByPayerId(string payerid)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var billHeaderList = _service.GetFinalBillByPayerHeadersList(corporateId, facilityId, payerid);
            return PartialView(PartialViews.FinalBillHeadersListView, billHeaderList);

        }

        public ActionResult GetClaimsByPayerIdView(string payerIds)
        {
            var mainList = _service.GetFinalBillByPayerHeadersList(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), payerIds);
            var opPatientList = mainList.Where(x => x.EncounterPatientType.Equals("OutPatient")).ToList();
            var inPatientList = mainList.Where(x => x.EncounterPatientType.Equals("InPatient")).ToList();
            var erPatientList = mainList.Where(x => x.EncounterPatientType.Equals("ERPatient")).ToList();

            var inPatientView = RenderPartialViewToString(PartialViews.InPatientFinalBillList, inPatientList);
            var outPatientView = RenderPartialViewToString(PartialViews.OutPatientFinalBillList, opPatientList);

            var erPatientView = RenderPartialViewToString(PartialViews.ErPatientFinalBillList, erPatientList);

            var jsonData = new { inPatientView, outPatientView, erPatientView };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the bill header details view.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <param name="typeId">
        ///     The type identifier.
        /// </param>
        /// <param name="corporateId">
        ///     The corporate identifier.
        /// </param>
        /// <param name="facilityId">
        ///     The facility identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="BillDetailsView" />.
        /// </returns>
        private BillDetailsView GetBillHeaderDetailsView(int id, string typeId, int corporateId, int facilityId)
        {
            var patientInfo = new PatientInfoCustomModel();
            var billHeaderList = new List<BillHeaderCustomModel>();
            var queryStringType = (QueryStringType)Enum.Parse(typeof(QueryStringType), typeId);
            var encounterId = 0;
            var patientId = 0;
            switch (queryStringType)
            {
                case QueryStringType.Encounter:


                    encounterId = id;

                    // Call Stored Procedures here to apply charges of Bed and Order Bill against the current Encounter ID
                    // Commented by Shashank On 15 Sept 2015
                    // bal.ApplyBedChargesAndOrderBill(id);

                    // Get Patient Details by current Encounter ID
                    patientId = _piService.GetPatientIdByEncounterId(id);
                    patientInfo = _piService.GetPatientDetailsByPatientId(Convert.ToInt32(patientId));

                    billHeaderList = _service.GetBillHeaderListByEncounterId(id, corporateId, facilityId);


                    break;
                case QueryStringType.Patient:

                    #region Patient Type


                    // Call Stored Procedures here to apply charges of Bed and Order Bill against the current Encounter ID
                    var encountersList = _eService.GetActiveEncounterIdsByPatientId(id);

                    // Apply Bed Charges to each encounter of Patient ID
                    // Commented by Shashank On 15 Sept 2015
                    // if (encountersList.Count > 0)
                    // {
                    // encountersList = encountersList.Distinct().ToList();
                    // encountersList.ForEach(item => bal.ApplyBedChargesAndOrderBill(item));
                    // }

                    // Get Patient Details by current Encounter ID
                    patientInfo = _piService.GetPatientDetailsByPatientId(id);

                    billHeaderList = _service.GetBillHeaderListByPatientId(id, corporateId, facilityId);


                    #endregion

                    break;
            }

            // Bill Details ViewModel to be binded to UI
            var billDetailsView = new BillDetailsView
            {
                PatientInfo = patientInfo,
                BillHeaderList = billHeaderList,
                BillActivityList = new List<BillDetailCustomModel>(),
                EncounterId = encounterId,
                QueryStringId = id,
                QueryStringTypeId = Convert.ToInt32(typeId),
                PatientId = patientId
            };

            return billDetailsView;
        }

        /// <summary>
        ///     Renders the partial view to string.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        private string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName)) viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion


        public JsonResult GetTableNumbers(string typeId)
        {
            var tn = _baService.GetTableNumbersList(typeId);
            return Json(tn, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CheckForDuplicateTableSet(string tableNumber, string typeId, int id)
        {
            var isExists = false;
            isExists = _baService.CheckForDuplicateTableSet(id, tableNumber, typeId);

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }
    }
}