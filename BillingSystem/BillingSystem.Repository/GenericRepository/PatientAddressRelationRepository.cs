using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class PatientAddressRelationRepository: GenericRepository<PatientAddressRelation>
    {
        public PatientAddressRelationRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
