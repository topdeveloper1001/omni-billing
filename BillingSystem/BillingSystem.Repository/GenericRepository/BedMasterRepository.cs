using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BillingSystem.Repository.GenericRepository
{
   public class BedMasterRepository:GenericRepository<UBedMaster>
    {
       private readonly DbContext _context;

       public BedMasterRepository(BillingEntities context)
           : base(context)
       {
           AutoSave = true;
           _context = context;
       }

       /// <summary>
       /// Gets the bed occupany data.
       /// </summary>
       /// <param name="corporateId">The corporate identifier.</param>
       /// <param name="facilityId">The facility identifier.</param>
       /// <returns></returns>
       public List<BedOccupancyCustomModel> GetBedOccupanyData(int corporateId, int facilityId)
       {
           try
           {
               if (_context != null)
               {
                   var spName = string.Format("EXEC {0} @pCorporateID,@pFacilityID", StoredProcedures.SPROC_GetDBBedOccupancyRate);
                   var sqlParameters = new SqlParameter[2];
                   sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                   sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                   IEnumerable<BedOccupancyCustomModel> result = _context.Database.SqlQuery<BedOccupancyCustomModel>(spName, sqlParameters);
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
       /// Gets the bed occupany by floor data.
       /// </summary>
       /// <param name="corporateId">The corporate identifier.</param>
       /// <param name="facilityId">The facility identifier.</param>
       /// <returns></returns>
       public List<BedOccupancyCustomModel> GetBedOccupanyByFloorData(int corporateId, int facilityId)
       {
           try
           {
               if (_context != null)
               {
                   var spName = string.Format("EXEC {0} @pCorporateID,@pFacilityID", StoredProcedures.SPROC_GetDBBedOccupancybyFloor);
                   var sqlParameters = new SqlParameter[2];
                   sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                   sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                   var result = _context.Database.SqlQuery<BedOccupancyCustomModel>(spName, sqlParameters);
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
       /// Gets the bed occupany data.
       /// </summary>
       /// <param name="corporateId">The corporate identifier.</param>
       /// <param name="facilityId">The facility identifier.</param>
       /// <param name="displayType">The display type.</param>
       /// <param name="fromDate">From date.</param>
       /// <param name="tillDate">The till date.</param>
       /// <returns></returns>
       public List<BedOccupancyCustomModel> GetEncounterTypeData(int corporateId, int facilityId, string displayType, DateTime fromDate, DateTime tillDate)
       {
           try
           {
               if (_context != null)
               {
                   var spName = string.Format("EXEC {0} @pCorporateID,@pFacilityID,@pAsOnDate,@pViewType", StoredProcedures.SPROC_GetDBEncounterType);
                   var sqlParameters = new SqlParameter[4];
                   sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                   sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                   sqlParameters[2] = new SqlParameter("pAsOnDate", tillDate);
                   sqlParameters[3] = new SqlParameter("pViewType", displayType);
                   IEnumerable<BedOccupancyCustomModel> result = _context.Database.SqlQuery<BedOccupancyCustomModel>(spName, sqlParameters);
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
       /// Gets the bed struture for facility.
       /// </summary>
       /// <param name="corporateid">The corporateid.</param>
       /// <param name="facilityid">The facilityid.</param>
       /// <returns></returns>
       public List<FacilityBedStructureCustomModel> GetBedStrutureForFacility(int corporateid, int facilityid)
       {
           try
           {
               if (_context != null)
               {
                   var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID", StoredProcedures.SPROC_GetDBBedStruture);
                   var sqlParameters = new SqlParameter[2];
                   sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
                   sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
                   IEnumerable<FacilityBedStructureCustomModel> result = _context.Database.SqlQuery<FacilityBedStructureCustomModel>(spName, sqlParameters);
                   return result.ToList();
               }
               return null;
           }
           catch (System.Exception ex)
           {
               throw ex;
           }
       }
    }
}
