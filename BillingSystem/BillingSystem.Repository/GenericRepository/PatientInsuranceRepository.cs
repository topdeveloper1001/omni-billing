using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
   public class PatientInsuranceRepository: GenericRepository<PatientInsurance>
    {
       public PatientInsuranceRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;

        }
    }
}
