using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class OrderActivityView
    {
        public OrderActivity CurrentOrderActivity { get; set; }
        public OrderActivityCustomModel CurrentLabOrderActivity { get; set; }
        public List<OrderActivityCustomModel> OrderActivityList { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }
        public List<OpenOrderCustomModel> ClosedOrdersList { get; set; }
        public List<OrderActivityCustomModel> ClosedOrderActivityList { get; set; }
        public List<OrderActivityCustomModel> labOrderActivityList { get; set; }
        public List<OrderActivityCustomModel> ClosedLabOrderActivityList { get; set; }
        public OpenOrder EncounterOrder { get; set; }
        public bool IsLabTest { get; set; }

        public MedicalVitalCustomModel CurrentMedicalVital { get; set; }
        public List<MedicalVitalCustomModel> MedicalVitalList { get; set; }
    }
}
