using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class TPFileXMLRepository : GenericRepository<TPFileXML>
    {
        public TPFileXMLRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
