using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class SysDiagramsRepository : GenericRepository<sysdiagrams>
    {
        public SysDiagramsRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
