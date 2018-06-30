using System.Collections.Generic;
using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using Hangfire;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using Unity;

namespace BillingSystem.Controllers
{
    public class CorporateController : BaseController
    {
        private readonly ICorporateService _service;
        private readonly ICountryService _cService;
        private readonly IUsersService _uService;

        public CorporateController(ICorporateService service, ICountryService cService, IUsersService uService)
        {
            _service = service;
            _cService = cService;
            _uService = uService;
        }

        /// <summary>
        /// Get the details of the Corporate View in the Model Corporate such as CorporateList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model Corporate to be passed to View Corporate
        /// </returns>
        [CheckRolesAuthorize("1")]
        public ActionResult Index()
        {
            //Get the Entity list
            var corporatId = Helpers.GetDefaultCorporateId();
            var corporateList = _service.GetCorporate(corporatId);
            //var corporateList = corporateBal.GetCorporate(corporatId);

            //Intialize the View Model i.e. CorporateView which is binded to Main View Index.cshtml under Corporate
            var corporateView = new CorporateView
            {
                CorporateList = corporateList,
                CurrentCorporate = new Model.Corporate()
            };

            //Pass the View Model in ActionResult to View Corporate
            return View(corporateView);
        }

        /// <summary>
        /// Bind all the Corporate list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the Corporate list object
        /// </returns>
        public ActionResult BindCorporateList()
        {
            //Get the corporate list
            var corporatId = Helpers.GetDefaultCorporateId();
            var corporateList = _service.GetCorporate(corporatId);

            //Pass the ActionResult with List of CorporateViewModel object to Partial View CorporateList
            return PartialView(PartialViews.CorporateList, corporateList);

        }

