using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
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
    public class HomeController : BaseController
    {
        private readonly IAddressService _addressService;
        private readonly ICommonService _service;
        private readonly IUserService _uservice;
        private readonly IFacilityService _fservice;

        public HomeController(IAddressService addressService, ICommonService service, IUserService uservice, IFacilityService fservice)
        {
            _addressService = addressService;
            _service = service;
            _uservice = uservice;
            _fservice = fservice;
        }

        [CustomEx]
        public async Task<IHttpActionResult> GetCountriesAsync()
        {
            var r = new Response<List<SelectList>>();
            var result = await _addressService.GetCountriesAsync();
            r.Data = result;
            return new R<List<SelectList>>(HttpStatusCode.OK, r, Request);
        }


        [CustomEx]
        public async Task<IHttpActionResult> GetStatesAsync([FromUri]long countryId)
        {
            var r = new Response<List<SelectList>>();
            var result = await _addressService.GetStatesAsync(countryId);
            r.Data = result;
            return new R<List<SelectList>>(HttpStatusCode.OK, r, Request);
        }

        [CustomEx]
        public async Task<IHttpActionResult> GetCitiesAsync(long stateId)
        {
            var list = await _addressService.GetCitiesAsync(stateId);

            var r = new Response<IEnumerable<SelectList>>
            {
                Data = list
            };

            if (!list.Any())
                r.Message = "No Records Found";
            return new R<IEnumerable<SelectList>>(HttpStatusCode.OK, r, Request);
        }

        [CustomEx]
        public async Task<IHttpActionResult> GetDefaultDataAsync(long userId)
        {
            var data = await _addressService.GetDefaultDataAsync(userId);
            var r = new Response<DefaultDataDto>
            {
                Data = data
            };

            if (data == null)
                r.Message = "No Records Found";

            return new R<DefaultDataDto>(HttpStatusCode.OK, r, Request);
        }

        [CustomEx]
        public async Task<IHttpActionResult> GetEntitiesByAddress(long cityId = 0, long corporateId = 0)
        {
            var list = await _addressService.GetEntitiesByCityAsync(cityId, corporateId);
            var r = new Response<List<SelectList>>
            {
                Data = list
            };

            if (!list.Any())
                r.Message = "No Records Found";

            return new R<List<SelectList>>(HttpStatusCode.OK, r, Request);
        }

        [CustomEx]
        public async Task<IHttpActionResult> GetCorporatesAsync(long corporateId = 0, long userId = 0)
        {
            var list = await _addressService.GetCorporatesAsync(corporateId, userId);
            var r = new Response<List<SelectList>>
            {
                Data = list
            };

            if (!list.Any())
                r.Message = "No Records Found";

            return new R<List<SelectList>>(HttpStatusCode.OK, r, Request);
        }

        [CustomEx]
        public async Task<IHttpActionResult> GetOffTimingReasonsAsync()
        {
            var list = await _service.GetListByCategory("80441");
            var r = new Response<List<SelectList>>
            {
                Data = list
            };

            if (!list.Any())
                r.Message = "No Records Found";

            return new R<List<SelectList>>(HttpStatusCode.OK, r, Request);
        }

    }
}
