
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class MedicalHistoryCustomModel : MedicalHistory
    {
        public string Drug { get; set; }
        public string DrugDecription { get; set; }
        public string DrugDuration { get; set; }
        public string DrugVolume { get; set; }
        public string DrugDosage { get; set; }
        public string DrugFrequency { get; set; }
        public string DrugCode { get; set; }
    }
}
