using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class HCPCSCodesService : IHCPCSCodesService
    {
        private readonly IRepository<HCPCSCodes> _repository;
        private readonly BillingEntities _context;

        public HCPCSCodesService(IRepository<HCPCSCodes> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Get the HCPCS Codes 
        /// </summary>
        /// <returns>Return the HCPCS Codes View Model</returns>
        public List<HCPCSCodes> GetHCPCSCodes(string HcpcsTableNumber)
        {
            var list = _repository.Where(x => x.IsActive == true && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).ToList();
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockNumber"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public List<HCPCSCodes> GetHCPCSCodesListOnDemand(int blockNumber, int blockSize, string HcpcsTableNumber)
        {
            int startIndex = (blockNumber - 1) * blockSize;
            var lstHCPCSCode = _repository.Where(s => s.IsActive == true && s.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).OrderByDescending(f => f.HCPCSCodesId).Skip(startIndex).Take(blockSize).ToList();
            return lstHCPCSCode;
        }
        /// <summary>
        /// Method to add the ServiceCode in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddHCPCSCodes(HCPCSCodes model, string HcpcsTableNumber)
        {
            model.CodeTableNumber = HcpcsTableNumber.Trim();
            if (model.HCPCSCodesId > 0)
            {
                var id = _repository.UpdateEntity(model, model.HCPCSCodesId);
                var current = _repository.GetSingle(model.HCPCSCodesId);

                model.CodeTableNumber = current.CodeTableNumber;
                return id != null ? Convert.ToInt32(id) : -1;
            }
            var newId = _repository.Create(model);
            return newId != null ? Convert.ToInt32(newId) : -1;

        }

        /// <summary>
        /// Method to add the Service Code in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public HCPCSCodes GetHCPCSCodesById(int id)
        {
            var current = _repository.Where(a => a.HCPCSCodesId == id).FirstOrDefault();
            return current;
        }

        public List<HCPCSCodes> GetFilteredHCPCSCodes(string text, string HcpcsTableNumber)
        {
            var list =
                _repository.Where(h => (h.CodeNumbering.Contains(text) || h.CodeDescription.Contains(text)
                    ) && h.IsActive != false && h.IsDeleted == false &&
                                         h.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).Take(100).ToList();
            return list;
        }


        public List<HCPCSCodes> GetHCPCSCodesFilterData(string text, string tableNumber)
        {
            var list = _repository.Where(h => (h.CodeNumbering.Contains(text) || h.CodeDescription.Contains(text)) && h.IsActive != false && h.IsDeleted != true && h.CodeTableNumber.Trim().Equals(tableNumber)).Take(100).ToList();
            return list;
        }

        public string GetHCPCSCodeDescription(string codeid, string HcpcsTableNumber)
        {
            var hcpcsCodes = _repository.Where(x => x.CodeNumbering == codeid && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).FirstOrDefault();
            return hcpcsCodes != null ? hcpcsCodes.CodeDescription : string.Empty;
        }
        public List<HCPCSCodes> GetActiveInActiveHCPCSCodes(bool showInActive, string HcpcsTableNumber)
        {
            var list = _repository.Where(x => x.IsActive == showInActive && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).ToList();
            return list;
        }


        public List<HCPCSCodes> ExportHCPCSCodes(string text, string tableNumber)
        {
            var list = _repository.Where(h => (h.CodeNumbering.Contains(text) || h.CodeDescription.Contains(text)) && h.IsActive != false && h.IsDeleted == false && h.CodeTableNumber.Trim().Equals(tableNumber)).Take(100).ToList();
            return list;
        }

    }
}
