using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class AlergyCustomModel
    {
        public MedicalRecord CurrentAlergy { get; set; }
        public string AlergyName { get; set; }
        public string AlergyType { get; set; }
        public string AddedBy { get; set; }
        public List<OtherDrugAlergy> OtherDrugAllergyList { get; set; }
        public string DrugName { get; set; }
        public int MedicalRecordID2 { get; set; }
    }
    [NotMapped]
    public class OtherDrugAlergy : MedicalRecord
    {
        public string DrugName { get; set; }
        public string DrugCode { get; set; }
    }
}
