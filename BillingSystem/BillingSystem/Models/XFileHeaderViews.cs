using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class XFileHeaderView
    {
     
        public XFileHeader CurrentXFileHeader { get; set; }
        public List<XFileHeaderCustomModel> XFileHeaderList { get; set; }

    }
}
