using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientInfoChangesQueueBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<PatientInfoChangesQueueCustomModel> GetPatientInfoChangesQueueList()
        {
            var list = new List<PatientInfoChangesQueueCustomModel>();
            using (var PatientInfoChangesQueueRep = UnitOfWork.PatientInfoChangesQueueRepository)
            {
                var lstPatientInfoChangesQueue = PatientInfoChangesQueueRep.Where(a => a.IsActive == null || (bool)a.IsActive).ToList();
                if (lstPatientInfoChangesQueue.Count > 0)
                {
                    list.AddRange(lstPatientInfoChangesQueue.Select(item => new PatientInfoChangesQueueCustomModel
                    {

                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<PatientInfoChangesQueueCustomModel> SavePatientInfoChangesQueue(PatientInfoChangesQueue model)
        {
            using (var rep = UnitOfWork.PatientInfoChangesQueueRepository)
            {
                if (model.Id > 0)
                {
                    var current = rep.GetSingle(model.Id);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(model, model.Id);
                }
                else
                    rep.Create(model);

                var currentId = model.Id;
                var list = GetPatientInfoChangesQueueList();
            }
            return new List<PatientInfoChangesQueueCustomModel>();
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="patientInfoChangesQueueId">The patient information changes queue identifier.</param>
        /// <returns></returns>
        public PatientInfoChangesQueue GetPatientInfoChangesQueueByID(int? patientInfoChangesQueueId)
        {
            using (var rep = UnitOfWork.PatientInfoChangesQueueRepository)
            {
                var model = rep.Where(x => x.Id == patientInfoChangesQueueId).FirstOrDefault();
                return model;
            }
        }
    }
}
