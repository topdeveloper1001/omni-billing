using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientInfoChangesQueueService : IPatientInfoChangesQueueService
    {
        private readonly IRepository<PatientInfoChangesQueue> _repository;

        public PatientInfoChangesQueueService(IRepository<PatientInfoChangesQueue> repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<PatientInfoChangesQueueCustomModel> GetPatientInfoChangesQueueList()
        {
            var list = new List<PatientInfoChangesQueueCustomModel>();
            var lstPatientInfoChangesQueue = _repository.Where(a => a.IsActive == null || (bool)a.IsActive).ToList();
            if (lstPatientInfoChangesQueue.Count > 0)
            {
                list.AddRange(lstPatientInfoChangesQueue.Select(item => new PatientInfoChangesQueueCustomModel
                {

                }));
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
            if (model.Id > 0)
            {
                var current = _repository.GetSingle(model.Id);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.Id);
            }
            else
                _repository.Create(model);

            var currentId = model.Id;
            var list = GetPatientInfoChangesQueueList();
            return list != null && list.Any() ? list : new List<PatientInfoChangesQueueCustomModel>();
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="patientInfoChangesQueueId">The patient information changes queue identifier.</param>
        /// <returns></returns>
        public PatientInfoChangesQueue GetPatientInfoChangesQueueByID(int? patientInfoChangesQueueId)
        {
            var model = _repository.Where(x => x.Id == patientInfoChangesQueueId).FirstOrDefault();
            return model;
        }
    }
}
