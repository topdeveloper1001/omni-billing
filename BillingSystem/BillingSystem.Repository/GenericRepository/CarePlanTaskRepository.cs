using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class CarePlanTaskRepository : GenericRepository<CarePlanTask>
    {
        public CarePlanTaskRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }

        
    }
}
