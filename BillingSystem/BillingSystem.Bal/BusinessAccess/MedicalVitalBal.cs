using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using System.Transactions;

namespace BillingSystem.Bal.BusinessAccess
{
    /// <summary>
    /// 
    /// </summary>
    public class MedicalVitalBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<MedicalVital> GetMedicalVital()
        {
            try
            {
                using (var MedicalVitalRep = UnitOfWork.MedicalVitalRepository)
                {
                    var lstMedicalVital = MedicalVitalRep.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted).ToList();
                    return lstMedicalVital;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            using (var MedicalVitalRep = UnitOfWork.MedicalVitalRepository)
            {
                if (MedicalVital.MedicalVitalID > 0)
                    MedicalVitalRep.UpdateEntity(MedicalVital, MedicalVital.MedicalVitalID);
                else
                    MedicalVitalRep.Create(MedicalVital);
                return MedicalVital.MedicalVitalID;
            }
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
                using (var moduleaccessRep = UnitOfWork.MedicalVitalRepository)
                {
                    newList.AddRange(moduleVitalList);
                    result = Convert.ToInt32(moduleaccessRep.Create(newList));
                    transScope.Complete();
                }
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
            using (var medicalVitalRep = UnitOfWork.MedicalVitalRepository)
            {
                var medicalVital = medicalVitalRep.Where(x => x.MedicalVitalID == MedicalVitalId).FirstOrDefault();
                return medicalVital;
            }
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
                using (var medicalVitalRep = UnitOfWork.MedicalVitalRepository)
                {
                    var lstMedicalvital = medicalVitalRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                        && a.PatientID == patientId && a.MedicalVitalType == type).OrderByDescending(x => x.CreatedDate).ToList();
                    foreach (var item in lstMedicalvital)
                    {
                        using (var userbal = new UsersBal())
                        {
                            var medicalvitalCustomModel = new MedicalVitalCustomModel()
                            {
                                MedicalVital = item,
                                PressureCustom = item.AnswerValueMin.Value.ToString("F"),
                                // item.AnswerValueMax != null && item.AnswerValueMax != Convert.ToDecimal(0.0000) ? item.AnswerValueMin.Value.ToString("F") + "/" + item.AnswerValueMax.Value.ToString("F") : ,
                                VitalAddedBy = userbal.GetNameByUserId(item.CommentBy),
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
                    }
                }
                return medicalVitalList.OrderByDescending(x => x.MedicalVital.CreatedDate).ThenBy(x => x.MedicalVitalName).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the custom medical vitals.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public List<MedicalVitalCustomModel> GetCustomMedicalVitals(int patientId, int type,int encounterid)
        {
            try
            {
                var medicalVitalList = new List<MedicalVitalCustomModel>();
                using (var medicalVitalRep = UnitOfWork.MedicalVitalRepository)
                {
                    var lstMedicalvital = medicalVitalRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                        && a.PatientID == patientId && a.MedicalVitalType == type && a.EncounterID == encounterid).OrderByDescending(x => x.CreatedDate).ToList();
                    foreach (var item in lstMedicalvital)
                    {
                        using (var userbal = new UsersBal())
                        {
                            using (var globalBal = new GlobalCodeBal())
                            {
                                var vitalGlobalCategory = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.LabTest).ToString();
                                //var UOMGlobalCategory = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.UnitOfMeasure).ToString();
                                var medicalvitalCustomModel = new MedicalVitalCustomModel()
                                {
                                    MedicalVital = item,
                                    VitalAddedBy = userbal.GetNameByUserId(item.CommentBy),
                                    VitalAddedOn = Convert.ToDateTime(item.CreatedDate),
                                    LabTestName = globalBal.GetGlobalCodeNameByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                                    LabTestValues = item.AnswerValueMax != null && item.AnswerValueMax != Convert.ToDecimal(0.0000) ? string.Format("{0} - {1}",item.AnswerValueMax ,item.AnswerValueMin):string.Empty,
                                    LabTestRange = globalBal.GetExternalVal1Val2ByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                                    //UnitOfMeasureName = globalBal.GetGlobalCodeNameByIdAndCategoryId(UOMGlobalCategory, Convert.ToInt32(item.AnswerUOM))
                                };
                                medicalVitalList.Add(medicalvitalCustomModel);
                            }
                        }
                    }
                }
                return medicalVitalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            try
            {
                var list = new List<MedicalVitalCustomModel>();
                using (var medicalVitalRep = UnitOfWork.MedicalVitalRepository)
                {
                    var result = medicalVitalRep.GetVitalsChartData(patientId, type, tillDate);
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
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                using (var medicalVitalRep = UnitOfWork.MedicalVitalRepository)
                {
                    var lstMedicalvital = medicalVitalRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                        && a.PatientID == patientId && a.MedicalVitalType == type && a.EncounterID == currentEncounterId).OrderByDescending(x => x.CreatedDate).ToList();
                    foreach (var item in lstMedicalvital)
                    {
                        using (var userbal = new UsersBal())
                        {
                            using (var globalBal = new GlobalCodeBal())
                            {
                                var medicalvitalCustomModel = new MedicalVitalCustomModel()
                                {
                                    MedicalVital = item,
                                    PressureCustom = item.AnswerValueMax != null && item.AnswerValueMax != Convert.ToDecimal(0.0000) ? item.AnswerValueMin.Value.ToString("F") + "/" + item.AnswerValueMax.Value.ToString("F") : item.AnswerValueMin.Value.ToString("F"),
                                    VitalAddedBy = userbal.GetNameByUserId(item.CommentBy),
                                    VitalAddedOn = Convert.ToDateTime(item.CreatedDate),
                                    MedicalVitalName = GetNameByGlobalCodeValue(Convert.ToInt32(item.GlobalCode).ToString(), Convert.ToInt32(GlobalCodeCategoryValue.Vitals).ToString()),
                                    UnitOfMeasureName = GetNameByGlobalCodeValue(Convert.ToInt32(item.AnswerUOM).ToString(), Convert.ToInt32(GlobalCodeCategoryValue.UnitOfMeasure).ToString()),
                                    //MedicalVitalName = globalBal.GetGlobalCodeNameByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                                    //UnitOfMeasureName = globalBal.GetGlobalCodeNameByIdAndCategoryId(uomGlobalCategory, Convert.ToInt32(item.AnswerUOM))
                                };
                                medicalVitalList.Add(medicalvitalCustomModel);
                            }
                        }
                    }
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
                using (var medicalVitalRep = UnitOfWork.MedicalVitalRepository)
                {
                    var lstMedicalvital = medicalVitalRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                        && a.PatientID == patientId && a.MedicalVitalType == type && a.EncounterID == encounterid).OrderByDescending(x => x.CreatedDate).ToList();
                    foreach (var item in lstMedicalvital)
                    {
                        using (var userbal = new UsersBal())
                        {
                            using (var globalBal = new GlobalCodeBal())
                            {
                                var vitalGlobalCategory = Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.LabTest).ToString();
                                var medicalvitalCustomModel = new MedicalVitalCustomModel()
                                {
                                    MedicalVital = item,
                                    VitalAddedBy = userbal.GetNameByUserId(item.CommentBy),
                                    VitalAddedOn = Convert.ToDateTime(item.CreatedDate),
                                    LabTestName = globalBal.GetGlobalCodeNameByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                                    LabTestValues = item.AnswerValueMax != null && item.AnswerValueMax != Convert.ToDecimal(0.0000) ? string.Format("{0} - {1}",  item.AnswerValueMin.Value.ToString("#.##"),item.AnswerValueMax.Value.ToString("#.##")) : string.Empty,
                                    LabTestRange = globalBal.GetExternalVal1Val2ByIdAndCategoryId(vitalGlobalCategory, Convert.ToInt32(item.GlobalCode)),
                                    //UnitOfMeasureName = globalBal.GetGlobalCodeNameByIdAndCategoryId(UOMGlobalCategory, Convert.ToInt32(item.AnswerUOM))
                                };
                                medicalVitalList.Add(medicalvitalCustomModel);
                            }
                        }
                    }
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
            try
            {
                var list = new List<MedicalVitalCustomModel>();
                using (var medicalVitalRep = UnitOfWork.MedicalVitalRepository)
                {
                    var result = medicalVitalRep.GetVitalsChart2(patientId, fromDate, tillDate);
                    if (result.Count > 0)
                    {
                        //result = result.Where(v => v.VitalCode == vitalCode).ToList();

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
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the risk factors.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public RiskFactorViewModel GetRiskFactors(int patientId)
        {
            try
            {
                var list = new RiskFactorViewModel();
                using (var medicalVitalRep = UnitOfWork.MedicalVitalRepository)
                {
                    var result = medicalVitalRep.GetRiskFactors(patientId);
                    list = result.FirstOrDefault();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

