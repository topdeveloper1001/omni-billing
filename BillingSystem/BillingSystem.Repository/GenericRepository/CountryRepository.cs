using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class CountryRepository :GenericRepository<Country>
    {
        public CountryRepository(BillingEntities context)
            : base(context)
        { }
    }
}
