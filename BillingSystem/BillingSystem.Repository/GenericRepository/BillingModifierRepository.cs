using BillingSystem.Model;
using BillingSystem.Model.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Repository.GenericRepository
{
    public class BillingModifierRepository : GenericRepository<BillingModifier>
    {
        private readonly DbContext _context;

        public BillingModifierRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }
    }
}
