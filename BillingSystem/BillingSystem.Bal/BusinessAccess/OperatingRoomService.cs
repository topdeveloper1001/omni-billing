using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class OperatingRoomService : IOperatingRoomService
    {
        private readonly IRepository<OperatingRoom> _repository;
        private readonly IMapper _mapper;

        public OperatingRoomService(IRepository<OperatingRoom> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the operating rooms list.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<OperatingRoomCustomModel> GetOperatingRoomsList(int type, int encounterId, int patientId)
        {
            var list = new List<OperatingRoomCustomModel>();
            var mList = encounterId > 0
                ? _repository.Where(
                    g =>
                        g.OperatingType == type && g.EncounterId == encounterId && g.PatientId == patientId &&
                        g.IsDeleted == false)
                    .ToList()
                : _repository.Where(g => g.OperatingType == type && g.PatientId == patientId && g.IsDeleted == false)
                    .ToList();
            list.AddRange(mList.Select(item => _mapper.Map<OperatingRoomCustomModel>(item)));
            return list;
        }

        /// <summary>
        /// Saves the operating room data.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<OperatingRoomCustomModel> SaveOperatingRoomData(OperatingRoom model)
        {
            if (model.Id > 0)
            {
                var current = _repository.GetSingle(model.Id);
                if (current != null)
                {
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                }
                _repository.UpdateEntity(model, model.Id);
            }
            else
                _repository.Create(model);

            ApplySurguryChargesToBill(Convert.ToInt32(model.CorporateID), Convert.ToInt32(model.FacilityId),
                Convert.ToInt32(model.EncounterId), string.Empty,
                0);
            var list = GetOperatingRoomsList(Convert.ToInt32(model.OperatingType),
                Convert.ToInt32(model.EncounterId),
                Convert.ToInt32(model.PatientId));
            return list;
        }

        /// <summary>
        /// Gets the operating room detail.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public OperatingRoom GetOperatingRoomDetail(int id)
        {
            var result = _repository.Where(g => g.Id == id).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Checks the duplicate record.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool CheckDuplicateRecord(OperatingRoom model)
        {
            var isExist = false;
            isExist = model.Id > 0
                ? _repository.Where(
                    o => o.Id != model.Id && o.PatientId == model.PatientId && o.EncounterId == model.EncounterId &&
                         o.IsDeleted == false && o.OperatingType == model.OperatingType &&
                         o.StartDay.HasValue && o.EndDay.HasValue && o.StartDay.Value == model.StartDay.Value &&
                         DateTime.Compare(o.EndDay.Value, model.EndDay.Value) == 0).Any()
                : _repository.Where(
                    o =>
                        o.IsDeleted == false && o.OperatingType == model.OperatingType &&
                        o.PatientId == model.PatientId && o.EncounterId == model.EncounterId &&
                        o.StartDay.HasValue && o.EndDay.HasValue
                        && o.StartDay.Value == model.StartDay.Value && o.EndDay.Value == model.EndDay.Value).Any();
            return isExist;
        }

        /// <summary>
        /// Applies the surgury charges to bill.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="reclaimFlag">The reclaim flag.</param>
        /// <param name="claimId">The claim identifier.</param>
        /// <returns></returns>
        public bool ApplySurguryChargesToBill(int corporateId, int facilityId, int encounterId, string reclaimFlag, Int64 claimId)
        {
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pEncounterID", encounterId);
            sqlParameters[3] = new SqlParameter("pReClaimFlag", reclaimFlag);
            sqlParameters[4] = new SqlParameter("pClaimId", claimId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_ApplySurguryChargesToBill.ToString(), sqlParameters);
            return true; 
        }
    }
}
