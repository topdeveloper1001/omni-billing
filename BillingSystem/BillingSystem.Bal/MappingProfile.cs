using AutoMapper;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Users, UsersViewModel>().ReverseMap();
            CreateMap<FacilityRole, FacilityRoleCustomModel>().ReverseMap();
            CreateMap<EquipmentMaster, EquipmentCustomModel>().ReverseMap();
            CreateMap<FacilityStructureCustomModel, FacilityStructure>().ReverseMap();
            CreateMap<FacilityCustomModel, Facility>().ReverseMap();
            CreateMap<FacilityDepartmentCustomModel, FacilityDepartment>().ReverseMap();
            CreateMap<ManualDashboardCustomModel, ManualDashboard>().ReverseMap();
            CreateMap<DashboardDisplayOrderCustomModel, DashboardDisplayOrder>().ReverseMap();
            CreateMap<DashboardIndicatorDataCustomModel, DashboardIndicatorData>().ReverseMap();
            CreateMap<DashboardIndicatorsCustomModel, DashboardIndicators>().ReverseMap();
            CreateMap<DeptTimming, DeptTimmingCustomModel>().ReverseMap();
            CreateMap<ProjectTargets, ProjectTargetsCustomModel>().ReverseMap();
            CreateMap<DashboardRemark, DashboardRemarkCustomModel>().ReverseMap();
            CreateMap<DashboardParameters, DashboardParametersCustomModel>().ReverseMap();
            CreateMap<IndicatorDataCheckListCustomModel, IndicatorDataCheckList>().ReverseMap();
        }
    }
}
