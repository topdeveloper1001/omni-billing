using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Repository.Common;

namespace BillingSystem.Bal.BusinessAccess
{

    /// <summary>
    /// The Categories bal.
    /// </summary>
    public class CatalogService : ICatalogService
    {

        private readonly IRepository<Catalog> _repository;
        private readonly BillingEntities _context;
        
        public CatalogService(IRepository<Catalog> repository, BillingEntities context)
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
        /// The <see cref="Catalog" />.
        /// </returns>
        public Catalog GetCatalogById(int id)
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
        public int SaveCatalog(Catalog model)
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

        public int DeleteCatalogData(Catalog model)
        {
            if (model.Id > 0)
            {
                _repository.Delete(model);
            }
            return model.Id;

        }

       
        /// <summary>
        /// Gets the technical specifications by facility identifier.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<Catalog> GetCatalogByFacilityId(int facilityId)
        {
            var list = _repository.Where(x => x.FacilityId == facilityId).ToList();
            return list;

        }

        /// <summary>
        /// Gets the Catalog data.
        /// </summary>

        /// <returns></returns>
        public List<CatalogCustomModel> GetCatalogData(int corporateId, int facilityId)
        {
            //var spName = string.Format("EXEC {0} @FacilityId,@CorporateId", StoredProcedures.SprocGetCatalog);
            var sqlParameters = new SqlParameter[2];

            sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("CorporateId", corporateId);


            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetCatalog.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<CatalogCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }

            
            
        }
        
        #endregion
    }
}
