using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class FacilityDepartmentRepository : GenericRepository<FacilityDepartment>
    {
        public FacilityDepartmentRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
