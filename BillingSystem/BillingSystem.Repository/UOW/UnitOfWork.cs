// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The unit of work.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Repository.UOW
{
    using System;

    using BillingSystem.Model;
    using BillingSystem.Repository.GenericRepository;
    using BillingSystem.Repository.Interfaces;

    /// <summary>
    ///     The unit of work.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region Fields


        /// <summary>
        ///     The _context.
        /// </summary>
        private readonly BillingEntities _context = new BillingEntities(); // create mfentities class object\

        private MedicalNecessityRepository _MedicalNecessityRepository;
        private BillingModifierRepository _BillingModifierRepository;
        private PlaceOfServiceRepository _PlaceOfServiceRepository;

        private ClinicianRosterRepository _ClinicianRosterRepository;
        private FavoriteClinicianRepository _FavoriteClinicianRepository;

        private ClinicianAppointmentTypesRepository _ClinicianAppointmentTypesRepository;


        private FutureOrderActivityRepository _FutureOrderActivityRepository;
        private FutureOpenOrderRepository _FutureOpenOrderRepository;
        private SchedulingParametersRepository _SchedulingParametersRepository;
        private PreSchedulingLinkRepository _PreSchedulingLinkRepository;
        /// <summary>
        /// The _ patient care activities repository
        /// </summary>
        private PatientCareActivitiesRepository _PatientCareActivitiesRepository;

        /// <summary>
        /// The _ patient care activities repository
        /// </summary>
        private PaymentTypeDetailRepository _PaymentTypeDetailRepository;
        /// <summary>
        ///     The _ atc codes repository.
        /// </summary>
        private ATCCodesRepository _ATCCodesRepository;

        /// <summary>
        /// The _ patient pre scheduling repository
        /// </summary>
        private PatientPreSchedulingRepository _PatientPreSchedulingRepository;


        /// <summary>
        ///     Gets the atc codes repository.
        /// </summary>
        private AppointmentTypesRepository _AppointmentTypesRepository;

        /// <summary>
        ///     The _ audit log repository.
        /// </summary>
        private AuditLogRepository _AuditLogRepository;

        /// <summary>
        ///     The _ authorization repository.
        /// </summary>
        private AuthorizationRepository _AuthorizationRepository;

        /// <summary>
        ///     The _ bed charges repository.
        /// </summary>
        private BedChargesRepository _BedChargesRepository;

        /// <summary>
        ///     The _ bed transaction repository.
        /// </summary>
        private BedTransactionRepository _BedTransactionRepository;

        /// <summary>
        ///     The _ bill activity repository.
        /// </summary>
        private BillActivityRepository _BillActivityRepository;

        /// <summary>
        ///     The _ billing code table set repository.
        /// </summary>
        private BillingCodeTableSetRepository _BillingCodeTableSetRepository;

        /// <summary>
        /// The _ care plan repository.
        /// </summary>
        private CarePlanRepository _CarePlanRepository;

        /// <summary>
        /// The _ care plan task repository.
        /// </summary>
        private CarePlanTaskRepository _CarePlanTaskRepository;

        /// <summary>
        ///     The _ corporate repository.
        /// </summary>
        private CorporateRepository _CorporateRepository;

        ///// <summary>
        /////     The _ dashboard display order repository.
        ///// </summary>
        //private DashboardDisplayOrderRepository _DashboardDisplayOrderRepository;

        ///// <summary>
        /////     The _ dashboard indicator data repository.
        ///// </summary>
        //private DashboardIndicatorDataRepository _DashboardIndicatorDataRepository;

        ///// <summary>
        /////     The _ dashboard indicators repository.
        ///// </summary>
        //private DashboardIndicatorsRepository _DashboardIndicatorsRepository;

        ///// <summary>
        /////     The _ dashboard parameters repository.
        ///// </summary>
        //private DashboardParametersRepository _DashboardParametersRepository;

        /// <summary>
        ///     The _ dashboard remark repository.
        /// </summary>
        private DashboardRemarkRepository _DashboardRemarkRepository;

        /// <summary>
        ///     The _ dept timming repository.
        /// </summary>
        private DeptTimmingRepository _DeptTimmingRepository;

        /// <summary>
        ///     The _ diagnosis code repository.
        /// </summary>
        private DiagnosisCodeRepository _DiagnosisCodeRepository;

        /// <summary>
        ///     The _ diagnosis respository.
        /// </summary>
        private DiagnosisRespository _DiagnosisRespository;

        /// <summary>
        ///     The _ discharge summary detail repository.
        /// </summary>
        private DischargeSummaryDetailRepository _DischargeSummaryDetailRepository;

        /// <summary>
        ///     The _ drug allergy log repository.
        /// </summary>
        private DrugAllergyLogRepository _DrugAllergyLogRepository;

        /// <summary>
        ///     The _ drug instruction and dosing repository.
        /// </summary>
        private DrugInstructionAndDosingRepository _DrugInstructionAndDosingRepository;

        /// <summary>
        ///     The _ drug interactions repository.
        /// </summary>
        private DrugInteractionsRepository _DrugInteractionsRepository;

        /// <summary>
        ///     The _ drug repository.
        /// </summary>
        private DrugRepository _DrugRepository;

        /// <summary>
        /// The _ equipment log respository.
        /// </summary>
        private EquipmentLogRespository _EquipmentLogRespository;

        /// <summary>
        ///     The _ error master repository.
        /// </summary>
        private ErrorMasterRepository _ErrorMasterRepository;

        /// <summary>
        ///     The _ facility department repository.
        /// </summary>
        private FacilityDepartmentRepository _FacilityDepartmentRepository;

        /// <summary>
        ///     The _ facility role repository.
        /// </summary>
        private FacilityRoleRepository _FacilityRoleRepository;

        /// <summary>
        /// The _ faculty rooster repository.
        /// </summary>
        private FacultyRoosterRepository _FacultyRoosterRepository;

        /// <summary>
        ///     The _ faculty timeslots repository.
        /// </summary>
        private FacultyTimeslotsRepository _FacultyTimeslotsRepository;

        /// <summary>
        ///     The _ holiday planner details repository.
        /// </summary>
        private HolidayPlannerDetailsRepository _HolidayPlannerDetailsRepository;

        /// <summary>
        ///     The _ holiday planner repository.
        /// </summary>
        private HolidayPlannerRepository _HolidayPlannerRepository;

        ///// <summary>
        /////     The _ indicator data check list repository.
        ///// </summary>
        //private IndicatorDataCheckListRepository _IndicatorDataCheckListRepository;

        /// <summary>
        ///     The _ lab test order set repository.
        /// </summary>
        private LabTestOrderSetRepository _LabTestOrderSetRepository;

        /// <summary>
        ///     EquipmentLogRespository
        ///     The _ lab test result repository.
        /// </summary>
        private LabTestResultRepository _LabTestResultRepository;

        /// <summary>
        ///     The _ mc order code rates repository.
        /// </summary>
        private MCOrderCodeRatesRepository _MCOrderCodeRatesRepository;

        /// <summary>
        ///     The _ mc rules table repository.
        /// </summary>
        private MCRulesTableRepository _MCRulesTableRepository;

        /// <summary>
        ///     The _ manual charges tracking repository.
        /// </summary>
        private ManualChargesTrackingRepository _ManualChargesTrackingRepository;

        ///// <summary>
        /////     The _ manual dashboard repository.
        ///// </summary>
        //private ManualDashboardRepository _ManualDashboardRepository;

        /// <summary>
        ///     The _ max values repository.
        /// </summary>
        private MaxValuesRepository _MaxValuesRepository;

        /// <summary>
        ///     The _ medical history repository.
        /// </summary>
        private MedicalHistoryRepository _MedicalHistoryRepository;

        /// <summary>
        ///     The _ medical notes repository.
        /// </summary>
        private MedicalNotesRepository _MedicalNotesRepository;

        /// <summary>
        ///     The _ medical record repository.
        /// </summary>
        private MedicalRecordRepository _MedicalRecordRepository;

        /// <summary>
        ///     The _ medical vital repository.
        /// </summary>
        private MedicalVitalRepository _MedicalVitalRepository;

        /// <summary>
        ///     The _ module access repository.
        /// </summary>
        private ModuleAccessRepository _ModuleAccessRepository;

        /// <summary>
        ///     The _ order activity repository.
        /// </summary>
        private OrderActivityRepository _OrderActivityRepository;

        /// <summary>
        ///     The _ parameters repository.
        /// </summary>
        private ParametersRepository _ParametersRepository;

        /// <summary>
        ///     The _ patient discharge summary repository.
        /// </summary>
        private PatientDischargeSummaryRepository _PatientDischargeSummaryRepository;

        /// <summary>
        ///     The _ patient evaluation repository.
        /// </summary>
        private PatientEvaluationRepository _PatientEvaluationRepository;

        /// <summary>
        ///     The _ patient info changes queue repository.
        /// </summary>
        private PatientInfoChangesQueueRepository _PatientInfoChangesQueueRepository;

        /// <summary>
        ///     The _ payment repository.
        /// </summary>
        private PaymentRepository _PaymentRepository;

        /// <summary>
        ///     The _ project dashboard repository.
        /// </summary>
        private ProjectDashboardRepository _ProjectDashboardRepository;

        /// <summary>
        ///     The _ project targets repository.
        /// </summary>
        private ProjectTargetsRepository _ProjectTargetsRepository;

        /// <summary>
        ///     The _ project task targets repository.
        /// </summary>
        private ProjectTaskTargetsRepository _ProjectTaskTargetsRepository;

        /// <summary>
        ///     The _ project tasks repository.
        /// </summary>
        private ProjectTasksRepository _ProjectTasksRepository;

        /// <summary>
        ///     The _ projects repository.
        /// </summary>
        private ProjectsRepository _ProjectsRepository;

        /// <summary>
        ///     The _ role tabs repository.
        /// </summary>
        private RoleTabsRepository _RoleTabsRepository;

        // private ManagedCareRepository _ManagedCareRepository;
        /// <summary>
        ///     The _ rule master repository.
        /// </summary>
        private RuleMasterRepository _RuleMasterRepository;

        /// <summary>
        ///     The _ rule step repository.
        /// </summary>
        private RuleStepRepository _RuleStepRepository;

        /// <summary>
        ///     The _ scrub edit track repository.
        /// </summary>
        private ScrubEditTrackRepository _ScrubEditTrackRepository;

        /// <summary>
        ///     The _ scrub header repository.
        /// </summary>
        private ScrubHeaderRepository _ScrubHeaderRepository;

        /// <summary>
        ///     The _ scrub report repository.
        /// </summary>
        private ScrubReportRepository _ScrubReportRepository;

        /// <summary>
        ///     The _ tp file header repository.
        /// </summary>
        private TPFileHeaderRepository _TPFileHeaderRepository;

        /// <summary>
        ///     The _ tp file xml repository.
        /// </summary>
        private TPFileXMLRepository _TPFileXMLRepository;

        /// <summary>
        ///     The _ tpxml parsed data repository.
        /// </summary>
        private TPXMLParsedDataRepository _TPXMLParsedDataRepository;

        /// <summary>
        ///     The _ tabs repository.
        /// </summary>
        private TabsRepository _TabsRepository;

        /// <summary>
        ///     The _ x advice xml parsed data repository.
        /// </summary>
        private XAdviceXMLParsedDataRepository _XAdviceXMLParsedDataRepository;

        /// <summary>
        ///     The _ x file header repository.
        /// </summary>
        private XFileHeaderRepository _XFileHeaderRepository;

        /// <summary>
        ///     The _ x file xml repository.
        /// </summary>
        private XFileXMLRepository _XFileXMLRepository;

        /// <summary>
        ///     The _ x payment file xml repository.
        /// </summary>
        private XPaymentFileXMLRepository _XPaymentFileXMLRepository;

        /// <summary>
        ///     The _ x payment return repository.
        /// </summary>
        private XPaymentReturnRepository _XPaymentReturnRepository;

        /// <summary>
        ///     The _ xactivity repository.
        /// </summary>
        private XactivityRepository _XactivityRepository;

        /// <summary>
        ///     The _ xclaim repository.
        /// </summary>
        private XclaimRepository _XclaimRepository;


        private PDFTemplatesRepository _PDFTemplatesRepository;
        /// <summary>
        ///     The _bed master repository.
        /// </summary>
        private BedMasterRepository _bedMasterRepository; // Shashank 10102014

        /// <summary>
        ///     The _bed rate card repository.
        /// </summary>
        private BedRateCardRepository _bedRateCardRepository;

        /// <summary>
        ///     The _bill header repository.
        /// </summary>
        private BillHeaderRepository _billHeaderRepository;

        /// <summary>
        ///     The _billing system parameters repository.
        /// </summary>
        private BillingSystemParametersRepository _billingSystemParametersRepository;

        /// <summary>
        ///     The _city repository.
        /// </summary>
        private CityRepository _cityRepository;

        /// <summary>
        ///     The _country repository.
        /// </summary>
        private CountryRepository _countryRepository;

        /// <summary>
        ///     The _cpt codes repository.
        /// </summary>
        private CPTCodesRepository _cptCodesRepository;

        ///// <summary>
        /////     The _dashboard budget repository.
        ///// </summary>
        //private DashboardBudgetRepository _dashboardBudgetRepository;

        /// <summary>
        ///     The _dashboard targets repository.
        /// </summary>
        private DashboardTargetsRepository _dashboardTargetsRepository;

        /// <summary>
        ///     The _dashboard transaction counter repository.
        /// </summary>
        private DashboardTransactionCounterRepository _dashboardTransactionCounterRepository;

        /// <summary>
        ///     The _denial repository.
        /// </summary>
        private DenialRepository _denialRepository;

        /// <summary>
        ///     The _disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     The _documents templates repository.
        /// </summary>
        private DocumentsTemplatesRepository _documentsTemplatesRepository;

        /// <summary>
        ///     The _drg codes repository.
        /// </summary>
        private DRGCodesRepository _drgCodesRepository;

        /// <summary>
        ///     The _encounter repository.
        /// </summary>
        private EncounterRepository _encounterRepository;

        /// <summary>
        ///     The _equipment repository.
        /// </summary>
        private EquipmentRepository _equipmentRepository;

        /// <summary>
        ///     The _facility repository.
        /// </summary>
        private FacilityRepository _facilityRepository;

        /// <summary>
        ///     The _facility structure repository.
        /// </summary>
        private FacilityStructureRepository _facilityStructureRepository;

        /// <summary>
        ///     The _favorites repository.
        /// </summary>
        private FavoritesRepository _favoritesRepository;

        /// <summary>
        ///     The _g code category repository.
        /// </summary>
        private GlobalCodeCategoryRepository _gCodeCategoryRepository;

        /// <summary>
        ///     The _global code repository.
        /// </summary>
        private GlobalCodeRepository _globalCodeRepository;

        /// <summary>
        ///     The _hcpcs codes repository.
        /// </summary>
        private HCPCSCodesRepository _hcpcsCodesRepository;

        /// <summary>
        ///     The _insurance company repository.
        /// </summary>
        private InsuranceCompanyRepository _insuranceCompanyRepository;

        /// <summary>
        ///     The _insurance plans repository.
        /// </summary>
        private InsurancePlansRepository _insurancePlansRepository;

        /// <summary>
        ///     The _insurance polices repository.
        /// </summary>
        private InsurancePolicesRepository _insurancePolicesRepository;

        /// <summary>
        ///     The _journal entry support repository.
        /// </summary>
        private JournalEntrySupportRepository _journalEntrySupportRepository;

        /// <summary>
        ///     The _login tracking repository.
        /// </summary>
        private LoginTrackingRepository _loginTrackingRepository;

        /// <summary>
        ///     The _managed care repository.
        /// </summary>
        private ManagedCareRepository _managedCareRepository;

        /// <summary>
        ///     The _mapping patient bed repository.
        /// </summary>
        private MappingPatientBedRepository _mappingPatientBedRepository; // Shashank 13102014

        /// <summary>
        ///     The _mc contract repository.
        /// </summary>
        private MCContractRepository _mcContractRepository;

        /// <summary>
        ///     The _open order activity schedule repository.
        /// </summary>
        private OpenOrderActivityScheduleRepository _openOrderActivityScheduleRepository;

        /// <summary>
        ///     The _open order repository.
        /// </summary>
        private OpenOrderRepository _openOrderRepository; // Shashank Awasthy 10092014

        /// <summary>
        ///     The _operating room repository.
        /// </summary>
        private OperatingRoomRepository _operatingRoomRepository;

        /// <summary>
        ///     The _patient address relation repository.
        /// </summary>
        private PatientAddressRelationRepository _patientAddressRelationRepository; // Shashank Awasthy10092014

        /// <summary>
        ///     The _patient info repository.
        /// </summary>
        private PatientInfoRepository _patientInfoRepository;

        /// <summary>
        ///     The _patient insurance repository.
        /// </summary>
        private PatientInsuranceRepository _patientInsuranceRepository;

        /// <summary>
        ///     The _patient login detail repository.
        /// </summary>
        private PatientLoginDetailRepository _patientLoginDetailRepository;

        /// <summary>
        ///     The _patient phone repository.
        /// </summary>
        private PatientPhoneRepository _patientPhoneRepository;

        /// <summary>
        ///     The _physician repository.
        /// </summary>
        private PhysicianRepository _physicianRepository;

        /// <summary>
        ///     The _role permission repository.
        /// </summary>
        private RolePermissionRepository _rolePermissionRepository;

        /// <summary>
        ///     The _role repository.
        /// </summary>
        private RoleRepository _roleRepository;

        /// <summary>
        ///     The _scheduling repository.
        /// </summary>
        private SchedulingRepository _schedulingRepository;

        /// <summary>
        ///     The _screen repository.
        /// </summary>
        private ScreenRepository _screenRepository;

        /// <summary>
        ///     The _servicecode repository.
        /// </summary>
        private ServiceCodeRepository _servicecodeRepository;

        /// <summary>
        ///     The _state repository.
        /// </summary>
        private StateRepository _stateRepository;

        /// <summary>
        ///     The _system configuration repository.
        /// </summary>
        private SystemConfigurationRepository _systemConfigurationRepository;

        /// <summary>
        ///     The _user role repository.
        /// </summary>
        private UserRoleRepository _userRoleRepository; // Ashwani

        /// <summary>
        ///     The _users repository.
        /// </summary>
        private UsersRepository _usersRepository;

        private PatientEvaluationSetRepository _patientEvaluationSetRepository;

        #endregion


        #region Public Properties
        public BillingModifierRepository BillingModifierRepository
        {

            get
            {
                if (_BillingModifierRepository == null)
                    _BillingModifierRepository = new BillingModifierRepository(_context);
                return _BillingModifierRepository;
            }
        }
        public PlaceOfServiceRepository PlaceOfServiceRepository
        {
            get
            {
                if (_PlaceOfServiceRepository == null)
                    _PlaceOfServiceRepository = new PlaceOfServiceRepository(_context);
                return _PlaceOfServiceRepository;
            }
        }
        

        public FavoriteClinicianRepository FavoriteClinicianRepository
        {
            get
            {
                if (_FavoriteClinicianRepository == null)
                    _FavoriteClinicianRepository = new FavoriteClinicianRepository(_context);
                return _FavoriteClinicianRepository;
            }
        }


        public ClinicianRosterRepository ClinicianRosterRepository
        {
            get
            {
                if (_ClinicianRosterRepository == null)
                    _ClinicianRosterRepository = new ClinicianRosterRepository(_context);
                return _ClinicianRosterRepository;
            }
        }

        public ClinicianAppointmentTypesRepository ClinicianAppointmentTypesRepository
        {
            get
            {
                if (_ClinicianAppointmentTypesRepository == null)
                    _ClinicianAppointmentTypesRepository = new ClinicianAppointmentTypesRepository(_context);
                return _ClinicianAppointmentTypesRepository;
            }
        }

        public SchedulingParametersRepository SchedulingParametersRepository
        {
            get
            {
                if (_SchedulingParametersRepository == null)
                    _SchedulingParametersRepository = new SchedulingParametersRepository(_context);
                return _SchedulingParametersRepository;
            }
        }

        public PreSchedulingLinkRepository PreSchedulingLinkRepository
        {
            get
            {
                if (_PreSchedulingLinkRepository == null)
                    _PreSchedulingLinkRepository = new PreSchedulingLinkRepository(_context);
                return _PreSchedulingLinkRepository;
            }
        }

        public FutureOpenOrderRepository FutureOpenOrderRepository
        {
            get
            {
                if (_FutureOpenOrderRepository == null)
                    _FutureOpenOrderRepository = new FutureOpenOrderRepository(_context);
                return _FutureOpenOrderRepository;
            }
        }

        public FutureOrderActivityRepository FutureOrderActivityRepository
        {
            get
            {
                if (_FutureOrderActivityRepository == null)
                    _FutureOrderActivityRepository = new FutureOrderActivityRepository(_context);
                return _FutureOrderActivityRepository;
            }

        }

        public PatientEvaluationSetRepository PatientEvaluationSetRepository
        {
            get
            {
                if (_patientEvaluationSetRepository == null)
                    _patientEvaluationSetRepository = new PatientEvaluationSetRepository(_context);
                return _patientEvaluationSetRepository;
            }
        }



        /// <summary>
        /// Gets the patient care activities repository.
        /// </summary>
        /// <value>
        /// The patient care activities repository.
        /// </value>
        public PaymentTypeDetailRepository PaymentTypeDetailRepository
        {
            get
            {
                if (_PaymentTypeDetailRepository == null)
                    _PaymentTypeDetailRepository = new PaymentTypeDetailRepository(_context);
                return _PaymentTypeDetailRepository;
            }
        }


        /// <summary>
        /// Gets the patient care activities repository.
        /// </summary>
        /// <value>
        /// The patient care activities repository.
        /// </value>
        public PatientCareActivitiesRepository PatientCareActivitiesRepository
        {
            get
            {
                if (_PatientCareActivitiesRepository == null)
                    _PatientCareActivitiesRepository = new PatientCareActivitiesRepository(_context);
                return _PatientCareActivitiesRepository;
            }
        }

        /// <summary>
        /// Gets the patient pre scheduling repository.
        /// </summary>
        /// <value>
        /// The patient pre scheduling repository.
        /// </value>
        public PatientPreSchedulingRepository PatientPreSchedulingRepository
        {
            get
            {
                if (_PatientPreSchedulingRepository == null)
                    _PatientPreSchedulingRepository = new PatientPreSchedulingRepository(_context);
                return _PatientPreSchedulingRepository;
            }
        }

        /// <summary>
        /// Gets the atc codes repository.
        /// </summary>
        /// <value>
        /// The atc codes repository.
        /// </value>
        public ATCCodesRepository ATCCodesRepository
        {
            get
            {
                if (_ATCCodesRepository == null)
                {
                    _ATCCodesRepository = new ATCCodesRepository(_context);
                }

                return _ATCCodesRepository;
            }
        }

        /// <summary>
        /// The _ patient care plan repository
        /// </summary>
        private PatientCarePlanRepository _PatientCarePlanRepository;

        /// <summary>
        /// Gets the patient care plan repository.
        /// </summary>
        /// <value>
        /// The patient care plan repository.
        /// </value>
        public PatientCarePlanRepository PatientCarePlanRepository
        {
            get
            {
                if (_PatientCarePlanRepository == null)
                    _PatientCarePlanRepository = new PatientCarePlanRepository(_context);
                return _PatientCarePlanRepository;
            }
        }

        /// <summary>
        /// Gets the appointment types repository.
        /// </summary>
        public AppointmentTypesRepository AppointmentTypesRepository
        {
            get
            {
                return _AppointmentTypesRepository
                       ?? (_AppointmentTypesRepository = new AppointmentTypesRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the audit log repository.
        /// </summary>
        public AuditLogRepository AuditLogRepository
        {
            get
            {
                return _AuditLogRepository ?? (_AuditLogRepository = new AuditLogRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the authorization repository.
        /// </summary>
        public AuthorizationRepository AuthorizationRepository
        {
            get
            {
                if (_AuthorizationRepository == null)
                {
                    _AuthorizationRepository = new AuthorizationRepository(_context);
                }

                return _AuthorizationRepository;
            }
        }

        /// <summary>
        ///     Gets the bed charges repository.
        /// </summary>
        public BedChargesRepository BedChargesRepository
        {
            get
            {
                if (_BedChargesRepository == null)
                {
                    _BedChargesRepository = new BedChargesRepository(_context);
                }

                return _BedChargesRepository;
            }
        }

        /// <summary>
        ///     Gets the bed master repository.
        /// </summary>
        public BedMasterRepository BedMasterRepository
        {
            get
            {
                if (_bedMasterRepository == null)
                {
                    _bedMasterRepository = new BedMasterRepository(_context);
                }

                return _bedMasterRepository;
            }
        }

        /// <summary>
        ///     Method is used to return PatientInfoRepository Repository object
        /// </summary>
        public BedRateCardRepository BedRateCardRepository
        {
            get
            {
                if (_bedRateCardRepository == null)
                {
                    _bedRateCardRepository = new BedRateCardRepository(_context);
                }

                return _bedRateCardRepository;
            }
        }

        /// <summary>
        ///     Gets the bed transaction repository.
        /// </summary>
        public BedTransactionRepository BedTransactionRepository
        {
            get
            {
                if (_BedTransactionRepository == null)
                {
                    _BedTransactionRepository = new BedTransactionRepository(_context);
                }

                return _BedTransactionRepository;
            }
        }

        /// <summary>
        ///     Gets the bill activity repository.
        /// </summary>
        public BillActivityRepository BillActivityRepository
        {
            get
            {
                if (_BillActivityRepository == null)
                {
                    _BillActivityRepository = new BillActivityRepository(_context);
                }

                return _BillActivityRepository;
            }
        }

        /// <summary>
        ///     Gets the bill header repository.
        /// </summary>
        public BillHeaderRepository BillHeaderRepository
        {
            get
            {
                if (_billHeaderRepository == null)
                {
                    _billHeaderRepository = new BillHeaderRepository(_context);
                }

                return _billHeaderRepository;
            }
        }

        /// <summary>
        ///     Gets the billing code table set repository.
        /// </summary>
        public BillingCodeTableSetRepository BillingCodeTableSetRepository
        {
            get
            {
                return _BillingCodeTableSetRepository
                       ?? (_BillingCodeTableSetRepository = new BillingCodeTableSetRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the billing system parameters repository.
        /// </summary>
        public BillingSystemParametersRepository BillingSystemParametersRepository
        {
            get
            {
                return _billingSystemParametersRepository
                       ?? (_billingSystemParametersRepository =
                           new BillingSystemParametersRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the cpt codes repository.
        /// </summary>
        public CPTCodesRepository CPTCodesRepository
        {
            get
            {
                if (_cptCodesRepository == null)
                {
                    _cptCodesRepository = new CPTCodesRepository(_context);
                }

                return _cptCodesRepository;
            }
        }

        /// <summary>
        /// Gets the care plan repository.
        /// </summary>
        public CarePlanRepository CarePlanRepository
        {
            get
            {
                if (_CarePlanRepository == null)
                {
                    _CarePlanRepository = new CarePlanRepository(_context);
                }

                return _CarePlanRepository;
            }
        }

        /// <summary>
        /// Gets the care plan task repository.
        /// </summary>
        public CarePlanTaskRepository CarePlanTaskRepository
        {
            get
            {
                if (_CarePlanTaskRepository == null)
                {
                    _CarePlanTaskRepository = new CarePlanTaskRepository(_context);
                }

                return _CarePlanTaskRepository;
            }
        }

        /// <summary>
        ///     Gets the city repository.
        /// </summary>
        public CityRepository CityRepository
        {
            get
            {
                if (_cityRepository == null)
                {
                    _cityRepository = new CityRepository(_context);
                }

                return _cityRepository;
            }
        }

        /// <summary>
        ///     Gets the corporate repository.
        /// </summary>
        public CorporateRepository CorporateRepository
        {
            get
            {
                if (_CorporateRepository == null)
                {
                    _CorporateRepository = new CorporateRepository(_context);
                }

                return _CorporateRepository;
            }
        }

        /// <summary>
        ///     Method is used to return Facility Repository object
        /// </summary>
        public CountryRepository CountryRepository
        {
            get
            {
                if (_countryRepository == null)
                {
                    _countryRepository = new CountryRepository(_context);
                }

                return _countryRepository;
            }
        }

        /// <summary>
        ///     Gets the drg codes repository.
        /// </summary>
        public DRGCodesRepository DRGCodesRepository
        {
            get
            {
                if (_drgCodesRepository == null)
                {
                    _drgCodesRepository = new DRGCodesRepository(_context);
                }

                return _drgCodesRepository;
            }
        }

        ///// <summary>
        /////     Gets the dashboard budget repository.
        ///// </summary>
        //public DashboardBudgetRepository DashboardBudgetRepository
        //{
        //    get
        //    {
        //        if (_dashboardBudgetRepository == null)
        //        {
        //            _dashboardBudgetRepository = new DashboardBudgetRepository(_context);
        //        }

        //        return _dashboardBudgetRepository;
        //    }
        //}

        ///// <summary>
        /////     Gets the dashboard display order repository.
        ///// </summary>
        //public DashboardDisplayOrderRepository DashboardDisplayOrderRepository
        //{
        //    get
        //    {
        //        return _DashboardDisplayOrderRepository
        //               ?? (_DashboardDisplayOrderRepository = new DashboardDisplayOrderRepository(_context));
        //    }
        //}

        ///// <summary>
        /////     Gets the dashboard indicator data repository.
        ///// </summary>
        //public DashboardIndicatorDataRepository DashboardIndicatorDataRepository
        //{
        //    get
        //    {
        //        if (_DashboardIndicatorDataRepository == null)
        //        {
        //            _DashboardIndicatorDataRepository = new DashboardIndicatorDataRepository(_context);
        //        }

        //        return _DashboardIndicatorDataRepository;
        //    }
        //}

        ///// <summary>
        /////     Gets the dashboard indicators repository.
        ///// </summary>
        //public DashboardIndicatorsRepository DashboardIndicatorsRepository
        //{
        //    get
        //    {
        //        if (_DashboardIndicatorsRepository == null)
        //        {
        //            _DashboardIndicatorsRepository = new DashboardIndicatorsRepository(_context);
        //        }

        //        return _DashboardIndicatorsRepository;
        //    }
        //}

        ///// <summary>
        /////     Gets the dashboard parameters repository.
        ///// </summary>
        //public DashboardParametersRepository DashboardParametersRepository
        //{
        //    get
        //    {
        //        if (_DashboardParametersRepository == null)
        //        {
        //            _DashboardParametersRepository = new DashboardParametersRepository(_context);
        //        }

        //        return _DashboardParametersRepository;
        //    }
        //}

        /// <summary>
        ///     Gets the dashboard remark repository.
        /// </summary>
        public DashboardRemarkRepository DashboardRemarkRepository
        {
            get
            {
                if (_DashboardRemarkRepository == null)
                {
                    _DashboardRemarkRepository = new DashboardRemarkRepository(_context);
                }

                return _DashboardRemarkRepository;
            }
        }

        /// <summary>
        ///     Gets the dashboard targets repository.
        /// </summary>
        public DashboardTargetsRepository DashboardTargetsRepository
        {
            get
            {
                return _dashboardTargetsRepository
                       ?? (_dashboardTargetsRepository = new DashboardTargetsRepository(_context));
            }
        }

        /// <summary>
        /// Gets the medical necessity repository.
        /// </summary>
        /// <value>
        /// The medical necessity repository.
        /// </value>
        public MedicalNecessityRepository MedicalNecessityRepository
        {
            get
            {
                return _MedicalNecessityRepository
                       ?? (_MedicalNecessityRepository = new MedicalNecessityRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the dashboard transaction counter repository.
        /// </summary>
        public DashboardTransactionCounterRepository DashboardTransactionCounterRepository
        {
            get
            {
                if (_dashboardTransactionCounterRepository == null)
                {
                    _dashboardTransactionCounterRepository =
                        new DashboardTransactionCounterRepository(_context);
                }

                return _dashboardTransactionCounterRepository;
            }
        }

        /// <summary>
        ///     Gets the denial repository.
        /// </summary>
        public DenialRepository DenialRepository
        {
            get
            {
                if (_denialRepository == null)
                {
                    _denialRepository = new DenialRepository(_context);
                }

                return _denialRepository;
            }
        }

        /// <summary>
        ///     Gets the dept timming repository.
        /// </summary>
        public DeptTimmingRepository DeptTimmingRepository
        {
            get
            {
                if (_DeptTimmingRepository == null)
                {
                    _DeptTimmingRepository = new DeptTimmingRepository(_context);
                }

                return _DeptTimmingRepository;
            }
        }

        /// <summary>
        ///     Gets the diagnosis code repository.
        /// </summary>
        public DiagnosisCodeRepository DiagnosisCodeRepository
        {
            get
            {
                if (_DiagnosisCodeRepository == null)
                {
                    _DiagnosisCodeRepository = new DiagnosisCodeRepository(_context);
                }

                return _DiagnosisCodeRepository;
            }
        }

        /// <summary>
        ///     Gets the diagnosis respository.
        /// </summary>
        public DiagnosisRespository DiagnosisRespository
        {
            get
            {
                if (_DiagnosisRespository == null)
                {
                    _DiagnosisRespository = new DiagnosisRespository(_context);
                }

                return _DiagnosisRespository;
            }
        }

        /// <summary>
        ///     Gets the discharge summary detail repository.
        /// </summary>
        public DischargeSummaryDetailRepository DischargeSummaryDetailRepository
        {
            get
            {
                if (_DischargeSummaryDetailRepository == null)
                {
                    _DischargeSummaryDetailRepository = new DischargeSummaryDetailRepository(_context);
                }

                return _DischargeSummaryDetailRepository;
            }
        }

        /// <summary>
        ///     Gets the documents templates repository.
        /// </summary>
        public DocumentsTemplatesRepository DocumentsTemplatesRepository
        {
            get
            {
                if (_documentsTemplatesRepository == null)
                {
                    _documentsTemplatesRepository = new DocumentsTemplatesRepository(_context);
                }

                return _documentsTemplatesRepository;
            }
        }

        /// <summary>
        ///     Gets the drug allergy log repository.
        /// </summary>
        public DrugAllergyLogRepository DrugAllergyLogRepository
        {
            get
            {
                if (_DrugAllergyLogRepository == null)
                {
                    _DrugAllergyLogRepository = new DrugAllergyLogRepository(_context);
                }

                return _DrugAllergyLogRepository;
            }
        }

        /// <summary>
        ///     Gets the drug instruction and dosing repository.
        /// </summary>
        public DrugInstructionAndDosingRepository DrugInstructionAndDosingRepository
        {
            get
            {
                if (_DrugInstructionAndDosingRepository == null)
                {
                    _DrugInstructionAndDosingRepository = new DrugInstructionAndDosingRepository(_context);
                }

                return _DrugInstructionAndDosingRepository;
            }
        }

        /// <summary>
        ///     Gets the drug interactions repository.
        /// </summary>
        public DrugInteractionsRepository DrugInteractionsRepository
        {
            get
            {
                if (_DrugInteractionsRepository == null)
                {
                    _DrugInteractionsRepository = new DrugInteractionsRepository(_context);
                }

                return _DrugInteractionsRepository;
            }
        }

        /// <summary>
        ///     Gets the drug repository.
        /// </summary>
        public DrugRepository DrugRepository
        {
            get
            {
                if (_DrugRepository == null)
                {
                    _DrugRepository = new DrugRepository(_context);
                }

                return _DrugRepository;
            }
        }

        /// <summary>
        ///     Gets the encounter repository.
        /// </summary>
        public EncounterRepository EncounterRepository
        {
            get
            {
                if (_encounterRepository == null)
                {
                    _encounterRepository = new EncounterRepository(_context);
                }

                return _encounterRepository;
            }
        }

        /// <summary>
        /// Gets the equipment log respository.
        /// </summary>
        public EquipmentLogRespository EquipmentLogRespository
        {
            get
            {
                if (_EquipmentLogRespository == null)
                {
                    _EquipmentLogRespository = new EquipmentLogRespository(_context);
                }

                return _EquipmentLogRespository;
            }
        }

        /// <summary>
        ///     Gets the equipment repository.
        /// </summary>
        public EquipmentRepository EquipmentRepository
        {
            get
            {
                if (_equipmentRepository == null)
                {
                    _equipmentRepository = new EquipmentRepository(_context);
                }

                return _equipmentRepository;
            }
        }

        /// <summary>
        ///     Gets the error master repository.
        /// </summary>
        public ErrorMasterRepository ErrorMasterRepository
        {
            get
            {
                if (_ErrorMasterRepository == null)
                {
                    _ErrorMasterRepository = new ErrorMasterRepository(_context);
                }

                return _ErrorMasterRepository;
            }
        }

        /// <summary>
        ///     Gets the facility department repository.
        /// </summary>
        public FacilityDepartmentRepository FacilityDepartmentRepository
        {
            get
            {
                if (_FacilityDepartmentRepository == null)
                {
                    _FacilityDepartmentRepository = new FacilityDepartmentRepository(_context);
                }

                return _FacilityDepartmentRepository;
            }
        }

        /// <summary>
        ///     Method is used to return Facility Repository object
        /// </summary>
        public FacilityRepository FacilityRepository
        {
            get
            {
                if (_facilityRepository == null)
                {
                    _facilityRepository = new FacilityRepository(_context);
                }

                return _facilityRepository;
            }
        }

        /// <summary>
        ///     Gets the facility role repository.
        /// </summary>
        public FacilityRoleRepository FacilityRoleRepository
        {
            get
            {
                if (_FacilityRoleRepository == null)
                {
                    _FacilityRoleRepository = new FacilityRoleRepository(_context);
                }

                return _FacilityRoleRepository;
            }
        }

        /// <summary>
        ///     Gets the facility structure repository.
        /// </summary>
        public FacilityStructureRepository FacilityStructureRepository
        {
            get
            {
                if (_facilityStructureRepository == null)
                {
                    _facilityStructureRepository = new FacilityStructureRepository(_context);
                }

                return _facilityStructureRepository;
            }
        }

        /// <summary>
        /// Gets the faculty rooster repository.
        /// </summary>
        public FacultyRoosterRepository FacultyRoosterRepository
        {
            get
            {
                if (_FacultyRoosterRepository == null)
                {
                    _FacultyRoosterRepository = new FacultyRoosterRepository(_context);
                }

                return _FacultyRoosterRepository;
            }
        }

        /// <summary>
        ///     Gets the faculty timeslots repository.
        /// </summary>
        public FacultyTimeslotsRepository FacultyTimeslotsRepository
        {
            get
            {
                if (_FacultyTimeslotsRepository == null)
                {
                    _FacultyTimeslotsRepository = new FacultyTimeslotsRepository(_context);
                }

                return _FacultyTimeslotsRepository;
            }
        }

        /// <summary>
        ///     Gets the favorites repository.
        /// </summary>
        public FavoritesRepository FavoritesRepository
        {
            get
            {
                if (_favoritesRepository == null)
                {
                    _favoritesRepository = new FavoritesRepository(_context);
                }

                return _favoritesRepository;
            }
        }

        /// <summary>
        ///     Method is used to return GlobalCodeCategory Repository object
        /// </summary>
        public GlobalCodeCategoryRepository GlobalCodeCategoryRepository
        {
            get
            {
                if (_gCodeCategoryRepository == null)
                {
                    _gCodeCategoryRepository = new GlobalCodeCategoryRepository(_context);
                }

                return _gCodeCategoryRepository;
            }
        }

        /// <summary>
        ///     Method is used to return GlobalCodeCategory Repository object
        /// </summary>
        public GlobalCodeRepository GlobalCodeRepository
        {
            get
            {
                if (_globalCodeRepository == null)
                {
                    _globalCodeRepository = new GlobalCodeRepository(_context);
                }

                return _globalCodeRepository;
            }
        }

        /// <summary>
        ///     Gets the hcpcs codes repository.
        /// </summary>
        public HCPCSCodesRepository HCPCSCodesRepository
        {
            get
            {
                if (_hcpcsCodesRepository == null)
                {
                    _hcpcsCodesRepository = new HCPCSCodesRepository(_context);
                }

                return _hcpcsCodesRepository;
            }
        }

        /// <summary>
        ///     Gets the holiday planner details repository.
        /// </summary>
        public HolidayPlannerDetailsRepository HolidayPlannerDetailsRepository
        {
            get
            {
                if (_HolidayPlannerDetailsRepository == null)
                {
                    _HolidayPlannerDetailsRepository = new HolidayPlannerDetailsRepository(_context);
                }

                return _HolidayPlannerDetailsRepository;
            }
        }

        /// <summary>
        ///     Gets the holiday planner repository.
        /// </summary>
        public HolidayPlannerRepository HolidayPlannerRepository
        {
            get
            {
                if (_HolidayPlannerRepository == null)
                {
                    _HolidayPlannerRepository = new HolidayPlannerRepository(_context);
                }

                return _HolidayPlannerRepository;
            }
        }

        ///// <summary>
        /////     Gets the indicator data check list repository.
        ///// </summary>
        //public IndicatorDataCheckListRepository IndicatorDataCheckListRepository
        //{
        //    get
        //    {
        //        if (_IndicatorDataCheckListRepository == null)
        //        {
        //            _IndicatorDataCheckListRepository = new IndicatorDataCheckListRepository(_context);
        //        }

        //        return _IndicatorDataCheckListRepository;
        //    }
        //}

        /// <summary>
        ///     Method is used to return Facility Repository object
        /// </summary>
        public InsuranceCompanyRepository InsuranceCompanyRepository
        {
            get
            {
                if (_insuranceCompanyRepository == null)
                {
                    _insuranceCompanyRepository = new InsuranceCompanyRepository(_context);
                }

                return _insuranceCompanyRepository;
            }
        }

        /// <summary>
        ///     Method is used to return systemConfiguration Repository object
        /// </summary>
        public InsurancePlansRepository InsurancePlansRepository
        {
            get
            {
                if (_insurancePlansRepository == null)
                {
                    _insurancePlansRepository = new InsurancePlansRepository(_context);
                }

                return _insurancePlansRepository;
            }
        }

        /// <summary>
        ///     Method is used to return GlobalCodeCategory Repository object
        /// </summary>
        public InsurancePolicesRepository InsurancePolicesRepository
        {
            get
            {
                if (_insurancePolicesRepository == null)
                {
                    _insurancePolicesRepository = new InsurancePolicesRepository(_context);
                }

                return _insurancePolicesRepository;
            }
        }

        /// <summary>
        ///     Gets the journal entry support repository.
        /// </summary>
        public JournalEntrySupportRepository JournalEntrySupportRepository
        {
            get
            {
                if (_journalEntrySupportRepository == null)
                {
                    _journalEntrySupportRepository = new JournalEntrySupportRepository(_context);
                }

                return _journalEntrySupportRepository;
            }
        }

        /// <summary>
        ///     Gets the lab test order set repository.
        /// </summary>
        public LabTestOrderSetRepository LabTestOrderSetRepository
        {
            get
            {
                if (_LabTestOrderSetRepository == null)
                {
                    _LabTestOrderSetRepository = new LabTestOrderSetRepository(_context);
                }

                return _LabTestOrderSetRepository;
            }
        }

        /// <summary>
        ///     Gets the lab test result repository.
        /// </summary>
        public LabTestResultRepository LabTestResultRepository
        {
            get
            {
                if (_LabTestResultRepository == null)
                {
                    _LabTestResultRepository = new LabTestResultRepository(_context);
                }

                return _LabTestResultRepository;
            }
        }

        /// <summary>
        ///     Gets the login tracking repository.
        /// </summary>
        public LoginTrackingRepository LoginTrackingRepository
        {
            get
            {
                if (_loginTrackingRepository == null)
                {
                    _loginTrackingRepository = new LoginTrackingRepository(_context);
                }

                return _loginTrackingRepository;
            }
        }

        /// <summary>
        ///     Gets the mc contract repository.
        /// </summary>
        public MCContractRepository MCContractRepository
        {
            get
            {
                return _mcContractRepository
                       ?? (_mcContractRepository = new MCContractRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the mc order code rates repository.
        /// </summary>
        public MCOrderCodeRatesRepository MCOrderCodeRatesRepository
        {
            get
            {
                return _MCOrderCodeRatesRepository
                       ?? (_MCOrderCodeRatesRepository = new MCOrderCodeRatesRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the mc rules table repository.
        /// </summary>
        public MCRulesTableRepository MCRulesTableRepository
        {
            get
            {
                if (_MCRulesTableRepository == null)
                {
                    _MCRulesTableRepository = new MCRulesTableRepository(_context);
                }

                return _MCRulesTableRepository;
            }
        }

        /// <summary>
        ///     Gets the managed care repository.
        /// </summary>
        public ManagedCareRepository ManagedCareRepository
        {
            get
            {
                if (_managedCareRepository == null)
                {
                    _managedCareRepository = new ManagedCareRepository(_context);
                }

                return _managedCareRepository;
            }
        }

        /// <summary>
        ///     Gets the manual charges tracking repository.
        /// </summary>
        public ManualChargesTrackingRepository ManualChargesTrackingRepository
        {
            get
            {
                if (_ManualChargesTrackingRepository == null)
                {
                    _ManualChargesTrackingRepository = new ManualChargesTrackingRepository(_context);
                }

                return _ManualChargesTrackingRepository;
            }
        }

        ///// <summary>
        /////     Gets the manual dashboard repository.
        ///// </summary>
        //public ManualDashboardRepository ManualDashboardRepository
        //{
        //    get
        //    {
        //        if (_ManualDashboardRepository == null)
        //        {
        //            _ManualDashboardRepository = new ManualDashboardRepository(_context);
        //        }

        //        return _ManualDashboardRepository;
        //    }
        //}

        /// <summary>
        ///     Gets the mapping patient bed repository.
        /// </summary>
        public MappingPatientBedRepository MappingPatientBedRepository
        {
            get
            {
                if (_mappingPatientBedRepository == null)
                {
                    _mappingPatientBedRepository = new MappingPatientBedRepository(_context);
                }

                return _mappingPatientBedRepository;
            }
        }

        /// <summary>
        ///     Gets the max values repository.
        /// </summary>
        public MaxValuesRepository MaxValuesRepository
        {
            get
            {
                if (_MaxValuesRepository == null)
                {
                    _MaxValuesRepository = new MaxValuesRepository(_context);
                }

                return _MaxValuesRepository;
            }
        }

        /// <summary>
        ///     Gets the medical history repository.
        /// </summary>
        public MedicalHistoryRepository MedicalHistoryRepository
        {
            get
            {
                if (_MedicalHistoryRepository == null)
                {
                    _MedicalHistoryRepository = new MedicalHistoryRepository(_context);
                }

                return _MedicalHistoryRepository;
            }
        }

        /// <summary>
        ///     Gets the medical notes repository.
        /// </summary>
        public MedicalNotesRepository MedicalNotesRepository
        {
            get
            {
                if (_MedicalNotesRepository == null)
                {
                    _MedicalNotesRepository = new MedicalNotesRepository(_context);
                }

                return _MedicalNotesRepository;
            }
        }

        /// <summary>
        ///     Gets the medical record repository.
        /// </summary>
        public MedicalRecordRepository MedicalRecordRepository
        {
            get
            {
                if (_MedicalRecordRepository == null)
                {
                    _MedicalRecordRepository = new MedicalRecordRepository(_context);
                }

                return _MedicalRecordRepository;
            }
        }

        /// <summary>
        ///     Gets the medical vital repository.
        /// </summary>
        public MedicalVitalRepository MedicalVitalRepository
        {
            get
            {
                if (_MedicalVitalRepository == null)
                {
                    _MedicalVitalRepository = new MedicalVitalRepository(_context);
                }

                return _MedicalVitalRepository;
            }
        }

        /// <summary>
        ///     Gets the module access repository.
        /// </summary>
        public ModuleAccessRepository ModuleAccessRepository
        {
            get
            {
                if (_ModuleAccessRepository == null)
                {
                    _ModuleAccessRepository = new ModuleAccessRepository(_context);
                }

                return _ModuleAccessRepository;
            }
        }

        /// <summary>
        ///     Gets the open order activity schedule repository.
        /// </summary>
        public OpenOrderActivityScheduleRepository OpenOrderActivityScheduleRepository
        {
            get
            {
                if (_openOrderActivityScheduleRepository == null)
                {
                    _openOrderActivityScheduleRepository = new OpenOrderActivityScheduleRepository(_context);
                }

                return _openOrderActivityScheduleRepository;
            }
        }

        /// <summary>
        ///     Gets the open order repository.
        /// </summary>
        public OpenOrderRepository OpenOrderRepository
        {
            get
            {
                if (_openOrderRepository == null)
                {
                    _openOrderRepository = new OpenOrderRepository(_context);
                }

                return _openOrderRepository;
            }
        }

        /// <summary>
        ///     Gets the operating room repository.
        /// </summary>
        public OperatingRoomRepository OperatingRoomRepository
        {
            get
            {
                return _operatingRoomRepository
                       ?? (_operatingRoomRepository = new OperatingRoomRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the order activity repository.
        /// </summary>
        public OrderActivityRepository OrderActivityRepository
        {
            get
            {
                if (_OrderActivityRepository == null)
                {
                    _OrderActivityRepository = new OrderActivityRepository(_context);
                }

                return _OrderActivityRepository;
            }
        }

        /// <summary>
        ///     Gets the parameters repository.
        /// </summary>
        public ParametersRepository ParametersRepository
        {
            get
            {
                if (_ParametersRepository == null)
                {
                    _ParametersRepository = new ParametersRepository(_context);
                }

                return _ParametersRepository;
            }
        }

        /// <summary>
        ///     Gets the patient address relation repository.
        /// </summary>
        public PatientAddressRelationRepository PatientAddressRelationRepository
        {
            get
            {
                if (_patientAddressRelationRepository == null)
                {
                    _patientAddressRelationRepository = new PatientAddressRelationRepository(_context);
                }

                return _patientAddressRelationRepository;
            }
        }

        /// <summary>
        ///     Gets the patient discharge summary repository.
        /// </summary>
        public PatientDischargeSummaryRepository PatientDischargeSummaryRepository
        {
            get
            {
                if (_PatientDischargeSummaryRepository == null)
                {
                    _PatientDischargeSummaryRepository = new PatientDischargeSummaryRepository(_context);
                }

                return _PatientDischargeSummaryRepository;
            }
        }

        /// <summary>
        ///     Gets the patient evaluation repository.
        /// </summary>
        public PatientEvaluationRepository PatientEvaluationRepository
        {
            get
            {
                if (_PatientEvaluationRepository == null)
                {
                    _PatientEvaluationRepository = new PatientEvaluationRepository(_context);
                }

                return _PatientEvaluationRepository;
            }
        }

        /// <summary>
        ///     Gets the patient info changes queue repository.
        /// </summary>
        public PatientInfoChangesQueueRepository PatientInfoChangesQueueRepository
        {
            get
            {
                if (_PatientInfoChangesQueueRepository == null)
                {
                    _PatientInfoChangesQueueRepository = new PatientInfoChangesQueueRepository(_context);
                }

                return _PatientInfoChangesQueueRepository;
            }
        }

        /// <summary>
        ///     Method is used to return PatientInfoRepository Repository object
        /// </summary>
        public PatientInfoRepository PatientInfoRepository
        {
            get
            {
                if (_patientInfoRepository == null)
                {
                    _patientInfoRepository = new PatientInfoRepository(_context);
                }

                return _patientInfoRepository;
            }
        }

        /// <summary>
        ///     Gets the patient insurance repository.
        /// </summary>
        public PatientInsuranceRepository PatientInsuranceRepository
        {
            get
            {
                if (_patientInsuranceRepository == null)
                {
                    _patientInsuranceRepository = new PatientInsuranceRepository(_context);
                }

                return _patientInsuranceRepository;
            }
        }

        /// <summary>
        ///     Gets the patient login detail repository.
        /// </summary>
        public PatientLoginDetailRepository PatientLoginDetailRepository
        {
            get
            {
                return _patientLoginDetailRepository
                       ?? (_patientLoginDetailRepository = new PatientLoginDetailRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the patient phone repository.
        /// </summary>
        public PatientPhoneRepository PatientPhoneRepository
        {
            get
            {
                if (_patientPhoneRepository == null)
                {
                    _patientPhoneRepository = new PatientPhoneRepository(_context);
                }

                return _patientPhoneRepository;
            }
        }

        /// <summary>
        ///     Gets the payment repository.
        /// </summary>
        public PaymentRepository PaymentRepository
        {
            get
            {
                if (_PaymentRepository == null)
                {
                    _PaymentRepository = new PaymentRepository(_context);
                }

                return _PaymentRepository;
            }
        }

        /// <summary>
        ///     Method is used to return Facility Repository object
        /// </summary>
        public PhysicianRepository PhysicianRepository
        {
            get
            {
                if (_physicianRepository == null)
                {
                    _physicianRepository = new PhysicianRepository(_context);
                }

                return _physicianRepository;
            }
        }

        /// <summary>
        ///     Gets the project dashboard repository.
        /// </summary>
        public ProjectDashboardRepository ProjectDashboardRepository
        {
            get
            {
                if (_ProjectDashboardRepository == null)
                {
                    _ProjectDashboardRepository = new ProjectDashboardRepository(_context);
                }

                return _ProjectDashboardRepository;
            }
        }

        /// <summary>
        ///     Gets the project targets repository.
        /// </summary>
        public ProjectTargetsRepository ProjectTargetsRepository
        {
            get
            {
                if (_ProjectTargetsRepository == null)
                {
                    _ProjectTargetsRepository = new ProjectTargetsRepository(_context);
                }

                return _ProjectTargetsRepository;
            }
        }

        /// <summary>
        ///     Gets the project task targets repository.
        /// </summary>
        public ProjectTaskTargetsRepository ProjectTaskTargetsRepository
        {
            get
            {
                if (_ProjectTaskTargetsRepository == null)
                {
                    _ProjectTaskTargetsRepository = new ProjectTaskTargetsRepository(_context);
                }

                return _ProjectTaskTargetsRepository;
            }
        }

        /// <summary>
        ///     Gets the project tasks repository.
        /// </summary>
        public ProjectTasksRepository ProjectTasksRepository
        {
            get
            {
                if (_ProjectTasksRepository == null)
                {
                    _ProjectTasksRepository = new ProjectTasksRepository(_context);
                }

                return _ProjectTasksRepository;
            }
        }

        /// <summary>
        ///     Gets the projects repository.
        /// </summary>
        public ProjectsRepository ProjectsRepository
        {
            get
            {
                if (_ProjectsRepository == null)
                {
                    _ProjectsRepository = new ProjectsRepository(_context);
                }

                return _ProjectsRepository;
            }
        }

        /// <summary>
        ///     Gets the role permission repository.
        /// </summary>
        public RolePermissionRepository RolePermissionRepository
        {
            get
            {
                if (_rolePermissionRepository == null)
                {
                    _rolePermissionRepository = new RolePermissionRepository(_context);
                }

                return _rolePermissionRepository;
            }
        }

        /// <summary>
        ///     Gets the role repository.
        /// </summary>
        public RoleRepository RoleRepository
        {
            get
            {
                if (_roleRepository == null)
                {
                    _roleRepository = new RoleRepository(_context);
                }

                return _roleRepository;
            }
        }

        /// <summary>
        ///     Gets the role tabs repository.
        /// </summary>
        public RoleTabsRepository RoleTabsRepository
        {
            get
            {
                if (_RoleTabsRepository == null)
                {
                    _RoleTabsRepository = new RoleTabsRepository(_context);
                }

                return _RoleTabsRepository;
            }
        }

        /// <summary>
        ///     Gets the rule master repository.
        /// </summary>
        public RuleMasterRepository RuleMasterRepository
        {
            get
            {
                if (_RuleMasterRepository == null)
                {
                    _RuleMasterRepository = new RuleMasterRepository(_context);
                }

                return _RuleMasterRepository;
            }
        }

        /// <summary>
        ///     Gets the rule step repository.
        /// </summary>
        public RuleStepRepository RuleStepRepository
        {
            get
            {
                if (_RuleStepRepository == null)
                {
                    _RuleStepRepository = new RuleStepRepository(_context);
                }

                return _RuleStepRepository;
            }
        }

        /// <summary>
        ///     Gets the screen repository.
        /// </summary>
        public SchedulingRepository SchedulingRepository
        {
            get
            {
                if (_schedulingRepository == null)
                {
                    _schedulingRepository = new SchedulingRepository(_context);
                }

                return _schedulingRepository;
            }
        }

        /// <summary>
        ///     Gets the screen repository.
        /// </summary>
        public ScreenRepository ScreenRepository
        {
            get
            {
                if (_screenRepository == null)
                {
                    _screenRepository = new ScreenRepository(_context);
                }

                return _screenRepository;
            }
        }

        /// <summary>
        ///     Gets the scrub edit track repository.
        /// </summary>
        public ScrubEditTrackRepository ScrubEditTrackRepository
        {
            get
            {
                if (_ScrubEditTrackRepository == null)
                {
                    _ScrubEditTrackRepository = new ScrubEditTrackRepository(_context);
                }

                return _ScrubEditTrackRepository;
            }
        }

        /// <summary>
        ///     Gets the scrub header repository.
        /// </summary>
        public ScrubHeaderRepository ScrubHeaderRepository
        {
            get
            {
                if (_ScrubHeaderRepository == null)
                {
                    _ScrubHeaderRepository = new ScrubHeaderRepository(_context);
                }

                return _ScrubHeaderRepository;
            }
        }

        /// <summary>
        ///     Gets the scrub report repository.
        /// </summary>
        public ScrubReportRepository ScrubReportRepository
        {
            get
            {
                if (_ScrubReportRepository == null)
                {
                    _ScrubReportRepository = new ScrubReportRepository(_context);
                }

                return _ScrubReportRepository;
            }
        }

        ///// <summary>
        ///// Method is used to return Encounter Order Repository object
        ///// </summary>
        // public EncounterOrderRepository EncounterOrderRepository
        // {
        // get
        // {
        // if (this._encounterOrderRepository == null)
        // {
        // this._encounterOrderRepository = new EncounterOrderRepository(_context);
        // }
        // return _encounterOrderRepository;
        // }
        // }

        /// <summary>
        ///     Method is used to return Service Code Repository object
        /// </summary>
        public ServiceCodeRepository ServiceCodeRepository
        {
            get
            {
                if (_servicecodeRepository == null)
                {
                    _servicecodeRepository = new ServiceCodeRepository(_context);
                }

                return _servicecodeRepository;
            }
        }

        /// <summary>
        ///     Gets the state repository.
        /// </summary>
        public StateRepository StateRepository
        {
            get
            {
                if (_stateRepository == null)
                {
                    _stateRepository = new StateRepository(_context);
                }

                return _stateRepository;
            }
        }

        /// <summary>
        ///     Method is used to return systemConfiguration Repository object
        /// </summary>
        public SystemConfigurationRepository SystemConfigurationRepository
        {
            get
            {
                if (_systemConfigurationRepository == null)
                {
                    _systemConfigurationRepository = new SystemConfigurationRepository(_context);
                }

                return _systemConfigurationRepository;
            }
        }

        /// <summary>
        ///     Gets the tp file header repository.
        /// </summary>
        public TPFileHeaderRepository TPFileHeaderRepository
        {
            get
            {
                if (_TPFileHeaderRepository == null)
                {
                    _TPFileHeaderRepository = new TPFileHeaderRepository(_context);
                }

                return _TPFileHeaderRepository;
            }
        }

        /// <summary>
        ///     Gets the tp file xml repository.
        /// </summary>
        public TPFileXMLRepository TPFileXMLRepository
        {
            get
            {
                if (_TPFileXMLRepository == null)
                {
                    _TPFileXMLRepository = new TPFileXMLRepository(_context);
                }

                return _TPFileXMLRepository;
            }
        }

        /// <summary>
        ///     Gets the tpxml parsed data repository.
        /// </summary>
        public TPXMLParsedDataRepository TPXMLParsedDataRepository
        {
            get
            {
                if (_TPXMLParsedDataRepository == null)
                {
                    _TPXMLParsedDataRepository = new TPXMLParsedDataRepository(_context);
                }

                return _TPXMLParsedDataRepository;
            }
        }

        /// <summary>
        ///     Gets the tabs repository.
        /// </summary>
        public TabsRepository TabsRepository
        {
            get
            {
                if (_TabsRepository == null)
                {
                    _TabsRepository = new TabsRepository(_context);
                }

                return _TabsRepository;
            }
        }

        ///// <summary>
        ///// Method is used to return Facility Repository object
        ///// </summary>
        // public LicenseRepository LicenseRepository
        // {
        // get
        // {
        // if (this._licenseRepository == null)
        // {
        // this._licenseRepository = new LicenseRepository(_context);
        // }
        // return _licenseRepository;
        // }
        // }

        /// <summary>
        ///     Gets the user role repository.
        /// </summary>
        public UserRoleRepository UserRoleRepository
        {
            get
            {
                if (_userRoleRepository == null)
                {
                    _userRoleRepository = new UserRoleRepository(_context);
                }

                return _userRoleRepository;
            }
        }

        /// <summary>
        ///     Method is used to return users Repository object
        /// </summary>
        public UsersRepository UsersRepository
        {
            get
            {
                return _usersRepository ?? (_usersRepository = new UsersRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the x advice xml parsed data repository.
        /// </summary>
        public XAdviceXMLParsedDataRepository XAdviceXMLParsedDataRepository
        {
            get
            {
                if (_XAdviceXMLParsedDataRepository == null)
                {
                    _XAdviceXMLParsedDataRepository = new XAdviceXMLParsedDataRepository(_context);
                }

                return _XAdviceXMLParsedDataRepository;
            }
        }

        /// <summary>
        ///     Gets the x file header repository.
        /// </summary>
        public XFileHeaderRepository XFileHeaderRepository
        {
            get
            {
                if (_XFileHeaderRepository == null)
                {
                    _XFileHeaderRepository = new XFileHeaderRepository(_context);
                }

                return _XFileHeaderRepository;
            }
        }

        /// <summary>
        ///     Gets the x file xml repository.
        /// </summary>
        public XFileXMLRepository XFileXMLRepository
        {
            get
            {
                if (_XFileXMLRepository == null)
                {
                    _XFileXMLRepository = new XFileXMLRepository(_context);
                }

                return _XFileXMLRepository;
            }
        }

        /// <summary>
        ///     Gets the x payment file xml repository.
        /// </summary>
        public XPaymentFileXMLRepository XPaymentFileXMLRepository
        {
            get
            {
                if (_XPaymentFileXMLRepository == null)
                {
                    _XPaymentFileXMLRepository = new XPaymentFileXMLRepository(_context);
                }

                return _XPaymentFileXMLRepository;
            }
        }

        /// <summary>
        ///     Gets the x payment return repository.
        /// </summary>
        public XPaymentReturnRepository XPaymentReturnRepository
        {
            get
            {
                if (_XPaymentReturnRepository == null)
                {
                    _XPaymentReturnRepository = new XPaymentReturnRepository(_context);
                }

                return _XPaymentReturnRepository;
            }
        }

        /// <summary>
        ///     Gets the xactivity repository.
        /// </summary>
        public XactivityRepository XactivityRepository
        {
            get
            {
                return _XactivityRepository ?? (_XactivityRepository = new XactivityRepository(_context));
            }
        }

        /// <summary>
        ///     Gets the xclaim repository.
        /// </summary>
        public XclaimRepository XclaimRepository
        {
            get
            {
                if (_XclaimRepository == null)
                {
                    _XclaimRepository = new XclaimRepository(_context);
                }

                return _XclaimRepository;
            }
        }

        public PDFTemplatesRepository PDFTemplatesRepository
        {
            get
            {
                if (_PDFTemplatesRepository == null)
                {
                    _PDFTemplatesRepository = new PDFTemplatesRepository(_context);
                }

                return _PDFTemplatesRepository;
            }
        }


        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
