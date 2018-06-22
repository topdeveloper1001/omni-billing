namespace BillingSystem.Model.Interfaces
{
    interface ICommonEntity<TKey> : IEntity<TKey>, IEntityUpdatable, IEntityCreatable
    {

    }
}
