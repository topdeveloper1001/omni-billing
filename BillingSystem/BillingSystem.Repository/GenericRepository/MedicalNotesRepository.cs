using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class MedicalNotesRepository : GenericRepository<MedicalNotes>
    {
        public MedicalNotesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
