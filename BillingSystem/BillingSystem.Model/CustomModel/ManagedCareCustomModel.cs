using System;

namespace BillingSystem.Model.CustomModel
{
    public class ManagedCareCustomModel
    {
        public int ManagedCareID { get; set; }
        public int? ManagedCareInsuranceID { get; set; }
        public int? ManagedCarePlanID { get; set; }
        public int? ManagedCarePolicyID { get; set; }
        public decimal? ManagedCareMultiplier { get; set; }
        public decimal? ManagedCareInpatientDeduct { get; set; }
        public decimal? ManagedCareOutpatientDeduct { get; set; }
        public decimal? ManagedCarePerDiems { get; set; }
        public int? CorporateID { get; set; }
        public int? FacilityID { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string InsuranceCompany { get; set; }
        public string InsurancePlan { get; set; }
        public string InsuarancePolicy { get; set; }
    }
}
