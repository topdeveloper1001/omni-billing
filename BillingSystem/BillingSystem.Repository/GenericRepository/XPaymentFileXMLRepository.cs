using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class XPaymentFileXMLRepository : GenericRepository<XPaymentFileXML>
    {
        public XPaymentFileXMLRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
