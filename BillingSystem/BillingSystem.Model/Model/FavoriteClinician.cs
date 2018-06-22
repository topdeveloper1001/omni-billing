using BillingSystem.Model.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    public class FavoriteClinician : ICommonEntity<long>
    {
        [Key]
        public long Id { get; set; }
        public long PatientId { get; set; }
        public long ClinicianId { get; set; }
        public bool IsActive { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
