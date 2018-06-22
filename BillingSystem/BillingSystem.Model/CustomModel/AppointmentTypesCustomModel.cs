using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingSystem.Model;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class AppointmentTypesCustomModel : AppointmentTypes
    {
        public string CPTRange { get; set; }

        /// <summary>
        /// Property is used to set time slots of different appointment types
        /// </summary>
        public string TimeSlot { get; set; }
        public string EquipmentRequired { get; set; }
        public int? CategoryNumber1 { get; set; }
    }
}
