using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class MCOrderCodeRatesView
    {
     
        public MCOrderCodeRates CurrentMCOrderCodeRates { get; set; }
        public List<MCOrderCodeRatesCustomModel> MCOrderCodeRatesList { get; set; }

    }
}
