namespace BillingSystem.Model.EntityDto
{
    public class LocationDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public long UserId { get; set; }
    }
}
