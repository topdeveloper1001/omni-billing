using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{

    /// <summary>
    /// The Categories bal.
    /// </summary>
    public class CategoriesService : ICategoriesService
    {

        private readonly IRepository<Categories> _repository;
        private readonly BillingEntities _context;
        
        public CategoriesService(IRepository<Categories> repository, BillingEntities context)
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
        /// The <see cref="Categories" />.
        /// </returns>
        public Categories GetCategoryById(int id)
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
        public int SaveCategories(Categories model)
        {
            if (model.Id > 0)
            {
                var current = _repository.GetSingle(model.Id);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.Update(model, model.Id);
            }
            else
            {
                _repository.Create(model);
            }

            return model.Id;

        }

        public int DeleteCategoriesData(Categories model)
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
        /// <param name="prodCatNum">The category number.</param>
        /// <param name="prodCat">The category name.</param>
        /// <returns></returns>
        public bool CheckDuplicateCategory(int id, string prodCatNum, string prodCat)
        { 
                var isExists = _repository.Where(model => model.Id != id && model.ProdCatNumber.Trim().ToLower().Equals(prodCatNum) && model.ProdCat.Trim().ToLower().Equals(prodCat))
                        .Any();
                return isExists;
             
        }


        /// <summary>
        /// Gets the appointment types data.
        /// </summary>

        /// <returns></returns>
        public List<CategoriesCustomModel> GetCategoriesData()
        {
            var spName = string.Format("EXEC {0}", StoredProcedures.SprocGetCategories);
            
            IEnumerable<CategoriesCustomModel> result = _context.Database.SqlQuery<CategoriesCustomModel>(spName);
                                    
            return result.ToList();
            
            //return _repository.GetAll().ToList();

        }
        
        #endregion
    }
}
