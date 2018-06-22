using System.Collections.Generic;

namespace BillingSystem.Model.CustomModel
{
    public class AlergyView
    {
        public List<AlergyCustomModel> AlergyList { get; set; }
        public List<GlobalCodeCategory> AllergiesHistoriesGCC { get; set; }
        public List<GlobalCodes> AllergiesHistoryGC { get; set; }
        public MedicalHistoryView MedicalHistoryView { get; set; }
    }


    public class MedicalHistoryView
    {
        public MedicalHistory CurrentMedicalHistory { get; set; }
        public List<MedicalHistoryCustomModel> MedicalHistoryList { get; set; }
    }
}
