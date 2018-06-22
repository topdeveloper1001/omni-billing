using System;

namespace BillingSystem.Model.Interfaces
{
    public interface IEntityUpdatable
    {
        //FK
        long? ModifiedBy { get; set; }   
        DateTime? ModifiedDate { get; set; }
    }
}
