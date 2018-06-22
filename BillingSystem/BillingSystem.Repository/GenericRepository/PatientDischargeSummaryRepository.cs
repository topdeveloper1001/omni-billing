using System;
using System.Data.Entity;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Repository.GenericRepository
{
    public class PatientDischargeSummaryRepository : GenericRepository<PatientDischargeSummary>
    {
        private readonly DbContext _context;
        public PatientDischargeSummaryRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }
        /*public bool SaveEvaluationManagement(PatientEvaluationCustomModel oPatientEvaluationCustomModel)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @CorporateId, @Facility, @PatientId, @EncounterId, @CategoryValue, @CodeValue, @ParentCodeValue, @Value, @CreatedBy, @CreatedDate", StoredProcedures.SPROC_SaveEvaluationManagement);
                    var sqlParameters = new SqlParameter[10];
                    sqlParameters[0] = new SqlParameter("CorporateId", oPatientEvaluationCustomModel.CorporateId);
                    sqlParameters[1] = new SqlParameter("Facility", oPatientEvaluationCustomModel.FacilityId);
                    sqlParameters[2] = new SqlParameter("PatientId", oPatientEvaluationCustomModel.PatientId);
                    sqlParameters[3] = new SqlParameter("EncounterId", oPatientEvaluationCustomModel.EncounterId);
                    sqlParameters[4] = new SqlParameter("CategoryValue", oPatientEvaluationCustomModel.CategoryValue);
                    sqlParameters[5] = new SqlParameter("CodeValue", oPatientEvaluationCustomModel.CodeValue);
                    sqlParameters[6] = new SqlParameter("ParentCodeValue", oPatientEvaluationCustomModel.ParentCodeValue);
                    sqlParameters[7] = new SqlParameter("Value", oPatientEvaluationCustomModel.Value);
                    sqlParameters[8] = new SqlParameter("CreatedBy", oPatientEvaluationCustomModel.CreatedBy);
                    sqlParameters[9] = new SqlParameter("CreatedDate", oPatientEvaluationCustomModel.CreatedDate);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }*/
    }
}
