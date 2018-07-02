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
    public class DischargeController : BaseController
    {
        private readonly IDischargeSummaryDetailService _service;
        private readonly IPatientDischargeSummaryService _pdsService;
        private readonly IDiagnosisService _diaService;
        private readonly IMedicalNotesService _mnService;
        private readonly IMedicalVitalService _mvService;
        private readonly IOpenOrderService _ooService;
        private readonly IGlobalCodeService _gService;

        public DischargeController(IDischargeSummaryDetailService service, IPatientDischargeSummaryService pdsService, IDiagnosisService diaService, IMedicalNotesService mnService, IMedicalVitalService mvService, IOpenOrderService ooService, IGlobalCodeService gService)
        {
            _service = service;
            _pdsService = pdsService;
            _diaService = diaService;
            _mnService = mnService;
            _mvService = mvService;
            _ooService = ooService;
            _gService = gService;
        }


        /// <summary>
        ///     Get the details of the PatientDischargeSummary View in the Model PatientDischargeSummary such as
        ///     PatientDischargeSummaryList, list of countries etc.
        /// </summary>
        /// <param name="patientId">
        ///     The patient Id.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter Id.
        /// </param>
        /// <returns>
        ///     returns the actionresult in the form of current object of the Model PatientDischargeSummary to be passed to View
        ///     PatientDischargeSummary
        /// </returns>
        public ActionResult DischargePartialView(int? patientId, int? encounterId)
        {
            var diagnosislist = _diaService.GetDiagnosisList(
                Convert.ToInt32(patientId),
                Convert.ToInt32(encounterId));

            if (diagnosislist.Count > 0)
            {
                diagnosislist = diagnosislist.OrderByDescending(d => d.CreatedDate).ToList();
            }

            var complicationsList = _mnService.GetMedicalNotesByPatientIdEncounterId(Convert.ToInt32(patientId), Convert.ToInt32(encounterId)).Where(x => x.MedicalNotes.MarkedComplication).ToList();
            var labTestsList = _mvService.GetCustomMedicalVitalsByPidEncounterId(Convert.ToInt32(patientId), Convert.ToInt32(MedicalRecordType.Vitals), Convert.ToInt32(encounterId));

            var openOrdersList = _ooService.GetAllOrdersByEncounterId(Convert.ToInt32(encounterId));
            var patientDischargeSummary = _pdsService.GetPatientDischargeSummaryByEncounterId(Convert.ToInt32(patientId));

            var dischargeView = new PatientDischargeSummaryView
            {
                CurrentPatientDischargeSummary = patientDischargeSummary,
                DiagnosisList = diagnosislist,
                DischargeMedicationsList = openOrdersList.Where(x => x.CategoryId == Convert.ToInt32(OrderTypeCategory.Pharmacy) && x.Status != Convert.ToInt32(OrderStatus.Open).ToString()).ToList(),
                ComplicationsList = complicationsList,
                LabTestsList = new List<MedicalVitalCustomModel>(),
                MedicationsInHouseList = new List<OpenOrderCustomModel>(),
                PatientInstructionsList = new List<DropdownListData>(),
                ProceduresList = openOrdersList.Where(x => x.OrderType == Convert.ToInt32(OrderType.CPT).ToString()).ToList(),
                ActiveMedicalProblemsList = _service.GetDischargeSummaryDetailListByTypeId(Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.ActiveMedicalProblems))),
                TypeOfFollowupsList = _service.GetDischargeSummaryDetailListByTypeId(Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.TypesOfFollowup))),
                PatientInstructions = _service.GetDischargeSummaryDetailListByTypeId(Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.PatientInstructions)))
            };

            // Pass the View Model in ActionResult to View PatientDischargeSummary
            var gccCodes = _gService.GetGlobalCodesByCategoriesSp("960,961,962");

            var jsonvariable =
                new
                {
                    MedicalProblems = gccCodes.Where(x => x.GlobalCodeCategoryValue == "960").ToList(),
                    PatientInstructions = gccCodes.Where(x => x.GlobalCodeCategoryValue == "961").ToList(),
                    FollowupTypes = gccCodes.Where(x => x.GlobalCodeCategoryValue == "962").ToList(),
                    partialView =
                            this.RenderPartialViewToStringBase(PartialViews.DischargeSummaryTab, dischargeView)
                };


            //return this.PartialView(PartialViews.DischargeSummaryTab, dischargeView);
            return this.Json(jsonvariable, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     TODO The update discharge details.
        /// </summary>
        /// <param name="model">
        ///     TODO The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult UpdateDischargeDetails(PatientDischargeSummary model)
        {
            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CreatedDate = currentDateTime;
            }
            else
            {
                model.ModifiedBy = userId;
                model.Modifieddate = currentDateTime;
            }

            var result = _pdsService.SavePatientDischargeSummary(model);
            return this.Json(result);
        }

        /// <summary>
        ///     TODO The add discharge summary detail.
        /// </summary>
        /// <param name="model">
        ///     TODO The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult AddDischargeSummaryDetail(DischargeSummaryDetail model)
        {
            var result = _service.SaveDischargeSummaryDetail(model);
            switch (model.AssociatedTypeId)
            {
                case "960":
                    return this.PartialView(PartialViews.ActiveMedicareProblem, result);
                case "961":
                    return this.PartialView(PartialViews.PatientInstructions, result);
                case "962":
                    return this.PartialView(PartialViews.FollowsType, result);
            }

            return this.PartialView(PartialViews.DischargeDetailsListView, result);
        }

        /// <summary>
        /// TODO The delete discharge detail.
        /// </summary>
        /// <param name="id">TODO The id.</param>
        /// <param name="typeId">TODO The type id.</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult DeleteDischargeDetail(int id, string typeId)
        {
            var result = _service.DeleteDischargeDetail(id, typeId);
            return this.PartialView(PartialViews.DischargeDetailsListView, result);
        }

        /// <summary>
        ///     TODO The check duplicate summary detail.
        /// </summary>
        /// <param name="model">
        ///     TODO The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult CheckDuplicateSummaryDetail(DischargeSummaryDetail model)
        {
            var result = _service.CheckIfRecordAlreadyAdded(model.AssociatedId, model.AssociatedTypeId);
            return this.Json(result);
        }

        #region To Sort the Medicare Active Problem

        /// <summary>
        ///     TODO The sort medicare active problem.
        /// </summary>
        /// <param name="patientId">
        ///     TODO The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortMedicareActiveProblem(int? patientId, int? encounterId)
        {
            var lst = _service.GetDischargeSummaryDetailListByTypeId(
                    Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.ActiveMedicalProblems)));

            // Pass the View Model in ActionResult to View PatientDischargeSummary
            return this.PartialView(PartialViews.ActiveMedicareProblem, lst);
        }


        /// <summary>
        ///     TODO The sort follows up.
        /// </summary>
        /// <param name="patientId">
        ///     TODO The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortFollowsUp(int? patientId, int? encounterId)
        {
            var followsUpList = _service.GetDischargeSummaryDetailListByTypeId(Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.TypesOfFollowup)));

            // Pass the View Model in ActionResult to View PatientDischargeSummary
            return this.PartialView(PartialViews.FollowsType, followsUpList);
        }

        /// <summary>
        ///     TODO The sort patient instruction.
        /// </summary>
        /// <param name="patientId">
        ///     TODO The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortPatientInstruction(int? patientId, int? encounterId)
        {
            var patientInstructions = _service.GetDischargeSummaryDetailListByTypeId(Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.PatientInstructions)));

            // Pass the View Model in ActionResult to View PatientDischargeSummary
            return this.PartialView(PartialViews.PatientInstructions, patientInstructions);
        }


        /// <summary>
        ///     TODO The sort diagnosis grid.
        /// </summary>
        /// <param name="patientId">
        ///     TODO The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortDiagnosisGrid(int? patientId, int? encounterId)
        {
            var diagnosislist = _diaService.GetDiagnosisList(Convert.ToInt32(patientId), Convert.ToInt32(encounterId));
            var viewpath = string.Format("../Summary/{0}", PartialViews.EHRDiagnosisList);
            return this.PartialView(viewpath, diagnosislist);
        }

        /// <summary>
        ///     Binds the discharge encounter order list sorted.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult BindDischargeEncounterOrderListSorted(int patientId, int encId)
        {
            var openOrdersList = _ooService.GetAllOrdersByEncounterId(Convert.ToInt32(encId)).ToList();
            openOrdersList =
                openOrdersList.Where(x => x.OrderType == Convert.ToInt32(OrderType.CPT).ToString()).ToList();
            //orderBal.GetPhysicianOrders(encId, orderStatus);
            var viewpath = string.Format("../Summary/{0}", PartialViews.DischargeOpenOrderList);
            return this.PartialView(viewpath, openOrdersList);
        }

        /// <summary>
        ///     TODO The sort procedures performed grid.
        /// </summary>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortProceduresPerformedGrid(int? encounterId)
        {
            var openOrdersList = _ooService.GetAllOrdersByEncounterId(Convert.ToInt32(encounterId));
            return this.PartialView(PartialViews.LabOpenOrderList, openOrdersList);
        }

        /// <summary>
        ///     TODO The sort lab test.
        /// </summary>
        /// <param name="patientId">
        ///     TODO The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortLabTest(int? patientId, int? encounterId)
        {
            var labTestsList = _mvService.GetCustomMedicalVitalsByPidEncounterId(
               Convert.ToInt32(patientId),
               Convert.ToInt32(MedicalRecordType.Vitals),
               Convert.ToInt32(encounterId));
            return this.PartialView(PartialViews.LabTestList, labTestsList);
        }

        /// <summary>
        ///     Binds the discharge open order by sort.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult BindDischargeOpenOrderBySort(int patientId, int encId)
        {
            var openOrdersList = _ooService.GetAllOrdersByEncounterId(Convert.ToInt32(encId)).ToList();
            openOrdersList =
                openOrdersList.Where(
                    x =>
                    x.CategoryId == Convert.ToInt32(OrderTypeCategory.Pharmacy)
                    && x.Status != Convert.ToInt32(OrderStatus.Open).ToString()).ToList();
            //orderBal.GetPhysicianOrders(encId, orderStatus);
            var viewpath = string.Format("../Summary/{0}", PartialViews.DischargeOpenOrderList1);
            return this.PartialView(viewpath, openOrdersList);

        }

        /// <summary>
        ///     Binds the discharge medication by sort.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult BindDischargeMedicationBySort(int patientId, int encId)
        {
            var openOrdersList = _ooService.GetAllOrdersByEncounterId(Convert.ToInt32(encId)).ToList();
            openOrdersList =
                openOrdersList.Where(
                    x =>
                    x.CategoryId == Convert.ToInt32(OrderTypeCategory.Pharmacy)
                    && x.Status != Convert.ToInt32(OrderStatus.Open).ToString()).ToList();
            //orderBal.GetPhysicianOrders(encId, orderStatus);
            var viewpath = string.Format("../Summary/{0}", PartialViews.DischargeMedicationList);
            return this.PartialView(viewpath, openOrdersList);
        }


        #endregion
    }
}