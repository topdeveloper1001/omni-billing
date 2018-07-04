using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Models;
namespace BillingSystem.Controllers
{
    using System.Web.Mvc;
    using Bal.BusinessAccess;
    using Common;
    using Model;
    using Models;

    /// <summary>
    /// Catalog controller.
    /// </summary>
    public class CatalogController : BaseController
    {

        private readonly ICatalogService _service;

        public CatalogController(ICatalogService service)
        {
            _service = service;
        }
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the Catalog list
        /// </summary>
        /// <returns>action result with the partial view containing the Catalog list object</returns>
        [HttpPost]
        public ActionResult BindCatalogList()
        {

            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            // Get Catalog list
            var cateogiresList = _service.GetCatalogData(corporateId, facilityId);


            // Pass the ActionResult with List of CatalogViewModel object to Partial View CatalogList
            return PartialView(PartialViews.CatalogList, cateogiresList);
            
        }


        
        /// <summary>
        /// Delete the current Catalog based on the Catalog ID passed in the CatalogModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteCatalog(int id)
        {
            
            // Get currentCatalog model object by current currentCatalog ID
            var currentCatalog = _service.GetCatalogById(id);

            // Check If currentCatalog model is not null
            if (currentCatalog != null)
            {

                // Update Operation of current currentCatalog
                int result = _service.DeleteCatalogData(currentCatalog);


                // return deleted ID of current Catalog as Json Result to the Ajax Call.
                return Json(result);
            }
            

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Get the details of the current Catalog in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetCatalog(int id)
        {
            
            // Call the AddCatalog Method to Add / Update current Catalog
            Catalog currentCatalog = _service.GetCatalogById(id);

            // Pass the ActionResult with the current CatalogViewModel object as model to PartialView CatalogAddEdit
            return PartialView(PartialViews.CatalogAddEdit, currentCatalog);
            
        }

        /// <summary>
        /// Get the details of the Catalog View in the Model Catalog such as CatalogList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model Catalog to be passed to View
        ///     Catalog
        /// </returns>
        public ActionResult CatalogMain()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            // Get the Entity list
            var catalogsList = _service.GetCatalogData(corporateId, facilityId);

            // Intialize the View Model i.e. CatalogView which is binded to Main View Index.cshtml under Catalog
            var catalogsView = new CatalogView
            {
                CatalogList = catalogsList,
                CurrentCatalog = new Catalog()
            };

            // Pass the View Model in ActionResult to View Catalog
            return View(catalogsView);
        }

        /// <summary>
        /// Reset the Catalog View Model and pass it to CatalogAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetCatalogForm()
        {
            // Intialize the new object of Catalog ViewModel
            var catalogsViewModel = new Catalog();

            // Pass the View Model as CatalogViewModel to PartialView CatalogAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.CatalogAddEdit, catalogsViewModel);
        }

        /// <summary>
        /// Add New or Update the Catalog based on if we pass the Catalog ID in the CatalogViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of Catalog row
        /// </returns>
        public ActionResult SaveCatalog(Catalog model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                if (model.Id > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    model.CreatedBy = userId;
                }

                // Call the AddCatalog Method to Add / Update current Catalog
                newId = _service.SaveCatalog(model);
                
            }

            return Json(newId);
        }


        public ActionResult GetCatalogData(int id)
        {

            var currentCatalog = _service.GetCatalogById(id);
            var jsonResult = new
            {
                currentCatalog.Id,
                currentCatalog.FacilityId,
                currentCatalog.CorporateId,
                currentCatalog.ItemID,
                currentCatalog.ManufacturerID,
                currentCatalog.ManufacturerName,
                currentCatalog.ProductID,
                currentCatalog.ProductName,
                currentCatalog.ProductDescription,
                currentCatalog.ManufacturerItemCode,
                currentCatalog.ItemDescription,
                currentCatalog.ItemImageURL,
                currentCatalog.VenderItemCode,
                currentCatalog.Pkg,
                currentCatalog.UnitPrice,
                currentCatalog.PriceDescription,
                currentCatalog.Availability,
                currentCatalog.CategoryPathID,
                currentCatalog.CategoryPathName,
                currentCatalog.PackingListDescritpion,
                currentCatalog.UnitWeight,
                currentCatalog.UnitVolume,
                currentCatalog.UOMFactor,
                currentCatalog.CountryOfOrigin,
                currentCatalog.HarmonizedTariffCode,
                currentCatalog.HazMatClass,
                currentCatalog.HazMatCode,
                currentCatalog.PharmacyProductType,
                currentCatalog.NationalDrugCode,
                currentCatalog.BrandID,
                currentCatalog.BrandName,
                currentCatalog.ProdCatNumber,
                currentCatalog.Size
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// Method is used to bind the technical specifications dropdown by passing facility id
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public ActionResult GetCatalogList(int facilityId)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            // Initialize the AppointmentTypes BAL object
            var catalogsList = _service.GetCatalogData(corporateId, facilityId);

            return Json(catalogsList, JsonRequestBehavior.AllowGet);

        }

        #endregion
    }
}
