using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class DrugAllergyLogRepository : GenericRepository<DrugAllergyLog>
    {
        public DrugAllergyLogRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
