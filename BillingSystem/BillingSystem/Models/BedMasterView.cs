using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class BedMasterView
    {
        public IEnumerable<BedMasterCustomModel> BedMasterList { get; set; }
        public UBedMaster CurrentBedMaster { get; set; }
    }
}