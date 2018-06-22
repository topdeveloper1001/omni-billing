using BillingSystem.Bal.Mapper;
using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    public class ScrubEditTrackController : BaseController
    {
        /// <summary>
        /// Get the details of the ScrubEditTrack View in the Model ScrubEditTrack such as ScrubEditTrackList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model ScrubEditTrack to be passed to View ScrubEditTrack
        /// </returns>
        public ActionResult ScrubEditTrackMain()
        {
            //Initialize the ScrubEditTrack BAL object
            var scrubEditTrackBal = new ScrubEditTrackBal();

            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            //Get the Entity list
            var scrubEditTrackList = scrubEditTrackBal.GetScrubEditTrack(corporateId, facilityId);

            //Intialize the View Model i.e. ScrubEditTrackView which is binded to Main View Index.cshtml under ScrubEditTrack
            var scrubEditTrackView = new ScrubEditTrackView
            {
               ScrubEditTrackList = scrubEditTrackList,
               CurrentScrubEditTrack = new Model.ScrubEditTrack()
            };

            //Pass the View Model in ActionResult to View ScrubEditTrack
            return View(scrubEditTrackView);
        }

        /// <summary>
        /// Bind all the ScrubEditTrack list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the ScrubEditTrack list object
        /// </returns>
        [HttpPost]
        public ActionResult BindScrubEditTrackList()
        {
            //Initialize the ScrubEditTrack BAL object
            using (var scrubEditTrackBal = new ScrubEditTrackBal())
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                //Get the facilities list
                var scrubEditTrackList = scrubEditTrackBal.GetScrubEditTrack(corporateId, facilityId);

                //Pass the ActionResult with List of ScrubEditTrackViewModel object to Partial View ScrubEditTrackList
                return PartialView(PartialViews.ScrubEditTrackList, scrubEditTrackList);
            }
        }

        /// <summary>
        /// Add New or Update the ScrubEditTrack based on if we pass the ScrubEditTrack ID in the ScrubEditTrackViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of ScrubEditTrack row
        /// </returns>
        public ActionResult SaveScrubEditTrack(ScrubEditTrack model)
        {
            //Initialize the newId variable 
            var newId = -1;
			var userId = Helpers.GetLoggedInUserId();

            //Check if Model is not null 
            if (model != null)
            {
                using (var bal = new ScrubEditTrackBal())
                {
                    if (model.ScrubEditTrackID > 0)
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = CurrentDateTime;
                    }
                    //Call the AddScrubEditTrack Method to Add / Update current ScrubEditTrack
                    newId = bal.SaveScrubEditTrack(model);
                }
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current ScrubEditTrack in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetScrubEditTrack(int id)
        {
            using (var bal = new ScrubEditTrackBal())
            {
                //Call the AddScrubEditTrack Method to Add / Update current ScrubEditTrack
                var currentScrubEditTrack = bal.GetScrubEditTrackByID(id);

                //Pass the ActionResult with the current ScrubEditTrackViewModel object as model to PartialView ScrubEditTrackAddEdit
                return PartialView(PartialViews.ScrubEditTrackAddEdit, currentScrubEditTrack);
            }
        }

        /// <summary>
        /// Delete the current ScrubEditTrack based on the ScrubEditTrack ID passed in the ScrubEditTrackModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteScrubEditTrack(int id)
        {
            using (var bal = new ScrubEditTrackBal())
            {
                //Get ScrubEditTrack model object by current ScrubEditTrack ID
                var currentScrubEditTrack = bal.GetScrubEditTrackByID(id);
				var userId = Helpers.GetLoggedInUserId();

                //Check If ScrubEditTrack model is not null
                if (currentScrubEditTrack != null)
                {
                    currentScrubEditTrack.IsActive = false;
                    currentScrubEditTrack.CreatedBy = userId;
                    currentScrubEditTrack.CreatedDate = CurrentDateTime;

                    //Update Operation of current ScrubEditTrack
                    var result = bal.SaveScrubEditTrack(currentScrubEditTrack);

                    //return deleted ID of current ScrubEditTrack as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the ScrubEditTrack View Model and pass it to ScrubEditTrackAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetScrubEditTrackForm()
        {
            //Intialize the new object of ScrubEditTrack ViewModel
            var scrubEditTrackViewModel = new Model.ScrubEditTrack();

            //Pass the View Model as ScrubEditTrackViewModel to PartialView ScrubEditTrackAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.ScrubEditTrackAddEdit, scrubEditTrackViewModel);
        }



        public ActionResult GetScrubEditTrackList()
        {
            //Initialize the ScrubEditTrack BAL object
            using (var scrubEditTrackBal = new ScrubEditTrackBal())
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                //Get the facilities list
                var scrubEditTrackList = scrubEditTrackBal.GetScrubEditTrack(corporateId, facilityId);

                //Pass the ActionResult with List of ScrubEditTrackViewModel object to Partial View ScrubEditTrackList
                return PartialView(PartialViews.ScrubEditTrackList, scrubEditTrackList);
            }
        }
    }
}
