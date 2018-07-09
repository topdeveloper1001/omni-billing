using BillingSystem.Model.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class FacilityCustomModel : Facility
    {
        public string CorporateName { get; set; }
        public string Region { get; set; }
        public List<FacilityContact> FacilityContact { get; set; }
    }
}
