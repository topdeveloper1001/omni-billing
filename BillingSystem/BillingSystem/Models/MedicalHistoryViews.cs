using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class MedicalHistoryView
    {
        public MedicalHistory CurrentMedicalHistory { get; set; }
        public List<MedicalHistoryCustomModel> MedicalHistoryList { get; set; }
    }
}
