using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ChargesDashboardView
    {
        public List<DashboardChargesCustomModel> IPChargesCustomModel { get; set; }
        public List<DashboardChargesCustomModel> OPChargesCustomModel { get; set; }
        public List<DashboardChargesCustomModel> RoomChargesCustomModel { get; set; }
        public List<DashboardChargesCustomModel> ERChargesCustomModel { get; set; }

        public List<DashboardChargesCustomModel> IPRevenueCustomModel { get; set; }
        public List<DashboardChargesCustomModel> OPRevenueCustomModel { get; set; }
        public List<DashboardChargesCustomModel> ERRevenueCustomModel { get; set; }
        //public DashboardBudget CurrentDashboardBudget { get; set; }
        public int CurrentFacilityId { get; set; }
        //public int CurrentCorporateId { get; set; }
        public DateTime CurrentDate { get; set; }
        public int NumberofCurrentDay { get; set; }
    }
}