using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IMappingPatientBedService
    {
        int AddUpdateMappingPatientBed(MappingPatientBed mappPatientBed);
        MappingPatientBed GetAllMappingPatientBedById(string id);
        MappingPatientBed GetMappingPatientBedByEncounterId(string id);
        MappingPatientBed GetMappingPatientBedById(int id);
        List<MappingPatientBed> GetMappingPatientBedList();
    }
}