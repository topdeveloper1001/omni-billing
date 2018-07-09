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

    public class Role
    {
        public Role()
        {
            this.FacilityRole = new HashSet<FacilityRole>();
            this.RolePermission = new HashSet<RolePermission>();
            this.RoleTabs = new HashSet<RoleTabs>();
            this.UserRole = new HashSet<UserRole>();
        }
        [Key]
        public int RoleID { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public Nullable<int> CorporateId { get; set; }
        public Nullable<int> FacilityId { get; set; }
        public bool IsGeneric { get; set; }
        public string RoleKey { get; set; }
        public int PortalId { get; set; }

        public virtual ICollection<FacilityRole> FacilityRole { get; set; }
        public virtual ICollection<RolePermission> RolePermission { get; set; }
        public virtual ICollection<RoleTabs> RoleTabs { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
