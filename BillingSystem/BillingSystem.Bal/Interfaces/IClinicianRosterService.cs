using System.Collections.Generic;
using System.Threading.Tasks;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IClinicianRosterService
    {
        Task<IEnumerable<ClinicianRosterCustomModel>> Delete(long facilityId, long corporateId, long userId, long id = 0, bool aStatus = true);
        Task<IEnumerable<ClinicianRosterCustomModel>> GetAll(long facilityId, long corporateId, long userId, long id = 0, bool aStatus = true);
        Task<ClinicianRosterCustomModel> GetSingle(long facilityId, long corporateId, long userId, long id = 0, bool aStatus = true);
        Task<IEnumerable<ClinicianRosterCustomModel>> GetSingleOrList(long cId, long fId, long userId, bool aStatus = true, long id = 0);
        Task<string> Save(ClinicianRosterCustomModel vm, long userId);
    }
}