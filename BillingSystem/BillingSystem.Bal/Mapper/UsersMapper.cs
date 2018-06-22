﻿using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.EntityDto;

namespace BillingSystem.Bal.Mapper
{
    public class UsersMapper : Mapper<Users, UsersViewModel>
    {
        public override UsersViewModel MapModelToViewModel(Users model)
        {
            var vm = base.MapModelToViewModel(model);
            return vm;
        }
    }


    public class UserDtoMapper : Mapper<Users, UserDto>
    {
        public override UserDto MapModelToViewModel(Users model)
        {
            var vm = base.MapModelToViewModel(model);
            vm.Name = $"{model.FirstName} {model.LastName}";
            return vm;
        }
    }
}
