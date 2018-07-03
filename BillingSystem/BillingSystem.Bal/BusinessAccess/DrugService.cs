using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;


namespace BillingSystem.Bal.BusinessAccess
{
    public class DrugService : IDrugService
    {
        private readonly IRepository<Drug> _repository;
        private readonly BillingEntities _context;

        public DrugService(IRepository<Drug> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }



        /// <summary>
        /// Get Entity Name By Id
        /// </summary>
        /// <param name="model">The drug.</param>
        /// <returns>
        /// Return the Entity Respository
        /// </returns>
        public int AddUptdateDrug(Drug model, string DrugTableNumber)
        {
            model.DrugTableNumber = DrugTableNumber;
            if (model.Id > 0)
            {
                var current = _repository.GetSingle(model.Id);
                model.DrugTableNumber = current.DrugTableNumber;
                model.DrugDescription = current.DrugDescription;
                _repository.UpdateEntity(model, model.Id);
            }
            else
                _repository.Create(model);
            return model.Id;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="drugId">The drug identifier.</param>
        /// <returns></returns>
        public Drug GetDrugByID(int? drugId)
        {
            var model = _repository.Where(x => x.Id == drugId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets the drug list.
        /// </summary> 
        /// <returns></returns>
        public List<Drug> GetDrugList(string DrugTableNumber)
        {
            var lstDrug = _repository.Where(d => !d.DrugStatus.Equals("Deleted") && d.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(x => x.Id).ToList();
            lstDrug = lstDrug.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
            return lstDrug.OrderBy(x => x.DrugCode).ToList();
        }

        /// <summary>
        /// Gets the drug list by drug view.
        /// </summary>Updated By Krishna on 08072015
        /// <param name="ViewVal">The view value.</param>
        /// <returns></returns>
        public List<Drug> GetDrugListByDrugView(string ViewVal, string DrugTableNumber)
        {
            var drugList = _repository.Where(a => a.DrugStatus.ToLower().Equals(ViewVal.ToLower()) && a.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(x => x.Id).ToList();
            drugList = drugList.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
            return ViewVal == "All" ? _repository.Where(d => !d.DrugStatus.Equals("Deleted") && d.DrugTableNumber.Trim().Equals(DrugTableNumber)).ToList() : drugList;
        }

        public List<Drug> GetDrugListOnDemand(int blockNumber, int blockSize, string viewVal, string DrugTableNumber)
        {
            try
            {
                int startIndex = (blockNumber - 1) * blockSize;

                var list = _repository.Where(s => s.DrugStatus.ToLower().Equals(viewVal.ToLower()) && s.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(f => f.Id).ToList();
                list = list.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList().Skip(startIndex).Take(blockSize).ToList();
                return viewVal == "All"
                            ? _repository.Where(d => !d.DrugStatus.Equals("Deleted") && d.DrugTableNumber.Trim().Equals(DrugTableNumber))
                                .OrderByDescending(f => f.Id)
                                .Skip(startIndex)
                                .Take(blockSize).ToList()
                            : list;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the drug listby brand code.
        /// </summary>
        /// <param name="brandCode">The brand code.</param>
        /// <returns></returns>
        public List<Drug> GetDrugListbyBrandCode(string brandCode, string DrugTableNumber)
        {
            var lstDrug = _repository.Where(d => (d.DrugStatus.Equals("Active") || d.DrugStatus.Equals("Grace")) && d.BrandCode == brandCode && d.InStock == true && d.DrugTableNumber.Trim().Equals(DrugTableNumber)).ToList();
            lstDrug = lstDrug.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
            return lstDrug.OrderBy(x => x.DrugCode).ToList();

        }

        /// <summary>
        /// Gets the drug code description.
        /// </summary>
        /// <param name="codeid">The codeid.</param>
        /// <returns></returns>
        public string GetDRUGCodeDescription(string codeid, string DrugTableNumber)
        {
            var model = _repository.Where(x => x.DrugCode == codeid && x.DrugTableNumber.Trim().Equals(DrugTableNumber)).FirstOrDefault();
            return model != null ? model.DrugPackageName : string.Empty;

        }

        /// <summary>
        /// Gets the filtered drug codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public List<Drug> GetFilteredDrugCodes(string text, string DrugTableNumber)
        {

            text = text.Trim().ToLower();
            var list =
                _repository.Where(
                    h =>
                        (h.DrugCode.Trim().ToLower().Contains(text) ||
                         h.DrugGenericName.Trim().ToLower().Contains(text) ||
                         h.DrugPackageName.Trim().ToLower().Contains(text)) &&
                        h.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(x => x.Id).ToList();
            list = list.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
            return list;

        }

        /// <summary>
        /// Gets the filtered drug codes status.
        /// </summary> Udated by krishna
        /// <param name="text">The text.</param>
        /// <param name="drugStatus">The drug status.</param>
        /// <returns></returns>
        public List<Drug> GetFilteredDrugCodesStatus(string text, string drugStatus, string DrugTableNumber)
        {
            text = text.Trim().ToLower();
            drugStatus = drugStatus == "0" ? "Active" : drugStatus;
            var list = _repository.Where(h =>
                       (h.DrugCode.Trim().ToLower().Contains(text) ||
                        h.DrugGenericName.Trim().ToLower().Contains(text) ||
                        h.DrugPackageName.Trim().ToLower().Contains(text)) &&
                       h.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(x => x.Id).ToList();
            list = list.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
            return drugStatus == "All" ? list : list.Where(x => x.DrugStatus.Trim().ToLower().Equals(drugStatus.ToLower())).ToList();
        }


        public List<Drug> GetFilteredDrugCodesData(string text, string drugStatus, string tableNumber)
        {
            text = text.Trim().ToLower();
            drugStatus = drugStatus == "0" ? "Active" : drugStatus;
            var list = _repository.Where(
                    h =>
                        (h.DrugCode.Trim().ToLower().Contains(text) ||
                         h.DrugGenericName.Trim().ToLower().Contains(text) ||
                         h.DrugPackageName.Trim().ToLower().Contains(text)) &&
                        h.DrugTableNumber.Trim().Equals(tableNumber)).OrderByDescending(x => x.Id).ToList();

            //list = list.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();

            return drugStatus == "All" ? list : list.Where(x => x.DrugStatus.Trim().ToLower().Equals(drugStatus.ToLower())).ToList();
        }

        /// <summary>
        /// Gets the drug listby drug code.
        /// </summary>
        /// <param name="drugCode">The drug code.</param>
        /// <returns></returns>
        public List<Drug> GetDrugListbyDrugCode(string drugCode, string DrugTableNumber)
        {
            var drugObj = _repository.Where(h => (h.DrugCode.Contains(drugCode)) && h.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(x => x.Id).ToList();
            drugObj = drugObj.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
            return drugObj;
        }

        /// <summary>
        /// Gets the drug codes by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public Drug GetCurrentDrugByCode(string code, string DrugTableNumber)
        {
            if (string.IsNullOrEmpty(code))
                code = string.Empty;

            code = code.Trim().ToLower();
            var drugCodes = _repository.Where(d => d.DrugCode.Trim().ToLower().Equals(code) && d.DrugTableNumber.Trim().Equals(DrugTableNumber)).FirstOrDefault();
            return drugCodes;
        }

        public string ImportDrugCodesToDB(DataTable dt, Int32 loggedInUser, string tableNumber, string type)
        {
            var returnStr = "";
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter
            {
                ParameterName = "insertDrugExcelToDB",
                SqlDbType = SqlDbType.Structured,
                Value = dt,
                TypeName = "DrugTT"
            };
            sqlParameters[1] = new SqlParameter("LoggedInUser", loggedInUser);
            sqlParameters[2] = new SqlParameter("TableNumber", tableNumber);
            sqlParameters[3] = new SqlParameter("Type", type);
            _repository.ExecuteCommand(StoredProcedures.SPROC_InsertDrugExcelToDB.ToString(), sqlParameters);
            returnStr = "Successfully imported!";
            return returnStr;

        }


        public List<Drug> ExportFilteredDrugCodes(string text, string tableNumber)
        {
            text = text.Trim().ToLower();
            var list =
                _repository.Where(
                    h =>
                        (h.DrugCode.Trim().ToLower().Contains(text) ||
                         h.DrugGenericName.Trim().ToLower().Contains(text) ||
                         h.DrugPackageName.Trim().ToLower().Contains(text)) &&
                        h.DrugTableNumber.Trim().Equals(tableNumber)).OrderByDescending(x => x.Id).ToList();
            list = list.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
            return list;
        }

    }
}

