﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DashboardTransactionCounterView
    {
     
        public DashboardTransactionCounter CurrentDashboardTransactionCounter { get; set; }
        public List<DashboardTransactionCounterCustomModel> DashboardTransactionCounterList { get; set; }

    }
}
