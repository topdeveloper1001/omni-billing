using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class BillScrubberDashboardView
    {
        public List<DashboardChargesCustomModel> NumberofTotalClaimsPaidonRemittance { get; set; }
        public List<DashboardChargesCustomModel> NumberofTotalClaimsDeniedonRemittance{get;set;}
        public List<DashboardChargesCustomModel> ClaimsAcceptancePercentageFirstSubmission { get; set; }

        public DateTime CurrentDate { get; set; }
        public int NumberofCurrentDay { get; set; }
        public double ClaimsValue { get; set; }
    }
}