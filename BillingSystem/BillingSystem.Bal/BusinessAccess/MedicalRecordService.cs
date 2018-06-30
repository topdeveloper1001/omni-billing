using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<MedicalRecord> _repository;
        private readonly IRepository<GlobalCodeCategory> _gcRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<Drug> _drRepository;
        private readonly IRepository<Encounter> _eRepository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly BillingEntities _context;

        public MedicalRecordService(IRepository<Users> uRepository, IRepository<MedicalRecord> repository, IRepository<GlobalCodeCategory> gcRepository, IRepository<GlobalCodes> gRepository, IRepository<Drug> drRepository, IRepository<Encounter> eRepository, IRepository<PatientInfo> piRepository, BillingEntities context)
        {
            _uRepository = uRepository;
            _repository = repository;
            _gcRepository = gcRepository;
            _gRepository = gRepository;
            _drRepository = drRepository;
            _eRepository = eRepository;
            _piRepository = piRepository;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MedicalRecord> GetMedicalRecord()
        {
            var lstMedicalRecord = _repository.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted
                ).ToList();
            return lstMedicalRecord;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Medical Records on the basis of patientID and </returns>
        public List<AlergyCustomModel> GetAlergyRecords(int pateintID, int MedicalRecordType)
        {
            var lstAlergies = new List<AlergyCustomModel>();
            var m = _repository.Where(a => a.IsDeleted != true && a.PatientID == pateintID
            && a.MedicalRecordType == MedicalRecordType).OrderByDescending(x => x.MedicalRecordID);

            AlergyCustomModel allergy = null;

            foreach (var item in m)
            {
                allergy = new AlergyCustomModel
                {
                    CurrentAlergy = item,
                    AlergyName = GetNameByGlobalCodeId(Convert.ToInt32(item.GlobalCode)),
                    AlergyType = GetGlobalCategoryNameById(Convert.ToString(item.GlobalCodeCategoryID)),
                    AddedBy = GetNameByUserId1(item.CreatedBy)
                };

                if ((item.GlobalCode != null && (int)item.GlobalCode == 0) && (item.GlobalCodeCategoryID != null
                    && (int)item.GlobalCodeCategoryID > 0))
                    allergy.AlergyName = "Other";


                lstAlergies.Add(allergy);
            }

            return lstAlergies;
        }

        private string GetNameByGlobalCodeId(int id)
        {
            var gl = _gRepository.Where(g => g.GlobalCodeID == id).FirstOrDefault();
            return gl != null ? gl.GlobalCodeName : string.Empty;
        }
        private string GetGlobalCategoryNameById(string categoryValue, string facilityId = "")
        {
            if (!string.IsNullOrEmpty(categoryValue))
            {
                var category = _gcRepository.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue) && g.IsDeleted.HasValue ? !g.IsDeleted.Value : false && (string.IsNullOrEmpty(facilityId) || g.FacilityNumber.Equals(facilityId))).FirstOrDefault();
                return category != null ? category.GlobalCodeCategoryName : string.Empty;
            }
            return string.Empty;
        }


        public string GetNameByUserId1(int? UserID)
        {
            var usersModel = _uRepository.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
            return usersModel != null ? usersModel.FirstName + " " + usersModel.LastName : string.Empty;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="medicalRecord"></param>
        /// <returns></returns>
        public int AddUptdateMedicalRecord(MedicalRecord medicalRecord)
        {
            if (medicalRecord.MedicalRecordID > 0)
                _repository.UpdateEntity(medicalRecord, medicalRecord.MedicalRecordID);
            else
                _repository.Create(medicalRecord);
            return medicalRecord.MedicalRecordID;
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
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter
            {
                ParameterName = "pDataArray",
                SqlDbType = SqlDbType.Structured,
                Value = dt,
                TypeName = "ValuesArrayT"
            };
            sqlParameters[1] = new SqlParameter("pPatientId", patientId);
            sqlParameters[2] = new SqlParameter("pEncounterId", encounterId);
            sqlParameters[3] = new SqlParameter("pCId", corporateId);
            sqlParameters[4] = new SqlParameter("pFId", facilityId);
            sqlParameters[5] = new SqlParameter("pUserId", userId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveMedicalRecord.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var result = ms.SingleResultSetFor<bool>();
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
            var medicalRecord = _repository.Where(x => x.MedicalRecordID == medicalRecordId).FirstOrDefault();
            return medicalRecord;
        }

        public List<MedicalRecord> GetAlergyRecordsByPatientIdEncounterId(int patientId, int encounterID)
        {
            var lstMedicalRecord = _repository.Where(a => a.EncounterID == encounterID && a.PatientID == patientId && a.IsDeleted == false).ToList();
            return lstMedicalRecord;
        }

        #region Other Drug Alergy List -- Added by Amit Jain on 13062015

        public int DeleteMedicalRecordById(int id)
        {
            int deletedResult;
            deletedResult = _repository.Delete(id);
            return deletedResult;
        }
        public List<OtherDrugAlergy> GetOtherDrugAlergyListByEncounter(int encounterId, string DrugTableNumber)
        {
            var finalList = new List<OtherDrugAlergy>();
            var list =
                _repository.Where(
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

                    var drug = GetDrugByCode(item.DetailAnswer.Trim(), DrugTableNumber);
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
        private Drug GetDrugByCode(string code, string DrugTableNumber)
        {
            var drug = _drRepository.Where(d => d.DrugCode.Trim().Equals(code) && d.DrugTableNumber.Trim().Equals(DrugTableNumber))
                    .FirstOrDefault();
            return drug ?? new Drug();

        }

        private PatientInfo GetPatientInfoByEncounterId(int encounterId)
        {
            var patientInfo = new PatientInfo();
            var patientId = _eRepository.Where(e => e.EncounterID == encounterId).FirstOrDefault() != null ? _eRepository.Where(e => e.EncounterID == encounterId).FirstOrDefault().PatientID : 0;

            if (patientId > 0) patientInfo = _piRepository.Where(p => p.PatientID == patientId && p.IsDeleted == false).FirstOrDefault();

            return patientInfo;
        }
        public int AddOtherDrugAlergy(MedicalRecord model)
        {
            var alreadyInRecords = false;
            var orderCode = model.DetailAnswer.Trim();
            if (!string.IsNullOrEmpty(orderCode))
            {
                alreadyInRecords =
                    _repository.Where(
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
                    _repository.Create(model);
                    return model.MedicalRecordID;
                }
            }
            return 0;
        }
        #endregion
    }
}

