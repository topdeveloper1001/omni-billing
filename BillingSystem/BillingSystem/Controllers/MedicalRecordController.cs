using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using System.Transactions;
using BillingSystem.Common.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class MedicalRecordController : BaseController
    {
        private readonly IMedicalRecordService _service;
        private readonly IGlobalCodeCategoryService _gcService;
        private readonly IGlobalCodeService _gService;

        public MedicalRecordController(IMedicalRecordService service, IGlobalCodeCategoryService gcService, IGlobalCodeService gService)
        {
            _service = service;
            _gcService = gcService;
            _gService = gService;
        }

        public ActionResult BindMedicalRecordList(int patientId)
        {
            var medicalRecordList = _service.GetAlergyRecords(patientId, Convert.ToInt32(Common.Common.MedicalRecordType.Allergies));
            return PartialView(PartialViews.AlergiesList, medicalRecordList);
        }
        public ActionResult SaveMedicalRecord(Model.MedicalRecord medicalRecordModel)
        {
            //Initialize the newId variable 
            var newId = -1;
            int loggedInUser = Helpers.GetLoggedInUserId();
            //Check if MedicalRecordViewModel 
            if (medicalRecordModel != null)
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
                newId = _service.AddUptdateMedicalRecord(medicalRecordModel);
            }
            return Json(newId);
        }
        public ActionResult GetMedicalRecord(int MedicalRecordID)
        {
            var currentMedicalRecord = _service.GetMedicalRecordById(MedicalRecordID);
            var CurrentAlergy = new AlergyCustomModel { CurrentAlergy = currentMedicalRecord };
            return PartialView(PartialViews.AlergiesAddAdit, CurrentAlergy);
        }
        public ActionResult DeleteMedicalRecord(int MedicalRecordID)
        {
            var currentMedicalRecord = _service.GetMedicalRecordById(MedicalRecordID);
            if (currentMedicalRecord != null)
            {
                currentMedicalRecord.IsDeleted = true;
                currentMedicalRecord.DeletedBy = Helpers.GetLoggedInUserId();
                currentMedicalRecord.DeletedDate = Helpers.GetInvariantCultureDateTime();

                var result = _service.AddUptdateMedicalRecord(currentMedicalRecord);
                return Json(result);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }
        public ActionResult ResetMedicalRecordForm()
        {
            //Intialize the new object of MedicalRecord ViewModel
            var CurrentAlergy = new AlergyCustomModel { CurrentAlergy = new MedicalRecord() };

            //Pass the View Model as MedicalRecordViewModel to PartialView MedicalRecordAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.AlergiesAddAdit, CurrentAlergy);
        }
        public ActionResult GetAlergyType()
        {
            var lstGlobalCodeCategories = _gcService.GetGlobalCodeCategoriesRange(Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.AlergyStartRange),
                Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.AlergyEndRange));
            return Json(lstGlobalCodeCategories);
        }
        public ActionResult GetAlergies(string GlobalCodeCategoryValue)
        {
            var lstGlobalCodes = _gService.GetGCodesListByCategoryValue(GlobalCodeCategoryValue);
            return Json(lstGlobalCodes);
        }
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

                var i = _service.SaveMedicalRecords(list, patientId, encounterId, userId, cId, fId);
                return Json(i);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GetMedicalRecordByPatientEncounter(int patientID, int encounterID)
        {
            var medicalRecords = _service.GetAlergyRecordsByPatientIdEncounterId(patientID, encounterID);
            return Json(medicalRecords);
        }
        public PartialViewResult BindOtherDrugAllergiesByEncounter(int encounterId, int deletedId, string newOrderCode)
        {
            using (var transScope = new TransactionScope())
            {
                if (deletedId > 0)
                    _service.DeleteMedicalRecordById(deletedId);
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
                        _service.AddOtherDrugAlergy(model);
                    }
                }

                var list = _service.GetOtherDrugAlergyListByEncounter(encounterId, Helpers.DefaultDrugTableNumber);
                transScope.Complete();

                return PartialView(PartialViews.OtherDrugAlergiesList, list);
            }
        }
    }
}
