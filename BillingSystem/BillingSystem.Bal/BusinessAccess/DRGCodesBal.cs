using System;
using BillingSystem.Model;
using System.Collections.Generic;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DRGCodesBal : BaseBal
    {
        public DRGCodesBal(string drgTableNumber)
        {
            if (!string.IsNullOrEmpty(drgTableNumber))
                DrgTableNumber = drgTableNumber;
        }

        /// <summary>
        /// Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<DRGCodes> GetDrgCodes()
        {
            using (var rep = UnitOfWork.DRGCodesRepository)
            {
                var list =
                    rep.Where(
                        _ =>
                            (_.IsDeleted == null || !(bool)_.IsDeleted) && _.CodeTableNumber.Trim().Equals(DrgTableNumber) && _.IsActive != false)
                        .ToList();
                return list;
            }
        }
        public List<DRGCodes> GetDrgCodesListOnDemand(int blockNumber, int blockSize)
        {
            try
            {
                int startIndex = (blockNumber - 1) * blockSize;
                using (var rep = UnitOfWork.DRGCodesRepository)
                {
                    var list =
                        rep.Where(
                            s =>
                                (s.IsDeleted == null || !(bool)s.IsDeleted) &&
                                s.CodeTableNumber.Trim().Equals(DrgTableNumber))
                                .OrderByDescending(f => f.DRGCodesId)
                                .Skip(startIndex)
                                .Take(blockSize)
                                .ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Method to add Update the DrgCode in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SaveDrgCode(DRGCodes model)
        {
            using (var rep = UnitOfWork.DRGCodesRepository)
            {
                model.CodeTableNumber = DrgTableNumber;
                if (model.DRGCodesId > 0)
                {
                    var current = rep.GetSingle(model.DRGCodesId);
                    model.CodeTableNumber = current.CodeTableNumber;
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(model, model.DRGCodesId);
                }
                else
                    rep.Create(model);
                return model.DRGCodesId;
            }
        }

        /// <summary>
        /// Method to Get DRG Code in the database.
        /// </summary>
        /// <returns></returns>
        public DRGCodes GetDrgCodesById(int id)
        {
            using (var rep = UnitOfWork.DRGCodesRepository)
            {
                var drgCodes = rep.Where(x => x.DRGCodesId == id).FirstOrDefault();
                return drgCodes;
            }
        }

        /// <summary>
        /// Gets the DRG code by DRG code.
        /// </summary>
        /// <param name="drgcode">The drgcode.</param>
        /// <returns></returns>
        public DRGCodes GetDrgCodesobjByCodeValue(string drgcode)
        {
            using (var rep = UnitOfWork.DRGCodesRepository)
            {
                var drg = rep.Where(d => d.CodeNumbering.Equals(drgcode) && d.CodeTableNumber.Trim().Equals(DrgTableNumber)).FirstOrDefault();
                return drg;
            }
        }

        /// <summary>
        /// Gets the DRG description by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public string GetDrgDescriptionByCode(string code)
        {
            using (var rep = UnitOfWork.DRGCodesRepository)
            {
                var drg =
                    rep.Where(d => d.CodeNumbering == code && d.CodeTableNumber.Trim().Equals(DrgTableNumber))
                        .FirstOrDefault();
                return drg != null ? drg.CodeDescription : string.Empty;
            }
        }

        /// <summary>
        /// Gets the DRG code by identifier.
        /// </summary>
        /// <param name="drgId">The DRG identifier.</param>
        /// <returns></returns>
        public string GetDrgCodeById(int drgId)
        {
            using (var rep = UnitOfWork.DRGCodesRepository)
            {
                var drg = rep.Where(d => d.DRGCodesId == drgId && d.CodeTableNumber.Trim().Equals(DrgTableNumber)).FirstOrDefault();
                return drg != null ? drg.CodeNumbering : string.Empty;
            }
        }

        /// <summary>
        /// Gets the filtered DRG codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public List<DRGCodes> GetFilteredDRGCodes(string text)
        {
            try
            {
                text = text.ToLower().Trim();
                using (var drgCodesRep = UnitOfWork.DRGCodesRepository)
                {
                    var lstDrgCodes =
                        drgCodesRep.Where(
                            d =>
                                (d.IsDeleted == null || !(bool)d.IsDeleted) &&
                                (d.CodeNumbering.ToLower().Contains(text) || d.CodeDescription.ToLower().Contains(text)) &&
                                d.CodeTableNumber.Trim().Equals(DrgTableNumber) && d.IsActive != false).ToList();
                    return lstDrgCodes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DRGCodes> GetDRGCodesFiltered(string text, string tableNumber)
        {
            try
            {
                text = text.ToLower().Trim();
                using (var drgCodesRep = UnitOfWork.DRGCodesRepository)
                {
                    var lstDrgCodes =
                        drgCodesRep.Where(
                            d =>
                                d.IsDeleted != true &&
                                (d.CodeNumbering.ToLower().Contains(text) || d.CodeDescription.ToLower().Contains(text)) &&
                                d.CodeTableNumber.Trim().Equals(tableNumber) && d.IsActive).ToList();
                    return lstDrgCodes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DRGCodes> GetActiveInactiveDrgCodes(bool showInActive)
        {
            using (var rep = UnitOfWork.DRGCodesRepository)
            {
                var list =
                    rep.Where(
                        _ =>
                            (_.IsDeleted == null || !(bool)_.IsDeleted) && _.CodeTableNumber.Trim().Equals(DrgTableNumber) && _.IsActive == showInActive)
                        .ToList();
                return list;
            }
        }


        public List<DRGCodes> ExportDRGCodes(string text, string tableNumber)
        {
            try
            {
                text = text.ToLower().Trim();
                using (var drgCodesRep = UnitOfWork.DRGCodesRepository)
                {
                    var lstDrgCodes =
                        drgCodesRep.Where(
                            d =>
                                (d.IsDeleted == null || !(bool)d.IsDeleted) &&
                                (d.CodeNumbering.ToLower().Contains(text) || d.CodeDescription.ToLower().Contains(text)) &&
                                d.CodeTableNumber.Trim().Equals(tableNumber) && d.IsActive != false).ToList();
                    return lstDrgCodes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
