using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _repository;
        private readonly IRepository<Encounter> _eRepository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IRepository<BillActivity> _blRepository;
        private readonly IRepository<GlobalCodes> _gRepository;

        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public PaymentService(IRepository<Payment> repository, IRepository<Encounter> eRepository, IRepository<PatientInfo> piRepository, IRepository<BillActivity> blRepository, IRepository<GlobalCodes> gRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _eRepository = eRepository;
            _piRepository = piRepository;
            _blRepository = blRepository;
            _gRepository = gRepository;
            _context = context;
            _mapper = mapper;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Applies the manual payment.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate identifier.
        /// </param>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ApplyManualPayment(int corporateId, int facilityId)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("CorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("FacilityID", facilityId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_ApplyPaymentManualToBill.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// The get expected payment ins not paid.
        /// </summary>
        /// <param name="corporateid">
        /// The corporateid.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ReviewExpectedPaymentReport> GetExpectedPaymentInsNotPaid(int corporateid, int facilityid)
        {
            var spName = string.Format("EXEC {0}  @pCorporateID , @pFacilityID ", StoredProcedures.SPROC_REP_ExpectedPaymentInsNotPaid);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
            var result = _context.Database.SqlQuery<ReviewExpectedPaymentReport>(spName, sqlParameters);
            return result;
        }

        /// <summary>
        /// The get expected payment ins variance.
        /// </summary>
        /// <param name="corporateid">
        /// The corporateid.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ReviewExpectedPaymentReport> GetExpectedPaymentInsVariance(int corporateid, int facilityid)
        {
            var spName = string.Format("EXEC {0}  @pCorporateID , @pFacilityID ", StoredProcedures.SPROC_REP_ExpectedPaymentInsVariance);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
            var result = _context.Database.SqlQuery<ReviewExpectedPaymentReport>(spName, sqlParameters);
            return result;
        }

        /// <summary>
        /// The get expected payment patient var.
        /// </summary>
        /// <param name="corporateid">
        /// The corporateid.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ReviewExpectedPaymentReport> GetExpectedPaymentPatientVar(int corporateid, int facilityid)
        {
            var spName = string.Format("EXEC {0}  @pCorporateID , @pFacilityID ", StoredProcedures.SPROC_REP_ExpectedPaymentPatientVar);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
            var result = _context.Database.SqlQuery<ReviewExpectedPaymentReport>(spName, sqlParameters);
            return result;
        }

        /// <summary>
        /// Gets the no payment recevied list.
        /// </summary>
        /// <param name="coporateid">
        /// The coporateid.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<AccountStatementCustomModel> GetNoPaymentReceviedList(int coporateid, int facilityid)
        {
            var spName = string.Format("EXEC {0}  @pCorporateID , @pFacilityID ", StoredProcedures.SPROC_REP_NoPaymentsReceived);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", coporateid);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
            IEnumerable<AccountStatementCustomModel> result = _context.Database.SqlQuery<AccountStatementCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the patient account statement.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<AccountStatementCustomModel> GetPatientAccountStatement(int patientId)
        {
            var spName = string.Format("EXEC {0}  @pPatientID", StoredProcedures.SPROC_REP_AccountStatementByPatient);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pPatientID", patientId);
            IEnumerable<AccountStatementCustomModel> result = _context.Database.SqlQuery<AccountStatementCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        /// <summary>
        /// Gets the payment bills.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <param name="billHeaderId">
        /// The bill header identifier.
        /// </param>
        /// <param name="corporateId">
        /// The corporate Id.
        /// </param>
        /// <param name="facilityId">
        /// The facility Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PaymentCustomModel> GetPaymentBills(
            int patientId,
            int encounterId,
            int billHeaderId,
            int corporateId,
            int facilityId)
        {
            var list = new List<PaymentCustomModel>();
            List<Payment> payments = corporateId > 0
                                         ? _repository.Where(
                                             a =>
                                             a.PayCorporateID != null && (int)a.PayCorporateID == corporateId
                                             && (int)a.PayFacilityID == facilityId).ToList()
                                         : _repository.GetAll().ToList();

            if (patientId > 0)
            {
                payments = _repository.Where(p => (int)p.PayFor == patientId).ToList();
            }

            if (encounterId > 0)
            {
                payments =
                    payments.Where(e => e.PayEncounterID != null && (int)e.PayEncounterID == encounterId).ToList();
            }

            if (billHeaderId > 0)
            {
                payments = payments.Where(b => b.PayBillID != null && (int)b.PayBillID == billHeaderId).ToList();
            }

            if (payments.Count > 0)
            {
                list.AddRange(
                    payments.Select(
                        item =>
                        new PaymentCustomModel
                        {
                            PayActivityID = item.PayActivityID,
                            PayAmount = item.PayAmount,
                            PayAppliedAmount = item.PayAppliedAmount,
                            PayBillID = item.PayBillID,
                            PayBillNumber = item.PayBillNumber,
                            PayBy = item.PayBy,
                            PayCorporateID = item.PayCorporateID,
                            PayCreatedBy = item.PayCreatedBy,
                            PayCreatedDate = item.PayCreatedDate,
                            PayDate = item.PayDate,
                            PayEncounterID = item.PayEncounterID,
                            PayFacilityID = item.PayFacilityID,
                            PayFor = item.PayFor,
                            PayIsActive = item.PayIsActive,
                            PayModifiedBy = item.PayModifiedBy,
                            PayModifiedDate = item.PayModifiedDate,
                            PayReference = item.PayReference,
                            PayStatus = item.PayStatus,
                            PayType = item.PayType,
                            PayXAAdviceID = item.PayXAAdviceID,
                            PayXADenialCode = item.PayXADenialCode,
                            PayXAFileHeaderID = item.PayXAFileHeaderID,
                            PaymentID = item.PaymentID,
                            PayForPatientName =
                                    GetPatientNameById(Convert.ToInt32(item.PayFor)),
                            PayByPatientName =
                                    GetPatientNameById(Convert.ToInt32(item.PayBy)),
                            EncounterNumber =
                                    GetEncounterNumberById(
                                        Convert.ToInt32(item.PayEncounterID)),
                        }));
            }

            return list;
        }

        private string GetPatientNameById(int PatientID)
        {
            var m = _piRepository.GetSingle(Convert.ToInt32(PatientID));
            return m != null ? m.PersonFirstName + " " + m.PersonLastName : string.Empty;
        }
        private string GetEncounterNumberById(int? encounterID)
        {
            var m = _eRepository.Where(e => e.EncounterID == Convert.ToInt32(encounterID)).FirstOrDefault();
            return m != null ? m.EncounterNumber : string.Empty;
        }
        /// <summary>
        /// Gets the payment by identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        /// <summary>
        /// Gets the payment by bill identifier.
        /// </summary>
        /// <param name="billHeaderId">
        /// The bill header identifier.
        /// </param>
        /// <returns>
        /// </returns>
        public List<PaymentCustomModel> GetPaymentByBillId(int billHeaderId)
        {
            var list = new List<PaymentCustomModel>();
            List<Payment> payments = _repository.Where(x => x.PayBillID == billHeaderId).ToList();
            if (payments.Any())
            {
                list.AddRange(
                    payments.Select(
                        item =>
                        new PaymentCustomModel
                        {
                            PayActivityID = item.PayActivityID,
                            PayAmount = item.PayAmount,
                            PayAppliedAmount = item.PayAppliedAmount,
                            PayBillID = item.PayBillID,
                            PayBillNumber = item.PayBillNumber,
                            PayBy = item.PayBy,
                            PayCorporateID = item.PayCorporateID,
                            PayCreatedBy = item.PayCreatedBy,
                            PayCreatedDate = item.PayCreatedDate,
                            PayDate = item.PayDate,
                            PayEncounterID = item.PayEncounterID,
                            PayFacilityID = item.PayFacilityID,
                            PayFor = item.PayFor,
                            PayIsActive = item.PayIsActive,
                            PayModifiedBy = item.PayModifiedBy,
                            PayModifiedDate = item.PayModifiedDate,
                            PayReference = item.PayReference,
                            PayStatus = item.PayStatus,
                            PayType = item.PayType,
                            PayXAAdviceID = item.PayXAAdviceID,
                            PayXADenialCode = item.PayXADenialCode,
                            PayXAFileHeaderID = item.PayXAFileHeaderID,
                            PaymentID = item.PaymentID,
                            PayForPatientName =
                                    GetPatientNameById(Convert.ToInt32(item.PayFor)),
                            PayByPatientName =
                                    GetPatientNameById(Convert.ToInt32(item.PayBy)),
                            EncounterNumber =
                                    GetEncounterNumberById(
                                        Convert.ToInt32(item.PayEncounterID)),
                            ActivityTypeName =
                                    GetActivityNameByActivityId(
                                        Convert.ToInt32(item.PayActivityID)),
                            ActivityTypeCode =
                                    GetActivityCodeByActivityId(
                                        Convert.ToInt32(item.PayActivityID)),
                            PayNETAmount = item.PayNETAmount,
                            PayAppliedStatus = item.PayAppliedStatus,
                            PayUnAppliedAmount = item.PayUnAppliedAmount,
                            PayStatusStr = GetPaymentStatusString(item.PayStatus)
                        }));
            }

            return list;
        }
        private string GetPaymentStatusString(int? paystatus)
        {
            var stringToreturn = string.Empty;
            switch (paystatus)
            {
                case 20:
                    stringToreturn = "Denial at Activity Level";
                    break;
                case 50:
                    stringToreturn = "Partial Paid";
                    break;
                case 100:
                    stringToreturn = "Fully Paid";
                    break;
                case 900:
                    stringToreturn = "Claim Level Denied";
                    break;
            }

            return stringToreturn;
        }

        private string GetActivityCodeByActivityId(int activityId)
        {
            var encounter = activityId != 0 ? _blRepository.Where(e => e.BillActivityID == activityId).FirstOrDefault() : null;
            return encounter != null ? encounter.ActivityCode : string.Empty;
        }

        private string GetActivityNameByActivityId(int activityId)
        {
            var encounter = activityId != 0 ? _blRepository.GetAll().FirstOrDefault(e => e.BillActivityID == activityId) : null;
            //return encounter != null ? encounter.ActivityType : string.Empty;
            return encounter != null ? GetNameByGlobalCodeValue(encounter.ActivityType, "1201") : string.Empty;
        }
        public string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }
        /// <summary>
        /// The get payment by id.
        /// </summary>
        /// <param name="paymentId">
        /// The payment id.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentCustomModel"/>.
        /// </returns>
        public PaymentCustomModel GetPaymentById(long paymentId)
        {
            var payment = new PaymentCustomModel();
            Payment item = _repository.Where(x => x.PaymentID == paymentId).FirstOrDefault();
            if (item != null)
            {
                payment = new PaymentCustomModel
                {
                    PayedDate =
                                      string.Format(
                                          "{0:d/M/yyyy HH:mm:ss}",
                                          Convert.ToDateTime(item.PayDate)),

                    // item.PayDate.ToString(),
                    PayActivityID = item.PayActivityID,
                    PayAmount = item.PayAmount,
                    PayAppliedAmount = item.PayAppliedAmount,
                    PayBillID = item.PayBillID,
                    PayBillNumber = item.PayBillNumber,
                    PayBy = item.PayBy,
                    PayCorporateID = item.PayCorporateID,
                    PayCreatedBy = item.PayCreatedBy,
                    PayCreatedDate = item.PayCreatedDate,
                    PayDate = item.PayDate,
                    PayEncounterID = item.PayEncounterID,
                    PayFacilityID = item.PayFacilityID,
                    PayFor = item.PayFor,
                    PayIsActive = item.PayIsActive,
                    PayModifiedBy = item.PayModifiedBy,
                    PayModifiedDate = item.PayModifiedDate,
                    PayReference = item.PayReference,
                    PayStatus = item.PayStatus,
                    PayType = item.PayType,
                    PayXAAdviceID = item.PayXAAdviceID,
                    PayXADenialCode = item.PayXADenialCode,
                    PayXAFileHeaderID = item.PayXAFileHeaderID,
                    PaymentID = item.PaymentID,
                    PayForPatientName =
                                      GetPatientNameById(Convert.ToInt32(item.PayFor)),
                    PayByPatientName =
                                      GetPatientNameById(Convert.ToInt32(item.PayBy)),
                    EncounterNumber =
                                      GetEncounterNumberById(
                                          Convert.ToInt32(item.PayEncounterID)),
                };
            }

            return payment;
        }

        /// <summary>
        /// Gets the un macted payment list.
        /// </summary>
        /// <param name="coporateid">
        /// The coporateid.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<AccountStatementCustomModel> GetUnMactedPaymentList(int coporateid, int facilityid)
        {
            var spName = string.Format("EXEC {0}  @pCorporateID , @pFacilityID ", StoredProcedures.SPROC_REP_PaymentsUnMatched);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", coporateid);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
            IEnumerable<AccountStatementCustomModel> result = _context.Database.SqlQuery<AccountStatementCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// The save custom payments.
        /// </summary>
        /// <param name="vm">
        /// The vm.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public long SaveCustomPayments(PaymentCustomModel vm)
        {
            Payment model = _mapper.Map<Payment>(vm);
            if (model.PaymentID > 0)
                _repository.UpdateEntity(model, Convert.ToInt32(model.PaymentID));
            else
                _repository.Create(model);
            return model.PaymentID;
        }

        /// <summary>
        /// Saves the payments.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public long SavePayments(Payment model)
        {
            if (model.PaymentID > 0)
                _repository.UpdateEntity(model, Convert.ToInt32(model.PaymentID));
            else
                _repository.Create(model);
            return model.PaymentID;
        }

        #endregion


        public PaymentViewDetail SaveAndApplyPayments(long cId, long fId, long userId, PaymentCustomModel vm, DateTime currentDateTime)
        {
            var vData = new PaymentViewDetail();
            var sqlParams = new SqlParameter[20];

            sqlParams[0] = new SqlParameter("@pCId", cId);
            sqlParams[1] = new SqlParameter("@pFId", fId);
            sqlParams[2] = new SqlParameter("@pDateTime", currentDateTime);
            sqlParams[3] = new SqlParameter("@pPaymentId", vm.PaymentID);
            sqlParams[4] = new SqlParameter("@pPReference", string.IsNullOrEmpty(vm.PayReference) ? string.Empty : vm.PayReference);
            sqlParams[5] = new SqlParameter("@pPayDate", vm.PayDate.GetValueOrDefault());
            sqlParams[6] = new SqlParameter("@pPTypeId", vm.PaymentTypeId.GetValueOrDefault());
            sqlParams[7] = new SqlParameter("@pPAmount", vm.PayAmount.GetValueOrDefault());
            sqlParams[8] = new SqlParameter("@pCardNo", string.IsNullOrEmpty(vm.PTDCardNumber) ? string.Empty : vm.PTDCardNumber);
            sqlParams[9] = new SqlParameter("@pExpiryMonth", string.IsNullOrEmpty(vm.PTDExpiryMonth) ? string.Empty : vm.PTDExpiryMonth);
            sqlParams[10] = new SqlParameter("@pExpiryYear", vm.PTDExpiryYear);
            sqlParams[11] = new SqlParameter("@pCardHolderName", string.IsNullOrEmpty(vm.PTDCardHolderName) ? string.Empty : vm.PTDCardHolderName);
            sqlParams[12] = new SqlParameter("@pSecurityNo", string.IsNullOrEmpty(vm.PTDSecurityNumber) ? string.Empty : vm.PTDSecurityNumber);
            sqlParams[13] = new SqlParameter("@pBillNumber", string.IsNullOrEmpty(vm.PayBillNumber) ? string.Empty : vm.PayBillNumber);
            sqlParams[14] = new SqlParameter("@pBillHeaderId", vm.PayBillID.GetValueOrDefault());
            sqlParams[15] = new SqlParameter("@pUserId", userId);
            sqlParams[16] = new SqlParameter("@pPType", vm.PayType.GetValueOrDefault());
            sqlParams[17] = new SqlParameter("@pPayFor_Id", vm.PayFor.GetValueOrDefault());
            sqlParams[18] = new SqlParameter("@pPayBy_Id", vm.PayFor.GetValueOrDefault());
            sqlParams[19] = new SqlParameter("@pEId", vm.PayEncounterID.GetValueOrDefault());

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveAndApplyPayments.ToString(), false, parameters: sqlParams))
            {
                vData = new PaymentViewDetail();

                //Get the list of the PaymentCustomModel
                vData.PaymentsList = r.ResultSetFor<PaymentCustomModel>().ToList();

                //Get the PaymentDetailCustomModel
                vData.PaymentDetails = r.ResultSetFor<PaymentDetailsCustomModel>().FirstOrDefault();

                //Get the list of Patients
                vData.PatientsList = r.ResultSetFor<DropdownListData>().ToList();

                vData.Success = true;
            }
            return vData;
        }


        public PaymentViewDetail GetPaymentsListAndOthersData(long cId, long fId, long userId, long billHeaderId, long patientId = 0, long encounterId = 0, string billNumber = "", long paymentId = 0)
        {
            var vData = new PaymentViewDetail();
            var sqlParams = new SqlParameter[8];

            sqlParams[0] = new SqlParameter("@pFId", fId);
            sqlParams[1] = new SqlParameter("@pBillHeaderId", billHeaderId);
            sqlParams[2] = new SqlParameter("@pUserId", userId);
            sqlParams[3] = new SqlParameter("@pPatientId", patientId);
            sqlParams[4] = new SqlParameter("@pEId", encounterId);
            sqlParams[5] = new SqlParameter("@pBillNumber", billNumber);
            sqlParams[6] = new SqlParameter("@pCId", cId);
            sqlParams[7] = new SqlParameter("@pPaymentId", paymentId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetPaymentsList.ToString(), false, parameters: sqlParams))
            {
                vData = new PaymentViewDetail();

                //Get the list of the PaymentCustomModel
                vData.PaymentsList = r.ResultSetFor<PaymentCustomModel>().ToList();

                //Get the PaymentDetailCustomModel
                vData.PaymentDetails = r.ResultSetFor<PaymentDetailsCustomModel>().FirstOrDefault();

                //Get the list of Patients
                vData.PatientsList = r.ResultSetFor<DropdownListData>().ToList();
            }
            return vData;
        }
    }
}