using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class HolidayPlannerDetailsView
    {
     
        public HolidayPlannerDetails CurrentHolidayPlannerDetails { get; set; }
        public List<HolidayPlannerDetailsCustomModel> HolidayPlannerDetailsList { get; set; }

    }
}
