using BillingSystem.Common;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace BillingSystem.Filters
{
    public class CheckRolesAuthorize : AuthorizeAttribute
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
                    action = CommonConfig.LoginAction,
                    controller = CommonConfig.LoginController,
                    ReturnUrl = context.HttpContext.Request.Url.PathAndQuery //to be used later
                });

                context.Result = new RedirectToRouteResult(values);
            }
            else
            {
                context.Result = new RedirectResult(CommonConfig.LoginUrl, false);
            }
        }

        public CheckRolesAuthorize(params string[] roleKeys)
        {
            var roles = new List<string>();
            var allRoles = (NameValueCollection)ConfigurationManager.GetSection("CustomRoles");
            foreach (var roleKey in roleKeys)
                roles.AddRange(allRoles[roleKey].Split(new[] { ',' }));

            Roles = string.Join(",", roles);
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
            var roles = Roles.Trim().ToLower();
            var currentRole = Helpers.CurrentRoleKey.Trim().ToLower();
            var currentAccess = string.IsNullOrEmpty(Roles) || roles.Contains(currentRole) || roles.Equals("all");
            if (userId == 0 || !currentAccess)
            {
                base.OnAuthorization(filterContext);
                filterContext.Result = !currentAccess
                    ? new RedirectResult(CommonConfig.UnauthorizedAccess, false)
                    : new RedirectResult(CommonConfig.LoginUrl, false);

            }
        }
    }
}
