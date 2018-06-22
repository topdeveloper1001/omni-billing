using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class RuleStepCustomModel : RuleStep
    {
        public string DataTypeText { get; set; }
        //public string LHSTText { get; set; }
        //public string LHSCText { get; set; }
        //public string LHSKText { get; set; }
        public string CompareTypeText { get; set; }
        //public string RHSTText { get; set; }
        //public string RHSCText { get; set; }
        //public string RHSKText { get; set; }
        public string RuleMasterCode { get; set; }
        public string RHSFromText { get; set; }
        public string SelectedSectionString { get; set; }
        public string CompareSectionString { get; set; }
        public string ConditionStartString { get; set; }
        public string ConditionEndString { get; set; }
    }
    public class RuleStepPreview
    {
        public string PreviewRule { get; set; }
    }
}
