using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientInsuranceService : IPatientInsuranceService
    {
        private readonly IRepository<PatientInsurance> _repository;
        private readonly IRepository<InsurancePolices> _ipRepository;
        private readonly IRepository<InsuranceCompany> _icRepository;
        private readonly IRepository<InsurancePlans> _iplRepository;

        public PatientInsuranceService(IRepository<PatientInsurance> repository, IRepository<InsurancePolices> ipRepository, IRepository<InsuranceCompany> icRepository, IRepository<InsurancePlans> iplRepository)
        {
            _repository = repository;
            _ipRepository = ipRepository;
            _icRepository = icRepository;
            _iplRepository = iplRepository;
        }


        /// <summary>
        /// Get the Patient Insurance
        /// </summary>
        /// <returns>Return the PatientInsurance </returns>
        public PatientInsuranceCustomModel GetPatientInsurance(int patientId)
        {
            var vm = new PatientInsuranceCustomModel();
            var list = _repository.Where(x => x.PatientID == patientId && !x.IsDeleted).OrderByDescending(m => m.CreatedDate).ToList();
            if (list.Count > 0)
            {
                vm = new PatientInsuranceCustomModel
                {
                    InsuranceCompanyId = list[0].InsuranceCompanyId,
                    CompanyName = GetInsuranceCompanyNameById(list[0].InsuranceCompanyId),
                    InsurancePlanId = list[0].InsurancePlanId,
                    InsurancePolicyId = list[0].InsurancePolicyId,
                    PatientID = patientId,
                    Expirydate = list[0].Expirydate,
                    Startdate = list[0].Startdate,
                    PatientInsuraceID = list[0].PatientInsuraceID,
                    PlanName = GetInsurancePlanNameById(list[0].InsurancePlanId),
                    PolicyName = GetInsurancePolicyNameById(list[0].InsurancePolicyId),
                    PersonHealthCareNumber = list[0].PersonHealthCareNumber,
                };

                if (list.Count == 2)
                {
                    vm.PatientInsuranceId2 = list[1].PatientInsuraceID;
                    vm.CompanyId2 = list[1].InsuranceCompanyId;
                    vm.Plan2 = list[1].InsurancePlanId;
                    vm.Policy2 = list[1].InsurancePolicyId;
                    vm.PersonHealthCareNumber2 = list[1].PersonHealthCareNumber;
                    vm.StartDate2 = list[1].Startdate;
                    vm.EndDate2 = list[1].Expirydate;
                }
                else
                {
                    vm.StartDate2 = DateTime.Now;
                    vm.EndDate2 = DateTime.Now;
                }
            }
            return vm;
        }

        private string GetInsurancePlanNameById(int? id)
        {
            var planId = id == null ? 0 : Convert.ToInt32(id);
            var ins = _iplRepository.Where(e => e.InsurancePlanId == planId).FirstOrDefault();
            return ins != null ? ins.PlanName : string.Empty;
        }
        private string GetInsuranceCompanyNameById(int id)
        {
            var ins = _icRepository.Where(e => e.InsuranceCompanyId == id).FirstOrDefault();
            return ins != null ? ins.InsuranceCompanyName : string.Empty;
        }
        private string GetInsurancePolicyNameById(int? id)
        {
            var policyId = id == null ? 0 : Convert.ToInt32(id);
            var ins = _ipRepository.Where(e => e.InsurancePolicyId == policyId).FirstOrDefault();
            return ins != null ? ins.PolicyName : string.Empty;
        }

        /// <summary>
        /// Get the Patient Insurance
        /// </summary>
        /// <returns>Return the PatientInsurance </returns>
        public bool IsInsuranceComapnyInUse(int InsuranceComapnyid)
        {
            try
            {
                var result = _repository.GetAll().Any(x => x.InsuranceCompanyId == InsuranceComapnyid && !(bool)x.IsDeleted);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to add the PatientInsurance in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<int> SavePatientInsurance(PatientInsurance model)
        {
            if (model != null)
            {
                if (model.PatientInsuraceID > 0)
                {
                    var current = _repository.GetSingle(model.PatientInsuraceID);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    _repository.UpdateEntity(model, model.PatientInsuraceID);
                }
                else
                    _repository.Create(model);

                var list = _repository.Where(p => p.PatientID == model.PatientID).OrderBy(m => m.CreatedDate).Select(m => m.PatientInsuraceID).ToList();
                return list;
            }
            return new List<int>();
        }

        public PatientInsuranceCustomModel GetPrimaryPatientInsurance(int patientId, bool isPrimary)
        {
            var cm = new PatientInsuranceCustomModel
            {
                IsDeleted = false,
                IsActive = true,
                StartDate2 = DateTime.Now,
                EndDate2 = DateTime.Now
            };

            var result = isPrimary
                ? _repository.Where(p => p.PatientID == patientId && p.IsPrimary != false).FirstOrDefault()
                : _repository.Where(p => p.PatientID == patientId && p.IsPrimary != true).FirstOrDefault();

            if (result != null)
            {
                cm = new PatientInsuranceCustomModel
                {
                    InsuranceCompanyId = result.InsuranceCompanyId,
                    CompanyName = GetInsuranceCompanyNameById(result.InsuranceCompanyId),
                    InsurancePlanId = result.InsurancePlanId,
                    InsurancePolicyId = result.InsurancePolicyId,
                    PatientID = patientId,
                    Expirydate = result.Expirydate,
                    Startdate = result.Startdate,
                    PatientInsuraceID = result.PatientInsuraceID,
                    PlanName = GetInsurancePlanNameById(result.InsurancePlanId),
                    PolicyName = GetInsurancePolicyNameById(result.InsurancePolicyId),
                    PersonHealthCareNumber = result.PersonHealthCareNumber,
                    IsDeleted = result.IsDeleted,
                    IsActive = result.IsActive,
                };
            }
            return cm;
        }

        public PatientInsuranceCustomModel GetPatientInsuranceView(int patientId)
        {
            var customModel = GetPrimaryPatientInsurance(patientId, true);
            if (customModel != null)
            {
                var vm = GetPrimaryPatientInsurance(patientId, false);
                if (vm != null)
                {
                    customModel.PatientInsuranceId2 = vm.PatientInsuraceID;
                    customModel.CompanyId2 = vm.InsuranceCompanyId;
                    customModel.Plan2 = vm.InsurancePlanId;
                    customModel.Policy2 = vm.InsurancePolicyId;
                    customModel.PersonHealthCareNumber2 = vm.PersonHealthCareNumber;
                    customModel.StartDate2 = vm.Startdate;
                    customModel.EndDate2 = vm.Expirydate;
                    customModel.IsActive = vm.IsActive;
                    customModel.IsDeleted = vm.IsDeleted;

                }
            }
            return customModel;
        }

    }
}
