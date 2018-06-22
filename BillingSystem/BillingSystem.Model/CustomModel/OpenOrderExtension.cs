using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
     [NotMapped]
    public class OpenOrderExtension
    {
        public Nullable<int> PhysicianID { get; set; }
        public Nullable<int> OrderCount { get; set; }
        public string OrderType { get; set; }
        public string OrderTypeName { get; set; }
        public string OrderCode { get; set; }
        public string OrderDescription { get; set; }
    }
     [NotMapped]
    public class MedicalVitalExtension
    {
        public string Name { get; set; }
        public int VitalCode { get; set; }
        public string VitalName { get; set; }
        public string XAxis { get; set; }
        public decimal Average { get; set; }
        public decimal Maximum { get; set; }
        public decimal Minimum { get; set; }
        public decimal LowerLimit { get; set; }
        public decimal UpperLimit { get; set; }
    }

    public class RevenueForecast
    {
        public int PatientID { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public string EncounterNumber { get; set; }
        public int EncounterID { get; set; }
        public decimal? EXPECTED { get; set; }
        public decimal? Planned { get; set; }
        public decimal? Closed { get; set; }
        public decimal? Cancelled { get; set; }
        public decimal? OnBill { get; set; }
        public decimal? BillConsolidation { get; set; }
        public decimal? BillPrelimnary { get; set; }
        public decimal? BillApproved { get; set; }
        public decimal? BillSentforClaim { get; set; }
        public decimal? PaymentReceived { get; set; }

        public DateTime? TRDate { get; set; }
    }

    public class RegistrationProductivity
    {
        public string UserName { get; set; }
        public int Registered { get; set; }
        public int Target { get; set; }
        public int CreatedBy { get; set; }
        public int FacilityID { get; set; }
        public string CreatedByUser { get; set; }
    }

    public class PatientBillingTrend
    {
        public int Target { get; set; }
        public int Billed { get; set; }
        public string Projection { get; set; }
        public string PatientName { get; set; }
        public int DisplayTypeId { get; set; }
        public int Volume { get; set; }
        public string Month { get; set; }
    }

    public class BillScrubberTrend
    {
        public string Physician { get; set; }
        public int DenialsCoded { get; set; }
    }

    public class ReviewExpectedPaymentReport
    {
        public string Insurer { get; set; }
        public string Plan { get; set; }
        public string EncounterID { get; set; }
        public string PatientName { get; set; }
        public DateTime? InsuranceBillDate { get; set; }
        public string BillNumber { get; set; }
        public decimal ExpectedPayment { get; set; }
        public DateTime? ExpectedPaymentDate { get; set; }
        public decimal ActualPayment { get; set; }
        public DateTime? ActualPaymentDate { get; set; }
        public decimal Variance { get; set; }
        public string Status { get; set; }
        public int PatientID { get; set; }
        public string PayerID { get; set; }
        public long? ClaimID { get; set; }
        public long? FileID { get; set; }
        public DateTime? EncounterEndDate { get; set; }
        public decimal ExpectedPatientPayment { get; set; }
        public decimal ActualPayments { get; set; }
    }


    public class ClaimDenialPercentage
    {
        public string Descrip { get; set; }
        public decimal Total { get; set; }
        public decimal Value { get; set; }
    }
}
