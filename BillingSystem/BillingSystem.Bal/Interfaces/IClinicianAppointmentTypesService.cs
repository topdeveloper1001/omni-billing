using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IClinicianAppointmentTypesService
    {
        ClinicianAppTypesCustomModel GetDataOnViewLoad(long facilityId, long userId);
        IEnumerable<ClinicianAppTypesCustomModel> GetList(long facilityId, long userId, long clinicianId = 0);
        long Save(ClinicianAppTypesCustomModel vm);
    }
}