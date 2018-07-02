using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;

using AutoMapper;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DeptTimmingService : IDeptTimmingService
    {
        private readonly IRepository<DeptTimming> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public DeptTimmingService(IRepository<DeptTimming> repository, IRepository<GlobalCodes> gRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _gRepository = gRepository;
            _context = context;
            _mapper = mapper;
        }

        public List<DeptTimmingCustomModel> GetDeptTimming()
        {
            var lst = _repository.GetAll().ToList();
            return lst.Select(x => _mapper.Map<DeptTimmingCustomModel>(x)).ToList();
        }
        public string GetTimingAddedById(int id)
        {
            var list = _repository.Where(x => x.FacilityStructureID == id && x.IsActive).FirstOrDefault();
            return list != null && list.IsActive ? "YES" : "NO";
        }
        public List<DeptTimmingCustomModel> GetDeptTimmingByDepartmentId(int departmenId)
        {
            var lst = _repository.Where(x => x.FacilityStructureID == departmenId).ToList();
            return lst.Select(x => _mapper.Map<DeptTimmingCustomModel>(x)).ToList();
        }

        public DeptTimming GetDeptTimmingById(int? deptTimmingId)
        {
            var m = _repository.Where(x => x.DeptTimmingId == deptTimmingId).FirstOrDefault();
            return m;
        }

        public int SaveDeptTimming(DeptTimming m)
        {
            if (m.DeptTimmingId > 0)
                _repository.UpdateEntity(m, m.DeptTimmingId);
            else
                _repository.Create(m);

            return m.DeptTimmingId;
        }

        public int SaveDeptTimmingList(List<DeptTimming> m)
        {
            try
            {
                var firstOrDefault = m.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var facilityDepartmentId = firstOrDefault.FacilityStructureID;
                    var mlst = _repository.Where(x => x.FacilityStructureID == facilityDepartmentId).ToList();
                    if (mlst != null && mlst.Any())
                        _repository.Delete(mlst);

                    _repository.Create(m);
                }

                return 1;

            }
            catch (Exception)
            {
                return -1;
            }
        }

        public int DeleteDepartmentTiming(int facilityStructureId)
        {
            try
            {
                var mlst = _repository.Where(x => x.FacilityStructureID == facilityStructureId).ToList();
                if (mlst != null && mlst.Any())
                    _repository.Delete(mlst);
                return 1;

            }
            catch (Exception)
            {

                return -1;
            }


        }

        public List<DeptTimming> GetDepTimingsById(int departmenId)
        {
            var lst = _repository.Where(x => x.FacilityStructureID == departmenId).ToList();
            return lst;
        }

        public List<DeptTimmingCustomModel> GetDeptTimmingByDepartmentId1(int departmenId)
        {
            var lst = _repository.Where(x => x.FacilityStructureID == departmenId).ToList();
            return lst.Select(x => _mapper.Map<DeptTimmingCustomModel>(x)).ToList();
        }

    }
}
