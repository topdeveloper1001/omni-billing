﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class PatientInfoChangesQueueView
    {
     
        public PatientInfoChangesQueue CurrentPatientInfoChangesQueue { get; set; }
        public List<PatientInfoChangesQueueCustomModel> PatientInfoChangesQueueList { get; set; }

    }
}
