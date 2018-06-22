namespace BillingSystem.Model.EntityDto
{
    public class FavoriteClinicianDto
    {
        public long Id { get; set; }
        public long ClinicianId { get; set; }
        public long PatientId { get; set; }
        public string Name { get; set; }
        public long? SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
    }
}