        public ActionResult GetCorporates()
        {
            //Get the corporate list
            var corporatId = Helpers.GetDefaultCorporateId();
            var corporateList = _service.GetCorporate(corporatId);

            //Pass the ActionResult with List of CorporateViewModel object to Partial View CorporateList
            return Json(new
            {
                iTotalRecords = corporateList.Count,
                iTotalDisplayRecords = corporateList.Count,
                aaData = corporateList.Select(x => new[] { x.CorporateID.ToString(), x.CorporateName, x.CorporateNumber, x.StreetAddress, x.Email, x.CorporateMainPhone })
            }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Add New or Update the Corporate based on if we pass the Corporate ID in the CorporateViewModel object.
        /// </summary>
        /// <param name="corporateModel">pass the details of Corporate in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of Corporate row
        /// </returns>
        public ActionResult SaveCorporate(Corporate corporateModel)
        {
            //Initialize the newId variable 
            var newId = -1;
            var isNew = corporateModel.CorporateID == 0;
            //Check if CorporateViewModel 
            if (corporateModel != null)
            {
                var isExist = _service.CheckDuplicateCorporateNumber(corporateModel.CorporateNumber,
                    corporateModel.CorporateID);
                if (isExist)
                    return Json("1");

                if (corporateModel.CorporateID > 0)
                {
                    corporateModel.ModifiedBy = Helpers.GetLoggedInUserId();
                    corporateModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    corporateModel.CreatedBy = Helpers.GetLoggedInUserId();
                    corporateModel.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }
                //Call the AddCorporate Method to Add / Update current Corporate
                newId = _service.AddUptdateCorporate(corporateModel);

                if (isNew && newId > 0)
                    BackgroundJob.Enqueue(() => CreateDefaultCorporateItems(corporateModel.CorporateID, corporateModel.CorporateName));

            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current Corporate in the view model by ID
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetCorporate(string Id)
        {
            var model = _service.GetCorporateById(Convert.ToInt32(Id));

            //Pass the ActionResult with the current CorporateViewModel object as model to PartialView CorporateAddEdit
            return PartialView(PartialViews.CorporateAddEdit, model);
        }
        public ActionResult GetCorporateById(string Id)
        {
            var currentCorporate = _service.GetCorporateById(Convert.ToInt32(Id));
            return Json(currentCorporate);

        }
        public bool CheckDefaultTableNumber(string defaultTableNumber, string corporateId)
        {
            return _service.CheckDefaultTableNumber(defaultTableNumber, Convert.ToInt32(corporateId));
        }
        public List<DropdownListData> GetCorporateList()
        {
            var list = new List<DropdownListData>();
            var cId = Helpers.GetDefaultCorporateId();
            var cList = _service.GetCorporateDDL(cId);
            list.AddRange(cList.Select(item => new DropdownListData
            {
                Text = item.CorporateName,
                Value = Convert.ToString(item.CorporateID)
            }));

            return list;
        }
        ///// <summary>
        ///// Delete the current Corporate based on the Corporate ID passed in the CorporateModel
        ///// </summary>
        ///// <param name="corporateModel">The corporate model.</param>
        ///// <returns></returns>
        //public ActionResult DeleteCorporate(Model.Corporate corporateModel)
        //{
        //    /*using (var corporateBal = new CorporateBal())
        //    {
        //        //Get Corporate model object by current Corporate ID
        //        var currentCorporate = corporateBal.GetCorporateById(Convert.ToInt32(corporateModel.CorporateID));

        //        //Check If Corporate model is not null
        //        if (currentCorporate != null)
        //        {
        //            currentCorporate.IsDeleted = true;
        //            currentCorporate.DeletedBy = Helpers.GetLoggedInUserId();
        //            currentCorporate.DeletedDate = Helpers.GetInvariantCultureDateTime();

        //            //Update Operation of current Corporate
        //            var result = corporateBal.AddUptdateCorporate(currentCorporate);

        //            //return deleted ID of current Corporate as Json Result to the Ajax Call.
        //            return Json(result);
        //        }
        //    }*/
        //    using (var corporateBal = new CorporateBal())
        //    {
        //        var value = corporateBal.DeleteCorporateData(Convert.ToString(corporateModel.CorporateID));
        //        return Json(value);
        //    }

        //    //Return the Json result as Action Result back JSON Call Success
        //}

        /// <summary>
        /// Deletes the corporate data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public JsonResult DeleteCorporateData(int corporateId)
        {
            if (Helpers.DefaultC != corporateId)
            {
                var value = _service.DeleteCorporateData(Convert.ToString(corporateId));
                return Json(value ? "1" : "0", JsonRequestBehavior.AllowGet);

            }
            return Json("-2", JsonRequestBehavior.AllowGet);

            //Return the Json result as Action Result back JSON Call Success
        }

        /// <summary>
        /// Reset the Corporate View Model and pass it to CorporateAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetCorporateForm()
        {
            //Intialize the new object of Corporate ViewModel
            var corporateViewModel = new Model.Corporate();

            //Pass the View Model as CorporateViewModel to PartialView CorporateAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.CorporateAddEdit, corporateViewModel);
        }

        //function to check if Corporate Exist assigned with role
        /// <summary>
        /// Checks the corporate exist.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public ActionResult CheckCorporateExist(int Id)
        {
            using (var facilityRoleBal = new FacilityRoleService())
            {
                var result = facilityRoleBal.CheckCorporateExist(Id);
                return Json(result);
            }

        }
        /// <summary>
        /// Method is used to check whethere user exists for passing corporate or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CheckUserExistForCorporate(int id)
        {
            var result = _uService.CheckUserExistForCorporate(id);
            return Json(result);

        }
        // Function to chek duplicate Corporate on the basis of username or Email
        /// <summary>
        /// Checks the duplicate corporate.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool CheckDuplicateCorporate(string name, int id)
        {
            return _service.CheckDuplicateCorporate(name, id);

        }

        public static void CreateDefaultCorporateItems(int cId, string cName)
        {
            var container = UnityConfig.RegisterComponents();
            var service = container.Resolve<ICorporateService>();
            service.CreateDefaultCorporateItem(cId, cName);
        }

        public JsonResult BindAllCorporateDataOnLoad(string typeId)
        {
            var tlist = new List<BillingCodeTableSet>();
            var plist = new List<CountryCustomModel>();
            tlist = _cService.GetTableNumbersList(typeId);
            plist = _cService.GetCountryWithCode();

            var jsonData = new
            {
                tlist,
                plist
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}
