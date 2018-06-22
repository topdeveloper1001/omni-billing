using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class InsuranceCompanyRepository : GenericRepository<InsuranceCompany>
    {
        public InsuranceCompanyRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
