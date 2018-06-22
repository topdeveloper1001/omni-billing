// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentBal.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The payment bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    /// The payment bal.
    /// </summary>
    public class PaymentBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentBal"/> class.
        /// </summary>
        public PaymentBal()
        {
            PaymentMapper = new PaymentMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the payment mapper.
        /// </summary>
        private PaymentMapper PaymentMapper { get; set; }

        #endregion

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
            bool result = false;
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                result = rep.ApplyManualPayment(corporateId, facilityId);
            }

            return result;
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                IEnumerable<ReviewExpectedPaymentReport> list = rep.GetExpectedPaymentInsNotPaid(
                    corporateid,
                    facilityid);
                return list;
            }
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                IEnumerable<ReviewExpectedPaymentReport> list = rep.GetExpectedPaymentInsVariance(
                    corporateid,
                    facilityid);
                return list;
            }
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                IEnumerable<ReviewExpectedPaymentReport> list = rep.GetExpectedPaymentPatientVar(
                    corporateid,
                    facilityid);
                return list;
            }
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                IEnumerable<AccountStatementCustomModel> result = rep.GetNoPaymentReceviedList(coporateid, facilityid);
                return result;
            }
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                IEnumerable<AccountStatementCustomModel> result = rep.GetPatientAccountStatement(patientId);
                return result;
            }
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                List<Payment> payments = corporateId > 0
                                             ? rep.Where(
                                                 a =>
                                                 a.PayCorporateID != null && (int)a.PayCorporateID == corporateId
                                                 && (int)a.PayFacilityID == facilityId).ToList()
                                             : rep.GetAll().ToList();

                if (patientId > 0)
                {
                    payments = rep.Where(p => (int)p.PayFor == patientId).ToList();
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
            }

            return list;
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                List<Payment> payments = rep.Where(x => x.PayBillID == billHeaderId).ToList();
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
            }

            return list;
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                Payment item = rep.Where(x => x.PaymentID == paymentId).FirstOrDefault();
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                IEnumerable<AccountStatementCustomModel> result = rep.GetUnMatchedPaymentList(coporateid, facilityid);
                return result;
            }
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                Payment model = PaymentMapper.MapViewModelToModel(vm);
                if (model.PaymentID > 0)
                {
                    rep.UpdateEntity(model, Convert.ToInt32(model.PaymentID));
                }
                else
                {
                    rep.Create(model);
                }

                return model.PaymentID;
            }
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
            using (PaymentRepository rep = UnitOfWork.PaymentRepository)
            {
                if (model.PaymentID > 0)
                {
                    rep.UpdateEntity(model, Convert.ToInt32(model.PaymentID));
                }
                else
                {
                    rep.Create(model);
                }

                return model.PaymentID;
            }
        }

        #endregion


        public PaymentViewDetail SaveAndApplyPayments(long cId, long fId, long userId, PaymentCustomModel vm, DateTime currentDateTime)
        {
            using (var rep = UnitOfWork.PaymentRepository)
            {
                return rep.SaveAndApplyPayments(cId, fId, userId, vm, currentDateTime);
            }
        }


        public PaymentViewDetail GetPaymentsListAndOthersData(long cId, long fId, long userId, long billHeaderId, long patientId = 0, long encounterId = 0, string billNumber = "", long paymentId = 0)
        {
            using (var rep = UnitOfWork.PaymentRepository)
            {
                return rep.GetPaymentsListAndOthersData(cId, fId, userId, billHeaderId, patientId, encounterId, billNumber, paymentId);
            }
        }
    }
}