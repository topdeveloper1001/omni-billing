using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DrugInstructionAndDosingCustomModel : DrugInstructionAndDosing
    {
        public string AdministrationInstructionsStr { get; set; }
        public string RecommendedDosingStr { get; set; }
    }
}
