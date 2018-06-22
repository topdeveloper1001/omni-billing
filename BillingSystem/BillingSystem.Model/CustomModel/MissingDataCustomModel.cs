
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class MissingDataCustomModel
    {
        public int PatientId { get; set; }
        public int InsuranceDetails { get; set; }
        public int PlanDetail { get; set; }
        public int PolicyDetail { get; set; }
        public int AuthorizationDetail { get; set; }
        public int MobileAvialable { get; set; }
        public string PersonName { get; set; }
        public string PersonEmirateId { get; set; }
        public string PersonMobileNumber { get; set; }
        public int EncounterDetail { get; set; }
        public string EncounterNumber { get; set; }
        public bool MissingData { get; set; }
    }
}
