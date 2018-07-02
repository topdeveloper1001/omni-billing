using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    public class EvaluationController : BaseController
    {
        private readonly IEncounterService _eService;
        private readonly IOpenOrderService _ooService;
        private readonly IMedicalRecordService _mrService;
        private readonly IMedicalNotesService _mnService;
        private readonly IMedicalVitalService _mvService;
        private readonly IPatientEvaluationService _service;
        private readonly IDiagnosisService _diaService;
        private readonly IOrderActivityService _oaService;
        private readonly IPatientDischargeSummaryService _pdsService;



        const string partialViewPath = "../Evaluation/";

        public EvaluationController(IEncounterService eService, IOpenOrderService ooService, IMedicalRecordService mrService, IMedicalNotesService mnService, IMedicalVitalService mvService, IPatientEvaluationService service, IDiagnosisService diaService, IOrderActivityService oaService, IPatientDischargeSummaryService pdsService)
        {
            _eService = eService;
            _ooService = ooService;
            _mrService = mrService;
            _mnService = mnService;
            _mvService = mvService;
            _service = service;
            _diaService = diaService;
            _oaService = oaService;
            _pdsService = pdsService;
        }


        //
        // GET: /Evaluation/
        public ActionResult Index(int? pId)
        {
            if (pId == null || Convert.ToInt32(pId) == 0)
                return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch,
                    new { messageId = Convert.ToInt32(MessageType.ViewEM) });

            var patientId = Convert.ToInt32(pId);
            var enList = _ooService.GetEncountersListByPatientId(patientId);
            var currentEncounterId = (enList != null && enList.Any() &&
                                      enList.First().EncounterEndTime == null)
                ? enList.First().EncounterID
                : 0;

            var medicalrecords = _mrService.GetMedicalRecord();
            var medicalvitals = _mvService.GetCustomMedicalVitalsByPidEncounterId(patientId,
                Convert.ToInt32(MedicalRecordType.Vitals), currentEncounterId);
            var closedOrderActivityList = new List<OrderActivityCustomModel>();
            var openOrderActivityList = new List<OrderActivityCustomModel>();
            //added by Shashank on Dec 01 2014
            var patientSummaryNotes =
                _mnService.GetMedicalNotesByPatientIdEncounterId(patientId, currentEncounterId);
            var allergiesList = _mrService.GetAlergyRecords(patientId,
                Convert.ToInt32(MedicalRecordType.Allergies));

            var orderStatus = OrderStatus.Open.ToString();
            var openOrdersList = _ooService.GetPhysicianOrders(currentEncounterId, orderStatus, Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var primarydiagnosisId = 0;
            var diagnosisList = _diaService.GetDiagnosisList(patientId, currentEncounterId);
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
            var encounterActivitesobj = _oaService.GetOrderActivitiesByEncounterId(Convert.ToInt32(currentEncounterId), Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var encounterActivitesClosedListObj =
                encounterActivitesobj.Where(
                    x =>
                        x.OrderActivityStatus != 0 &&
                        x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)).OrderBy(x => x.ExecutedDate)
                    .ToList();
            var encounterActivitesOpenListObj =
                encounterActivitesobj.Where(
                    x =>
                        x.OrderActivityStatus == 0 ||
                        x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                       ).OrderBy(x => x.OrderScheduleDate)
                    .ToList();
            closedOrderActivityList = encounterActivitesClosedListObj;
            openOrderActivityList = encounterActivitesOpenListObj;

            //added by Ashwani
            var summaryView = new EvaluationViews
            {
                EncounterOrder = new OpenOrder(),
                IsLabTest = false,
                MedicalHistoryList = new List<MedicalHistoryCustomModel>(),
                labOrderActivityList = new List<OrderActivityCustomModel>(),
                CurrentLabOrderActivity = new OrderActivityCustomModel(),
                OpenOrdersList = openOrdersList,
                MedicalVitalList = medicalvitals,
                ClosedOrdersList = _ooService.GetPhysicianOrders(currentEncounterId, OrderStatus.Closed.ToString(), Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber),
                AlergyList = allergiesList,
                CurrentOrderActivity = new OrderActivity { OrderActivityStatus = 1 },
                OrderActivityList = openOrderActivityList,
                ClosedOrderActivityList = closedOrderActivityList,
                ClosedLabOrderActivityList = new List<OrderActivityCustomModel>(),
            };
            return View(summaryView);
        }




        public ActionResult Index2()
        {
            return View();
        }


        public ActionResult SaveEvaluationManagement(List<string> basicInfo, long patientId, long encounterId, string estatus, string rowExists, int setId, string imagePath)
        {
            if (encounterId == 0 && rowExists == "0" && patientId > 0)
            {
                var currentEncounter = _eService.GetActiveEncounterByPateintId(Convert.ToInt32(patientId));
                if (currentEncounter != null)
                    encounterId = currentEncounter.EncounterID;
                else
                    return Json(-1, JsonRequestBehavior.AllowGet);
            }

            var result = _service.SavePatientEvaluationData(basicInfo, patientId, encounterId, Helpers.GetSysAdminCorporateID()
                , Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId(), setId, estatus, imagePath);
            return Json(result.Status > 0 ? result.Status : -1, JsonRequestBehavior.AllowGet);


        }

        public ActionResult GetSignatureData(int ecounterId, int patinetId, string setId)
        {
            var list = _service.GetSignaturePath(ecounterId, patinetId, setId);
            return Json(list);
        }


        public ActionResult SignatureEnableDisable(int setId, int patientId)
        {

            var logedInUser = Helpers.GetLoggedInUserId();

            var signedUser = _service.GetCreatedByFromEvaluationSet(setId, patientId);
            if (logedInUser == signedUser)
            {
                return Json("1");
            }
            else
            {
                return Json("2");
            }
        }


        public ActionResult EvaluationData(int? pId, string setId)
        {
            var sId = setId ?? "0";
            if (pId == null || Convert.ToInt32(pId) == 0)
                return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch, new { messageId = Convert.ToInt32(MessageType.ViewEM) });

            var patientId = Convert.ToInt32(pId);

            var enList = _ooService.GetEncountersListByPatientId(patientId);
            var currentEncounterId = (enList != null && enList.Any() &&
                                     enList.First().EncounterEndTime == null)
               ? enList.First().EncounterID
               : 0;

            var medicalvitals = _mvService.GetCustomMedicalVitalsByPidEncounterId(patientId,
                Convert.ToInt32(MedicalRecordType.Vitals), currentEncounterId);

            var cOrderActs = new List<OrderActivityCustomModel>();
            var oOrderActs = new List<OrderActivityCustomModel>();

            var allergiesList = _mrService.GetAlergyRecords(patientId,
                Convert.ToInt32(MedicalRecordType.Allergies));

            var orderStatus = OrderStatus.Open.ToString();
            var openOrdersList = _ooService.GetPhysicianOrders(currentEncounterId, orderStatus, Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber);

            var encounterActivitesobj = _oaService.GetOrderActivitiesByEncounterId(Convert.ToInt32(currentEncounterId), Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var encounterActivitesClosedListObj =
                encounterActivitesobj.Where(x =>
                    x.OrderActivityStatus != 0 &&
                    x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open))
                    .OrderBy(x => x.ExecutedDate)
                    .ToList();
            var encounterActivitesOpenListObj =
                encounterActivitesobj.Where(x => x.OrderActivityStatus == 0 ||
                                                 x.OrderActivityStatus ==
                                                 Convert.ToInt32(OpenOrderActivityStatus.Open)
                    ).OrderBy(x => x.OrderScheduleDate)
                    .ToList();

            cOrderActs = encounterActivitesClosedListObj;
            oOrderActs = encounterActivitesOpenListObj;


            #region Patient Evaluation Lists

            var listChiefComplaint = new List<PatientEvaluation>();
            var listHistoryPresentIllness = new List<PatientEvaluation>();
            var listReviewSystems = new List<PatientEvaluation>();
            var listOrganSystems = new List<PatientEvaluation>();
            var listBodyAreas = new List<PatientEvaluation>();
            var listAmtComplexityReviewed = new List<PatientEvaluation>();
            var listDiagnisosManagement = new List<PatientEvaluation>();
            var listRiskComplications = new List<PatientEvaluation>();
            var listPatientCounseled = new List<PatientEvaluation>();
            var listElecSignature = new List<PatientEvaluation>();
            var listBasicInfo = new List<PatientEvaluation>();
            var imageSource = string.Empty;
            var evGcCategories = new[] { "4610", "4613", "4615", "4616", "4617", "4624", "4625", "4627", "4628", "4629" };

            var result = _pdsService.GetPatientEvaluationData(patientId, Convert.ToInt32(currentEncounterId), evGcCategories, setId);
            if (result.Count > 0)
            {
                imageSource = result.FirstOrDefault().ExternalValue3;
                listChiefComplaint = result.Where(f => f.CategoryValue.Equals("4610")).ToList();
                listHistoryPresentIllness = result.Where(f => f.CategoryValue.Equals("4613")).ToList();
                listReviewSystems = result.Where(f => f.CategoryValue.Equals("4615")).ToList();
                listOrganSystems = result.Where(f => f.CategoryValue.Equals("4616")).ToList();
                listBodyAreas = result.Where(f => f.CategoryValue.Equals("4617")).ToList();
                listAmtComplexityReviewed = result.Where(f => f.CategoryValue.Equals("4624")).ToList();
                listDiagnisosManagement = result.Where(f => f.CategoryValue.Equals("4625")).ToList();
                listRiskComplications = result.Where(f => f.CategoryValue.Equals("4626")).ToList();
                listPatientCounseled = result.Where(f => f.CategoryValue.Equals("4627")).ToList();
                listElecSignature = result.Where(f => f.CategoryValue.Equals("4628")).ToList();
                listBasicInfo = result.Where(f => f.CategoryValue.Equals("4629")).ToList();

            }

            #endregion Patient Evaluation Lists

            var summaryView = new EvaluationViews
            {
                MedicalHistoryList = new List<MedicalHistoryCustomModel>(),
                labOrderActivityList = new List<OrderActivityCustomModel>(),
                CurrentLabOrderActivity = new OrderActivityCustomModel(),
                OpenOrdersList = openOrdersList,
                MedicalVitalList = medicalvitals,
                ClosedOrdersList = _ooService.GetPhysicianOrders(currentEncounterId, OrderStatus.Closed.ToString(), Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber),
                AlergyList = allergiesList,
                OrderActivityList = oOrderActs,
                ClosedOrderActivityList = cOrderActs,
                BasicInfoItems = listBasicInfo,
                ChiefComplaintItems = listChiefComplaint,
                HistoryPresentIllnessList = listHistoryPresentIllness,
                ReviewSystemsList = listReviewSystems,
                OrganSystemsList = listOrganSystems,
                BodyAreasList = listBodyAreas,
                AmtComplexityReviewedList = listAmtComplexityReviewed,
                DiagnisosManagementList = listDiagnisosManagement,
                RiskComplicationsList = listRiskComplications,
                PatientCounseledList = listPatientCounseled,
                ElecSignatureList = listElecSignature,
                SetId = sId
            };

            return PartialView(PartialViews.EvaluationPView, summaryView);
        }

    }
}