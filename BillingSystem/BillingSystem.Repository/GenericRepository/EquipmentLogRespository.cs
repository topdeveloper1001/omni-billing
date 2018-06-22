using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class EquipmentLogRespository : GenericRepository<EquipmentLog>
    {
        public EquipmentLogRespository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}
