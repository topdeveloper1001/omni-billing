using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class AuthorizationView
    {
     
        public Authorization CurrentAuthorization { get; set; }
        public List<AuthorizationCustomModel> AuthorizationList { get; set; }
    }
}
