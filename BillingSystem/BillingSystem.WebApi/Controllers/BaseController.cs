using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace BillingSystem.WebApi.Controllers
{
    public class BaseController : ApiController
    {
        public static string GetClaimValue(string key = "")
        {
            if (string.IsNullOrEmpty(key))
                key = "userid";

            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }

        public static long UserId
        {
            get
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                if (identity == null)
                    return 0;

                var claim = identity.Claims.FirstOrDefault(c => c.Type.Equals("userid"));
                var id = claim?.Value;
                return (!string.IsNullOrEmpty(id)) ? Convert.ToInt64(id) : 0;
            }
        }
    }
}
