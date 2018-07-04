using AutoMapper;
using BillingSystem.Bal;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository;
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
            container.RegisterType<IDashboardDisplayOrderService, DashboardDisplayOrderService>();
            container.RegisterType<IDashboardIndicatorDataService, DashboardIndicatorDataService>();
            container.RegisterType<IDashboardIndicatorsService, DashboardIndicatorsService>();
            container.RegisterType<IDashboardParametersService, DashboardParametersService>();
            container.RegisterType<IDashboardRemarkService, DashboardRemarkService>();
            container.RegisterType<IDashboardService, DashboardService>();
            container.RegisterType<IDashboardTargetsService, DashboardTargetsService>();
            container.RegisterType<IDashboardTransactionCounterService, DashboardTransactionCounterService>();
            container.RegisterType<IDenialService, DenialService>();
            container.RegisterType<IDeptTimmingService, DeptTimmingService>();
            container.RegisterType<IDiagnosisCodeService, DiagnosisCodeService>();
            container.RegisterType<IDiagnosisService, DiagnosisService>();
            container.RegisterType<IDischargeSummaryDetailService, DischargeSummaryDetailService>();
            container.RegisterType<IDocumentsTemplatesService, DocumentsTemplatesService>();
            container.RegisterType<IDRGCodesService, DRGCodesService>();
            container.RegisterType<IDrugAllergyLogService, DrugAllergyLogService>();
            container.RegisterType<IDrugInstructionAndDosingService, DrugInstructionAndDosingService>();
            container.RegisterType<IDrugInteractionsService, DrugInteractionsService>();
            container.RegisterType<IDrugService, DrugService>();
            container.RegisterType<IEncounterService, EncounterService>();
            container.RegisterType<IEquipmentService, EquipmentService>();
            container.RegisterType<IErrorMasterService, ErrorMasterService>();
            container.RegisterType<IFacilityDepartmentService, FacilityDepartmentService>();
            container.RegisterType<IFacilityRoleService, FacilityRoleService>();
            container.RegisterType<IFacilityService, FacilityService>();
            container.RegisterType<IFacilityStructureService, FacilityStructureService>();
            container.RegisterType<IFacultyRoosterService, FacultyRoosterService>();
            container.RegisterType<IFacultyTimeslotsService, FacultyTimeslotsService>();
            container.RegisterType<IFavoritesService, FavoritesService>();
            container.RegisterType<IFutureOpenOrderService, FutureOpenOrderService>();
            container.RegisterType<IFutureOrderActivityService, FutureOrderActivityService>();
            container.RegisterType<IGlobalCodeCategoryMasterService, GlobalCodeCategoryMasterService>();
            container.RegisterType<IGlobalCodeCategoryService, GlobalCodeCategoryService>();
            container.RegisterType<IGlobalCodeService, GlobalCodeService>();
            container.RegisterType<IHCPCSCodesService, HCPCSCodesService>();
            container.RegisterType<IHolidayPlannerDetailsService, HolidayPlannerDetailsService>();
            container.RegisterType<IHolidayPlannerService, HolidayPlannerService>();
            container.RegisterType<IIndicatorDataCheckListService, IndicatorDataCheckListService>();
            container.RegisterType<IInsuranceCompanyService, InsuranceCompanyService>();
            container.RegisterType<IInsurancePlansService, InsurancePlansService>();
            container.RegisterType<IInsurancePolicesService, InsurancePolicesService>();
            container.RegisterType<ILabTestOrderSetService, LabTestOrderSetService>();
            container.RegisterType<ILabTestResultService, LabTestResultService>();
            container.RegisterType<ILoginTrackingService, LoginTrackingService>();
            container.RegisterType<IManagedCareService, ManagedCareService>();
            container.RegisterType<IManualChargesTrackingService, ManualChargesTrackingService>();
            container.RegisterType<IManualDashboardService, ManualDashboardService>();
            container.RegisterType<IMappingPatientBedService, MappingPatientBedService>();
            container.RegisterType<IMcContractService, McContractService>();
            container.RegisterType<IMCOrderCodeRatesService, MCOrderCodeRatesService>();
            container.RegisterType<IMCRulesTableService, MCRulesTableService>();
            container.RegisterType<IMedicalHistoryService, MedicalHistoryService>();
            container.RegisterType<IMedicalNecessityService, MedicalNecessityService>();
            container.RegisterType<IMedicalNotesService, MedicalNotesService>();
            container.RegisterType<IMedicalRecordService, MedicalRecordService>();
            container.RegisterType<IMedicalVitalService, MedicalVitalService>();
            container.RegisterType<IMissingDataService, MissingDataService>();
            container.RegisterType<IModuleAccessService, ModuleAccessService>();
            container.RegisterType<IOpenOrderActivityScheduleService, OpenOrderActivityScheduleService>();
            container.RegisterType<IOpenOrderService, OpenOrderService>();
            container.RegisterType<IOperatingRoomService, OperatingRoomService>();
            container.RegisterType<IOrderActivityService, OrderActivityService>();
            container.RegisterType<IParametersService, ParametersService>();
            container.RegisterType<IPatientAddressRelationService, PatientAddressRelationService>();
            container.RegisterType<IPatientCareActivitiesService, PatientCareActivitiesService>();
            container.RegisterType<IPatientCarePlanService, PatientCarePlanService>();
            container.RegisterType<IPatientDischargeSummaryService, PatientDischargeSummaryService>();
            container.RegisterType<IPatientEvaluationService, PatientEvaluationService>();
            container.RegisterType<IPatientInfoChangesQueueService, PatientInfoChangesQueueService>();
            container.RegisterType<IPatientInfoService, PatientInfoService>();
            container.RegisterType<IPatientInsuranceService, PatientInsuranceService>();
            container.RegisterType<IPatientLoginDetailService, PatientLoginDetailService>();
            container.RegisterType<IPatientPhoneService, PatientPhoneService>();
            container.RegisterType<IPatientPreSchedulingService, PatientPreSchedulingService>();
            container.RegisterType<IPaymentService, PaymentService>();
            container.RegisterType<IPaymentTypeDetailService, PaymentTypeDetailService>();
            container.RegisterType<IPDFTemplatesService, PDFTemplatesService>();
            container.RegisterType<IPhysicianService, PhysicianService>();
            container.RegisterType<IPlaceOfServiceService, PlaceOfServiceService>();
            container.RegisterType<IPreliminaryBillService, PreliminaryBillService>();
            container.RegisterType<IPreSchedulingLinkService, PreSchedulingLinkService>();
            container.RegisterType<IProjectDashboardService, ProjectDashboardService>();
            container.RegisterType<IProjectsService, ProjectsService>();
            container.RegisterType<IProjectTargetsService, ProjectTargetsService>();
            container.RegisterType<IProjectTasksService, ProjectTasksService>();
            container.RegisterType<IProjectTaskTargetsService, ProjectTaskTargetsService>();
            container.RegisterType<IReportingService, ReportingService>();
            container.RegisterType<IRolePermissionService, RolePermissionService>();
            container.RegisterType<IRoleService, RoleService>();
            container.RegisterType<IRoleTabsService, RoleTabsService>();
            container.RegisterType<IRuleMasterService, RuleMasterService>();
            container.RegisterType<IRuleStepService, RuleStepService>();
            container.RegisterType<ISchedulingParametersService, SchedulingParametersService>();
            container.RegisterType<ISchedulingService, SchedulingService>();
            container.RegisterType<IScreenService, ScreenService>();
            container.RegisterType<IScrubEditTrackService, ScrubEditTrackService>();
            container.RegisterType<IScrubReportService, ScrubReportService>();
            container.RegisterType<IServiceCodeService, ServiceCodeService>();
            container.RegisterType<IStateService, StateService>();
            container.RegisterType<ISystemConfigurationService, SystemConfigurationService>();
            container.RegisterType<ITabsService, TabsService>();
            container.RegisterType<ITpFileHeaderService, TpFileHeaderService>();
            container.RegisterType<ITPXMLParsedDataService, TPXMLParsedDataService>();
            container.RegisterType<IUploadChargesService, UploadChargesService>();
            container.RegisterType<IUserRoleService, UserRoleService>();
            container.RegisterType<IUsersService, UsersService>();
            container.RegisterType<IXactivityService, XactivityService>();
            container.RegisterType<IXAdviceXMLParsedDataService, XAdviceXMLParsedDataService>();
            container.RegisterType<IXclaimService, XclaimService>();
            container.RegisterType<IXFileHeaderService, XFileHeaderService>();
            container.RegisterType<IXMLBillingService, XMLBillingService>();
            container.RegisterType<IXmlReportingService, XmlReportingService>();
            container.RegisterType<IXPaymentFileXMLService, XPaymentFileXMLService>();
            container.RegisterType<IXPaymentReturnService, XPaymentReturnService>();


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