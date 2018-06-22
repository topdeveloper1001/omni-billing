using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class OperatingRoomView
    {
        public OperatingRoom OperatingRoom { get; set; }
        public OperatingRoom AnesthesiaTime { get; set; }
        public List<OperatingRoomCustomModel> OperatingRoomsList { get; set; }
        public List<OperatingRoomCustomModel> AnesthesiaTimesList { get; set; }
    }
}
