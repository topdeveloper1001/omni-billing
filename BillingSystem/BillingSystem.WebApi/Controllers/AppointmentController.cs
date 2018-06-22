using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Requests;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.EntityDto;
using BillingSystem.WebApi.Common;
using BillingSystem.WebApi.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace BillingSystem.WebApi.Controllers
{
    [JwtAuthenticate]
    public class AppointmentController : BaseController
    {
        private readonly IAppointmentService _service;
        private readonly ICommonService _cservice;


        public AppointmentController(IAppointmentService service, ICommonService cService)
        {
            _service = service;
            _cservice = cService;
        }

        [CustomEx]
        [HttpGet]
        public async Task<IHttpActionResult> GetAppoinmentTypesAsync()
        {
            var r = new Response<List<AppointmentTypeDto>>();
            var result = await _service.GetAppointmentTypesAsync();
            r.Data = result;
            return new R<List<AppointmentTypeDto>>(HttpStatusCode.OK, r, Request);
        }

        //[CustomEx]
        //[HttpGet]
        //public async Task<IHttpActionResult> GetAvailableTimeslotsAsync([FromUri]DateTime appointmentDate, long appointmentTypeId
        //    , bool isFirst, long specialty = 0, long clinicianId = 0, long facilityId = 0, string timeFrom = "", string timeTo = "", int maxCount = 5)
        //{
        //    var r = new Response<IEnumerable<TimeSlotsDto>>();
        //    var result = await _service.GetAvailableTimeSlotsAsync(clinicianId, appointmentDate, appointmentTypeId, isFirst, specialty, timeFrom, timeTo, maxCount, facilityId);
        //    r.Data = result;
        //    return new R<IEnumerable<TimeSlotsDto>>(HttpStatusCode.OK, r, Request);
        //}

        [HttpPost]
        [CustomEx]
        public async Task<IHttpActionResult> BookAnAppointmentAsync([FromBody]AppointmentDto m)
        {
            var r = new Response<CommonDto>();
            var result = await _service.BookAnAppointmentAsync(m);

            if (result.Status <= 0)
            {
                if (result.Status < 0)
                {
                    r.Code = 0;
                    r.Message = result.Message;
                    r.Data = new CommonDto { NewId = 0 };
                    return new R<CommonDto>(HttpStatusCode.Forbidden, r, Request);
                }
                else
                {
                    r.Code = 0;
                    r.Message = "Error in Web API";
                    r.Data = new CommonDto { NewId = 0 };
                    return new R<CommonDto>(HttpStatusCode.InternalServerError, r, Request);
                }
            }
            else
                r.Data = new CommonDto { NewId = result.Status };

            return new R<CommonDto>(HttpStatusCode.OK, r, Request);
        }

        [HttpGet]
        [CustomEx]
        public async Task<IHttpActionResult> GetBookedAppointmentsAsync([FromUri]UpcomingAppointmentsRequest m)
        {
            var result = await _service.GetBookedAppointmentsAsync(m);

            if (result == null || !result.Any())
            {
                var r = new Response<IEnumerable<BookedAppointmentDto>> { Data = result, Code = 0, Message = "No Records Found" };
                return new R<IEnumerable<BookedAppointmentDto>>(HttpStatusCode.NotFound, r, Request);
            }

            return new R<IEnumerable<BookedAppointmentDto>>(HttpStatusCode.OK, new Response<IEnumerable<BookedAppointmentDto>> { Data = result }, Request);
        }

        [HttpPost]
        [CustomEx]
        public IHttpActionResult CancelAppointment([FromBody]AppointmentDto m)
        {
            var result = _service.CancelAppointment(m.Id, Convert.ToInt32(m.PatientId), m.AppointmentDetails);
            return new R<bool>(HttpStatusCode.OK, new Response<bool> { Data = result }, Request);
        }

        [HttpGet]
        [CustomEx]
        public async Task<IHttpActionResult> GetCliniciansAndTimeSlotsAsync([FromUri]AvailableTimeSlotsRequest m)
        {
            var result = await _service.GetCliniciansAndTheirTimeSlotsAsync(m);

            if (result == null || !result.Any())
            {
                var response = new Response<IEnumerable<ClinicianDto>> { Data = result, Code = 0, Message = "No Records Found" };
                return new R<IEnumerable<ClinicianDto>>(HttpStatusCode.NotFound, response, Request);
            }

            return new R<IEnumerable<ClinicianDto>>(HttpStatusCode.OK, new Response<IEnumerable<ClinicianDto>> { Data = result }, Request);
        }


        [CustomEx]
        public async Task<IHttpActionResult> GetAppointmentStatusListAsync()
        {

            var exclusions = new[] { "6", "7", "8", "9", "10", "11", "12" };
            var r = new Response<List<SelectList>>
            {
                Data = await _cservice.GetListByCategory("4903", exclusions.ToList())
            };

            return new R<List<SelectList>>(HttpStatusCode.OK, r, Request);
        }


        [CustomEx]
        [HttpPost]
        public async Task<IHttpActionResult> SaveClinicianOffTimingAsync([FromBody]VacationDto m)
        {
            //var userId = GetClaimValue();


            var r = new Response<CommonDto>();
            var result = await _service.SaveCliniciansOffTimingsAsync(m, m.LoggedInUserId);
            long newId = 0;
            if (long.TryParse(result, out newId))
            {
                r.Code = 1;
                r.Data = new CommonDto { NewId = newId };
                r.Message = "Success";
                return new R<CommonDto>(HttpStatusCode.OK, r, Request);
            }
            else
            {
                r.Code = 0;
                r.Data = new CommonDto { NewId = 0 };
                r.Message = result;
                return new R<CommonDto>(HttpStatusCode.Conflict, r, Request);
            }
        }


        [HttpPost]
        [CustomEx]
        public IHttpActionResult UpdateAppointment([FromBody]AppointmentDto m)
        {
            var result = _service.CompleteAppointment(m.Id, Convert.ToInt32(m.PatientId), m.AppointmentDetails);
            return new R<bool>(HttpStatusCode.OK, new Response<bool> { Data = result }, Request);
        }
    }
}
