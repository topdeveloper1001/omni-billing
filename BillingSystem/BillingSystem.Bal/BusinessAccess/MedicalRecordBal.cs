using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Transactions;
using BillingSystem.Repository.Common;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MedicalRecordBal : BaseBal
    {
        private readonly IRepository<Users> _uRepository;
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MedicalRecord> GetMedicalRecord()
        {
            try
            {
                using (var medicalRecordRep = UnitOfWork.MedicalRecordRepository)
                {
                    var lstMedicalRecord = medicalRecordRep.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted
                        //&& a.IsActive == null || (bool)a.IsActive
                        ).ToList();
                    return lstMedicalRecord;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MedicalRecordBal()
        {

        }

        public MedicalRecordBal(string drgTableNumber)
        {
            DrgTableNumber = drgTableNumber;
        }


        public MedicalRecordBal(string drgTableNumber, string drugTableNumber)
        {
            DrgTableNumber = drgTableNumber;
            DrugTableNumber = drugTableNumber;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Medical Records on the basis of patientID and </returns>
        public List<AlergyCustomModel> GetAlergyRecords(int pateintID, int MedicalRecordType)
        {
            try
            {
                var lstAlergies = new List<AlergyCustomModel>();
                using (var medicalRecordRep = UnitOfWork.MedicalRecordRepository)
                {
                    var m = medicalRecordRep.Where(a => a.IsDeleted != true && a.PatientID == pateintID 
                    && a.MedicalRecordType == MedicalRecordType).OrderByDescending(x => x.MedicalRecordID);

                    AlergyCustomModel allergy = null;
                    var gcBal = new GlobalCodeBal();
                    var gccBal = new GlobalCodeCategoryBal();

                    foreach (var item in m)
                    {
                        allergy = new AlergyCustomModel
                        {
                            CurrentAlergy = item,
                            AlergyName = gcBal.GetNameByGlobalCodeId(Convert.ToInt32(item.GlobalCode)),
                            AlergyType = gccBal.GetGlobalCategoryNameById(Convert.ToString(item.GlobalCodeCategoryID)),
                            AddedBy = GetNameByUserId1(item.CreatedBy)
                        };

                        if ((item.GlobalCode != null && (int)item.GlobalCode == 0) && (item.GlobalCodeCategoryID != null
                            && (int)item.GlobalCodeCategoryID > 0))
                            allergy.AlergyName = "Other";


                        lstAlergies.Add(allergy);
                    }

                    return lstAlergies;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetNameByUserId1(int? UserID)
        {
            var usersModel = _uRepository.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
            return usersModel != null ? usersModel.FirstName + " " + usersModel.LastName : string.Empty;
        }
        /// <summary>
        /// Get Entity Name By Id
        /// </summary>
        /// <returns>Return the Entity Respository</returns>
        //public string GetMedicalRecordNameById(int? medicalRecordId)
        //{
        //    using (var medicalRecordRep = UnitOfWork.MedicalRecordRepository)
        //    {
        //        var iQueryabletransactions = medicalRecordRep.Where(a => a.MedicalRecordID == medicalRecordId).FirstOrDefault();
        //        return (iQueryabletransactions != null) ? iQueryabletransactions.MedicalRecordName : string.Empty;
        //    }
        //}

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="medicalRecord"></param>
        /// <returns></returns>
        public int AddUptdateMedicalRecord(MedicalRecord medicalRecord)
        {
            using (var medicalRecordRep = UnitOfWork.MedicalRecordRepository)
            {
                if (medicalRecord.MedicalRecordID > 0)
                    medicalRecordRep.UpdateEntity(medicalRecord, medicalRecord.MedicalRecordID);
                else
                    medicalRecordRep.Create(medicalRecord);
                return medicalRecord.MedicalRecordID;
            }
        }

        /// <summary>
        /// Adds the update medical record list.
        /// </summary>
        /// <param name="list">The medical record list.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterID">The encounter identifier.</param>
        /// <returns></returns>
        public bool SaveMedicalRecords(List<MedicalRecord> list, long patientId, long encounterId, long userId, long corporateId, long facilityId)
        {
            var dt = ExtensionMethods.CreateCommonDatatable();

            foreach (var item in list)
            {
                dt.Rows.Add(new object[] { 0,item.GlobalCodeCategoryID,item.GlobalCode,Convert.ToString(item.ShortAnswer)
                    ,item.DetailAnswer,item.Comments,Convert.ToString((int)MedicalRecordType.Allergies),string.Empty });
            }

            using (var rep = UnitOfWork.MedicalRecordRepository)
            {
                var result = rep.SaveMedicalRecords(dt, patientId, encounterId, corporateId, facilityId, userId);
                return result;
            }
        }
        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="medicalRecordId"></param>
        /// <returns></returns>
        public MedicalRecord GetMedicalRecordById(int? medicalRecordId)
        {
            using (var medicalRecordRep = UnitOfWork.MedicalRecordRepository)
            {
                var medicalRecord = medicalRecordRep.Where(x => x.MedicalRecordID == medicalRecordId).FirstOrDefault();
                return medicalRecord;
            }
        }

        public List<MedicalRecord> GetAlergyRecordsByPatientIdEncounterId(int patientId, int encounterID)
        {
            using (var medicalRecordRep = UnitOfWork.MedicalRecordRepository)
            {
                var lstMedicalRecord = medicalRecordRep.Where(a => a.EncounterID == encounterID && a.PatientID == patientId && a.IsDeleted == false).ToList();
                return lstMedicalRecord;
            }
        }

        #region Other Drug Alergy List -- Added by Amit Jain on 13062015

        public int DeleteMedicalRecordById(int id)
        {
            int deletedResult;
            using (var rep = UnitOfWork.MedicalRecordRepository)
            {
                rep.Delete(id);
                deletedResult = id;
            }
            return deletedResult;
        }
        public List<OtherDrugAlergy> GetOtherDrugAlergyListByEncounter(int encounterId)
        {
            var finalList = new List<OtherDrugAlergy>();
            using (var rep = UnitOfWork.MedicalRecordRepository)
            {
                var list =
                    rep.Where(
                        m =>
                            m.GlobalCodeCategoryID == 8102 && m.EncounterID == encounterId && m.IsDeleted == false &&
                            m.GlobalCode == 0 && m.DetailAnswer != null && !m.DetailAnswer.Trim().Equals("0"))
                        .ToList();

                if (list.Any())
                {
                    finalList.AddRange(list.Select(item =>
                    {
                        if (string.IsNullOrEmpty(item.DetailAnswer))
                            item.DetailAnswer = string.Empty;

                        var drug = GetDrugByCode(item.DetailAnswer.Trim());
                        var vm = new OtherDrugAlergy
                        {
                            MedicalRecordID = item.MedicalRecordID,
                            GlobalCode = item.GlobalCode,
                            GlobalCodeCategoryID = item.GlobalCodeCategoryID,
                            DrugName = drug.DrugGenericName,
                            DrugCode = drug.DrugCode
                        };
                        return vm;
                    }));
                }
                return finalList;
            }
        }
        public int AddOtherDrugAlergy(MedicalRecord model)
        {
            using (var rep = UnitOfWork.MedicalRecordRepository)
            {
                var alreadyInRecords = false;
                var orderCode = model.DetailAnswer.Trim();
                if (!string.IsNullOrEmpty(orderCode))
                {
                    alreadyInRecords =
                        rep.Where(
                            m =>
                                m.EncounterID == model.EncounterID && m.DetailAnswer.Trim().Equals(orderCode) &&
                                m.IsDeleted == false).Any();

                }
                if (!alreadyInRecords)
                {
                    var pInfo = GetPatientInfoByEncounterId(Convert.ToInt32(model.EncounterID));

                    if (pInfo != null && pInfo.PatientID > 0)
                    {
                        model.PatientID = pInfo.PatientID;
                        model.MedicalRecordNumber = pInfo.PersonMedicalRecordNumber;
                        model.FacilityID = pInfo.FacilityId;
                        rep.Create(model);
                        return model.MedicalRecordID;
                    }
                }
                return 0;
            }
        }

        #endregion
    }
}

