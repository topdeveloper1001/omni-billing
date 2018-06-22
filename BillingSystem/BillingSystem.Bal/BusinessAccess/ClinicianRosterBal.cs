using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Model.CustomModel;
using System.Threading.Tasks;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ClinicianRosterBal : BaseBal
    {
        private ClinicianRosterMapper ClinicianRosterMapper { get; set; }

        public ClinicianRosterBal()
        {
            ClinicianRosterMapper = new ClinicianRosterMapper();
        }

        public async Task<IEnumerable<ClinicianRosterCustomModel>> GetAll(long facilityId, long corporateId, long userId, long id = 0, bool aStatus = true)
        {
            using (var rep = UnitOfWork.ClinicianRosterRepository)
                return await rep.GetSingleOrList(corporateId, facilityId, userId, aStatus, id);
        }

        public async Task<ClinicianRosterCustomModel> GetSingle(long facilityId, long corporateId, long userId, long id = 0, bool aStatus = true)
        {
            using (var rep = UnitOfWork.ClinicianRosterRepository)
            {
                var list = await rep.GetSingleOrList(corporateId, facilityId, userId, aStatus, id);
                var vm = list.FirstOrDefault();
                return vm;
            }
        }

        public async Task<string> Save(ClinicianRosterCustomModel vm, long userId)
        {
            using (var rep = UnitOfWork.ClinicianRosterRepository)
            {
                var model = ClinicianRosterMapper.MapViewModelToModel(vm);
                var result = await rep.SaveRecordV2(model, userId);
                return result;
            }
        }

        public async Task<IEnumerable<ClinicianRosterCustomModel>> Delete(long facilityId, long corporateId, long userId, long id = 0, bool aStatus = true)
        {
            using (var rep = UnitOfWork.ClinicianRosterRepository)
                return await rep.DeleteRecord(corporateId, facilityId, userId, id);
        }
    }
}
