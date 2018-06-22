using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class RoomChargesView
    {
        public List<BillDetailCustomModel> RoomChargesList { get; set; }
        public OpenOrder CurrentRoomCharge { get; set; }
    }
}