using Hangfire;
using System.Configuration;
using Owin;

namespace BillingSystem
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            GlobalConfiguration.Configuration.UseSqlServerStorage(
                ConfigurationManager.ConnectionStrings["BillingEntities"].ConnectionString);

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}