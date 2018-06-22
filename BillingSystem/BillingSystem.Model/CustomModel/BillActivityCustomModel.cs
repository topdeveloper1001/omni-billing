using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class BillActivityCustomModel : BillActivity
    {
        public string CorporateName { get; set; }
        public string FacilityName { get; set; }
    }


    public class BillDetailCustomModel
    {
        [Key]
        public int Id { get; set; }
        public string ActivityTypeName { get; set; }
        public string ActivityType { get; set; }
        public string ActivityCode { get; set; }
        public DateTime? OrderedOn { get; set; }
        public DateTime? ExecutedOn { get; set; }
        public decimal? QuantityOrdered { get; set; }
        public decimal? GrossCharges { get; set; }
        public string BillNumber { get; set; }
        public decimal? MCDiscount { get; set; }
        public decimal? PayerShareNet { get; set; }
        public decimal? PatientShare { get; set; }
        public decimal? ActivityCost { get; set; }

        public string ActivityCodeDescription { get; set; }

        public int CorporateID { get; set; }
        public int FacilityID { get; set; }
        public string ActivityName { get; set; }
        public int PatientID { get; set; }
        public int EncounterID { get; set; }
        public int BillActivityID { get; set; }
        public int BillHeaderID { get; set; }
        public decimal? GrossChargesSum { get; set; }

    }

    public class BillPdfFormatCustomModel
    {
        public string FacilityName { get; set; }
        public string FacilityAddress { get; set; }
        public string FacilityPhoneNumber { get; set; }
        public string FacilityFaxNumber { get; set; }
        public string BillNumber { get; set; }
        public DateTime? BillDate { get; set; }
        public string PatientName { get; set; }
        public DateTime? AddmittedOn { get; set; }
        public string PatientSex { get; set; }
        public DateTime? DischargedOn { get; set; }
        public string PatientAddress { get; set; }
        public decimal? PatientShare { get; set; }
        public decimal? PayorShare { get; set; }
        public decimal? Gross { get; set; }
        public decimal? PaidPatientShare { get; set; }
        public decimal? PaidPayorShare { get; set; }
        public decimal? PaidGross { get; set; }
        public decimal? BalancePatientShare { get; set; }
        public decimal? BalancePayorShare { get; set; }
        public decimal? BalanceGross { get; set; }
    }
}
