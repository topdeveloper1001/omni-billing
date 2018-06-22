using BillingSystem.Model.EntityDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillingSystem.Bal.Interfaces
{
    public interface IClinicianService
    {
        Task<List<ClinicianDto>> GetCliniciansBySpecialityAsync(int specialityId);
        Task<List<SpecialityDto>> GetClinicianSpecialitiesAsync(long clincianId=0);
        Task<List<LocationDto>> GetLocationsAsync(int clinicianId);
        Task<AppointmentDetailsDto> GetCliniciansAndSpecialitiesAsync(long appointmentTypeId, int cityId = 0, long facilityId = 0);
        Task<string> GetCliniciansAndSpecialitiesAsync2(long appointmentTypeId, int cityId = 0, long facilityId = 0);
        Task<ClinicianDetailDto> GetClinicianDetailsForBookingAnAppoinment(long clinicianId);
        Task<WeeklyScheduleDto> GetAppointmentsWeeklyScheduledDataAsync(long userId=0, long facilityId=0);
    }
}
