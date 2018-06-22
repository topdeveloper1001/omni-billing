using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class EquipmentMapper : Mapper<EquipmentMaster, EquipmentCustomModel>
    {
        public override EquipmentCustomModel MapModelToViewModel(EquipmentMaster model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new EquipmentBal())
                {
                    vm.EquipmentTypeName = bal.GetAppointmentNameById(Convert.ToInt32(model.EquipmentType),
                        Convert.ToInt32(model.CorporateId), Convert.ToInt32(model.FacilityId));
                    vm.FacilityName = bal.GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityId));
                }

                using (var fBal = new FacilityStructureBal())
                {
                    var departmentName = fBal.GetParentNameByFacilityStructureId(model.FacilityStructureId);
                    var obj = fBal.GetFacilityStructureById(model.FacilityStructureId);
                    vm.AssignedRoom = obj != null &&
                                      !string.IsNullOrEmpty(obj.FacilityStructureName)
                        ? obj.FacilityStructureName
                        : string.Empty;
                    vm.RoomDepartment = departmentName;

                    if (!string.IsNullOrEmpty(model.BaseLocation))
                    {
                        var dep = fBal.GetFacilityStructureById(Convert.ToInt32(model.BaseLocation));
                        vm.Department = dep != null ? dep.FacilityStructureName : string.Empty;
                    }
                }
            }
            return vm;
        }
    }
}
