using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class CarePlanTaskView
    {
     
        public CarePlanTask CurrentCarePlanTask { get; set; }
        public List<CarePlanTaskCustomModel> CarePlanTaskList { get; set; }

    }
}
