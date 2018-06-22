using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ManagedCareView
    {

        public ManagedCare CurrentManagedCare { get; set; }
        public List<ManagedCareCustomModel> ManagedCareList { get; set; }

    }
}
