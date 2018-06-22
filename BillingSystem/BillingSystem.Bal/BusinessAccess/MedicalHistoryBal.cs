using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Bal.Mapper;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MedicalHistoryBal : BaseBal
    {
        private MedicalHistoryMapper MedicalHistoryMapper { get; set; }

        public MedicalHistoryBal(string drugTableNumber)
        {
            DrugTableNumber = drugTableNumber;
            MedicalHistoryMapper = new MedicalHistoryMapper(DrugTableNumber);
        }

        public MedicalHistoryBal()
        {
            MedicalHistoryMapper = new MedicalHistoryMapper(DrugTableNumber);
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MedicalHistoryCustomModel> GetMedicalHistory(int patientId, int encounterId)
        {
            var list = new List<MedicalHistoryCustomModel>();
            using (var rep = UnitOfWork.MedicalHistoryRepository)
            {
                var model = rep.Where(a => a.IsDeleted != true && a.PatientId == patientId).ToList();
                if (model.Any())
                    list.AddRange(model.Select(item => MedicalHistoryMapper.MapModelToViewModel(item)));
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUpdateMedicalHistory(MedicalHistory model)
        {
            using (var rep = UnitOfWork.MedicalHistoryRepository)
            {
                var id = model.Id;
                if (id > 0)
                {
                    var current = rep.Where(m => m.Id == id).FirstOrDefault();
                    if (current != null)
                    {
                        model.CreatedBy = current.CreatedBy;
                        model.CreatedDate = current.CreatedDate;
                        if (model.IsDeleted == true)
                        {
                            current.DeletedBy = model.DeletedBy;
                            current.DeletedDate = model.DeletedDate;
                            current.IsDeleted = true;
                            model = current;
                        }
                        rep.UpdateEntity(model, model.Id);
                    }
                }
                else
                    rep.Create(model);
                return model.Id;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public MedicalHistoryCustomModel GetMedicalHistoryById(int id)
        {
            var vm = new MedicalHistoryCustomModel();
            using (var rep = UnitOfWork.MedicalHistoryRepository)
            {
                var model = rep.Where(x => x.Id == id).FirstOrDefault();
                if (model != null)
                    vm = MedicalHistoryMapper.MapModelToViewModel(model);
            }
            return vm;
        }

        public AlergyView GetAlergyAndMedicalHistorDataOnLoad(long patientId, long userId, long encounterId = 0)
        {
            using (var rep = UnitOfWork.MedicalHistoryRepository)
                return rep.GetPatientHistoryAndAlleryData(patientId, encounterId, userId);
        }
    }
}

