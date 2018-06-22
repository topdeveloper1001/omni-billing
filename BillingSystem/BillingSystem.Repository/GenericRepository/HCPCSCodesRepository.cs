using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
 
   public class HCPCSCodesRepository : GenericRepository<HCPCSCodes>
    {
       public HCPCSCodesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
       }
    }
}
