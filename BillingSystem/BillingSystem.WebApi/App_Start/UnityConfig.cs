using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using System.Web.Http;
using Unity;
using Unity.WebApi;
using BillingSystem.Bal;
using Unity.Lifetime;
using BillingSystem.Model;
using Unity.Injection;
using System.Data.Entity;
using AutoMapper;
using BillingSystem.Bal;

namespace BillingSystem.WebApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterType<ICommonService, CommonService>();
            container.RegisterType<IAppointmentService, AppointmentService>();
            container.RegisterType<IPatientService, PatientService>();
            container.RegisterType<IClinicianService, ClinicianService>();
            container.RegisterType<IFacilityService, FacilityService>();
            container.RegisterType<IAddressService, AddressService>();
            container.RegisterType<ICommonService, CommonService>();

            container.RegisterType(typeof(IRepository<>), typeof(Repository<>), new TransientLifetimeManager());
            container.RegisterType<IMapper>(new InjectionFactory(XmlConfigurator =>
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                });
                return mapperConfig.CreateMapper();
            }));

            container.RegisterType<DbContext, BillingEntities>(new PerThreadLifetimeManager());


            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}