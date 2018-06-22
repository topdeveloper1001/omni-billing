using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
namespace BillingSystem.Models
{
    public class EquipmentView
    {
        public List<EquipmentCustomModel> EquipmentList { get; set; }
        public EquipmentMaster CurrentEquipment { get; set; }
    }
}