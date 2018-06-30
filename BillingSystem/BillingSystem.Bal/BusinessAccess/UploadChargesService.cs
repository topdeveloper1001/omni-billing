using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class UploadChargesService : IUploadChargesService
    {
        private readonly IRepository<BillHeader> _bhRepository;
        private readonly BillingEntities _context;

        public UploadChargesService(IRepository<BillHeader> bhRepository, BillingEntities context)
        {
            _bhRepository = bhRepository;
            _context = context;
        }


        /// <summary>
        ///     Gets the x payment return denial claims.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public List<PatientInfoXReturnPaymentCustomModel> GetXPaymentReturnDenialClaims(CommonModel common)
        {
            string spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID",
                          StoredProcedures.SPROC_GetXPaymentReturnDenialClaims);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", common.CorporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", common.FacilityId);
            IEnumerable<PatientInfoXReturnPaymentCustomModel> result = _context.Database.SqlQuery<PatientInfoXReturnPaymentCustomModel>(spName, sqlParameters);

            var patientInfoLst = result.ToList();
            var returnCustomLst = new List<PatientInfoXReturnPaymentCustomModel>();
            if (patientInfoLst.Count > 0)
            {
                if (!string.IsNullOrEmpty(common.PersonLastName))
                    returnCustomLst.AddRange(
                        patientInfoLst.Where(
                            p => p.PersonLastName.ToLower().Contains(common.PersonLastName.ToLower())).ToList());
                if (!string.IsNullOrEmpty(common.PersonPassportNumber))
                    returnCustomLst.AddRange(
                        patientInfoLst.Where(
                            p =>
                                p.PersonPassportNumber != null &&
                                p.PersonPassportNumber.ToLower().Contains(common.PersonPassportNumber.ToLower()))
                            .ToList());
                if (!string.IsNullOrEmpty(common.PersonEmiratesIDNumber))
                    returnCustomLst.AddRange(
                        patientInfoLst.Where(
                            p =>
                                p.PersonEmiratesIDNumber != null && p.PersonEmiratesIDNumber.ToLower()
                                    .Contains(common.PersonEmiratesIDNumber.ToLower())).ToList());

                if (common.PersonBirthDate.HasValue)
                    returnCustomLst.AddRange(
                        patientInfoLst.Where(
                            p => p.PersonBirthdate.Date.Equals(common.PersonBirthDate.Value.Date)).ToList());

                if (!string.IsNullOrEmpty(common.ContactMobilePhone))
                    returnCustomLst.AddRange(
                        patientInfoLst.Where(
                            p =>
                                !string.IsNullOrEmpty(p.PhoneNo) &&
                                p.PhoneNo.ToLower().Contains(common.ContactMobilePhone.ToLower())).ToList());
            }
            return returnCustomLst.ToList();

        }

        /// <summary>
        ///     Gets the bill details by bill header identifier.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> GetBillDetailsByBillHeaderId(int billHeaderId)
        {

            var billheaderactivitiesList = GetBillDetailsByBillHeaderIdRep(billHeaderId);
            var totalrow = new BillDetailCustomModel
            {
                ActivityType = string.Empty,
                ActivityTypeName = string.Empty,
                ActivityCode = string.Empty,
                BillNumber = string.Empty,
                BillHeaderID = 0,
                BillActivityID = 0,
                CorporateID = 0,
                EncounterID = 0,
                FacilityID = 0,
                OrderedOn = null,
                ExecutedOn = null,
                QuantityOrdered = null,
                ActivityCodeDescription = "Total",
                GrossCharges = billheaderactivitiesList.Sum(x => x.GrossCharges),
                PayerShareNet = billheaderactivitiesList.Sum(x => x.PayerShareNet),
                PatientShare = billheaderactivitiesList.Sum(x => x.PatientShare),
                GrossChargesSum =
                    billheaderactivitiesList.Sum(x => x.PayerShareNet) +
                    billheaderactivitiesList.Sum(x => x.PatientShare)
            };
            billheaderactivitiesList.Add(totalrow);
            return billheaderactivitiesList;

        }
        private bool DeleteBillActivityRep(int billActivityId, int userId)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pBillActivityID ", billActivityId);
            sqlParameters[1] = new SqlParameter("pCreatedBy ", userId);
            _bhRepository.ExecuteCommand(StoredProcedures.SPROC_DeleteBillActivites.ToString(), sqlParameters);
            return true;
        }
        private List<BillDetailCustomModel> GetBillDetailsByBillHeaderIdRep(int billheaderId)
        {
            var spName = string.Format("EXEC {0} @pBillHeaderID ", StoredProcedures.SPROC_BillHeaderDetailsView);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pBillHeaderID ", billheaderId);
            IEnumerable<BillDetailCustomModel> result = _context.Database.SqlQuery<BillDetailCustomModel>(spName, sqlParameters);
            return result.ToList();

        }

        /// <summary>
        ///     Deletes the bill activity.
        /// </summary>
        /// <param name="billActivityId">The bill activity identifier.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> DeleteBillActivity(int billActivityId, int userid, int billHeaderId)
        {

            var billheaderactivitiesList = new List<BillDetailCustomModel>();
            var isupdated = DeleteBillActivityRep(billActivityId, userid);
            if (isupdated)
            {
                billheaderactivitiesList = GetBillDetailsByBillHeaderIdRep(billHeaderId);

                var totalrow = new BillDetailCustomModel
                {
                    ActivityType = string.Empty,
                    ActivityTypeName = string.Empty,
                    ActivityCode = string.Empty,
                    BillNumber = string.Empty,
                    BillHeaderID = 0,
                    BillActivityID = 0,
                    CorporateID = 0,
                    EncounterID = 0,
                    FacilityID = 0,
                    OrderedOn = null,
                    ExecutedOn = null,
                    QuantityOrdered = null,
                    ActivityCodeDescription = "Total",
                    GrossCharges = billheaderactivitiesList.Sum(x => x.GrossCharges),
                    PayerShareNet = billheaderactivitiesList.Sum(x => x.PayerShareNet),
                    PatientShare = billheaderactivitiesList.Sum(x => x.PatientShare),
                    GrossChargesSum =
                        billheaderactivitiesList.Sum(x => x.PayerShareNet) +
                        billheaderactivitiesList.Sum(x => x.PatientShare)
                };
                billheaderactivitiesList.Add(totalrow);
            }
            return billheaderactivitiesList;

        }


        public List<PatientInfoXReturnPaymentCustomModel> GetXPaymentReturnDenialClaimsByPatientId(int patientId, int encounterId, int billHeaderId)
        {
            string spName = string.Format("EXEC {0} @pPatientId, @pEncounterId, @pBillHeaderId",
                          StoredProcedures.SPROC_GetXPaymentReturnDenialClaimsByPatientId);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pPatientId", patientId);
            sqlParameters[1] = new SqlParameter("pEncounterId", encounterId);
            sqlParameters[2] = new SqlParameter("pBillHeaderId", billHeaderId);
            IEnumerable<PatientInfoXReturnPaymentCustomModel> result =
                _context.Database.SqlQuery<PatientInfoXReturnPaymentCustomModel>(spName, sqlParameters);
            return result.ToList();
        }
         
    }
}