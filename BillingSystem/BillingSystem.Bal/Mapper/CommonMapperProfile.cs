using AutoMapper;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.EntityDto;

namespace BillingSystem.Bal.Mapper
{
    public class CommonMapperProfile : Profile
    {
        public CommonMapperProfile()
        {
            CreateMap<Scheduling, SchedulingCustomModel>().ReverseMap();
        }
    }
}
