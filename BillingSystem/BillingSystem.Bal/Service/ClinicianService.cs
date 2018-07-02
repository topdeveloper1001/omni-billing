using BillingSystem.Bal.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingSystem.Model.EntityDto;
using System.Data.Entity;
using BillingSystem.Model;
using BillingSystem.Common.Common;
using System.Data.SqlClient;

namespace BillingSystem.Bal.Service
{
    public class ClinicianService : IClinicianService, IService<SpecialityDto>
    {
        private readonly IRepository<Scheduling> _scRepository;
        private readonly IRepository<Physician> _phRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly BillingEntities _context;

        public ClinicianService(IRepository<Scheduling> scRepository, IRepository<Physician> phRepository, IRepository<GlobalCodes> gRepository, BillingEntities context)
        {
            _scRepository = scRepository;
            _phRepository = phRepository;
            _gRepository = gRepository;
            _context = context;
        }

        public async Task<WeeklyScheduleDto> GetAppointmentsWeeklyScheduledDataAsync(long userId = 0, long facilityId = 0)
        {
            var sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("pUserId", userId);
            sqlParams[1] = new SqlParameter("pFacilityId", facilityId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetAppointmentsWeeklyScheduledData.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.GetResultWithJsonAsync<WeeklyScheduleDto>(JsonResultsArray.AppointmentWeeklyScheduledData.ToString())).FirstOrDefault();
                return result;
            }
        }

        public async Task<ClinicianDetailDto> GetClinicianDetailsForBookingAnAppoinment(long clinicianId)
        {
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("pClinicianId", clinicianId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetClinicianDetailsForRescheduling.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.GetResultWithJsonAsync<ClinicianDetailDto>(JsonResultsArray.ClinicanDetail.ToString())).FirstOrDefault();
                return result;
            }
        }

        public async Task<AppointmentDetailsDto> GetCliniciansAndSpecialitiesAsync(long appointmentTypeId, int cityId = 0, long facilityId = 0)
        {
            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("pAppointmentTypeId", appointmentTypeId);
            sqlParams[1] = new SqlParameter("pCityId", cityId);
            sqlParams[2] = new SqlParameter("pFacilityId", facilityId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetCliniciansAndSpecialtiesByAppType.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.GetResultWithJsonAsync<AppointmentDetailsDto>(JsonResultsArray.CliniciansData.ToString())).FirstOrDefault();
                return result;
            }
        }

        public async Task<string> GetCliniciansAndSpecialitiesAsync2(long appointmentTypeId, int cityId = 0, long facilityId = 0)
        {
            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("pAppointmentTypeId", appointmentTypeId);
            sqlParams[1] = new SqlParameter("pCityId", cityId);
            sqlParams[2] = new SqlParameter("pfacilityId", facilityId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetCliniciansAndSpecialtiesByAppType.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = await ms.GetJsonStringResult();
                return result;
            }
        }

        public async Task<List<ClinicianDto>> GetCliniciansBySpecialityAsync(int specialityId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("sId", specialityId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetCliniciansBySpecialty.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var result = await ms.GetResultWithJsonAsync<ClinicianDto>(JsonResultsArray.Clinicians.ToString());
                return result;
            }
        }

        public async Task<List<SpecialityDto>> GetClinicianSpecialitiesAsync(long clinicianId)
        {
            var list = await GetListByCategory("1121");
            if (clinicianId != 0)
            {
                var specialtyId = await _phRepository.Where(a => a.Id == clinicianId).Select(b => b.FacultySpeciality).FirstOrDefaultAsync();
                list = list.Where(a => a.Id == specialtyId).ToList();
            }
            return list;
        }

        public async Task<List<SpecialityDto>> GetListByCategory(string category, List<string> exclusions = null)
        {
            var result = _gRepository.Where(g => g.GlobalCodeCategoryValue.Equals(category)
            && g.IsActive && g.IsDeleted != true).Select(m =>
            new SpecialityDto
            {
                Id = m.GlobalCodeValue,
                Name = m.GlobalCodeName
            }).ToList();

            return await Task.FromResult(result);

        }

        public async Task<List<LocationDto>> GetLocationsAsync(int clinicianId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("clinicianId", clinicianId);

            //using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetLocationsByClinician.ToString(), isCompiled: false, parameters: sqlParameters))
            //{
            //    var reader = await multiResultSet.GetReaderAsync();
            //    var result = GenericHelper.GetJsonResponse<LocationDto>(reader, "Locations");
            //    return result;
            //}
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetLocationsByClinician.ToString(), isCompiled: false))
            {
                var result = await ms.GetResultWithJsonAsync<LocationDto>(JsonResultsArray.Locations.ToString());
                return result;
            }
        }
    }
}
