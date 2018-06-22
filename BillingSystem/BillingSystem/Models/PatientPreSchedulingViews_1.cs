using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class PatientPreSchedulingView
    {
     
        public PatientPreScheduling CurrentPatientPreScheduling { get; set; }
        public List<PatientPreSchedulingCustomModel> PatientPreSchedulingList { get; set; }

    }
}
