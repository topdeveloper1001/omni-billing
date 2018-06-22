using BillingSystem.Model;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class RoleTabsView
    {
        public Role CurrentRole { get; set; }
        public IEnumerable<TabsCustomModel> TabList { get; set; }
        public List<Role> RoleList { get; set; }
    }
}