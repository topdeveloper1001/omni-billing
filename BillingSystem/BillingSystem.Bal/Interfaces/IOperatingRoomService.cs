using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IOperatingRoomService
    {
        bool ApplySurguryChargesToBill(int corporateId, int facilityId, int encounterId, string reclaimFlag, long claimId);
        bool CheckDuplicateRecord(OperatingRoom model);
        OperatingRoom GetOperatingRoomDetail(int id);
        List<OperatingRoomCustomModel> GetOperatingRoomsList(int type, int encounterId, int patientId);
        List<OperatingRoomCustomModel> SaveOperatingRoomData(OperatingRoom model);
    }
}