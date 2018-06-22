using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class HolidayPlannerView
    {
     
        public HolidayPlanner CurrentHolidayPlanner { get; set; }
        public List<HolidayPlannerCustomModel> HolidayPlannerList { get; set; }

    }
}
