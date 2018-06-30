using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MedicalNecessityService : IMedicalNecessityService
    {
        private readonly IRepository<MedicalNecessity> _repository;
        private readonly BillingEntities _context;

        public MedicalNecessityService(IRepository<MedicalNecessity> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MedicalNecessityCustomModel> GetMedicalNecessity(string DiagnosisTableNumber)
        {
            var spName = string.Format("EXEC {0} @TableNumber", StoredProcedures.SPROC_GetMedicalNecessityData);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@TableNumber", DiagnosisTableNumber);
            IEnumerable<MedicalNecessityCustomModel> result = _context.Database.SqlQuery<MedicalNecessityCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="medicalNecessityId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="MedicalNecessity" />.
        /// </returns>
        public MedicalNecessity GetMedicalNecessityById(int? medicalNecessityId)
        {
            MedicalNecessity model = _repository.Where(x => x.Id == medicalNecessityId).FirstOrDefault();
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
        public int SaveMedicalNecessity(MedicalNecessity model)
        {
            if (model.Id > 0)
                _repository.UpdateEntity(model, model.Id);
            else
                _repository.Create(model);

            return model.Id;

        }

        #endregion
    }
}
