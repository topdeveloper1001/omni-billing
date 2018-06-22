using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class StateRepository :GenericRepository<State>
    {
        public StateRepository(BillingEntities context)
            : base(context)
        { }
    }
}
