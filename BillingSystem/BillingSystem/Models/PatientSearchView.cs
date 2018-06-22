using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Models
{
    public class PatientSearchView :PatientInfo
    {
        public IEnumerable<PatientInfoCustomModel> PatientSearchList { get; set; }
    }
}