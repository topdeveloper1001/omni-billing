﻿using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class LoginTrackingCustomModel : LoginTracking
    {
        public string UserName { get; set; }
        public string AssignedRoles { get; set; }
        public string AssignedFacilities { get; set; }
    }
}
