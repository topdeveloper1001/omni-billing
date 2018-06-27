using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class CorporateService : ICorporateService
    {
        private readonly IRepository<Corporate> _repository;
        private readonly BillingEntities _context;

        public CorporateService(IRepository<Corporate> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<Corporate> GetCorporate(int cId)
        {
            try
            {
                var lstCorporate = cId > 0 ? _repository.Where(a => a.IsDeleted != true && a.CorporateID == cId).ToList() : _repository.Where(a => a.IsDeleted != true).ToList();
                return lstCorporate.OrderBy(x => x.CorporateNumber, new NumericComparer()).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Entity Name By Id
        /// </summary>
        /// <returns>Return the Entity Respository</returns>
        public string GetCorporateNameById(int? corporateId)
        {
            var q = _repository.Where(a => a.CorporateID == corporateId).FirstOrDefault();
            return (q != null) ? q.CorporateName : string.Empty;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="corporate"></param>
        /// <returns></returns>
        public int AddUptdateCorporate(Corporate corporate)
        {
            int newId;
            if (corporate.CorporateID > 0)
            {
                var currentData = GetCorporateById(corporate.CorporateID);
                corporate.CreatedBy = currentData.CreatedBy;
                corporate.CreatedDate = currentData.CreatedDate;
                newId = Convert.ToInt32(_repository.UpdateEntity(corporate, corporate.CorporateID));
            }
            else
                newId = Convert.ToInt32(_repository.Create(corporate));

            return newId;

        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="CorporateId">The corporate identifier.</param>
        /// <returns></returns>
        public Corporate GetCorporateById(int? CorporateId)
        {
            var corporate = _repository.Where(x => x.CorporateID == CorporateId).FirstOrDefault();
            return corporate;

        }

        /// <summary>
        /// Method to To check Duplicate Corporate on the basis of Name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool CheckDuplicateCorporate(string name, int id)
        {

            var res = _repository.Where(x => x.CorporateID != id && x.IsDeleted != true && x.CorporateName == name).FirstOrDefault();
            return res != null;

        }

        /// <summary>
        /// Checks the duplicate corporate number.
        /// </summary>Updated By Krishna on 21082015
        /// <param name="number">The number.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool CheckDuplicateCorporateNumber(string number, int id)
        {
            var res = _repository.Where(x => x.CorporateID != id && x.IsDeleted != true && x.CorporateNumber == number).FirstOrDefault();
            return res != null;

        }

        /// <summary>
        /// Deletes the corporate data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public bool DeleteCorporateData(string corporateId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("CId", corporateId);
            _repository.ExecuteCommand(StoredProcedures.CleanupAllDataByCorporate.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Gets the corporate DDL.
        /// </summary>
        /// <param name="cId">The c identifier.</param>
        /// <returns></returns>
        public List<Corporate> GetCorporateDDL(int cId)
        {
            var lst = _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted) && (cId == 0 || a.CorporateID == cId)).ToList();
            return lst.OrderBy(x => x.CorporateID).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultTableNumber"></param>
        /// <param name="corporateId"></param>
        /// <returns></returns>
        public bool CheckDefaultTableNumber(string defaultTableNumber, int corporateId)
        {
            bool retValue;
            var count = _repository.Where(i => i.DefaultDRUGTableNumber == defaultTableNumber && i.CorporateID != corporateId).Count();
            retValue = count > 0;

            return retValue;
        }

        /// <summary>
        /// Gets all corporate.
        /// </summary>
        /// <returns></returns>
        public List<Corporate> GetAllCorporate()
        {
            var lstCorporate = _repository.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted).ToList();
            return lstCorporate.OrderByDescending(x => x.CorporateID).ToList();

        }

        public void CreateDefaultCorporateItem(int corporateId, string corporateName)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("aCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("aCorporateName", corporateName);
            _repository.ExecuteCommand(StoredProcedures.SPROC_DefaultCorporateItems.ToString(), sqlParameters);
        }


        public List<DropdownListData> GetCorporateDropdownData(int cId)
        {
            var list = new List<DropdownListData>();
            var lstCorporate = _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted) && (cId == 0 || a.CorporateID == cId)).ToList();

            list.AddRange(lstCorporate.Select(item => new DropdownListData
            {
                Text = item.CorporateName,
                Value = Convert.ToString(item.CorporateID)
            }));

            return list;
        }
    }
}

