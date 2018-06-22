using BillingSystem.Model;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class UserRoleView
    {
        public int UserID { get; set; }
        public List<Role> RolesList { get; set; }
        public List<Users> UsersList { get; set; }
    }
}