using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class XPaymentReturnService : IXPaymentReturnService
    {
        private readonly IRepository<XPaymentReturn> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;

        public XPaymentReturnService(IRepository<XPaymentReturn> repository, IRepository<GlobalCodes> gRepository)
        {
            _repository = repository;
            _gRepository = gRepository;
        }


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<XPaymentReturnCustomModel> GetXPaymentReturn()
        {
            var list = new List<XPaymentReturnCustomModel>();
            var lstXPaymentReturn = _repository.GetAll().ToList();
            if (lstXPaymentReturn.Count > 0)
            {
                list.AddRange(lstXPaymentReturn.Select(item => new XPaymentReturnCustomModel
                {
                    XPaymentReturnID = item.XPaymentReturnID,
                    SenderID = item.SenderID,
                    ReceiverID = item.ReceiverID,
                    TransactionDate = item.TransactionDate,
                    RecordCount = item.RecordCount,
                    DispositionFlag = item.DispositionFlag,
                    ID = item.ID,
                    IDPayer = item.IDPayer,
                    ProviderID = item.ProviderID,
                    DenialCode = item.DenialCode,
                    PaymentReference = item.PaymentReference,
                    DateSettlement = item.DateSettlement,
                    FacilityID = item.FacilityID,
                    AActivityID = item.AActivityID,
                    AStart = item.AStart,
                    AType = item.AType,
                    ACode = item.ACode,
                    AQuantity = item.AQuantity,
                    AANet = item.AANet,
                    AAOrderingClinician = item.AAOrderingClinician,
                    AAPriorAuthorizationID = item.AAPriorAuthorizationID,
                    AAGross = item.AAGross,
                    AAPatientShare = item.AAPatientShare,
                    AAPaymentAmount = item.AAPaymentAmount,
                    AADenialCode = item.AADenialCode,
                    XModifiedBy = item.XModifiedBy,
                    XModifiedDate = item.XModifiedDate,
                    AdviceStatus = item.AdviceStatus,
                    XCorporateID = item.XCorporateID,
                    XFacilityID = item.XFacilityID,
                }));
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveXPaymentReturn(XPaymentReturn model)
        {
            if (model.XPaymentReturnID > 0)
                _repository.UpdateEntity(model, Convert.ToInt32(model.XPaymentReturnID));
            else
                _repository.Create(model);
            return Convert.ToInt32(model.XPaymentReturnID);
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="xPaymentReturnId">The x payment return identifier.</param>
        /// <returns></returns>
        public XPaymentReturnCustomModel GetXPaymentReturnById(int? xPaymentReturnId)
        {
            var model = _repository.Where(x => x.XPaymentReturnID == xPaymentReturnId).FirstOrDefault();
            if (model != null)
            {
                var xPaymentReturnCustomModelobj = new XPaymentReturnCustomModel()
                {
                    XPaymentReturnID = model.XPaymentReturnID,
                    SenderID = model.SenderID,
                    ReceiverID = model.ReceiverID,
                    TransactionDate = model.TransactionDate,
                    RecordCount = model.RecordCount,
                    DispositionFlag = model.DispositionFlag,
                    ID = model.ID,
                    IDPayer = model.IDPayer,
                    ProviderID = model.ProviderID,
                    DenialCode = model.DenialCode,
                    PaymentReference = model.PaymentReference,
                    DateSettlement = model.DateSettlement,
                    FacilityID = model.FacilityID,
                    AActivityID = model.AActivityID,
                    AStart = model.AStart,
                    AType = model.AType,
                    ACode = model.ACode,
                    AQuantity = model.AQuantity,
                    AANet = model.AANet,
                    AAOrderingClinician = model.AAOrderingClinician,
                    AAPriorAuthorizationID = model.AAPriorAuthorizationID,
                    AAGross = model.AAGross,
                    AAPatientShare = model.AAPatientShare,
                    AAPaymentAmount = model.AAPaymentAmount,
                    AADenialCode = model.AADenialCode,
                    XModifiedBy = model.XModifiedBy,
                    XModifiedDate = model.XModifiedDate,
                    AdviceStatus = model.AdviceStatus,
                    XCorporateID = model.XCorporateID,
                    XFacilityID = model.XFacilityID,
                    ActivityTypeName = GetNameByGlobalCodeId(Convert.ToInt32(model.AType))
                };
                return xPaymentReturnCustomModelobj;
            }
            return new XPaymentReturnCustomModel();
        }
        private string GetNameByGlobalCodeId(int id)
        {
            var gl = _gRepository.Where(g => g.GlobalCodeID == id).FirstOrDefault();
            return gl != null ? gl.GlobalCodeName : string.Empty;
        }
        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }
        /// <summary>
        /// Gets the x payment model return by identifier.
        /// </summary>
        /// <param name="xPaymentReturnId">The x payment return identifier.</param>
        /// <returns></returns>
        public XPaymentReturn GetXPaymentModelReturnById(int? xPaymentReturnId)
        {
            var model = _repository.Where(x => x.XPaymentReturnID == xPaymentReturnId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Generates the remittance information.
        /// </summary>
        /// <param name="claimid">The claimid.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public bool GenerateRemittanceInfo(int claimid, int corporateId, int facilityId)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("PassedInClaim ", claimid);
            sqlParameters[1] = new SqlParameter("CorporateID", corporateId);
            sqlParameters[2] = new SqlParameter("FacilityID", facilityId);
            _repository.ExecuteCommand(StoredProcedures.GenerateRemittanceInfo.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Gets the x payment return by claim identifier.
        /// </summary>
        /// <param name="claimId">The claim identifier.</param>
        /// <returns></returns>
        public List<XPaymentReturnCustomModel> GetXPaymentReturnByClaimId(int claimId)
        {
            var list = new List<XPaymentReturnCustomModel>();
            var lstXPaymentReturn = _repository.Where(x => x.ID == claimId && (x.AdviceStatus == 0 || x.AdviceStatus == null)).ToList();
            if (lstXPaymentReturn.Any())
            {
                list.AddRange(lstXPaymentReturn.Select(item => new XPaymentReturnCustomModel
                {
                    XPaymentReturnID = item.XPaymentReturnID,
                    SenderID = item.SenderID,
                    ReceiverID = item.ReceiverID,
                    TransactionDate = item.TransactionDate,
                    RecordCount = item.RecordCount,
                    DispositionFlag = item.DispositionFlag,
                    ID = item.ID,
                    IDPayer = item.IDPayer,
                    ProviderID = item.ProviderID,
                    DenialCode = item.DenialCode,
                    PaymentReference = item.PaymentReference,
                    DateSettlement = item.DateSettlement,
                    FacilityID = item.FacilityID,
                    AActivityID = item.AActivityID,
                    AStart = item.AStart,
                    AType = item.AType,
                    ACode = item.ACode,
                    AQuantity = item.AQuantity,
                    AANet = item.AANet,
                    AAOrderingClinician = item.AAOrderingClinician,
                    AAPriorAuthorizationID = item.AAPriorAuthorizationID,
                    AAGross = item.AAGross,
                    AAPatientShare = item.AAPatientShare,
                    AAPaymentAmount = item.AAPaymentAmount,
                    AADenialCode = item.AADenialCode,
                    XModifiedBy = item.XModifiedBy,
                    XModifiedDate = item.XModifiedDate,
                    AdviceStatus = item.AdviceStatus,
                    XCorporateID = item.XCorporateID,
                    XFacilityID = item.XFacilityID,
                    ActivityTypeName = GetNameByGlobalCodeValue(Convert.ToInt32(item.AType).ToString(), Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString())
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the x payment return model by claim identifier.
        /// </summary>
        /// <param name="claimId">The claim identifier.</param>
        /// <returns></returns>
        public List<XPaymentReturn> GetXPaymentReturnModelByClaimId(int claimId)
        {
            var lstXPaymentReturn = _repository.Where(x => x.ID == claimId && (x.AdviceStatus == 0 || x.AdviceStatus == null)).ToList();
            return lstXPaymentReturn;
        }

        /// <summary>
        /// Generates the remittance information.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public bool GenerateRemittanceXmlFile(int corporateId, int facilityId)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("CorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("FacilityID", facilityId);
            _repository.ExecuteCommand(StoredProcedures.GenerateRemittanceXMLFile.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Gets the claim payment.
        /// </summary>
        /// <param name="claimid">The claimid.</param>
        /// <returns></returns>
        public bool GetClaimPayment(int claimid)
        {
            var lstXPaymentReturn = _repository.Where(x => x.ID == claimid).ToList();
            return lstXPaymentReturn.Any();
        }
    }
}
