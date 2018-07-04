using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<UBedMaster> _bdRepository;
        private readonly IRepository<Users> _uRepository;
        private readonly BillingEntities _context;

        public DashboardService(IRepository<UBedMaster> bdRepository, IRepository<Users> uRepository, BillingEntities context)
        {
            _bdRepository = bdRepository;
            _uRepository = uRepository;
            _context = context;
        }

        /// <summary>
        /// Gets the dashboard chart data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BedOccupancyCustomModel> GetDashboardChartData(int corporateId, int facilityId)
        {
            var list = new List<BedOccupancyCustomModel>();
            var spName = string.Format("EXEC {0} @pCorporateID,@pFacilityID", StoredProcedures.SPROC_GetDBBedOccupancyRate);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            IEnumerable<BedOccupancyCustomModel> result = _context.Database.SqlQuery<BedOccupancyCustomModel>(spName, sqlParameters);

            if (result != null && result.ToList().Count > 0)
            {
                list.AddRange(result.Select(item => new BedOccupancyCustomModel
                {
                    IsOccupied = item.IsOccupied,
                    BedStatus = item.BedStatus,
                    Beds = item.Beds, //X Axis
                    TotalBeds = item.TotalBeds, //Y Axis
                }));

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

            var spName = string.Format("EXEC {0} @pCorporateID,@pFacilityID", StoredProcedures.SPROC_GetDBBedOccupancybyFloor);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            var result = _context.Database.SqlQuery<BedOccupancyCustomModel>(spName, sqlParameters);

            if (result != null && result.ToList().Count > 0)
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
            var spName = string.Format("EXEC {0} @pCorporateID,@pFacilityID,@pAsOnDate,@pViewType", StoredProcedures.SPROC_GetDBEncounterType);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pAsOnDate", tillDate);
            sqlParameters[3] = new SqlParameter("pViewType", displayType);
            IEnumerable<BedOccupancyCustomModel> result = _context.Database.SqlQuery<BedOccupancyCustomModel>(spName, sqlParameters);

            if (result != null && result.ToList().Count > 0)
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
            var spName = string.Format("EXEC {0} @pFacilityID, @pDisplayTypeID, @pTillDate", StoredProcedures.SPROC_GetDBRegistrationProductivity);
            var sqlParameters = new object[3];
            sqlParameters[0] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[1] = new SqlParameter("pDisplayTypeID", displayType);
            sqlParameters[2] = new SqlParameter("pTillDate", tillDate);
            IEnumerable<RegistrationProductivity> result = _context.Database.SqlQuery<RegistrationProductivity>(spName, sqlParameters);
            return result.ToList();

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
        public List<DashboardChargesCustomModel> GetHighChartsRegistrationProductivityData(int facilityId, int corporateId, string displayType, string fiscalyear, string graphtype)
        {
            var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pType, @pFiscalYear, @pGraphType", StoredProcedures.SPROC_GetCounterRegistrationProductivity);
            var sqlParameters = new object[5];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pType", displayType);
            sqlParameters[3] = new SqlParameter("pFiscalYear", fiscalyear);
            sqlParameters[4] = new SqlParameter("pGraphType", graphtype);
            IEnumerable<DashboardChargesCustomModel> result = _context.Database.SqlQuery<DashboardChargesCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


        public IEnumerable<PatientBillingTrend> GetInPatientBillingTrendData(int facilityId, DateTime? tillDate)
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


        public List<PatientBillingTrend> GetOutPatientVisits(string displayType)
        {
            var list = new List<PatientBillingTrend>();
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

            }
            return list;

        }


        public List<PatientBillingTrend> GetInPatientDischarges(string displayType)
        {
            var list = new List<PatientBillingTrend>();
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
            var spName = string.Format("EXEC {0} ", StoredProcedures.SPROC_GetClaimDenialPercent.ToString());
            var sqlParameters = new SqlParameter[0];
            IEnumerable<ClaimDenialPercentage> result = _context.Database.SqlQuery<ClaimDenialPercentage>(spName, sqlParameters);
            return result.ToList();
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
            var spName = string.Format("EXEC {0} @CID, @FID, @BudgetFor, @pFiscalYear", StoredProcedures.SPROC_GetBillingTrendData);
            var sqlParameters = new object[4];
            sqlParameters[0] = new SqlParameter("CID", corporateId);
            sqlParameters[1] = new SqlParameter("FID", facilityId);
            sqlParameters[2] = new SqlParameter("BudgetFor", displayType);
            sqlParameters[3] = new SqlParameter("pFiscalYear", fiscalyear);
            IEnumerable<DashboardChargesCustomModel> result = _context.Database.SqlQuery<DashboardChargesCustomModel>(spName, sqlParameters);
            return result.ToList();
        }
    }
}
