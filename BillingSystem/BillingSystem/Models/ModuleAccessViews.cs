using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ModuleAccessView
    {
     
        public ModuleAccess CurrentModuleAccess { get; set; }
        public List<ModuleAccess> ModuleAccessList { get; set; }
        public IEnumerable<TabsCustomModel> TabList { get; set; }
    }
}
