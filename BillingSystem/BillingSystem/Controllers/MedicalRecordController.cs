using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using System.Transactions;
using BillingSystem.Common.Common;

namespace BillingSystem.Controllers
{
    public class MedicalRecordController : BaseController
    {
        /// <summary>
        /// Get the details of the MedicalRecord View in the Model MedicalRecord such as MedicalRecordList, list of countries etc.
        /// </summary>
        /// <returns>returns the actionresult in the form of current object of the Model MedicalRecord to be passed to View MedicalRecord</returns>
        //public ActionResult MedicalRecordMain()
        //{
        //    //Initialize the MedicalRecord BAL object
        //    var medicalRecordBal = new MedicalRecordBal();

        //    //Get the Entity list
        //    var medicalRecordList = medicalRecordBal.GetMedicalRecord();

        //    //Intialize the View Model i.e. MedicalRecordView which is binded to Main View Index.cshtml under MedicalRecord
        //    var medicalRecordView = new MedicalRecordView
        //    {
        //       MedicalRecordList = medicalRecordList,
        //       CurrentMedicalRecord = new Model.MedicalRecord()
        //    };

        //    //Pass the View Model in ActionResult to View MedicalRecord
        //    return View(medicalRecordView);
        //}

        /// <summary>
        /// Bind all the MedicalRecord list
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns>
        /// action result with the partial view containing the MedicalRecord list object
        /// </returns>
        //  [HttpPost]
        public ActionResult BindMedicalRecordList(int patientId)
        {
            //Initialize the MedicalRecord BAL object
            using (var medicalRecordBal = new MedicalRecordService())
            {
                //Get the facilities list
                var medicalRecordList = medicalRecordBal.GetAlergyRecords(patientId, Convert.ToInt32(Common.Common.MedicalRecordType.Allergies));

                //Pass the ActionResult with List of MedicalRecordViewModel object to Partial View MedicalRecordList
                return PartialView(PartialViews.AlergiesList, medicalRecordList);
            }
        }

