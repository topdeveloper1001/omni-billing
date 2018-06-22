// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BillActivityBal.cs" company="Spadez">
//   Omni Health care
// </copyright>
// <summary>
//   The bill activity bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BillingSystem.Common.Common;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The bill activity bal.
    /// </summary>
    public class BillActivityBal : BaseBal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BillActivityBal"/> class.
        /// </summary>
        public BillActivityBal()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillActivityBal"/> class.
        /// </summary>
        /// <param name="cptTableNumber">
        /// The cpt table number.
        /// </param>
        /// <param name="serviceCodeTableNumber">
        /// The service code table number.
        /// </param>
        /// <param name="drgTableNumber">
        /// The drg table number.
        /// </param>
        /// <param name="drugTableNumber">
        /// The drug table number.
        /// </param>
        /// <param name="hcpcsTableNumber">
        /// The hcpcs table number.
        /// </param>
        /// <param name="diagnosisTableNumber">
        /// The diagnosis table number.
        /// </param>
        public BillActivityBal(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber))
            {
                CptTableNumber = cptTableNumber;
            }

            if (!string.IsNullOrEmpty(serviceCodeTableNumber))
            {
                ServiceCodeTableNumber = serviceCodeTableNumber;
            }

            if (!string.IsNullOrEmpty(drgTableNumber))
            {
                DrgTableNumber = drgTableNumber;
            }

            if (!string.IsNullOrEmpty(drugTableNumber))
            {
                DrugTableNumber = drugTableNumber;
            }

            if (!string.IsNullOrEmpty(hcpcsTableNumber))
            {
                HcpcsTableNumber = hcpcsTableNumber;
            }

            if (!string.IsNullOrEmpty(diagnosisTableNumber))
            {
                DiagnosisTableNumber = diagnosisTableNumber;
            }
        }

        /// <summary>
        /// Gets the bill detail view.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> GetBillActivitiesByBillHeaderId(int billHeaderId)
        {
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var list = rep.GetBillDetailView(billHeaderId);
                //list = list.Select(item =>
                //    {
                //        item.ActivityTypeName = GetNameByGlobalCodeValue(
                //            item.ActivityType,
                //            Convert.ToString((int)GlobalCodeCategoryValue.CodeTypes));
                //        var billHeader = rep.Where(a => a.BillHeaderID == billHeaderId).FirstOrDefault();
                //        if (billHeader != null)
                //        {
                //            item.BillNumber = billHeader.BillNumber;
                //            item.ActivityCodeDescription = GetCodeDescription(item.ActivityCode, item.ActivityType);
                //        }
                //        return item;
                //    }).OrderBy(b => b.ExecutedOn).ToList();
                var totalrow = new BillDetailCustomModel()
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
                    GrossCharges = list.Sum(x => x.GrossCharges),
                    PayerShareNet = list.Sum(x => x.PayerShareNet),
                    PatientShare = list.Sum(x => x.PatientShare),
                    ActivityCost = list.Sum(x => x.ActivityCost),
                    GrossChargesSum = list.Sum(x => x.PayerShareNet) + list.Sum(x => x.PatientShare)
                };
                list.Add(totalrow);
                return list;
            }
        }

        /// <summary>
        /// Gets the bill activities by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> GetBillActivitiesByEncounterId(int encounterId)
        {
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var list = rep.GetEncounterBillDetailView(encounterId);
                list = list.Select(item =>
                {
                    item.ActivityTypeName = GetNameByGlobalCodeValue(item.ActivityType,
                        Convert.ToString((int)GlobalCodeCategoryValue.CodeTypes));
                    var billHeader = rep.Where(a => a.BillHeaderID == item.BillHeaderID).FirstOrDefault();
                    if (billHeader != null)
                    {
                        item.BillNumber = billHeader.BillNumber;
                        item.ActivityCodeDescription = GetCodeDescription(item.ActivityCode, item.ActivityType);
                    }
                    return item;
                }).OrderBy(b => b.ExecutedOn).ToList();
                var totalrow = new BillDetailCustomModel()
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
                    GrossCharges = list.Sum(x => x.GrossCharges),
                    PayerShareNet = list.Sum(x => x.PayerShareNet),
                    PatientShare = list.Sum(x => x.PatientShare),
                    GrossChargesSum = list.Sum(x => x.PayerShareNet) + list.Sum(x => x.PatientShare)
                };
                list.Add(totalrow);
                return list;
            }
        }

        /// <summary>
        /// Gets the bill header identifier by bill activity identifier.
        /// </summary>
        /// <param name="billactivityId">The billactivity identifier.</param>
        /// <returns></returns>
        public int GetPatientIdByBillActivityId(int billactivityId)
        {
            using (var rep = UnitOfWork.BillActivityRepository)
            {
                var billActivityObj = rep.Where(x => x.BillActivityID == billactivityId).FirstOrDefault();
                return billActivityObj != null ? Convert.ToInt32(billActivityObj.ActivityID) : 0;
            }
        }

        /// <summary>
        /// Gets the encounter identifier by bill activity identifier.
        /// </summary>
        /// <param name="billactivityId">The billactivity identifier.</param>
        /// <returns></returns>
        public int GetEncounterIdByBillActivityId(int billactivityId)
        {
            using (var rep = UnitOfWork.BillActivityRepository)
            {
                var billActivityObj = rep.Where(x => x.BillActivityID == billactivityId).FirstOrDefault();
                return billActivityObj != null ? Convert.ToInt32(billActivityObj.EncounterID) : 0;
            }
        }

        /// <summary>
        /// Deletes the bill activity from bill.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool DeleteBillActivityFromBill(int id)
        {
            using (var rep = UnitOfWork.BillActivityRepository)
            {
                rep.Delete(id);
                return true;
            }
        }

        //public object GetPatientBabyMotherLink(int patientId)
        //{
        //    using (var rep = UnitOfWork.PatientInfoRepository)
        //    {
        //        var PinfoObj
        //        return true;
        //    }
        //}

        public bool DeleteDiagnosisTypeBillActivity(int encounterId, int patientId, string actCode, int userId)
        {
            using (var rep = UnitOfWork.BillActivityRepository)
            {
                var current =
                    rep.Where(
                        b => b.ActivityCode.Equals(actCode) && b.EncounterID == encounterId && b.PatientID == patientId && b.IsDeleted == false)
                        .FirstOrDefault();

                if (current != null)
                {
                    using (var headerRep = UnitOfWork.BillHeaderRepository)
                    {
                        headerRep.DeleteBillActivity(current.BillActivityID, userId);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the bill PDF format.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public List<BillPdfFormatCustomModel> GetBillPdfFormat(int billHeaderId)
        {
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var list = rep.GetBillFormatData(billHeaderId);
                return list;
            }
        }
    }
}

