using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using System;
using System.Web.Mvc;

namespace BillingSystem.Controllers
{
    public class EncounterDetailController : BaseController
    {
        private readonly IEncounterService _eService;
        private readonly IBillActivityService _blService;
        private readonly IPreliminaryBillService _plService;

        public EncounterDetailController(IEncounterService eService, IBillActivityService blService, IPreliminaryBillService plService)
        {
            _eService = eService;
            _blService = blService;
            _plService = plService;
        }

        //
        // GET: /EncounterDetail/
        /// <summary>
        /// Indexes the specified enc identifier.
        /// </summary>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult Index(string encId)
        {
            if (!string.IsNullOrEmpty(encId))
            {
                var encounterCustomModel = _eService.GetEncounterDetailByEncounterID(Convert.ToInt32(encId));
                var encounterTransactionLst = _blService.GetBillActivitiesByEncounterId(Convert.ToInt32(encId), Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
                encounterCustomModel.EncounterBedTransaction = encounterTransactionLst;
                return View(encounterCustomModel);
            }


            return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch);
        }
    }
}