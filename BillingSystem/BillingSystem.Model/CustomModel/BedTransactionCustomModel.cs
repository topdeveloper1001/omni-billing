using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class BedTransactionCustomModel : BedTransaction
    {
        //public BedTransaction BedTransaction { get; set; }
        public string FacilityName { get; set; }
        public string BedTypeName { get; set; }
        public string PatientName { get; set; }
        public string BedName { get; set; }
        public string BilledCharges { get; set; }
        public string Balance { get; set; }
        public string BillProgressStatus { get; set; }
        public string BillStatusStr { get; set; }
    }
}
