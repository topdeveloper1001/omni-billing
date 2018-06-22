using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class InsurancePlansBal : BaseBal
    {
        /// <summary>
        /// Get Insurance Plans Number By Id
        /// </summary>
        /// <returns>Return the Insurance Company View Model</returns>
        public string GetInsurancePlanNumberById(string number)
        {
            using (var insurancePlansRep = UnitOfWork.InsurancePlansRepository)
            {
                if (!string.IsNullOrEmpty(number))
                {
                    var planNumber = Convert.ToInt32(number);
                    var iQueryabletransactions = insurancePlansRep.Where(a => a.InsurancePlanId == planNumber).FirstOrDefault();
                    return (iQueryabletransactions != null) ? iQueryabletransactions.PlanNumber : string.Empty;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Method to add the Insurance Plans in the database.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public int AddUpdateInsurancePlans(InsurancePlans m)
        {
            using (var rep = UnitOfWork.InsurancePlansRepository)
            {
                if (m.InsurancePlanId > 0)
                {
                    var current = rep.GetSingle(m.InsurancePlanId);
                    m.CreatedBy = current.CreatedBy;
                    m.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(m, m.InsurancePlanId);
                }
                else
                    rep.Create(m);
                return m.InsurancePlanId;
            }
        }

        /// <summary>
        /// Gets the insurance plan by company identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool GetInsurancePlanByCompanyId(int id)
        {
            using (var rep = UnitOfWork.InsurancePlansRepository)
            {
                var isExist = rep.Where(x => x.InsuranceCompanyId == id && x.IsDeleted != true).FirstOrDefault();
                if (isExist != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Method to add the Insurance Plans in the database by Id.
        /// </summary>
        /// <param name="insurancePlanId"></param>
        /// <returns></returns>
        public InsurancePlans GetInsurancePlanById(int? insurancePlanId)
        {
            using (var insurancePlansRep = UnitOfWork.InsurancePlansRepository)
            {
                var insurancePlans = insurancePlansRep.Where(x => x.InsurancePlanId == insurancePlanId).FirstOrDefault();
                return insurancePlans;
            }
        }

        public List<InsurancePlans> GetInsurancePlansByCompanyId(int companyId, DateTime curreDateTime)
        {
            using (var rep = UnitOfWork.InsurancePlansRepository)
            {
                var list = rep.Where(i => i.InsuranceCompanyId == companyId && !i.IsDeleted && i.IsActive).ToList();
                return list.Where(_ => _.PlanEndDate != null && _.PlanEndDate >= curreDateTime).ToList();
            }
        }


        /// <summary>
        /// Method to Get the Available Beds from the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InsurancePlanCustomModel> GetInsurancePlanList(bool showIsActive, int fId, int cId, int userId)
        {
            using (var rep = UnitOfWork.InsurancePlansRepository)
                return rep.GetInsurancePlansByFacility(fId, cId, showIsActive, userId);
        }

        //function to validate plan name  and Plan Number
        public int ValidatePlanNamePlanNumber(string planName, string planNumber, int id)
        {
            using (var insuranceplanRep = UnitOfWork.InsurancePlansRepository)
            {
                var insuranceModelIfPlanNamePlanNumberMatch = insuranceplanRep.Where(x => x.InsurancePlanId != id && x.PlanName.ToLower() == planName.ToLower() && x.PlanNumber.Equals(planNumber) && x.IsDeleted != true).FirstOrDefault() != null;
                if (insuranceModelIfPlanNamePlanNumberMatch)
                    return 1;//1 means PlanName and PlanNumber matched
                var insuranceModelIfPlanNameMatch = insuranceplanRep.Where(x => x.InsurancePlanId != id && x.PlanName.ToLower() == planName.ToLower() && x.IsDeleted != true).FirstOrDefault() != null;
                if (insuranceModelIfPlanNameMatch)
                    return 2;//2 means PlanName  matched
                var insuranceModelIfPlanNumberMatch = insuranceplanRep.Where(x => x.InsurancePlanId != id && x.PlanNumber.Equals(planNumber) && x.IsDeleted != true).FirstOrDefault() != null;
                if (insuranceModelIfPlanNumberMatch)
                    return 3;//3 means PlanNumber number matched
                return 0;
            }
        }


        //function to validate plan name  and Plan Number
        public int CheckDuplicateInsurancePlan(string planName, string planNumber, int id, long insCompanyId)
        {
            using (var rep = UnitOfWork.InsurancePlansRepository)
            {
                var samePlanNameAndNumber = rep.Where(x => x.InsurancePlanId != id && x.PlanName.ToLower().Equals(planName.ToLower())
                                             && x.PlanNumber.Equals(planNumber) && x.IsDeleted != true && x.InsuranceCompanyId == insCompanyId).Any();
                if (samePlanNameAndNumber)
                    return 1;//1 means PlanName and PlanNumber matched


                var planNameExists = rep.Where(x => x.InsurancePlanId != id && x.PlanName.ToLower().Equals(planName.ToLower())
                                    && x.IsDeleted != true && x.InsuranceCompanyId == insCompanyId).Any();
                if (planNameExists)
                    return 2;//2 means PlanName  matched


                var planNumberExists = rep.Where(x => x.InsurancePlanId != id
                                        && x.PlanNumber.Equals(planNumber) && x.IsDeleted != true && x.InsuranceCompanyId == insCompanyId).Any();
                if (planNumberExists)
                    return 3;//3 means PlanNumber number matched

                return 0;
            }
        }
    }
}
