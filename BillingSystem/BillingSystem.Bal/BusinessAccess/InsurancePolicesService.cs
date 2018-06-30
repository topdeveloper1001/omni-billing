using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class InsurancePolicesService : IInsurancePolicesService
    {
        private readonly IRepository<InsurancePolices> _repository;
        private readonly IRepository<InsurancePlans> _ipRepository;
        private readonly IRepository<InsuranceCompany> _icRepository;
        private readonly IRepository<MCContract> _mcRepository;

        private readonly BillingEntities _context;

        public InsurancePolicesService(IRepository<InsurancePolices> repository, IRepository<InsurancePlans> ipRepository, IRepository<InsuranceCompany> icRepository, IRepository<MCContract> mcRepository, BillingEntities context)
        {
            _repository = repository;
            _ipRepository = ipRepository;
            _icRepository = icRepository;
            _mcRepository = mcRepository;
            _context = context;
        }


        /// <summary>
        /// Method to add Update the Insurance Polices in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InsurancePolicyCustomModel> AddUpdateInsurancePolices(InsurancePolices m)
        {
            var currentPlan = _ipRepository.GetSingle(m.InsurancePlanId);
            if (currentPlan != null)
            {
                m.PlanName = currentPlan.PlanName;
                m.PlanNumber = currentPlan.PlanNumber;
            }

            if (m.InsurancePolicyId > 0)
            {
                var current = _repository.GetSingle(m.InsurancePolicyId);
                m.CreatedBy = current.CreatedBy;
                m.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(m, m.InsurancePolicyId);
            }
            else
                _repository.Create(m);

            if (m.InsurancePolicyId > 0)
                return GetInsurancePolicyListByFacility(m.FacilityId, m.CorporateId, m.CreatedBy);
            return Enumerable.Empty<InsurancePolicyCustomModel>();
        }

        /// <summary>
        /// Method to Get the InsurancePolicy in the database.
        /// </summary>
        /// <returns></returns>
        public InsurancePolices GetInsurancePolicyById(int insurancePolicyId)
        {
            var m = _repository.Where(x => x.InsurancePolicyId == insurancePolicyId).FirstOrDefault();
            return m;
        }

        /// <summary>
        /// Method to add the Insurance Plans in the database by Id.
        /// </summary>
        /// <param name="insuranceCompanyId"></param>
        /// <returns></returns>
        public List<InsurancePlans> GetInsurancePlanByCompanyId(int? insuranceCompanyId)
        {
            var list = _ipRepository.Where(x => x.InsuranceCompanyId == insuranceCompanyId).ToList();
            return list;
        }

        /// <summary>
        /// Get the InsurancePolices
        /// </summary>
        /// <returns>Return the InsurancePolices View Model</returns>
        public IEnumerable<InsurancePolices> GetInsurancePolicesByPlanId(int planId, DateTime cuDateTime)
        {
            var currentDate = cuDateTime.Date;
            var list = _repository.Where(p => p.IsDeleted == false && p.InsurancePlanId == planId && (p.PolicyEndDate != null && (DateTime)p.PolicyEndDate >= currentDate)).ToList();
            return list;
        }

        //function to validate policyName number and Policy number
        public int ValidatePolicyNamePolicyNumber(string policyName, string policyNumber, int id)
        {
            var result = 0;
            if (_repository.Where(x =>
                        x.InsurancePolicyId != id && x.PolicyName.ToLower().Equals(policyName.ToLower()) &&
                        x.PolicyNumber.Equals(policyNumber) && !x.IsDeleted).Any())
                result = 1;             //1 means PolicyName and PolicyNumber matched

            if (_repository.Where(x => x.InsurancePolicyId != id && x.PolicyName.ToLower().Equals(policyName.ToLower()) &&
                        !x.IsDeleted).Any())
                result = 2;             //2 means PolicyName  matched

            if (_repository.Where(x => x.InsurancePolicyId != id && x.PolicyNumber.Equals(policyNumber) && !x.IsDeleted).Any())
                result = 3;             //3 means PolicyNumber number matched

            return result;
        }

        /// <summary>
        /// Method to Get the Available Beds from the database.
        /// </summary>
        /// <returns></returns>
        public List<InsurancePolicyCustomModel> GetInsurancePolicyList()
        {
            var lstInsuranceCompany = new List<InsurancePolicyCustomModel>();
            var insuranceCompanyList = _repository.GetAll().Where(i => i.IsActive && i.IsDeleted == false).ToList();

            lstInsuranceCompany.AddRange(insuranceCompanyList.Select(item => new InsurancePolicyCustomModel
            {
                InsurancePolices = item,
                InsuranceCompanyName = GetCompanyNameById(item.InsuranceCompanyId),
                PlanName = GetInsurancePlanNameById(item.InsurancePlanId),
                PlanNumber = GetInsurancePlanNumberById(item.InsurancePlanId.ToString()),
                ManagedCareCode = !string.IsNullOrEmpty(item.McContractCode) ? string.Format("{0} - {1}", item.McContractCode, GetManageCareName(item.McContractCode)) : string.Empty
            }));
            return lstInsuranceCompany;
        }
        private string GetManageCareName(string MCCode)
        {
            var mcOverviewObj = _mcRepository.Where(x => x.MCCode == MCCode);
            var firstFileString = mcOverviewObj.FirstOrDefault();
            return firstFileString != null ? firstFileString.ModelName : "";
        }
        private string GetCompanyNameById(int? insuranceCompanyId)
        {
            var current = _icRepository.Where(a => a.InsuranceCompanyId == insuranceCompanyId).FirstOrDefault();
            return (current != null) ? current.InsuranceCompanyName : string.Empty;
        }
        private string GetInsurancePlanNumberById(string number)
        {
            if (!string.IsNullOrEmpty(number))
            {
                var planNumber = Convert.ToInt32(number);
                var m = _ipRepository.Where(a => a.InsurancePlanId == planNumber).FirstOrDefault();
                return (m != null) ? m.PlanNumber : string.Empty;
            }
            return string.Empty;
        }

        public string GetInsurancePlanNameById(int? id)
        {
            var planId = id == null ? 0 : Convert.ToInt32(id);
            var ins = _repository.Where(e => e.InsurancePlanId == planId).FirstOrDefault();
            return ins != null ? ins.PlanName : string.Empty;
        }

        public bool CheckInsurancePolicyExist(int planId)
        {
            var isExist = _repository.Where(x => x.InsurancePlanId == planId && x.IsDeleted != true).FirstOrDefault();
            return isExist != null ? true : false;
        }

        public IEnumerable<InsurancePolicyCustomModel> GetInsurancePolicyListByFacility(int fId, int cId, int userId)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pFId", fId);
            sqlParameters[1] = new SqlParameter("pCId", cId);
            sqlParameters[2] = new SqlParameter("pIsActive", true);
            sqlParameters[3] = new SqlParameter("pUserId", userId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetInsurancePolicyListByFacility.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var mList = ms.GetResultWithJson<InsurancePolicyCustomModel>(JsonResultsArray.PolicyResult.ToString());
                return mList;
            }
        }
    }
}
