using System;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;
using System.Data.SqlClient;
using System.Data.Entity;

namespace BillingSystem.Repository.GenericRepository
{
    public class MappingPatientBedRepository : GenericRepository<MappingPatientBed>
    {
        private readonly DbContext _context;

        public MappingPatientBedRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public EncounterCustomModel GetPatientBedInformation(int patientId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pPatientID", StoredProcedures.SPROC_GetPatientBedInformation);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pPatientID", patientId);
                    var result = _context.Database.SqlQuery<EncounterCustomModel>(spName, sqlParameters);
                    var bedInfo = result.FirstOrDefault();
                    return bedInfo ?? new EncounterCustomModel();
                }
                return new EncounterCustomModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EncounterCustomModel GetPatientBedInformationByBedId(int patientId, int bedId, string serviceCodeValue)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pPatientID, @pBedID, @pOverRideServiceCode", StoredProcedures.SPROC_GetSelectedBedInformation);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pPatientID", patientId);
                    sqlParameters[1] = new SqlParameter("pBedID", bedId);
                    sqlParameters[2] = new SqlParameter("pOverRideServiceCode", string.IsNullOrEmpty(serviceCodeValue) ? "0" : serviceCodeValue);
                    var result = _context.Database.SqlQuery<EncounterCustomModel>(spName, sqlParameters);
                    var bedInfo = result.FirstOrDefault();
                    return bedInfo ?? new EncounterCustomModel();
                }
                return new EncounterCustomModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
