using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class BillingDashboardView
    {
        public List<DashboardChargesCustomModel> ClaimSubmmitedNumber { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedDollorAmount { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedInpatientNumber { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedDollorInpatientAmount { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedOutpatientNumber { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedDollorOutpatientAmount { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedERpatientNumber { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedEROutpatientAmount { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedAvgDollorAmount { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedAvgDollorAmountInPatinet { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedAvgDollorAmountOPPatinet { get; set; }
        public List<DashboardChargesCustomModel> ClaimSubmmitedAvgDollorAmountERPatinet { get; set; }

        public DateTime CurrentDate { get; set; }
        public int NumberofCurrentDay { get; set; }
    }
}