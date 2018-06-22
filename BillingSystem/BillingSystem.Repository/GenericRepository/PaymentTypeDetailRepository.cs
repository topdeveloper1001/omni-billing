using BillingSystem.Model;
using BillingSystem.Model.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class PaymentTypeDetailRepository : GenericRepository<PaymentTypeDetail>
    {
        public PaymentTypeDetailRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
