//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class XFileHeader
    {
        [Key]
        public long FileID { get; set; }
        public string FileType { get; set; }
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> RecordCount { get; set; }
        public string DispositionFlag { get; set; }
        public string XPath { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> SentDate { get; set; }
        public string AcknowledgeNum { get; set; }
        public Nullable<System.DateTime> AcknowledgeDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> CorporateID { get; set; }
        public Nullable<int> FacilityID { get; set; }
    }
}
