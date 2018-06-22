using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class AuthorizationCustomModel : Authorization
    {
        public bool isActive { get; set; }
        public string AuthrizationTypeStr { get; set; }
        public string IdPayer { get; set; }
        public bool XhrResponse { get; set; }
        public long MissedEncounterId { get; set; }
    }

    [NotMapped]
    public class AuthorizationViewData
    {
        public IEnumerable<AuthorizationCustomModel> AuthList { get; set; }
        public IEnumerable<DocumentsTemplates> Docs { get; set; }
        public int AuthorizationId { get; set; }
    }
}
