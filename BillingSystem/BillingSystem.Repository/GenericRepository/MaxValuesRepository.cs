using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class MaxValuesRepository : GenericRepository<MaxValues>
    {
        public MaxValuesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
