using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ModuleAccessService : IModuleAccessService
    {
        private readonly IRepository<ModuleAccess> _repository;

        public ModuleAccessService(IRepository<ModuleAccess> repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ModuleAccess> GetModuleAccess()
        {
            var lstModuleAccess = _repository.Where(a => a.IsDeleted != false).ToList();
            return lstModuleAccess;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="moduleAccess"></param>
        /// <returns></returns>
        public int DeleteEntry(ModuleAccess moduleAccess)
        {
            if (moduleAccess.ModuleAccessID > 0)
                _repository.UpdateEntity(moduleAccess, moduleAccess.ModuleAccessID);
            else
                _repository.Create(moduleAccess);
            return moduleAccess.ModuleAccessID;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <param name="moduleAccessId"></param>
        /// <returns></returns>
        public ModuleAccess GetModuleAccessByID(int? moduleAccessId)
        {
            var moduleAccess = _repository.Where(x => x.ModuleAccessID == moduleAccessId).FirstOrDefault();
            return moduleAccess;
        }

        /// <summary>
        /// Gets the modules access list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<ModuleAccess> GetModulesAccessList(int corporateId, int? facilityid)
        {
            var list = _repository.Where(rp => rp.CorporateID == corporateId).ToList();
            if (facilityid != 0)
            {
                if (list.Any(fc => fc.FacilityID == facilityid))
                    list = list.Where(fc => fc.FacilityID == facilityid).ToList();
            }
            return list;
        }

        /// <summary>
        /// Method is used to add update the module access
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="corporateid"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public int Save(DataTable dt, int corporateid, int facilityId, DateTime? currentDate, int loggedinUserId = 0)
        {
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter
            {
                ParameterName = "pModuleAccessList",
                SqlDbType = SqlDbType.Structured,
                Value = dt,
                TypeName = "ModuleAccessTT"
            };
            sqlParameters[1] = new SqlParameter("pCorporateId", corporateid);
            sqlParameters[2] = new SqlParameter("pFacilityId", facilityId);
            sqlParameters[3] = new SqlParameter("pLoggedInUserId", loggedinUserId);
            sqlParameters[4] = new SqlParameter("pCurrentDate", currentDate);
            _repository.ExecuteCommand(StoredProcedures.SPROC_AddUpdateModuleAccess.ToString(), sqlParameters);
            return 1;
        }
    }
}

