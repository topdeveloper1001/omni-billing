using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class MedicalRecordView
    {
     
        public MedicalRecord CurrentMedicalRecord { get; set; }
        public List<MedicalRecord> MedicalRecordList { get; set; }

    }
}
