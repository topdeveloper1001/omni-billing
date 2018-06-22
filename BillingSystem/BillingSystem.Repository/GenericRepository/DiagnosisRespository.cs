using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BillingSystem.Repository.GenericRepository
{
    public class DiagnosisRespository : GenericRepository<Diagnosis>
    {
        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        public DiagnosisRespository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Get the physician order list
        /// </summary>
        /// <param name="encounterId">
        /// </param>
        /// <param name="orderStatus">
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public DiagnosisTabData GetDiagnosisTabData(long pId, long eId = 0, long physicianId = 0, string diagnosisTn = "", string drgTn = "")
        {
            DiagnosisTabData vm = new DiagnosisTabData();
            try
            {
                var sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("PId", pId);
                sqlParameters[1] = new SqlParameter("EId", eId);
                sqlParameters[2] = new SqlParameter("PhysicianId", physicianId);
                sqlParameters[3] = new SqlParameter("DiagnosisTN", diagnosisTn);
                sqlParameters[4] = new SqlParameter("DrgTN", drgTn);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDiagnosisTabData.ToString(), parameters: sqlParameters, isCompiled: false))
                {
                    vm.CurrentDiagnosisList = r.GetResultWithJson<DiagnosisCustomModel>(JsonResultsArray.Diagnosis.ToString());
                    vm.PreviousDiagnosisList = r.GetResultWithJson<DiagnosisCustomModel>(JsonResultsArray.Diagnosis.ToString());
                    vm.FavOrdersList = r.GetResultWithJson<FavoritesCustomModel>(JsonResultsArray.FavoriteDiagnosis.ToString());
                    return vm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<DiagnosisCustomModel> GetPreviousDiagnosisData(long pId, long eId = 0, long physicianId = 0, string diagnosisTn = "", string drgTn = "")
        {
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("pEncounterId", eId);
            sqlParameters[1] = new SqlParameter("pPatientId", pId);
            sqlParameters[2] = new SqlParameter("pDRGTN", drgTn);
            sqlParameters[3] = new SqlParameter("pDiagnosisTN", diagnosisTn);
            sqlParameters[4] = new SqlParameter("pEncounterNumber", string.Empty);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetPreviousDiagnosisData.ToString(), parameters: sqlParameters, isCompiled: false))
                return r.GetResultWithJson<DiagnosisCustomModel>(JsonResultsArray.Diagnosis.ToString());

        }

        public IEnumerable<DiagnosisCustomModel> GetCurrentDiagnosisData(long pId, long eId, string drgTn = "")
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pPId", pId);
            sqlParameters[1] = new SqlParameter("pEId", eId);
            sqlParameters[2] = new SqlParameter("pEncounterNumber", DBNull.Value);
            sqlParameters[3] = new SqlParameter("pDRGTN", drgTn);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetCurrentDiagnosisData.ToString(), parameters: sqlParameters, isCompiled: false))
                return r.GetResultWithJson<DiagnosisCustomModel>(JsonResultsArray.Diagnosis.ToString());

        }

        public IEnumerable<FavoritesCustomModel> GetFavoriteDiagnosisData(long userId, string diagnosisTn = "", string drgTn = "")
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pUserId", userId);
            sqlParameters[1] = new SqlParameter("pDRGTN", drgTn);
            sqlParameters[2] = new SqlParameter("pDiagnosisTN", diagnosisTn);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetFavoriteDiagnosisData.ToString(), parameters: sqlParameters, isCompiled: false))
                return r.GetResultWithJson<FavoritesCustomModel>(JsonResultsArray.FavoriteDiagnosis.ToString());

        }


        public DiagnosisTabData DeleteCurrentDiagnosis(long userId, long id, string drgTn)
        {
            var result = new DiagnosisTabData { ExecutionStatus = 0 };
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pPId", userId);
            sqlParameters[1] = new SqlParameter("pId", id);
            sqlParameters[2] = new SqlParameter("pDRGTN", drgTn);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocDeleteDiagnosisById.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                result.ExecutionStatus = r.ResultSetFor<int>().FirstOrDefault();
                if (result.ExecutionStatus > 0)
                {
                    result.PrimaryExists = r.ResultSetFor<bool>().FirstOrDefault();
                    result.MajorCPTExists = r.ResultSetFor<bool>().FirstOrDefault();
                    result.MajorDRGExists = r.ResultSetFor<bool>().FirstOrDefault();

                    result.CurrentDiagnosisList = r.GetResultWithJson<DiagnosisCustomModel>(JsonResultsArray.Diagnosis.ToString());
                }
            }
            return result;
        }
    }
}
