using BillingSystem.Bal.Mapper;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.UOW;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PlaceOfServiceBal : BaseBal
    {
        private PlaceOfServiceMapper PlaceOfServiceMapper { get; set; }

        public PlaceOfServiceBal()
        {
            if (PlaceOfServiceMapper == null)
                PlaceOfServiceMapper = new PlaceOfServiceMapper();
        }

        public IEnumerable<PlaceOfServiceCustomModel> GetListByEntity(long facilityId, long corporateId)
        {
            using (var rep = UnitOfWork.PlaceOfServiceRepository)
            {
                var m = rep.Where(b => b.FacilityId == facilityId && b.IsActive).ToList();
                var vm = m.Select(a => PlaceOfServiceMapper.MapModelToViewModel(a));
                return vm;
            }
        }

        public PlaceOfServiceCustomModel GetById(long id)
        {
            using (var rep = UnitOfWork.PlaceOfServiceRepository)
            {
                var m = rep.GetSingle(id);
                var vm = PlaceOfServiceMapper.MapModelToViewModel(m);
                return vm;
            }
        }

        public long SaveRecord(PlaceOfServiceCustomModel vm)
        {
            long? executedStatus = null;
            using (var rep = UnitOfWork.PlaceOfServiceRepository)
            {
                var duplicate = rep.Where(x => x.Code == vm.Code && x.IsActive && x.FacilityId == vm.FacilityId && x.CorporateId == vm.CorporateId && x.Id != vm.Id).ToList();
                if (duplicate != null && duplicate.Any())
                    return -1;

                var m = PlaceOfServiceMapper.MapViewModelToModel(vm);
                if (m.Id > 0)
                {
                    var current = rep.GetSingle(m.Id);
                    if (current != null)
                    {
                        m.CreatedBy = current.CreatedBy;
                        m.CreatedDate = current.CreatedDate;
                    }
                    executedStatus = rep.UpdateEntity(m, m.Id);
                }
                else
                    executedStatus = rep.Create(m);
                return executedStatus.HasValue && executedStatus.Value > 0 ? executedStatus.Value : 0;
            }
        }

        public long DeleteRecord(long id, int userId, DateTime dateTime)
        {
            using (var rep = UnitOfWork.PlaceOfServiceRepository)
            {
                var current = rep.GetSingle(id);
                if (current != null)
                {
                    current.IsActive = false;
                    current.ModifiedBy = userId;
                    current.ModifiedDate = dateTime;
                    var result = rep.UpdateEntity(current, id);
                    return result ?? 0;
                }
                return 0;
            }
        }

    }
}
