using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    public class McContractCustomModel
    {
        public int MCContractID { get; set; }
        public string MCCode { get; set; }
        public string MCLevel { get; set; }
        public string MCEncounterType { get; set; }
        public string MCOrderType { get; set; }
        public string MCOrderCode { get; set; }
        public int? MCPatientPercent { get; set; }
        public decimal? MCInPatientBaseRate { get; set; }
        public decimal? MCAnnualOutOfPocket { get; set; }
        public int? MCPatientFixed { get; set; }
        public int? MCPatientCapping { get; set; }
        public int? MCMultiplier { get; set; }
        public int? MCWaitingDays { get; set; }
        public int? MCExpireAfterDays { get; set; }
        public int? MCApplyWeightAge { get; set; }
        public bool? MCPerDiemsApplicable { get; set; }
        public bool? MCCarveoutsApplicable { get; set; }
        public string MCDRGTableNumber { get; set; }
        public string MCCPTTableNumber { get; set; }
        public string MCCodeRangeFrom { get; set; }
        public string MCCodeRangeTill { get; set; }
        public int BCCreatedBy { get; set; }
        public DateTime BCCreatedDate { get; set; }
        public int? BCModifiedBy { get; set; }
        public DateTime? BCModifiedDate { get; set; }
        public bool? BCIsActive { get; set; }
        public string ModelName { get; set; }
        public int? InitialSubmitDay { get; set; }
        public int? ResubmitDays1 { get; set; }
        public int? ResubmitDays2 { get; set; }
        public int? PenaltyLateSubmission { get; set; }
        public int? BillScrubberRule { get; set; }
        public int? ExpectedPaymentDays { get; set; }
        public decimal? MCMultiplierOutpatient { get; set; }
        public decimal? MCMultiplierEmergencyRoom { get; set; }
        public decimal? MCMultiplierOther { get; set; }
        public decimal? MCInpatientDeduct { get; set; }
        public decimal? MCOutpatientDeduct { get; set; }
        public bool? MCEMCertified { get; set; }
        public decimal? MCPenaltyRateResubmission { get; set; }
        public decimal? MCRuleSetNumber { get; set; }
        public decimal? MCAddon { get; set; }
        public decimal? MCExpectedFixedrate { get; set; }
        public decimal? MCExpectedPercentage { get; set; }
        public bool? MCInPatientType { get; set; }
        public bool? MCOPPatientType { get; set; }
        public bool? MCERPatientType { get; set; }
        public string MCGeneralLedgerAccount { get; set; }
        public string ARGeneralLedgerAccount { get; set; }

        public int CorporateId { get; set; }
        public int FacilityId { get; set; }

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
