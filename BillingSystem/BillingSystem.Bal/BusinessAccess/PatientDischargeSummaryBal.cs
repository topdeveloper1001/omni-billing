using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Repository.GenericRepository;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientDischargeSummaryBal : BaseBal
    {

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SavePatientDischargeSummary(PatientDischargeSummary model)
        {
            using (var rep = UnitOfWork.PatientDischargeSummaryRepository)
            {
                var current =
                    rep.Where(p => p.PatientId == model.PatientId && p.EncounterId == model.EncounterId)
                        .FirstOrDefault();
                if (current != null)
                {
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    model.Id = current.Id;
                    rep.UpdateEntity(model, model.Id);
                }
                else
                    rep.Create(model);
                return model.Id;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="encounterId">The patient discharge summary identifier.</param>
        /// <returns></returns>
        public PatientDischargeSummary GetPatientDischargeSummaryByEncounterId(int? encounterId)
        {
            using (var rep = UnitOfWork.PatientDischargeSummaryRepository)
            {
                var model = rep.Where(x => x.PatientId == encounterId).FirstOrDefault();
                return model;
            }
        }

        public List<PatientEvaluation> ListPatientEvaluation(int patientId, int encounterId, string globalCodeCategory, string setId)
        {
            List<PatientEvaluation> list;
            using (var rep = UnitOfWork.PatientEvaluationRepository)
            {
                list =
                    rep.Where(
                        i =>
                            i.PatientId == patientId && i.EncounterId == encounterId &&
                            i.CategoryValue == globalCodeCategory && i.ExternalValue1 == "1" && i.ExternalValue2.Trim().Equals(setId))
                        .ToList();
            }
            return list.Any() ? list : new List<PatientEvaluation>();
        }


        public List<PatientEvaluation> GetPatientEvaluationData(int patientId, int encounterId, IEnumerable<string> categories, string setId)
        {
            List<PatientEvaluation> list;
            using (var rep = UnitOfWork.PatientEvaluationRepository)
            {
                list =
                    rep.Where(
                        i => i.PatientId == patientId && i.EncounterId == encounterId &&
                             categories.Contains(i.CategoryValue) && i.ExternalValue1.Equals("1") &&
                             i.ExternalValue2.Trim().Equals(setId))
                        .ToList();
            }
            return list.Any() ? list : new List<PatientEvaluation>();
        }
    }
}
