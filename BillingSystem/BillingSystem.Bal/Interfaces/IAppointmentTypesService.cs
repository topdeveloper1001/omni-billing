using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IAppointmentTypesService
    {
        bool CheckDuplicateAppointmentType(int id, string name, int? corporateId, int? facilityId);
        bool CheckDuplicateCategoryNumber(int id, string categoryNumber, int? corporateId, int? facilityId);
        int DeleteAppointmentTyepsData(AppointmentTypes model);
        List<AppointmentTypes> GetAppointmentTypesByFacilityId(int facilityId, List<int> ids);
        AppointmentTypes GetAppointmentTypesById(int id);
        List<AppointmentTypesCustomModel> GetAppointmentTypesData(int corporateId, int facilityId, bool showInActive);
        List<AppointmentTypes> GetAppointmneTypesByFacilityId(int facilityId);
        int GetMaxCategoryNumber(int facilityId, int corporateId);
        bool IsEquipmentRequiredWithProcedure(int id);
        int SaveAppointmentTypes(AppointmentTypes model);
    }
}