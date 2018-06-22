using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace BillingSystem.WebApi.Common
{
    public class R<T> : IHttpActionResult
    {
        public readonly HttpStatusCode _httpStatusCode;
        public readonly HttpRequestMessage _request;
        Response<T> _value;

        public R(HttpStatusCode httpStatusCode, Response<T> value, HttpRequestMessage request)
        {
            _httpStatusCode = httpStatusCode;
            _value = value;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            switch (_httpStatusCode)
            {
                case HttpStatusCode.Continue:
                    break;
                case HttpStatusCode.SwitchingProtocols:
                    break;
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.Found:
                    _value.Code = 1;
                    _value.Message = "Success";
                    break;
                case HttpStatusCode.NonAuthoritativeInformation:
                    break;
                case HttpStatusCode.NoContent:
                    _value.Code = 1;
                    _value.Message = "No Records Found";
                    break;
                case HttpStatusCode.ResetContent:
                    break;
                case HttpStatusCode.PartialContent:
                    break;
                case HttpStatusCode.Ambiguous:
                    break;
                case HttpStatusCode.Moved:
                    break;
                case HttpStatusCode.SeeOther:
                    break;
                case HttpStatusCode.NotModified:
                    break;
                case HttpStatusCode.UseProxy:
                    break;
                case HttpStatusCode.Unused:
                    break;
                case HttpStatusCode.TemporaryRedirect:
                    break;
                case HttpStatusCode.BadRequest:
                    _value.Code = 0;
                    _value.Message = "Error in Web API";
                    break;
                case HttpStatusCode.Unauthorized:
                    _value.Code = -1;
                    _value.Message = "Unauthorized";
                    break;
                case HttpStatusCode.PaymentRequired:
                    break;
                case HttpStatusCode.Forbidden:
                    break;
                case HttpStatusCode.NotFound:
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    break;
                case HttpStatusCode.NotAcceptable:
                    break;
                case HttpStatusCode.ProxyAuthenticationRequired:
                    break;
                case HttpStatusCode.RequestTimeout:
                    break;
                case HttpStatusCode.Conflict:
                    break;
                case HttpStatusCode.Gone:
                    break;
                case HttpStatusCode.LengthRequired:
                    break;
                case HttpStatusCode.PreconditionFailed:
                    break;
                case HttpStatusCode.RequestEntityTooLarge:
                    break;
                case HttpStatusCode.RequestUriTooLong:
                    break;
                case HttpStatusCode.UnsupportedMediaType:
                    break;
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                    break;
                case HttpStatusCode.ExpectationFailed:
                    break;
                case HttpStatusCode.UpgradeRequired:
                    break;
                case HttpStatusCode.InternalServerError:
                    break;
                case HttpStatusCode.NotImplemented:
                    break;
                case HttpStatusCode.BadGateway:
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    break;
                case HttpStatusCode.GatewayTimeout:
                    break;
                case HttpStatusCode.HttpVersionNotSupported:
                    break;
                default:
                    break;
            }

            var response = new HttpResponseMessage()
            {
                StatusCode = _httpStatusCode,
                RequestMessage = _request,
                Content = new ObjectContent(typeof(Response<T>), _value, new JsonMediaTypeFormatter())
            };

            return Task.FromResult(response);
        }
    }

    public class Response<T>
    {
        public int Code { get; set; } = -1;
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
