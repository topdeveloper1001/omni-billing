using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class TechnicalSpecificationsView
    {
     
        public TechnicalSpecifications CurrentTechnicalSpecification { get; set; }
        public List<TechnicalSpecificationsCustomModel> TechnicalSpecificationsList { get; set; }

    }
}
