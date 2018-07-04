using System;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    public class TechnicalSpecifications
    {
        [Key]
        public int Id { get; set; }
        public long ItemID { get; set; }
        [MaxLength(120)]
        public string TechSpec { get; set; }

        public int? CorporateId { get; set; }

        public int? FacilityId { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

    }
}
