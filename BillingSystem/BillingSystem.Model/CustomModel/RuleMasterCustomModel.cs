
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class RuleMasterCustomModel : RuleMaster
    {
        public string RuleTypeString { get; set; }
        public string EffectiveStartDateString { get; set; }
        public string EffectiveEndDateString { get; set; }
        public string CorporateString { get; set; }
        public string FacilityString { get; set; }
        public string RoleIdString { get; set; }
        public string RuleSpecifiedForString { get; set; }
        public int RuleCode1 { get; set; }
    }
}
