
using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Bal.Mapper;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DiagnosisBal : BaseBal
    {
        private DiagnosisMapper DiagnosisMapper { get; set; }

        public DiagnosisBal(string diagnosisTableNumber, string drgTableNumber)
        {
            if (!string.IsNullOrEmpty(diagnosisTableNumber))
                DiagnosisTableNumber = diagnosisTableNumber;

            if (!string.IsNullOrEmpty(drgTableNumber))
                DrgTableNumber = drgTableNumber;

            DiagnosisMapper = new DiagnosisMapper(drgTableNumber, diagnosisTableNumber);
        }


        public DiagnosisBal(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber))
                CptTableNumber = cptTableNumber;

            if (!string.IsNullOrEmpty(serviceCodeTableNumber))
                ServiceCodeTableNumber = serviceCodeTableNumber;

            if (!string.IsNullOrEmpty(drgTableNumber))
                DrgTableNumber = drgTableNumber;

            if (!string.IsNullOrEmpty(drugTableNumber))
                DrugTableNumber = drugTableNumber;

            if (!string.IsNullOrEmpty(hcpcsTableNumber))
                HcpcsTableNumber = hcpcsTableNumber;

            if (!string.IsNullOrEmpty(diagnosisTableNumber))
                DiagnosisTableNumber = diagnosisTableNumber;

            DiagnosisMapper = new DiagnosisMapper(drgTableNumber, diagnosisTableNumber);
        }

        //Function to get all GlobalCodes
        /// <summary>
        /// Gets all diagnosis codes.
        /// </summary>
        /// <returns></returns>
        public List<DiagnosisCode> GetAllDiagnosisCodes(int facilityId)
        {
            var currentDateTime = GetInvariantCultureDateTime(facilityId);
            using (var gcRep = UnitOfWork.DiagnosisCodeRepository)
            {
                var list =
                    gcRep.Where(
                        s =>
                            (s.DiagnosisEffectiveStartDate == null ||
                             (s.DiagnosisEffectiveStartDate <= currentDateTime &&
                              s.DiagnosisEffectiveEndDate >= currentDateTime)) &&
                            s.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).ToList();
                return list;
            }
        }

        /// <summary>
        /// Gets the diagnosis list.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<DiagnosisCustomModel> GetDiagnosisList(int patientId)
        {
            var list = new List<DiagnosisCustomModel>();
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                //Get the current diagnosis record by Encounter ID and Patient ID
                var diagnosisList =
                    rep.Where(d => d.PatientID != null && (int)d.PatientID == patientId)
                        .OrderBy(ss => ss.DiagnosisType)
                        .ToList();
                list.AddRange(diagnosisList.Select(item => DiagnosisMapper.MapModelToViewModel(item)));
            }
            /*Updated By krishna on 30072015*/
            return list.OrderByDescending(x => x.CreatedDate).ToList();
        }

        /// <summary>
        /// Gets the diagnosis list.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public List<DiagnosisCustomModel> GetDiagnosisList(int patientId, int encounterid)
        {
            var list = new List<DiagnosisCustomModel>();
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                //Get the current diagnosis record by Encounter ID and Patient ID
                var diagnosisList = rep.Where(d => d.PatientID != null && (int)d.PatientID == patientId && d.EncounterID == encounterid).OrderBy(ss => ss.DiagnosisType).ThenByDescending(x => x.CreatedDate).ToList();
                //using (var encounterBal = new EncounterBal())
                //{
                //    //Add items to List
                //    list.AddRange(diagnosisList.Select(item => new DiagnosisCustomModel
                //    {
                //        DiagnosisID = item.DiagnosisID,
                //        DiagnosisType = item.DiagnosisType,
                //        CorporateID = item.CorporateID,
                //        FacilityID = item.FacilityID,
                //        PatientID = item.PatientID,
                //        EncounterID = item.EncounterID,
                //        MedicalRecordNumber = item.MedicalRecordNumber,
                //        DiagnosisCode = item.DiagnosisType == 3 ? GetDRGCodeById(Convert.ToInt32(item.DRGCodeID)).CodeNumbering : item.DiagnosisCode,
                //        DiagnosisCodeId = item.DiagnosisCodeId,
                //        DiagnosisCodeDescription = item.DiagnosisType == 3 ? GetDRGCodeById(Convert.ToInt32(item.DRGCodeID)).CodeDescription : item.DiagnosisCodeDescription,
                //        Notes = item.Notes,
                //        InitiallyEnteredByPhysicianId = item.InitiallyEnteredByPhysicianId,
                //        ReviewedByCoderID = item.ReviewedByCoderID,
                //        ReviewedByPhysicianID = item.ReviewedByPhysicianID,
                //        CreatedBy = item.CreatedBy,
                //        CreatedDate = item.CreatedDate,
                //        ModifiedBy = item.ModifiedBy,
                //        DRGCodeID = item.DRGCodeID,
                //        DiagnosisTypeName = GetDiagnosisTypeById(item.DiagnosisType),
                //        EncounterNumber =
                //            encounterBal.GetEncounterNumberByEncounterId(Convert.ToInt32(item.EncounterID)),
                //        DrgCodeValue = GetDRGCodeById(Convert.ToInt32(item.DRGCodeID)).CodeNumbering,
                //        DrgCodeDescription = GetDRGCodeById(Convert.ToInt32(item.DRGCodeID)).CodeDescription,
                //        EnteredBy = GetUserNameById(item.CreatedBy)
                //    }));
                //}

                list.AddRange(diagnosisList.Select(item => DiagnosisMapper.MapModelToViewModel(item)));
            }
            //return list;
            /*Updated By krishna on 30072015*/
            return list.OrderByDescending(x => x.CreatedDate).ToList();
        }

        /// <summary>
        /// Gets the previous diagnosis list.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public List<DiagnosisCustomModel> GetPreviousDiagnosisList(int patientId, int encounterid)
        {
            var list = new List<DiagnosisCustomModel>();
            using (var rep = this.UnitOfWork.DiagnosisRespository)
            {
                //Get the current diagnosis record by Encounter ID and Patient ID
                var diagnosisList = rep.Where(d => d.PatientID != null && (int)d.PatientID == patientId && d.EncounterID != encounterid).OrderBy(ss => ss.DiagnosisType).ToList();
                list.AddRange(diagnosisList.Select(item => this.DiagnosisMapper.MapModelToViewModel(item)));
            }
            /*Updated By krishna on 30072015*/
            return list.OrderByDescending(x => x.CreatedDate).ToList();
        }

        /// <summary>
        /// Gets the filtered diagnosis codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<DiagnosisCode> GetFilteredDiagnosisCodes(string text, long userId, long facilityId)
        {
            using (var gcRep = UnitOfWork.DiagnosisCodeRepository)
            {
                return gcRep.GetDiagnosisCodes(text, DiagnosisTableNumber, userId, facilityId);
                //if (!string.IsNullOrEmpty(text))
                //    text = text.ToLower();

                //var searchString = text.Split(' ');

                //var list = gcRep.Where(
                //    s =>
                //        s.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).ToList();

                //var nList = list.Where(
                //    s => (s.DiagnosisCode1.ToLower().Contains(text) ||
                //         s.DiagnosisFullDescription.ToLower().ContainsAny(searchString) ||
                //         s.DiagnosisMediumDescription.ToLower().ContainsAny(searchString) ||
                //         s.ShortDescription.ToLower().ContainsAny(searchString)))
                //    .OrderBy(m => m.DiagnosisFullDescription)
                //    .ToList();

                //return nList;
            }
        }

        /// <summary>
        /// Saves the diagnosis.
        /// </summary>
        /// <param name="vm">The model.</param>
        /// <returns></returns>
        public int SaveDiagnosis(DiagnosisCustomModel vm)
        {
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                var model = DiagnosisMapper.MapViewModelToModel(vm);
                var diagnosisCode = GetDiagnosisCodeById(vm.DiagnosisCode);
                if (model.DiagnosisType == (int)DiagnosisType.Primary || model.DiagnosisType == (int)DiagnosisType.Secondary)
                    model.DiagnosisCodeId = diagnosisCode.DiagnosisTableNumberId;

                if (model.DiagnosisID > 0)
                    rep.UpdateEntity(model, model.DiagnosisID);
                else
                    rep.Create(model);

                return model.DiagnosisID;
            }
        }

        /// <summary>
        /// Gets the diagnosis code by identifier.
        /// </summary>
        /// <param name="codeId">The code identifier.</param>
        /// <returns></returns>
        private DiagnosisCode GetDiagnosisCodeById(string codeId)
        {
            using (var rep = UnitOfWork.DiagnosisCodeRepository)
            {
                var diagnosis = rep.Where(d => d.DiagnosisCode1 == codeId && d.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).FirstOrDefault();
                return diagnosis ?? new DiagnosisCode();
            }
        }

        /// <summary>
        /// Gets the new diagnosis by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public DiagnosisCustomModel GetNewDiagnosisByEncounterId(int encounterId, int patientId)
        {
            var diagnosis = new Diagnosis();
            var diagnosisCustomModel = DiagnosisMapper.MapModelToViewModel(diagnosis);

            //using (var rep = UnitOfWork.DiagnosisRespository)
            //{



            //    var drg = GetDRGCodeById(Convert.ToInt32(diagnosis.DRGCodeID));

            //    //Initialize the customModel object.
            //    diagnosisCustomModel = new DiagnosisCustomModel
            //    {
            //        DiagnosisID = diagnosis.DiagnosisID,
            //        DiagnosisType = diagnosis.DiagnosisType,
            //        MedicalRecordNumber = diagnosis.MedicalRecordNumber,
            //        DiagnosisCodeId = diagnosis.DiagnosisCodeId,
            //        DiagnosisCodeDescription = diagnosis.DiagnosisCodeDescription,
            //        Notes = diagnosis.Notes,
            //        InitiallyEnteredByPhysicianId = diagnosis.InitiallyEnteredByPhysicianId,
            //        ReviewedByCoderID = diagnosis.ReviewedByCoderID,
            //        ReviewedByPhysicianID = diagnosis.ReviewedByPhysicianID,
            //        CreatedBy = diagnosis.CreatedBy,
            //        CreatedDate = diagnosis.CreatedDate,
            //        ModifiedBy = diagnosis.ModifiedBy,
            //        PrimaryDiagnosisCode = new DiagnosisCode(),
            //        SecondaryDiagnosisCode = new DiagnosisCode(),
            //        DrgCodeValue = drg.CodeNumbering,
            //        DrgCodeDescription = drg.CodeDescription,
            //        EnteredBy = GetUserNameById(diagnosis.CreatedBy)
            //    };
            //}
            return diagnosisCustomModel;
        }

        /// <summary>
        /// Gets the diagnosis type by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetDiagnosisTypeById(int id)
        {
            switch (id)
            {
                case 1:
                    return DiagnosisType.Primary.ToString() + " Diagnosis";
                case 2:
                    return DiagnosisType.Secondary.ToString() + " Diagnosis";
                case 3:
                    return "DRG";
                case 4:
                    return "Major CPT";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the diagnosis by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Diagnosis GetDiagnosisById(string id)
        {
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                var currentId = Convert.ToInt32(id);
                var diagnosis = rep.Where(d => d.DiagnosisID == currentId).FirstOrDefault();
                return diagnosis;
            }
        }

        /// <summary>
        /// Checks if any diagnosis exists.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public int CheckIfAnyDiagnosisExists(int encounterId)
        {
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                var primaryDiagnosis = Convert.ToInt32(DiagnosisType.Primary);
                var diagnosis = rep.Where(d => d.EncounterID == encounterId && d.DiagnosisType == primaryDiagnosis).FirstOrDefault();
                return diagnosis != null ? diagnosis.DiagnosisID : 0;
            }
        }

        /// <summary>
        /// Gets the diagnosis information by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public Diagnosis GetDiagnosisInfoByEncounterId(int encounterId)
        {
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                var primaryDiagnosis = Convert.ToInt32(DiagnosisType.Primary);
                var diagnosis = rep.Where(d => d.EncounterID == encounterId && d.DiagnosisType == primaryDiagnosis).FirstOrDefault();
                return diagnosis;
            }
        }

        /// <summary>
        /// Gets the diagnosis list by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<DiagnosisCustomModel> GetDiagnosisListByEncounterId(int encounterId)
        {
            var list = new List<DiagnosisCustomModel>();
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                //Get the current diagnosis record by Encounter ID and Patient ID
                var diagnosisList =
                    rep.Where(d => d.PatientID != null && (int)d.EncounterID == encounterId)
                        .OrderBy(ss => ss.DiagnosisType)
                        .ToList();
                list.AddRange(diagnosisList.Select(item => DiagnosisMapper.MapModelToViewModel(item)));
            }
            return list;
        }

        /// <summary>
        /// Checks if duplicate diagnosis against current encounter.
        /// </summary>
        /// <param name="diagnosisCode">The diagnosis code.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="diagnosisId">The diagnosis identifier.</param>
        /// <returns></returns>
        public bool CheckIfDuplicateDiagnosisAgainstCurrentEncounter(string diagnosisCode, int encounterId, int diagnosisId)
        {
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                var result = diagnosisId == 0
                    ? rep.Where(
                        d =>
                            d.EncounterID == encounterId && d.DiagnosisCode.Equals(diagnosisCode) &&
                            d.IsDeleted == false).Any()
                    : rep.Where(
                        d => d.DiagnosisID != diagnosisId &&
                            d.EncounterID == encounterId && d.DiagnosisCode.Equals(diagnosisCode) &&
                            d.IsDeleted == false).Any();
                return result;
            }
        }

        /// <summary>
        /// Gets the DRG diagnosis information by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public Diagnosis GetDRGDiagnosisInfoByEncounterId(int encounterId)
        {
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                var drgDiagnosis = Convert.ToInt32(DiagnosisType.DRG);
                var diagnosis = rep.Where(d => d.EncounterID == encounterId && d.DiagnosisType == drgDiagnosis).FirstOrDefault();
                return diagnosis;
            }
        }

        public IEnumerable<DiagnosisCode> ExportFilteredDiagnosisCodes(string text, string tableNumber)
        {
            using (var gcRep = UnitOfWork.DiagnosisCodeRepository)
            {
                if (!string.IsNullOrEmpty(text))
                    text = text.ToLower();

                var searchString = text.Split(' ');

                var list = gcRep.Where(
                    s =>
                        s.DiagnosisTableNumber.Trim().Equals(tableNumber)).ToList();

                var nList = list.Where(
                    s => (s.DiagnosisCode1.ToLower().Contains(text) ||
                         s.DiagnosisFullDescription.ToLower().ContainsAny(searchString) ||
                         s.DiagnosisMediumDescription.ToLower().ContainsAny(searchString) ||
                         s.ShortDescription.ToLower().ContainsAny(searchString)))
                    .OrderBy(m => m.DiagnosisFullDescription)
                    .ToList();

                return nList;
            }
        }


        public List<DiagnosisCode> GetFilteredDiagnosis(List<string> codeList)
        {

            using (var gcRep = UnitOfWork.DiagnosisCodeRepository)
            {
                var list =
                     gcRep.Where(
                         s =>
                             (s.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber) &&
                            codeList.Contains(s.DiagnosisCode1))).ToList();


                return list;
            }
        }

        public DiagnosisTabData GetDiagnosisTabData(long pId, long eId = 0, long physicianId = 0, string diagnosisTn = "", string drgTn = "")
        {
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                var vm = rep.GetDiagnosisTabData(pId, eId, physicianId, diagnosisTn, drgTn);
                return vm;
            }
        }

        public IEnumerable<DiagnosisCustomModel> GetCurrentDiagnosisData(long pId, long eid)
        {
            using (var rep = UnitOfWork.DiagnosisRespository)
                return rep.GetCurrentDiagnosisData(pId, eid);
        }

        public DiagnosisTabData DeleteCurrentDiagnosis(long userId, long id, string drgTn)
        {
            using (var rep = UnitOfWork.DiagnosisRespository)
                return rep.DeleteCurrentDiagnosis(userId, id, drgTn);
        }
    }
}
