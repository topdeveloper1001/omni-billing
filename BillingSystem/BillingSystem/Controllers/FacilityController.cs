using BillingSystem.Models;
using System.Web.Mvc;
using BillingSystem.Common;
using BillingSystem.Model;
using System;
using BillingSystem.Model.CustomModel;
using System.Collections.Generic;
using Unity;

namespace BillingSystem.Controllers
{
    using System.Linq;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common.Common;
    using Hangfire;

    public class FacilityController : BaseController
    {
        private readonly IFacilityService _service;

        public FacilityController(IFacilityService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get the details of the Facility View in the Model Facility such as FacilityList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model Facility to be passed to View Facility
        /// </returns>
        public ActionResult Index()
        {
            //Get the facilities list
            var cId = Helpers.GetDefaultCorporateId();
            //var facilityList = _service.GetFacilities(cId);
            var facilityList = _service.GetFacilityList(cId);

            //Intialize the View Model i.e. FacilityView which is binded to Main View Index.cshtml under Facility
            var facilityView = new FacilityView
            {
                FacilityList = facilityList,
                CurrentFacility = new Facility() { CorporateID = Helpers.GetSysAdminCorporateID() }
            };

            //Pass the View Model in ActionResult to View Facility
            return View(facilityView);
        }

        /// <summary>
        /// Get List Of Facilities and convert to json string.
        /// </summary>
        /// <returns>
        /// json result with faclitiy lists.
        /// </returns>
        public JsonResult GetFListJson()
        {
            var cId = Helpers.GetDefaultCorporateId();
            //var facilityList = _service.GetFacilities(cId);
            var facilityList = _service.GetFacilityList(cId);
            var jsonResult = new
            {
                aaData = facilityList.Select(f => new[] {
                f.FacilityName, f.CorporateName, f.FacilityNumber, f.FacilityStreetAddress,
                Convert.ToString(f.FacilityZipCode.GetValueOrDefault()), f.FacilityLicenseNumber,
                Convert.ToString(f.FacilityLicenseNumberExpire.GetValueOrDefault()),
                 f.FacilityId.ToString()})
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Bind all the Facility list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the facility list object
        /// </returns>
        public ActionResult BindFaciltyList()
        {
            //Get the facilities list
            var cId = Helpers.GetDefaultCorporateId();
            //var facilityList = _service.GetFacilities(cId);
            var facilityList = _service.GetFacilityList(cId);
            //Pass the ActionResult with List of FacilityViewModel object to Partial View FacilityList
            return PartialView(PartialViews.FacilityList, facilityList);
        }

        /// <summary>
        /// Get the details of the current facility in the view model by ID
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        /// 
        //[AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetFacility(int facilityId)
        {
            //Call the AddFacility Method to Add / Update current facility
            var currentFacility = _service.GetFacilityById(facilityId);
            var jsonResult = new
            {
                currentFacility.FacilityId,
                currentFacility.FacilityNumber,
                currentFacility.FacilityName,
                currentFacility.FacilityStreetAddress,
                currentFacility.FacilityStreetAddress2,
                currentFacility.FacilityCity,
                currentFacility.FacilityZipCode,
                currentFacility.FacilityLicenseNumber,
                FacilityLicenseNumberExpire = currentFacility.FacilityLicenseNumberExpire.GetShortDateString1(),
                currentFacility.FacilityTypeLicense,
                currentFacility.FacilityRelated,
                currentFacility.FacilityTotalStaffedBed,
                currentFacility.FacilityTotalLicenseBed,
                currentFacility.FacilityPOBox,
                currentFacility.CountryID,
                currentFacility.FacilityState,
                currentFacility.IsActive,
                currentFacility.CorporateID,
                currentFacility.FacilityTimeZone,
                currentFacility.RegionId,
                currentFacility.IsDeleted,
                currentFacility.SenderID,
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add New or Update the facility based on if we pass the Facility ID in the FacilityViewModel object.
        /// </summary>
        /// <param name="model">pass the details of facility in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of facility row
        /// </returns>
        public ActionResult SaveFacility(Facility model)
        {
            var list = new List<FacilityCustomModel>();
            var input = model.FacilityTimeZone;
            TimeZoneInfo cst = TimeZoneInfo.FindSystemTimeZoneById(input);

            var test = cst.DisplayName;
            string sub = test.Substring(4, 6);

            model.TimeZ = sub;
            //Check if FacilityViewModel 
            if (model != null)
            {
                var fId = model.FacilityId;

                var status = _service.CheckDuplicateFacilityNoAndLicenseNo(model.FacilityNumber, model.FacilityLicenseNumber,
                model.FacilityId, model.CorporateID.Value);

                if (status > 0)
                    return Json(status, JsonRequestBehavior.AllowGet);

                if (model.FacilityId > 0)
                {
                    model.ModifiedBy = Helpers.GetLoggedInUserId();
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    model.CreatedBy = Helpers.GetLoggedInUserId();
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();


                }


                //Call the AddFacility Method to Add / Update current facility
                list = _service.AddUpdateFacility(model, out fId);

                if (fId > 0)
                {
                    BackgroundJob.Enqueue(() => CreateDefaultFacilityItems(fId, model.FacilityName, Helpers.GetLoggedInUserId()));
                }
            }
            return PartialView(PartialViews.FacilityList, list);
        }

        /// <summary>
        /// Get the details of the current facility in the view model by ID
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        /// 


        /// <summary>
        /// Delete the current facility based on the Facility ID passed in the SharedViewModel
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public ActionResult DeleteFacility(int facilityId)
        {
            var list = new List<FacilityCustomModel>();
            /*bool isExists;
            using (var facilityRoleBal = new FacilityRoleBal())
                isExists = facilityRoleBal.CheckFacilityExist(facilityId);

            if (isExists)
                return Json(1, JsonRequestBehavior.AllowGet);

            using (var _service = new FacilityBal())
            {
                //Get facility model object by current facility ID
                var currentFacility = _service.GetFacilityById(facilityId);

                //Check If facility model is not null
                if (currentFacility != null)
                {
                    currentFacility.IsDeleted = true;
                    currentFacility.DeletedBy = Helpers.GetLoggedInUserId();
                    currentFacility.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current facility
                    list = _service.AddUpdateFacility(currentFacility);
                }
            }*/
            var cId = Helpers.GetDefaultCorporateId();
            _service.DeleteFacilityData(Convert.ToString(facilityId));
            list = _service.GetFacilityList(cId);


            //var oFacilityBal = new FacilityBal();
            //var currentFacility = oFacilityBal.GetFacilityById(facilityId);
            //oFacilityBal.DeleteFacilityData(Convert.ToString(facilityId));


            //list = oFacilityBal.GetFacilityList(Convert.ToInt32(currentFacility.CorporateID));

            return PartialView(PartialViews.FacilityList, list);
        }

        //function to Get Facilities by corporate without Countries .
        /// <summary>
        /// Gets the corporate facilities.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetCorporateFacilities(int Id)
        {
            var result = _service.GetFacilitiesByCorporateIdWithoutCountries(Id).ToList().OrderBy(x => x.FacilityName).ToList();
            return Json(result);
        }

        public static void CreateDefaultFacilityItems(int fId, string fName, int userId)
        {

            var container = UnityConfig.RegisterComponents();
            var service = container.Resolve<IFacilityService>();
            service.CreateDefaultFacilityItems(fId, fName, userId);
        }
        [AcceptVerbs(HttpVerbs.Post)]
         public ActionResult GetFacilityNameById(string facilityNumber)
        {
            if (string.IsNullOrEmpty(facilityNumber))
            {
                if (Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    facilityNumber = session.FacilityNumber;
                }
            }
            var name = _service.GetFacilityNameByNumber(facilityNumber);
            return Json(name);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetFacilitiesbyCorporate(int corporateid)
        {
            var finalList = new List<DropdownListData>();
            var list = _service.GetFacilitiesByCorporateId(corporateid).ToList().OrderBy(x => x.FacilityName).ToList();
            if (list.Count > 0)
            {
                var facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : Helpers.GetDefaultFacilityId();
                if (facilityId > 0 && corporateid > 0)
                    list = list.Where(f => f.FacilityId == facilityId).ToList();

                finalList.AddRange(list.Select(item => new DropdownListData
                {
                    Text = item.FacilityName,
                    Value = Convert.ToString(item.FacilityId)
                }));
            }
            return Json(finalList);
        }
        public ActionResult GetFacilitiesDropdownDataWithFacilityNumber(int? corporateId)
        {
            var facilities = _service.GetFacilities(Convert.ToInt32(corporateId));
            if (facilities.Count > 0)
            {
                var list = new List<SelectListItem>();
                list.AddRange(facilities.Select(item => new SelectListItem
                {
                    Text = item.FacilityName,
                    Value = item.FacilityNumber,
                }));
                return Json(list);
            }
            return Json(null);
        }
        public ActionResult GetFacilitiesWithoutCorporateDropdownData()
        {
            var cId = Helpers.GetDefaultCorporateId();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var facilities = userisAdmin ? _service.GetFacilitiesWithoutCorporateFacility(cId) : _service.GetFacilitiesWithoutCorporateFacility(cId, Helpers.GetDefaultFacilityId());
            if (facilities.Any())
            {
                var list = new List<SelectListItem>();
                list.AddRange(facilities.Select(item => new SelectListItem
                {
                    Text = item.FacilityName,
                    Value = Convert.ToString(item.FacilityId),
                }));
                return Json(list);
            }

            return Json(null);
        }
        public ActionResult GetFacilitiesDropdownData()
        {
            var cId = Helpers.GetDefaultCorporateId();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var facilities = userisAdmin ? _service.GetFacilities(cId) : _service.GetFacilities(cId, Helpers.GetDefaultFacilityId());
            if (facilities.Any())
            {
                var list = new List<SelectListItem>();
                list.AddRange(facilities.Select(item => new SelectListItem
                {
                    Text = item.FacilityName,
                    Value = Convert.ToString(item.FacilityId),
                }));

                list = list.OrderBy(f => f.Text).ToList();
                return Json(list);
            }
            return Json(null);
        }
        public ActionResult GetFacilitiesDropdownDataOnScheduler()
        {
            var cId = Helpers.GetSysAdminCorporateID();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var facilities = userisAdmin ? _service.GetFacilities(cId) : _service.GetFacilities(cId, Helpers.GetDefaultFacilityId());
            if (facilities.Any())
            {
                var list = new List<SelectListItem>();
                list.AddRange(facilities.Select(item => new SelectListItem
                {
                    Text = item.FacilityName,
                    Value = Convert.ToString(item.FacilityId),
                }));

                list = list.OrderBy(f => f.Text).ToList();
                return Json(list);
            }
            return Json(null);
        }
        /// <summary>
        /// Gets the facilty list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFaciltyListTreeView()
        {
            // Get the facilities list
            var cId = Helpers.GetSysAdminCorporateID();
            var facilityList = _service.GetFacilityList(cId);
            var viewpath = string.Format("../Scheduler/{0}", PartialViews.LocationListView);
            return PartialView(viewpath, facilityList);
        }

    }
}
