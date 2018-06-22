using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DiagnosisCodeBal : BaseBal
    {
        public DiagnosisCodeBal(string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(diagnosisTableNumber))
                DiagnosisTableNumber = diagnosisTableNumber;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<DiagnosisCode> GetDiagnosisCode()
        {
            try
            {
                using (var diagnosisCodeRep = UnitOfWork.DiagnosisCodeRepository)
                {
                    var lstDiagnosisCode = diagnosisCodeRep.Where(x => x.IsDeleted == false && x.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).OrderByDescending(f => f.DiagnosisTableNumberId).ToList();
                    return lstDiagnosisCode;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DiagnosisCode> GetListOnDemand(int blockNumber, int blockSize)
        {
            try
            {
                int startIndex = (blockNumber - 1) * blockSize;
                using (var diagnosisCodeRep = UnitOfWork.DiagnosisCodeRepository)
                {
                    var lstDiagnosisCode =
                        diagnosisCodeRep.Where(
                            x => x.IsDeleted == false && x.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber))
                            .OrderByDescending(f => f.DiagnosisTableNumberId)
                            .Skip(startIndex)
                            .Take(blockSize)
                            .ToList();
                    return lstDiagnosisCode;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUptdateDiagnosisCode(DiagnosisCode model)
        {
            using (var rep = UnitOfWork.DiagnosisCodeRepository)
            {
                model.DiagnosisTableNumber = DiagnosisTableNumber;
                if (model.DiagnosisTableNumberId > 0)
                {
                    var current = rep.GetSingle(model.DiagnosisTableNumberId);
                    model.DiagnosisTableNumber = current.DiagnosisTableNumber;
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(model, model.DiagnosisTableNumberId);
                }
                else
                    rep.Create(model);
                return model.DiagnosisTableNumberId;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <param name="diagnosisCodeId"></param>
        /// <returns></returns>
        public DiagnosisCode GetDiagnosisCodeByID(int? diagnosisCodeId)
        {
            using (var diagnosisCodeRep = UnitOfWork.DiagnosisCodeRepository)
            {
                var diagnosisCode =
                    diagnosisCodeRep.Where(
                        x =>
                            x.DiagnosisTableNumberId == diagnosisCodeId && x.IsDeleted == false).FirstOrDefault();
                return diagnosisCode;
            }
        }

        public List<DiagnosisCode> GetFilteredDiagnosisCodes(string text)
        {
            try
            {
                using (var diagnosisCodeRep = UnitOfWork.DiagnosisCodeRepository)
                {
                    var lstdiagnosisCode =
                        diagnosisCodeRep.Where(
                                x =>
                                    x.IsDeleted == false &&
                                    (x.DiagnosisCode1.Contains(text) || x.ShortDescription.Contains(text)) &&
                                    x.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber))
                            .ToList();
                    return lstdiagnosisCode;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DiagnosisCode> GetFilteredDiagnosisCodesData(string text, string tableNumber)
        {
            try
            {
                using (var diagnosisCodeRep = UnitOfWork.DiagnosisCodeRepository)
                {
                    var lstdiagnosisCode =
                        diagnosisCodeRep.Where(
                                x =>
                                    x.IsDeleted != true &&
                                    (x.DiagnosisCode1.Contains(text) || x.ShortDescription.Contains(text) || x.DiagnosisMediumDescription.Contains(text)) &&
                                    x.DiagnosisTableNumber.Trim().Equals(tableNumber))
                            .ToList();
                    return lstdiagnosisCode;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DiagnosisCode GetDiagnosisCodeByCodeId(string id)
        {
            try
            {
                using (var rep = UnitOfWork.DiagnosisCodeRepository)
                {
                    var model =
                        rep.Where(
                            x =>
                                x.IsDeleted == false && (x.DiagnosisCode1.Trim().Equals(id)) &&
                                x.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).FirstOrDefault();
                    return model ?? new DiagnosisCode();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the diagnosis code desc by identifier.
        /// </summary>
        /// <param name="codeId">The code identifier.</param>
        /// <returns></returns>
        public string GetDiagnosisCodeDescById(string codeId)
        {
            using (var rep = UnitOfWork.DiagnosisCodeRepository)
            {
                var model =
                    rep.Where(d => d.DiagnosisCode1 == codeId && d.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber))
                        .FirstOrDefault();
                return model != null ? model.DiagnosisFullDescription : "";
            }
        }

        public List<DiagnosisCode> GetDiagnosisCodeData(bool showInActive)
        {
            try
            {
                using (var diagnosisCodeRep = UnitOfWork.DiagnosisCodeRepository)
                {
                    var lstDiagnosisCode = diagnosisCodeRep.Where(x => x.IsDeleted == showInActive && x.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).OrderByDescending(f => f.DiagnosisTableNumberId).ToList();
                    return lstDiagnosisCode;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

