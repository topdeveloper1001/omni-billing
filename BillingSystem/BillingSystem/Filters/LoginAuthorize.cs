using BillingSystem.Common;
using System.Web.Mvc;
using System.Web.Routing;

namespace BillingSystem.Filters
{
    public class LoginAuthorize : AuthorizeAttribute
    {
        /// <summary>
        /// Function     : Handle Unauthorized Request
        /// Objective    : This function to Overide default HandleUnauthorizedRequest function
        /// </summary>
        /// <param name="context">
        /// The filter Context.
        /// </param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            if (context.HttpContext.Request.Url != null)
            {
                var values = new RouteValueDictionary(new
                {
                    action = ActionResults.login,
                    controller = ControllerNames.home,
                    ReturnUrl = context.HttpContext.Request.Url.PathAndQuery //to be used later
                });

                context.Result = new RedirectToRouteResult(values);
            }
            else
            {
                context.Result = new RedirectResult(CommonConfig.LoginUrl, false);
            }
        }

        /// <summary>
        /// Function     : OnAuthorization
        /// Objective    : This function to Overide default OnAuthorization function
        /// </summary>
        /// <param name="filterContext">
        /// The filter Context.
        /// </param>
        /// <returns>
        /// </returns>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var userId = Helpers.GetLoggedInUserId();
            if (userId == 0)
            {
                base.OnAuthorization(filterContext);
                filterContext.Result = new RedirectResult(CommonConfig.LoginUrl, false);
            }
        }
    }
}
