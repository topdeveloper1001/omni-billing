using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class BillHeaderCustomModel : BillHeader
    {
        public string FacilityName { get; set; }
        public string CorporateName { get; set; }
        public string EncounterNumber { get; set; }
        public string InsuranceCompany { get; set; }
        public string FileHeaderText { get; set; }
        public string DenialCodeDescritption { get; set; }

        public string PatientName { get; set; }
        public int BStatus { get; set; }
        public string BillHeaderStatus { get; set; }
        public string EncounterStatus { get; set; }
        public decimal? GrossChargesSum { get; set; }
        public bool EncounterType { get; set; }
        public bool LongTermPatient { get; set; }
        public int IsAutoClosed { get; set; }
        public string EncounterPatientType { get; set; }


        public decimal? ActualPayerPayment { get; set; }
        public decimal? VariancePayerPayment { get; set; }
        public decimal? ActualPatientPayment { get; set; }
        public decimal? VariancePatientPayment { get; set; }
        public int? ClaimFileId { get; set; }
        public int? RemittanceFileid { get; set; }
    }
    [NotMapped]
    public class BillHeaderXMLModel
    {
        public string XMLOUT { get; set; }
    }
    [NotMapped]
    public class BillHeaderPreXMLModel
    {
        public int? ID { get; set; }
        public string ID2 { get; set; }
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? RecordCount { get; set; }
        public string DispositionFlag { get; set; }
        public string IDPayer { get; set; }
        public string ProviderID { get; set; }
        public string PaymentReference { get; set; }
        public DateTime? DateSettlement { get; set; }
        public int? FacilityID { get; set; }
        public DateTime? Start { get; set; }
        public int? EncounterID { get; set; }
        public int? BillActivityID { get; set; }
        public string ActivityType { get; set; }
        public string DiagnosisCode { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Net { get; set; }
        public decimal? List { get; set; }
        public string OrderingClinician { get; set; }
        public string Clinician { get; set; }
        public string PriorAuthorizationID { get; set; }
        public int? OrderingClinicianID { get; set; }
        public int? BillHeaderID { get; set; }
        public DateTime? OrderedDate { get; set; }
        public int? AdminstratingClinicianID { get; set; }
        public string AuthorizationCode { get; set; }
        public decimal? Gross { get; set; }
        public decimal? PatientShare { get; set; }
        public decimal? PaymentAmount { get; set; }
        public decimal? MCDiscount { get; set; }
        public string FacilityLicNumber { get; set; }
    }
    [NotMapped]
    public class XMLBillingFileStatus
    {
        public string SuccessFlag { get; set; }
        public int? RecordCounts { get; set; }
        public int? ClaimCounts { get; set; }
    }
}
