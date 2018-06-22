using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class ProjectsRepository : GenericRepository<Projects>
    {
        public ProjectsRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
