using AutoMapper;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.Model;

namespace BillingSystem.Bal
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<Users, UsersViewModel>().ReverseMap();
            //CreateMap<FacilityRole, FacilityRoleCustomModel>().ReverseMap();
            //CreateMap<EquipmentMaster, EquipmentCustomModel>().ReverseMap();
            //CreateMap<FacilityStructureCustomModel, FacilityStructure>().ReverseMap();
            //CreateMap<FacilityCustomModel, Facility>().ReverseMap();
            //CreateMap<FacilityDepartmentCustomModel, FacilityDepartment>().ReverseMap();
            //CreateMap<ManualDashboardCustomModel, ManualDashboard>().ReverseMap();
            //CreateMap<DashboardDisplayOrderCustomModel, DashboardDisplayOrder>().ReverseMap();
            //CreateMap<DashboardIndicatorDataCustomModel, DashboardIndicatorData>().ReverseMap();
            //CreateMap<DashboardIndicatorsCustomModel, DashboardIndicators>().ReverseMap();
            //CreateMap<DeptTimming, DeptTimmingCustomModel>().ReverseMap();
            //CreateMap<ProjectTargets, ProjectTargetsCustomModel>().ReverseMap();
            //CreateMap<DashboardRemark, DashboardRemarkCustomModel>().ReverseMap();
            //CreateMap<DashboardParameters, DashboardParametersCustomModel>().ReverseMap();
            //CreateMap<IndicatorDataCheckListCustomModel, IndicatorDataCheckList>().ReverseMap();
            CreateMap<AppointmentTypesCustomModel, AppointmentTypes>().ReverseMap();
            CreateMap<AuthorizationCustomModel, Authorization>().ReverseMap();
            CreateMap<BedRateCardCustomModel, BedRateCard>().ReverseMap();
            CreateMap<BillingModifierCustomModel, BillingModifier>().ReverseMap();
            CreateMap<CareplanCustomModel, Careplan>().ReverseMap();
            CreateMap<CarePlanTaskCustomModel, CarePlanTask>().ReverseMap();
            CreateMap<ClinicianRosterCustomModel, ClinicianRoster>().ReverseMap();
            CreateMap<DashboardDisplayOrderCustomModel, DashboardDisplayOrder>().ReverseMap();
            CreateMap<DashboardIndicatorDataCustomModel, DashboardIndicatorData>().ReverseMap();
            CreateMap<DashboardIndicatorsCustomModel, DashboardIndicators>().ReverseMap();
            CreateMap<DashboardParametersCustomModel, DashboardParameters>().ReverseMap();
            CreateMap<DashboardRemarkCustomModel, DashboardRemark>().ReverseMap();
            CreateMap<DenialCodeCustomModel, Denial>().ReverseMap();
            CreateMap<DeptTimmingCustomModel, DeptTimming>().ReverseMap();
            CreateMap<DiagnosisCustomModel, Diagnosis>().ReverseMap();
            CreateMap<DrugAllergyLogCustomModel, DrugAllergyLog>().ReverseMap();
            CreateMap<DrugInstructionAndDosingCustomModel, DrugInstructionAndDosing>().ReverseMap();
            CreateMap<DrugInteractions, DrugInteractionsCustomModel>().ReverseMap();
            CreateMap<EncounterCustomModel, Encounter>().ReverseMap();
            CreateMap<EquipmentCustomModel, EquipmentMaster>().ReverseMap();
            CreateMap<FacilityDepartmentCustomModel, FacilityDepartment>().ReverseMap();
            CreateMap<FacilityCustomModel, Facility>().ReverseMap();
            CreateMap<FacilityRoleCustomModel, FacilityRole>().ReverseMap();
            CreateMap<FacilityStructureCustomModel, FacilityStructure>().ReverseMap();
            CreateMap<FacultyRooster, FacultyRoosterLogCustomModel>().ReverseMap();
            CreateMap<FacultyRoosterCustomModel, FacultyRooster>().ReverseMap();
            CreateMap<FacultyTimeslotsCustomModel, FacultyTimeslots>().ReverseMap();
            CreateMap<FutureOpenOrderCustomModel, FutureOpenOrder>().ReverseMap();
            CreateMap<FutureOrderActivityCustomModel, FutureOrderActivity>().ReverseMap();
            CreateMap<GlobalCodeCustomModel, GlobalCodes>().ReverseMap();
            CreateMap<HolidayPlannerDetailsCustomModel, HolidayPlannerDetails>().ReverseMap();
            CreateMap<HolidayPlannerCustomModel, HolidayPlanner>().ReverseMap();
            CreateMap<IndicatorDataCheckListCustomModel, IndicatorDataCheckList>().ReverseMap();
            CreateMap<ManualDashboardCustomModel, ManualDashboard>().ReverseMap();
            CreateMap<MedicalHistoryCustomModel, MedicalHistory>().ReverseMap();
            CreateMap<MedicalNecessityCustomModel, MedicalNecessity>().ReverseMap();
            CreateMap<OpenOrderCustomModel, OpenOrder>().ReverseMap();
            CreateMap<OperatingRoomCustomModel, OperatingRoom>().ReverseMap();
            CreateMap<OrderActivityCustomModel, OrderActivity>().ReverseMap();
            CreateMap<PatientCarePlanCustomModel, PatientCarePlan>().ReverseMap();
            CreateMap<PatientEvaluationCustomModel, PatientEvaluation>().ReverseMap();
            CreateMap<PatientInfoCustomModel, PatientInfo>().ReverseMap();
            CreateMap<PatientLoginDetailCustomModel, PatientLoginDetail>().ReverseMap();
            CreateMap<PatientPreSchedulingCustomModel, PatientPreScheduling>().ReverseMap();
            CreateMap<PaymentCustomModel, Payment>().ReverseMap();
            CreateMap<PhysicianCustomModel, Physician>().ReverseMap();
            CreateMap<PlaceOfServiceCustomModel, PlaceOfService>().ReverseMap();
            CreateMap<PreSchedulingLinkCustomModel, PreSchedulingLink>().ReverseMap();
            CreateMap<ProjectsCustomModel, Projects>().ReverseMap();
            CreateMap<ProjectTargetsCustomModel, ProjectTargets>().ReverseMap();
            CreateMap<ProjectTasksCustomModel, ProjectTasks>().ReverseMap();
            CreateMap<ProjectTaskTargetsCustomModel, ProjectTaskTargets>().ReverseMap();
            CreateMap<RuleMasterCustomModel, RuleMaster>().ReverseMap();
            CreateMap<SchedulingCustomModel, Scheduling>().ReverseMap();
            CreateMap<SchedulingParametersCustomModel, SchedulingParameters>().ReverseMap();
            CreateMap<ScrubHeaderCustomModel, ScrubHeader>().ReverseMap();
            CreateMap<ScrubReportCustomModel, ScrubReport>().ReverseMap();
            CreateMap<ServiceCodeCustomModel, ServiceCode>().ReverseMap();
            CreateMap<TPFileHeaderCustomModel, TPFileHeader>().ReverseMap();
            CreateMap<TPXMLParsedDataCustomModel, TPXMLParsedData>().ReverseMap();
            CreateMap<UsersViewModel, Users>().ReverseMap();
            CreateMap<XFileHeader, XFileHeaderCustomModel>().ReverseMap();
            CreateMap<XAdviceXMLParsedData, XAdviceXMLParsedDataCustomModel>().ReverseMap();

        }
    }
}
