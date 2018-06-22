using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DrugInteractionsView
    {
     
        public DrugInteractions CurrentDrugInteractions { get; set; }
        public List<DrugInteractionsCustomModel> DrugInteractionsList { get; set; }

    }
}
