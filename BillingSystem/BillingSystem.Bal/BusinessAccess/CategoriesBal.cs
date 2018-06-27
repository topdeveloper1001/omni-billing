using System;

namespace BillingSystem.Bal.BusinessAccess
{
    using System.Collections.Generic;
    using System.Linq;

    using Mapper;
    using Model;
    using Model.CustomModel;

    /// <summary>
    /// The categories and subcategories bal.
    /// </summary>
    public class CategoriesBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesBal"/> class.
        /// </summary>
        public CategoriesBal()
        {
            
        }

        #endregion

        
        #region Public Methods and Operators

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The <see cref="Categories" />.
        /// </returns>
        public Categories GetCategoryById(int? id)
        {
            using (var rep = UnitOfWork.CategoriesRepository)
            {
                var model = rep.Where(x => x.Id == id).FirstOrDefault();
                return model;
            }
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
            using (var rep = UnitOfWork.CategoriesRepository)
            {
                if (model.Id > 0)
                {
                    var current = rep.GetSingle(model.Id);
                    model.CreatedBy = current.CreatedBy;
                    model.CraetedDate = current.CraetedDate;
                    rep.UpdateEntity(model, model.Id);
                }
                else
                {
                    rep.Create(model);
                }

                return model.Id;
            }
        }

        public int DeleteCategoriesData(Categories model)
        {
            using (var rep = UnitOfWork.CategoriesRepository)
            {
                if (model.Id > 0)
                {
                    rep.Delete(model);
                }
                return model.Id;

            }
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
            using (var rep = UnitOfWork.CategoriesRepository)
            {
                var isExists =
                    rep.Where(model => model.Id != id && model.ProdCatNumber.Trim().ToLower().Equals(prodCatNum) && model.ProdCat.Trim().ToLower().Equals(prodCat))
                        .Any();
                return isExists;
            }
        }

        
        /// <summary>
        /// Gets the appointment types data.
        /// </summary>
        
        /// <returns></returns>
        public List<Categories> GetCategoriesData()
        {
            using (var rep = UnitOfWork.CategoriesRepository)
            {
                var list = rep.GetCategoriesData().ToList();
                return list;
            }
        }

        
        #endregion
    }
}
