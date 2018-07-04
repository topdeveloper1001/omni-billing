using BillingSystem.Common;
using BillingSystem.Common.Common;
using System;
using System.Web.Mvc;

namespace BillingSystem.Filters
{
    /// <summary>
    /// Custom authorization attribute for setting per-method accessibility 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CheckPermissionsAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// The name of each action that must be permissible for this method, separated by a comma.
        /// </summary>
        //public string Permissions { get; set; }

        //public string CurrentController { get; set; }
        //public string CurrentAction { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var currentController = filterContext.RouteData.Values.ContainsKey(OtherKeys.controller.ToString())
                ? Convert.ToString(filterContext.RouteData.Values[OtherKeys.controller.ToString()]) : string.Empty;

            var currentAction = filterContext.RouteData.Values.ContainsKey(OtherKeys.action.ToString())
                ? Convert.ToString(filterContext.RouteData.Values[OtherKeys.action.ToString()]) : string.Empty;

            var isAuthorized = Helpers.CheckAccess(currentController, currentAction);

            if (!isAuthorized)
                filterContext.Result = new RedirectResult(CommonConfig.UnauthorizedAccess, false);
        }

        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{
        //    bool isUserAuthorized = base.AuthorizeCore(httpContext);
        //    return isUserAuthorized && Helpers.CheckAccess(CurrentController, CurrentAction);
        //}
    }
}
