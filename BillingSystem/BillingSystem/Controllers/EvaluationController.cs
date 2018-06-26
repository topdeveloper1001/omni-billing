using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
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


        const string partialViewPath = "../Evaluation/";

        public EvaluationController(IEncounterService eService)
        {
            _eService = eService;
        }

        //
        // GET: /Evaluation/
        public ActionResult Index(int? pId)
        {
            if (pId == null || Convert.ToInt32(pId) == 0)
                return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch,
                    new { messageId = Convert.ToInt32(MessageType.ViewEM) });

            var patientId = Convert.ToInt32(pId);
            using (var orderBal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var enList = orderBal.GetEncountersListByPatientId(patientId);
                using (var medicalRecordbal = new MedicalRecordBal()) //Updated by Shashank on Oct 28, 2014
                {
                    using (var medicalnotesbal = new MedicalNotesBal()) //Updated by Shashank on Oct 28, 2014
                    {
                        using (var medicalVitals = new MedicalVitalBal())
                        {
                            using (var diagnosisBal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
                            {
                                var currentEncounterId = (enList != null && enList.Any() &&
                                                          enList.First().EncounterEndTime == null)
                                    ? enList.First().EncounterID
                                    : 0;

                                var medicalrecords = medicalRecordbal.GetMedicalRecord();
                                var medicalvitals = medicalVitals.GetCustomMedicalVitalsByPidEncounterId(patientId,
                                    Convert.ToInt32(MedicalRecordType.Vitals), currentEncounterId);
                                var closedOrderActivityList = new List<OrderActivityCustomModel>();
                                var openOrderActivityList = new List<OrderActivityCustomModel>();
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
                                using (var orderActivityBal = new OrderActivityBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
                                {
                                    var encounterActivitesobj = orderActivityBal.GetOrderActivitiesByEncounterId(Convert.ToInt32(currentEncounterId));
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
                                }

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
                                    ClosedOrdersList = orderBal.GetPhysicianOrders(currentEncounterId, OrderStatus.Closed.ToString()),
                                    AlergyList = allergiesList,
                                    CurrentOrderActivity = new OrderActivity { OrderActivityStatus = 1 },
                                    OrderActivityList = openOrderActivityList,
                                    ClosedOrderActivityList = closedOrderActivityList,
                                    ClosedLabOrderActivityList = new List<OrderActivityCustomModel>(),
                                };
                                return View(summaryView);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Evaluations the data.
        /// </summary>
        /// <param name="pId">The p identifier.</param>
        /// <returns></returns>
        //public ActionResult EvaluationData(int? pId, string setId)
        //{
        //    var sId = setId ?? "0";
        //    if (pId == null || Convert.ToInt32(pId) == 0)
        //        return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch,
        //            new { messageId = Convert.ToInt32(MessageType.ViewEM) });

        //    var patientId = Convert.ToInt32(pId);
        //    using (var orderBal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
        //    {
        //        var enList = orderBal.GetEncountersListByPatientId(patientId);
        //        using (var medicalRecordbal = new MedicalRecordBal())
        //        {
        //            using (var medicalVitals = new MedicalVitalBal())
        //            {
        //                var currentEncounterId = (enList != null && enList.Any() &&
        //                                          enList.First().EncounterEndTime == null)
        //                    ? enList.First().EncounterID
        //                    : 0;

        //                var medicalvitals = medicalVitals.GetCustomMedicalVitalsByPidEncounterId(patientId,
        //                    Convert.ToInt32(MedicalRecordType.Vitals), currentEncounterId);
        //                List<OrderActivityCustomModel> closedOrderActivityList;
        //                List<OrderActivityCustomModel> openOrderActivityList;
        //                var allergiesList = medicalRecordbal.GetAlergyRecords(patientId,
        //                    Convert.ToInt32(MedicalRecordType.Allergies));

        //                var orderStatus = OrderStatus.Open.ToString();
        //                var openOrdersList = orderBal.GetPhysicianOrders(currentEncounterId, orderStatus);

        //                using (var orderActivityBal = new OrderActivityBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
        //                {
        //                    var encounterActivitesobj = orderActivityBal.GetOrderActivitiesByEncounterId(Convert.ToInt32(currentEncounterId));
        //                    var encounterActivitesClosedListObj =
        //                        encounterActivitesobj.Where(
        //                            x =>
        //                                x.OrderActivityStatus != 0 &&
        //                                x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)).OrderBy(x => x.ExecutedDate)
        //                            .ToList();
        //                    var encounterActivitesOpenListObj =
        //                        encounterActivitesobj.Where(
        //                            x =>
        //                                x.OrderActivityStatus == 0 ||
        //                                x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
        //                               ).OrderBy(x => x.OrderScheduleDate)
        //                            .ToList();
        //                    closedOrderActivityList = encounterActivitesClosedListObj;
        //                    openOrderActivityList = encounterActivitesOpenListObj;
        //                }

        //                #region Patient Evaluation Lists
        //                List<PatientEvaluation> listBasicInfo;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listBasicInfo = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4629", setId);
        //                }
        //                List<PatientEvaluation> listChiefComplaint;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listChiefComplaint = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4610", setId);
        //                }
        //                List<PatientEvaluation> listHistoryPresentIllness;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listHistoryPresentIllness = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4613", setId);
        //                }
        //                List<PatientEvaluation> listReviewSystems;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listReviewSystems = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4615", setId);
        //                }
        //                List<PatientEvaluation> listOrganSystems;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listOrganSystems = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4616", setId);
        //                }
        //                List<PatientEvaluation> listBodyAreas;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listBodyAreas = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4617", setId);
        //                }
        //                List<PatientEvaluation> listAmtComplexityReviewed;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listAmtComplexityReviewed = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4624", setId);
        //                }
        //                List<PatientEvaluation> listDiagnisosManagement;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listDiagnisosManagement = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4625", setId);
        //                }
        //                List<PatientEvaluation> listRiskComplications;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listRiskComplications = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4626", setId);
        //                }
        //                List<PatientEvaluation> listPatientCounseled;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listPatientCounseled = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4627", setId);
        //                }
        //                List<PatientEvaluation> listElecSignature;
        //                using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                {
        //                    listElecSignature = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                        Convert.ToInt32(currentEncounterId), "4628", setId);
        //                }
        //                #endregion Patient Evaluation Lists

        //                var summaryView = new EvaluationViews
        //                {
        //                    EncounterOrder = new OpenOrder(),
        //                    IsLabTest = false,
        //                    MedicalHistoryList = new List<MedicalHistoryCustomModel>(),
        //                    labOrderActivityList = new List<OrderActivityCustomModel>(),
        //                    CurrentLabOrderActivity = new OrderActivityCustomModel(),
        //                    OpenOrdersList = openOrdersList,
        //                    MedicalVitalList = medicalvitals,
        //                    ClosedOrdersList =
        //                        orderBal.GetPhysicianOrders(currentEncounterId, OrderStatus.Closed.ToString()),
        //                    AlergyList = allergiesList,
        //                    CurrentOrderActivity = new OrderActivity { OrderActivityStatus = 1 },
        //                    OrderActivityList = openOrderActivityList,
        //                    ClosedOrderActivityList = closedOrderActivityList,
        //                    ClosedLabOrderActivityList = new List<OrderActivityCustomModel>(),
        //                    BasicInfoItems = listBasicInfo,
        //                    ChiefComplaintItems = listChiefComplaint,
        //                    HistoryPresentIllnessList = listHistoryPresentIllness,
        //                    ReviewSystemsList = listReviewSystems,
        //                    OrganSystemsList = listOrganSystems,
        //                    BodyAreasList = listBodyAreas,
        //                    AmtComplexityReviewedList = listAmtComplexityReviewed,
        //                    DiagnisosManagementList = listDiagnisosManagement,
        //                    RiskComplicationsList = listRiskComplications,
        //                    PatientCounseledList = listPatientCounseled,
        //                    ElecSignatureList = listElecSignature,
        //                    SetId = sId
        //                };
        //                return PartialView(PartialViews.EvaluationPView, summaryView);
        //            }
        //        }
        //    }
        //}

        public ActionResult Index2()
        {
            return View();
        }

        /*public ActionResult SaveEvaluationManagement(List<string> basicInfo, string patientId, string encounterId)
        {
            using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
            {
                foreach (var item in basicInfo)
                {
                    var oPatientEvaluationCustomModel = new PatientEvaluationCustomModel
                    {
                        CorporateId = Helpers.GetSysAdminCorporateID(),
                        FacilityId = Helpers.GetDefaultFacilityId(),
                        PatientId = Convert.ToInt32(patientId),
                        EncounterId = Convert.ToInt32(encounterId),
                        CategoryValue = item.Split('-')[2],
                        CodeValue = item.Split('-')[1],
                        ParentCodeValue = item.Split('-')[3],
                        Value = item.Split('-')[0],
                        CreatedBy = Helpers.GetLoggedInUserId(),
                        CreatedDate = Helpers.GetInvariantCultureDateTime()
                    };
                    oPatientDischargeSummaryBal.SaveEvaluationManagement(oPatientEvaluationCustomModel);
                }
            }
            return Json("");
        }*/
        public ActionResult SaveEvaluationManagement(List<string> basicInfo, long patientId, long encounterId, string estatus, string rowExists, int setId, string imagePath)
        {
            //var eNumId = -1;

            using (var bal = new PatientEvaluationBal())
            {
                if (encounterId == 0 && rowExists == "0" && patientId > 0)
                {
                    var currentEncounter = _eService.GetActiveEncounterByPateintId(Convert.ToInt32(patientId));
                    if (currentEncounter != null)
                        encounterId = currentEncounter.EncounterID;
                    else
                        return Json(-1, JsonRequestBehavior.AllowGet);
                }

                var result = bal.SavePatientEvaluationData(basicInfo, patientId, encounterId, Helpers.GetSysAdminCorporateID()
                    , Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId(), setId, estatus, imagePath);
                return Json(result.Status > 0 ? result.Status : -1, JsonRequestBehavior.AllowGet);


                //var evaluationSet = new PatientEvaluationSet()
                // {
                //     SetId = setId,
                //     PatientId = Convert.ToInt32(patientId),
                //     EncounterId = Convert.ToInt32(encounterId),
                //     CreatedBy = Helpers.GetLoggedInUserId(),
                //     CreatedDate = Helpers.GetInvariantCultureDateTime(),
                //     FormType = "Evaluation Management"
                // };
                //var id = oPatientEvaluationBal.SaveEvaluationSet(evaluationSet);
                //foreach (var item in basicInfo)
                //{
                //    var oPatientEvaluationModel = new PatientEvaluation
                //    {
                //        ExternalValue2 = Convert.ToString(id),
                //        CorporateId = Helpers.GetSysAdminCorporateID(),
                //        FacilityId = Helpers.GetDefaultFacilityId(),
                //        PatientId = Convert.ToInt32(patientId),
                //        EncounterId = Convert.ToInt32(encounterId),
                //        CategoryValue = item.Split('-')[2],
                //        CodeValue = item.Split('-')[1],
                //        ParentCodeValue = item.Split('-')[3],
                //        Value = item.Split('-')[0],
                //        CreatedBy = Helpers.GetLoggedInUserId(),
                //        CreatedDate = Helpers.GetInvariantCultureDateTime(),
                //        ExternalValue1 = estatus,
                //        ExternalValue3 = imagePath
                //    };

                //    eNumId = rowExists == "0"
                //        ? oPatientEvaluationBal.SaveEvaluationManagement(oPatientEvaluationModel)
                //        : oPatientEvaluationBal.UpdateEvaluationManagement(oPatientEvaluationModel);
                //}
            }
        }



        //public ActionResult EvaluationData(int? pId, string setId)
        //{
        //    //var logedInUser = Helpers.GetLoggedInUserId();

        //    var sId = setId ?? "0";
        //    if (pId == null || Convert.ToInt32(pId) == 0)
        //        return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch,
        //            new { messageId = Convert.ToInt32(MessageType.ViewEM) });

        //    var patientId = Convert.ToInt32(pId);
        //    using (var orderBal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
        //    {
        //        var enList = orderBal.GetEncountersListByPatientId(patientId);
        //        using (var medicalRecordbal = new MedicalRecordBal()) //Updated by Shashank on Oct 28, 2014
        //        {
        //            using (var medicalnotesbal = new MedicalNotesBal()) //Updated by Shashank on Oct 28, 2014
        //            {
        //                using (var medicalVitals = new MedicalVitalBal())
        //                {
        //                    using (var diagnosisBal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
        //                    {
        //                        var currentEncounterId = (enList != null && enList.Any() &&
        //                                                  enList.First().EncounterEndTime == null)
        //                            ? enList.First().EncounterID
        //                            : 0;

        //                        var medicalrecords = medicalRecordbal.GetMedicalRecord();
        //                        var medicalvitals = medicalVitals.GetCustomMedicalVitalsByPidEncounterId(patientId,
        //                            Convert.ToInt32(MedicalRecordType.Vitals), currentEncounterId);
        //                        var closedOrderActivityList = new List<OrderActivityCustomModel>();
        //                        var openOrderActivityList = new List<OrderActivityCustomModel>();
        //                        //added by Shashank on Dec 01 2014
        //                        var patientSummaryNotes =
        //                            medicalnotesbal.GetMedicalNotesByPatientIdEncounterId(patientId, currentEncounterId);
        //                        var allergiesList = medicalRecordbal.GetAlergyRecords(patientId,
        //                            Convert.ToInt32(MedicalRecordType.Allergies));

        //                        var orderStatus = OrderStatus.Open.ToString();
        //                        var openOrdersList = orderBal.GetPhysicianOrders(currentEncounterId, orderStatus);
        //                        var primarydiagnosisId = 0;
        //                        var diagnosisList = diagnosisBal.GetDiagnosisList(patientId, currentEncounterId);
        //                        if (diagnosisList.Any())
        //                        {
        //                            var diagnosisCustomModel =
        //                                diagnosisList.SingleOrDefault(
        //                                    x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary));
        //                            if (diagnosisCustomModel != null)
        //                            {
        //                                primarydiagnosisId = diagnosisCustomModel.DiagnosisID;
        //                            }
        //                        }
        //                        using (var orderActivityBal = new OrderActivityBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
        //                        {
        //                            var encounterActivitesobj = orderActivityBal.GetOrderActivitiesByEncounterId(Convert.ToInt32(currentEncounterId));
        //                            var encounterActivitesClosedListObj =
        //                                encounterActivitesobj.Where(
        //                                    x =>
        //                                        x.OrderActivityStatus != 0 &&
        //                                        x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)).OrderBy(x => x.ExecutedDate)
        //                                    .ToList();
        //                            var encounterActivitesOpenListObj =
        //                                encounterActivitesobj.Where(
        //                                    x =>
        //                                        x.OrderActivityStatus == 0 ||
        //                                        x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
        //                                       ).OrderBy(x => x.OrderScheduleDate)
        //                                    .ToList();
        //                            closedOrderActivityList = encounterActivitesClosedListObj;
        //                            openOrderActivityList = encounterActivitesOpenListObj;
        //                        }

        //                        #region Patient Evaluation Lists
        //                        List<PatientEvaluation> listBasicInfo;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listBasicInfo = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4629", setId);
        //                        }
        //                        List<PatientEvaluation> listChiefComplaint;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listChiefComplaint = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4610", setId);
        //                        }
        //                        List<PatientEvaluation> listHistoryPresentIllness;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listHistoryPresentIllness = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4613", setId);
        //                        }
        //                        List<PatientEvaluation> listReviewSystems;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listReviewSystems = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4615", setId);
        //                        }
        //                        List<PatientEvaluation> listOrganSystems;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listOrganSystems = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4616", setId);
        //                        }
        //                        List<PatientEvaluation> listBodyAreas;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listBodyAreas = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4617", setId);
        //                        }
        //                        List<PatientEvaluation> listAmtComplexityReviewed;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listAmtComplexityReviewed = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4624", setId);
        //                        }
        //                        List<PatientEvaluation> listDiagnisosManagement;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listDiagnisosManagement = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4625", setId);
        //                        }
        //                        List<PatientEvaluation> listRiskComplications;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listRiskComplications = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4626", setId);
        //                        }
        //                        List<PatientEvaluation> listPatientCounseled;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listPatientCounseled = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4627", setId);
        //                        }
        //                        List<PatientEvaluation> listElecSignature;
        //                        using (var oPatientDischargeSummaryBal = new PatientDischargeSummaryBal())
        //                        {
        //                            listElecSignature = oPatientDischargeSummaryBal.ListPatientEvaluation(patientId,
        //                                Convert.ToInt32(currentEncounterId), "4628", setId);
        //                        }
        //                        #endregion Patient Evaluation Lists
        //                        //added by Ashwani
        //                        var summaryView = new EvaluationViews
        //                        {
        //                            //EncounterOrder = new OpenOrder(),
        //                            //IsLabTest = false,
        //                            MedicalHistoryList = new List<MedicalHistoryCustomModel>(),
        //                            labOrderActivityList = new List<OrderActivityCustomModel>(),
        //                            CurrentLabOrderActivity = new OrderActivityCustomModel(),
        //                            OpenOrdersList = openOrdersList,
        //                            MedicalVitalList = medicalvitals,
        //                            ClosedOrdersList =
        //                                orderBal.GetPhysicianOrders(currentEncounterId, OrderStatus.Closed.ToString()),
        //                            AlergyList = allergiesList,
        //                            //CurrentOrderActivity = new OrderActivity { OrderActivityStatus = 1 },
        //                            OrderActivityList = openOrderActivityList,
        //                            ClosedOrderActivityList = closedOrderActivityList,
        //                            //ClosedLabOrderActivityList = new List<OrderActivityCustomModel>(),
        //                            BasicInfoItems = listBasicInfo,
        //                            ChiefComplaintItems = listChiefComplaint,
        //                            HistoryPresentIllnessList = listHistoryPresentIllness,
        //                            ReviewSystemsList = listReviewSystems,
        //                            OrganSystemsList = listOrganSystems,
        //                            BodyAreasList = listBodyAreas,
        //                            AmtComplexityReviewedList = listAmtComplexityReviewed,
        //                            DiagnisosManagementList = listDiagnisosManagement,
        //                            RiskComplicationsList = listRiskComplications,
        //                            PatientCounseledList = listPatientCounseled,
        //                            ElecSignatureList = listElecSignature,
        //                            SetId = sId
        //                        };
        //                        return PartialView(PartialViews.EvaluationPView, summaryView);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


        public ActionResult GetSignatureData(int ecounterId, int patinetId, string setId)
        {
            using (var eBal = new PatientEvaluationBal())
            {
                var list = eBal.GetSignaturePath(ecounterId, patinetId, setId);
                return Json(list);

            }
        }


        public ActionResult SignatureEnableDisable(int setId, int patientId)
        {

            var logedInUser = Helpers.GetLoggedInUserId();
            using (var pEval = new PatientEvaluationBal())
            {
                var signedUser = pEval.GetCreatedByFromEvaluationSet(setId, patientId);
                if (logedInUser == signedUser)
                {
                    return Json("1");
                }
                else
                {
                    return Json("2");
                }
            }
        }


        public ActionResult EvaluationData(int? pId, string setId)
        {
            var sId = setId ?? "0";
            if (pId == null || Convert.ToInt32(pId) == 0)
                return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch, new { messageId = Convert.ToInt32(MessageType.ViewEM) });

            var patientId = Convert.ToInt32(pId);
            using (var orderBal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var enList = orderBal.GetEncountersListByPatientId(patientId);
                using (var medicalRecordbal = new MedicalRecordBal())
                {
                    using (var medicalVitals = new MedicalVitalBal())
                    {
                        var currentEncounterId = (enList != null && enList.Any() &&
                                                  enList.First().EncounterEndTime == null)
                            ? enList.First().EncounterID
                            : 0;

                        var medicalvitals = medicalVitals.GetCustomMedicalVitalsByPidEncounterId(patientId,
                            Convert.ToInt32(MedicalRecordType.Vitals), currentEncounterId);

                        var cOrderActs = new List<OrderActivityCustomModel>();
                        var oOrderActs = new List<OrderActivityCustomModel>();

                        var allergiesList = medicalRecordbal.GetAlergyRecords(patientId,
                            Convert.ToInt32(MedicalRecordType.Allergies));

                        var orderStatus = OrderStatus.Open.ToString();
                        var openOrdersList = orderBal.GetPhysicianOrders(currentEncounterId, orderStatus);

                        using (var bal = new OrderActivityBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
                        {
                            var encounterActivitesobj = bal.GetOrderActivitiesByEncounterId(Convert.ToInt32(currentEncounterId));
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
                        }

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

                        using (var bal = new PatientDischargeSummaryBal())
                        {

                            var result = bal.GetPatientEvaluationData(patientId, Convert.ToInt32(currentEncounterId),
                                evGcCategories, setId);
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
                        }

                        #endregion Patient Evaluation Lists

                        var summaryView = new EvaluationViews
                        {
                            MedicalHistoryList = new List<MedicalHistoryCustomModel>(),
                            labOrderActivityList = new List<OrderActivityCustomModel>(),
                            CurrentLabOrderActivity = new OrderActivityCustomModel(),
                            OpenOrdersList = openOrdersList,
                            MedicalVitalList = medicalvitals,
                            ClosedOrdersList = orderBal.GetPhysicianOrders(currentEncounterId, OrderStatus.Closed.ToString()),
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

                        //var viewName = $"{partialViewPath}{PartialViews.EvaluationPView}";

                        //var sView = RenderPartialViewToStringBase(viewName, summaryView);
                        //var jsonData = new { sView, imageSource };
                        //var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                        //jsonResult.MaxJsonLength = int.MaxValue;
                        //return jsonResult;
                    }
                }
            }
        }

    }
}