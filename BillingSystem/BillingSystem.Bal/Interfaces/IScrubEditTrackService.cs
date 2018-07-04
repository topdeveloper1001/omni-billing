using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IScrubEditTrackService
    {
        List<ScrubEditTrackCustomModel> GetScrubEditTrack(int corporateId, int facilityId);
        ScrubEditTrack GetScrubEditTrackByID(int? ScrubEditTrackId);
        int SaveScrubEditTrack(ScrubEditTrack model);
    }
}