using BillingSystem.Model;


namespace BillingSystem.Repository.GenericRepository
{
    public class ScreenRepository : GenericRepository<Screen>
    {
        public ScreenRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }

    }
}
