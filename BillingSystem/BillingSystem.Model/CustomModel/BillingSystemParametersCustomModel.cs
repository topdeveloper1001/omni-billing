using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class BillingSystemParametersCustomModel
    {
        public int Id { get; set; }
        public string FacilityNumber { get; set; }
        public int? CorporateId { get; set; }
        public decimal? BillHoldDays { get; set; }
        public decimal? ARGLacct { get; set; }
        public decimal? MgdCareGLacct { get; set; }
        public decimal? BadDebtGLacct { get; set; }
        public decimal? SmallBalanceGLacct { get; set; }
        public decimal? SmallBalanceAmount { get; set; }
        public decimal? SmallBalanceWriteoffDays { get; set; }
        public DateTime? OupatientCloseBillsTime { get; set; }
        public decimal? ERCloseBillsHours { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ExternalValue1 { get; set; }
        public string ExternalValue2 { get; set; }
        public string ExternalValue3 { get; set; }
        public string ExternalValue4 { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CPTTableNumber { get; set; }
        public string ServiceCodeTableNumber { get; set; }
        public string DRGTableNumber { get; set; }
        public string HCPCSTableNumber { get; set; }
        public string DiagnosisTableNumber { get; set; }
        public string DrugTableNumber { get; set; }
        public string BillEditRuleTableNumber { get; set; }
        public long DefaultCountry { get; set; }

        public string FacilityName { get; set; }
        public string Country { get; set; }
    }
}
