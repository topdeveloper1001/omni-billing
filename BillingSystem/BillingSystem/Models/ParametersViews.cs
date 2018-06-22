using System.Collections.Generic;

using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ParametersView
    {
     
        public Parameters CurrentParameters { get; set; }
        public List<ParametersCustomModel> ParametersList { get; set; }

    }
}
