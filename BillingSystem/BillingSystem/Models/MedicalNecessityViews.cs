using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class MedicalNecessityView
    {
     
        public MedicalNecessity CurrentMedicalNecessity { get; set; }
        public List<MedicalNecessityCustomModel> MedicalNecessityList { get; set; }

    }
}
