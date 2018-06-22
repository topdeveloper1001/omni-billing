
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ScrubReportCustomModel : ScrubReport
    {
        public string CompareTypeText { get; set; }
        public string RuleMasterDesc { get; set; }
        public string RuleStepDesc { get; set; }
        public string ErrorText { get; set; }

        public string CorrectedValue1 { get; set; }
        public string CorrectedValue2 { get; set; }
        public string CssClass { get; set; }

        public string LHSVDesc { get; set; }
        public string RHSVDesc { get; set; }

        public string ErrorResolutionTxt { get; set; }
        public string RuleGroup { get; set; }

        public string ConStartDesc { get; set; }
        public string ConEndDesc { get; set; }
        //public string RuleDataType { get; set; }

        public int BillHeaderId { get; set; }
        public int EncounterId { get; set; }
        public int PatientId { get; set; }

        public string LhsTooltip { get; set; }
        public string RhsTooltip { get; set; }
        public string RuleStepsValue { get; set; }
    }

    public class ScrubCorrectionModel
    {
        public int RetStatus { get; set; }
    }

}
