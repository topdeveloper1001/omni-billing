using BillingSystem.Model.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    public class BaseEntity<T> : IEntity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
