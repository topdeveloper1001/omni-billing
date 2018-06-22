using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DiagnosisCodeView
    {
        public DiagnosisCode CurrentDiagnosisCode { get; set; }
        public List<DiagnosisCode> DiagnosisCodeList { get; set; }
        public int UserId { get; set; }
    }
}
