using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ManagedCareBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ManagedCareCustomModel> GetManagedCareListByCorporate(int corporateId)
        {
            try
            {
                var list = new List<ManagedCareCustomModel>();
                using (var ManagedCareRep = UnitOfWork.ManagedCareRepository)
                {
                    var lstManagedCare = corporateId > 0 ? ManagedCareRep.Where(a => (a.CorporateID != null && (int)a.CorporateID == corporateId) && a.IsActive).ToList() : ManagedCareRep.Where(a => a.IsActive).ToList();

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
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Entity Name By Id
        /// </summary>
        /// <returns>Return the Entity Respository</returns>
        // public string GetManagedCareNameById(int? ManagedCareID)
        // {
        //   using (var ManagedCareRep = UnitOfWork.ManagedCareRepository)
        //   {
        //       var iQueryabletransactions = ManagedCareRep.Where(a => a.ManagedCareId == ManagedCareID).FirstOrDefault();
        //       return (iQueryabletransactions != null) ? iQueryabletransactions.ManagedCareName : string.Empty;
        //   }
        //}

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="ManagedCare"></param>
        /// <returns></returns>
        public int AddUptdateManagedCare(ManagedCare ManagedCare)
        {
            using (var ManagedCareRep = UnitOfWork.ManagedCareRepository)
            {
                if (ManagedCare.ManagedCareID > 0)
                    ManagedCareRep.UpdateEntity(ManagedCare, ManagedCare.ManagedCareID);
                else
                    ManagedCareRep.Create(ManagedCare);
                return ManagedCare.ManagedCareID;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public ManagedCare GetManagedCareByID(int? ManagedCareId)
        {
            using (var ManagedCareRep = UnitOfWork.ManagedCareRepository)
            {
                var ManagedCare = ManagedCareRep.Where(x => x.ManagedCareID == ManagedCareId).FirstOrDefault();
                return ManagedCare;
            }
        }
    }
}

