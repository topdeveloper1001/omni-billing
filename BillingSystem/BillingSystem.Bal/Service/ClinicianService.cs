using BillingSystem.Bal.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingSystem.Model.EntityDto;
using BillingSystem.Repository.UOW;
using System.Data.Entity;

namespace BillingSystem.Bal.Service
{
    public class ClinicianService : IClinicianService, IService<SpecialityDto>
    {
        private readonly UnitOfWork _uow;

        public ClinicianService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<WeeklyScheduleDto> GetAppointmentsWeeklyScheduledDataAsync(long userId = 0, long facilityId = 0)
        {
            using (var rep = _uow.SchedulingRepository)
                return await rep.GetAppointmentsWeeklyScheduledDataAsync(userId, facilityId);
        }

        public async Task<ClinicianDetailDto> GetClinicianDetailsForBookingAnAppoinment(long clinicianId)
        {
            using (var rep = _uow.SchedulingRepository)
                return await rep.GetClinicianDetailsForBookingAnAppoinment(clinicianId);
        }

        public async Task<AppointmentDetailsDto> GetCliniciansAndSpecialitiesAsync(long appointmentTypeId, int cityId = 0, long facilityId = 0)
        {
            using (var rep = _uow.SchedulingRepository)
                return await rep.GetCliniciansAndSpecialitiesAsync(appointmentTypeId, cityId, facilityId);
        }

        public async Task<string> GetCliniciansAndSpecialitiesAsync2(long appointmentTypeId, int cityId = 0, long facilityId = 0)
        {
            using (var rep = _uow.SchedulingRepository)
                return await rep.GetCliniciansAndSpecialitiesAsync2(appointmentTypeId, cityId, facilityId);
        }

        public async Task<List<ClinicianDto>> GetCliniciansBySpecialityAsync(int specialityId)
        {
            using (var rep = _uow.AppointmentTypesRepository)
            {
                return await rep.GetCliniciansBySpecialyAsync(specialityId);
            }
        }

        public async Task<List<SpecialityDto>> GetClinicianSpecialitiesAsync(long clinicianId)
        {
            var list = await GetListByCategory("1121");
            if (clinicianId != 0)
            {
                using (var rep1 = _uow.PhysicianRepository)
                {
                    var specialtyId = await rep1.Where(a => a.Id == clinicianId).Select(b => b.FacultySpeciality).FirstOrDefaultAsync();
                    list = list.Where(a => a.Id == specialtyId).ToList();
                }
            }
            return list;
        }

        public async Task<List<SpecialityDto>> GetListByCategory(string category, List<string> exclusions = null)
        {
            using (var rep = _uow.GlobalCodeRepository)
            {
                var result = rep.Where(g => g.GlobalCodeCategoryValue.Equals(category)
                && g.IsActive && g.IsDeleted != true).Select(m =>
                new SpecialityDto
                {
                    Id = m.GlobalCodeValue,
                    Name = m.GlobalCodeName
                }).ToList();

                return await Task.FromResult(result);
            }
        }

        public async Task<List<LocationDto>> GetLocationsAsync(int clinicianId)
        {
            using (var rep = _uow.AppointmentTypesRepository)
                return await rep.GetLocationsByClinicianAsync(clinicianId);
        }
    }
}
