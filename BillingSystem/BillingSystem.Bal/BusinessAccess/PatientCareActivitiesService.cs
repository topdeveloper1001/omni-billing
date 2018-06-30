using System;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientCareActivitiesService : IPatientCareActivitiesService
    {
         private readonly IRepository<PatientAddressRelation> _repository;
        private readonly IRepository<PatientCareActivities> _pcaRepository;
        private readonly IRepository<Facility> _fRepository;

        public PatientCareActivitiesService(IRepository<PatientAddressRelation> repository, IRepository<PatientCareActivities> pcaRepository, IRepository<Facility> fRepository)
        {
            _repository = repository;
            _pcaRepository = pcaRepository;
            _fRepository = fRepository;
        }


        /// <summary>
        /// Adds the uptdate patient care activity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public int AddUptdatePatientCareActivity(int id, int facilityid, string status)
        {
            try
            {
                var objectToUpdate = GetPatientCarePlanActivity(id);
                objectToUpdate.AdministrativeOn = GetInvariantCultureDateTime(facilityid);
                objectToUpdate.ExtValue4 = status;
                objectToUpdate.ModeifiedDate = GetInvariantCultureDateTime(facilityid);
                objectToUpdate.ModifiedBy = 9001;
                objectToUpdate.AdministrativeOn = GetInvariantCultureDateTime(facilityid);
                _pcaRepository.UpdateEntity(objectToUpdate, id);

                return id;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        private DateTime GetInvariantCultureDateTime(int facilityid)
        {
            var facilityObj = _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault() != null ? _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault().FacilityTimeZone : TimeZoneInfo.Utc.ToString();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return convertedTime;
        }
        /// <summary>
        /// Gets the patient care plan activity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public PatientCareActivities GetPatientCarePlanActivity(int id)
        {
            return _pcaRepository.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}