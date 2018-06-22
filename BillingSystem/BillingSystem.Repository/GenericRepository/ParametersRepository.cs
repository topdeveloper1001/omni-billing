using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class ParametersRepository : GenericRepository<Parameters>
    {
        public ParametersRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
