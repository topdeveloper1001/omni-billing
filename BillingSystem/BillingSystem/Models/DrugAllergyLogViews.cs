using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DrugAllergyLogView
    {
     
        public DrugAllergyLog CurrentDrugAllergyLog { get; set; }
        public List<DrugAllergyLogCustomModel> DrugAllergyLogList { get; set; }

    }
}