        /// <summary>
        /// Add New or Update the MedicalRecord based on if we pass the MedicalRecord ID in the MedicalRecordViewModel object.
        /// </summary>
        /// <param name="medicalRecordModel">pass the details of MedicalRecord in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of MedicalRecord row
        /// </returns>
        public ActionResult SaveMedicalRecord(Model.MedicalRecord medicalRecordModel)
        {
            //Initialize the newId variable 
            var newId = -1;
            int loggedInUser = Helpers.GetLoggedInUserId();
            //Check if MedicalRecordViewModel 
            if (medicalRecordModel != null)
            {
                using (var medicalRecordBal = new MedicalRecordService())
                {
                    medicalRecordModel.MedicalRecordType = Convert.ToInt32(Common.Common.MedicalRecordType.Allergies);
                    medicalRecordModel.CorporateID = Helpers.GetSysAdminCorporateID();
                    medicalRecordModel.FacilityID = Helpers.GetDefaultFacilityId();
                    if (!string.IsNullOrEmpty(medicalRecordModel.Comments.Trim()))
                    {
                        medicalRecordModel.CommentBy = loggedInUser;
                        medicalRecordModel.CommentDate = Helpers.GetInvariantCultureDateTime();//later we will fetch it from DB
                    }
                    if (medicalRecordModel.MedicalRecordID > 0)
                    {
                        medicalRecordModel.ModifiedBy = loggedInUser;
                        medicalRecordModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    }
                    else
                    {
                        medicalRecordModel.CreatedBy = loggedInUser;
                        medicalRecordModel.CreatedDate = Helpers.GetInvariantCultureDateTime();//later we will fetch it from DB
                        medicalRecordModel.IsDeleted = false;
                    }
                    //Call the AddMedicalRecord Method to Add / Update current MedicalRecord
                    newId = medicalRecordBal.AddUptdateMedicalRecord(medicalRecordModel);
                }
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current MedicalRecord in the view model by ID
        /// </summary>
        /// <param name="MedicalRecordID">The medical record identifier.</param>
        /// <returns></returns>
        public ActionResult GetMedicalRecord(int MedicalRecordID)
        {
            using (var medicalRecordBal = new MedicalRecordService())
            {
                //Call the AddMedicalRecord Method to Add / Update current MedicalRecord
                var currentMedicalRecord = medicalRecordBal.GetMedicalRecordById(MedicalRecordID);

                //If the view is shown in ViewMode only, then ViewBag.ViewOnly is set to true otherwise false.
                //ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);
                var CurrentAlergy = new AlergyCustomModel { CurrentAlergy = currentMedicalRecord };
                //Pass the ActionResult with the current MedicalRecordViewModel object as model to PartialView MedicalRecordAddEdit
                return PartialView(PartialViews.AlergiesAddAdit, CurrentAlergy);
            }
        }

        /// <summary>
        /// Delete the current MedicalRecord based on the MedicalRecord ID passed in the MedicalRecordModel
        /// </summary>
        /// <param name="MedicalRecordID">The medical record identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteMedicalRecord(int MedicalRecordID)
        {
            using (var medicalRecordBal = new MedicalRecordService())
            {
                //Get MedicalRecord model object by current MedicalRecord ID
                var currentMedicalRecord = medicalRecordBal.GetMedicalRecordById(MedicalRecordID);

                //Check If MedicalRecord model is not null
                if (currentMedicalRecord != null)
                {
                    currentMedicalRecord.IsDeleted = true;
                    currentMedicalRecord.DeletedBy = Helpers.GetLoggedInUserId();
                    currentMedicalRecord.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current MedicalRecord
                    var result = medicalRecordBal.AddUptdateMedicalRecord(currentMedicalRecord);

                    //return deleted ID of current MedicalRecord as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the MedicalRecord View Model and pass it to MedicalRecordAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetMedicalRecordForm()
        {
            //Intialize the new object of MedicalRecord ViewModel
            var CurrentAlergy = new AlergyCustomModel { CurrentAlergy = new MedicalRecord() };

            //Pass the View Model as MedicalRecordViewModel to PartialView MedicalRecordAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.AlergiesAddAdit, CurrentAlergy);
        }

        //function to get alergytype
        /// <summary>
        /// Gets the type of the alergy.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAlergyType()
        {
            var objGlobalCodeCategoryBal = new GlobalCodeCategoryService();
            var lstGlobalCodeCategories = objGlobalCodeCategoryBal.GetGlobalCodeCategoriesRange(Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.AlergyStartRange),
                Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.AlergyEndRange));
            return Json(lstGlobalCodeCategories);
        }
        //function to get alergies
        /// <summary>
        /// Gets the alergies.
        /// </summary>
        /// <param name="GlobalCodeCategoryValue">The global code category value.</param>
        /// <returns></returns>
        public ActionResult GetAlergies(string GlobalCodeCategoryValue)
        {
            var objGlobalCodeCategoryBal = new GlobalCodeCategoryService();
            var objGlobalCodeBal = new GlobalCodeService();
            var lstGlobalCodes = objGlobalCodeBal.GetGCodesListByCategoryValue(GlobalCodeCategoryValue);
            return Json(lstGlobalCodes);
        }

        /// <summary>
        /// Adds the medical record.
        /// </summary>
        /// <param name="list">The object medical record list.</param>
        /// <param name="patienID">The patien identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult AddMedicalRecord(List<MedicalRecord> list, int patientId, int encounterId)
        {
            try
            {
                var userId = Helpers.GetLoggedInUserId();
                var cId = Helpers.GetSysAdminCorporateID();
                var fId = Helpers.GetDefaultFacilityId();
                var currentDate = Helpers.GetInvariantCultureDateTime();

                list.ForEach(a =>
                {
                    a.MedicalRecordType = (int)MedicalRecordType.Allergies;
                    a.CorporateID = cId;
                    a.FacilityID = fId;
                    a.ShortAnswer = true;
                    a.IsDeleted = false;
                    a.CreatedBy = userId;
                    a.CreatedDate = currentDate;
                });

                var bal = new MedicalRecordService();
                var i = bal.SaveMedicalRecords(list, patientId, encounterId, userId, cId, fId);
                return Json(i);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the medical record by patient encounter.
        /// </summary>
        /// <param name="patientID">The patient identifier.</param>
        /// <param name="encounterID">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult GetMedicalRecordByPatientEncounter(int patientID, int encounterID)
        {
            var objMedicalRecordBal = new MedicalRecordService();
            //var medicalRecords = objMedicalRecordBal.GetAlergyRecordsByPatientIdEncounterId(Convert.ToInt32(patienID), Convert.ToInt32(encounterID));
            var medicalRecords = objMedicalRecordBal.GetAlergyRecordsByPatientIdEncounterId(patientID, encounterID);
            //List<int?> GlobalCodes = (from q in medicalRecords select q.GlobalCode).ToList();
            return Json(medicalRecords);
        }

        /*
         * By: Amit Jain
         * On: 13062015
         * Purpose: Get multiple Other Drug Allergy List by Current Encounter ID
         */
        public PartialViewResult BindOtherDrugAllergiesByEncounter(int encounterId, int deletedId, string newOrderCode)
        {
            using (var transScope = new TransactionScope())
            {
                using (var bal = new MedicalRecordService(Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber))
                {
                    if (deletedId > 0)
                        bal.DeleteMedicalRecordById(deletedId);
                    else
                    {
                        if (!string.IsNullOrEmpty(newOrderCode))
                        {
                            var userId = Helpers.GetLoggedInUserId();
                            var currentDateTime = Helpers.GetInvariantCultureDateTime();
                            var corporateId = Helpers.GetSysAdminCorporateID();
                            var model = new MedicalRecord
                            {
                                CommentBy = userId,
                                CommentDate = currentDateTime,
                                Comments = string.Empty,
                                CorporateID = corporateId,
                                CreatedBy = userId,
                                CreatedDate = currentDateTime,
                                DeletedBy = 0,
                                DetailAnswer = newOrderCode,
                                EncounterID = encounterId,
                                FacilityID = Helpers.GetDefaultFacilityId(),
                                GlobalCode = 0,
                                GlobalCodeCategoryID = 8102,
                                IsDeleted = false,
                                MedicalRecordType = 3 //will change it later with the Enum value
                            };
                            bal.AddOtherDrugAlergy(model);
                        }
                    }

                    var list = bal.GetOtherDrugAlergyListByEncounter(encounterId);
                    transScope.Complete();

                    return PartialView(PartialViews.OtherDrugAlergiesList, list);
                }
            }
        }
    }
}
