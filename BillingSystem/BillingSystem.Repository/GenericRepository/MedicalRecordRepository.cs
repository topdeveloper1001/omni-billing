using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Repository.Common;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

namespace BillingSystem.Repository.GenericRepository
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>
    {

        private readonly DbContext _context;

        public MedicalRecordRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public bool SaveMedicalRecords(DataTable data, long patientId, long eId, long cId, long fId, long userId)
        {
            var sqlParameters = new SqlParameter[6];
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

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveMedicalRecord.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var result = ms.SingleResultSetFor<bool>();
                return result;
            }
        }
    }
}
