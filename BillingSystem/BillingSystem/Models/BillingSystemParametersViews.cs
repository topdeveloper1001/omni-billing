using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class BillingSystemParametersView
    {
        public BillingSystemParameters CurrentBillingSystemParameters { get; set; }
        public List<BillingSystemParametersCustomModel> BillingSystemParametersList { get; set; }
    }
}
