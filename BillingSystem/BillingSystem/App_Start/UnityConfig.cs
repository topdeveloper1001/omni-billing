using AutoMapper;
using BillingSystem.Bal;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository;
using BillingSystem.Repository.Interfaces;
using System.Data.Entity;
using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Mvc5;

namespace BillingSystem
{
    public static class UnityConfig
    {
        public static IUnityContainer RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterType<IAppointmentTypesService, AppointmentTypesService>();
            container.RegisterType<ICategoriesService, CategoriesService>();
            container.RegisterType<ITechnicalSpecificationsService, TechnicalSpecificationsService>();
            container.RegisterType<ICatalogService, CatalogService>();
            container.RegisterType<IATCCodesService, ATCCodesService>(); 
            container.RegisterType<IAuditLogService, AuditLogService>(); 
            container.RegisterType<IAuthorizationService, AuthorizationService>(); 
            container.RegisterType<IBedChargesService, BedChargesService>(); 
            container.RegisterType<IBedMasterService, BedMasterService>(); 
            container.RegisterType<IBedRateCardService, BedRateCardService>(); 
            container.RegisterType<IBillActivityService, BillActivityService>(); 
            container.RegisterType<IBillHeaderService, BillHeaderService>(); 
            container.RegisterType<IBillingModifierService, BillingModifierService>(); 
            container.RegisterType<IBillingSystemParametersService, BillingSystemParametersService>(); 
            container.RegisterType<ICarePlanService, CarePlanService>(); 
            container.RegisterType<ICarePlanTaskService, CarePlanTaskService>(); 
            container.RegisterType<ICityService, CityService>(); 
            container.RegisterType<IClinicianAppointmentTypesService, ClinicianAppointmentTypesService>(); 
            container.RegisterType<IClinicianRosterService, ClinicianRosterService>(); 
            container.RegisterType<ICorporateService, CorporateService>(); 
            container.RegisterType<ICountryService, CountryService>(); 
            container.RegisterType<ICPTCodesService, CPTCodesService>(); 
            container.RegisterType<IDashboardBudgetService, DashboardBudgetService>(); 
            container.RegisterType<IDashboardService, DashboardService>(); 
            container.RegisterType<IEncounterService, EncounterService>(); 
            container.RegisterType<IFacilityStructureService, FacilityStructureService>(); 
            container.RegisterType<IPatientInfoService, PatientInfoService>(); 
            container.RegisterType<IPreliminaryBillService, PreliminaryBillService>(); 
            container.RegisterType<IUsersService, UsersService>();  
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

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }
    }
}