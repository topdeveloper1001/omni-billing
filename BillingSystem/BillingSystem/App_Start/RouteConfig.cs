using BillingSystem.Common;
using System.Web.Mvc;
using System.Web.Routing;

namespace BillingSystem
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = CommonConfig.LoginController, action = CommonConfig.LoginAction, id = UrlParameter.Optional }
            );
        }
    }
}