using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ISchedulingParametersService
    {
        SchedulingParametersCustomModel FindById(int? id);
        SchedulingParametersCustomModel GetDataByFacilityId(long facilityId);
        long SaveRecord(SchedulingParametersCustomModel vm);
    }
}