﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DischargeSummaryDetailView
    {
     
        public DischargeSummaryDetail CurrentDischargeSummaryDetail { get; set; }
        public List<DischargeSummaryDetailCustomModel> DischargeSummaryDetailList { get; set; }

    }
}
