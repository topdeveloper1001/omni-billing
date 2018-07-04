using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DrugInstructionAndDosingCustomModel
    {
        public int Id { get; set; }
        public string GreenrainCode { get; set; }
        public string ATCCode { get; set; }
        public string PackageName { get; set; }
        public string GenericName { get; set; }
        public string AdminInstructions { get; set; }
        public string RecommendedDosing { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string AdministrationInstructionsStr { get; set; }
        public string RecommendedDosingStr { get; set; }
    }
}
