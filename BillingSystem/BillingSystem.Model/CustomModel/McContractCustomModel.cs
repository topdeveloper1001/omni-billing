using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class McContractCustomModel : MCContract
    {
        public string OrderCodeDescription { get; set; }
        public string OrderTypeText { get; set; }
        public string OrderCategoryId { get; set; }
        public string OrderSubCategoryId { get; set; }
        public string PatientTypeText { get; set; }
    }
    [NotMapped]
    public class McContractOverViewCustomModel
    {
        public string MCOverview { get; set; }
    }
}
