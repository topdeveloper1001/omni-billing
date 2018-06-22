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
    public class PlaceOfServiceRepository : GenericRepository<PlaceOfService>
    { 
        private readonly DbContext _context;

        public PlaceOfServiceRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }
    }
}
