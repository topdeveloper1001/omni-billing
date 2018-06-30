using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MappingPatientBedService : IMappingPatientBedService
    {
        private readonly IRepository<MappingPatientBed> _repository;

        public MappingPatientBedService(IRepository<MappingPatientBed> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get the GetBedRateCardsList
        /// </summary>
        /// <returns>Return the BedRateCard List</returns>
        public List<MappingPatientBed> GetMappingPatientBedList()
        {
            try
            {
                List<MappingPatientBed> lst;
                lst = _repository.GetAll().ToList();
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to add/Update the BedRateCard in the database.
        /// </summary>
        /// <param name="bedRateCardVm"></param>
        /// <returns></returns>
        public int AddUpdateMappingPatientBed(MappingPatientBed mappPatientBed)
        {
            if (mappPatientBed.MappingPatientBedId > 0)
                _repository.UpdateEntity(mappPatientBed, mappPatientBed.MappingPatientBedId);
            else
                _repository.Create(mappPatientBed);
            return mappPatientBed.MappingPatientBedId;
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public MappingPatientBed GetMappingPatientBedById(int id)
        {
            var bedMaster = new MappingPatientBed();
            bedMaster = _repository.Where(x => x.MappingPatientBedId == id).FirstOrDefault();
            return bedMaster;
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public MappingPatientBed GetMappingPatientBedByEncounterId(string id)
        {
            var bedMaster = new MappingPatientBed();
            bedMaster = _repository.Where(x => x.EncounterID == id && x.EndDate == null).FirstOrDefault();
            return bedMaster;
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public MappingPatientBed GetAllMappingPatientBedById(string id)
        {
            var bedMaster = new MappingPatientBed();
            bedMaster = _repository.Where(x => x.BedNumber == id && x.EndDate == null).FirstOrDefault();
            return bedMaster;
        }
    }
}
