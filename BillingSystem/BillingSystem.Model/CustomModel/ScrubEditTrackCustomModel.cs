using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ScrubEditTrackCustomModel:ScrubEditTrack
    {
        public string TrackSideName { get; set; }
        public string CreatedByName { get; set; }
        public string CorrectionCodeText { get; set; }
        public string BillNumber { get; set; }
    }
}
