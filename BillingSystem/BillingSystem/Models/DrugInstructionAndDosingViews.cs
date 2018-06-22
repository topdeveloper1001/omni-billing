using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DrugInstructionAndDosingView
    {
     
        public DrugInstructionAndDosing CurrentDrugInstructionAndDosing { get; set; }
        public List<DrugInstructionAndDosingCustomModel> DrugInstructionAndDosingList { get; set; }

    }
}
