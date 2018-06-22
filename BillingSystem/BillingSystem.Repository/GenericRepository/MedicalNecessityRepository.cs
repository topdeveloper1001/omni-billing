using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Repository.GenericRepository
{
    public class MedicalNecessityRepository : GenericRepository<MedicalNecessity>
    {
        private readonly DbContext _context;
        public MedicalNecessityRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        public List<MedicalNecessityCustomModel> GetMedicalNecessityData(string tableNumber)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @TableNumber", StoredProcedures.SPROC_GetMedicalNecessityData);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("@TableNumber", tableNumber);
                    IEnumerable<MedicalNecessityCustomModel> result = _context.Database.SqlQuery<MedicalNecessityCustomModel>(spName, sqlParameters);
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
