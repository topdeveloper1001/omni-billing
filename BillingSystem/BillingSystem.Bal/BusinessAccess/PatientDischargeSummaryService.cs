using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;


namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientDischargeSummaryService : IPatientDischargeSummaryService
    {
        private readonly IRepository<PatientDischargeSummary> _repository;
        private readonly IRepository<PatientEvaluation> _peRepository;

        public PatientDischargeSummaryService(IRepository<PatientDischargeSummary> repository, IRepository<PatientEvaluation> peRepository)
        {
            _repository = repository;
            _peRepository = peRepository;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SavePatientDischargeSummary(PatientDischargeSummary model)
        {
            var current =
                _repository.Where(p => p.PatientId == model.PatientId && p.EncounterId == model.EncounterId)
                    .FirstOrDefault();
            if (current != null)
            {
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                model.Id = current.Id;
                _repository.UpdateEntity(model, model.Id);
            }
            else
                _repository.Create(model);
            return model.Id;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="encounterId">The patient discharge summary identifier.</param>
        /// <returns></returns>
        public PatientDischargeSummary GetPatientDischargeSummaryByEncounterId(int? encounterId)
        {
            var model = _repository.Where(x => x.PatientId == encounterId).FirstOrDefault();
            return model;
        }

        public List<PatientEvaluation> ListPatientEvaluation(int patientId, int encounterId, string globalCodeCategory, string setId)
        {
            var list = _peRepository.Where(i => i.PatientId == patientId && i.EncounterId == encounterId && i.CategoryValue == globalCodeCategory && i.ExternalValue1 == "1" && i.ExternalValue2.Trim().Equals(setId)).ToList();
            return list.Any() ? list : new List<PatientEvaluation>();
        }


        public List<PatientEvaluation> GetPatientEvaluationData(int patientId, int encounterId, IEnumerable<string> categories, string setId)
        {
            var list = _peRepository.Where(i => i.PatientId == patientId && i.EncounterId == encounterId && categories.Contains(i.CategoryValue) && i.ExternalValue1.Equals("1") && i.ExternalValue2.Trim().Equals(setId)).ToList();
            return list.Any() ? list : new List<PatientEvaluation>();
        }
    }
}
