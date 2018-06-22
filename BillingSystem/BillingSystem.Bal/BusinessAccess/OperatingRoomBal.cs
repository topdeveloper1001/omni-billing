using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class OperatingRoomBal : BaseBal
    {

        /// <summary>
        /// Gets or sets the operating room mapper.
        /// </summary>
        /// <value>
        /// The operating room mapper.
        /// </value>
        private OperatingRoomMapper OperatingRoomMapper { get; set; }

        public OperatingRoomBal(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber))
                CptTableNumber = cptTableNumber;

            if (!string.IsNullOrEmpty(serviceCodeTableNumber))
                ServiceCodeTableNumber = serviceCodeTableNumber;

            if (!string.IsNullOrEmpty(drgTableNumber))
                DrgTableNumber = drgTableNumber;

            if (!string.IsNullOrEmpty(drugTableNumber))
                DrugTableNumber = drugTableNumber;

            if (!string.IsNullOrEmpty(hcpcsTableNumber))
                HcpcsTableNumber = hcpcsTableNumber;

            if (!string.IsNullOrEmpty(diagnosisTableNumber))
                DiagnosisTableNumber = diagnosisTableNumber;

            OperatingRoomMapper = new OperatingRoomMapper(CptTableNumber, ServiceCodeTableNumber, DrgTableNumber,
                DrugTableNumber, HcpcsTableNumber, DiagnosisTableNumber);
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
            using (var rep = UnitOfWork.OperatingRoomRepository)
            {
                var mList = encounterId > 0
                    ? rep.Where(
                        g =>
                            g.OperatingType == type && g.EncounterId == encounterId && g.PatientId == patientId &&
                            g.IsDeleted == false)
                        .ToList()
                    : rep.Where(g => g.OperatingType == type && g.PatientId == patientId && g.IsDeleted == false)
                        .ToList();
                list.AddRange(mList.Select(item => OperatingRoomMapper.MapModelToViewModel(item)));
                return list;
            }
        }

        /// <summary>
        /// Saves the operating room data.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<OperatingRoomCustomModel> SaveOperatingRoomData(OperatingRoom model)
        {
            using (var rep = UnitOfWork.OperatingRoomRepository)
            {
                if (model.Id > 0)
                {
                    var current = rep.GetSingle(model.Id);
                    if (current != null)
                    {
                        model.CreatedBy = current.CreatedBy;
                        model.CreatedDate = current.CreatedDate;
                    }
                    rep.UpdateEntity(model, model.Id);
                }
                else
                    rep.Create(model);

            }
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
            using (var rep = UnitOfWork.OperatingRoomRepository)
            {
                var result = rep.Where(g => g.Id == id).FirstOrDefault();
                return result;
            }
        }

        /// <summary>
        /// Checks the duplicate record.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool CheckDuplicateRecord(OperatingRoom model)
        {
            using (var rep = UnitOfWork.OperatingRoomRepository)
            {
                var isExist = false;
                isExist = model.Id > 0
                    ? rep.Where(
                        o => o.Id != model.Id && o.PatientId == model.PatientId && o.EncounterId == model.EncounterId &&
                             o.IsDeleted == false && o.OperatingType == model.OperatingType &&
                             o.StartDay.HasValue && o.EndDay.HasValue && o.StartDay.Value == model.StartDay.Value &&
                             DateTime.Compare(o.EndDay.Value, model.EndDay.Value) == 0).Any()
                    : rep.Where(
                        o =>
                            o.IsDeleted == false && o.OperatingType == model.OperatingType &&
                            o.PatientId == model.PatientId && o.EncounterId == model.EncounterId &&
                            o.StartDay.HasValue && o.EndDay.HasValue
                            && o.StartDay.Value == model.StartDay.Value && o.EndDay.Value == model.EndDay.Value).Any();
                return isExist;
            }
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
            using (var rep = UnitOfWork.OperatingRoomRepository)
            {
                var result = rep.ApplySurguryChargesToBill(encounterId, corporateId, facilityId, reclaimFlag, claimId);
                return result;
            }
        }
    }
}
