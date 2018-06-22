// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientCareActivitiesBal.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The patient care activities bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System.Linq;

    using BillingSystem.Model;

    /// <summary>
    /// The patient care activities bal.
    /// </summary>
    public class PatientCareActivitiesBal : BaseBal
    {
        /// <summary>
        /// Adds the uptdate patient care activity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public int AddUptdatePatientCareActivity(int id, int facilityid,string status)
        {
            try
            {
                using (var patientCareActivityRep = this.UnitOfWork.PatientCareActivitiesRepository)
                {
                    var objectToUpdate = GetPatientCarePlanActivity(id);
                    objectToUpdate.AdministrativeOn = this.GetInvariantCultureDateTime(facilityid);
                    objectToUpdate.ExtValue4 = status;
                    objectToUpdate.ModeifiedDate = this.GetInvariantCultureDateTime(facilityid);
                    objectToUpdate.ModifiedBy = 9001;
                    objectToUpdate.AdministrativeOn = this.GetInvariantCultureDateTime(facilityid);
                    patientCareActivityRep.UpdateEntity(objectToUpdate, id);
                }

                return id;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the patient care plan activity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public PatientCareActivities GetPatientCarePlanActivity(int id)
        {
            using (var patientCareActivityRep = this.UnitOfWork.PatientCareActivitiesRepository)
            {
               return patientCareActivityRep.Where(x => x.Id == id).FirstOrDefault();
            }  
        }
    }
}