using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;



namespace BillingSystem.Bal.BusinessAccess
{
    public class InsurancePlansService : IInsurancePlansService
    {
        private readonly IRepository<InsurancePlans> _repository;
        private readonly BillingEntities _context;

        public InsurancePlansService(IRepository<InsurancePlans> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Get Insurance Plans Number By Id
        /// </summary>
        /// <returns>Return the Insurance Company View Model</returns>
        public string GetInsurancePlanNumberById(string number)
        {
            if (!string.IsNullOrEmpty(number))
            {
                var planNumber = Convert.ToInt32(number);
                var m = _repository.Where(a => a.InsurancePlanId == planNumber).FirstOrDefault();
                return (m != null) ? m.PlanNumber : string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// Method to add the Insurance Plans in the database.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public int AddUpdateInsurancePlans(InsurancePlans m)
        {
            if (m.InsurancePlanId > 0)
            {
                var current = _repository.GetSingle(m.InsurancePlanId);
                m.CreatedBy = current.CreatedBy;
                m.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(m, m.InsurancePlanId);
            }
            else
                _repository.Create(m);
            return m.InsurancePlanId;
        }

        /// <summary>
        /// Gets the insurance plan by company identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool GetInsurancePlanByCompanyId(int id)
        {
            var isExist = _repository.Where(x => x.InsuranceCompanyId == id && x.IsDeleted != true).FirstOrDefault();
            if (isExist != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method to add the Insurance Plans in the database by Id.
        /// </summary>
        /// <param name="insurancePlanId"></param>
        /// <returns></returns>
        public InsurancePlans GetInsurancePlanById(int? insurancePlanId)
        {
            var m = _repository.Where(x => x.InsurancePlanId == insurancePlanId).FirstOrDefault();
            return m;
        }

        public List<InsurancePlans> GetInsurancePlansByCompanyId(int companyId, DateTime curreDateTime)
        {
            var list = _repository.Where(i => i.InsuranceCompanyId == companyId && !i.IsDeleted && i.IsActive).ToList();
            return list.Where(_ => _.PlanEndDate != null && _.PlanEndDate >= curreDateTime).ToList();
        }


        /// <summary>
        /// Method to Get the Available Beds from the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InsurancePlanCustomModel> GetInsurancePlanList(bool showIsActive, int fId, int cId, int userId)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pFId", fId);
            sqlParameters[1] = new SqlParameter("pCId", cId);
            sqlParameters[2] = new SqlParameter("pIsActive", showIsActive);
            sqlParameters[3] = new SqlParameter("pUserId", userId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetInsurancePlansByFacility.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var mList = ms.GetResultWithJson<InsurancePlanCustomModel>(JsonResultsArray.PlanResult.ToString());
                return mList;
            }
        }

        //function to validate plan name  and Plan Number
        public int ValidatePlanNamePlanNumber(string planName, string planNumber, int id)
        {
            var insuranceModelIfPlanNamePlanNumberMatch = _repository.Where(x => x.InsurancePlanId != id && x.PlanName.ToLower() == planName.ToLower() && x.PlanNumber.Equals(planNumber) && x.IsDeleted != true).FirstOrDefault() != null;
            if (insuranceModelIfPlanNamePlanNumberMatch)
                return 1;//1 means PlanName and PlanNumber matched
            var insuranceModelIfPlanNameMatch = _repository.Where(x => x.InsurancePlanId != id && x.PlanName.ToLower() == planName.ToLower() && x.IsDeleted != true).FirstOrDefault() != null;
            if (insuranceModelIfPlanNameMatch)
                return 2;//2 means PlanName  matched
            var insuranceModelIfPlanNumberMatch = _repository.Where(x => x.InsurancePlanId != id && x.PlanNumber.Equals(planNumber) && x.IsDeleted != true).FirstOrDefault() != null;
            if (insuranceModelIfPlanNumberMatch)
                return 3;//3 means PlanNumber number matched
            return 0;
        }


        //function to validate plan name  and Plan Number
        public int CheckDuplicateInsurancePlan(string planName, string planNumber, int id, long insCompanyId)
        {
            var samePlanNameAndNumber = _repository.Where(x => x.InsurancePlanId != id && x.PlanName.ToLower().Equals(planName.ToLower())
                                         && x.PlanNumber.Equals(planNumber) && x.IsDeleted != true && x.InsuranceCompanyId == insCompanyId).Any();
            if (samePlanNameAndNumber)
                return 1;//1 means PlanName and PlanNumber matched


            var planNameExists = _repository.Where(x => x.InsurancePlanId != id && x.PlanName.ToLower().Equals(planName.ToLower())
                                && x.IsDeleted != true && x.InsuranceCompanyId == insCompanyId).Any();
            if (planNameExists)
                return 2;//2 means PlanName  matched


            var planNumberExists = _repository.Where(x => x.InsurancePlanId != id
                                    && x.PlanNumber.Equals(planNumber) && x.IsDeleted != true && x.InsuranceCompanyId == insCompanyId).Any();
            if (planNumberExists)
                return 3;//3 means PlanNumber number matched

            return 0;
        }
    }
}
