using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class OpenOrderActivityScheduleView
    {
        public List<OpenOrderActivityScheduleCustomModel> OpenOrderActivityScheduleList { get; set; }
        public OpenOrderActivitySchedule CurrentOpenOrderActivitySchedule { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }
        public List<OpenOrderCustomModel> ClosedOrdersList { get; set; }
    }
}