using System.Data.Entity;
using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class FavoriteClinicianRepository : GenericRepository<FavoriteClinician>
    {
        private readonly DbContext _context;

        public FavoriteClinicianRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

    }
}
