using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientAddressRelationBal : BaseBal
    {
        /// <summary>
        /// Get the PatientAddress
        /// </summary>
        public IEnumerable<PatientAddressRelationCustomModel> GetPatientAddressRelation(int patientId)
        {
            try
            {
                IEnumerable<PatientAddressRelationCustomModel> patientAddressRelationobj;
                using (var patientAddressRelationRep = UnitOfWork.PatientAddressRelationRepository)
                {
                    using (var globalCodeRepository = UnitOfWork.GlobalCodeRepository)
                    {
                        var lstPatientAddressRelation =
                            patientAddressRelationRep.Where(x => x.PatientID == patientId && (!x.IsDeleted)).ToList();
                        var globalcodecategoryAddressRealtion =
                            Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.PatientRelationTypes).ToString();
                        var countrybal = new CountryBal();
                        var countriesdata = countrybal.GetCountries();
                        var globalrelationType =
                            globalCodeRepository.Where(
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
                    }
                    return patientAddressRelationobj;
                }
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
                using (var rep = UnitOfWork.PatientAddressRelationRepository)
                {
                    if (model.PatientAddressRelationID > 0)
                    {
                        var current = rep.GetSingle(model.PatientAddressRelationID);
                        model.CreatedBy = current.CreatedBy;
                        model.CreatedDate = current.CreatedDate;
                        rep.UpdateEntity(model, model.PatientAddressRelationID);
                    }
                    else
                        rep.Create(model);
                    return model.PatientAddressRelationID;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PatientAddressRelation GetPatientRelationAddressById(int patientAddresssRelationId)
        {
            using (var patientAddressRelationRep = UnitOfWork.PatientAddressRelationRepository)
            {
                var patientPhoneModel = patientAddressRelationRep.Where(x => x.PatientAddressRelationID == patientAddresssRelationId && !(bool)x.IsDeleted).FirstOrDefault();
                return patientPhoneModel;
            }
        }
    }
}
