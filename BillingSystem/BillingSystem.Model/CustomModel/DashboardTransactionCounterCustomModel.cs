﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DashboardTransactionCounterCustomModel
    {
        public int CounterId { get; set; }
        public int? StatisticDescription { get; set; }
        public DateTime? ActivityDay { get; set; }
        public decimal? ActivityTotal { get; set; }
        public decimal? DepartmentNumber { get; set; }
        public int CorporateId { get; set; }
        public int FacilityId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public string DashboardStatDescstring { get; set; }
    }

    [NotMapped]
    public class ChargesReportCustomModel
    {
        public DateTime? ActivityDate { get; set; }
        public string ActivityType { get; set; }
        public decimal? ActivityTotal { get; set; }
        public decimal? OtherTotal { get; set; }
        public int? CorporateId { get; set; }
        public int? FacilityId { get; set; }
        public decimal? Department { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? EncounterId { get; set; }
        public int? PatientId { get; set; }
        public string PatientName { get; set; }
        public string EncounterNumber { get; set; }
        public string DepartmentName { get; set; }
        public string Payor { get; set; }
        public string PhysicianName { get; set; }
        public string CTPCode { get; set; }
        public string CPTDescription { get; set; }
        public string PatientType { get; set; }
        public int Performed { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int NotApplicable { get; set; }

    }
}
