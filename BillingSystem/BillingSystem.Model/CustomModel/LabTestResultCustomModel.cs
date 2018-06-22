
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class LabTestResultCustomModel : LabTestResult
    {
        public string SpecifimenString { get; set; }
        public string GenderString { get; set; }
        public string MeasurementValueString { get; set; }
        public string AgeFromString { get; set; }
        public string AgeToString { get; set; }
    }
}
