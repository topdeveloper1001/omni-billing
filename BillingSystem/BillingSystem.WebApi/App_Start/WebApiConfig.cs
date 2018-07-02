using BillingSystem.WebApi.Filters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Application;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace BillingSystem.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new CustomEx());

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