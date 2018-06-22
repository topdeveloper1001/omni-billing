using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class ProjectTasksRepository : GenericRepository<ProjectTasks>
    {
        public ProjectTasksRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
