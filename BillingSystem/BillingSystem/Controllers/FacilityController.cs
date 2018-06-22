using System.Net;
using BillingSystem.Models;
using System.Web.Mvc;
using BillingSystem.Common;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using System;
using BillingSystem.Model.CustomModel;
using System.Collections.Generic;
using Glimpse.Core.Configuration;

namespace BillingSystem.Controllers
{
    using System.Linq;
    using Hangfire;

    public class FacilityController : BaseController
    {
        /// <summary>
        /// Get the details of the Facility View in the Model Facility such as FacilityList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model Facility to be passed to View Facility
        /// </returns>
        public ActionResult Index()
        {
            //Initialize the Facility Communicator object
            var facilityBal = new FacilityBal();

            //Get the facilities list
            var cId = Helpers.GetDefaultCorporateId();
            //var facilityList = facilityBal.GetFacilities(cId);
            var facilityList = facilityBal.GetFacilityList(cId);

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
            var facilityBal = new FacilityBal();
            //Get the facilities list
            var cId = Helpers.GetDefaultCorporateId();
            //var facilityList = facilityBal.GetFacilities(cId);
            var facilityList = facilityBal.GetFacilityList(cId);
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
            //Initialize the Facility Communicator object
            using (var facilityBal = new FacilityBal())
            {
                //Get the facilities list
                var cId = Helpers.GetDefaultCorporateId();
                //var facilityList = facilityBal.GetFacilities(cId);
                var facilityList = facilityBal.GetFacilityList(cId);
                //Pass the ActionResult with List of FacilityViewModel object to Partial View FacilityList
                return PartialView(PartialViews.FacilityList, facilityList);
            }
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
            using (var facilityBal = new FacilityBal())
            {
                //Call the AddFacility Method to Add / Update current facility
                var currentFacility = facilityBal.GetFacilityById(facilityId);
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

                using (var bal = new FacilityBal())
                {
                    var status = bal.CheckDuplicateFacilityNoAndLicenseNo(model.FacilityNumber, model.FacilityLicenseNumber,
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
                    list = bal.AddUpdateFacility(model, out fId);

                    if (fId > 0)
                    {
                        BackgroundJob.Enqueue(() => CreateDefaultFacilityItems(fId, model.FacilityName, Helpers.GetLoggedInUserId()));
                    }
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

            using (var bal = new FacilityBal())
            {
                //Get facility model object by current facility ID
                var currentFacility = bal.GetFacilityById(facilityId);

                //Check If facility model is not null
                if (currentFacility != null)
                {
                    currentFacility.IsDeleted = true;
                    currentFacility.DeletedBy = Helpers.GetLoggedInUserId();
                    currentFacility.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current facility
                    list = bal.AddUpdateFacility(currentFacility);
                }
            }*/
            var facilityBal = new FacilityBal();
            var cId = Helpers.GetDefaultCorporateId();
            facilityBal.DeleteFacilityData(Convert.ToString(facilityId));
            list = facilityBal.GetFacilityList(cId);


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
            using (var facilityBal = new FacilityBal())
            {
                var result = facilityBal.GetFacilitiesByCorporateIdWithoutCountries(Id).ToList().OrderBy(x => x.FacilityName).ToList();
                return Json(result);
            }
        }

        public static void CreateDefaultFacilityItems(int fId, string fName, int userId)
        {
            using (var bal = new FacilityBal())
            {
                bal.CreateDefaultFacilityItems(fId, fName, userId);
            }
        }
    }
}
