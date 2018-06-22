using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class OperatingRoomCustomModel : OperatingRoom
    {
        public string OperatingTypeText { get; set; }
        public string EncounterNumber { get; set; }
        public string Patient { get; set; }
        public string CodeDescription { get; set; }
        public string StatusDescription { get; set; }
    }
}
