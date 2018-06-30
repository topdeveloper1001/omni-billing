using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DiagnosisCodeService : IDiagnosisCodeService
    {
        private readonly IRepository<DiagnosisCode> _repository;

        public DiagnosisCodeService(IRepository<DiagnosisCode> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<DiagnosisCode> GetDiagnosisCode(string DiagnosisTableNumber)
        {
            var lst = _repository.Where(x => x.IsDeleted == false && x.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).OrderByDescending(f => f.DiagnosisTableNumberId).ToList();
            return lst;
        }

        public List<DiagnosisCode> GetListOnDemand(int blockNumber, int blockSize, string DiagnosisTableNumber)
        {
            int startIndex = (blockNumber - 1) * blockSize;
            var lst = _repository.Where(x => x.IsDeleted == false && x.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).OrderByDescending(f => f.DiagnosisTableNumberId).Skip(startIndex).Take(blockSize).ToList();
            return lst;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public int AddUptdateDiagnosisCode(DiagnosisCode m, string DiagnosisTableNumber)
        {
            m.DiagnosisTableNumber = DiagnosisTableNumber;
            if (m.DiagnosisTableNumberId > 0)
            {
                var current = _repository.GetSingle(m.DiagnosisTableNumberId);
                m.DiagnosisTableNumber = current.DiagnosisTableNumber;
                m.CreatedBy = current.CreatedBy;
                m.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(m, m.DiagnosisTableNumberId);
            }
            else
                _repository.Create(m);
            return m.DiagnosisTableNumberId;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <param name="diagnosisCodeId"></param>
        /// <returns></returns>
        public DiagnosisCode GetDiagnosisCodeByID(int? diagnosisCodeId)
        {
            var m = _repository.Where(x => x.DiagnosisTableNumberId == diagnosisCodeId && x.IsDeleted == false).FirstOrDefault();
            return m;
        }

        public List<DiagnosisCode> GetFilteredDiagnosisCodes(string text, string DiagnosisTableNumber)
        {
            var lst = _repository.Where(x => x.IsDeleted == false && (x.DiagnosisCode1.Contains(text) || x.ShortDescription.Contains(text)) && x.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).ToList();
            return lst;

        }

        public List<DiagnosisCode> GetFilteredDiagnosisCodesData(string text, string tableNumber)
        {
            var lst = _repository.Where(x => x.IsDeleted != true && (x.DiagnosisCode1.Contains(text) || x.ShortDescription.Contains(text) || x.DiagnosisMediumDescription.Contains(text)) && x.DiagnosisTableNumber.Trim().Equals(tableNumber)).ToList();
            return lst;
        }

        public DiagnosisCode GetDiagnosisCodeByCodeId(string id, string DiagnosisTableNumber)
        {
            var m = _repository.Where(x => x.IsDeleted == false && (x.DiagnosisCode1.Trim().Equals(id)) && x.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).FirstOrDefault();
            return m ?? new DiagnosisCode();

        }

        /// <summary>
        /// Gets the diagnosis code desc by identifier.
        /// </summary>
        /// <param name="codeId">The code identifier.</param>
        /// <returns></returns>
        public string GetDiagnosisCodeDescById(string codeId, string DiagnosisTableNumber)
        {
            var model = _repository.Where(d => d.DiagnosisCode1 == codeId && d.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber))
                    .FirstOrDefault();
            return model != null ? model.DiagnosisFullDescription : "";
        }

        public List<DiagnosisCode> GetDiagnosisCodeData(bool showInActive, string DiagnosisTableNumber)
        {
            var mlst = _repository.Where(x => x.IsDeleted == showInActive && x.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).OrderByDescending(f => f.DiagnosisTableNumberId).ToList();
            return mlst;
        }

    }
}

