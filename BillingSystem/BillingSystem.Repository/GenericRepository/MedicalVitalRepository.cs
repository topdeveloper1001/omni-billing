using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using System.Linq;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Repository.GenericRepository
{
    public class MedicalVitalRepository : GenericRepository<MedicalVital>
    {
        private readonly DbContext _context;

        public MedicalVitalRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        public List<MedicalVitalExtension> GetVitalsChartData(int patientId, int displayType, DateTime tillDate)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pPatientID, @pDisplayTypeID, @pTillDate", StoredProcedures.SPROC_GetVitals.ToString());
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pPatientID", patientId);
                    sqlParameters[1] = new SqlParameter("pDisplayTypeID", displayType);
                    sqlParameters[2] = new SqlParameter("pTillDate", tillDate);
                    IEnumerable<MedicalVitalExtension> result = _context.Database.SqlQuery<MedicalVitalExtension>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public List<MedicalVitalExtension> GetVitalsChart2(int patientId, DateTime? fromDate, DateTime? tillDate)
        {
            try
            {
                if (_context != null)
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
                    return result.ToList();
                }
                return null;
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
        public List<RiskFactorViewModel> GetRiskFactors(int patientId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pPatientId", StoredProcedures.SPROC_GetRiskFactors);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pPatientId", patientId);
                    IEnumerable<RiskFactorViewModel> result = _context.Database.SqlQuery<RiskFactorViewModel>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
