using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DashboardBal : BaseBal
    {

        /// <summary>
        /// Gets the dashboard chart data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BedOccupancyCustomModel> GetDashboardChartData(int corporateId, int facilityId)
        {
            var list = new List<BedOccupancyCustomModel>();
            using (var rep = UnitOfWork.BedMasterRepository)
            {
                var result = rep.GetBedOccupanyData(corporateId, facilityId);
                if (result != null && result.Count > 0)
                {
                    list.AddRange(result.Select(item => new BedOccupancyCustomModel
                    {
                        IsOccupied = item.IsOccupied,
                        BedStatus = item.BedStatus,
                        Beds = item.Beds, //X Axis
                        TotalBeds = item.TotalBeds, //Y Axis
                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the bed occupency by floor data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BedOccupancyCustomModel> GetBedOccupencyByFloorData(int corporateId, int facilityId)
        {
            var list = new List<BedOccupancyCustomModel>();
            using (var rep = UnitOfWork.BedMasterRepository)
            {
                var result = rep.GetBedOccupanyByFloorData(corporateId, facilityId);
                if (result != null && result.Count > 0)
                {
                    list.AddRange(result.Select(item => new BedOccupancyCustomModel
                    {
                        IsOccupied = item.IsOccupied,
                        BedStatus = item.BedStatus,
                        Bed = item.Bed,
                        Room = item.Room,
                        Department = item.Department,
                        Floor = item.Floor,
                        SortOrder = Convert.ToInt32(item.SortOrder),
                    }));

                    list = list.OrderBy(t => t.SortOrder).ToList();
                }
            }
            return list;
        }


        /// <summary>
        /// Gets the encounter type data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public List<BedOccupancyCustomModel> GetEncounterTypeData(int corporateId, int facilityId, string displayType,
            DateTime fromDate, DateTime tillDate)
        {
            var list = new List<BedOccupancyCustomModel>();
            using (var rep = UnitOfWork.BedMasterRepository)
            {
                var result = rep.GetEncounterTypeData(corporateId, facilityId, displayType, fromDate, tillDate);
                if (result != null && result.Count > 0)
                {
                    list.AddRange(result.Select(item => new BedOccupancyCustomModel
                    {
                        //IsOccupied = item.IsOccupied,
                        //BedStatus = item.BedStatus,
                        //Beds = item.Beds,               //X Axis
                        //TotalBeds = item.TotalBeds      //Y Axis

                        TypeName = item.TypeName,
                        Budget = item.Budget, //X Axis
                        Current = item.Current, //Y Axis
                        Previous = item.Previous //Y Axis
                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the registration productivity data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public IEnumerable<RegistrationProductivity> GetRegistrationProductivityData(int facilityId, int displayType,
            DateTime? tillDate)
        {
            using (var rep = UnitOfWork.UsersRepository)
            {
                var result = rep.GetRegistrationProductivityData(facilityId, displayType, tillDate);
                //if (result.Any())
                //{
                //    result = result.Select(d =>
                //        {
                //            d.CreatedByUser = GetUserNameById(d.CreatedBy);
                //            return d;
                //        }).ToList();
                //}
                return result;
            }
        }


        /// <summary>
        /// Gets the high charts registration productivity data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="graphtype">The graphtype.</param>
        /// <returns></returns>
        public List<DashboardChargesCustomModel> GetHighChartsRegistrationProductivityData(int facilityId, int corporateId, string displayType, string fiscalyear,string graphtype)
        {
            using (var rep = UnitOfWork.UsersRepository)
            {
                var result = rep.GetHighChartsRegistrationProductivityData(facilityId, corporateId, displayType, fiscalyear, graphtype);
                return result;
            }
        }


        public IEnumerable<PatientBillingTrend> GetInPatientBillingTrendData(int facilityId, DateTime? tillDate)
        {
            using (var rep = UnitOfWork.UsersRepository)
            {
                var result = new List<PatientBillingTrend>();
                result.Add(new PatientBillingTrend
                {
                    Billed = 201,
                    Target = 350,
                    PatientName = "Mr. Amit Jain",
                    DisplayTypeId = 1,
                    Volume = 590,
                    Month = "Jan"
                });
                result.Add(new PatientBillingTrend
                {
                    Billed = 231,
                    Target = 300,
                    PatientName = "Mr. Bobby Buttar",
                    DisplayTypeId = 1,
                    Volume = 500,
                    Month = "Feb"
                });
                result.Add(new PatientBillingTrend
                {
                    Billed = 361,
                    Target = 450,
                    PatientName = "Mr. Michael Scofield",
                    DisplayTypeId = 1,
                    Volume = 700,
                    Month = "Mar"
                });
                result.Add(new PatientBillingTrend
                {
                    Billed = 287,
                    Target = 550,
                    PatientName = "Mr. Kinley Brown",
                    DisplayTypeId = 1,
                    Volume = 590,
                    Month = "April"
                });
                result.Add(new PatientBillingTrend
                {
                    Billed = 261,
                    Target = 350,
                    PatientName = "Mr. Joe Black",
                    DisplayTypeId = 1,
                    Volume = 590,
                    Month = "May"
                });
                return result;
            }
        }


        public List<PatientBillingTrend> GetOutPatientVisits(string displayType)
        {
            var list = new List<PatientBillingTrend>();
            using (var rep = UnitOfWork.BedMasterRepository)
            {
                //var result = rep.GetEncounterTypeData(corporateId, facilityId, displayType, fromDate, tillDate);
                if (displayType == "0")
                {
                    list.Add(new PatientBillingTrend
                    {
                        Volume = 700,
                        Billed = 570,
                        PatientName = "Mr. Decosta",
                        Target = 700
                    });
                }
                else
                {
                    list.Add(new PatientBillingTrend
                    {
                        Volume = 550,
                        Billed = 410,
                        PatientName = "Mr. Decosta",
                        Target = 550
                    });
                    //list.AddRange(result.Select(item => new PatientBillingTrend
                    //                {

                    //                    TypeName = item.TypeName,
                    //                    Budget = item.Budget,               //X Axis
                    //                    Current = item.Current,             //Y Axis
                    //                    Previous = item.Previous            //Y Axis
                    //                }));
                }
                return list;
            }
        }


        public List<PatientBillingTrend> GetInPatientDischarges(string displayType)
        {
            var list = new List<PatientBillingTrend>();
            using (var rep = UnitOfWork.BedMasterRepository)
            {
                //var result = rep.GetEncounterTypeData(corporateId, facilityId, displayType, fromDate, tillDate);
                if (displayType == "0")
                {
                    list.Add(new PatientBillingTrend
                    {
                        Volume = 700,
                        Billed = 570,
                        PatientName = "Mr. Phyc 1",
                        Target = 700
                    });
                }
                else
                {
                    list.Add(new PatientBillingTrend
                    {
                        Volume = 550,
                        Billed = 410,
                        PatientName = "Mr. Physician 2",
                        Target = 550
                    });
                }
                return list;
            }
        }

        #region Bill Scrubber

        public List<BillScrubberTrend> GetDenialsCodedByPhysicians(int facilityId)
        {
            var list = new List<BillScrubberTrend>
            {
                new BillScrubberTrend
                {
                    Physician = "Dr. Farook",
                    DenialsCoded = 87
                },
                new BillScrubberTrend
                {
                    Physician = "Dr. Abraj",
                    DenialsCoded = 124
                },
                new BillScrubberTrend
                {
                    Physician = "Dr. Jones",
                    DenialsCoded = 230
                },
                new BillScrubberTrend
                {
                    Physician = "Dr. Smith",
                    DenialsCoded = 345
                },
                new BillScrubberTrend
                {
                    Physician = "Dr. Perry",
                    DenialsCoded = 455
                }
            };

            return list;
        }

        #endregion

        public List<ClaimDenialPercentage> GetClaimDenialPercent()
        {
            using (var rep = UnitOfWork.DenialRepository)
            {
                var denialdetails = rep.GetDenialCodesReport();
                return denialdetails;
            }
        }

        /// <summary>
        /// Gets the high charts billing trend data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public List<DashboardChargesCustomModel> GetHighChartsBillingTrendData(int facilityId, int corporateId, string displayType, string fiscalyear)
        {
            using (var rep = UnitOfWork.UsersRepository)
            {
                var result = rep.GetHighChartsBillingTrendData(facilityId, corporateId, displayType, fiscalyear);
                return result;
            }
        }
    }
}
