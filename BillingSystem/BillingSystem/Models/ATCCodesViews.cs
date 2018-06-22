using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class ATCCodesView
    {
     
        public ATCCodes CurrentATCCodes { get; set; }
        public List<ATCCodesCustomModel> ATCCodesList { get; set; }

    }
}
