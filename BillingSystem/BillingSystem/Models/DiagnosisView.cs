using BillingSystem.Model.CustomModel;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class DiagnosisView
    {
        public PatientInfoCustomModel PatientInfo { get; set; }
        public DiagnosisCustomModel CurrentDiagnosis { get; set; }
        public List<DiagnosisCustomModel> DiagnosisList { get; set; }
        public List<DiagnosisCustomModel> previousDiagnosisList { get; set; }
        public List<FavoritesCustomModel> FavoriteDiagnosisList { get; set; }
    }
}