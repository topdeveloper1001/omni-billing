using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class CorporateCustomModel
    {
        public List<Tabs> ParentTabs { get; set; }
        public int CorporateID { get; set; }
        public string CorporateNumber { get; set; }
        public string CorporateName { get; set; }
        public string StreetAddress { get; set; }
        public string StreetAddress2 { get; set; }
        public string CityID { get; set; }
        public string StateID { get; set; }
        public string CountryID { get; set; }
        public string CorporateZipCode { get; set; }
        public string CorporateMainPhone { get; set; }
        public string CorporateFax { get; set; }
        public string CorporateSecondPhone { get; set; }
        public string CorporatePOBox { get; set; }
        public string Email { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string DefaultCPTTableNumber { get; set; }
        public string DefaultHCPCSTableNumber { get; set; }
        public string DefaultDRUGTableNumber { get; set; }
        public string DefaultDRGTableNumber { get; set; }
        public string DefaultServiceCodeTableNumber { get; set; }
        public string DefaultDiagnosisTableNumber { get; set; }
        public string BillEditRuleTableNumber { get; set; }
    }
}
