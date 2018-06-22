using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class GenerateBarCode
    {
        public string PatientId { get; set; }
        public Int32 EncounterId { get; set; }
        public string PatientEncounterId { get; set; }
        public string PatientName { get; set; }
        public string EncounterName { get; set; }
        public string DateOfBirth { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string BarCodeNumbering { get; set; }
        public string CollectionDateTime { get; set; }
        public string Site { get; set; }
        public Int32 UserId { get; set; }
        public string TestOrderedShortName { get; set; }
        public string CptNumber { get; set; }
        public string BarCode { get; set; }
        public string BarCodeReadValue { get; set; }
        public string BarCodeHtml { get; set; }
        public Int32 OrderActivityId { get; set; }
        public string LoggedInUserName { get; set; }
        public string OrderType { get; set; }
    }
}
