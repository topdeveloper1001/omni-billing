﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
   public  class BillEditorUsersCustomModel
    {
       public int UserId { get; set; }
       public string Name { get; set; }
       public int RoleId { get; set; }
       public string RoleName { get; set; }
    }
}
