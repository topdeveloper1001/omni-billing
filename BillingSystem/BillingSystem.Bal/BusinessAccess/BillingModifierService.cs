using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{
    public class BillingModifierService : IBillingModifierService
    {
        private readonly IRepository<BillingModifier> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public BillingModifierService(IRepository<BillingModifier> repository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<BillingModifierCustomModel> GetListByEntity(long facilityId, long corporateId)
        {
            var m = _repository.Where(b => b.FacilityId == facilityId && b.IsActive).ToList();
            var vm = m.Select(a => _mapper.Map<BillingModifierCustomModel>(a));
            return vm;

        }

        public BillingModifierCustomModel GetById(long id)
        {
            var m = _repository.GetSingle(id);
            var vm = _mapper.Map<BillingModifierCustomModel>(m);
            return vm;

        }

        public long SaveRecord(BillingModifierCustomModel vm)
        {
            long? exeStatus = null;
            var duplicate = _repository.Where(x => x.Code == vm.Code && x.IsActive && x.FacilityId == vm.FacilityId && x.CorporateId == vm.CorporateId && x.Id != vm.Id).ToList();
            if (duplicate != null && duplicate.Any())
                return -1;

            var m = _mapper.Map<BillingModifier>(vm);
            if (vm.Id > 0)
            {
                var current = _repository.GetSingle(vm.Id);
                if (current != null)
                {
                    m.CreatedBy = current.CreatedBy;
                    m.CreatedDate = current.CreatedDate;
                }
                exeStatus = _repository.Updatei(m, m.Id);
            }
            else
                exeStatus = _repository.Create(m);
            return exeStatus ?? 0;
        }

        public long DeleteRecord(long id, int userId, DateTime dateTime)
        {
            long? exeStatus = null;
            var current = _repository.GetSingle(id);
            if (current != null)
            {
                current.IsActive = false;
                current.ModifiedBy = userId;
                current.ModifiedDate = dateTime;
                exeStatus = _repository.Updatei(current, id);

            }
            return exeStatus ?? 0;
        }
    }
}
