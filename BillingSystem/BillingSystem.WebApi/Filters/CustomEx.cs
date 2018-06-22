using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;

namespace BillingSystem.WebApi.Filters
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class CustomEx : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exceptionMessage = actionExecutedContext.Exception.InnerException == null
                           ? actionExecutedContext.Exception.Message
                           : actionExecutedContext.Exception.InnerException.Message;

            var error = new ErrorResult
            {
                Code = 0,
                Message = exceptionMessage,
                Data = "{}"
                //ReasonPhrase = exceptionMessage
            };
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ReasonPhrase = "Wep API Error",
                Content = new ObjectContent(typeof(ErrorResult), error, new JsonMediaTypeFormatter())
            };

            actionExecutedContext.Response = response;
        }
    }
}
