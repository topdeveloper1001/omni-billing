using System;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Models;
using System.Linq;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class PreliminaryBillController : BaseController
    {
        private readonly IEncounterService _eService;
        private readonly IPatientInfoService _piService;
        private readonly IPreliminaryBillService _plService;
        private readonly IBillActivityService _baService;

        public PreliminaryBillController(IEncounterService eService, IPatientInfoService piService, IPreliminaryBillService plService, IBillActivityService baService)
        {
            _eService = eService;
            _piService = piService;
            _plService = plService;
            _baService = baService;
        }

        //
        // GET: /PreliminaryBill/
        /// <summary>
        /// Indexes the specified e identifier.
        /// </summary>
        /// <param name="eId">The e identifier.</param>
        /// <param name="pId">The p identifier.</param>
        /// <returns></returns>
        public ActionResult Index(int? eId, int? pId)
        {
            if (pId == null)
                return RedirectToAction(ActionResults.activeEncounterDefaultAction, ControllerNames.activeEncounterController, new { message = 2 });

            var encounterId = Convert.ToInt32(eId);
            var patientId = Convert.ToInt32(pId);

            var bedTransactionView = new BedTransactionView { EncounterId = encounterId, PatientId = patientId };
            //Get Patient Info by Patient Id
            bedTransactionView.PatientInfo = _piService.GetPatientDetailsByPatientId(patientId);

            if (encounterId == 0)
            {
                var en = _eService.GetEncounterStateByPatientId(patientId);
                if (en != null && en.EncounterID > 0)
                {
                    encounterId = en.EncounterID;
                    bedTransactionView.EncounterId = encounterId;
                }
            }

            var encounter = _eService.GetEncounterByEncounterId(encounterId);
            if (encounter != null)
            {
                bedTransactionView.EncounterNumber = encounter.EncounterNumber;
                var pType = encounter.EncounterPatientType;
                bedTransactionView.EncounterType =
                    Helpers.GetEncounterPatientType(encounter.EncounterPatientType != null
                        ? Convert.ToInt32(encounter.EncounterPatientType)
                        : 0);
            }

            var list = _plService.GetBedTransactionList(encounterId);

            //var encounterTransactionBal = new BillActivityService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var encounterTransactionLst = _baService.GetBillActivitiesByEncounterId(encounterId, Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber);
            bedTransactionView.EncounterTransactionLst = encounterTransactionLst;
            if (encounterTransactionLst.Count > 0)
            {
                var computed = encounterTransactionLst.Sum(m => m.GrossCharges);
                if (computed.HasValue)
                    bedTransactionView.TotalCharges = Convert.ToString(computed.Value);
            }
            bedTransactionView.BedTransactionList = list;
            return View(bedTransactionView);
        }
    }
}