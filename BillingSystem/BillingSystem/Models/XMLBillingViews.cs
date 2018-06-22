using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    using BillingSystem.Model.CustomModel;

    public class XMLBillingView
    {
        public XFileHeader CurrentXFileHeader { get; set; }
        public List<XFileHeaderCustomModel> XFileHeaderList { get; set; }

    }
}
