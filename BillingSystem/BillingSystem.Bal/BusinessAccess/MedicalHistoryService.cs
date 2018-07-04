using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using AutoMapper;

using System.Data.SqlClient;
using BillingSystem.Common.Common;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MedicalHistoryService : IMedicalHistoryService
    {
        private readonly IRepository<MedicalHistory> _repository;
        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public MedicalHistoryService(IRepository<MedicalHistory> repository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MedicalHistoryCustomModel> GetMedicalHistory(int patientId, int encounterId)
        {
            var list = new List<MedicalHistoryCustomModel>();
            var model = _repository.Where(a => a.IsDeleted != true && a.PatientId == patientId).ToList();
            if (model.Any())
                list.AddRange(model.Select(item => _mapper.Map<MedicalHistoryCustomModel>(item)));
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUpdateMedicalHistory(MedicalHistory model)
        {
            var id = model.Id;
            if (id > 0)
            {
                var current = _repository.Where(m => m.Id == id).FirstOrDefault();
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
                    _repository.UpdateEntity(model, model.Id);
                }
            }
            else
                _repository.Create(model);
            return model.Id;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public MedicalHistoryCustomModel GetMedicalHistoryById(int id)
        {
            var vm = new MedicalHistoryCustomModel();
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            if (model != null)
                vm = _mapper.Map<MedicalHistoryCustomModel>(model);
            return vm;
        }

        public AlergyView GetAlergyAndMedicalHistorDataOnLoad(long patientId, long userId, long encounterId = 0)
        {
            var vm = new AlergyView();

            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pPatientId", patientId);
            sqlParameters[1] = new SqlParameter("pEncounterId", encounterId);
            sqlParameters[2] = new SqlParameter("pUserId", userId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetMedicalHistoryAndAllergyData.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var mhList = ms.GetResultWithJson<MedicalHistoryCustomModel>(JsonResultsArray.MedicalHistory.ToString());

                vm.MedicalHistoryView = new MedicalHistoryView { CurrentMedicalHistory = new MedicalHistory(), MedicalHistoryList = mhList };
                vm.AllergiesHistoriesGCC = ms.GetResultWithJson<GlobalCodeCategory>(JsonResultsArray.GlobalCategory.ToString());
                vm.AllergiesHistoryGC = ms.GetResultWithJson<GlobalCodes>(JsonResultsArray.GlobalCode.ToString());
                vm.AlergyList = ms.GetResultWithJson<AlergyCustomModel>(JsonResultsArray.Allergy.ToString());
            }
            return vm;
        }
    }
}

