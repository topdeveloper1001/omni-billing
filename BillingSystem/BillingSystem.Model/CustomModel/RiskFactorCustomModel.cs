using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public  class RiskFactorCustomModel
    {
        public string Weight { get; set; }
        public string WeightStatus { get; set; }

        public string BMI { get; set; }
        public string BMIStatus { get; set; }

        public string BloodPressure { get; set; }
        public string BloodPressureStatus { get; set; }

        public string Diabetes { get; set; }
        public string DiabetesStatus { get; set; }

        public string FamilyHistory { get; set; }
        public string Cholesterol { get; set; }
        public string CholesterolStatus { get; set; }

        public string HDL { get; set; }
        public string HDLStatus { get; set; }

        public List<AlergyCustomModel> AlergyList { get; set; }

        public bool Smoker { get; set; }
        public string Alcohol { get; set; }
    }
}
