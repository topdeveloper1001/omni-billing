using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IRepository<EquipmentMaster> _repository;
        private readonly IRepository<EquipmentLog> _lRepository;
        private readonly IRepository<Scheduling> _sRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public EquipmentService(IRepository<EquipmentMaster> repository, IRepository<EquipmentLog> lRepository, IRepository<Scheduling> sRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _lRepository = lRepository;
            _sRepository = sRepository;
            _context = context;
            _mapper = mapper;
        }

        //Function to get Equipment by Master ID
        public EquipmentMaster GetEquipmentByMasterId(int id)
        {
            var m = _repository.Where(x => x.EquipmentMasterId == id).FirstOrDefault();
            return m;
        }

        public int AddUpdateEquipment(EquipmentMaster equipment)
        {
            try
            {
                var currentDateTime = equipment.EquipmentMasterId > 0
                    ? equipment.ModifiedDate
                    : equipment.CreatedDate;

                var userId = equipment.EquipmentMasterId > 0
                    ? equipment.ModifiedBy
                    : equipment.CreatedBy;

                if (equipment.EquipmentMasterId > 0)
                {
                    _repository.UpdateEntity(equipment, equipment.EquipmentMasterId);
                }
                else
                    _repository.Create(equipment);

                var eqLog = new EquipmentLog
                {
                    EquipmentId = equipment.EquipmentMasterId,
                    DisableDate = null,
                    EnableDate = null,
                    CreatedBy = userId,
                    CreatedDate = currentDateTime


                };

                if (equipment.EquipmentDisabled)
                    eqLog.DisableDate = currentDateTime;
                else
                    eqLog.EnableDate = currentDateTime;

                SaveEquipmentLog(eqLog);

                return equipment.EquipmentMasterId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int UpdateEuipmentCustomModel(EquipmentCustomModel vm)
        {
            var currentDateTime = vm.ModifiedDate;
            var userId = vm.ModifiedBy;

            var m = _mapper.Map<EquipmentMaster>(vm);
            if (m.EquipmentMasterId > 0)
            {
                _repository.UpdateEntity(m, m.EquipmentMasterId);
            }

            var eqLog = new EquipmentLog
            {
                EquipmentId = m.EquipmentMasterId,
                CreatedBy = userId,
                CreatedDate = currentDateTime,
                DisableDate = null,
                EnableDate = null
            };
            if (vm.ActiveDeactive)
            {
                eqLog.EnableDate = currentDateTime;
            }
            else
            {
                eqLog.DisableDate = currentDateTime;
            }
            SaveEquipmentLog(eqLog);
            return m.EquipmentMasterId;
        }

        public List<EquipmentCustomModel> GetEquipmentList(bool showIsDisabled, string facilityId)
        {
            var lst = _repository.Where(x => x.EquipmentDisabled == showIsDisabled && x.IsDeleted == false && x.FacilityId.Trim().ToLower().Equals(facilityId)).ToList();
            return lst.Select(x => _mapper.Map<EquipmentCustomModel>(x)).ToList();

        }


        public List<EquipmentMaster> GetEquipmentListByFacilityId(string facilityId, int facilityStructureId)
        {
            var fsList = new List<int> { 0, facilityStructureId };
            fsList = fsList.Distinct().ToList();
            var list = _repository.Where(x => x.IsDeleted != true && x.FacilityId.Trim().Equals(facilityId) && x.EquipmentDisabled != true && fsList.Contains(x.FacilityStructureId)).ToList();
            return list;
        }


        public bool AddRoomIdToEquipments(int facilityStructureId, List<int> equipmentIds)
        {
            bool status;
            var removalList = _repository.Where(e => e.FacilityStructureId == facilityStructureId).ToList();

            foreach (var item in removalList)
                item.FacilityStructureId = 0;

            var list = _repository.Where(e => equipmentIds.Contains(e.EquipmentMasterId)).ToList();
            foreach (var item in list)
                item.FacilityStructureId = facilityStructureId;
            _repository.Update(list);
            status = true;
            return status;
        }


        private int SaveEquipmentLog(EquipmentLog m)
        {
            _lRepository.Create(m);
            return m.Id;
        }

        public List<EquipmentCustomModel> GetDeletedEquipmentList(bool showIsDeleted, string facilityId)
        {
            var lst = _repository.Where(x => x.IsDeleted == showIsDeleted && x.FacilityId.Trim().ToLower().Equals(facilityId) && x.EquipmentDisabled != true).ToList();
            return lst.Select(x => _mapper.Map<EquipmentCustomModel>(x)).ToList();
        }

        public List<EquipmentMaster> GetEquipmentDataByMasterId(int id)
        {
            var lst = _repository.Where(x => x.EquipmentMasterId == id).ToList();
            return lst;
        }

        public int GetFacilityStructureIdByEquipmentMasterId(int equipmentMasterId)
        {
            var structureId = _repository.Where(x => x.EquipmentMasterId == equipmentMasterId).Max(x => x.FacilityStructureId);
            return structureId;
        }

        public bool CheckEquipmentInScheduling(int equipmentId)
        {
            var list = _sRepository.Where(x => x.EquipmentAssigned == equipmentId).FirstOrDefault();
            return list != null ? true : false;
        }

    }
}
