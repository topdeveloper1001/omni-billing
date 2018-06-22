using BillingSystem.Model;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class RoleView
    {
        public List<Role> RolesList { get; set; }
        public Role CurrentRole { get; set; }
        public ScreenView screenView { get; set; }
    }
}