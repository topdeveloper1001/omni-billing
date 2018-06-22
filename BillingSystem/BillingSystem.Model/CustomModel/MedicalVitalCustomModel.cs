
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class MedicalVitalCustomModel
    {
        public MedicalVital MedicalVital { get; set; }
        public string MedicalVitalName { get; set; }
        public string UnitOfMeasureName { get; set; }
        public string VitalAddedBy { get; set; }
        public DateTime VitalAddedOn { get; set; }

        public string BloodPressureSystolic { get; set; }
        public string BloodPressureDiastolic { get; set; }
        public string PressureCustom { get; set; }
        public string Temperature { get; set; }
        public string TemperatureUOM { get; set; }

        public string PulseRate { get; set; }
        public string Weight { get; set; }
        public string WeightUOM { get; set; }

        public string Glucose { get; set; }
        public string GlucoseUOM { get; set; }

        //For Charts
        public string Name { get; set; }
        public int VitalCode { get; set; }
        public string VitalName { get; set; }
        public string XAxis { get; set; }
        public decimal Average { get; set; }
        public decimal Maximum { get; set; }
        public decimal Minimum { get; set; }
        public decimal LowerLimit { get; set; }
        public decimal UpperLimit { get; set; }

        public string LabTestName { get; set; }
        public string LabTestValues { get; set; }
        public string LabTestRange { get; set; }

        public int MedicalVitalID2 { get; set; }
    }

    [NotMapped]
    public class RiskFactorViewModel
    {
        public decimal Weight { get; set; }
        public string WeightStatus { get; set; }
        public decimal BloodPressureSys { get; set; }
        public string BloodPressureSysStaus { get; set; }
        public decimal BloodPressureDis { get; set; }
        public string BloodPressureDisStatus { get; set; }
        public string Diabities { get; set; }
        public string Smoker { get; set; }
        public string Alcohlic { get; set; }
    }
}
