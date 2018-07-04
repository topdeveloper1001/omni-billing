using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DashboardTargetsCustomModel
    {
        public int TargetId { get; set; }
        public string TargetDescription { get; set; }
        public int? RoleId { get; set; }
        public int? UnitOfMeasure { get; set; }
        public int? TimingIncrement { get; set; }
        public decimal? Target { get; set; }
        public int CorporateId { get; set; }
        public int FacilityId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public string RoleName { get; set; }
        public string UOMstr { get; set; }
        public string TimmingIncrementStr { get; set; }
    }
}
