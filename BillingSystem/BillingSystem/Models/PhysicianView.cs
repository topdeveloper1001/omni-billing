using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class PhysicianView
    {
        public List<PhysicianViewModel> PhysicianList { get; set; }
        public PhysicianViewModel CurrentPhysician { get; set; }
    }
}
