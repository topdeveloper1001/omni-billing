using System;
using BillingSystem.Model.CustomModel;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class DashboardView
    {
        //public string IsOccupied { get; set; }
        //public string Beds { get; set; }
        //public string TotalBeds { get; set; }
        //public string BedStatus { get; set; }
        //public string Room { get; set; }
        //public string Department { get; set; }
        //public string Floor { get; set; }
        public List<BedOccupancyFloorDashboard> BedOccupencyList { get; set; }
        public BedOccupancyCustomModel CurrentBedOccupency { get; set; }
        public DateTime CurrentDate { get; set; }
        public int NumberofCurrentDay { get; set; }
    }
}