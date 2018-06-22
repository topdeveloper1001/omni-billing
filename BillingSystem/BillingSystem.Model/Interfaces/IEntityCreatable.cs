using System;

namespace BillingSystem.Model.Interfaces
{
    public interface IEntityCreatable
    {
        long CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
    }
}
