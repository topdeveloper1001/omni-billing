using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class InsuranceCompanyService : IInsuranceCompanyService
    {
        private readonly IRepository<InsuranceCompany> _repository;
        private readonly IRepository<PatientInsurance> _pRepository;

        public InsuranceCompanyService(IRepository<InsuranceCompany> repository, IRepository<PatientInsurance> pRepository)
        {
            _repository = repository;
            _pRepository = pRepository;
        }

        /// <summary>
        /// Get the Insurance Company
        /// </summary>
        /// <returns>Return the Insurance Company View Model</returns>
        public List<InsuranceCompany> GetInsuranceCompanies(bool showInActive, int facilityId = 0, int cId = 0)
        {
            var list = _repository.Where(a => !a.IsDeleted && a.IsActive == showInActive && (facilityId == 0 || a.FacilityId == facilityId)).ToList();
            return list;
        }

        /// <summary>
        /// Method to add/Update the Insurance Company in the database.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public int SaveInsuranceCompany(InsuranceCompany m)
        {
            if (m.InsuranceCompanyId > 0)
            {
                var current = _repository.GetSingle(m.InsuranceCompanyId);
                if (current != null)
                {
                    current.ModifiedBy = m.ModifiedBy;
                    current.ModifiedDate = m.ModifiedDate;

                    current.InsuranceCompanyName = m.InsuranceCompanyName;
                    current.InsuranceCompanyStreetAddress = m.InsuranceCompanyStreetAddress;
                    current.InsuranceCompanyStreetAddress2 = m.InsuranceCompanyStreetAddress2;
                    current.InsuranceCompanyCity = m.InsuranceCompanyCity;
                    current.InsuranceCompanyDenialsContact = m.InsuranceCompanyDenialsContact;
                    current.TPAId = m.TPAId;
                    current.InsuranceCompanyLicenseNumber = m.InsuranceCompanyLicenseNumber;

                    current.InsuranceCompanyTypeLicense = m.InsuranceCompanyTypeLicense;
                    current.InsuranceCompanyAuthorizationContact = m.InsuranceCompanyAuthorizationContact;
                    current.InsuranceCompanyRelated = m.InsuranceCompanyRelated;
                    current.InsuranceCompanyLicenseNumberExpire = m.InsuranceCompanyLicenseNumberExpire;
                    current.InsuranceCompanyZipCode = m.InsuranceCompanyZipCode;
                    current.InsuranceCompanyEmailAddress = m.InsuranceCompanyEmailAddress;

                    current.InsuranceCompanyClaimsContact = m.InsuranceCompanyClaimsContact;
                    current.InsuranceCompanyMainContact = m.InsuranceCompanyMainContact;
                    current.InsuranceCompanyPOBox = m.InsuranceCompanyPOBox;
                    current.InsuranceCompanyCountry = m.InsuranceCompanyCountry;
                    current.InsuranceCompanyClaimsContactPhone = m.InsuranceCompanyClaimsContactPhone;
                    current.InsuranceCompanyCountry = m.InsuranceCompanyCountry;
                    current.InsuranceCompanyDenialsPhone = m.InsuranceCompanyDenialsPhone;


                    current.InsuranceCompanyMainPhone = m.InsuranceCompanyMainPhone;
                    current.InsuranceCompanySecondPhone = m.InsuranceCompanySecondPhone;
                    current.InsuranceCompanyMainContactPhone = m.InsuranceCompanyMainContactPhone;
                    current.InsuranceCompanyAuthorizationPhone = m.InsuranceCompanyAuthorizationPhone;
                    current.InsuranceCompanyFax = m.InsuranceCompanyFax;

                    _repository.UpdateEntity(current, current.InsuranceCompanyId);
                }
            }
            else
                _repository.Create(m);
            return m.InsuranceCompanyId;
        }

        /// <summary>
        /// Method to add the Insurance Company in the database By Id.
        /// </summary>
        /// <param name="insuranceCompanyId"></param>
        /// <returns></returns>
        public InsuranceCompany GetInsuranceCompanyById(int? insuranceCompanyId)
        {
            var insuranceCompany = _repository.Where(x => x.InsuranceCompanyId == insuranceCompanyId).FirstOrDefault();
            return insuranceCompany;
        }

        //Function to het CompanyName By ID
        public string GetCompanyNameById(int? insuranceCompanyId)
        {
            var current = _repository.Where(a => a.InsuranceCompanyId == insuranceCompanyId).FirstOrDefault();
            return (current != null) ? current.InsuranceCompanyName : string.Empty;
        }

        //function to validate facility number and Licence number
        public int ValidateInsuranceCompanyNameInsuranceCompanyLicenseNumber(string insuranceCompanyName, string licenseNumber, int id)
        {
            var insuranceModelIfInsuranceCompanyNameInsuranceCompanyLicenseNumberMatch =
                _repository.Where(
                    x =>
                        x.InsuranceCompanyId != id &&
                        x.InsuranceCompanyName.ToLower() == insuranceCompanyName.ToLower() &&
                        x.InsuranceCompanyLicenseNumber.Equals(licenseNumber) && x.IsDeleted != true)
                    .FirstOrDefault() != null;
            if (insuranceModelIfInsuranceCompanyNameInsuranceCompanyLicenseNumberMatch)
                return 1;//1 means InsuranceCompanyName and InsuranceCompanyLicenseNumber matched
            var insuranceModelIfInsuranceCompanyNameMatch = _repository.Where(x => x.InsuranceCompanyId != id && x.InsuranceCompanyName.ToLower() == insuranceCompanyName.ToLower() && x.IsDeleted != true).FirstOrDefault() != null;
            if (insuranceModelIfInsuranceCompanyNameMatch)
                return 2;//2 means InsuranceCompanyName  matched
            var insuranceModelIfInsuranceCompanyLicenseNumberMatch = _repository.Where(x => x.InsuranceCompanyId != id && x.InsuranceCompanyLicenseNumber.Equals(licenseNumber) && x.IsDeleted != true).FirstOrDefault() != null;
            if (insuranceModelIfInsuranceCompanyLicenseNumberMatch)
                return 3;//3 means InsuranceCompanyLicenseNumber number matched
            return 0;
        }

        /// <summary>
        /// Gets the payer identifier.
        /// </summary>
        /// <param name="insuranceCompanyId">The insurance company identifier.</param>
        /// <returns></returns>
        public string GetPayerId(int insuranceCompanyId)
        {
            var payerId = string.Empty;
            var result = _repository.Where(i => i.InsuranceCompanyId == insuranceCompanyId).FirstOrDefault();
            if (result != null)
                payerId = result.InsuranceCompanyLicenseNumber;
            return payerId;
        }

        /// <summary>
        /// Gets the payer identifier.
        /// </summary>
        /// <param name="insuranceCompanyId">The insurance company identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public string GetPayerId(int insuranceCompanyId, int patientId)
        {
            var payerId = string.Empty;
            var result = _pRepository.Where(i => i.InsuranceCompanyId == insuranceCompanyId && i.PatientID == patientId).OrderBy(x => x.CreatedDate).FirstOrDefault();
            if (result != null)
                payerId = result.PersonHealthCareNumber.ToString();
            return payerId;
        }


        public InsuranceCompany GetInsuranceDetailsByPayorId(string payorId)
        {
            var m = _repository.Where(x => x.InsuranceCompanyLicenseNumber.Equals(payorId)).FirstOrDefault();
            return m;
        }
    }
}
