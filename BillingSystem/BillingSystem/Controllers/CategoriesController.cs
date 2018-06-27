// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoriesController.cs" company="">
//   OmniHealthcare
// </copyright>
// <summary>
//   Categories and Subcategories controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System.Web.Mvc;
    using Bal.BusinessAccess;
    using Common;
    using Model;
    using Models;

    /// <summary>
    /// Categories controller.
    /// </summary>
    public class CategoriesController : BaseController
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the Categories list
        /// </summary>
        /// <returns>action result with the partial view containing the AppointmentTypes list object</returns>
        [HttpPost]
        public ActionResult BindCategoriesList()
        {
            
            // Initialize the AppointmentTypes BAL object
            using (var categoriesBal = new CategoriesBal())
            {
                // Get categories list
                var cateogiresList = categoriesBal.GetCategoriesData();


                // Pass the ActionResult with List of CategoriesViewModel object to Partial View CategoriesList
                return PartialView(PartialViews.CategoriesList, cateogiresList);
            }
        }


        
        /// <summary>
        /// Delete the current Category based on the Category ID passed in the CategoriesModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteCategory(int id)
        {
            using (var bal = new CategoriesBal())
            {
                // Get currentCategory model object by current currentCategory ID
                var currentCategory = bal.GetCategoryById(id);

                // Check If currentCategory model is not null
                if (currentCategory != null)
                {

                    // Update Operation of current currentCategory
                    int result = bal.DeleteCategoriesData(currentCategory);


                    // return deleted ID of current Category as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Get the details of the current Category in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetCategory(int id)
        {
            using (var bal = new CategoriesBal())
            {
                // Call the AddCategories Method to Add / Update current Categories
                Categories currentCategory = bal.GetCategoryById(id);

                // Pass the ActionResult with the current CategoriesViewModel object as model to PartialView CategoriesAddEdit
                return PartialView(PartialViews.CategoriesAddEdit, currentCategory);
            }
        }

        /// <summary>
        /// Get the details of the Categories View in the Model Categories such as CategoriesList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model Categories to be passed to View
        ///     Categories
        /// </returns>
        public ActionResult CategoriesMain()
        {

            // Initialize the Categories BAL object
            var categoriesBal = new CategoriesBal();

            // Get the Entity list
            var categoriesList = categoriesBal.GetCategoriesData();

            // Intialize the View Model i.e. CategoriesView which is binded to Main View Index.cshtml under Categories
            var cateogiresView = new CategoriesView
            {
                CategoriesList = categoriesList,
                CurrentCategory = new Categories()
            };

            // Pass the View Model in ActionResult to View Categories
            return View(cateogiresView);
        }

        /// <summary>
        /// Reset the Categories View Model and pass it to CategoriesAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetCategoriesForm()
        {
            // Intialize the new object of AppointmentTypes ViewModel
            var categoriesViewModel = new Categories();

            // Pass the View Model as CategoriesViewModel to PartialView CategoriesAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.CategoriesAddEdit, categoriesViewModel);
        }

        /// <summary>
        /// Add New or Update the Categories based on if we pass the Categories ID in the CategoriesViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of Categories row
        /// </returns>
        public ActionResult SaveCategories(Categories model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new CategoriesBal())
                {
                    var isExist = bal.CheckDuplicateCategory(model.Id, model.ProdCatNumber, model.ProdCat);

                    if (isExist)
                        return Json("-1");

                    if (model.Id > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    }
                    else
                    {
                        model.CraetedDate = Helpers.GetInvariantCultureDateTime();
                        model.CreatedBy = userId;
                    }

                    // Call the AddCategories Method to Add / Update current Categories
                    newId = bal.SaveCategories(model);
                }
            }

            return Json(newId);
        }


        public ActionResult GetCategoriesData(int id)
        {

            using (var category = new CategoriesBal())
            {

                var currentCategory = category.GetCategoryById(id);
                var jsonResult = new
                {
                    currentCategory.Id,
                    currentCategory.ProdCatNumber,
                    currentCategory.ProdCat,
                    currentCategory.ProdSubcat,
                    currentCategory.ProdSubcat2,
                    currentCategory.ProdSubcat3

                };
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }


        }
        

        #endregion
    }
}
