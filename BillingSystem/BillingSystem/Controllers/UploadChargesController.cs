// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using Unity;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    /// <summary>
    /// The upload charges controller.
    /// </summary>
    public class UploadChargesController : BaseController
    {
        private readonly IBedMasterService _bedService;
        private readonly IEncounterService _eService;
        private readonly IBillActivityService _baService;
        private readonly IBedChargesService _bcService;
        private readonly IBillHeaderService _bhService;
        private readonly IOpenOrderService _ooService;
        private readonly IOrderActivityService _oaService;
        private readonly IUploadChargesService _service;
        private readonly IDiagnosisService _diaService;
        private readonly IPatientInfoService _piService;
        private readonly IManualChargesTrackingService _mctService;
        private readonly IServiceCodeService _scService;
        private readonly IGlobalCodeService _gService;

        public UploadChargesController(IBedMasterService bedService, IEncounterService eService, IBillActivityService baService, IBedChargesService bcService, IBillHeaderService bhService, IOpenOrderService ooService, IOrderActivityService oaService, IUploadChargesService service, IDiagnosisService diaService, IPatientInfoService piService, IManualChargesTrackingService mctService, IServiceCodeService scService, IGlobalCodeService gService)
        {
            _bedService = bedService;
            _eService = eService;
            _baService = baService;
            _bcService = bcService;
            _bhService = bhService;
            _ooService = ooService;
            _oaService = oaService;
            _service = service;
            _diaService = diaService;
            _piService = piService;
            _mctService = mctService;
            _scService = scService;
            _gService = gService;
        }



        //
        // GET: /UploadCharges/
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? pId, int? eId, int? bhId)
        {
            var uploadCharges = new UploadChargesView
            {
                EncounterOrder = new OpenOrder(),
                OpenOrdersList = new List<OpenOrderCustomModel>(),
                CurrentDiagnosis = new DiagnosisCustomModel(),
                PatientId = pId.HasValue ? Convert.ToInt32(pId) : 0,
                EncounterId = eId.HasValue ? Convert.ToInt32(eId) : 0,
                BillHeaderId = bhId.HasValue ? Convert.ToInt32(bhId) : 0,
                DiagnosisViewCustom = new DiagnosisView
                {
                    CurrentDiagnosis = new DiagnosisCustomModel(),
                    DiagnosisList = new List<DiagnosisCustomModel>()
                },
                RoomChargesViewCustom = new RoomChargesView
                {
                    CurrentRoomCharge = new OpenOrder(),
                    RoomChargesList = new List<BillDetailCustomModel>()
                }
            };
            return View(uploadCharges);
        }

        /// <summary>
        /// Updates the order activities.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public ActionResult UpdateOrderActivities(int orderId)
        {
            var orderactvities = _oaService.GetOrderActivitiesByOrderId(orderId);
            var loggedinuserid = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            foreach (var orderActivityCustomModel in orderactvities)
            {
                orderActivityCustomModel.ExecutedBy = loggedinuserid;
                orderActivityCustomModel.ExecutedDate = currentDateTime;
                orderActivityCustomModel.ModifiedBy = loggedinuserid;
                orderActivityCustomModel.ModifiedDate = currentDateTime;
                orderActivityCustomModel.OrderActivityStatus = Convert.ToInt32(OpenOrderActivityStatus.Closed);
                _oaService.AddUptdateOrderActivity(orderActivityCustomModel);
            }
            return Json(orderId);
        }

        /// <summary>
        /// Gets the orders by encounter identifier.
        /// </summary>
        /// <param name="EncounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult GetOrdersByEncounterId(int EncounterId)
        {
            var orderactvities = _ooService.GetAllOrdersByEncounterId(EncounterId);
            return PartialView("UserControls/_OpenOrderList", orderactvities);
        }

        /// <summary>
        /// Gets the patient denail search result.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public ActionResult GetPatientDenailSearchResult(CommonModel common)
        {
            var facilityid = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            common.FacilityId = facilityid;
            common.CorporateId = corporateid;
            var objPatientInfoData = _service.GetXPaymentReturnDenialClaims(common);
            ViewBag.Message = null;
            return PartialView(PartialViews.PatientCustomSerachList, objPatientInfoData);
        }

        /// <summary>
        /// Resets the physician order form.
        /// </summary>
        /// <param name="EncId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult ResetPhysicianOrderForm(int EncId)
        {
            var encObj = _eService.GetEncounterByEncounterId(EncId);
            //Intialize the new object of Facility ViewModel
            var encounterOrder = new OpenOrder { StartDate = encObj.EncounterStartTime, EndDate = encObj.EncounterEndTime, OrderStatus = Convert.ToInt32(OrderStatus.Closed).ToString() };

            //Pass the View Model as FacilityViewModel to PartialView FacilityAddEdit just to update the AddEdit partial view.
            return PartialView("UserControls/_OpenOrderAddEdit", encounterOrder);
        }

        /// <summary>
        /// Binds the encounter order list.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult BindEncounterOrderList(int encounterId)
        {
            var encounterIdint = Convert.ToInt32(encounterId);
            var status = OrderStatus.Open.ToString();
            var listOfOrders = _ooService.GetAllOrdersByEncounterId(encounterIdint).ToList();
            return PartialView(PartialViews.PhysicianOpenOrderList, listOfOrders);
        }

        /// <summary>
        /// The check diagnosis date range.
        /// </summary>
        /// <param name="encounterId">
        /// The encounter id.
        /// </param>
        /// <param name="orderStartDate">
        /// The order start date.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult CheckDiagnosisDateRange(int encounterId, string orderStartDate)
        {
            var diagnosisDateValid = false;
            var encounterObj = _eService.GetEncounterByEncounterId(encounterId);
            if (encounterObj != null)
            {
                var encStartTime = encounterObj.EncounterStartTime;
                var encEndTime = encounterObj.EncounterEndTime;
                var pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                //var orderStartDate1 = DateTime.ParseExact(orderStartDate, pattern + " HH:mm", new CultureInfo("en-US"));
                var orderStartDate1 = Convert.ToDateTime(orderStartDate);
                var currentDateTime = Helpers.GetInvariantCultureDateTime();
                if (encStartTime != null)
                {
                    diagnosisDateValid = (Convert.ToDateTime(orderStartDate1) >= encStartTime &&
                                          ((encEndTime == null &&
                                            Convert.ToDateTime(orderStartDate1) <= currentDateTime) ||
                                           Convert.ToDateTime(orderStartDate1) <= encEndTime));

                }
                else
                {
                    return Json(diagnosisDateValid);
                }
            }
            return Json(diagnosisDateValid);

        }

        /// <summary>
        /// Checks the order date range.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="orderStartDate">The order start date.</param>
        /// <param name="orderEndDate">The order end date.</param>
        /// <returns></returns>
        public ActionResult CheckOrderDateRange(int encounterId, string orderStartDate, string orderEndDate)
        {
            var orderDateValid = false;
            var encounterObj = _eService.GetEncounterByEncounterId(encounterId);
            if (encounterObj != null)
            {
                var encStartTime = encounterObj.EncounterStartTime;
                var encEndTime = encounterObj.EncounterEndTime ?? (!string.IsNullOrEmpty(encounterObj.EncounterDischargeLocation)
                                                                       ? Convert.ToDateTime(encounterObj.EncounterDischargeLocation)
                                                                       : CurrentDateTime);
                var pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                //var orderStartDate1 = DateTime.ParseExact(orderStartDate, pattern + " HH:mm", new CultureInfo("en-US"));
                //var orderEndDate1 = DateTime.ParseExact(orderEndDate, pattern + " HH:mm", new CultureInfo("en-US"));

                var orderStartDate1 = orderStartDate;

                var orderEndDate1 = orderEndDate;
                //var orderStartDate1 = DateTime.Parse(orderStartDate);
                //var orderEndDate1 = DateTime.Parse(orderEndDate);
                //DateTime orderEndDate1;
                //DateTime.TryParseExact(orderEndDate, "mm/dd/yyyy HH:mm tt", null, System.Globalization.DateTimeStyles.None,out orderEndDate1);
                if (encStartTime != null)
                {
                    if (encEndTime != null)
                    {
                        if ((Convert.ToDateTime(orderStartDate1) == encStartTime))
                        {
                            orderDateValid = (Convert.ToDateTime(orderStartDate1) == encStartTime &&
                                              Convert.ToDateTime(orderEndDate1) <= encEndTime) &&
                                             (Convert.ToDateTime(orderEndDate1) >= encStartTime &&
                                              Convert.ToDateTime(orderEndDate1) <= encEndTime);
                        }
                        else
                        {
                            orderDateValid = (Convert.ToDateTime(orderStartDate1) >= encStartTime &&
                                              Convert.ToDateTime(orderEndDate1) <= encEndTime) &&
                                             (Convert.ToDateTime(orderEndDate1) >= encStartTime &&
                                              Convert.ToDateTime(orderEndDate1) <= encEndTime);
                        }
                    }
                    else
                    {
                        orderDateValid = (Convert.ToDateTime(orderStartDate1) >= encStartTime);
                    }
                }
            }
            return Json(orderDateValid);

        }

        /// <summary>
        /// Gets the diagnosis by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult GetDiagnosisByEncounterId(int encounterId)
        {
            var currentEncounterdiagnosislist = _diaService.GetDiagnosisListByEncounterId(encounterId);
            var diagnosislistUCpath = string.Format("../Diagnosis/{0}", PartialViews.DiagnosisList);
            return PartialView(diagnosislistUCpath, currentEncounterdiagnosislist);
        }

        /// <summary>
        /// Diagnosises the check.
        /// </summary>
        /// <param name="EncounterID">The encounter identifier.</param>
        /// <param name="PatientID">The patient identifier.</param>
        /// <returns>Partial View</returns>
        public ActionResult DiagnosisCheck(int EncounterID, int PatientID)
        {
            List<DiagnosisCustomModel> list = null;
            DiagnosisCustomModel diagnosisModel = null;
            PatientInfoCustomModel patientInfo = null;
            var isPrimary = true;
            var isMajorCPT = true;
            var dModel = _diaService.GetNewDiagnosisByEncounterId(EncounterID, PatientID);
            var dList = _diaService.GetDiagnosisList(PatientID, EncounterID);
            isMajorCPT = !dList.Any(x => x.DiagnosisType == 4);
            dModel.IsMajorCPT = isMajorCPT;
            dModel.IsMajorDRG = !dList.Any(x => x.DiagnosisType == 3);
            list = dList != null && dList.Count > 0 ? dList : new List<DiagnosisCustomModel>();
            patientInfo = _piService.GetPatientDetailsByPatientId(PatientID);
            diagnosisModel = dModel ?? new DiagnosisCustomModel();
            isPrimary = list.Count == 0;

            if (PatientID != 0)
            {
                diagnosisModel.PatientID = PatientID;
                diagnosisModel.EncounterID = EncounterID;
                diagnosisModel.CorporateID = patientInfo.CorporateId;
                diagnosisModel.FacilityID = patientInfo.PatientInfo.FacilityId;
            }

            diagnosisModel.IsPrimary = isPrimary;
            var diagnosisView = new DiagnosisView
            {
                CurrentDiagnosis = diagnosisModel,
                DiagnosisList = list,
            };
            return PartialView(PartialViews.UpdateDiagnosis, diagnosisView);
        }

        /// <summary>
        /// Gets the bill details by bill header identifier.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns>HTML STRING</returns>
        public ActionResult GetBillDetailsByBillHeaderId(int billHeaderId)
        {
            //var _service = new BillActivityService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var list = _baService.GetBillActivitiesByBillHeaderId(billHeaderId);

            // var objBillHeaderDetails = _service.GetBillDetailsByBillHeaderId(billHeaderId);
            return PartialView(PartialViews.UploadChargesBillActivitiesList, list);
        }

        /// <summary>
        /// Deletes the bill activity.
        /// </summary>
        /// <param name="billActivityId">The bill activity identifier.</param>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteBillActivity(int billActivityId, int billHeaderId)
        {
            var objBillHeaderDetails = _service.DeleteBillActivity(billActivityId, Helpers.GetLoggedInUserId(),
                billHeaderId);
            return PartialView(PartialViews.UploadChargesBillActivitiesList, objBillHeaderDetails);
        }

        /// <summary>
        /// Adds the update maunal charges audit log.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult AddUpdateMaunalChargesAuditLog(DiagnosisCustomModel model)
        {
            var userId = Helpers.GetLoggedInUserId();
            var currentdatetime = Helpers.GetInvariantCultureDateTime();
            var moduletrackingobj = new ManualChargesTracking
            {
                //BillHeaderID = 
                CorporateID = Convert.ToInt32(model.CorporateID),
                FacilityID = Convert.ToInt32(model.FacilityID),
                PatientID = Convert.ToInt32(model.PatientID),
                EncounterID = Convert.ToInt32(model.EncounterID),
                TrackingValue = model.DiagnosisCode,
                TrackingTableName = "Diagnosis",
                TrackingTypeNameVal = "",
                IsVisible = true,
            };
            if (model.DiagnosisID == 0)
            {
                moduletrackingobj.CreatedBy = userId;
                moduletrackingobj.CreatedDate = currentdatetime;
                moduletrackingobj.TrackingType = "2";
            }
            else
            {
                moduletrackingobj.ModifiedBy = userId;
                moduletrackingobj.ModifiedDate = currentdatetime;
                moduletrackingobj.TrackingType = "3";
            }

            if (!string.IsNullOrEmpty(model.DiagnosisCode))
            {
                if (model.DiagnosisType == 1 || model.DiagnosisType == 2)
                {
                    moduletrackingobj.ManualChargesTrackingID = 0;
                    moduletrackingobj.TrackingColumnName = "DiagnosisCode";
                    moduletrackingobj.TrackingTypeNameVal = Convert.ToInt32(OrderType.Diagnosis).ToString();
                    _mctService.AddUptdateManualChargesTracking(moduletrackingobj);
                }
                if (model.DRGCodeID != null)
                {
                    moduletrackingobj.ManualChargesTrackingID = 0;
                    moduletrackingobj.TrackingColumnName = "DRG";
                    moduletrackingobj.TrackingTypeNameVal = Convert.ToInt32(OrderType.DRG).ToString();
                    moduletrackingobj.TrackingValue = model.DRGCodeID.ToString();
                    _mctService.AddUptdateManualChargesTracking(moduletrackingobj);
                }
                if (!string.IsNullOrEmpty(model.MajorCPTCodeId))
                {
                    moduletrackingobj.ManualChargesTrackingID = 0;
                    moduletrackingobj.TrackingColumnName = "Major CPT";
                    moduletrackingobj.TrackingValue = model.MajorCPTCodeId.ToString();
                    moduletrackingobj.TrackingTypeNameVal = Convert.ToInt32(OrderType.CPT).ToString();
                    _mctService.AddUptdateManualChargesTracking(moduletrackingobj);
                }
            }
            else
            {
                if (model.DiagnosisType == 3)
                {
                    moduletrackingobj.ManualChargesTrackingID = 0;
                    moduletrackingobj.TrackingColumnName = "DRG";
                    moduletrackingobj.TrackingTypeNameVal = Convert.ToInt32(OrderType.DRG).ToString();
                    moduletrackingobj.TrackingValue = model.DRGCodeID.ToString();
                    _mctService.AddUptdateManualChargesTracking(moduletrackingobj);
                }
                else if (model.DiagnosisType == 4)
                {
                    moduletrackingobj.ManualChargesTrackingID = 0;
                    moduletrackingobj.TrackingColumnName = "Major CPT";
                    moduletrackingobj.TrackingTypeNameVal = Convert.ToInt32(OrderType.CPT).ToString();
                    _mctService.AddUptdateManualChargesTracking(moduletrackingobj);
                }
            }
            return null;
        }

        /// <summary>
        /// Maunals the charges addition audit log deletion.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="diagnosisType">Type of the diagnosis.</param>
        /// <returns></returns>
        public ActionResult MaunalChargesAdditionAuditLogDeletion(string id, string diagnosisType)
        {
            var userId = Helpers.GetLoggedInUserId();
            var currentdatetime = Helpers.GetInvariantCultureDateTime();
            var diangnosisObj = _diaService.GetDiagnosisById(id);
            var moduletrackingobj = new ManualChargesTracking
            {
                ManualChargesTrackingID = 0,
                CorporateID = Convert.ToInt32(diangnosisObj.CorporateID),
                FacilityID = Convert.ToInt32(diangnosisObj.FacilityID),
                PatientID = Convert.ToInt32(diangnosisObj.PatientID),
                EncounterID = Convert.ToInt32(diangnosisObj.EncounterID),
                TrackingValue = diangnosisObj.DiagnosisCode,
                TrackingTableName = "Diagnosis",
                IsVisible = true,
                CreatedBy = userId,
                CreatedDate = currentdatetime,
                TrackingType = "1",
            };
            if (diangnosisObj.DiagnosisType == 1 || diangnosisObj.DiagnosisType == 2)
            {
                moduletrackingobj.TrackingColumnName = "Diagnosis";
                moduletrackingobj.TrackingTypeNameVal = Convert.ToInt32(OrderType.Diagnosis).ToString();
            }
            else if (diangnosisObj.DiagnosisType == 3)
            {
                moduletrackingobj.TrackingColumnName = "DRG";
                moduletrackingobj.TrackingTypeNameVal = Convert.ToInt32(OrderType.DRG).ToString();
            }
            else if (diangnosisObj.DiagnosisType == 4)
            {
                moduletrackingobj.TrackingColumnName = "Major CPT";
                moduletrackingobj.TrackingTypeNameVal = Convert.ToInt32(OrderType.CPT).ToString();
            }
            _mctService.AddUptdateManualChargesTracking(moduletrackingobj);
            return null;
        }

        /// <summary>
        /// Gets the room charges by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult GetRoomChargesByEncounterId(int encounterId, int? claimId)
        {
            //using (var billactivityBal = new BillActivityService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            //{
            var orderactvities = new List<BillDetailCustomModel>();
            if (claimId != null)
            {
                orderactvities = _service.GetBillDetailsByBillHeaderId(Convert.ToInt32(claimId)).Where(x => x.ActivityType == "8").ToList();
            }
            else
            {
                orderactvities =
                    _baService.GetBillActivitiesByEncounterId(encounterId, Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber)
                        .Where(x => x.ActivityType == "8")
                        .ToList();
            }
            //var a = abc();
            var roomChargesViewCustom = new RoomChargesView
            {
                CurrentRoomCharge = new OpenOrder(),
                RoomChargesList = orderactvities
            };
            return PartialView("UserControls/_RoomChargesMain", roomChargesViewCustom);

        }


        /// <summary>
        /// Rooms the charges addition audit log insertion.
        /// </summary>
        /// <param name="chargedadded">The chargedadded.</param>
        /// <returns></returns>
        public ActionResult RoomChargesAdditionAuditLogInsertion(BedCharges chargedadded)
        {
            var userId = Helpers.GetLoggedInUserId();
            var currentdatetime = Helpers.GetInvariantCultureDateTime();
            var moduletrackingobj = new ManualChargesTracking
            {
                ManualChargesTrackingID = 0,
                CorporateID = Convert.ToInt32(chargedadded.BCCorporateID),
                FacilityID = Convert.ToInt32(chargedadded.BCFacilityID),
                PatientID = Convert.ToInt32(chargedadded.BCPatientID),
                EncounterID = Convert.ToInt32(chargedadded.BCEncounterID),
                TrackingColumnName = "Service Code",
                TrackingTypeNameVal = Convert.ToInt32(OrderType.BedCharges).ToString(),
                TrackingValue = chargedadded.ServiceCodeValue,
                TrackingTableName = "Bed Charges",
                IsVisible = true,
                CreatedBy = userId,
                CreatedDate = currentdatetime,
                TrackingType = "2",
            };
            _mctService.AddUptdateManualChargesTracking(moduletrackingobj);
            return null;
        }

        /// <summary>
        /// Gets the room charges.
        /// </summary>
        /// <param name="claimid">The claimid.</param>
        /// <returns></returns>
        public ActionResult GetRoomCharges(int encounterid, int? claimId)
        {
            //var orderactvities = billactivityBal.GetBillActivitiesByEncounterId(encounterid).Where(x => x.ActivityType == "8").ToList();
            var orderactvities = new List<BillDetailCustomModel>();
            if (claimId != null)
            {
                orderactvities = _service.GetBillDetailsByBillHeaderId(Convert.ToInt32(claimId)).Where(x => x.ActivityType == "8").ToList();
            }
            else
            {
                orderactvities =
                    _baService.GetBillActivitiesByEncounterId(encounterid, Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber)
                        .Where(x => x.ActivityType == "8")
                        .ToList();
            }
            return PartialView("UserControls/_RoomChargesList", orderactvities);

        }

        /// <summary>
        /// Checks the room charges date range.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="orderStartDate">The order start date.</param>
        /// <param name="orderEndDate">The order end date.</param>
        /// <returns></returns>
        public ActionResult CheckRoomChargesDateRange(int encounterId, string orderStartDate, string orderEndDate)
        {
            var orderDateValid = false;
            var encounterObj = _eService.GetEncounterByEncounterId(encounterId);
            if (encounterObj != null)
            {
                var encStartTime = encounterObj.EncounterStartTime;
                var encEndTime = encounterObj.EncounterEndTime;
                if (encStartTime != null)
                {
                    if (encEndTime != null)
                    {
                        if ((Convert.ToDateTime(orderStartDate) == encStartTime))
                        {
                            orderDateValid = (Convert.ToDateTime(orderStartDate) == encStartTime &&
                                              Convert.ToDateTime(orderEndDate) <= encEndTime) &&
                                             (Convert.ToDateTime(orderEndDate) >= encStartTime &&
                                              Convert.ToDateTime(orderEndDate) <= encEndTime);
                        }
                        else
                        {
                            orderDateValid = (Convert.ToDateTime(orderStartDate) >= encStartTime &&
                                              Convert.ToDateTime(orderEndDate) <= encEndTime) &&
                                             (Convert.ToDateTime(orderEndDate) >= encStartTime &&
                                              Convert.ToDateTime(orderEndDate) <= encEndTime);
                        }
                    }
                    else
                    {
                        orderDateValid = (Convert.ToDateTime(orderStartDate) >= encStartTime);
                    }
                }
            }
            return Json(orderDateValid);

        }

        /// <summary>
        /// Deletes the room charges bill activity.
        /// </summary>
        /// <param name="billActivityId">The bill activity identifier.</param>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteRoomChargesBillActivity(int billActivityId, int billHeaderId)
        {
            var objBillHeaderDetails = _service.DeleteBillActivity(billActivityId, Helpers.GetLoggedInUserId(),
                billHeaderId).Where(x => x.ActivityType == "8").ToList();
            return PartialView(PartialViews.RoomChargesList, objBillHeaderDetails);
        }

        /// <summary>
        /// Gets the room charges by service code.
        /// </summary>
        /// <param name="serviceCode">The service code.</param>
        /// <param name="effectiveDate"></param>
        /// <returns></returns>
        public ActionResult GetRoomChargesByServiceCode(string serviceCode, DateTime? effectiveDate)
        {
            var serviceCodeObj = _scService.GetServiceCodePriceByCodeValue(serviceCode, null);
            return Json(serviceCodeObj, JsonRequestBehavior.AllowGet);
        }

        #region Room Charges...

        /// <summary>
        /// Adds the manual room charges.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public ActionResult AddManualRoomCharges(List<OpenOrderCustomModel> order)
        {
            var userId = Helpers.GetLoggedInUserId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();

            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            try
            {
                var list = new List<BedCharges>();

                foreach (var item in order)
                {
                    DateTime? startDate = null;
                    if (item.StartDate.HasValue)
                        startDate = item.StartDate.Value;

                    var itemGross = _scService.GetServiceCodePriceByCodeValue(item.OrderCode, null);
                    var itemObj = new BedCharges
                    {
                        BedChargesID = item.OpenOrderID,
                        BCCorporateID = corporateId,
                        BCFacilityID = facilityId,
                        BCPatientID = item.PatientID,
                        BCEncounterID = item.EncounterID,
                        BCBedID = 0,
                        BCMappingBedPatientID = 0,
                        BCRangeStart = 1,
                        BCRangeEnd = 1,
                        BCBedRateTypeID = 1,
                        BCTransactionDate = item.StartDate,
                        BCRangeEffectiveDays = 1,
                        BCUnitRate = 0,
                        BCGross = itemGross,
                        BCActivityStartDate = item.StartDate,
                        BCActivityEndDate = item.EndDate,
                        BCTotalEffectiveDays = 1,
                        BCStatus = "0",
                        BCCreatedBy = userId,
                        BCCreatedDate = currentDateTime,
                        BCModifiedBy = null,
                        BCModifiedDate = null,
                        BCIsActive = true,
                        ServiceCodeValue = item.OrderCode
                    };

                    var isexist = _bcService.CheckBedChargeExist(Convert.ToInt32(itemObj.BCEncounterID),
                        Convert.ToInt32(itemObj.BCPatientID), itemObj.BCTransactionDate);

                    if (!isexist)
                        RoomChargesAdditionAuditLogInsertion(itemObj);

                    list.Add(itemObj);
                }
                var savedActivities = _bcService.SaveBedChargesList(list);
                return Json(savedActivities);
            }
            catch (Exception)
            {
                return Json(false);

            }
        }
        #endregion


        #region Add Open Orders Manually
        /// <summary>
        /// Adds the manual orders.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public ActionResult AddManualOrders(List<OpenOrderCustomModel> order)
        {
            var userId = Helpers.GetLoggedInUserId();
            var corporateId = Helpers.GetDefaultCorporateId();
            if (corporateId == 0 && userId > 0)
                corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            try
            {
                order = order.Where(item => item != null).ToList();
                var objListOpenOrders = order.Select(item => new OpenOrder
                {
                    OpenOrderID = item.OpenOrderID,
                    OpenOrderPrescribedDate = item.StartDate,
                    PhysicianID = userId,
                    PatientID = item.PatientID,
                    EncounterID = item.EncounterID,
                    DiagnosisCode = item.DiagnosisCode,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    CategoryId = item.CategoryId,
                    SubCategoryId = item.SubCategoryId,
                    OrderType = item.OrderType,
                    OrderCode = item.OrderCode,
                    Quantity = item.Quantity,
                    FrequencyCode = item.FrequencyCode,
                    PeriodDays = "1",
                    OrderNotes = item.OrderNotes,
                    OrderStatus = item.OrderStatus,
                    IsActivitySchecduled = false,
                    ActivitySchecduledOn = item.StartDate,
                    ItemName = item.ItemName,
                    ItemStrength = item.ItemStrength,
                    ItemDosage = item.ItemDosage,
                    IsActive = item.IsActive,
                    CreatedBy = userId,
                    CreatedDate = currentDateTime,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    IsDeleted = item.IsDeleted,
                    DeletedBy = item.DeletedBy,
                    DeletedDate = item.DeletedDate,
                    CorporateID = corporateId,
                    FacilityID = facilityId,
                    IsApproved = true,
                    EV1 = "9090",
                    EV2 = Convert.ToString(item.ClaimId)
                }).ToList();

                _ooService.AddUpdatePhysicianMultipleOpenOrder(objListOpenOrders);
            }
            catch (Exception)
            {
                return Json(false);
            }
            return Json(true);
        }
        #endregion



        public PartialViewResult GetPatientResultByPatientId(int patientId, int encounterId, int billHeaderId)
        {
            var objPatientInfoData = _service.GetXPaymentReturnDenialClaimsByPatientId(patientId, encounterId, billHeaderId);
            ViewBag.Message = null;
            return PartialView(PartialViews.PatientCustomSerachList, objPatientInfoData);
        }


        #region Not In Use currently....
        /// <summary>
        /// Abcs this instance.
        /// </summary>
        /// <returns></returns>
        private int abc()
        {
            var a = new int[5] { 99, 50, 92, 97, 100 };
            var listordered = (a.OrderByDescending(n => n)).Distinct().Take(2).ToList();
            if (listordered.Count == 2)
            {
                var listorderedproduct = listordered[0] * listordered[1];
                return listorderedproduct;
            }
            return 0;
        }

        #endregion

        /// <summary>
        /// Gets the bill virtual discharge details.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public ActionResult GetBillVirtualDischargeDetails(int patientId, int encounterId, int billHeaderId)
        {
            var objPatientInfoData = _service.GetXPaymentReturnDenialClaimsByPatientId(patientId, encounterId, billHeaderId);
            ViewBag.Message = null;
            return PartialView(PartialViews.PatientCustomSerachList, objPatientInfoData);
        }
    }
}
