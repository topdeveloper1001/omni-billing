using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using System.Data.Entity;
using System.Data.SqlClient;

namespace BillingSystem.Repository.GenericRepository
{
    public class MedicalHistoryRepository : GenericRepository<MedicalHistory>
    {
        private readonly DbContext _context;

        public MedicalHistoryRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public AlergyView GetPatientHistoryAndAlleryData(long patientId, long encounterId, long userId)
        {
            var vm = new AlergyView();

            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pPatientId", patientId);
            sqlParameters[1] = new SqlParameter("pEncounterId", encounterId);
            sqlParameters[2] = new SqlParameter("pUserId", userId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetMedicalHistoryAndAllergyData.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var mhList = ms.GetResultWithJson<MedicalHistoryCustomModel>(JsonResultsArray.MedicalHistory.ToString());

                vm.MedicalHistoryView = new MedicalHistoryView { CurrentMedicalHistory = new MedicalHistory(), MedicalHistoryList = mhList };
                vm.AllergiesHistoriesGCC = ms.GetResultWithJson<GlobalCodeCategory>(JsonResultsArray.GlobalCategory.ToString());
                vm.AllergiesHistoryGC = ms.GetResultWithJson<GlobalCodes>(JsonResultsArray.GlobalCode.ToString());
                vm.AlergyList = ms.GetResultWithJson<AlergyCustomModel>(JsonResultsArray.Allergy.ToString());
            }
            return vm;
        }
    }
}
