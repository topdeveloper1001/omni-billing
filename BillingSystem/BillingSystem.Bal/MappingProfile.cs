﻿using AutoMapper;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.Model;

namespace BillingSystem.Bal
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppointmentTypesCustomModel, AppointmentTypes>().ReverseMap();
            CreateMap<AuthorizationCustomModel, Authorization>().ReverseMap();
            CreateMap<BedCharges, BedChargesCustomModel>().ReverseMap();
            CreateMap<UBedMaster, BedMasterCustomModel>().ReverseMap();
            CreateMap<BedRateCardCustomModel, BedRateCard>().ReverseMap();
            CreateMap<BillActivity, BillActivityCustomModel>().ReverseMap();
            CreateMap<BillingModifier, BillingModifierCustomModel>().ReverseMap();
            CreateMap<CareplanCustomModel, Careplan>().ReverseMap();
            CreateMap<CarePlanTaskCustomModel, CarePlanTask>().ReverseMap();
            CreateMap<Categories, CategoriesCustomModel>().ReverseMap();
            CreateMap<City, CityCustomModel>().ReverseMap();
            CreateMap<ClinicianRosterCustomModel, ClinicianRoster>().ReverseMap();
            CreateMap<Corporate, CorporateCustomModel>().ReverseMap();
            CreateMap<CPTCodes, CPTCodesCustomModel>().ReverseMap();
            CreateMap<DashboardBudget, DashboardBudgetCustomModel>().ReverseMap();
            CreateMap<DashboardDisplayOrderCustomModel, DashboardDisplayOrder>().ReverseMap();
            CreateMap<DashboardIndicatorDataCustomModel, DashboardIndicatorData>().ReverseMap();
            CreateMap<DashboardIndicators, DashboardIndicatorsCustomModel>().ReverseMap();
            CreateMap<DashboardParametersCustomModel, DashboardParameters>().ReverseMap();
            CreateMap<DashboardRemarkCustomModel, DashboardRemark>().ReverseMap();
            CreateMap<DashboardTargets, DashboardTargetsCustomModel>().ReverseMap();
            CreateMap<DashboardTransactionCounter, DashboardTransactionCounterCustomModel>().ReverseMap();
            CreateMap<DenialCodeCustomModel, Denial>().ReverseMap();
            CreateMap<DeptTimmingCustomModel, DeptTimming>().ReverseMap();
            CreateMap<DiagnosisCustomModel, Diagnosis>().ReverseMap();
            CreateMap<DischargeSummaryDetail, DischargeSummaryDetailCustomModel>().ReverseMap();
            CreateMap<DocumentsTemplates, DocumentsTemplatesCustomModel>().ReverseMap();
            CreateMap<DrugAllergyLog, DrugAllergyLogCustomModel>().ReverseMap();
            CreateMap<DrugInstructionAndDosingCustomModel, DrugInstructionAndDosing>().ReverseMap();
            CreateMap<DrugInteractions, DrugInteractionsCustomModel>().ReverseMap();
            CreateMap<Encounter, EncounterCustomModel>().ReverseMap();
            CreateMap<EquipmentMaster, EquipmentCustomModel>().ReverseMap();
            CreateMap<FacilityDepartmentCustomModel, FacilityDepartment>().ReverseMap();
            CreateMap<FacilityCustomModel, Facility>().ReverseMap();
            CreateMap<FacilityRoleCustomModel, FacilityRole>().ReverseMap();
            CreateMap<FacilityStructureCustomModel, FacilityStructure>().ReverseMap();
            CreateMap<FacultyRooster, FacultyRoosterLogCustomModel>().ReverseMap();
            CreateMap<FacultyRoosterCustomModel, FacultyRooster>().ReverseMap();
            CreateMap<UserDefinedDescriptions, FavoritesCustomModel>().ReverseMap();
            CreateMap<FacultyTimeslotsCustomModel, FacultyTimeslots>().ReverseMap();
            CreateMap<FutureOpenOrderCustomModel, FutureOpenOrder>().ReverseMap();
            CreateMap<FutureOrderActivityCustomModel, FutureOrderActivity>().ReverseMap();
            CreateMap<GlobalCodeCustomModel, GlobalCodes>().ReverseMap();
            CreateMap<HolidayPlannerDetailsCustomModel, HolidayPlannerDetails>().ReverseMap();
            CreateMap<HolidayPlannerCustomModel, HolidayPlanner>().ReverseMap();
            CreateMap<IndicatorDataCheckListCustomModel, IndicatorDataCheckList>().ReverseMap();
            CreateMap<ManagedCare, ManagedCareCustomModel>().ReverseMap();
            CreateMap<ManualChargesTracking, ManualChargesTrackingCustomModel>().ReverseMap();
            CreateMap<ManualDashboardCustomModel, ManualDashboard>().ReverseMap();
            CreateMap<MCContract, McContractCustomModel>().ReverseMap();
            CreateMap<MCOrderCodeRates, MCOrderCodeRatesCustomModel>().ReverseMap();
            CreateMap<MCRulesTable, MCRulesTableCustomModel>().ReverseMap();
            CreateMap<MedicalHistoryCustomModel, MedicalHistory>().ReverseMap();
            CreateMap<MedicalNecessityCustomModel, MedicalNecessity>().ReverseMap();
            CreateMap<MedicalNotes, MedicalNotesCustomModel>().ReverseMap();
            CreateMap<MedicalVital, MedicalVitalCustomModel>().ReverseMap();
            CreateMap<OpenOrder, OpenOrderCustomModel>().ReverseMap();
            CreateMap<OperatingRoomCustomModel, OperatingRoom>().ReverseMap();
            CreateMap<OrderActivity, OrderActivityCustomModel>().ReverseMap();
            CreateMap<OpenOrderActivitySchedule, OpenOrderActivityScheduleCustomModel>().ReverseMap();
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
