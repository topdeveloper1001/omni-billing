using System;
using BillingSystem.Model;
using System.Collections.Generic;
using System.Linq;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DRGCodesService : IDRGCodesService
    {
        private readonly IRepository<DRGCodes> _repository;
        private readonly BillingEntities _context;

        public DRGCodesService(IRepository<DRGCodes> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }


        /// <summary>
        /// Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<DRGCodes> GetDrgCodes(string DrgTableNumber)
        {
            var list = _repository.Where(x => (x.IsDeleted == null || !(bool)x.IsDeleted) && x.CodeTableNumber.Trim().Equals(DrgTableNumber) && x.IsActive != false).ToList();
            return list;
        }
        public List<DRGCodes> GetDrgCodesListOnDemand(int blockNumber, int blockSize, string DrgTableNumber)
        {
            int startIndex = (blockNumber - 1) * blockSize;

            var list = _repository.Where(s => (s.IsDeleted == null || !(bool)s.IsDeleted) && s.CodeTableNumber.Trim().Equals(DrgTableNumber)).OrderByDescending(f => f.DRGCodesId).Skip(startIndex).Take(blockSize).ToList();
            return list;
        }
        /// <summary>
        /// Method to add Update the DrgCode in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SaveDrgCode(DRGCodes model, string DrgTableNumber)
        {
            model.CodeTableNumber = DrgTableNumber;
            if (model.DRGCodesId > 0)
            {
                var current = _repository.GetSingle(model.DRGCodesId);
                model.CodeTableNumber = current.CodeTableNumber;
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.DRGCodesId);
            }
            else
                _repository.Create(model);
            return model.DRGCodesId;
        }

        /// <summary>
        /// Method to Get DRG Code in the database.
        /// </summary>
        /// <returns></returns>
        public DRGCodes GetDrgCodesById(int id)
        {
            var drgCodes = _repository.Where(x => x.DRGCodesId == id).FirstOrDefault();
            return drgCodes;
        }

        /// <summary>
        /// Gets the DRG code by DRG code.
        /// </summary>
        /// <param name="drgcode">The drgcode.</param>
        /// <returns></returns>
        public DRGCodes GetDrgCodesobjByCodeValue(string drgcode, string DrgTableNumber)
        {
            var drg = _repository.Where(d => d.CodeNumbering.Equals(drgcode) && d.CodeTableNumber.Trim().Equals(DrgTableNumber)).FirstOrDefault();
            return drg;
        }

        /// <summary>
        /// Gets the DRG description by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public string GetDrgDescriptionByCode(string code, string DrgTableNumber)
        {
            var drg = _repository.Where(d => d.CodeNumbering == code && d.CodeTableNumber.Trim().Equals(DrgTableNumber))
                    .FirstOrDefault();
            return drg != null ? drg.CodeDescription : string.Empty;
        }

        /// <summary>
        /// Gets the DRG code by identifier.
        /// </summary>
        /// <param name="drgId">The DRG identifier.</param>
        /// <returns></returns>
        public string GetDrgCodeById(int drgId, string DrgTableNumber)
        {
            var drg = _repository.Where(d => d.DRGCodesId == drgId && d.CodeTableNumber.Trim().Equals(DrgTableNumber)).FirstOrDefault();
            return drg != null ? drg.CodeNumbering : string.Empty;
        }

        /// <summary>
        /// Gets the filtered DRG codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public List<DRGCodes> GetFilteredDRGCodes(string text, string DrgTableNumber)
        {
            text = text.ToLower().Trim();
            var lstDrgCodes = _repository.Where(d => (d.IsDeleted == null || !(bool)d.IsDeleted) && (d.CodeNumbering.ToLower().Contains(text) || d.CodeDescription.ToLower().Contains(text)) && d.CodeTableNumber.Trim().Equals(DrgTableNumber) && d.IsActive != false).ToList();
            return lstDrgCodes;
        }

        public List<DRGCodes> GetDRGCodesFiltered(string text, string tableNumber)
        {
            text = text.ToLower().Trim();
            var lstDrgCodes = _repository.Where(d => d.IsDeleted != true && (d.CodeNumbering.ToLower().Contains(text) || d.CodeDescription.ToLower().Contains(text)) && d.CodeTableNumber.Trim().Equals(tableNumber) && d.IsActive).ToList();
            return lstDrgCodes;
        }
        public List<DRGCodes> GetActiveInactiveDrgCodes(bool showInActive, string DrgTableNumber)
        {
            var list = _repository.Where(x => (x.IsDeleted == null || !(bool)x.IsDeleted) && x.CodeTableNumber.Trim().Equals(DrgTableNumber) && x.IsActive == showInActive).ToList();
            return list;
        }


        public List<DRGCodes> ExportDRGCodes(string text, string tableNumber)
        {
            text = text.ToLower().Trim();
            var lstDrgCodes = _repository.Where(d => (d.IsDeleted == null || !(bool)d.IsDeleted) && (d.CodeNumbering.ToLower().Contains(text) || d.CodeDescription.ToLower().Contains(text)) && d.CodeTableNumber.Trim().Equals(tableNumber) && d.IsActive != false).ToList();
            return lstDrgCodes;
        }
    }
}
