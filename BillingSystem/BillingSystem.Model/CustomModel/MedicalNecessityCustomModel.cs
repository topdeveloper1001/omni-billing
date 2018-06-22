using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingSystem.Model;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class MedicalNecessityCustomModel : MedicalNecessity
    {
        public string Description { get; set; }
    }
}
