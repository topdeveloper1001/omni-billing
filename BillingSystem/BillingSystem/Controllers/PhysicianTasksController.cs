// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicianTasksController.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The physician tasks controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Common.Common;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// The physician tasks controller.
    /// </summary>
    public class PhysicianTasksController : Controller
    {
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
            using (var encounterBal = new EncounterBal(Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrugTableNumber))
            {
                var facilitId = Helpers.GetDefaultFacilityId();


                var activeEncounter = new ActiveEncounter
                { 
                    // Patients List Where EncounterPatientType is In Patient
                    ActiveInPatientEncounterList =
                        encounterBal.GetActiveEncounters(new CommonModel
                        {
                            EncounterPatientType = Convert.ToInt32(EncounterPatientType.InPatient),
                            FacilityNumber = facilitId.ToString()
                        }),

                    // Patients List Where EncounterPatientType is Out Patient
                    ActiveOutPatientEncounterList =
                        encounterBal.GetActiveEncounters(new CommonModel
                        {
                            EncounterPatientType = Convert.ToInt32(EncounterPatientType.OutPatient),
                            FacilityNumber = facilitId.ToString()
                        }),

                    // Patients List Where EncounterPatientType is ER Patient
                    ActiveEmergencyEncounterList =
                        encounterBal.GetActiveEncounters(new CommonModel
                        {
                            EncounterPatientType = Convert.ToInt32(EncounterPatientType.ERPatient),
                            FacilityNumber = facilitId.ToString()
                        }).ToList().OrderBy(x => x.TriageSortingValue).ToList()
                };
                return View(activeEncounter);
            }
        }

        #endregion
    }
}