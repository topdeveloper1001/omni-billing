// To Do: MedicalHistoryController.cs
// FileName :MedicalHistoryController.cs
// CreatedDate: 2015-09-02 11:58 AM
// ModifiedDate: 2016-05-11 7:04 PM
// CreatedBy: Shashank Awasthy

namespace BillingSystem.Controllers
{
    #region

    using System.Collections.Generic;
    using System.Web.Mvc;

    using Bal.BusinessAccess;
    using Common;
    using Model;
    using Model.CustomModel;

    #endregion

    public class MedicalHistoryController : BaseController
    {
        /// <summary>
        ///     Add New or Update the MedicalHistory based on if we pass the MedicalHistory ID in the MedicalHistoryViewModel
        ///     object.
        /// </summary>
        /// <param name="model">pass the details of MedicalHistory in the view model</param>
        /// <returns>
        ///     returns the newly added or updated ID of MedicalHistory row
        /// </returns>
        public ActionResult SaveMedicalHistory(MedicalHistory model)
        {
            //Initialize the newId variable 
            var list = new List<MedicalHistoryCustomModel>();

            //Check if MedicalHistoryViewModel 
            if (model != null)
            {
                using (var bal = new MedicalHistoryService(Helpers.DefaultDrugTableNumber))
                {
                    var userId = Helpers.GetLoggedInUserId();
                    var currentDateTime = Helpers.GetInvariantCultureDateTime();
                    if (model.Id > 0)
                    {
                        if (model.IsDeleted == true)
                        {
                            model.DeletedBy = userId;
                            model.DeletedDate = currentDateTime;
                        }
                        else
                        {
                            model.ModifiedBy = userId;
                            model.ModifiedDate = currentDateTime;
                        }
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = currentDateTime;
                    }

                    //Call the AddMedicalHistory Method to Add / Update / Delete Current Medication
                    bal.AddUpdateMedicalHistory(model);

                    list = bal.GetMedicalHistory(model.PatientId, model.EncounterId);
                }
            }
            return this.PartialView(PartialViews.MedicalHistoryList, list);
        }

        /// <summary>
        ///     Get the details of the current MedicalHistory in the view model by ID
        /// </summary>
        /// <param name="medicalRecordId">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetMedicalHistory(int medicalRecordId)
        {
            using (var bal = new MedicalHistoryService(Helpers.DefaultDrugTableNumber))
            {
                //Call the AddMedicalHistory Method to Add / Update current MedicalHistory
                var vm = bal.GetMedicalHistoryById(medicalRecordId);

                return this.Json(vm, JsonRequestBehavior.AllowGet);
            }
        }
    }
}