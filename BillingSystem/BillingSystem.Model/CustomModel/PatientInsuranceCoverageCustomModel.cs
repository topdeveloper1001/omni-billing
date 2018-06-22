using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class PatientInsuranceCustomModel : PatientInsurance
    {
        public string CompanyName { get; set; }
        public string PlanName { get; set; }
        public string PolicyName { get; set; }

        public int CompanyId2 { get; set; }
        public int Plan2 { get; set; }
        public int Policy2 { get; set; }
        public DateTime StartDate2 { get; set; }
        public DateTime EndDate2 { get; set; }
        public int PatientInsuranceId2 { get; set; }
        public string PersonHealthCareNumber2 { get; set; }
    }
}
