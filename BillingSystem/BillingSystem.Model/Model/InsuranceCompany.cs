//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    using System;
    using System.Collections.Generic;

    public partial class InsuranceCompany
    {
        [Key]
        public int InsuranceCompanyId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string InsuranceCompanyStreetAddress { get; set; }
        public string InsuranceCompanyStreetAddress2 { get; set; }
        public string InsuranceCompanyCity { get; set; }
        public Nullable<int> InsuranceCompanyState { get; set; }
        public Nullable<int> InsuranceCompanyCountry { get; set; }
        public string InsuranceCompanyZipCode { get; set; }
        public string InsuranceCompanyMainPhone { get; set; }
        public string InsuranceCompanyFax { get; set; }
        public string InsuranceCompanySecondPhone { get; set; }
        public string InsuranceCompanyPOBox { get; set; }
        public string InsuranceCompanyLicenseNumber { get; set; }
        public Nullable<System.DateTime> InsuranceCompanyLicenseNumberExpire { get; set; }
        public string InsuranceCompanyTypeLicense { get; set; }
        public string InsuranceCompanyRelated { get; set; }
        public string InsuranceCompanyMainContact { get; set; }
        public string InsuranceCompanyMainContactPhone { get; set; }
        public string InsuranceCompanyClaimsContact { get; set; }
        public string InsuranceCompanyClaimsContactPhone { get; set; }
        public string InsuranceCompanyAuthorizationContact { get; set; }
        public string InsuranceCompanyAuthorizationPhone { get; set; }
        public string InsuranceCompanyDenialsContact { get; set; }
        public string InsuranceCompanyDenialsPhone { get; set; }
        public string InsuranceCompanyEmailAddress { get; set; }
        public string InsuranceCompanyPayerID { get; set; }
        public Nullable<decimal> InsuranceCompanyTableNumber { get; set; }
        public string InsuranceCompanyTableName { get; set; }

        [MaxLength(10)]
        public string TPAId { get; set; }
        public int RemittancePayerId { get; set; }

        [MaxLength(50)]
        public string ClaimSubmission { get; set; }

        [MaxLength(50)]
        public string BillFileType { get; set; }

        [MaxLength(50)]
        public string ANSIPayer { get; set; }

        public bool Capitation { get; set; }

        public int FacilityId { get; set; }
        public int CorporateId { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
    }
}
