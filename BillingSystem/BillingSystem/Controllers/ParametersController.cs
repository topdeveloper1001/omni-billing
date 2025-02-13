﻿using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using System;
using System.Web.Mvc;

namespace BillingSystem.Controllers
{
    public class ParametersController : BaseController
    {
        private readonly IParametersService _service;
        private readonly IFacilityService _fService;

        public ParametersController(IParametersService service, IFacilityService fService)
        {
            _service = service;
            _fService = fService;
        }


        /// <summary>
        /// Get the details of the Parameters View in the Model Parameters such as ParametersList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model Parameters to be passed to View Parameters
        /// </returns>
        public ActionResult ParametersMain()
        {
            var corporateid = Helpers.GetDefaultCorporateId();
            var facilityid = Helpers.GetDefaultFacilityId();
            //Get the Entity list
            var parametersList = _service.GetParametersCustomModel(corporateid, facilityid);

            //Intialize the View Model i.e. ParametersView which is binded to Main View Index.cshtml under Parameters
            var parametersView = new ParametersView
            {
                ParametersList = parametersList,
                CurrentParameters = new ParametersCustomModel()
            };
            parametersView.CurrentParameters.EffectiveStartDate = Helpers.GetInvariantCultureDateTime();
            //Pass the View Model in ActionResult to View Parameters
            return View(parametersView);
        }

        /// <summary>
        /// Bind all the Parameters list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the Parameters list object
        /// </returns>
        public ActionResult BindParametersList()
        {
            var corporateid = Helpers.GetDefaultCorporateId();
            var facilityid = Helpers.GetDefaultFacilityId();
            //Get the facilities list
            var parametersList = _service.GetParametersCustomModel(corporateid, facilityid);

            //Pass the ActionResult with List of ParametersViewModel object to Partial View ParametersList
            return PartialView(PartialViews.ParametersList, parametersList);
        }

        /// <summary>
        /// Add New or Update the Parameters based on if we pass the Parameters ID in the ParametersViewModel object.
        /// </summary>
        /// <param name="parametersModel">pass the details of Parameters in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of Parameters row
        /// </returns>
        public ActionResult SaveParameters(Model.Parameters parametersModel)
        {
            //Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var facilityNumber = _fService.GetFacilityNumberById(facilityId);
            //Check if ParametersViewModel 
            if (parametersModel != null)
            {
                if (parametersModel.ParametersID > 0)
                {
                    parametersModel.ModifiedBy = userId;
                    parametersModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                parametersModel.CorporateID = corporateId;
                parametersModel.FacilityID = facilityId;
                parametersModel.FacilityNumber = facilityNumber;
                parametersModel.CreatedBy = userId;
                parametersModel.CreatedDate = Helpers.GetInvariantCultureDateTime();
                //Call the AddParameters Method to Add / Update current Parameters
                newId = _service.AddUptdateParameters(parametersModel);
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current Parameters in the view model by ID
        /// </summary>
        /// <param name="ParametersID">The parameters identifier.</param>
        /// <returns></returns>
        public ActionResult GetParameters(int ParametersID)
        {
            //Call the AddParameters Method to Add / Update current Parameters
            var currentParameters = _service.GetParametersCustomModelByID(Convert.ToInt32(ParametersID));

            //If the view is shown in ViewMode only, then ViewBag.ViewOnly is set to true otherwise false.
            //ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);

            //Pass the ActionResult with the current ParametersViewModel object as model to PartialView ParametersAddEdit
            return PartialView(PartialViews.ParametersAddEdit, currentParameters);
        }

        /// <summary>
        /// Delete the current Parameters based on the Parameters ID passed in the ParametersModel
        /// </summary>
        /// <param name="ParametersID">The parameters identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteParameters(int ParametersID)
        {
            var currentParameters = _service.GetParametersByID(Convert.ToInt32(ParametersID));
            var userId = Helpers.GetLoggedInUserId();
            if (currentParameters != null)
            {
                currentParameters.IsActive = false;
                currentParameters.ModifiedBy = userId;
                currentParameters.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                var result = _service.AddUptdateParameters(currentParameters);

                return Json(result);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the Parameters View Model and pass it to ParametersAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetParametersForm()
        {
            //Intialize the new object of Parameters ViewModel
            var parametersViewModel = new ParametersCustomModel() { EffectiveStartDate = Helpers.GetInvariantCultureDateTime() };
            //Pass the View Model as ParametersViewModel to PartialView ParametersAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.ParametersAddEdit, parametersViewModel);
        }
    }
}
