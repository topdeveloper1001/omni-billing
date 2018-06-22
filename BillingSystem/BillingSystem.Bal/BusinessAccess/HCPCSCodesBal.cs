using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;

namespace BillingSystem.Bal.BusinessAccess
{
    public class HCPCSCodesBal : BaseBal
    {
        public HCPCSCodesBal(string hcpcsTableNumber)
        {
            if (!string.IsNullOrEmpty(hcpcsTableNumber))
                HcpcsTableNumber = hcpcsTableNumber;
        }

        /// <summary>
        /// Get the HCPCS Codes 
        /// </summary>
        /// <returns>Return the HCPCS Codes View Model</returns>
        public List<HCPCSCodes> GetHCPCSCodes()
        {
            try
            {
                using (var HCPCSCodesRep = UnitOfWork.HCPCSCodesRepository)
                {
                    var list = HCPCSCodesRep.Where(x => x.IsActive == true && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockNumber"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public List<HCPCSCodes> GetHCPCSCodesListOnDemand(int blockNumber, int blockSize)
        {
            try
            {
                int startIndex = (blockNumber - 1) * blockSize;
                using (var HCPCSCodesRep = UnitOfWork.HCPCSCodesRepository)
                {
                    //var lstHCPCSCode =
                    //    HCPCSCodesRep.Where(
                    //        s =>
                    //            (s.IsDeleted == null || !(bool)s.IsDeleted) &&
                    //            s.IsActive == true &&
                    //            s.CodeTableNumber.Trim().Equals(HcpcsTableNumber))
                    //        .OrderByDescending(f => f.HCPCSCodesId)
                    //        .Skip(startIndex)
                    //        .Take(blockSize)
                    //        .ToList();
                    var lstHCPCSCode =
                        HCPCSCodesRep.Where(
                            s =>
                                s.IsActive == true &&
                                s.CodeTableNumber.Trim().Equals(HcpcsTableNumber))
                            .OrderByDescending(f => f.HCPCSCodesId)
                            .Skip(startIndex)
                            .Take(blockSize)
                            .ToList();
                    return lstHCPCSCode;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Method to add the ServiceCode in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddHCPCSCodes(HCPCSCodes model)
        {
            using (var repository = UnitOfWork.HCPCSCodesRepository)
            {
                model.CodeTableNumber = HcpcsTableNumber.Trim();
                if (model.HCPCSCodesId > 0)
                {
                    var id = repository.UpdateEntity(model, model.HCPCSCodesId);
                    var current = repository.GetSingle(model.HCPCSCodesId);

                    model.CodeTableNumber = current.CodeTableNumber;
                    return id != null ? Convert.ToInt32(id) : -1;
                }
                var newId = repository.Create(model);
                return newId != null ? Convert.ToInt32(newId) : -1;
            }
        }

        /// <summary>
        /// Method to add the Service Code in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public HCPCSCodes GetHCPCSCodesById(int id)
        {
            using (var rep = UnitOfWork.HCPCSCodesRepository)
            {
                var current =
                    rep.Where(a => a.HCPCSCodesId == id)
                        .FirstOrDefault();
                return current;
            }
        }

        public List<HCPCSCodes> GetFilteredHCPCSCodes(string text)
        {
            try
            {
                using (var HCPCSCodesRep = UnitOfWork.HCPCSCodesRepository)
                {
                    var list =
                        HCPCSCodesRep.Where(h => (h.CodeNumbering.Contains(text) || h.CodeDescription.Contains(text)
                            ) && h.IsActive != false && h.IsDeleted == false &&
                                                 h.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).Take(100).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<HCPCSCodes> GetHCPCSCodesFilterData(string text, string tableNumber)
        {
            try
            {
                using (var HCPCSCodesRep = UnitOfWork.HCPCSCodesRepository)
                {
                    var list =
                        HCPCSCodesRep.Where(h => (h.CodeNumbering.Contains(text) || h.CodeDescription.Contains(text)
                            ) && h.IsActive != false && h.IsDeleted != true &&
                                                 h.CodeTableNumber.Trim().Equals(tableNumber)).Take(100).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetHCPCSCodeDescription(string codeid)
        {
            using (var hcpcsCodesRep = UnitOfWork.HCPCSCodesRepository)
            {
                var hcpcsCodes = hcpcsCodesRep.Where(x => x.CodeNumbering == codeid && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).FirstOrDefault();
                return hcpcsCodes != null ? hcpcsCodes.CodeDescription : string.Empty;
            }
        }
        public List<HCPCSCodes> GetActiveInActiveHCPCSCodes(bool showInActive)
        {
            try
            {
                using (var HCPCSCodesRep = UnitOfWork.HCPCSCodesRepository)
                {
                    var list = HCPCSCodesRep.Where(x => x.IsActive == showInActive && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<HCPCSCodes> ExportHCPCSCodes(string text, string tableNumber)
        {
            try
            {
                using (var HCPCSCodesRep = UnitOfWork.HCPCSCodesRepository)
                {
                    var list =
                        HCPCSCodesRep.Where(h => (h.CodeNumbering.Contains(text) || h.CodeDescription.Contains(text)
                            ) && h.IsActive != false && h.IsDeleted == false &&
                                                 h.CodeTableNumber.Trim().Equals(tableNumber)).Take(100).ToList();
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
