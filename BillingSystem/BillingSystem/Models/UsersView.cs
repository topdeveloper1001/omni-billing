using BillingSystem.Model.CustomModel;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class UsersView
    {
        public List<UsersCustomModel> UsersList { get; set; }
        public UsersCustomModel CurrentUser { get; set; }
    }
}