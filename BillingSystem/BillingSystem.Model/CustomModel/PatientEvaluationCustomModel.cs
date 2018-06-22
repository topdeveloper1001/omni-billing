using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class PatientEvaluationCustomModel : PatientEvaluation
    {
        public string GlobalCodeCategoryName { get; set; }
        public string SubSection { get; set; }

        public string GlobalCodeName { get; set; }
        public string EnteredBy { get; set; }
    }
     [NotMapped]
    public class BasicInfoView
    {
        public string NewPatient { get; set; }
        public string EstablishedPatient { get; set; }
        public string PatientNonverbal { get; set; }
        public string Family { get; set; }
        public string MedicalPersonnel { get; set; }
    }
     [NotMapped]
    public class ChiefComplaintReasonView
    {
        public string Consult { get; set; }
        public string Others { get; set; }
    }
}
