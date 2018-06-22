using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class DrugInstructionAndDosingRepository : GenericRepository<DrugInstructionAndDosing>
    {
        public DrugInstructionAndDosingRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
