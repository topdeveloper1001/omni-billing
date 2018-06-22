using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class PatientCarePlanView
    {
     
        public PatientCarePlan CurrentPatientCarePlan { get; set; }
        public CarePlanTask CurrentCarePlanTask { get; set; }
        public List<PatientCarePlanCustomModel> PatientCarePlanList { get; set; }

    }
}
