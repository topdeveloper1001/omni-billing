using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using System;
using System.Web.Mvc;

namespace BillingSystem.Controllers
{
    public class EncounterDetailController : BaseController
    {
        //
        // GET: /EncounterDetail/
        /// <summary>
        /// Indexes the specified enc identifier.
        /// </summary>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult Index(string encId)
        {
            using (var bal = new EncounterBal())
            {
                using (var preliminaryBillBal = new PreliminaryBillBal())
                {
                    if (!string.IsNullOrEmpty(encId))
                    {
                        var encounterCustomModel = bal.GetEncounterDetailByEncounterID(Convert.ToInt32(encId));
                        var encounterTransactionBal = new BillActivityBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
                        var encounterTransactionLst = encounterTransactionBal.GetBillActivitiesByEncounterId(Convert.ToInt32(encId));
                        encounterCustomModel.EncounterBedTransaction = encounterTransactionLst;
                        //encounterCustomModel.EncounterBedTransaction =
                        //    preliminaryBillBal.GetBedTransactionByEncounterID(Convert.ToInt32(encId));
                        return View(encounterCustomModel);
                    }
                }
            }
            return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch);
        }
	}
}