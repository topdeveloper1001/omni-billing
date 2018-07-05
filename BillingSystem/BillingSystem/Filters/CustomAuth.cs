using BillingSystem.Common;
using BillingSystem.Common.Common;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace BillingSystem.Filters
{
    public class CustomAuth : AuthorizeAttribute
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
            var status = Helpers.GetLoggedInUserId() > 0 ? 0 : 1;
            var filterResult = string.Empty;

            if (status == 0)
            {
                var currentController = filterContext.RouteData.Values.ContainsKey(OtherKeys.controller.ToString())
                        ? Convert.ToString(filterContext.RouteData.Values[OtherKeys.controller.ToString()]) : string.Empty;

                var currentAction = filterContext.RouteData.Values.ContainsKey(OtherKeys.action.ToString())
                    ? Convert.ToString(filterContext.RouteData.Values[OtherKeys.action.ToString()]) : string.Empty;

                var isAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();
                status = Helpers.CheckAccess(currentController, currentAction, isAjaxRequest) || currentAction.ToLower().Equals("welcome") ? 0 : 2;
            }

            if (status > 0)
            {
                switch (status)
                {
                    case 1:
                        filterResult = CommonConfig.LoginUrl;
                        break;
                    case 2:
                        filterResult = CommonConfig.UnauthorizedAccess;
                        break;
                    default:
                        break;
                }

                filterContext.Result = new RedirectResult(filterResult, false);
                base.OnAuthorization(filterContext);
            }
        }
    }
}
