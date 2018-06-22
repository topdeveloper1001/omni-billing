namespace BillingSystem.Model.Interfaces
{
    public interface ISoftDelete
    {
        bool? IsDeleted { get; set; }
    }
}
