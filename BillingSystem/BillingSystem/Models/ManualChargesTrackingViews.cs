using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class ManualChargesTrackingView
    {
     
        public ManualChargesTracking CurrentManualChargesTracking { get; set; }
        public List<ManualChargesTrackingCustomModel> ManualChargesTrackingList { get; set; }

    }
}
