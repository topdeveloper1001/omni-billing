using BillingSystem.WebApi.Common;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace BillingSystem.WebApi.Filters
{
    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        public string ReasonPhrase { get; }

        public HttpRequestMessage Request { get; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            var response = new ErrorResult
            {
                Message = "Unauthorized",
                ReasonPhrase = ReasonPhrase
            };
            return Request.CreateResponse(HttpStatusCode.Unauthorized, response, new JsonMediaTypeFormatter());
        }
    }

    public class ErrorResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public string ReasonPhrase { get; set; }
    }
}
