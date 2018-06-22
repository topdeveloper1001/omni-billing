using BillingSystem.Bal.Interfaces;
using BillingSystem.WebApi.Common;
using BillingSystem.WebApi.Filters;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BillingSystem.Model.EntityDto;
using System.Collections.Generic;

namespace BillingSystem.WebApi.Controllers
{
    [JwtAuthenticate]
    public class FavController : ApiController
    {
        private readonly IPatientService _service;

        public FavController(IPatientService service)
        {
            _service = service;
        }

        [CustomEx]
        [HttpGet]
        public async Task<IHttpActionResult> GetFavoriteCliniciansAsync([FromUri]long patientId)
        {
            var r = new Response<List<FavoriteClinicianDto>>();
            var result = await _service.GetFavoriteCliniciansAsync(patientId);
            r.Data = result;
            return new R<List<FavoriteClinicianDto>>(HttpStatusCode.OK, r, Request);
        }

        [CustomEx]
        [HttpPost]
        public async Task<IHttpActionResult> AddFavoriteClinicianAsync([FromBody]FavoriteClinicianDto m)
        {
            var r = new Response<CommonDto> { Code = 1, Message = "Success" };

            var result = await _service.AddClinicianAsFavoriteAsync(m);
            if (result <= 0)
            {
                r.Code = 0;
                r.Message = "Error";
                r.Data = new CommonDto { NewId = 0 };
                return new R<CommonDto>(HttpStatusCode.InternalServerError, r, Request);
            }
            else
                r.Data = new CommonDto { NewId = result };

            return new R<CommonDto>(HttpStatusCode.OK, r, Request);
        }

        [CustomEx]
        [HttpPost]
        public IHttpActionResult RemoveFavorite([FromBody]FavoriteClinicianDto m)
        {
            var response = new Response<bool> { Code = 1, Message = "Success", Data = true };
            var result = _service.RemoveClinicianAsFavorite(m.Id);
            if (result <= 0)
            {
                response.Code = 0;
                response.Message = result == 0 ? "No Record Found!" : "Error in Web API";
                response.Data = false;
                return new R<bool>(result == 0 ? HttpStatusCode.NotFound : HttpStatusCode.InternalServerError, response, Request);
            }
            return new R<bool>(HttpStatusCode.OK, response, Request);
        }
    }
}
