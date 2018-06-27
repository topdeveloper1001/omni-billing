using System;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{


    /// <summary>
    /// The physician tasks controller.
    /// </summary>
    public class PhysicianTasksController : Controller
    {
        private readonly IEncounterService _eService;

        public PhysicianTasksController(IEncounterService eService)
        {
            _eService = eService;
        }

        // GET: /PhysicianTasks/
        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            var facilitId = Helpers.GetDefaultFacilityId();


            var activeEncounter = new ActiveEncounter
            {
                // Patients List Where EncounterPatientType is In Patient
                ActiveInPatientEncounterList =
                    _eService.GetActiveEncounters(new CommonModel
                    {
                        EncounterPatientType = Convert.ToInt32(EncounterPatientType.InPatient),
                        FacilityNumber = facilitId.ToString()
                    }, Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber),

                // Patients List Where EncounterPatientType is Out Patient
                ActiveOutPatientEncounterList =
                    _eService.GetActiveEncounters(new CommonModel
                    {
                        EncounterPatientType = Convert.ToInt32(EncounterPatientType.OutPatient),
                        FacilityNumber = facilitId.ToString()
                    }, Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber),

                // Patients List Where EncounterPatientType is ER Patient
                ActiveEmergencyEncounterList =
                    _eService.GetActiveEncounters(new CommonModel
                    {
                        EncounterPatientType = Convert.ToInt32(EncounterPatientType.ERPatient),
                        FacilityNumber = facilitId.ToString()
                    }, Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber).ToList().OrderBy(x => x.TriageSortingValue).ToList()
            };
            return View(activeEncounter);
        }


        #endregion
    }
}