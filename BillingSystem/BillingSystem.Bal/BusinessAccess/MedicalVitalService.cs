using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using System.Transactions;

using System.Data.SqlClient;
using AutoMapper;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{ 
    public class MedicalVitalService : IMedicalVitalService
    {
        private readonly IRepository<MedicalVital> _repository;
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public MedicalVitalService(IRepository<MedicalVital> repository, IRepository<Users> uRepository, IRepository<GlobalCodes> gRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _uRepository = uRepository;
            _gRepository = gRepository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<MedicalVital> GetMedicalVital()
        {
            var lstMedicalVital = _repository.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted).ToList();
            return lstMedicalVital;
        }

        /// <summary>
        /// Get Entity Name By Id
        /// </summary>
        /// <param name="MedicalVital">The medical vital.</param>
        /// <returns>
        /// Return the Entity Respository
        /// </returns>
        public int AddUptdateMedicalVital(MedicalVital MedicalVital)
        {
            if (MedicalVital.MedicalVitalID > 0)
                _repository.UpdateEntity(MedicalVital, MedicalVital.MedicalVitalID);
            else
                _repository.Create(MedicalVital);
            return MedicalVital.MedicalVitalID;
        }

        //Function to add/delete role permissions
        /// <summary>
        /// Adds the update module access.
        /// </summary>
        /// <param name="moduleVitalList">The module vital list.</param>
        /// <returns></returns>
        public int AddUpdateModuleAccess(List<MedicalVital> moduleVitalList)
        {
            var result = -1;
            var newList = new List<MedicalVital>();
            using (var transScope = new TransactionScope())
            {
                newList.AddRange(moduleVitalList);
                result = Convert.ToInt32(_repository.Create(newList));
                transScope.Complete();
            }
            return result;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="MedicalVitalId">The medical vital identifier.</param>
        /// <returns></returns>
        public MedicalVital GetMedicalVitalByID(int? MedicalVitalId)
        {
            var medicalVital = _repository.Where(x => x.MedicalVitalID == MedicalVitalId).FirstOrDefault();
            return medicalVital;
        }

        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<MedicalVitalCustomModel> GetCustomMedicalVitals(int patientId, int type)
        {
            try
            {
                var medicalVitalList = new List<MedicalVitalCustomModel>();
                var lstMedicalvital = _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                    && a.PatientID == patientId && a.MedicalVitalType == type).OrderByDescending(x => x.CreatedDate).ToList();
                foreach (var item in lstMedicalvital)
                {
                    var medicalvitalCustomModel = new MedicalVitalCustomModel()
                    {
                        MedicalVital = item,
                        PressureCustom = item.AnswerValueMin.Value.ToString("F"),
                        // item.AnswerValueMax != null && item.AnswerValueMax != Convert.ToDecimal(0.0000) ? item.AnswerValueMin.Value.ToString("F") + "/" + item.AnswerValueMax.Value.ToString("F") : ,
                        VitalAddedBy = GetNameByUserId1(item.CommentBy),
                        VitalAddedOn = Convert.ToDateTime(item.CreatedDate),
                        MedicalVitalName =
                            GetNameByGlobalCodeValue(Convert.ToInt32(item.GlobalCode).ToString(),
                                Convert.ToInt32(GlobalCodeCategoryValue.Vitals).ToString()),
                        UnitOfMeasureName =
                            GetNameByGlobalCodeValue(Convert.ToInt32(item.AnswerUOM).ToString(),
                                Convert.ToInt32(GlobalCodeCategoryValue.UnitOfMeasure).ToString()),
                        //    MedicalVitalName = globalBal.GetGlobalCodeNameByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                        //    UnitOfMeasureName = globalBal.GetGlobalCodeNameByIdAndCategoryId(UOMGlobalCategory, Convert.ToInt32(item.AnswerUOM))
                    };
                    medicalVitalList.Add(medicalvitalCustomModel);

                }
                return medicalVitalList.OrderByDescending(x => x.MedicalVital.CreatedDate).ThenBy(x => x.MedicalVitalName).ToList();
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
        /// Gets the custom medical vitals.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public List<MedicalVitalCustomModel> GetCustomMedicalVitals(int patientId, int type, int encounterid)
        {
            try
            {
                var medicalVitalList = new List<MedicalVitalCustomModel>();
                var lstMedicalvital = _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                    && a.PatientID == patientId && a.MedicalVitalType == type && a.EncounterID == encounterid).OrderByDescending(x => x.CreatedDate).ToList();
                foreach (var item in lstMedicalvital)
                {
                    var vitalGlobalCategory = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.LabTest).ToString();
                    //var UOMGlobalCategory = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.UnitOfMeasure).ToString();
                    var medicalvitalCustomModel = new MedicalVitalCustomModel()
                    {
                        MedicalVital = item,
                        VitalAddedBy = GetNameByUserId1(item.CommentBy),
                        VitalAddedOn = Convert.ToDateTime(item.CreatedDate),
                        LabTestName = GetGlobalCodeNameByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                        LabTestValues = item.AnswerValueMax != null && item.AnswerValueMax != Convert.ToDecimal(0.0000) ? string.Format("{0} - {1}", item.AnswerValueMax, item.AnswerValueMin) : string.Empty,
                        LabTestRange = GetExternalVal1Val2ByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                        //UnitOfMeasureName = globalBal.GetGlobalCodeNameByIdAndCategoryId(UOMGlobalCategory, Convert.ToInt32(item.AnswerUOM))
                    };
                    medicalVitalList.Add(medicalvitalCustomModel);
                }
                return medicalVitalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetGlobalCodeNameByIdAndCategoryId(string categoryId, int globalCodeId)
        {
            var globalCode = _gRepository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryId) && c.GlobalCodeID == globalCodeId).FirstOrDefault();
            if (globalCode != null)
                return globalCode.GlobalCodeName;
            return string.Empty;
        }
        private string GetExternalVal1Val2ByIdAndCategoryId(string categoryId, int globalCodeId)
        {
            var globalCode = _gRepository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryId) && c.GlobalCodeID == globalCodeId).FirstOrDefault();
            if (globalCode != null)
                return string.Format("{0} - {1}", globalCode.ExternalValue1, globalCode.ExternalValue2);
            return string.Empty;
        }

        /// <summary>
        /// Gets the medical vitals chart data.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public List<MedicalVitalCustomModel> GetMedicalVitalsChartData(int patientId, int type, DateTime tillDate)
        {
            var spName = string.Format("EXEC {0} @pPatientID, @pDisplayTypeID, @pTillDate", StoredProcedures.SPROC_GetVitals.ToString());
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pPatientID", patientId);
            sqlParameters[1] = new SqlParameter("pDisplayTypeID", type);
            sqlParameters[2] = new SqlParameter("pTillDate", tillDate);
            IEnumerable<MedicalVitalExtension> r = _context.Database.SqlQuery<MedicalVitalExtension>(spName, sqlParameters);

            var list = new List<MedicalVitalCustomModel>();
            var result = r.ToList();
            if (result.Count > 0)
            {
                list.AddRange(result.Select(item => new MedicalVitalCustomModel
                {
                    Name = item.Name,
                    VitalCode = item.VitalCode,
                    VitalName = item.VitalName,
                    XAxis = item.XAxis,
                    Average = item.Average,
                    Maximum = item.Maximum,
                    Minimum = item.Minimum,
                    LowerLimit = item.LowerLimit,
                    UpperLimit = item.UpperLimit
                }));
            }
            return list;

        }
        private string GetNameByUserId(int? UserID)
        {
            var usersModel = _uRepository.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
            return usersModel != null ? usersModel.FirstName + " " + usersModel.LastName : string.Empty;
        }
        /// <summary>
        /// Gets the custom medical vitals by pid encounter identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="currentEncounterId">The current encounter identifier.</param>
        /// <returns></returns>
        public List<MedicalVitalCustomModel> GetCustomMedicalVitalsByPidEncounterId(int patientId, int type, int currentEncounterId)
        {
            try
            {
                var medicalVitalList = new List<MedicalVitalCustomModel>();
                var lstMedicalvital = _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                    && a.PatientID == patientId && a.MedicalVitalType == type && a.EncounterID == currentEncounterId).OrderByDescending(x => x.CreatedDate).ToList();
                foreach (var item in lstMedicalvital)
                {
                    var medicalvitalCustomModel = new MedicalVitalCustomModel()
                    {
                        MedicalVital = item,
                        PressureCustom = item.AnswerValueMax != null && item.AnswerValueMax != Convert.ToDecimal(0.0000) ? item.AnswerValueMin.Value.ToString("F") + "/" + item.AnswerValueMax.Value.ToString("F") : item.AnswerValueMin.Value.ToString("F"),
                        VitalAddedBy = GetNameByUserId(item.CommentBy),
                        VitalAddedOn = Convert.ToDateTime(item.CreatedDate),
                        MedicalVitalName = GetNameByGlobalCodeValue(Convert.ToInt32(item.GlobalCode).ToString(), Convert.ToInt32(GlobalCodeCategoryValue.Vitals).ToString()),
                        UnitOfMeasureName = GetNameByGlobalCodeValue(Convert.ToInt32(item.AnswerUOM).ToString(), Convert.ToInt32(GlobalCodeCategoryValue.UnitOfMeasure).ToString()),
                        //MedicalVitalName = globalBal.GetGlobalCodeNameByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                        //UnitOfMeasureName = globalBal.GetGlobalCodeNameByIdAndCategoryId(uomGlobalCategory, Convert.ToInt32(item.AnswerUOM))
                    };
                    medicalVitalList.Add(medicalvitalCustomModel);
                }
                return medicalVitalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the custom lab test.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public List<MedicalVitalCustomModel> GetCustomLabTest(int patientId, int encounterid, int type)
        {
            try
            {
                var medicalVitalList = new List<MedicalVitalCustomModel>();
                var lstMedicalvital = _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                    && a.PatientID == patientId && a.MedicalVitalType == type && a.EncounterID == encounterid).OrderByDescending(x => x.CreatedDate).ToList();
                foreach (var item in lstMedicalvital)
                {
                    var vitalGlobalCategory = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.LabTest).ToString();
                    var medicalvitalCustomModel = new MedicalVitalCustomModel()
                    {
                        MedicalVital = item,
                        VitalAddedBy = GetNameByUserId(item.CommentBy),
                        VitalAddedOn = Convert.ToDateTime(item.CreatedDate),
                        LabTestName = GetGlobalCodeNameByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                        LabTestValues = item.AnswerValueMax != null && item.AnswerValueMax != Convert.ToDecimal(0.0000) ? string.Format("{0} - {1}", item.AnswerValueMin.Value.ToString("#.##"), item.AnswerValueMax.Value.ToString("#.##")) : string.Empty,
                        LabTestRange = GetExternalVal1Val2ByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                        //UnitOfMeasureName = globalBal.GetGlobalCodeNameByIdAndCategoryId(UOMGlobalCategory, Convert.ToInt32(item.AnswerUOM))
                    };
                    medicalVitalList.Add(medicalvitalCustomModel);
                }
                return medicalVitalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the medical vital chart2.
        /// </summary>
        /// <param name="vitalCode">The vital code.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public List<MedicalVitalCustomModel> GetMedicalVitalChart2(int vitalCode, int patientId, DateTime? fromDate, DateTime? tillDate)
        {
            if (fromDate == null)
                fromDate = DateTime.Now;
            if (tillDate == null)
                tillDate = DateTime.Now;

            var spName = string.Format("EXEC {0} @pPatientID, @pFromDate, @pTillDate", StoredProcedures.SPROC_GetVitalsDR);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pPatientID", patientId);
            sqlParameters[1] = new SqlParameter("pFromDate", fromDate);
            sqlParameters[2] = new SqlParameter("pTillDate", tillDate);
            IEnumerable<MedicalVitalExtension> result = _context.Database.SqlQuery<MedicalVitalExtension>(spName, sqlParameters);

            return result.Select(x => _mapper.Map<MedicalVitalCustomModel>(x)).ToList();

            //var list = new List<MedicalVitalCustomModel>();
            //    var result = _repository.GetVitalsChart2(patientId, fromDate, tillDate);
            //    if (result.Count > 0)
            //    {
            //        //result = result.Where(v => v.VitalCode == vitalCode).ToList();

            //        list.AddRange(result.Select(item => new MedicalVitalCustomModel
            //        {
            //            Name = item.Name,
            //            VitalCode = item.VitalCode,
            //            VitalName = item.VitalName,
            //            XAxis = item.XAxis,
            //            Average = item.Average,
            //            Maximum = item.Maximum,
            //            Minimum = item.Minimum,
            //            LowerLimit = item.LowerLimit,
            //            UpperLimit = item.UpperLimit
            //        }));
            //    }
            //    return list;

        }

        /// <summary>
        /// Gets the risk factors.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public RiskFactorViewModel GetRiskFactors(int patientId)
        {
            var spName = string.Format("EXEC {0} @pPatientId", StoredProcedures.SPROC_GetRiskFactors);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pPatientId", patientId);
            IEnumerable<RiskFactorViewModel> result = _context.Database.SqlQuery<RiskFactorViewModel>(spName, sqlParameters);
            return result.FirstOrDefault();
        }
    }
}

