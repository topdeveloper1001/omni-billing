using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DiagnosisCustomModel : Diagnosis
    {
        public DiagnosisCustomModel()
        {
            //CurrentPatient = new PatientInfoCustomModel();
            PrimaryDiagnosisCode = new DiagnosisCode();
            SecondaryDiagnosisCode = new DiagnosisCode();
            IntiallyEnteredBy = true;
            ReviewedBy = true;
        }

        //public PatientInfoCustomModel CurrentPatient { get; set; }
        public DiagnosisCode PrimaryDiagnosisCode { get; set; }
        public DiagnosisCode SecondaryDiagnosisCode { get; set; }
        public bool IntiallyEnteredBy { get; set; }
        public bool ReviewedBy { get; set; }
        public string DiagnosisTypeName { get; set; }
        public bool IsPrimary { get; set; }
        public string EncounterNumber { get; set; }
        public string DrgCodeValue { get; set; }
        public string DrgCodeDescription { get; set; }
        public string EnteredBy { get; set; }
        public bool IsMajorCPT { get; set; }
        public string MajorCPTCodeId { get; set; }
        public Int32 PrimaryDiagnosisId { get; set; }
        public bool IsMajorDRG { get; set; }
    }
}
