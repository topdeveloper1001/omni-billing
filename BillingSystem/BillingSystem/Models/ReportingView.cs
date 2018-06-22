using BillingSystem.Model.CustomModel;
using System;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class ReportingView
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Title { get; set; }
        public int ReportingType { get; set; }
        public int UserId { get; set; }
        public string ReportingTypeAction { get; set; }
        public bool ShowAllRecords { get; set; }
        public string ViewType { get; set; }
        public int CorporateId { get; set; }
    }
}