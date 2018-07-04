using BillingSystem.Common;
using System.Web.Mvc;
using System.Web.Routing;

namespace BillingSystem.Filters
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var e = filterContext.HttpContext;
            if (Helpers.GetLoggedInUserId() == 0)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    // For AJAX requests, we're overriding the returned JSON result with a simple string,
                    // indicating to the calling JavaScript code that a redirect should be performed.
                    filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    //var context = new HttpContextWrapper(new HttpContext(null,response:new HttpResponse(badRequest));
                    filterContext.Result = new JsonResult
                    {
                        Data = "_Logon_",
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet
                    };
                }
                else
                {
                    filterContext.Result =
                        new RedirectToRouteResult(
                            new RouteValueDictionary
                                {
                                        { "action", "Index" },
                                        { "controller", "Login" },
                                        { "returnUrl", filterContext.HttpContext.Request.RawUrl }
                                });
                }
                return;
            }
        }
    }
}
