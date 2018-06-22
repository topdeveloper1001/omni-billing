using System.Data.Entity;
using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class SchedulingParametersRepository : GenericRepository<SchedulingParameters>
    {
        private readonly DbContext _context;

        public SchedulingParametersRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;

        }
    }
}
