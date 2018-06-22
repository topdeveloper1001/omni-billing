
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class TPFileHeaderCustomModel : TPFileHeader
    {
        public bool ShowExecute { get; set; }
    }
}
