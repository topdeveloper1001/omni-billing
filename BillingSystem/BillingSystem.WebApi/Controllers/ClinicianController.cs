using BillingSystem.Bal.Interfaces;
using BillingSystem.Model.EntityDto;
using BillingSystem.WebApi.Common;
using BillingSystem.WebApi.Filters;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace BillingSystem.WebApi.Controllers
{
    [JwtAuthenticate]
    public class ClinicianController : ApiController
    {
        private readonly IClinicianService _service;

        public ClinicianController(IClinicianService service)
        {
            _service = service;
        }

        [HttpGet]
        [CustomEx]
        public async Task<IHttpActionResult> GetCliniciansBySpecialityAsync([FromUri]int specialtyId)
        {
            var r = new Response<List<ClinicianDto>>();
            var result = await _service.GetCliniciansBySpecialityAsync(specialtyId);
            r.Data = result;
            return new R<List<ClinicianDto>>(HttpStatusCode.OK, r, Request);
        }

        [HttpGet]
        [CustomEx]
        public async Task<IHttpActionResult> GetSpecialitiesAsync(long clinicianId = 0)
        {
            var response = new Response<List<SpecialityDto>>();
            var result = await _service.GetClinicianSpecialitiesAsync();
            response.Data = result;
            return new R<List<SpecialityDto>>(HttpStatusCode.OK, response, Request);
        }


        [HttpGet]
        [CustomEx]
        public async Task<IHttpActionResult> GetCliniciansAndSpecialitiesAsync([FromUri]long appointmentTypeId = 0, int cityId = 0, long facilityId = 0)
        {
            var response = new Response<AppointmentDetailsDto>();
            var result = await _service.GetCliniciansAndSpecialitiesAsync(appointmentTypeId, cityId, facilityId);
            response.Data = result;
            return new R<AppointmentDetailsDto>(HttpStatusCode.OK, response, Request);
        }


        //[HttpGet]
        //[CustomEx]
        //public async Task<IHttpActionResult> Test([FromUri]long appointmentTypeId, int cityId = 0, long facilityId = 0)
        //{
        //    var response = new Response<string>();
        //    var result = await _service.GetCliniciansAndSpecialitiesAsync2(appointmentTypeId, cityId, facilityId);
        //    response.Data = result;
        //    return new R<string>(HttpStatusCode.OK, response, Request);
        //}

        [HttpGet]
        [CustomEx]
        public async Task<IHttpActionResult> GetClinicianDetailsForBookingAnAppoinment([FromUri]long clinicianId)
        {
            var response = new Response<ClinicianDetailDto>();
            var result = await _service.GetClinicianDetailsForBookingAnAppoinment(clinicianId);
            response.Data = result;
            return new R<ClinicianDetailDto>(HttpStatusCode.OK, response, Request);
        }


        [CustomEx]
        public async Task<IHttpActionResult> GetAppointmentsWeeklyBookedData([FromUri]long userId = 0, long facilityId = 0)
        {
            var response = new Response<WeeklyScheduleDto>();
            var result = await _service.GetAppointmentsWeeklyScheduledDataAsync(userId, facilityId);
            response.Data = result;
            return new R<WeeklyScheduleDto>(HttpStatusCode.OK, response, Request);
        }
    }
}
