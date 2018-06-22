using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class DenialView
    {
        public List<DenialCodeCustomModel> DenialList { get; set; }
        public Denial CurrentDenial { get; set; }
    }
}