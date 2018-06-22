using BillingSystem.Bal.Mapper;
using BillingSystem.Model.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{
    public class BillingModifierBal : BaseBal
    {
        private BillingModifierMapper BillingModifierMapper { get; set; }

        public BillingModifierBal()
        {
            BillingModifierMapper = new BillingModifierMapper();

        }

        public IEnumerable<BillingModifierCustomModel> GetListByEntity(long facilityId, long corporateId)
        {
            using (var rep = UnitOfWork.BillingModifierRepository)
            {
                var m = rep.Where(b => b.FacilityId == facilityId && b.IsActive).ToList();
                var vm = m.Select(a => BillingModifierMapper.MapModelToViewModel(a));
                return vm;
            }
        }

        public BillingModifierCustomModel GetById(long id)
        {
            using (var rep = UnitOfWork.BillingModifierRepository)
            {
                var m = rep.GetSingle(id);
                var vm = BillingModifierMapper.MapModelToViewModel(m);
                return vm;
            }
        }

        public long SaveRecord(BillingModifierCustomModel vm)
        {
            long? exeStatus = null;
            using (var rep = UnitOfWork.BillingModifierRepository)
            {
                var duplicate = rep.Where(x => x.Code == vm.Code && x.IsActive && x.FacilityId == vm.FacilityId && x.CorporateId == vm.CorporateId && x.Id != vm.Id).ToList();
                if (duplicate != null && duplicate.Any())
                    return -1;

                var m = BillingModifierMapper.MapViewModelToModel(vm);
                if (vm.Id > 0)
                {
                    var current = rep.GetSingle(vm.Id);
                    if (current != null)
                    {
                        m.CreatedBy = current.CreatedBy;
                        m.CreatedDate = current.CreatedDate;
                    }
                    exeStatus = rep.UpdateEntity(m, m.Id);
                }
                else
                    exeStatus = rep.Create(m);
                return exeStatus ?? 0;
            }
        }

        public long DeleteRecord(long id, int userId, DateTime dateTime)
        {
            long? exeStatus = null;
            using (var rep = UnitOfWork.BillingModifierRepository)
            {
                var current = rep.GetSingle(id);
                if (current != null)
                {
                    current.IsActive = false;
                    current.ModifiedBy = userId;
                    current.ModifiedDate = dateTime;
                    exeStatus = rep.UpdateEntity(current, id);

                }
                return exeStatus ?? 0;
            }
        }
    }
}
