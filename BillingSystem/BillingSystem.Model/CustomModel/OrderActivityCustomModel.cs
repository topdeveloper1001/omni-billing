
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class OrderActivityCustomModel : OrderActivity
    {
        public string OrderCodeDescription { get; set; }
        public string Status { get; set; }
        public string OrderTypeName { get; set; }
        public string OrderDescription { get; set; }
        public string DiagnosisDescription { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public bool ShowEditAction { get; set; }
        public bool ShowSpecimanEditAction { get; set; }

        public string ResultUOMStr { get; set; }
        public string LabResultTypeStr { get; set; }
        public string LabResultStatusStr { get; set; }
        public string SpecimenTypeStr { get; set; }
        public string LabUOM { get; set; }
        public bool? PartiallyExecutedBool { get; set; }
        public string PartiallyExecutedstatus { get; set; }

        public DateTime? EncounterStartDate { get; set; }
        public DateTime? EncounterEndDate { get; set; }

    }
    [NotMapped]
    public class LabOrderActivityResultStatus
    {
        public string LabTestStatusFlag { get; set; }
        public string LabTestSpecimanStr { get; set; }
        public string LabUOM { get; set; }
    }
    [NotMapped]
    public class MarViewCustomModel
    {
        #region Returned Model from MARView
        public string OrderInfo { get; set; }
        public string SchTime { get; set; }
        public string OrderStatus { get; set; }
        public Int32 D1 { get; set; }
        public Int32 D2 { get; set; }
        public Int32 D3 { get; set; }
        public Int32 D4 { get; set; }
        public Int32 D5 { get; set; }
        public Int32 D6 { get; set; }
        public Int32 D7 { get; set; }
        public Int32 D8 { get; set; }
        public Int32 D9 { get; set; }
        public Int32 D10 { get; set; }
        public Int32 D11 { get; set; }
        public Int32 D12 { get; set; }
        public Int32 D13 { get; set; }
        public Int32 D14 { get; set; }
        public Int32 D15 { get; set; }
        public Int32 D16 { get; set; }
        public Int32 D17 { get; set; }
        public Int32 D18 { get; set; }
        public Int32 D19 { get; set; }
        public Int32 D20 { get; set; }
        public Int32 D21 { get; set; }
        public Int32 D22 { get; set; }
        public Int32 D23 { get; set; }
        public Int32 D24 { get; set; }
        public Int32 D25 { get; set; }
        public Int32 D26 { get; set; }
        public Int32 D27 { get; set; }
        public Int32 D28 { get; set; }
        public Int32 D29 { get; set; }
        public Int32 D30 { get; set; }
        public Int32 D31 { get; set; }
        

        #region Passing parameters
        public Int32 PatientId { get; set; }
        public Int32 EncounterId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime TillDate { get; set; }
        public Int32 DisplayFlag { get; set; }
        public string MonthValue { get; set; }

        #endregion
        
        #endregion
    }
    [NotMapped]
    public class MarGroupCustomModel
    {
        public string OrderInfo { get; set; }
        public string OrderStatus { get; set; }
        public IEnumerable<MarViewCustomModel> Collection { get; set; }
        public MarGroupCustomModel(string OrderInfo, string OrderStatus, List<MarViewCustomModel> collection)
        {
            this.OrderInfo = OrderInfo;
            this.OrderStatus = OrderStatus;
            this.Collection = collection.Where(x => x.OrderInfo == OrderInfo).ToList();
        }
    }
}
