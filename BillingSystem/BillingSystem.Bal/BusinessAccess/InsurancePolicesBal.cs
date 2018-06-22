using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Linq.Expressions;

namespace BillingSystem.Bal.BusinessAccess
{
    public class InsurancePolicesBal : BaseBal
    {
        /// <summary>
        /// Method to add Update the Insurance Polices in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InsurancePolicyCustomModel> AddUpdateInsurancePolices(InsurancePolices m)
        {
            using (var rep = UnitOfWork.InsurancePolicesRepository)
            {
                ///* 
                // * Adding Plan Details in Policy Table in Database
                // * On: 31 Jan, 2016
                // * By: Amit Jain
                // */
                using (var pRep = UnitOfWork.InsurancePlansRepository)
                {
                    var currentPlan = pRep.GetSingle(m.InsurancePlanId);
                    if (currentPlan != null)
                    {
                        m.PlanName = currentPlan.PlanName;
                        m.PlanNumber = currentPlan.PlanNumber;
                    }
                }

                if (m.InsurancePolicyId > 0)
                {
                    var current = rep.GetSingle(m.InsurancePolicyId);
                    m.CreatedBy = current.CreatedBy;
                    m.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(m, m.InsurancePolicyId);
                }
                else
                    rep.Create(m);

                if (m.InsurancePolicyId > 0)
                    return rep.GetInsurancePolicyListByFacility(m.FacilityId, m.CorporateId, true, m.CreatedBy);
            }
            return Enumerable.Empty<InsurancePolicyCustomModel>();
        }

        /// <summary>
        /// Method to Get the InsurancePolicy in the database.
        /// </summary>
        /// <returns></returns>
        public InsurancePolices GetInsurancePolicyById(int insurancePolicyId)
        {
            using (var rep = UnitOfWork.InsurancePolicesRepository)
            {
                var m = rep.Where(x => x.InsurancePolicyId == insurancePolicyId).FirstOrDefault();
                return m;
            }
        }

        /// <summary>
        /// Method to add the Insurance Plans in the database by Id.
        /// </summary>
        /// <param name="insuranceCompanyId"></param>
        /// <returns></returns>
        public List<InsurancePlans> GetInsurancePlanByCompanyId(int? insuranceCompanyId)
        {
            using (var insurancePlansRep = UnitOfWork.InsurancePlansRepository)
            {
                var list = insurancePlansRep.Where(x => x.InsuranceCompanyId == insuranceCompanyId).ToList();
                return list;
            }
        }

        /// <summary>
        /// Get the InsurancePolices
        /// </summary>
        /// <returns>Return the InsurancePolices View Model</returns>
        public IEnumerable<InsurancePolices> GetInsurancePolicesByPlanId(int planId, DateTime cuDateTime)
        {
            var currentDate = cuDateTime.Date;
            using (var rep = UnitOfWork.InsurancePolicesRepository)
            {
                var list =
                    rep.Where(
                        p => p.IsDeleted == false &&
                            p.InsurancePlanId == planId &&
                            (p.PolicyEndDate != null && (DateTime)p.PolicyEndDate >= currentDate)).ToList();
                return list;
            }
        }

        //function to validate policyName number and Policy number
        public int ValidatePolicyNamePolicyNumber(string policyName, string policyNumber, int id)
        {
            var result = 0;
            using (var rep = UnitOfWork.InsurancePolicesRepository)
            {
                if (rep.Where(x =>
                            x.InsurancePolicyId != id && x.PolicyName.ToLower().Equals(policyName.ToLower()) &&
                            x.PolicyNumber.Equals(policyNumber) && !x.IsDeleted).Any())
                    result = 1;             //1 means PolicyName and PolicyNumber matched

                if (rep.Where(x => x.InsurancePolicyId != id && x.PolicyName.ToLower().Equals(policyName.ToLower()) &&
                            !x.IsDeleted).Any())
                    result = 2;             //2 means PolicyName  matched

                if (rep.Where(x => x.InsurancePolicyId != id && x.PolicyNumber.Equals(policyNumber) && !x.IsDeleted).Any())
                    result = 3;             //3 means PolicyNumber number matched

                return result;
            }
        }

        /// <summary>
        /// Method to Get the Available Beds from the database.
        /// </summary>
        /// <returns></returns>
        public List<InsurancePolicyCustomModel> GetInsurancePolicyList()
        {
            var lstInsuranceCompany = new List<InsurancePolicyCustomModel>();
            using (var rep = UnitOfWork.InsurancePolicesRepository)
            {
                var insuranceCompanyList = rep.GetAll().Where(i => i.IsActive && i.IsDeleted == false).ToList();

                var insuranceCompanyBal = new InsuranceCompanyBal();
                var insurancePlansBal = new InsurancePlansBal();
                var managecareBal = new McContractBal();
                lstInsuranceCompany.AddRange(insuranceCompanyList.Select(item => new InsurancePolicyCustomModel
                {
                    InsurancePolices = item,
                    InsuranceCompanyName = insuranceCompanyBal.GetCompanyNameById(item.InsuranceCompanyId),
                    PlanName = GetInsurancePlanNameById(item.InsurancePlanId),
                    PlanNumber = insurancePlansBal.GetInsurancePlanNumberById(item.InsurancePlanId.ToString()),
                    ManagedCareCode = !string.IsNullOrEmpty(item.McContractCode) ? string.Format("{0} - {1}", item.McContractCode, managecareBal.GetManageCareName(item.McContractCode)) : string.Empty
                }));
                return lstInsuranceCompany;
            }
        }


        public bool CheckInsurancePolicyExist(int planId)
        {
            using (var rep = UnitOfWork.InsurancePolicesRepository)
            {
                var isExist = rep.Where(x => x.InsurancePlanId == planId && x.IsDeleted != true).FirstOrDefault();
                return isExist != null ? true : false;
            }
        }

        public IEnumerable<InsurancePolicyCustomModel> GetInsurancePolicyListByFacility(int fId, int cId, int userId)
        {
            using (var rep = UnitOfWork.InsurancePolicesRepository)
                return rep.GetInsurancePolicyListByFacility(fId, cId, true, userId);
        }
    }
}
