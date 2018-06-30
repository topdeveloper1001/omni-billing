using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ManagedCareService : IManagedCareService
    {
        private readonly IRepository<ManagedCare> _repository;
        private readonly IRepository<InsurancePolices> _ipRepository;
        private readonly IRepository<InsuranceCompany> _icRepository;
        private readonly IRepository<InsurancePlans> _iplRepository;

        public ManagedCareService(IRepository<ManagedCare> repository, IRepository<InsurancePolices> ipRepository, IRepository<InsuranceCompany> icRepository, IRepository<InsurancePlans> iplRepository)
        {
            _repository = repository;
            _ipRepository = ipRepository;
            _icRepository = icRepository;
            _iplRepository = iplRepository;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ManagedCareCustomModel> GetManagedCareListByCorporate(int corporateId)
        {
            try
            {
                var list = new List<ManagedCareCustomModel>();
                var lstManagedCare = corporateId > 0 ? _repository.Where(a => (a.CorporateID != null && (int)a.CorporateID == corporateId) && a.IsActive).ToList() : _repository.Where(a => a.IsActive).ToList();

                if (lstManagedCare.Count > 0)
                {
                    list.AddRange(lstManagedCare.Select(item => new ManagedCareCustomModel
                    {
                        CorporateID = item.CorporateID,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        FacilityID = item.FacilityID,
                        InsuarancePolicy = GetInsurancePolicyNameById(item.ManagedCarePolicyID),
                        InsuranceCompany = GetInsuranceCompanyNameById(Convert.ToInt32(item.ManagedCareInsuranceID)),
                        InsurancePlan = GetInsurancePlanNameById(item.ManagedCarePlanID),
                        IsActive = item.IsActive,
                        ManagedCareID = item.ManagedCareID,
                        ManagedCareInpatientDeduct = item.ManagedCareInpatientDeduct,
                        ManagedCareInsuranceID = item.ManagedCareInsuranceID,
                        ManagedCareMultiplier = item.ManagedCareMultiplier,
                        ManagedCareOutpatientDeduct = item.ManagedCareOutpatientDeduct,
                        ManagedCarePerDiems = item.ManagedCarePerDiems,
                        ManagedCarePlanID = item.ManagedCarePlanID,
                        ManagedCarePolicyID = item.ManagedCarePolicyID,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate
                    }));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="ManagedCare"></param>
        /// <returns></returns>
        public int AddUptdateManagedCare(ManagedCare ManagedCare)
        {
            if (ManagedCare.ManagedCareID > 0)
                _repository.UpdateEntity(ManagedCare, ManagedCare.ManagedCareID);
            else
                _repository.Create(ManagedCare);
            return ManagedCare.ManagedCareID;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public ManagedCare GetManagedCareByID(int? ManagedCareId)
        {
            var ManagedCare = _repository.Where(x => x.ManagedCareID == ManagedCareId).FirstOrDefault();
            return ManagedCare;
        }
    }
}

