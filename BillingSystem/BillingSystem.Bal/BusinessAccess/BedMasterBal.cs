using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using WebGrease.Css.Extensions;

namespace BillingSystem.Bal.BusinessAccess
{
    public class BedMasterBal : BaseBal
    {
        public BedMasterBal()
        {

        }

        public BedMasterBal(string serviceTableNumber)
        {
            ServiceCodeTableNumber = serviceTableNumber;
        }

        /// <summary>
        /// Method to add/Update the BedRateCard in the database.
        /// </summary>
        /// <param name="bedMaster"></param>
        /// <returns></returns>
        public int AddUpdateBedMaster(UBedMaster bedMaster)
        {
            // bedMaster.FacilityStructureId = 12;
            using (var bedMasterRepository = UnitOfWork.BedMasterRepository)
            {
                if (bedMaster.BedId > 0)
                    bedMasterRepository.UpdateEntity(bedMaster, bedMaster.BedId);
                else
                    bedMasterRepository.Create(bedMaster);
            }
            return bedMaster.BedId;
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UBedMaster GetBedMasterById(int id)
        {
            using (var bedRateCardRep = UnitOfWork.BedMasterRepository)
            {
                var bedMaster = bedRateCardRep.Where(x => x.BedId == id).FirstOrDefault();
                return bedMaster;
            }
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UBedMaster GetBedMasterByStructureId(int id)
        {
            using (var bedRateCardRep = UnitOfWork.BedMasterRepository)
            {
                var bedMaster = bedRateCardRep.Where(x => x.FacilityStructureId == id).FirstOrDefault();
                return bedMaster;
            }
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UBedMaster GetBedMasterIdByStructureId(int id)
        {
            using (var bedRateCardRep = UnitOfWork.BedMasterRepository)
            {
                var bedMaster = bedRateCardRep.Where(x => x.FacilityStructureId == id).FirstOrDefault();
                return bedMaster ?? new UBedMaster();
            }
        }

        //private DateTime? GetExpectedEndDate(string bedid)
        //{
        //    var mappingPatientBed = new MappingPatientBedBal();
        //    var mappingPatientBedObj = mappingPatientBed.GetAllMappingPatientBedById(bedid);
        //    return mappingPatientBedObj == null ? null : mappingPatientBed.GetAllMappingPatientBedById(bedid).ExpectedEndDate;
        //}

        private string GetBedNameFromFacilityStructure(int facilityStructureId)
        {
            using (var rep = UnitOfWork.FacilityStructureRepository)
            {
                var list = rep.Where(f => f.FacilityStructureId == facilityStructureId).FirstOrDefault();
                return list != null ? list.FacilityStructureName : string.Empty;
            }
        }

        public string GetBedNameFromBedId(int bedId)
        {
            using (var rep = UnitOfWork.FacilityStructureRepository)
            {
                var bedmasterObj = GetBedMasterById(bedId);
                if (bedmasterObj != null)
                {
                    var list = rep.Where(f => f.FacilityStructureId == bedmasterObj.FacilityStructureId).FirstOrDefault();
                    return list != null ? list.FacilityStructureName : string.Empty;
                }
                return string.Empty;
            }
        }

        public string GetBedNameByInPatientEncounterId(string encounterId)
        {
            var bedName = string.Empty;
            using (var mappingBedRep = UnitOfWork.MappingPatientBedRepository)
            {
                var bedTypeAssigned = mappingBedRep.Where(m => m.EncounterID.Equals(encounterId) && m.EndDate == null).OrderByDescending(x => x.PatientID).FirstOrDefault();
                if (bedTypeAssigned != null)
                {
                    var bedId = Convert.ToInt32(bedTypeAssigned.BedNumber);
                    using (var rep = UnitOfWork.BedMasterRepository)
                    {
                        var bedMaster = rep.Where(b => b.BedId == bedId).FirstOrDefault();
                        if (bedMaster != null)
                        {
                            var fsId = Convert.ToInt32(bedMaster.FacilityStructureId);
                            using (var fsRep = UnitOfWork.FacilityStructureRepository)
                            {
                                var fsStructure = fsRep.Where(f => f.FacilityStructureId == fsId).FirstOrDefault();
                                if (fsStructure != null)
                                    bedName = fsStructure.FacilityStructureName;
                            }
                        }
                    }
                }
            }
            return bedName;
        }

        public string GetBedByInPatientEncounterId(string encounterId)
        {
            var bedmaster = "0";
            using (var mappingBedRep = UnitOfWork.MappingPatientBedRepository)
            {
                var bedTypeAssigned = mappingBedRep.Where(m => m.EncounterID.Equals(encounterId) && m.EndDate == null).FirstOrDefault();
                // Commented the Below line as there is error of not checking the ENd time is null then it will always picks the first value of the encounter mapping
                // var bedTypeAssigned = mappingBedRep.Where(m => m.EncounterID.Equals(encounterId)).FirstOrDefault();
                if (bedTypeAssigned != null)
                {
                    var bedId = Convert.ToInt32(bedTypeAssigned.BedNumber);
                    using (var rep = UnitOfWork.BedMasterRepository)
                    {
                        var bedMaster = rep.Where(b => b.BedId == bedId).FirstOrDefault();
                        if (bedMaster != null)
                        {
                            bedmaster = bedMaster.BedId.ToString();
                        }
                    }
                }
            }
            return bedmaster;
        }

        /// <summary>
        /// Get the GetBedRateCardsList
        /// </summary>
        /// <returns>Return the BedRateCard List</returns>
        public IEnumerable<BedMasterCustomModel> GetBedMasterListByRole(int facilityId, int corporateid)
        {
            var lstBedMaster = new List<BedMasterCustomModel>();
            using (var bedRateCardRep = UnitOfWork.BedMasterRepository)
            {
                var bedList =
                    bedRateCardRep.GetAll()
                        .Where(
                            _ =>
                                (_.IsDeleted == null || !(bool)_.IsDeleted) && _.IsActive && _.FacilityId == facilityId)
                        .ToList();

                var facBal = new FacilityBal();
                var globalBal = new GlobalCodeBal();

                lstBedMaster.AddRange(bedList.Select(item => new BedMasterCustomModel
                {
                    BedMaster = item,
                    FacilityName = facBal.GetFacilityNameById(Convert.ToInt32(item.FacilityId)),
                    BedTypeName =
                        globalBal.GetNameByGlobalCodeValueAndCategoryValue(
                            Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.Bedtypes).ToString(),
                            Convert.ToString(item.BedType)),
                    BedName = GetBedNameFromFacilityStructure(Convert.ToInt32(item.FacilityStructureId))
                }));

                return lstBedMaster;

            }
        }

        public string GetOverRideBedTypeByInPatientEncounterId(string encounterId)
        {
            var bedmaster = string.Empty;
            using (var mappingBedRep = UnitOfWork.MappingPatientBedRepository)
            {
                var bedTypeAssigned = mappingBedRep.Where(m => m.EncounterID.Equals(encounterId)).FirstOrDefault();
                if (bedTypeAssigned != null)
                {
                    bedmaster = Convert.ToInt32(bedTypeAssigned.OverrideBedType).ToString();
                }
            }
            return bedmaster;
        }

        public bool CheckIdBedOccupied(int bedid)
        {
            using (var bedRateCardRep = UnitOfWork.BedMasterRepository)
            {
                var bedMaster = bedRateCardRep.Where(x => x.BedId == bedid && x.IsOccupied != false).FirstOrDefault();
                return bedMaster != null;
            }
        }



        /// <summary>
        /// Deletes the bed master by identifier.
        /// </summary>
        /// <param name="facilityStructureId">The facility structure identifier.</param>
        /// <returns></returns>
        public bool DeleteBedMasterById(int facilityStructureId)
        {
            using (var bedRateCardRep = UnitOfWork.BedMasterRepository)
            {
                var bedMaster = bedRateCardRep.Where(x => x.FacilityStructureId == facilityStructureId).FirstOrDefault();
                return bedRateCardRep.Delete(bedMaster) > 0;
            }
        }



        /// <summary>
        /// Gets the bed struture for facility.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <returns></returns>
        public List<FacilityBedStructureCustomModel> GetBedStrutureForFacility(int facilityId, int corporateid)
        {
            using (var rep = UnitOfWork.BedMasterRepository)
            {
                var allBeds = rep.GetBedStrutureForFacility(corporateid, facilityId);
                if (allBeds.Any())
                {

                    /*
                     * Changes by: Amit Jain
                     * On: 02 March, 2016
                     * Purpose: Earlier as per the below commented code, the check was missing from BedRateCard, Now Implemented
                     */
                    /*********************************Changes start here************************************/

                    //foreach (var facilityBedStructureCustomModel in ovrrdeBList)
                    //{
                    //    facilityBedStructureCustomModel.BedOverrideTypeList =
                    //        facilityBedStructureCustomModel.CanOverRide
                    //            ? GetCustomOverrideableBedServiceType(facilityBedStructureCustomModel.OverRideWith)
                    //            : null;
                    //}

                    if (allBeds.Count > 0)
                    {
                        allBeds = allBeds.Select(item =>
                        {
                            item.BedOverrideTypeList = item.CanOverRide && !string.IsNullOrEmpty(item.OverRideWith)
                                ? GetOverrideBedsListInEncounters(item.OverRideWith)
                                : new List<DropdownListData>();
                            return item;
                        }).ToList();
                    }

                    /*********************************Changes end here************************************/
                }
                return allBeds;
            }
        }

        /// <summary>
        /// Gets the type of the custom overrideable bed service.
        /// </summary>
        /// <param name="serviceCodesString">The service codes string.</param>
        /// <returns></returns>
        private IEnumerable<DropdownListData> GetCustomOverrideableBedServiceType(string serviceCodesString)
        {
            var overRideBedChargesDdl = new List<DropdownListData>();
            using (var serviceCodeRepository = UnitOfWork.ServiceCodeRepository)
            {
                var overirdeWithServiceCodes = !string.IsNullOrEmpty(serviceCodesString)
                    ? serviceCodesString
                    : string.Empty;
                //var serviceCodesList = serviceCodeRepository.GetAll();
                var serviceCodesList = serviceCodeRepository.Where(s => s.IsActive == true && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).ToList();
                var overrideServiceCodesList = new List<ServiceCode>();
                if (overirdeWithServiceCodes.Contains(","))
                {
                    var serviceCodesArray = overirdeWithServiceCodes.Split(',');
                    foreach (var item in serviceCodesArray)
                    {
                        overrideServiceCodesList.AddRange(
                            serviceCodesList.Where(x => x.ServiceCodeValue.ToLower().Equals(item.ToLower())));
                    }
                }
                else
                {
                    overrideServiceCodesList.AddRange(
                        serviceCodesList.Where(
                            x => x.ServiceCodeValue.ToLower().Equals(overirdeWithServiceCodes.ToLower())));
                }
                overrideServiceCodesList = overrideServiceCodesList.Distinct().ToList();
                overRideBedChargesDdl.AddRange(overrideServiceCodesList.Select(item => new DropdownListData
                {
                    Text = item.ServiceCodeValue + " " + item.ServiceCodeDescription,
                    Value = item.ServiceCodeValue,
                }));
                return overRideBedChargesDdl;
            }
        }

        /// <summary>
        /// Gets the bed type by service code.
        /// </summary>
        /// <param name="serviceCode">The service code.</param>
        /// <returns></returns>
        public int? GetBedTypeByServiceCode(string serviceCode)
        {
            using (var bedRateCardRep = UnitOfWork.BedRateCardRepository)
            {
                var bedMaster = bedRateCardRep.GetAll().FirstOrDefault(x => x.ServiceCodeValue.Equals(serviceCode));
                return bedMaster != null ? (bedMaster.BedTypes) : null;
            }
        }



        /// <summary>
        /// Gets the type of the custom overrideable bed service.
        /// </summary>
        /// <param name="serviceCodeValues">The service codes string.</param>
        /// <returns></returns>
        public IEnumerable<DropdownListData> GetOverrideBedsListInEncounters(string serviceCodeValues)
        {
            var currentDateTime = DateTime.Now.Date;
            var ddlData = new List<DropdownListData>();
            serviceCodeValues = !string.IsNullOrEmpty(serviceCodeValues) ? serviceCodeValues.Trim() : string.Empty;
            if (!string.IsNullOrEmpty(serviceCodeValues))
            {
                var scArray = serviceCodeValues.Split(',').ToList();
                using (var rep = UnitOfWork.ServiceCodeRepository)
                {
                    var scList =
                        rep.Where(
                            s => s.IsActive == true && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)
                                 && scArray.Contains(s.ServiceCodeValue.Trim())).ToList();

                    using (var brRep = UnitOfWork.BedRateCardRepository)
                    {
                        var list =
                            brRep.Where(
                                b =>
                                    scArray.Contains(b.ServiceCodeValue.Trim()) && b.IsDeleted != true &&
                                    b.IsActive &&
                                    b.EffectiveFrom <= currentDateTime && (b.EffectiveTill == null || b.EffectiveTill >= currentDateTime))
                                .Select(s => s.ServiceCodeValue.Trim())
                                .ToList();

                        scList = scList.Where(s => list.Contains(s.ServiceCodeValue.Trim())).Distinct().ToList();

                        if (scList.Count > 0)
                            scList = scList.OrderBy(v => v.ServiceCodeValue, new NumericComparer()).ToList();
                    }

                    if (scList.Count > 0)
                    {
                        ddlData.AddRange(scList.Select(item => new DropdownListData
                        {
                            Text = item.ServiceCodeValue + " " + item.ServiceCodeDescription,
                            Value = item.ServiceCodeValue
                        }));
                    }
                }
            }
            return ddlData;
        }





        //--------------------Not In Use-------------------------------------
        //public List<BedMasterCustomModel> GetAvialableBedMasterList(int facilityId, int floorid, int departmentid, int roomid)
        //{
        //    var lstBedMaster = new List<BedMasterCustomModel>();
        //    using (var bedRateCardRep = UnitOfWork.BedMasterRepository)
        //    {
        //        var bedList =
        //            bedRateCardRep.GetAll().Where(_ => (_.IsDeleted == null || !(bool)_.IsDeleted) && _.IsActive).ToList();
        //        var facBal = new FacilityBal();
        //        var globalBal = new GlobalCodeBal();
        //        var facilitystructure = new FacilityStructureBal();

        //        using (var facStructBal = new FacilityStructureBal())
        //        {
        //            var objPatientInfoData = facStructBal.FindFacilityStruture(facilityId, floorid, departmentid, roomid);
        //            lstBedMaster.AddRange(bedList.Select(item => new BedMasterCustomModel
        //            {
        //                BedMaster = item,
        //                FacilityName = facBal.GetFacilityNameById(Convert.ToInt32(item.FacilityId)),
        //                BedTypeName = globalBal.GetGlobalCodeNameByIdAndCategoryId(Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.Bedtypes).ToString(), Convert.ToInt32(item.BedType)),
        //                BedName = GetBedNameFromFacilityStructure(Convert.ToInt32(item.FacilityStructureId)),
        //                ExpectedEndDate = GetExpectedEndDate(item.BedId.ToString()),
        //                //BedOverrideTypeList = globalBal.GetGlobalCodeWithExternalByCategoryIdBedTypeId(Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.Bedtypes).ToString(), Convert.ToInt32(item.FacilityStructureId)),
        //                BedOverrideTypeList = GetOverrideableBedServiceType(Convert.ToInt32(item.FacilityStructureId)),
        //                BedOverride = facilitystructure.GetExternalValueByStrutureId(Convert.ToInt32(item.FacilityStructureId)),
        //                BedMasterModel = facilitystructure.GetBedParents(item.FacilityStructureId)
        //            }));

        //            lstBedMaster = lstBedMaster.Where(t => (objPatientInfoData.Any(z => z.FacilityStructureId == t.BedMaster.FacilityStructureId))).ToList();
        //        }
        //        return lstBedMaster;
        //    }
        //}

        //public IEnumerable<BedMasterCustomModel> GetAvialableBedMasterList(int facilityId)
        //{
        //    var lstBedMaster = new List<BedMasterCustomModel>();
        //    using (var bedRateCardRep = UnitOfWork.BedMasterRepository)
        //    {
        //        var bedList =
        //            bedRateCardRep.GetAll().Where(_ => (_.IsDeleted == null || !(bool)_.IsDeleted) && _.IsActive).ToList();

        //        //Added by Amit Jain on 20102014
        //        if (facilityId > 0)
        //            bedList = bedList.Where(f => f.FacilityId == facilityId).ToList();

        //        var facBal = new FacilityBal();
        //        var globalBal = new GlobalCodeBal();
        //        var facilitystructure = new FacilityStructureBal();

        //        //var servicecodeBal = new MappingBedServiceBal();
        //        lstBedMaster.AddRange(bedList.Select(item => new BedMasterCustomModel
        //        {
        //            BedMaster = item,
        //            FacilityName = facBal.GetFacilityNameById(Convert.ToInt32(item.FacilityId)),
        //            BedTypeName = globalBal.GetGlobalCodeNameByIdAndCategoryId(Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.Bedtypes).ToString(), Convert.ToInt32(item.BedType)),
        //            BedName = GetBedNameFromFacilityStructure(Convert.ToInt32(item.FacilityStructureId)),
        //            ExpectedEndDate = GetExpectedEndDate(item.BedId.ToString()),
        //            //BedOverrideTypeList = globalBal.GetGlobalCodeWithExternalByCategoryIdBedTypeId(Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.Bedtypes).ToString(), Convert.ToInt32(item.FacilityStructureId)),
        //            BedOverrideTypeList = GetOverrideableBedServiceType(Convert.ToInt32(item.FacilityStructureId)),
        //            BedOverride = facilitystructure.GetExternalValueByStrutureId(Convert.ToInt32(item.FacilityStructureId)),
        //            BedMasterModel = facilitystructure.GetBedParents(item.FacilityStructureId),
        //            NonChargeableRoom = IsRoomNonChargeable(Convert.ToInt32(item.FacilityStructureId)),
        //        }));
        //        return lstBedMaster;
        //    }
        //}


        //private bool IsRoomNonChargeable(int facilityStructureId)
        //{
        //    using (var rep = UnitOfWork.FacilityStructureRepository)
        //    {
        //        var facilityStructureIdOfRoom =
        //            rep.Where(f => f.FacilityStructureId == facilityStructureId)
        //                .Select(f1 => f1.ParentId)
        //                .FirstOrDefault();
        //        if (facilityStructureIdOfRoom > 0)
        //        {
        //            var isExist =
        //                rep.Where(
        //                    f =>
        //                        f.FacilityStructureId == facilityStructureIdOfRoom &&
        //                        !string.IsNullOrEmpty(f.ExternalValue1) && f.ExternalValue1.Equals("1")).Any();
        //            return isExist;
        //        }
        //    }
        //    return false;
        //}


        /// <summary>
        /// Gets the type of the overrideable bed service.
        /// </summary>
        /// <param name="facilityStructureId">The facility structure identifier.</param>
        /// <returns></returns>
        //public IEnumerable<DropdownListData> GetOverrideableBedServiceType(int facilityStructureId)
        //{
        //    var overRideBedChargesDDL = new List<DropdownListData>();
        //    using (var rep = UnitOfWork.ServiceCodeRepository)
        //    {
        //        using (var facilitystructureBal = new FacilityStructureBal())
        //        {
        //            var facilityStrutureobj = facilitystructureBal.GetFacilityStructureById(facilityStructureId);
        //            if (!string.IsNullOrEmpty(facilityStrutureobj.ExternalValue1))
        //            {
        //                var overirdeWithServiceCodes = !string.IsNullOrEmpty(facilityStrutureobj.ExternalValue2)
        //                    ? facilityStrutureobj.ExternalValue2
        //                    : string.Empty;
        //                var serviceCodesList = rep.GetAll();
        //                var overrideServiceCodesList = new List<ServiceCode>();
        //                if (overirdeWithServiceCodes.Contains(","))
        //                {
        //                    var serviceCodesArray = overirdeWithServiceCodes.Split(',');
        //                    foreach (var item in serviceCodesArray)
        //                    {
        //                        overrideServiceCodesList.AddRange(
        //                            serviceCodesList.Where(x => x.ServiceCodeValue.ToLower().Equals(item.ToLower())));
        //                    }
        //                }
        //                else
        //                {
        //                    overrideServiceCodesList.AddRange(
        //                            serviceCodesList.Where(x => x.ServiceCodeValue.ToLower().Equals(overirdeWithServiceCodes.ToLower())));
        //                }
        //                overrideServiceCodesList = overrideServiceCodesList.Distinct().ToList();
        //                foreach (var item in overrideServiceCodesList)
        //                {
        //                    using (var bedratecard = new BedRateCardBal())
        //                    {
        //                        var dropdowndata = new DropdownListData
        //                        {
        //                            Text = item.ServiceCodeDescription,
        //                            Value = item.ServiceCodeValue,
        //                        };
        //                        overRideBedChargesDDL.Add(dropdowndata);
        //                    }
        //                }
        //            }
        //            return overRideBedChargesDDL;
        //        }
        //    }
        //}



        //--------------------Not In Use-------------------------------------
    }
}
