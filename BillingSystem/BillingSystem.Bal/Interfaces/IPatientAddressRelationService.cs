using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientAddressRelationService
    {
        int AddPatientAddressRelation(PatientAddressRelation model);
        IEnumerable<PatientAddressRelationCustomModel> GetPatientAddressRelation(int patientId);
        PatientAddressRelation GetPatientRelationAddressById(int patientAddresssRelationId);
    }
}