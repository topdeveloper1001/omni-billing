namespace BillingSystem.Model.CustomModel
{
    public class DischargeSummaryDetailCustomModel 
    {
        public int Id { get; set; }
        public int? DischargeSummaryId { get; set; }
        public string AssociatedId { get; set; }
        public string AssociatedTypeId { get; set; }
        public string OtherValue { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
