using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    using BillingSystem.Model.Model;

    public class FutureOrderActivityView
    {
     
        public FutureOrderActivity CurrentFutureOrderActivity { get; set; }
        public List<FutureOrderActivityCustomModel> FutureOrderActivityList { get; set; }

    }
}
