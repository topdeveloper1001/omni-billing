// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BillHeaderController.cs" company="Spadez">
//   Omnihelathcare
// </copyright>
// <summary>
//   The bill header controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    ///     The bill header controller.
    /// </summary>
    public class BillHeaderController : BaseController
    {
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
                // ViewBag.ShowPatientHeader = false;
                using (var bal = new BillHeaderBal())
                {
                    // ......Commented by Shashank On 15 Sept -2015

                    // var encountersList = bal.GetAllEncounterIdsInBillHeader();
                    ////Apply Bed Charges to each encounter of each Patient of Bill Header List
                    // encountersList.ForEach(item => bal.ApplyBedChargesAndOrderBill(item));

                    // ......Commented by Shashank On 15 Sept -2015
                    var billHeaderList = bal.GetAllBillHeaderList(corporateId, facilityId);

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
            using (
                var bal = new BillActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list = bal.GetBillActivitiesByBillHeaderId(billHeaderId);
                return PartialView(PartialViews.BillActivityList, list);
            }
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
            using (var bal = new BillHeaderBal())
            {
                var billHeader = bal.GetBillHeaderById(billHeaderId);
                return PartialView(PartialViews.BillHeaderAddEdit, billHeader);
            }
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
            using (var billheaderBal = new BillHeaderBal())
            {
                var billheaderObj = billheaderBal.GetBillHeaderToUpdateById(billHeaderId);
                return Json(billheaderObj, JsonRequestBehavior.AllowGet);
            }
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
            using (var billheaderBal = new BillHeaderBal())
            {
                var billclaimlist = billheaderBal.GetBillHeadersByBillId(billHeaderId);
                return Json(billclaimlist);
            }
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
                using (var bal = new ErrorMasterBal())
                {
                    var list = bal.GetSearchedDenialsList(text);
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
            using (var billheaderBal = new BillHeaderBal())
            {
                var billheaderObj = billheaderBal.GetPreXMLFile(billHeaderId, facilityId);
                return PartialView(PartialViews.PreXMLFileList, billheaderObj);
            }
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
            using (var bal = new BillHeaderBal())
            {
                var corporateId = Helpers.GetDefaultCorporateId();
                var facilityId = Helpers.GetDefaultFacilityId();
                var result =
                    bal.GetBillHeaderListByEncounterId(encounterId, corporateId, facilityId)
                        .OrderByDescending(x => x.BillHeaderID)
                        .FirstOrDefault();
                if (result != null && result.AuthID != null && result.MCID != null)
                {
                    return Json(true);
                }

                return Json(false);
            }
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

            using (
                var bal = new BillHeaderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var eId = Convert.ToInt32(encounterId);

                // Get Patient Details by current Encounter ID
                var patientId = bal.GetPatientIdByEncounterId(eId);
                var patientInfo = bal.GetPatientDetailsByPatientId(patientId);

                // Bill Details ViewModel to be binded to UI
                var billDetailsView = new BillDetailsView
                {
                    PatientInfo = patientInfo,
                    BillHeaderList =
                                                  bal.GetBillHeaderListByEncounterId(
                                                      eId,
                                                      corporateId,
                                                      facilityId),

                    // BillActivityList = new List<BillActivityCustomModel>(),
                    EncounterId = eId
                };

                // Pass the View Model in ActionResult to View BillHeader
                return View(billDetailsView);
            }
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
            using (var bal = new BillHeaderBal())
            {
                var corporateId = Helpers.GetDefaultCorporateId();
                var facilityId = Helpers.GetDefaultFacilityId();

                // Commented by Shashank on Sept 15 2015
                // if (applyBedCharges)
                // bal.ApplyBedChargesAndOrderBill(encounterId);
                var list = bal.GetBillHeaderListByEncounterId(encounterId, corporateId, facilityId);
                return PartialView(PartialViews.BillHeaderList, list);
            }
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
            using (var bal = new BillHeaderBal())
            {
                if (encounterId == 0)
                {
                    var billHeaderDetail = bal.GetBillHeaderById(billHeaderId);
                    encounterId = Convert.ToInt32(billHeaderDetail.EncounterID);
                }

                // var result = bal.UpdateBillHeadersByEncounterId(encounterId, patientId, isEncounterSelected);
                // var result = bal.UpdateBillHeadersByEncounterId(encounterId, patientId, isEncounterSelected, corporateId, facilityId);
                var result = bal.UpdateBillHeaderByBillHeaderEncounterId(
                    encounterId,
                    billHeaderId,
                    isEncounterSelected,
                    patientId,
                    corporateId,
                    facilityId);
                return PartialView(PartialViews.BillHeaderList, result);
            }
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
                using (var bal = new BillHeaderBal())
                {
                    var current = bal.GetBillHeaderToUpdateById(model.BillHeaderID);
                    current.DenialCode = model.DenialCode;
                    current.PaymentReference = model.PaymentReference;
                    current.DateSettlement = model.DateSettlement;
                    current.PaymentAmount = model.PaymentAmount;
                    current.PatientPayReference = model.PatientPayReference;
                    current.PatientDateSettlement = model.PatientDateSettlement;
                    current.PatientPayAmount = model.PatientPayAmount;
                    current.ModifiedBy = Helpers.GetLoggedInUserId();
                    current.ModifiedDate = Helpers.GetInvariantCultureDateTime(Convert.ToInt32(model.FacilityID));
                    result = bal.SaveManualPayment(current);
                }
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
            using (var encounterBal = new EncounterBal())
            {
                var billheaderObj = encounterBal.GetEncounterEndCheck(encounterId, userId);
                return Json(billheaderObj, JsonRequestBehavior.AllowGet);
            }
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
                using (var bal = new BillHeaderBal())
                {
                    var lstBhIds = billHeaderIds.Split(',').Select(int.Parse).ToList();

                    // var gcBal = new GlobalCodeBal();
                    // var gc = gcBal.GetGlobalCodeByCategoryValueAndGlobalCodeName(Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString(CultureInfo.InvariantCulture), BillHeaderStatus.P.ToString());
                    // bal.SetBillHeaderStatus(lstBhIds, gc.GlobalCodeValue, oldStatus);
                    var encounteridlist =
                        lstBhIds.Select(bal.GetBillHeaderById)
                            .Select(billHeaderObj => Convert.ToInt32(billHeaderObj.EncounterID))
                            .ToList();
                    var preliminaryStatus = bal.SetPreliminaryBillStatusByEncounterId(encounteridlist, userId);

                    if (typeId == Convert.ToInt32(QueryStringType.Encounter))
                    {
                        list = bal.GetBillHeaderListByEncounterId(id, corporateId, facilityId);
                    }
                    else if (typeId == Convert.ToInt32(QueryStringType.Patient))
                    {
                        list = bal.GetBillHeaderListByPatientId(id, corporateId, facilityId);
                    }
                    else
                    {
                        list = bal.GetAllBillHeaderList(corporateId, facilityId);
                    }

                    return PartialView(PartialViews.BillHeaderList, list);
                }
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
                    using (var bal = new XMLBillingBal())
                    {
                        var result = bal.RemittanceXMLParser(xml, savedFileName, true, corporateId, facilityId);
                        var msg = string.IsNullOrEmpty(result) || result.Equals("1") ? "1" : result;
                        //if (result)
                        //{
                        return RedirectToAction("ManualPayment", new { message = msg, encounterId = EncounterID });
                        //}
                    }
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
            using (var bal = new BillHeaderBal())
            {
                var billHeaderList = bal.GetFinalBillHeadersList(0, 0, false, corporateId, facilityId);
                var payerWiseBillHeaderList = bal.GetFinalBillPayerHeadersList(corporateId, facilityId);

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
            }

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
            using (var bal = new BillHeaderBal())
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var data = bal.SendEClaimsByPayer(Helpers.GetDefaultFacilityId(), payerId, billHeaderIds);
                using (var xmlBilling = new XMLBillingBal())
                {
                    var xmlFileId = xmlBilling.GetLatestXFileHeaderId();
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

                    var getXfileHeader = xmlBilling.GetXFileHeaderByID(xmlFileId);
                    if (getXfileHeader != null)
                    {
                        getXfileHeader.XPath = xmlString;
                        xmlBilling.AddUptdateXFileHeader(getXfileHeader);
                    }
                }
                var facilityId = Helpers.GetDefaultFacilityId();

                var billHeaderListView = string.Empty;
                //var billHeaderList = bal.GetFinalBillByPayerHeadersList(corporateId, Helpers.GetDefaultFacilityId(), payerId);
                var billHeaderList = bal.GetFinalBillHeadersList(0, 0, false, corporateId, facilityId);
                var opPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("OutPatient")).ToList();
                var inPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("InPatient")).ToList();
                var erPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("ERPatient")).ToList();

                var payerClaimsList = bal.GetFinalBillPayerHeadersList(corporateId, Helpers.GetDefaultFacilityId());
                var payerClaimsView = RenderPartialViewToString(PartialViews.PayerClaimsList, payerClaimsList);
                var inPatientView = RenderPartialViewToString(PartialViews.InPatientFinalBillList, inPatientList);
                var outPatientView = RenderPartialViewToString(PartialViews.OutPatientFinalBillList, opPatientList);

                var erPatientView = RenderPartialViewToString(PartialViews.ErPatientFinalBillList, erPatientList);

                var jsonData = new { payerClaimsView, inPatientView, outPatientView, erPatientView };
                var datatoReturn = Json(jsonData, JsonRequestBehavior.AllowGet);
                datatoReturn.MaxJsonLength = int.MaxValue;
                return datatoReturn;
            }
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
            using (var bal = new BillHeaderBal())
            {
                listToReturn = bal.GetAllBillHeaderList(corporateId, facilityId);
            }
            return PartialView(PartialViews.BillHeaderList, listToReturn);
        }

        #endregion

        #region Review Final Bills For Claim Submissions

        public ActionResult GetBillPayerHeadersList()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            using (var bal = new BillHeaderBal())
            {
                var billHeaderList = bal.GetFinalBillHeadersList(0, 0, false, corporateId, facilityId);

                var opPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("OutPatient")).ToList();
                var inPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("InPatient")).ToList();
                var erPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("ERPatient")).ToList();
                var payerClaimsList = bal.GetFinalBillPayerHeadersList(corporateId, Helpers.GetDefaultFacilityId());

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
        }

        public ActionResult GetFinalBillByPayerHeadersList(string payerId)
        {
            var inPatientView = "";
            var outPatientView = "";
            var erPatientView = "";
            List<BillHeaderCustomModel> billHeaderList = null;

            using (var bal = new BillHeaderBal())
            {
                billHeaderList = !string.IsNullOrEmpty(payerId) ?
                       bal.GetFinalBillByPayerHeadersList(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), payerId) :
                       bal.GetFinalBillHeadersList(0, 0, false, Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId());

                var opPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("OutPatient")).ToList();
                var inPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("InPatient")).ToList();
                var erPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("ERPatient")).ToList();
                inPatientView = RenderPartialViewToString(PartialViews.InPatientFinalBillList, inPatientList);
                outPatientView = RenderPartialViewToString(PartialViews.OutPatientFinalBillList, opPatientList);

                erPatientView = RenderPartialViewToString(PartialViews.ErPatientFinalBillList, erPatientList);
                var jsonData = new { inPatientView, outPatientView, erPatientView };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FinalBillsListView(int? id, int? typeId)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            BillDetailsView billDetailsView = null;
            if (id == null || typeId == null || Convert.ToInt32(id) == 0)
            {
                ViewBag.ShowPatientHeader = false;
                using (var bal = new BillHeaderBal())
                {
                    var billHeaderList = bal.GetFinalBillHeadersList(0, 0, false, corporateId, facilityId);

                    /* Who: Krishna
                     When: 12-April-2016
                     What: To get 3 different List (In/Op/Er)
                     Why: Clicent Requriment Task Id On Tms: XXXXX*/
                    var opPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("OutPatient")).ToList();
                    var inPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("InPatient")).ToList();
                    var erPatientList = billHeaderList.Where(x => x.EncounterPatientType.Equals("ERPatient")).ToList();

                    var payerWiseBillHeaderList = bal.GetFinalBillPayerHeadersList(corporateId, facilityId);

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
            using (var bal = new BillHeaderBal())
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var billHeaderList = bal.GetFinalBillByPayerHeadersList(corporateId, facilityId, payerid);
                return PartialView(PartialViews.FinalBillHeadersListView, billHeaderList);
            }
        }

        public ActionResult GetClaimsByPayerIdView(string payerIds)
        {
            using (var bal = new BillHeaderBal())
            {
                var mainList = bal.GetFinalBillByPayerHeadersList(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId(), payerIds);
                var opPatientList = mainList.Where(x => x.EncounterPatientType.Equals("OutPatient")).ToList();
                var inPatientList = mainList.Where(x => x.EncounterPatientType.Equals("InPatient")).ToList();
                var erPatientList = mainList.Where(x => x.EncounterPatientType.Equals("ERPatient")).ToList();

                var inPatientView = RenderPartialViewToString(PartialViews.InPatientFinalBillList, inPatientList);
                var outPatientView = RenderPartialViewToString(PartialViews.OutPatientFinalBillList, opPatientList);

                var erPatientView = RenderPartialViewToString(PartialViews.ErPatientFinalBillList, erPatientList);

                var jsonData = new { inPatientView, outPatientView, erPatientView };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
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

                    using (
                        var bal = new BillHeaderBal(
                            Helpers.DefaultCptTableNumber,
                            Helpers.DefaultServiceCodeTableNumber,
                            Helpers.DefaultDrgTableNumber,
                            Helpers.DefaultDrugTableNumber,
                            Helpers.DefaultHcPcsTableNumber,
                            Helpers.DefaultDiagnosisTableNumber))
                    {
                        encounterId = id;

                        // Call Stored Procedures here to apply charges of Bed and Order Bill against the current Encounter ID
                        // Commented by Shashank On 15 Sept 2015
                        // bal.ApplyBedChargesAndOrderBill(id);

                        // Get Patient Details by current Encounter ID
                        patientId = bal.GetPatientIdByEncounterId(id);
                        patientInfo = bal.GetPatientDetailsByPatientId(Convert.ToInt32(patientId));

                        billHeaderList = bal.GetBillHeaderListByEncounterId(id, corporateId, facilityId);
                    }

                    break;
                case QueryStringType.Patient:

                    #region Patient Type

                    using (
                        var bal = new BillHeaderBal(
                            Helpers.DefaultCptTableNumber,
                            Helpers.DefaultServiceCodeTableNumber,
                            Helpers.DefaultDrgTableNumber,
                            Helpers.DefaultDrugTableNumber,
                            Helpers.DefaultHcPcsTableNumber,
                            Helpers.DefaultDiagnosisTableNumber))
                    {
                        // Call Stored Procedures here to apply charges of Bed and Order Bill against the current Encounter ID
                        var encountersList = bal.GetActiveEncounterIdsByPatientId(id);

                        // Apply Bed Charges to each encounter of Patient ID
                        // Commented by Shashank On 15 Sept 2015
                        // if (encountersList.Count > 0)
                        // {
                        // encountersList = encountersList.Distinct().ToList();
                        // encountersList.ForEach(item => bal.ApplyBedChargesAndOrderBill(item));
                        // }

                        // Get Patient Details by current Encounter ID
                        patientInfo = bal.GetPatientDetailsByPatientId(id);

                        billHeaderList = bal.GetBillHeaderListByPatientId(id, corporateId, facilityId);
                    }

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
    }
}