using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BillingSystem.Common;
using BillingSystem.WebApi.Common;
using BillingSystem.WebApi.Middleware;
using BillingSystem.Model.EntityDto;
using BillingSystem.WebApi.Filters;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.WebApi.Controllers
{
    public class AuthController : ApiController
    {
        private readonly IUsersService _service;

        public AuthController(IUsersService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost]
        [CustomEx]
        public async Task<IHttpActionResult> AuthenticateAsync([FromBody]UserDto vm)
        {
            //EncryptDecrypt.GetDecryptedData("O1yC58YWrr/HGFQyfok2Gw==", "");
            var r = new Response<TokenResponse>
            {
                Message = "Invalid Username or Password",
                Data = new TokenResponse()
            };

            var u = await _service.AuthenticateAsync(vm.UserName, vm.Password, vm.DeviceToken, vm.Platform, vm.IsPatient);

            if (u != null && u.UserID > 0)
            {
                r.Data = JwtMiddleware.GenerateToken(u);
                return new R<TokenResponse>(HttpStatusCode.OK, r, Request);
            }
            else
                return new R<TokenResponse>(HttpStatusCode.NotFound, r, Request);
        }

        [CustomEx]
        [JwtAuthenticate]
        public IHttpActionResult LogOff()
        {
            return Ok("Success");
        }
    }
}
