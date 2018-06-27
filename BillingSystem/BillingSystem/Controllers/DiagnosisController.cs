using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class DiagnosisController : BaseController
    {
        private readonly IBillActivityService _blService;
        private readonly ICPTCodesService _cptService;
        private readonly IEncounterService _eService;

        public DiagnosisController(IBillActivityService blService, ICPTCodesService cptService, IEncounterService eService)
        {
            _blService = blService;
            _cptService = cptService;
            _eService = eService;
        }

        /// <summary>
        /// Additionals the diagnosis.
        /// </summary>
        /// <param name="pId">The p identifier.</param>
        /// <param name="eId">The e identifier.</param>
        /// <returns></returns>
        public ActionResult AdditionalDiagnosis(int? pId, int? eId)
        {
            if (pId == null || eId == null)
                return RedirectToAction(ControllerNames.activeEncounterController, ActionResults.activeEncounterDefaultAction, new { message = 4 });

            var patientId = Convert.ToInt32(pId);
            var encounterId = Convert.ToInt32(eId);
            DiagnosisCustomModel diagnosisModel = null;
            PatientInfoCustomModel patientInfo = null;
            List<DiagnosisCustomModel> list = null;
            var isPrimary = true;
            var isMajorCPT = true;
            using (var bal = new DiagnosisBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var dModel = bal.GetNewDiagnosisByEncounterId(encounterId, patientId);
                var dList = bal.GetDiagnosisList(patientId, encounterId);
                isMajorCPT = !dList.Any(x => x.DiagnosisType == 4);
                dModel.IsMajorCPT = isMajorCPT;
                dModel.IsMajorDRG = !dList.Any(x => x.DiagnosisType == 3);
                list = dList != null && dList.Count > 0 ? dList : new List<DiagnosisCustomModel>();
                diagnosisModel = dModel ?? new DiagnosisCustomModel();
                patientInfo = bal.GetPatientDetailsByPatientId(patientId);
                isPrimary = list.Count == 0;
            }


            if (patientInfo != null)
            {
                diagnosisModel.PatientID = patientInfo.PatientInfo.PatientID;
                diagnosisModel.EncounterID = eId != null ? Convert.ToInt32(eId) : (patientInfo.CurrentEncounter != null ? patientInfo.CurrentEncounter.EncounterID : 0);
                diagnosisModel.CorporateID = patientInfo.CorporateId;
                diagnosisModel.FacilityID = patientInfo.PatientInfo.FacilityId;
            }

            diagnosisModel.IsPrimary = isPrimary;
            var favDiagnosisBal = new FavoritesBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var favDiagnosisList = favDiagnosisBal.GetFavoriteOrders(Helpers.GetLoggedInUserId());
            favDiagnosisList =
                favDiagnosisList.Where(_ => _.CategoryId == Convert.ToInt32(OrderType.Diagnosis).ToString())
                    .ToList();
            var diagnosisView = new DiagnosisView
            {
                CurrentDiagnosis = diagnosisModel,
                PatientInfo = patientInfo,
                DiagnosisList = list,
                FavoriteDiagnosisList = favDiagnosisList
            };
            return View(diagnosisView);
        }

        /// <summary>
        /// Indexes the specified p identifier.
        /// </summary>
        /// <param name="pId">The p identifier.</param>
        /// <param name="eId">The e identifier.</param>
        /// <returns></returns>
        public ActionResult Index(int? pId, int? eId)
        {
            if (pId == null || eId == null)
                return RedirectToAction(ControllerNames.activeEncounterController, ActionResults.activeEncounterDefaultAction, new { message = 3 });

            var patientId = Convert.ToInt32(pId);
            var encounterId = Convert.ToInt32(eId);
            DiagnosisCustomModel diagnosisModel;
            PatientInfoCustomModel patientInfo;
            List<DiagnosisCustomModel> list;
            var isPrimary = true;
            using (var bal = new DiagnosisBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var dModel = bal.GetNewDiagnosisByEncounterId(encounterId, patientId);
                var dList = bal.GetDiagnosisList(patientId, encounterId);
                var isMajorCpt = dList.All(x => x.DiagnosisType != 4);
                dModel.IsMajorCPT = isMajorCpt;
                dModel.IsMajorDRG = dList.All(x => x.DiagnosisType != 3);
                list = dList != null && dList.Count > 0 ? dList : new List<DiagnosisCustomModel>();
                diagnosisModel = dModel ?? new DiagnosisCustomModel();
                patientInfo = bal.GetPatientDetailsByPatientId(patientId);
                isPrimary = list.Count == 0;
            }


            if (patientInfo != null)
            {
                diagnosisModel.PatientID = patientInfo.PatientInfo.PatientID;
                diagnosisModel.EncounterID = eId != null ? Convert.ToInt32(eId) : (patientInfo.CurrentEncounter != null ? patientInfo.CurrentEncounter.EncounterID : 0);
                diagnosisModel.CorporateID = patientInfo.CorporateId;
                diagnosisModel.FacilityID = patientInfo.PatientInfo.FacilityId;
            }

            diagnosisModel.IsPrimary = isPrimary;
            var favDiagnosisBal = new FavoritesBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var favDiagnosisList = favDiagnosisBal.GetFavoriteOrders(Helpers.GetLoggedInUserId());
            favDiagnosisList =
                favDiagnosisList.Where(_ => _.CategoryId == Convert.ToInt32(OrderType.Diagnosis).ToString())
                    .ToList();
            var diagnosisView = new DiagnosisView
            {
                CurrentDiagnosis = diagnosisModel,
                PatientInfo = patientInfo,
                DiagnosisList = list,
                FavoriteDiagnosisList = favDiagnosisList
            };
            return View(diagnosisView);
        }

        /// <summary>
        /// Gets the diagnosis codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="diagCode">The List</param>
        /// <returns></returns>
        public JsonResult GetDiagnosisCodes(string text, List<string> diagCode)
        {
            if (!string.IsNullOrEmpty(text))
            {
                using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
                {
                    var userId = Helpers.GetLoggedInUserId();

                    var list = diagCode == null
                        ? bal.GetFilteredDiagnosisCodes(text, userId, Helpers.GetDefaultFacilityId())
                        : bal.GetFilteredDiagnosisCodes(text, userId, Helpers.GetDefaultFacilityId())
                        .Where(e => !diagCode.Contains(e.DiagnosisCode1));

                    var filteredList = list.Select(item => new
                    {
                        ID = item.DiagnosisCode1,
                        Menu_Title = string.Format("{0} - {1}", item.DiagnosisFullDescription, item.DiagnosisCode1),
                        Name = item.DiagnosisFullDescription
                    }).ToList();

                    return Json(filteredList, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null);
        }

        ///// <summary>
        ///// Saves the diagnosis code.
        ///// </summary>
        ///// <param name="model">The model.</param>
        ///// <returns></returns>
        //public JsonResult SaveDiagnosisCode(DiagnosisCustomModel model)
        //{
        //    int result = 0;
        //    using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
        //    {
        //        //var encounterDiagnosis = bal.GetDiagnosisList(Convert.ToInt32(model.PatientID), Convert.ToInt32(model.EncounterID));
        //        //if (encounterDiagnosis.Any(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary)))
        //        //{
        //        //    if (model.DiagnosisID == 0)
        //        //    {
        //        //        if (model.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary))
        //        //        {
        //        //            return Json("-1");
        //        //        }
        //        //    }
        //        //}
        //        //if (encounterDiagnosis.Any(x => x.DiagnosisID != model.DiagnosisID && x.DiagnosisCode == model.DiagnosisCode && (x.IsDeleted == null || x.IsDeleted ==false)))
        //        //{
        //        //        return Json("-2");
        //        //}

        //        var isPrimary = IsPrimary(model);
        //        if (isPrimary != 0)
        //            return Json(isPrimary.ToString());

        //        var userId = Helpers.GetLoggedInUserId();
        //        var physicicanId = (model.InitiallyEnteredByPhysicianId != null &&
        //                            Convert.ToInt32(model.InitiallyEnteredByPhysicianId) > 0)
        //            ? userId
        //            : 0;
        //        var coderId = (model.ReviewedByCoderID != null && Convert.ToInt32(model.ReviewedByCoderID) > 0)
        //            ? userId
        //            : 0;
        //        model.InitiallyEnteredByPhysicianId = physicicanId;
        //        model.ReviewedByCoderID = coderId;
        //        model.ReviewedByPhysicianID = coderId == 0 ? physicicanId : 0;

        //        if (model.DiagnosisID == 0)
        //        {
        //            model.CreatedBy = userId;
        //            model.CreatedDate = Helpers.GetInvariantCultureDateTime();
        //        }
        //        else
        //        {
        //            model.ModifiedBy = userId;
        //            model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
        //        }
        //        var DRGCodeId = 0;
        //        if (!string.IsNullOrEmpty(model.DiagnosisCode) && model.DRGCodeID > 0)
        //        {
        //            DRGCodeId = Convert.ToInt32(model.DRGCodeID);
        //        }

        //        if (!string.IsNullOrEmpty(model.DiagnosisCode))
        //        {
        //            model.DRGCodeID = null;
        //            result = bal.SaveDiagnosis(model);
        //        }
        //        if (DRGCodeId > 0)
        //        {
        //            model.DRGCodeID = DRGCodeId;
        //            if (model.DiagnosisID == 0)
        //            {
        //                model.DiagnosisType = 3; //----For DRG Code
        //                result = bal.SaveDiagnosis(model);
        //            }
        //            else if (model.DiagnosisID != 0 && model.DiagnosisType == 0)
        //            {
        //                model.DiagnosisType = 3; //----For DRG Code
        //                result = bal.SaveDiagnosis(model);
        //            }
        //            else
        //            {
        //                var drGsave = new Diagnosis()
        //                {
        //                    DiagnosisID = 0,
        //                    DiagnosisType = 3,
        //                    CorporateID = model.CorporateID,
        //                    FacilityID = model.FacilityID,
        //                    PatientID = model.PatientID,
        //                    EncounterID = model.EncounterID,
        //                    DRGCodeID = model.DRGCodeID,
        //                    MedicalRecordNumber = model.MedicalRecordNumber,
        //                    DiagnosisCodeId = null,
        //                    DiagnosisCode = null,
        //                    DiagnosisCodeDescription = null,
        //                    Notes = model.Notes,
        //                    InitiallyEnteredByPhysicianId = model.InitiallyEnteredByPhysicianId,
        //                    ReviewedByCoderID = model.ReviewedByCoderID,
        //                    ReviewedByPhysicianID = model.ReviewedByPhysicianID,
        //                    CreatedBy = model.CreatedBy,
        //                    CreatedDate = model.CreatedDate,
        //                    ModifiedBy = model.ModifiedBy,
        //                    ModifiedDate = model.ModifiedDate,
        //                    IsDeleted = model.IsDeleted,
        //                    DeletedBy = model.DeletedBy,
        //                    DeletedDate = model.DeletedDate,
        //                };
        //                result = bal.SaveDiagnosis(drGsave);
        //            }
        //        }
        //    }
        //    return Json(result);
        //}

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetList(string patientId)
        {
            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var encounterobj = _eService.GetActiveEncounterByPateintId(Convert.ToInt32(patientId));
                var diagnosisList = new List<DiagnosisCustomModel>();
                if (encounterobj != null)
                {
                    diagnosisList = bal.GetDiagnosisList(Convert.ToInt32(patientId), encounterobj.EncounterID);
                }
                return PartialView(PartialViews.DiagnosisList, diagnosisList);
            }
        }
        /// <summary>
        /// Gets the Current Diagnosis list
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        public ActionResult GetUploadChargesCurrentDiagnosisList(string patientId, string encounterId)
        {
            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var diagnosisList = bal.GetDiagnosisList(Convert.ToInt32(patientId), Convert.ToInt32(encounterId));

                return PartialView(PartialViews.DiagnosisList, diagnosisList);
            }
        }
        /// <summary>
        /// Gets the diagnosis by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetDiagnosisById(string id)
        {
            DRGCodes drg = null;
            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var diagnosis = bal.GetDiagnosisById(id);
                if (diagnosis.DRGCodeID != null && (int)diagnosis.DRGCodeID > 0)
                    drg = GetDrgCodeById((int)diagnosis.DRGCodeID);
                var jsonResult = new
                {
                    id = diagnosis.DiagnosisID,
                    Notes = diagnosis.Notes,
                    type = diagnosis.DiagnosisType,
                    code = diagnosis.DiagnosisCode,
                    codeId = diagnosis.DiagnosisCodeId,
                    CodeDescription = diagnosis.DiagnosisCodeDescription,
                    initialEnteredBy = Convert.ToInt32(diagnosis.InitiallyEnteredByPhysicianId),
                    reviewedByCoder = Convert.ToInt32(diagnosis.ReviewedByCoderID),
                    reviewedByPhysician = Convert.ToInt32(diagnosis.ReviewedByPhysicianID),
                    mrn = diagnosis.MedicalRecordNumber,
                    cId = Convert.ToInt32(diagnosis.CorporateID),
                    eId = Convert.ToInt32(diagnosis.EncounterID),
                    patientId = Convert.ToInt32(diagnosis.PatientID),
                    facilityId = Convert.ToInt32(diagnosis.FacilityID),
                    createdBy = Convert.ToInt32(diagnosis.CreatedBy),
                    CreatedDate = diagnosis.CreatedDate.GetDateTimeString24HoursFormat(),
                    DrgCodeId = Convert.ToInt32(diagnosis.DRGCodeID),
                    DrgCodeValue = drg != null ? drg.CodeNumbering : string.Empty,
                    DrgCodeDescription = drg != null ? drg.CodeDescription : string.Empty
                };
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Deletes the diagnosis.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult DeleteDiagnosis(int id)
        {
            //using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            //{
            //    var diagnosis = bal.GetDiagnosisById(Convert.ToString(id));
            //    if (diagnosis != null)
            //    {
            //        if (diagnosis.DiagnosisType != Convert.ToInt32(DiagnosisType.Primary))
            //        {
            //            var isDeleted = bal.DeleteDiagnosis(id);

            //            //  Changes done by Shashank
            //            /*
            //             * WHO: Shashank
            //             * WHEN: 30 March, 2016
            //             * WHY: The below code is added to delete the DRG from other tables i.e. OrderActivity and BillActivity
            //             * WHAT: First add the check only for the DRG type as only DRG goes to  OrderActivity and BillActivity
            //             * Second: The Diagnosis code will always be null for the DRG type so the code in the Bal will not run
            //             * we have to get the DRG code from the DRG table using the DRGCodeId column from the Diagnosis table
            //             */
            //            if (diagnosis.DiagnosisType == Convert.ToInt32(DiagnosisType.DRG) && Helpers.RunDeleteBillActivityInDiagnosis > 0)
            //            {
            //                var drgCodenumber = GetDrgCodeById(Convert.ToInt32(diagnosis.DRGCodeID));
            //                using (var billBal = new BillActivityBal())
            //                {
            //                    billBal.DeleteDiagnosisTypeBillActivity(
            //                        Convert.ToInt32(diagnosis.EncounterID),
            //                        Convert.ToInt32(diagnosis.PatientID),
            //                        drgCodenumber.CodeNumbering,
            //                        Helpers.GetLoggedInUserId());
            //                }
            //            }
            //            /****Delete the Diagnosis from other tables ends here***/


            //            var isPrimary = bal.CheckIfPrimaryDiagnosis(Convert.ToInt32(diagnosis.PatientID),
            //                Convert.ToInt32(diagnosis.EncounterID),
            //                (int)DiagnosisType.Primary);
            //            var isMajorCptDone = bal.CheckIfPrimaryDiagnosis(Convert.ToInt32(diagnosis.PatientID),
            //                Convert.ToInt32(diagnosis.EncounterID),
            //                (int)DiagnosisType.CPT);
            //            var isDrgDone = bal.CheckIfPrimaryDiagnosis(Convert.ToInt32(diagnosis.PatientID),
            //                Convert.ToInt32(diagnosis.EncounterID),
            //                (int)DiagnosisType.DRG);

            //            var jsonResult = new
            //            {
            //                isDeleted,
            //                isPrimary,
            //                isMajorCptDone,
            //                isDrgDone
            //            };
            //            return Json(jsonResult, JsonRequestBehavior.AllowGet);
            //        }
            //        return Json("-1", JsonRequestBehavior.AllowGet);
            //    }
            //    return Json("-1", JsonRequestBehavior.AllowGet);
            //}

            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var result = bal.DeleteCurrentDiagnosis(Helpers.GetLoggedInUserId(), id, Helpers.DefaultDrgTableNumber);
                if (result.ExecutionStatus > 0)
                {
                    var list = result.CurrentDiagnosisList.Select(f => new[] {Convert.ToString(f.DiagnosisID),Convert.ToString(f.DiagnosisCodeId), f.DiagnosisTypeName, f.DiagnosisCode, f.DiagnosisCodeDescription,
                    f.Notes, f.CreatedDate.HasValue?f.CreatedDate.Value.ToString("d"):string.Empty, Convert.ToString( f.EnteredBy), Convert.ToString(f.DiagnosisType) });

                    var jsonData = new
                    {
                        list,
                        status = result.ExecutionStatus,
                        IsPrimary = result.PrimaryExists,
                        IsMajorCPT = result.MajorCPTExists,
                        IsMajorDRG = result.MajorDRGExists
                    };
                    var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }

                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the current diagnosis list.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetCurrentDiagnosisList(string patientId)
        {
            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var list = bal.GetDiagnosisList(Convert.ToInt32(patientId));
                return PartialView(PartialViews.DiagnosisList, list);
            }
        }

        /// <summary>
        /// Adds the diagnosis by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult AddDiagnosisById(string id)
        {
            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var diagnosis = bal.GetDiagnosisById(id);

                var drgBal = new DRGCodesBal(Helpers.DefaultDrgTableNumber);
                var drg = drgBal.GetDrgCodesById(Convert.ToInt32(diagnosis.DRGCodeID));
                //var encounterobj = encounterBal.GetActiveEncounterByPateintId(Convert.ToInt32(diagnosis.PatientID));
                var jsonResult = new
                {
                    id = diagnosis.DiagnosisID,
                    diagnosis.Notes,
                    type = diagnosis.DiagnosisType,
                    code = diagnosis.DiagnosisType == (int)DiagnosisType.DRG ? drg.CodeNumbering : diagnosis.DiagnosisCode,
                    codeId = diagnosis.DiagnosisType == (int)DiagnosisType.DRG ? diagnosis.DRGCodeID : diagnosis.DiagnosisCodeId,
                    CodeDescription = diagnosis.DiagnosisType == (int)DiagnosisType.DRG ? drg.CodeDescription : diagnosis.DiagnosisCodeDescription,
                    initialEnteredBy = diagnosis.InitiallyEnteredByPhysicianId.HasValue ? diagnosis.InitiallyEnteredByPhysicianId.Value : 0,
                    reviewedByCoder = Convert.ToInt32(diagnosis.ReviewedByCoderID),
                    reviewedByPhysician = Convert.ToInt32(diagnosis.ReviewedByPhysicianID),
                    mrn = diagnosis.MedicalRecordNumber,
                    cId = Convert.ToInt32(diagnosis.CorporateID),
                    //eId = Convert.ToInt32(encounterobj.EncounterID),
                    patientId = Convert.ToInt32(diagnosis.PatientID),
                    facilityId = Convert.ToInt32(diagnosis.FacilityID),
                    createdBy = Convert.ToInt32(diagnosis.CreatedBy),
                    CreatedDate = Helpers.GetInvariantCultureDateTime().GetDateTimeString24HoursFormat(),//Convert.ToString(diagnosis.CreatedDate),
                    DrgCodeId = diagnosis.DRGCodeID,
                    DrgCodeValue = diagnosis.DiagnosisType == (int)DiagnosisType.DRG ? drg.CodeNumbering : diagnosis.DiagnosisCode,
                    DrgCodeDescription = diagnosis.DiagnosisType == (int)DiagnosisType.DRG ? drg.CodeDescription : diagnosis.DiagnosisCodeDescription,
                };
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
                //eId = Convert.ToInt32(diagnosis.EncounterID),
            }
        }

        /// <summary>
        /// Determines whether the specified model is primary.
        /// </summary>
        /// <param name="vm">The model.</param>
        /// <returns></returns>
        public int IsPrimary(DiagnosisCustomModel vm)
        {
            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var current = bal.GetDiagnosisList(Convert.ToInt32(vm.PatientID),
                    Convert.ToInt32(vm.EncounterID));

                if (current.Any(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary)) && vm.DiagnosisID == 0 && vm.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary))
                    return -1;
                if (current.Any(
                        x =>
                            x.EncounterID != null && (x.DiagnosisID != vm.DiagnosisID && x.DiagnosisCode == vm.DiagnosisCode && x.DiagnosisType == vm.DiagnosisType &&
                                                      (x.IsDeleted == null || x.IsDeleted == false) && (int)x.EncounterID == Convert.ToInt32(vm.EncounterID))))
                    return -2;
                return 0;
            }
        }

        /// <summary>
        /// Gets the DRG codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public JsonResult GetDrgCodes(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                using (var bal = new DRGCodesBal(Helpers.DefaultDrgTableNumber))
                {
                    var list = bal.GetFilteredDRGCodes(text);
                    var filteredList = list.Select(item => new
                    {
                        ID = item.DRGCodesId,
                        Menu_Title = string.Format("{0} - {1}", item.CodeDescription, item.CodeNumbering),
                        Name = item.CodeDescription,
                        Code = item.CodeNumbering
                    }).ToList();

                    return Json(filteredList, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null);
        }

        /// <summary>
        /// Gets the DRG code by identifier.
        /// </summary>
        /// <param name="drgCodeId">The DRG code identifier.</param>
        /// <returns></returns>
        private DRGCodes GetDrgCodeById(int drgCodeId)
        {
            using (var bal = new DRGCodesBal(Helpers.DefaultDrgTableNumber))
            {
                var drg = bal.GetDrgCodesById(drgCodeId);
                return drg;
            }
        }

        /// <summary>
        /// Gets the diagnosis by code identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <param name="Pid">The pid.</param>
        /// <param name="Eid">The eid.</param>
        /// <returns></returns>
        public ActionResult GetDiagnosisByCodeId(string Id, int Pid, int Eid)
        {
            using (var bal = new DiagnosisCodeBal(Helpers.DefaultDiagnosisTableNumber))
            {
                var Dtype = Convert.ToInt32(DiagnosisType.Primary);
                var drgBal = new DRGCodesBal(Helpers.DefaultDrgTableNumber);
                var diagnosisCode = bal.GetDiagnosisCodeByCodeId(Id);
                var drgCode = drgBal.GetDrgCodesobjByCodeValue(Id);
                using (var Dbal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
                {
                    var encounterDiagnosis = Dbal.GetDiagnosisList(Convert.ToInt32(Pid),
                        Convert.ToInt32(Eid));
                    if (encounterDiagnosis.Any(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary)))
                    {
                        Dtype =
                            Convert.ToInt32(!string.IsNullOrEmpty(diagnosisCode.DiagnosisCode1)
                                ? DiagnosisType.Secondary
                                : DiagnosisType.DRG);
                    }
                }
                var jsonResult = new
                {
                    id = 0,
                    Notes = string.Empty,
                    patientId = Convert.ToInt32(Pid),
                    type = Dtype,
                    code =
                        !string.IsNullOrEmpty(diagnosisCode.DiagnosisCode1)
                            ? diagnosisCode.DiagnosisCode1
                            : drgCode.CodeNumbering,
                    codeId = diagnosisCode.DiagnosisTableNumberId,
                    CodeDescription =
                        !string.IsNullOrEmpty(diagnosisCode.DiagnosisFullDescription)
                            ? diagnosisCode.DiagnosisFullDescription
                            : drgCode.CodeDescription,
                    CreatedDate = Helpers.GetInvariantCultureDateTime().GetDateTimeString24HoursFormat(),
                    DrgCodeId = !string.IsNullOrEmpty(diagnosisCode.DiagnosisCode1) ? 0 : drgCode.DRGCodesId,
                    DrgCodeValue =
                        !string.IsNullOrEmpty(diagnosisCode.DiagnosisCode1)
                            ? diagnosisCode.DiagnosisCode1
                            : drgCode.CodeNumbering,
                    DrgCodeDescription =
                        !string.IsNullOrEmpty(diagnosisCode.DiagnosisFullDescription)
                            ? diagnosisCode.DiagnosisFullDescription
                            : drgCode.CodeDescription,
                };
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the CPT codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public JsonResult GetCPTCodes(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {

                var list = _cptService.GetFilteredCodes(text, Helpers.DefaultCptTableNumber);
                var filteredList = list.Select(item => new
                {
                    ID = item.CPTCodesId,
                    Menu_Title = string.Format("{0} - {1}", item.CodeDescription, item.CodeNumbering),
                    Name = item.CodeDescription,
                    Code = item.CodeNumbering
                }).ToList();

                return Json(filteredList, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }

        /// <summary>
        /// Gets the favorites diagnosis.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFavoritesDiagnosis()
        {
            var userid = Helpers.GetLoggedInUserId();
            var favDiagnosisBal = new FavoritesBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var favDiagnosisList = favDiagnosisBal.GetFavoriteOrders(userid);
            favDiagnosisList = favDiagnosisList.Where(_ => _.CategoryId == Convert.ToInt32(OrderType.Diagnosis).ToString())
                    .ToList();

            return PartialView(PartialViews.PhyFavDiagnosisList, favDiagnosisList);
        }

        /// <summary>
        /// Checks the duplicate diagnosis code.
        /// </summary>
        /// <param name="diagnosisCode">The diagnosis code.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="diagnosisId">The diagnosis identifier.</param>
        /// <returns></returns>
        public bool CheckDuplicateDiagnosisCode(string diagnosisCode, int encounterId, int diagnosisId)
        {
            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var isExists = bal.CheckIfDuplicateDiagnosisAgainstCurrentEncounter(diagnosisCode, encounterId, diagnosisId);
                return isExists;
            }
        }

        /// <summary>
        /// Saves the diagnosis order activity.
        /// </summary>
        /// <param name="model">The DRG activity.</param>
        /// <returns></returns>
        public bool SaveDiagnosisOrderActivity(Diagnosis model)
        {
            try
            {
                var drgBal = new DRGCodesBal(Helpers.DefaultDrgTableNumber);
                var drgCode = drgBal.GetDrgCodeById(Convert.ToInt32(model.DRGCodeID));
                var orderactivitySave = new OrderActivity
                {
                    OrderActivityID = 0,
                    OrderType = Convert.ToInt32(OrderType.DRG),
                    OrderCode = drgCode,
                    OrderCategoryID = 0,
                    OrderSubCategoryID = 0,
                    OrderActivityStatus = 2,
                    CorporateID = model.CorporateID,
                    FacilityID = model.FacilityID,
                    PatientID = model.PatientID,
                    EncounterID = model.EncounterID,
                    MedicalRecordNumber = model.MedicalRecordNumber,
                    OrderID = model.DiagnosisID,
                    OrderBy = model.CreatedBy ?? model.ModifiedBy,
                    OrderActivityQuantity = 1,
                    OrderScheduleDate = model.CreatedDate ?? model.ModifiedDate,
                    PlannedBy = model.CreatedBy ?? model.ModifiedBy,
                    PlannedDate = model.CreatedDate ?? model.ModifiedDate,
                    PlannedFor = null,
                    ExecutedBy = model.CreatedBy ?? model.ModifiedBy,
                    ExecutedDate = model.CreatedDate ?? model.ModifiedDate,
                    ExecutedQuantity = 1,
                    ResultValueMin = null,
                    ResultValueMax = null,
                    ResultUOM = null,
                    Comments = model.Notes,
                    IsActive = true,
                    ModifiedBy = null,
                    ModifiedDate = null,
                    CreatedBy = model.CreatedBy ?? model.ModifiedBy,
                    CreatedDate = model.CreatedDate ?? model.ModifiedDate,
                };
                using (var bal = new OrderActivityBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
                {
                    var alreadyExist =
                        bal.GetOrderActivitiesByEncounterId(Convert.ToInt32(model.EncounterID)).FirstOrDefault(x => x.OrderType == 9);
                    if (alreadyExist != null)
                    {
                        orderactivitySave.OrderActivityID = alreadyExist.OrderActivityID;

                        var encbillActivity = _blService.GetBillActivitiesByEncounterId(Convert.ToInt32(model.EncounterID), Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber).ToList();
                        var encDrgactivity = encbillActivity.FirstOrDefault(x => x.ActivityType == "9");
                        if (encDrgactivity != null)
                        {
                            _blService.DeleteBillActivityFromBill(encDrgactivity.BillActivityID);
                        }

                    }
                    bal.AddUptdateOrderActivity(orderactivitySave);
                    bal.ApplyOrderActivityToBill(Convert.ToInt32(model.CorporateID), Convert.ToInt32(model.FacilityID), Convert.ToInt32(model.EncounterID), string.Empty, 0);

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public JsonResult SaveDiagnosisCustomCode(DiagnosisCustomModel model)
        {
            int result = 0;
            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var isPrimary = IsPrimary(model);
                if (isPrimary != 0) return Json(isPrimary, JsonRequestBehavior.AllowGet);
                var userId = Helpers.GetLoggedInUserId();
                var physicicanId = (model.InitiallyEnteredByPhysicianId != null && Convert.ToInt32(model.InitiallyEnteredByPhysicianId) > 0)
                    ? userId
                    : 0;
                var coderId = (model.ReviewedByCoderID != null && Convert.ToInt32(model.ReviewedByCoderID) > 0)
                    ? userId
                    : 0;


                model.InitiallyEnteredByPhysicianId = physicicanId;
                model.ReviewedByCoderID = coderId;
                model.ReviewedByPhysicianID = coderId == 0 ? physicicanId : 0;

                model.CreatedDate = model.CreatedDate ?? Helpers.GetInvariantCultureDateTime();
                if (model.DiagnosisID == 0)
                {
                    model.CreatedBy = userId;
                }
                else
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }

                var drgCodeId = model.DRGCodeID;
                var diagnosisCodeId = model.DiagnosisID;

                if (!string.IsNullOrEmpty(model.DiagnosisCode) && model.DRGCodeID > 0)
                    drgCodeId = Convert.ToInt32(model.DRGCodeID);

                var primaryDone = bal.CheckIfPrimaryDiagnosis(Convert.ToInt32(model.PatientID),
                    Convert.ToInt32(model.EncounterID), (int)DiagnosisType.Primary);

                //Primary or Secondary Diagnosis Cases
                if (!string.IsNullOrEmpty(model.DiagnosisCode) && (model.DiagnosisType == (int)DiagnosisType.Primary || model.DiagnosisType == (int)DiagnosisType.Secondary))
                {
                    model.DRGCodeID = null;
                    bool isExists = bal.CheckIfDuplicateDiagnosisAgainstCurrentEncounter(model.DiagnosisCode,
                        Convert.ToInt32(model.EncounterID), model.DiagnosisID);
                    if (!isExists)
                    {
                        result = bal.SaveDiagnosis(model);
                        primaryDone = true;
                    }
                    else
                        return Json(new { primaryDone, result = -2 }, JsonRequestBehavior.AllowGet);
                }

                var majorCptDone = false;
                var majorDrgDone = false;

                //DRG Case
                if (drgCodeId > 0)
                {
                    model.DRGCodeID = drgCodeId;
                    model.InitiallyEnteredByPhysicianId = physicicanId;
                    model.ReviewedByPhysicianID = physicicanId;
                    model.ReviewedByCoderID = coderId;
                    if (diagnosisCodeId != 0 && model.DiagnosisType == (int)DiagnosisType.DRG)
                        model.DiagnosisCode = null;

                    if (diagnosisCodeId == 0)
                    {
                        model.DiagnosisCodeId = null;
                        model.DiagnosisCode = null;
                        model.DiagnosisCodeDescription = null;
                    }
                    model.DiagnosisType = (int)DiagnosisType.DRG;
                    result = bal.SaveDiagnosis(model);
                    SaveDiagnosisOrderActivity(model);
                    majorDrgDone = true;
                }
                else
                {
                    majorDrgDone = bal.CheckIfPrimaryDiagnosis(Convert.ToInt32(model.PatientID),
                          Convert.ToInt32(model.EncounterID), (int)DiagnosisType.DRG);
                }



                //Major CPT Case 
                if (!string.IsNullOrEmpty(model.MajorCPTCodeId))
                {
                    model.InitiallyEnteredByPhysicianId = physicicanId;
                    model.ReviewedByPhysicianID = physicicanId;
                    model.ReviewedByCoderID = coderId;
                    model.DiagnosisType = (int)DiagnosisType.CPT;
                    model.DRGCodeID = null;
                    model.DiagnosisCodeId = null;
                    model.DiagnosisCode = model.MajorCPTCodeId;

                    var cptCodedesc = _cptService.GetCPTCodeDescription(model.MajorCPTCodeId, Helpers.DefaultCptTableNumber);
                    model.DiagnosisCodeDescription = cptCodedesc;

                    result = bal.SaveDiagnosis(model);
                    majorCptDone = true;
                }
                else
                {
                    majorCptDone = bal.CheckIfPrimaryDiagnosis(Convert.ToInt32(model.PatientID),
                          Convert.ToInt32(model.EncounterID), (int)DiagnosisType.CPT);
                }
                var currentDiagnosis = bal.GetCurrentDiagnosisData(Convert.ToInt32(model.PatientID),
                          Convert.ToInt32(model.EncounterID));

                var currenttabData = currentDiagnosis.Select(f => new[] {Convert.ToString(f.DiagnosisID),Convert.ToString(f.DiagnosisCodeId), f.DiagnosisTypeName, f.DiagnosisCode, f.DiagnosisCodeDescription,
                    f.Notes, f.CreatedDate.HasValue?f.CreatedDate.Value.ToString("d"):string.Empty, Convert.ToString( f.EnteredBy), Convert.ToString(f.DiagnosisType) });

                var jsonResult = new
                {
                    primaryDone,
                    result,
                    majorDrgDone,
                    majorCptDone,
                    currenttabData
                };
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetDiagnosisCodesData(List<string> diagCode)
        {
            var list = new List<DiagnosisCode>();
            if (diagCode.Count > 0)
            {

                using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
                {
                    list = bal.GetFilteredDiagnosis(diagCode);
                    var filteredList = list.Select(item => new
                    {
                        ID = item.DiagnosisCode1,
                        Menu_Title = string.Format("{0} - {1}", item.DiagnosisFullDescription, item.DiagnosisCode1),
                        Name = item.DiagnosisFullDescription
                    }).ToList();

                    return Json(filteredList, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null);
        }

        /// <summary>
        /// Sorts the diagnosis tab grid.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult SortDiagnosisTabGrid(int Pid, int Eid)
        {
            var daignosisBal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber);
            var diagnosislist = daignosisBal.GetDiagnosisList(Convert.ToInt32(Pid), Convert.ToInt32(Eid));
            var viewpath = string.Format("../Diagnosis/{0}", PartialViews.PhyAllOrders);
            return this.PartialView(PartialViews.DiagnosisList, diagnosislist);
        }

        public JsonResult GetFavoriteDiagnosisData()
        {
            using (var bal = new FavoritesBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var list = bal.GetFavoriteDiagnosisData(Helpers.GetLoggedInUserId());
                var jData = list.Select(f => new[] { Convert.ToString(f.UserDefinedDescriptionID), f.CategoryName, f.CodeId, f.CodeDesc, f.UserDefineDescription, f.CodeId });
                var s = Json(jData, JsonRequestBehavior.AllowGet);
                s.MaxJsonLength = int.MaxValue;
                s.RecursionLimit = int.MaxValue;
                return s;
            }
        }


        public JsonResult GetCurrentDiagnosisData(long patientId, long encounterId)
        {
            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var result = bal.GetCurrentDiagnosisData(patientId, encounterId);
                var list = result.Select(f => new[] {Convert.ToString(f.DiagnosisID),Convert.ToString(f.DiagnosisCodeId), f.DiagnosisTypeName, f.DiagnosisCode, f.DiagnosisCodeDescription,
                    f.Notes, f.CreatedDate.HasValue?f.CreatedDate.Value.ToString("d"):string.Empty, Convert.ToString( f.EnteredBy), Convert.ToString(f.DiagnosisType) });

                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
    }
}
