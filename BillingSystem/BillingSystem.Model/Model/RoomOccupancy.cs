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
    
    public partial class RoomOccupancy
    {
        [Key]
        public int Id { get; set; }
        public Nullable<int> FacilityNumber { get; set; }
        public string FacilityRoomNumber { get; set; }
        public Nullable<int> FacilityRoomType { get; set; }
        public string FacilityBedNumber { get; set; }
        public bool Occupied { get; set; }
        public Nullable<int> RoomId { get; set; }
    }
}
