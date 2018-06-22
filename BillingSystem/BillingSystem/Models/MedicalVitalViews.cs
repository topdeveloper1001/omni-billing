using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class MedicalVitalView
    {

        public MedicalVitalCustomModel CurrentMedicalVital { get; set; }
        public List<MedicalVitalCustomModel> MedicalVitalList { get; set; }
        public List<OpenOrderCustomModel> ClosedOrdersList { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }
        public List<OrderActivityCustomModel> ClosedOrdersActivitesList { get; set; }
        public List<OrderActivityCustomModel> OpenOrdersActivitesList { get; set; }
        public List<OrderActivityCustomModel> LabOpenOrdersActivitesList { get; set; }
        public List<OrderActivityCustomModel> ClosedLabOrdersActivitesList { get; set; }
        public OpenOrder EncounterOrder { get; set; }
        public bool IsLabTest { get; set; }
    }
}
