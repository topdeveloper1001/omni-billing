using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model.EntityDto;
using BillingSystem.WebApi.Common;
using BillingSystem.WebApi.Filters;
using BillingSystem.WebApi.Middleware;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace BillingSystem.WebApi.Controllers
{
    public class PatientController : ApiController
    {
        private readonly IPatientService _service;
        private readonly IUsersService _uservice;

        public PatientController(IPatientService service, IUsersService uservice)
        {
            _service = service;
            _uservice = uservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [CustomEx]
        public async Task<R<TokenResponse>> AddNewPatientAsync(PatientDto p)
        {
            var r = new Response<TokenResponse>();
            var m = await _service.SavePatientDetails(p);
            if (m != null && m.UserID > 0)
            {
                r.Code = 1;
                r.Message = "Authenticated";
                r.Data = JwtMiddleware.GenerateToken(m);
                return new R<TokenResponse>(HttpStatusCode.OK, r, Request);
            }
            else
            {
                r.Code = 0;
                r.Message = "Email Already Exists!";
                r.Data = new TokenResponse();
            }
            return new R<TokenResponse>(HttpStatusCode.Ambiguous, r, Request);
        }

        [JwtAuthenticate]
        [HttpGet]
        [CustomEx]
        public async Task<R<UserDto>> GetUserDetailsAsync([FromUri]long userId, bool isPatient)
        {
            var response = new Response<UserDto>();
            var m = await _uservice.GetUserAsync(userId, isPatient);
            if (m != null && m.UserID > 0)
            {
                response.Data = m;
                return new R<UserDto>(HttpStatusCode.OK, response, Request);
            }
            else
            {
                response.Data = new UserDto();
                return new R<UserDto>(HttpStatusCode.NotFound, response, Request);
            }
        }

        [JwtAuthenticate]
        [HttpPost]
        [CustomEx]
        public async Task<R<UserDto>> UpdatePatientDetailsAsync(PatientDto p)
        {
            var r = new Response<UserDto>();
            var m = await _service.SavePatientDetails(p);
            if (m != null && m.UserID > 0)
            {
                r.Data = m;
                return new R<UserDto>(HttpStatusCode.OK, r, Request);
            }
            else
            {
                r.Code = 0;
                r.Message = "User Not Found";
                r.Data = Enumerable.Empty<UserDto>().FirstOrDefault();
                return new R<UserDto>(HttpStatusCode.NotFound, r, Request);
            }
        }


        [JwtAuthenticate]
        [CustomEx]
        public async Task<R<bool>> SaveUserLocationAsync([FromBody]LocationDto l)
        {
            var r = new Response<bool>();
            var m = await _uservice.SaveUserLocationAsync(l.Latitude, l.Longitude, l.UserId, true);
            r.Data = m;
            return new R<bool>(HttpStatusCode.OK, r, Request);
        }


        [JwtAuthenticate]
        [CustomEx]
        public async Task<IHttpActionResult> GetPatientsByUserIdAsync(long userId = 0)
        {
            var list = await _uservice.GetPatientsByUserIdAsync(userId);
            var r = new Response<List<PatientDto>>
            {
                Data = list
            };

            if (!list.Any())
                r.Message = "No Records Found";

            return new R<List<PatientDto>>(HttpStatusCode.OK, r, Request);
        }
    }
}
