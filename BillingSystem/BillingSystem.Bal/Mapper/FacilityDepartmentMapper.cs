﻿using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class FacilityDepartmentMapper : Mapper<FacilityDepartment, FacilityDepartmentCustomModel>
    {
        public override FacilityDepartmentCustomModel MapModelToViewModel(FacilityDepartment model)
        {
            var vm = base.MapModelToViewModel(model);
            using (var bal = new BaseBal())
            {
                //var type = bal.GetNameByGlobalCodeValue(Convert.ToString(model.OperatingType),
                //    Convert.ToString((int)GlobalCodeCategoryValue.OperatingRoomType));
                //var statusdesc = bal.GetNameByGlobalCodeValue(Convert.ToString(model.Status),
                //    Convert.ToString((int)GlobalCodeCategoryValue.OrderStatus));
                //var encounterNumber = bal.GetEncounterNumberById(Convert.ToInt32(model.EncounterId));
            }
            return vm;
        }
    }
}
