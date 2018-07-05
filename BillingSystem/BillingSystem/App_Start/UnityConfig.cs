using AutoMapper;
using BillingSystem.Bal;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
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
            container.RegisterSingleton<IAppointmentTypesService, AppointmentTypesService>();
            container.RegisterSingleton<IATCCodesService, ATCCodesService>();
            container.RegisterSingleton<IAuditLogService, AuditLogService>();
            container.RegisterSingleton<IAuthorizationService, AuthorizationService>();
            container.RegisterSingleton<IBedChargesService, BedChargesService>();
            container.RegisterSingleton<IBedMasterService, BedMasterService>();
            container.RegisterSingleton<IBedRateCardService, BedRateCardService>();
            container.RegisterSingleton<IBillActivityService, BillActivityService>();
            container.RegisterSingleton<IBillHeaderService, BillHeaderService>();
            container.RegisterSingleton<IBillingModifierService, BillingModifierService>();
            container.RegisterSingleton<IBillingSystemParametersService, BillingSystemParametersService>();
            container.RegisterSingleton<ICarePlanService, CarePlanService>();
            container.RegisterSingleton<ICategoriesService, CategoriesService>();
            container.RegisterSingleton<ICarePlanTaskService, CarePlanTaskService>();
            container.RegisterSingleton<ICityService, CityService>();
            container.RegisterSingleton<IClinicianAppointmentTypesService, ClinicianAppointmentTypesService>();
            container.RegisterSingleton<IClinicianRosterService, ClinicianRosterService>();
            container.RegisterSingleton<ICorporateService, CorporateService>();
            container.RegisterSingleton<ICountryService, CountryService>();
            container.RegisterSingleton<ICPTCodesService, CPTCodesService>();
            container.RegisterSingleton<IDashboardBudgetService, DashboardBudgetService>();
            container.RegisterSingleton<IDashboardDisplayOrderService, DashboardDisplayOrderService>();
            container.RegisterSingleton<IDashboardIndicatorDataService, DashboardIndicatorDataService>();
            container.RegisterSingleton<IDashboardIndicatorsService, DashboardIndicatorsService>();
            container.RegisterSingleton<IDashboardParametersService, DashboardParametersService>();
            container.RegisterSingleton<IDashboardRemarkService, DashboardRemarkService>();
            container.RegisterSingleton<IDashboardService, DashboardService>();
            container.RegisterSingleton<IDashboardTargetsService, DashboardTargetsService>();
            container.RegisterSingleton<IDashboardTransactionCounterService, DashboardTransactionCounterService>();
            container.RegisterSingleton<IDenialService, DenialService>();
            container.RegisterSingleton<IDeptTimmingService, DeptTimmingService>();
            container.RegisterSingleton<IDiagnosisCodeService, DiagnosisCodeService>();
            container.RegisterSingleton<IDiagnosisService, DiagnosisService>();
            container.RegisterSingleton<IDischargeSummaryDetailService, DischargeSummaryDetailService>();
            container.RegisterSingleton<IDocumentsTemplatesService, DocumentsTemplatesService>();
            container.RegisterSingleton<IDRGCodesService, DRGCodesService>();
            container.RegisterSingleton<IDrugAllergyLogService, DrugAllergyLogService>();
            container.RegisterSingleton<IDrugInstructionAndDosingService, DrugInstructionAndDosingService>();
            container.RegisterSingleton<IDrugInteractionsService, DrugInteractionsService>();
            container.RegisterSingleton<IDrugService, DrugService>();
            container.RegisterSingleton<IEncounterService, EncounterService>();
            container.RegisterSingleton<IEquipmentService, EquipmentService>();
            container.RegisterSingleton<IErrorMasterService, ErrorMasterService>();
            container.RegisterSingleton<IFacilityDepartmentService, FacilityDepartmentService>();
            container.RegisterSingleton<IFacilityRoleService, FacilityRoleService>();
            container.RegisterSingleton<IFacilityService, FacilityService>();
            container.RegisterSingleton<IFacilityStructureService, FacilityStructureService>();
            container.RegisterSingleton<IFacultyRoosterService, FacultyRoosterService>();
            container.RegisterSingleton<IFacultyTimeslotsService, FacultyTimeslotsService>();
            container.RegisterSingleton<IFavoritesService, FavoritesService>();
            container.RegisterSingleton<IFutureOpenOrderService, FutureOpenOrderService>();
            container.RegisterSingleton<IFutureOrderActivityService, FutureOrderActivityService>();
            container.RegisterSingleton<IGlobalCodeCategoryMasterService, GlobalCodeCategoryMasterService>();
            container.RegisterSingleton<IGlobalCodeCategoryService, GlobalCodeCategoryService>();
            container.RegisterSingleton<IGlobalCodeService, GlobalCodeService>();
            container.RegisterSingleton<IHCPCSCodesService, HCPCSCodesService>();
            container.RegisterSingleton<IHolidayPlannerDetailsService, HolidayPlannerDetailsService>();
            container.RegisterSingleton<IHolidayPlannerService, HolidayPlannerService>();
            container.RegisterSingleton<IIndicatorDataCheckListService, IndicatorDataCheckListService>();
            container.RegisterSingleton<IInsuranceCompanyService, InsuranceCompanyService>();
            container.RegisterSingleton<IInsurancePlansService, InsurancePlansService>();
            container.RegisterSingleton<IInsurancePolicesService, InsurancePolicesService>();
            container.RegisterSingleton<ILabTestOrderSetService, LabTestOrderSetService>();
            container.RegisterSingleton<ILabTestResultService, LabTestResultService>();
            container.RegisterSingleton<ILoginTrackingService, LoginTrackingService>();
            container.RegisterSingleton<IManagedCareService, ManagedCareService>();
            container.RegisterSingleton<IManualChargesTrackingService, ManualChargesTrackingService>();
            container.RegisterSingleton<IManualDashboardService, ManualDashboardService>();
            container.RegisterSingleton<IMappingPatientBedService, MappingPatientBedService>();
            container.RegisterSingleton<IMcContractService, McContractService>();
            container.RegisterSingleton<IMCOrderCodeRatesService, MCOrderCodeRatesService>();
            container.RegisterSingleton<IMCRulesTableService, MCRulesTableService>();
            container.RegisterSingleton<IMedicalHistoryService, MedicalHistoryService>();
            container.RegisterSingleton<IMedicalNecessityService, MedicalNecessityService>();
            container.RegisterSingleton<IMedicalNotesService, MedicalNotesService>();
            container.RegisterSingleton<IMedicalRecordService, MedicalRecordService>();
            container.RegisterSingleton<IMedicalVitalService, MedicalVitalService>();
            container.RegisterSingleton<IMissingDataService, MissingDataService>();
            container.RegisterSingleton<IModuleAccessService, ModuleAccessService>();
            container.RegisterSingleton<IOpenOrderActivityScheduleService, OpenOrderActivityScheduleService>();
            container.RegisterSingleton<IOpenOrderService, OpenOrderService>();
            container.RegisterSingleton<IOperatingRoomService, OperatingRoomService>();
            container.RegisterSingleton<IOrderActivityService, OrderActivityService>();
            container.RegisterSingleton<IParametersService, ParametersService>();
            container.RegisterSingleton<IPatientAddressRelationService, PatientAddressRelationService>();
            container.RegisterSingleton<IPatientCareActivitiesService, PatientCareActivitiesService>();
            container.RegisterSingleton<IPatientCarePlanService, PatientCarePlanService>();
            container.RegisterSingleton<IPatientDischargeSummaryService, PatientDischargeSummaryService>();
            container.RegisterSingleton<IPatientEvaluationService, PatientEvaluationService>();
            container.RegisterSingleton<IPatientInfoChangesQueueService, PatientInfoChangesQueueService>();
            container.RegisterSingleton<IPatientInfoService, PatientInfoService>();
            container.RegisterSingleton<IPatientInsuranceService, PatientInsuranceService>();
            container.RegisterSingleton<IPatientLoginDetailService, PatientLoginDetailService>();
            container.RegisterSingleton<IPatientPhoneService, PatientPhoneService>();
            container.RegisterSingleton<IPatientPreSchedulingService, PatientPreSchedulingService>();
            container.RegisterSingleton<IPaymentService, PaymentService>();
            container.RegisterSingleton<IPaymentTypeDetailService, PaymentTypeDetailService>();
            container.RegisterSingleton<IPDFTemplatesService, PDFTemplatesService>();
            container.RegisterSingleton<IPhysicianService, PhysicianService>();
            container.RegisterSingleton<IPlaceOfServiceService, PlaceOfServiceService>();
            container.RegisterSingleton<IPreliminaryBillService, PreliminaryBillService>();
            container.RegisterSingleton<IPreSchedulingLinkService, PreSchedulingLinkService>();
            container.RegisterSingleton<IProjectDashboardService, ProjectDashboardService>();
            container.RegisterSingleton<IProjectsService, ProjectsService>();
            container.RegisterSingleton<IProjectTargetsService, ProjectTargetsService>();
            container.RegisterSingleton<IProjectTasksService, ProjectTasksService>();
            container.RegisterSingleton<IProjectTaskTargetsService, ProjectTaskTargetsService>();
            container.RegisterSingleton<IReportingService, ReportingService>();
            container.RegisterSingleton<IRolePermissionService, RolePermissionService>();
            container.RegisterSingleton<IRoleService, RoleService>();
            container.RegisterSingleton<IRoleTabsService, RoleTabsService>();
            container.RegisterSingleton<IRuleMasterService, RuleMasterService>();
            container.RegisterSingleton<IRuleStepService, RuleStepService>();
            container.RegisterSingleton<ISchedulingParametersService, SchedulingParametersService>();
            container.RegisterSingleton<ISchedulingService, SchedulingService>();
            container.RegisterSingleton<IScreenService, ScreenService>();
            container.RegisterSingleton<IScrubEditTrackService, ScrubEditTrackService>();
            container.RegisterSingleton<IScrubReportService, ScrubReportService>();
            container.RegisterSingleton<IServiceCodeService, ServiceCodeService>();
            container.RegisterSingleton<IStateService, StateService>();
            container.RegisterSingleton<ISystemConfigurationService, SystemConfigurationService>();
            container.RegisterSingleton<ITabsService, TabsService>();
            container.RegisterSingleton<ITechnicalSpecificationsService, TechnicalSpecificationsService>();
            container.RegisterSingleton<ITpFileHeaderService, TpFileHeaderService>();
            container.RegisterSingleton<ITPXMLParsedDataService, TPXMLParsedDataService>();
            container.RegisterSingleton<IUploadChargesService, UploadChargesService>();
            container.RegisterSingleton<IUserRoleService, UserRoleService>();
            container.RegisterSingleton<IUsersService, UsersService>();
            container.RegisterSingleton<IXactivityService, XactivityService>();
            container.RegisterSingleton<IXAdviceXMLParsedDataService, XAdviceXMLParsedDataService>();
            container.RegisterSingleton<IXclaimService, XclaimService>();
            container.RegisterSingleton<IXFileHeaderService, XFileHeaderService>();
            container.RegisterSingleton<IXMLBillingService, XMLBillingService>();
            container.RegisterSingleton<IXmlReportingService, XmlReportingService>();
            container.RegisterSingleton<IXPaymentFileXMLService, XPaymentFileXMLService>();
            container.RegisterSingleton<IXPaymentReturnService, XPaymentReturnService>();

            container.RegisterType(typeof(IRepository<>), typeof(Repository<>));

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