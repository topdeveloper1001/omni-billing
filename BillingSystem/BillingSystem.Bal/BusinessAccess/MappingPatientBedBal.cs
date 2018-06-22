using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;

namespace BillingSystem.Bal
{
    public class MappingPatientBedBal : BaseBal
    {

        /// <summary>
        /// Get the GetBedRateCardsList
        /// </summary>
        /// <returns>Return the BedRateCard List</returns>
        public List<MappingPatientBed> GetMappingPatientBedList()
        {
            try
            {
                List<MappingPatientBed> lstMappingPatientBed;
                using (var mappingbedpatinetRep = UnitOfWork.MappingPatientBedRepository)
                {
                    lstMappingPatientBed = mappingbedpatinetRep.GetAll().ToList();
                }
                return lstMappingPatientBed;
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
            using (var bedMasterRepository = UnitOfWork.MappingPatientBedRepository)
            {
                if (mappPatientBed.MappingPatientBedId > 0)
                    bedMasterRepository.UpdateEntity(mappPatientBed, mappPatientBed.MappingPatientBedId);
                else
                    bedMasterRepository.Create(mappPatientBed);
            }
            return mappPatientBed.MappingPatientBedId;
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public MappingPatientBed GetMappingPatientBedById(int id)
        {
            using (var bedRateCardRep = UnitOfWork.MappingPatientBedRepository)
            {
                var bedMaster = new MappingPatientBed();
                bedMaster = bedRateCardRep.Where(x => x.MappingPatientBedId == id).FirstOrDefault();
                return bedMaster;
            }
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public MappingPatientBed GetMappingPatientBedByEncounterId(string id)
        {
            using (var bedRateCardRep = UnitOfWork.MappingPatientBedRepository)
            {
                var bedMaster = new MappingPatientBed();
                bedMaster = bedRateCardRep.Where(x => x.EncounterID == id && x.EndDate ==null).FirstOrDefault();
                return bedMaster;
            }
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public MappingPatientBed GetAllMappingPatientBedById(string id)
        {
            using (var bedRateCardRep = UnitOfWork.MappingPatientBedRepository)
            {
                var bedMaster = new MappingPatientBed();
                bedMaster = bedRateCardRep.Where(x => x.BedNumber == id && x.EndDate == null).FirstOrDefault();
                return bedMaster;
            }
        }
    }
}
