using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class ProjectTargetsRepository : GenericRepository<ProjectTargets>
    {
        public ProjectTargetsRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
