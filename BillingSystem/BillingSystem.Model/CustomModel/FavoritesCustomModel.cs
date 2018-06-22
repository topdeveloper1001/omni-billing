using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class FavoritesCustomModel : UserDefinedDescriptions
    {
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string CategoryName { get; set; }
        public string CodeDesc { get; set; }
        public string CodeValue { get; set; }
    }
}
