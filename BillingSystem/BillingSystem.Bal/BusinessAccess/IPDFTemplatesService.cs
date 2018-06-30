using System.Collections.Generic;
using BillingSystem.Model.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPDFTemplatesService
    {
        string GetNewFormDetailsByFormType();
        List<OtherPatientForm> GetNursingAssessmentFormData(int patientId, int encounterId, string setId);
        string GetSignaturePathNurseForm(int ecounterId, int patinetId, string setId);
        List<OtherPatientForm> ListNurseAssessmentForm(int patientId, int encounterId, string globalCodeCategory, string setId);
        int SavePDFTemplates(OtherPatientForm oPatientNusre);
        int UpdateOutPatientAssessment(OtherPatientForm m);
    }
}