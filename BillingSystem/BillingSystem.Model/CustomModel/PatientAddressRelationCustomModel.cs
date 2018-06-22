using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
   public class PatientAddressRelationCustomModel
    {
         public PatientAddressRelation PatientAddressRelation { get; set; }
         public string PatientAddressRelationTypeName { get; set; }
         public string PatientCountryName { get; set; }
         public string PatientStateName { get; set; }
         public string PatientCityName { get; set; }
    }
}
