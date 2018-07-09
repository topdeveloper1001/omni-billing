using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    public class FacilityContact : BaseEntity<long>
    {
        [MaxLength(100)]
        public string ContactName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        public int FacilityId { get; set; }

        public bool IsMain { get; set; }
    }
}
