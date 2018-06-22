using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ChargesReportViews
    {
        public List<ChargesReportCustomModel> IPChargesList { get; set; }
        public List<ChargesReportCustomModel> OPChargesList { get; set; }
        public List<ChargesReportCustomModel> ERChargesList { get; set; }
    }
}