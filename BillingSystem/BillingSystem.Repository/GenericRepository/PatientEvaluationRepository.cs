using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Repository.Common;

namespace BillingSystem.Repository.GenericRepository
{
    public class PatientEvaluationRepository : GenericRepository<PatientEvaluation>
    {
        private readonly DbContext _context;

        public PatientEvaluationRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }
        public bool UpdateEvaluationManagement(PatientEvaluation oPatientEvaluationModel)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @PatientId, @EncounterId,@GlobalCodeCategory,@GlobalCode,@Value,@ExternalValue1,@ExternalValue3 ", StoredProcedures.SPROC_UpdatePatientEvaluation);
                    var sqlParameters = new SqlParameter[7];

                    sqlParameters[0] = new SqlParameter("PatientId", oPatientEvaluationModel.PatientId);
                    sqlParameters[1] = new SqlParameter("EncounterId", oPatientEvaluationModel.EncounterId);
                    sqlParameters[2] = new SqlParameter("GlobalCodeCategory", oPatientEvaluationModel.CategoryValue);
                    sqlParameters[3] = new SqlParameter("GlobalCode", oPatientEvaluationModel.CodeValue);
                    sqlParameters[4] = new SqlParameter("Value", oPatientEvaluationModel.Value);
                    sqlParameters[5] = new SqlParameter("ExternalValue1", oPatientEvaluationModel.ExternalValue1);
                    sqlParameters[6] = new SqlParameter("ExternalValue3", oPatientEvaluationModel.ExternalValue3);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public ResponseData SavePatientEvaluationManagement(DataTable data, long patientId, long eId, long cId, long fId, long userId, long setId)
        {
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter
            {
                ParameterName = "pDataArray",
                SqlDbType = SqlDbType.Structured,
                Value = data,
                TypeName = "ValuesArrayT"
            };
            sqlParameters[1] = new SqlParameter("pPatientId", patientId);
            sqlParameters[2] = new SqlParameter("pEncounterId", eId);
            sqlParameters[3] = new SqlParameter("pCId", cId);
            sqlParameters[4] = new SqlParameter("pFId", fId);
            sqlParameters[5] = new SqlParameter("pUserId", userId);
            sqlParameters[6] = new SqlParameter("pSetId", setId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSavePatientEvaluation.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var result = ms.SingleResultSetFor<ResponseData>();
                return result;
            }
        }
    }
}
