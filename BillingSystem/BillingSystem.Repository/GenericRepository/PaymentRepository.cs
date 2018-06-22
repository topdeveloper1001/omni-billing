using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;

namespace BillingSystem.Repository.GenericRepository
{
    public class PaymentRepository : GenericRepository<Payment>
    {
        private readonly DbContext _context;
        public PaymentRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Applies the manual payment.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public bool ApplyManualPayment(int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0}  @CorporateID ,@FacilityID", StoredProcedures.SPROC_ApplyPaymentManualToBill);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("CorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("FacilityID", facilityId);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }


        /// <summary>
        /// Gets the patient account statement.
        /// </summary>
        /// <param name="patientid">The patientid.</param>
        /// <returns></returns>
        public IEnumerable<AccountStatementCustomModel> GetPatientAccountStatement(int patientid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0}  @pPatientID", StoredProcedures.SPROC_REP_AccountStatementByPatient);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pPatientID", patientid);
                    IEnumerable<AccountStatementCustomModel> result = _context.Database.SqlQuery<AccountStatementCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }


        /// <summary>
        /// Gets the no payment recevied. 
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public IEnumerable<AccountStatementCustomModel> GetNoPaymentReceviedList(int corporateid, int facilityid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0}  @pCorporateID , @pFacilityID ", StoredProcedures.SPROC_REP_NoPaymentsReceived);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
                    IEnumerable<AccountStatementCustomModel> result = _context.Database.SqlQuery<AccountStatementCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// Gets the un matched payment list.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public IEnumerable<AccountStatementCustomModel> GetUnMatchedPaymentList(int corporateid, int facilityid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0}  @pCorporateID , @pFacilityID ", StoredProcedures.SPROC_REP_PaymentsUnMatched);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
                    IEnumerable<AccountStatementCustomModel> result = _context.Database.SqlQuery<AccountStatementCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }


        public IEnumerable<ReviewExpectedPaymentReport> GetExpectedPaymentInsNotPaid(int corporateid, int facilityid)
        {
            var list = new List<ReviewExpectedPaymentReport>();
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0}  @pCorporateID , @pFacilityID ", StoredProcedures.SPROC_REP_ExpectedPaymentInsNotPaid);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
                    var result = _context.Database.SqlQuery<ReviewExpectedPaymentReport>(spName, sqlParameters);
                    list = result.ToList();
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        public IEnumerable<ReviewExpectedPaymentReport> GetExpectedPaymentPatientVar(int corporateid, int facilityid)
        {
            var list = new List<ReviewExpectedPaymentReport>();
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0}  @pCorporateID , @pFacilityID ", StoredProcedures.SPROC_REP_ExpectedPaymentPatientVar);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
                    var result = _context.Database.SqlQuery<ReviewExpectedPaymentReport>(spName, sqlParameters);
                    list = result.ToList();
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        public IEnumerable<ReviewExpectedPaymentReport> GetExpectedPaymentInsVariance(int corporateid, int facilityid)
        {
            var list = new List<ReviewExpectedPaymentReport>();
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0}  @pCorporateID , @pFacilityID ", StoredProcedures.SPROC_REP_ExpectedPaymentInsVariance);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
                    var result = _context.Database.SqlQuery<ReviewExpectedPaymentReport>(spName, sqlParameters);
                    list = result.ToList();
                }
            }
            catch (Exception)
            {
            }
            return list;
        }


        public PaymentViewDetail SaveAndApplyPayments(long cId, long fId, long userId, PaymentCustomModel vm, DateTime currentDateTime)
        {
            PaymentViewDetail vData = null;
            try
            {
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return vData;
            //return new List<ClinicianRosterCustomModel>();
        }

        public PaymentViewDetail GetPaymentsListAndOthersData(long cId, long fId, long userId, long billHeaderId, long patientId = 0, long encounterId = 0, string billNumber = "", long paymentId = 0)
        {
            PaymentViewDetail vData = null;
            try
            {
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return vData;
        }
    }
}
