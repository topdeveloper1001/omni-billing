using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class ScrubEditTrackView
    {
     
        public ScrubEditTrack CurrentScrubEditTrack { get; set; }
        public List<ScrubEditTrackCustomModel> ScrubEditTrackList { get; set; }

    }
}
