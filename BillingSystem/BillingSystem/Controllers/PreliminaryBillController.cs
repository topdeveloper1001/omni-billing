using System;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Models;
using System.Linq;

namespace BillingSystem.Controllers
{
    public class PreliminaryBillController : BaseController
    {
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
            using (var bal = new PreliminaryBillBal())
            {
                //Get Patient Info by Patient Id
                using (var pBal = new PatientInfoBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
                    bedTransactionView.PatientInfo = pBal.GetPatientDetailsByPatientId(patientId);

                using (var eBal = new EncounterBal())
                {
                    if (encounterId == 0)
                    {
                        var en = eBal.GetEncounterStateByPatientId(patientId);
                        if (en != null && en.EncounterID > 0)
                        {
                            encounterId = en.EncounterID;
                            bedTransactionView.EncounterId = encounterId;
                        }
                    }

                    var encounter = eBal.GetEncounterByEncounterId(encounterId);
                    if (encounter != null)
                    {
                        bedTransactionView.EncounterNumber = encounter.EncounterNumber;
                        var pType = encounter.EncounterPatientType;
                        bedTransactionView.EncounterType =
                            Helpers.GetEncounterPatientType(encounter.EncounterPatientType != null
                                ? Convert.ToInt32(encounter.EncounterPatientType)
                                : 0);
                    }
                }

                var list = bal.GetBedTransactionList(encounterId);

                var encounterTransactionBal = new BillActivityBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
                var encounterTransactionLst = encounterTransactionBal.GetBillActivitiesByEncounterId(encounterId);
                bedTransactionView.EncounterTransactionLst = encounterTransactionLst;
                if (encounterTransactionLst.Count > 0)
                {
                    var computed = encounterTransactionLst.Sum(m => m.GrossCharges);
                    if (computed.HasValue)
                        bedTransactionView.TotalCharges = Convert.ToString(computed.Value);
                }
                bedTransactionView.BedTransactionList = list;
            }
            return View(bedTransactionView);
        }
    }
}