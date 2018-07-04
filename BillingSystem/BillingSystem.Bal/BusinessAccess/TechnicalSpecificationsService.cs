using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{

    /// <summary>
    /// The Categories bal.
    /// </summary>
    public class TechnicalSpecificationsService : ITechnicalSpecificationsService
    {

        private readonly IRepository<TechnicalSpecifications> _repository;
        private readonly BillingEntities _context;

        public TechnicalSpecificationsService(IRepository<TechnicalSpecifications> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The <see cref="TechnicalSpecifications" />.
        /// </returns>
        public TechnicalSpecifications GetTechnicalSpecificationById(int id)
        {
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        public int SaveTechnicalSpecifications(TechnicalSpecifications model)
        {
            if (model.Id > 0)
            {
                _repository.Update(model, model.Id);
            }
            else
            {
                _repository.Create(model);
            }

            return model.Id;

        }

        public int DeleteTechnicalSpecificationsData(TechnicalSpecifications model)
        {
            if (model.Id > 0)
            {
                _repository.Delete(model);
            }
            return model.Id;

        }

        /// <summary>
        /// Checks the type of the duplicate appointment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="itemID">The Item number.</param>
        /// <returns></returns>
        public bool CheckDuplicateTechnicalSpecification(int id, long itemID, int? corporateId, int? facilityId)
        {
            var isExists = _repository.Where(model => model.Id != id && model.ItemID == itemID
            && model.CorporateId == corporateId && model.FacilityId == facilityId)
                    .Any();
            return isExists;
        }

        /// <summary>
        /// Gets the technical specifications by facility identifier.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<TechnicalSpecifications> GetTechnicalSpecificationsByFacilityId(int facilityId)
        {
            var list = _repository.Where(x => x.FacilityId == facilityId).ToList();
            return list;

        }

        /// <summary>
        /// Gets the TechnicalSpecifications data.
        /// </summary>
        /// <returns></returns>
        public List<TechnicalSpecificationsCustomModel> GetTechnicalSpecificationsData(int corporateId, int facilityId)
        {
            //var spName = string.Format("EXEC {0} @FacilityId,@CorporateId", StoredProcedures.SprocGetTechnicalSpecifications);
            var sqlParameters = new SqlParameter[2];

            sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("CorporateId", corporateId);


            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetTechnicalSpecifications.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<TechnicalSpecificationsCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }

            
            
        }

        #endregion
    }
}
