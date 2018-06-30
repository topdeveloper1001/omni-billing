using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientAddressRelationService : IPatientAddressRelationService
    {
        private readonly IRepository<Country> _cRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<PatientAddressRelation> _repository;

        public PatientAddressRelationService(IRepository<Country> cRepository, IRepository<GlobalCodes> gRepository, IRepository<PatientAddressRelation> repository)
        {
            _cRepository = cRepository;
            _gRepository = gRepository;
            _repository = repository;
        }

        /// <summary>
        /// Get the PatientAddress
        /// </summary>
        public IEnumerable<PatientAddressRelationCustomModel> GetPatientAddressRelation(int patientId)
        {
            try
            {
                IEnumerable<PatientAddressRelationCustomModel> patientAddressRelationobj;
                var lstPatientAddressRelation =
                    _repository.Where(x => x.PatientID == patientId && (!x.IsDeleted)).ToList();
                var globalcodecategoryAddressRealtion =
                    Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.PatientRelationTypes).ToString();

                var countriesdata = _cRepository.GetAll().OrderBy(c => c.CountryName).ToList();
                var globalrelationType =
                    _gRepository.Where(
                        x => x.GlobalCodeCategoryValue == globalcodecategoryAddressRealtion);
                patientAddressRelationobj = (from p in lstPatientAddressRelation
                                             join fac in globalrelationType
                                             on p.PatientAddressRelationType equals Convert.ToInt32(fac.GlobalCodeValue) into joinedPrimaryFacility
                                             from fac in joinedPrimaryFacility.DefaultIfEmpty()
                                             let firstOrDefault = countriesdata.FirstOrDefault(_ => _.CountryID == p.CountryID)
                                             where firstOrDefault != null
                                             select new PatientAddressRelationCustomModel
                                             {
                                                 PatientAddressRelation = p,
                                                 PatientAddressRelationTypeName = fac != null ? fac.GlobalCodeName : string.Empty,
                                                 PatientCountryName = firstOrDefault.CountryName
                                             }).ToList();
                return patientAddressRelationobj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to add the facility in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddPatientAddressRelation(PatientAddressRelation model)
        {
            try
            {
                if (model.PatientAddressRelationID > 0)
                {
                    var current = _repository.GetSingle(model.PatientAddressRelationID);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    _repository.UpdateEntity(model, model.PatientAddressRelationID);
                }
                else
                    _repository.Create(model);
                return model.PatientAddressRelationID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PatientAddressRelation GetPatientRelationAddressById(int patientAddresssRelationId)
        {
            var patientPhoneModel = _repository.Where(x => x.PatientAddressRelationID == patientAddresssRelationId && !(bool)x.IsDeleted).FirstOrDefault();
            return patientPhoneModel;
        }
    }
}
