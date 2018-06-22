using BillingSystem.WebApi.App_Start;
using BillingSystem.WebApi.Filters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Application;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Routing;

namespace BillingSystem.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //var cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors);
            //config.MessageHandlers.Add(new PreflightRequestsHandler());

            //StructuremapWebApi.Start();


            //config.Filters.Clear();
            //config.Filters.Add(new CustomEx());

            //config.Routes.Clear();
            //config.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            //config.Routes.MapHttpRoute("DefaultApiWithActionId", "Api/{controller}/{action}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            //config.Routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}");
            //config.Routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //config.Routes.MapHttpRoute("DefaultApiPost", "Api/{controller}/{action}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            ////config.Routes.MapHttpRoute("ControllerAndAction", "api/{controller}/{action}");

            //var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            //jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();


            config.Filters.Add(new CustomEx());

            //config.Services.Replace(typeof(IExceptionHandler), new CustomEx());
            // Web API configuration and services
            StructuremapWebApi.Start();

            config.Routes.MapHttpRoute(
            name: "swagger_root",
            routeTemplate: "",
            defaults: null,
            constraints: null,
            handler: new RedirectHandler((message => message.RequestUri.ToString()), "swagger"));

            config.Routes.MapHttpRoute("ControllerAndAction", "api/{controller}/{action}");

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}