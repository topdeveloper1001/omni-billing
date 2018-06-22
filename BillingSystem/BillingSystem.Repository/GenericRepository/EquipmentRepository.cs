using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class EquipmentRepository : GenericRepository<EquipmentMaster>
    {
        public EquipmentRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
