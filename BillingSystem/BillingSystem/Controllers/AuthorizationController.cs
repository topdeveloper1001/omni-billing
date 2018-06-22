// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationController.cs" company="Spadez Solutions PVT. LTD.">
//    ServicesDotCom
// </copyright>
// <summary>
//   Defines the AuthorizationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Models;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The authorization controller.
    /// </summary>
    public class AuthorizationController : BaseController
    {
        /// <summary>
        /// Get the details of the Authorization View in the Model Authorization such as AuthorizationList, list of countries etc.
        /// </summary>
        /// <param name="pId">The p identifier.</param>
        /// <returns>
        /// returns the ActionResult in the form of current object of the Model Authorization to be passed to View Authorization
        /// </returns>
        public ActionResult AuthorizationMain(string pId)
        {
            if (string.IsNullOrEmpty(pId))
            {
                return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch, new { messageid = Convert.ToInt32(MessageType.ViewAuthorization) });
            }

            // Initialize the Authorization BAL object
            var bal = new AuthorizationBal();

            // Get the Entity list
            var authorizationList = bal.GetAuthorization();
            var memberId = bal.GetInsuranceMemberIdByPatientId(Convert.ToInt32(pId));

            // Intialize the View Model i.e. AuthorizationView which is binded to Main View Index.cshtml under Authorization
            var authorizationView = new AuthorizationView
            {
                AuthorizationList = authorizationList,
                CurrentAuthorization = new Authorization { IsDeleted = false, AuthorizationMemberID = memberId }
            };

            // Pass the View Model in ActionResult to View Authorization
            return View(authorizationView);
        }

        /// <summary>
        /// Bind all the Authorization list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the Authorization list object
        /// </returns>
        [HttpPost]
        public ActionResult BindAuthorizationList()
        {
            // Initialize the Authorization BAL object
            using (var authorizationBal = new AuthorizationBal())
            {
                // Get the facilities list
                var authorizationList = authorizationBal.GetAuthorization();

                // Pass the ActionResult with List of AuthorizationViewModel object to Partial View AuthorizationList
                return PartialView(PartialViews.AuthorizationList, authorizationList);
            }
        }

        /// <summary>
        /// Get the details of the current Authorization in the view model by ID
        /// </summary>
        /// <param name="authorizationModel">The authorization model.</param>
        /// <returns></returns>
        public ActionResult GetAuthorization(Authorization authorizationModel)
        {
            using (var authorizationBal = new AuthorizationBal())
            {
                // Call the AddAuthorization Method to Add / Update current Authorization
                var currentAuthorization = authorizationBal.GetAuthorizationById(Convert.ToInt32(authorizationModel.AuthorizationID));

                // If the view is shown in ViewMode only, then ViewBag.ViewOnly is set to true otherwise false.
                // ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);

                // Pass the ActionResult with the current AuthorizationViewModel object as model to PartialView AuthorizationAddEdit
                return PartialView(PartialViews.AuthorizationAddEdit, currentAuthorization);
            }
        }

        /// <summary>
        /// Delete the current Authorization based on the Authorization ID passed in the AuthorizationModel
        /// </summary>
        /// <param name="authorizationModel">The authorization model.</param>
        /// <returns></returns>
        public ActionResult DeleteAuthorization(Authorization authorizationModel)
        {
            using (var authorizationBal = new AuthorizationBal())
            {
                // Get Authorization model object by current Authorization ID
                var currentAuthorization = authorizationBal.GetAuthorizationById(Convert.ToInt32(authorizationModel.AuthorizationID));

                // Check If Authorization model is not null
                if (currentAuthorization != null)
                {
                    currentAuthorization.IsDeleted = true;
                    currentAuthorization.DeletedBy = Helpers.GetLoggedInUserId();
                    currentAuthorization.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    // Update Operation of current Authorization
                    var result = authorizationBal.AddUptdateAuthorization(currentAuthorization);

                    // return deleted ID of current Authorization as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the Authorization View Model and pass it to AuthorizationAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetAuthorizationForm()
        {
            // Intialize the new object of Authorization ViewModel
            var authorizationViewModel = new Authorization();

            // Pass the View Model as AuthorizationViewModel to PartialView AuthorizationAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.AuthorizationAddEdit, authorizationViewModel);
        }

        /// <summary>
        /// Generates the authentication XML.
        /// </summary>
        /// <param name="authId">The authentication identifier.</param>
        /// <returns></returns>
        public ActionResult GenerateAuthXml(int authId)
        {
            var result = string.Empty;
            using (var bal = new AuthorizationBal())
            {
                if (authId > 0)
                {
                    var list = bal.GenerateEAuthorizationFile(Helpers.GetDefaultFacilityId(), "Test");
                    result = "success";
                }
            }

            return Json(result);
        }


        public ActionResult SaveAuthorizationAsync(AuthorizationCustomModel vm)
        {
            var newId = 0;
            vm.CreatedBy = Helpers.GetLoggedInUserId();
            vm.CreatedDate = Helpers.GetInvariantCultureDateTime();
            vm.CorporateID = Helpers.GetSysAdminCorporateID();
            vm.FacilityID = Helpers.GetDefaultFacilityId();

            using (var bal = new AuthorizationBal())
            {
                var jsonData = string.Empty;
                var listOfDocs = Upload1(vm.PatientID.Value, "1");
                if (listOfDocs.Any())
                    jsonData = JsonConvert.SerializeObject(listOfDocs, new JsonSerializerSettings { ContractResolver = new DynamicContractResolver("filename,filepath") });


                var result = bal.SaveAuthorizationAsync1(vm, jsonData, true, true);
                newId = result.AuthorizationId;
                string errorMessage = string.Empty;

                var jsonAuthlistString = ViewRenderer.RenderPartialView(PartialViews.AuthorizationList, result.AuthList, ControllerContext, out errorMessage); //RenderPartialViewToStringBase(PartialViews.AuthorizationList, result.AuthList);
                var jsonDocslistString = ViewRenderer.RenderPartialView(PartialViews.PatientDocumentsList, result.Docs, ControllerContext, out errorMessage);   //RenderPartialViewToStringBase(PartialViews.PatientDocumentsList, result.Docs);

                var jsonResult = new
                {
                    Id = newId,
                    authListView = jsonAuthlistString,
                    docsView = jsonDocslistString
                };

                if (newId <= 0)
                    Delete(listOfDocs);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> SaveDocumentsAsync(long patientId)
        {
            IEnumerable<DocumentsTemplates> docs = null;
            if (Request.Files.Count > 0)
            {
                Session[SessionNames.Files.ToString()] = Request.Files;
                var result = await Upload(patientId, "1");
                if (result.Any())
                {
                    using (var bal = new DocumentsTemplatesBal())
                    {
                        docs = await bal.SaveDocumentsAsync(null, true, "");
                        if (docs.Any())
                        {
                            docs = docs.Where(a => a.DocumentName.Equals("Authorization File")).ToList();
                        }
                    }
                }
            }
            return Json(docs, JsonRequestBehavior.AllowGet);
        }

        #region Method No longer In-Use
        /// <summary>
        /// Add New or Update the Authorization based on if we pass the Authorization ID in the AuthorizationViewModel object.
        /// </summary>
        /// <param name="m">pass the details of Authorization in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of Authorization row
        /// </returns>
        public ActionResult SaveAuthorization(Authorization m)
        {
            // Initialize the newId variable 
            var newId = -1;

            // Check if AuthorizationViewModel 
            if (m != null)
            {
                using (var authorizationBal = new AuthorizationBal())
                {
                    var getAuthorizationList =
                        authorizationBal.GetAuthorizationsByEncounterId(m.EncounterID);
                    if (getAuthorizationList.Any())
                    {
                        if (m.AuthorizationID == 0)
                        {
                            if (
                                getAuthorizationList.Any(
                                    x =>
                                        m.AuthorizationEnd != null && (m.AuthorizationStart != null && ((x.AuthorizationStart.HasValue &&
                                                                                                                                           x.AuthorizationStart.Value >= m.AuthorizationStart.Value) &&
                                                                                                                                          (x.AuthorizationEnd.HasValue &&
                                                                                                                                           x.AuthorizationEnd.Value <= m.AuthorizationEnd.Value)))))
                            {
                                return Json(-1);
                            }
                        }
                    }

                    var loggedinUserid = Helpers.GetLoggedInUserId();
                    m.FacilityID = Helpers.GetDefaultFacilityId();
                    m.CorporateID = Helpers.GetSysAdminCorporateID();

                    // Code added by Shashank TO provide the AuthOrder date  as the Encounter start date.
                    // Code changed on 14 March 2016
                    //var encounterStartTime =
                    //    new EncounterBal().GetEncounterStartDateByEncounterId(Convert.ToInt32(authorizationModel.EncounterID));
                    //authorizationModel.AuthorizationDateOrdered = encounterStartTime;
                    if (m.AuthorizationID > 0)
                    {
                        m.ModifiedBy = loggedinUserid;
                        m.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                        var authrizationObj = authorizationBal.GetAuthorizationById(m.AuthorizationID);

                        // -- Commented the code line now the Autorization Order date will be equal to Encounter start date
                        m.AuthorizationDateOrdered = authrizationObj.AuthorizationDateOrdered;
                        m.CreatedBy = authrizationObj.CreatedBy;
                        m.CreatedDate = authrizationObj.CreatedDate;
                    }
                    else
                    {
                        m.AuthorizationDateOrdered = Helpers.GetInvariantCultureDateTime();
                        m.CreatedBy = loggedinUserid;
                        m.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    }

                    // Call the AddAuthorization Method to Add / Update current Authorization
                    newId = authorizationBal.AddUptdateAuthorization(m);
                }
            }

            return Json(newId);
        }

        #endregion
    }
}
