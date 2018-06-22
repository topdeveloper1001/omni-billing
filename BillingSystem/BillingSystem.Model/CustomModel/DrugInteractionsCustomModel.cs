
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DrugInteractionsCustomModel: DrugInteractions
    {
        public string ReactionCategoryStr { get; set; }
        public string WarningStr { get; set; }
        public string OrderTypeName { get; set; }
    }
}
