using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
     public class PatientPreSchedulingService : IPatientPreSchedulingService
    {
        private readonly IRepository<PatientPreScheduling> _repository;

        public PatientPreSchedulingService(IRepository<PatientPreScheduling> repository)
        {
            _repository = repository;
        }


        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<PatientPreSchedulingCustomModel> GetPatientPreScheduling()
        {
            var list = new List<PatientPreSchedulingCustomModel>();
            var lstPatientPreScheduling = _repository.GetAll().ToList();
            if (lstPatientPreScheduling.Count > 0)
            {
                list.AddRange(lstPatientPreScheduling.Select(item => new PatientPreSchedulingCustomModel()));
            }

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="patientPreSchedulingId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="PatientPreScheduling" />.
        /// </returns>
        public PatientPreScheduling GetPatientPreSchedulingById(int? patientPreSchedulingId)
        {
            var model = _repository.Where(x => x.PatientPreSchedulingId == patientPreSchedulingId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SavePatientPreScheduling(PatientPreScheduling model)
        {
            if (model.PatientPreSchedulingId > 0)
                _repository.UpdateEntity(model, model.PatientPreSchedulingId);
            else
                _repository.Create(model);

            return model.PatientPreSchedulingId;
        }


        #endregion
    }
}
