using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using BillingSystem.Model;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DrugBal : BaseBal
    {
        public DrugBal(string drugTableNumber)
        {
            if (!string.IsNullOrEmpty(drugTableNumber))
                DrugTableNumber = drugTableNumber;
        }


        /// <summary>
        /// Get Entity Name By Id
        /// </summary>
        /// <param name="model">The drug.</param>
        /// <returns>
        /// Return the Entity Respository
        /// </returns>
        public int AddUptdateDrug(Drug model)
        {
            using (var rep = UnitOfWork.DrugRepository)
            {
                model.DrugTableNumber = DrugTableNumber;
                if (model.Id > 0)
                {
                    var current = rep.GetSingle(model.Id);
                    model.DrugTableNumber = current.DrugTableNumber;
                    model.DrugDescription = current.DrugDescription;
                    rep.UpdateEntity(model, model.Id);
                }
                else
                    rep.Create(model);
                return model.Id;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="drugId">The drug identifier.</param>
        /// <returns></returns>
        public Drug GetDrugByID(int? drugId)
        {
            using (var DrugRep = UnitOfWork.DrugRepository)
            {
                var model =
                    DrugRep.Where(x => x.Id == drugId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Gets the drug list.
        /// </summary> 
        /// <returns></returns>
        public List<Drug> GetDrugList()
        {
            using (var rep = UnitOfWork.DrugRepository)
            {
                var lstDrug = rep.Where(d => !d.DrugStatus.Equals("Deleted") && d.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(x => x.Id).ToList();
                lstDrug = lstDrug.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
                return lstDrug.OrderBy(x => x.DrugCode).ToList();
            }
        }

        /// <summary>
        /// Gets the drug list by drug view.
        /// </summary>Updated By Krishna on 08072015
        /// <param name="ViewVal">The view value.</param>
        /// <returns></returns>
        public List<Drug> GetDrugListByDrugView(string ViewVal)
        {
            using (var rep = UnitOfWork.DrugRepository)
            {
                var drugList =
                    rep.Where(
                        a =>
                            a.DrugStatus.ToLower().Equals(ViewVal.ToLower()) &&
                            a.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(x => x.Id).ToList();
                drugList = drugList.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
                return ViewVal == "All" ? rep.Where(d => !d.DrugStatus.Equals("Deleted") && d.DrugTableNumber.Trim().Equals(DrugTableNumber)).ToList() : drugList;
            }
        }

        public List<Drug> GetDrugListOnDemand(int blockNumber, int blockSize, string viewVal)
        {
            try
            {
                int startIndex = (blockNumber - 1) * blockSize;
                using (var rep = UnitOfWork.DrugRepository)
                {
                    var list =
                        rep.Where(
                            s =>
                                s.DrugStatus.ToLower().Equals(viewVal.ToLower()) &&
                                s.DrugTableNumber.Trim().Equals(DrugTableNumber))
                            .OrderByDescending(f => f.Id)
                            .ToList();
                    list =
                        list.GroupBy(x => x.DrugCode)
                            .Select(x => x.First())
                            .ToList()
                            .Skip(startIndex)
                            .Take(blockSize)
                            .ToList();
                    return viewVal == "All"
                        ? rep.Where(d => !d.DrugStatus.Equals("Deleted") && d.DrugTableNumber.Trim().Equals(DrugTableNumber))
                            .OrderByDescending(f => f.Id)
                            .Skip(startIndex)
                            .Take(blockSize).ToList()
                        : list;
                }
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
        public List<Drug> GetDrugListbyBrandCode(string brandCode)
        {
            using (var rep = UnitOfWork.DrugRepository)
            {
                var lstDrug = rep.Where(d => (d.DrugStatus.Equals("Active") || d.DrugStatus.Equals("Grace")) && d.BrandCode == brandCode && d.InStock == true && d.DrugTableNumber.Trim().Equals(DrugTableNumber)).ToList();
                lstDrug = lstDrug.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
                return lstDrug.OrderBy(x => x.DrugCode).ToList();
            }
        }

        /// <summary>
        /// Gets the drug code description.
        /// </summary>
        /// <param name="codeid">The codeid.</param>
        /// <returns></returns>
        public string GetDRUGCodeDescription(string codeid)
        {
            using (var rep = UnitOfWork.DrugRepository)
            {
                var model = rep.Where(x => x.DrugCode == codeid && x.DrugTableNumber.Trim().Equals(DrugTableNumber)).FirstOrDefault();
                return model != null ? model.DrugPackageName : string.Empty;
            }
        }

        /// <summary>
        /// Gets the filtered drug codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public List<Drug> GetFilteredDrugCodes(string text)
        {
            try
            {
                using (var rep = UnitOfWork.DrugRepository)
                {
                    text = text.Trim().ToLower();
                    var list =
                        rep.Where(
                            h =>
                                (h.DrugCode.Trim().ToLower().Contains(text) ||
                                 h.DrugGenericName.Trim().ToLower().Contains(text) ||
                                 h.DrugPackageName.Trim().ToLower().Contains(text)) &&
                                h.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(x => x.Id).ToList();
                    list = list.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the filtered drug codes status.
        /// </summary> Udated by krishna
        /// <param name="text">The text.</param>
        /// <param name="drugStatus">The drug status.</param>
        /// <returns></returns>
        public List<Drug> GetFilteredDrugCodesStatus(string text, string drugStatus)
        {
            try
            {
                using (var rep = UnitOfWork.DrugRepository)
                {
                    text = text.Trim().ToLower();
                    drugStatus = drugStatus == "0" ? "Active" : drugStatus;
                    var list =
                        rep.Where(
                            h =>
                                (h.DrugCode.Trim().ToLower().Contains(text) ||
                                 h.DrugGenericName.Trim().ToLower().Contains(text) ||
                                 h.DrugPackageName.Trim().ToLower().Contains(text)) &&
                                h.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(x => x.Id).ToList();
                    list = list.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
                    return drugStatus == "All" ? list : list.Where(x => x.DrugStatus.Trim().ToLower().Equals(drugStatus.ToLower())).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<Drug> GetFilteredDrugCodesData(string text, string drugStatus, string tableNumber)
        {
            try
            {
                using (var rep = UnitOfWork.DrugRepository)
                {
                    text = text.Trim().ToLower();
                    drugStatus = drugStatus == "0" ? "Active" : drugStatus;
                    var list = rep.Where(
                            h =>
                                (h.DrugCode.Trim().ToLower().Contains(text) ||
                                 h.DrugGenericName.Trim().ToLower().Contains(text) ||
                                 h.DrugPackageName.Trim().ToLower().Contains(text)) &&
                                h.DrugTableNumber.Trim().Equals(tableNumber)).OrderByDescending(x => x.Id).ToList();

                    //list = list.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();

                    return drugStatus == "All" ? list : list.Where(x => x.DrugStatus.Trim().ToLower().Equals(drugStatus.ToLower())).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the drug listby drug code.
        /// </summary>
        /// <param name="drugCode">The drug code.</param>
        /// <returns></returns>
        public List<Drug> GetDrugListbyDrugCode(string drugCode)
        {
            try
            {
                using (var rep = UnitOfWork.DrugRepository)
                {
                    var drugObj =
                        rep.Where(h => (h.DrugCode.Contains(drugCode)) && h.DrugTableNumber.Trim().Equals(DrugTableNumber))
                            .OrderByDescending(x => x.Id).ToList();
                    drugObj = drugObj.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
                    return drugObj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the drug codes by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public Drug GetCurrentDrugByCode(string code)
        {
            using (var drugRep = UnitOfWork.DrugRepository)
            {
                if (string.IsNullOrEmpty(code))
                    code = string.Empty;

                code = code.Trim().ToLower();
                var drugCodes =
                    drugRep.Where(
                        d => d.DrugCode.Trim().ToLower().Equals(code) && d.DrugTableNumber.Trim().Equals(DrugTableNumber))
                        .FirstOrDefault();
                return drugCodes;
            }
        }

        public string ImportDrugCodesToDB(DataTable dt, Int32 loggedInUser, string tableNumber, string type)
        {
            var returnStr = "";
            using (var rep = UnitOfWork.DrugRepository)
            {
               returnStr = rep.ImportDrugCodesToDB(dt, loggedInUser, tableNumber, type);
            }
            return returnStr;
        }


        public List<Drug> ExportFilteredDrugCodes(string text,string tableNumber)
        {
            try
            {
                using (var rep = UnitOfWork.DrugRepository)
                {
                    text = text.Trim().ToLower();
                    var list =
                        rep.Where(
                            h =>
                                (h.DrugCode.Trim().ToLower().Contains(text) ||
                                 h.DrugGenericName.Trim().ToLower().Contains(text) ||
                                 h.DrugPackageName.Trim().ToLower().Contains(text)) &&
                                h.DrugTableNumber.Trim().Equals(tableNumber)).OrderByDescending(x => x.Id).ToList();
                    list = list.GroupBy(x => x.DrugCode).Select(x => x.First()).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

