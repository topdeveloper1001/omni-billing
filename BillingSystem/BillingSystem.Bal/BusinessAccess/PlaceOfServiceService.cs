using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PlaceOfServiceService : IPlaceOfServiceService
    {
        private readonly IRepository<PlaceOfService> _repository;
        private readonly IMapper _mapper;

        public PlaceOfServiceService(IRepository<PlaceOfService> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<PlaceOfServiceCustomModel> GetListByEntity(long facilityId, long corporateId)
        {
            var m = _repository.Where(b => b.FacilityId == facilityId && b.IsActive).ToList();
            var vm = m.Select(a => _mapper.Map<PlaceOfServiceCustomModel>(a));
            return vm;
        }

        public PlaceOfServiceCustomModel GetById(long id)
        {
            var m = _repository.GetSingle(id);
            var vm = _mapper.Map<PlaceOfServiceCustomModel>(m);

            return vm;

        }

        public long SaveRecord(PlaceOfServiceCustomModel vm)
        {
            long? executedStatus = null;
            var duplicate = _repository.Where(x => x.Code == vm.Code && x.IsActive && x.FacilityId == vm.FacilityId && x.CorporateId == vm.CorporateId && x.Id != vm.Id).ToList();
            if (duplicate != null && duplicate.Any())
                return -1;

            var m = _mapper.Map<PlaceOfService>(vm);
            if (m.Id > 0)
            {
                var current = _repository.GetSingle(m.Id);
                if (current != null)
                {
                    m.CreatedBy = current.CreatedBy;
                    m.CreatedDate = current.CreatedDate;
                }
                executedStatus = _repository.Updatei(m, m.Id);
            }
            else
                executedStatus = _repository.Create(m);
            return executedStatus.HasValue && executedStatus.Value > 0 ? executedStatus.Value : 0;
        }

        public long DeleteRecord(long id, int userId, DateTime dateTime)
        {
            var current = _repository.GetSingle(id);
            if (current != null)
            {
                current.IsActive = false;
                current.ModifiedBy = userId;
                current.ModifiedDate = dateTime;
                var result = _repository.Updatei(current, id);
                return result;
            }
            return 0;
        }

    }
}
